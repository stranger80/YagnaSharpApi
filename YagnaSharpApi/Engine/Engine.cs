using Golem.ActivityApi.Client.Model;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using YagnaSharpApi.Engine.Commands;
using YagnaSharpApi.Engine.Events;
using YagnaSharpApi.Engine.MarketStrategy;
using YagnaSharpApi.Entities;
using YagnaSharpApi.Entities.Events;
using YagnaSharpApi.Exceptions;
using YagnaSharpApi.Mapper;
using YagnaSharpApi.Repository;
using YagnaSharpApi.Storage;
using YagnaSharpApi.Utils;

namespace YagnaSharpApi.Engine
{
    public class Engine : IDisposable
    {
        private bool disposedValue;
        protected CancellationTokenSource cancellationTokenSource;
                
        public IMarketStrategy MarketStrategy { get; set; }
        public StorageProvider StorageProvider { get; set; }
        public AgreementPool AgreementPool { get; set; }
        public IMarketRepository MarketRepository { get; set; }
        public IActivityRepository ActivityRepository { get; set; }
        public IPaymentRepository PaymentRepository { get; set; }


        protected IDictionary<string, InvoiceEntity> InvoicesByAgreementId = new ConcurrentDictionary<string, InvoiceEntity>();
        protected HashSet<string> AgreementsToPay = new HashSet<string>();
        protected HashSet<string> AgreementsAcceptingDebitNotes = new HashSet<string>();
        protected ConcurrentBag<AllocationEntity> Allocations = new ConcurrentBag<AllocationEntity>();
        protected List<Task> Workers = new List<Task>();

        public ApiConfiguration Configuration { get; set; }

        public decimal Budget { get; set; }
        public string SubnetTag { get; set; }

        public event EventHandler<Event> OnExecutorEvent;

        private int workerId = 0;


        static Engine()
        {
            MapConfig.Init();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="package"></param>
        /// <param name="maxWorkers"></param>
        /// <param name="budget"></param>
        /// <param name="timeout">in seconds</param>
        /// <param name="subnetTag"></param>
        /// <param name="marketStrategy"></param>
        public Engine(decimal budget, string subnetTag, ApiConfiguration config = default, IMarketStrategy marketStrategy = null)
        {
            this.Configuration = config ?? new ApiConfiguration();
            var apiFactory = new ApiFactory(this.Configuration);
            var mapper = Mapper.MapConfig.Config.CreateMapper();

            this.MarketRepository = new MarketRepository(apiFactory.GetMarketRequestorApi(), mapper);
            this.ActivityRepository = new ActivityRepository(apiFactory.GetActivityRequestorControlApi(), mapper);
            this.PaymentRepository = new PaymentRepository(apiFactory.GetPaymentRequestorApi(), mapper);

            this.MarketStrategy = marketStrategy ?? new DummyMarketStrategy(this.MarketRepository, new MarketStrategyConditions() );

            this.StorageProvider = new GftpProvider();
            this.AgreementPool = new AgreementPool();

            this.Budget = budget;

            this.cancellationTokenSource = new CancellationTokenSource();

            this.SubnetTag = subnetTag;

            // TODO setup event handlers for all the above
            this.MarketStrategy.OnMarketEvent += MarketStrategy_OnMarketEvent;
            this.AgreementPool.OnAgreementEvent += AgreementPool_OnAgreementEvent; 
        }

        private void AgreementPool_OnAgreementEvent(object sender, AgreementEvent e)
        {
            this.OnExecutorEvent?.Invoke(this, e);
        }

        private void MarketStrategy_OnMarketEvent(object sender, Event e)
        {
            this.OnExecutorEvent?.Invoke(this, e);
        }

        protected void ForwardExecutorEvent(object sender, Event e)
        {
            this.OnExecutorEvent?.Invoke(sender, e);
        }

        protected async Task<DemandBuilder> CreateDemandBuilderAsync(DateTime expirationTime, IPackage payload)
        {
            DemandBuilder demand = new DemandBuilder();

            demand.Add(Properties.SRV_COMP_EXPIRATION, expirationTime);
            demand.Add(Properties.SRV_COMP_CAPS_MULTI_ACTIVITY, true);
            // make sure to include subnet
            if (this.SubnetTag != null)
            {
                demand.Add(Properties.NODE_DEBUG_SUBNET, this.SubnetTag);
                demand.Ensure($"({Properties.NODE_DEBUG_SUBNET}={this.SubnetTag})");
            }

            await payload.DecorateDemandAsync(demand);
            await this.PaymentRepository.DecorateDemandAsync(this.Allocations, demand);
            
            return demand;
        }

        /// <summary>
        /// Logic to subscribe the Demand on the market and process incoming Offer proposals
        /// NOTE: this also includes "negotiation" of the payment platform.
        /// </summary>
        /// <param name="repo"></param>
        /// <param name="demand"></param>
        /// <returns></returns>
        public async Task FindOffersAsync(DemandBuilder demand, CancellationToken cancellationToken /* TODO pass objects to collect stats */)
        {
            int iterCount = 0;

            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    System.Diagnostics.Debug.WriteLine($"Starting {iterCount++} Demand Subscription...");

                    await foreach (var prop in this.MarketStrategy.FindOffersAsync(demand, cancellationToken))
                    {
                        System.Diagnostics.Debug.WriteLine("Adding Proposal to AgreementPool...");
                        // Add to AgreementPool
                        this.AgreementPool.AddProposal(prop.Item1, prop.Item2);
                        System.Diagnostics.Debug.WriteLine("Added Proposal to AgreementPool...");
                    }
                }
                catch (Exception exc)
                {
                    // TODO how to handle this???
                }
            }
        }

        public async Task<IEnumerable<AllocationEntity>> CreateAllocationsAsync()
        {
            var result = new List<AllocationEntity>();

            var accounts = await this.PaymentRepository.GetAccountsAsync();

            foreach(var account in accounts)
            {
                try
                {
                    var allocation = await this.PaymentRepository.CreateAllocationAsync(
                        account.Address,
                        account.Platform,
                        this.Budget
                        // TODO should we assume some default expiry time for allocation?
                        // this.Expires.AddSeconds(this.Configuration.InvoiceTimeout));
                        );

                    result.Add(allocation);
                    this.OnExecutorEvent?.Invoke(this, new AllocationCreated(allocation.AllocationId));
                }
                catch (Exception exc)
                {
                    this.OnExecutorEvent?.Invoke(this, new AllocationFailed(exc));
                }
            }

            return result;
        }

        protected AllocationEntity GetAllocationForInvoice(InvoiceEntity invoice)
        {
            var result = this.Allocations
                .Where(alloc => alloc.Address == invoice.PayerAddr && alloc.PaymentPlatform == invoice.PaymentPlatform)
                .FirstOrDefault();

            if (result == null)
                throw new Exception($"No allocation for {invoice.PayerAddr} {invoice.PaymentPlatform}");

            return result;
        }

        public async Task<ActivityEntity> CreateActivityAsync(AgreementEntity agreement)
        {
            return await this.ActivityRepository.CreateActivityAsync(agreement);
        }

        public async Task ProcessBatchesAsync(AgreementEntity agreement, ActivityEntity activity, IAsyncEnumerable<Script> commandGenerator, WorkContext ctx, string taskId)
        {

            await using (var iter = commandGenerator.GetAsyncEnumerator())
            {
                var isItem = await iter.MoveNextAsync();

                while (isItem)
                {
                    var batch = iter.Current;

                    await batch.BeforeAsync();

                    if(!activity.IsActivityInitialized && !batch.IsInitialized)
                    {
                        // do explicit initialization
                        await this.PerformImplicitInitAsync(agreement, activity, ctx, taskId);
                    }

                    var commandsBuilder = new ExeScriptBuilder();
                    batch.Evaluate(commandsBuilder);

                    var commands = commandsBuilder.GetCommands();

                    // ...at this point we should have the exescript built by commandBuilder
                    var commandResults = activity.ExecAsync(commands);

                    this.OnExecutorEvent?.Invoke(this, new ScriptSent(agreement.AgreementId, taskId, commands));

                    await foreach (var result in commandResults)
                    {
                        batch.StoreResult(result);

                        // TODO raise command executed event
                        if (result.Result == ExeScriptCommandResult.ResultEnum.Error)
                            throw new CommandExecutionException(commands[result.Index], result);
                    }

                    this.OnExecutorEvent?.Invoke(this, new GettingResults(agreement.AgreementId, taskId));
                    await batch.AfterAsync();
                    this.OnExecutorEvent?.Invoke(this, new ScriptFinished(agreement.AgreementId, taskId, batch));

                    await this.AcceptPaymentForAgreement(agreement.AgreementId, true);

                    isItem = await iter.MoveNextAsync();
                }


            }
        }

        protected async Task PerformImplicitInitAsync(AgreementEntity agreement, ActivityEntity activity, WorkContext ctx, string taskId)
        {
            async IAsyncEnumerable<Script> ImplicitInitAsync()
            {
                var script = ctx.NewScript();
                script.Append(new DeployStep());
                script.Append(new StartStep());
                yield return script;
            }
            await this.ProcessBatchesAsync(agreement, activity, ImplicitInitAsync(), ctx, taskId);

        }

        public async Task ProcessInvoicesAsync(CancellationToken cancellationToken /* pass objects to collect stats */)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    await foreach (var invoiceEvent in this.PaymentRepository.GetInvoiceEventsAsync(cancellationToken))
                    {
                        if (this.AgreementsToPay.Contains(invoiceEvent.Invoice?.AgreementId))
                        {
                            this.OnExecutorEvent?.Invoke(this, new InvoiceReceived(
                                invoiceEvent.Invoice.AgreementId,
                                invoiceEvent.Invoice.InvoiceId,
                                invoiceEvent.Invoice.Amount));
                            var allocation = this.GetAllocationForInvoice(invoiceEvent.Invoice);

                            try
                            {
                                await invoiceEvent.Invoice?.AcceptAsync(invoiceEvent.Invoice.Amount, allocation);

                                this.AgreementsToPay.Remove(invoiceEvent.Invoice?.AgreementId);
                                this.OnExecutorEvent?.Invoke(this, new InvoiceAccepted(
                                    invoiceEvent.Invoice.AgreementId,
                                    invoiceEvent.Invoice.InvoiceId,
                                    invoiceEvent.Invoice.Amount));
                            }
                            catch (Exception exc)
                            {
                                this.OnExecutorEvent?.Invoke(this, new PaymentFailed(
                                    invoiceEvent.Invoice.AgreementId,
                                    exc));

                            }
                        }
                        else
                        {
                            this.InvoicesByAgreementId[invoiceEvent.Invoice.AgreementId] = invoiceEvent.Invoice;
                        }
                    }
                }
                catch (Exception exc)
                {
                    // TODO log the warning and start again
                }
            }
        }

        public async Task ProcessDebitNotesAsync(CancellationToken cancellationToken /* pass objects to collect stats */)
        {
            // TODO
        }

        public void AcceptDebitNotesForAgreement(string agreementId)
        {
            this.AgreementsAcceptingDebitNotes.Add(agreementId);
        }

        public async Task AcceptPaymentForAgreement(string agreementId, bool partial = false)
        {
            this.OnExecutorEvent?.Invoke(this, new PaymentPrepared(agreementId));

            if (!this.InvoicesByAgreementId.ContainsKey(agreementId))
            {
                this.AgreementsToPay.Add(agreementId);
                this.OnExecutorEvent?.Invoke(this, new PaymentQueued(agreementId));
                return;
            }
            InvoiceEntity invoice = this.InvoicesByAgreementId[agreementId];
            this.InvoicesByAgreementId.Remove(agreementId);

            var allocation = this.GetAllocationForInvoice(invoice);
            await invoice.AcceptAsync(invoice.Amount, allocation);
            this.OnExecutorEvent?.Invoke(this, new PaymentAccepted(agreementId, invoice.InvoiceId, invoice.Amount));
        }



        #region Dispose

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    this.cancellationTokenSource.Cancel();

                    this.MarketRepository.Dispose();
                    this.StorageProvider.Dispose();

                    this.cancellationTokenSource.Dispose();
                }
                disposedValue = true;
            }
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
