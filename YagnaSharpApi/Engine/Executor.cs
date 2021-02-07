using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using YagnaSharpApi.Engine.Commands;
using YagnaSharpApi.Engine.MarketStrategy;
using YagnaSharpApi.Entities;
using YagnaSharpApi.Entities.Events;
using YagnaSharpApi.Exceptions;
using YagnaSharpApi.Repository;
using YagnaSharpApi.Storage;
using YagnaSharpApi.Utils;

namespace YagnaSharpApi.Engine
{
    public class Executor : IDisposable
    {
        private bool disposedValue;
        private CancellationTokenSource cancellationTokenSource;
                
        public IPackage Package { get; set; }
        public IMarketStrategy MarketStrategy { get; set; }
        public StorageProvider StorageProvider { get; set; }
        public AgreementPool AgreementPool { get; set; }
        public IMarketRepository MarketRepository { get; set; }
        public IActivityRepository ActivityRepository { get; set; }
        public IPaymentRepository PaymentRepository { get; set; }


        protected IDictionary<string, InvoiceEntity> InvoicesByAgreementId = new ConcurrentDictionary<string, InvoiceEntity>();
        protected List<string> AgreementsToPay = new List<string>();
        protected ConcurrentBag<AllocationEntity> Allocations = new ConcurrentBag<AllocationEntity>();
        protected List<Task> Workers = new List<Task>();

        public ApiConfiguration Configuration { get; set; }

        public decimal Budget { get; set; }
        public DateTime Expires { get; set; }
        public string SubnetTag { get; set; }

        private int workerId = 0;
        private int maxWorkers = 0;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="package"></param>
        /// <param name="maxWorkers"></param>
        /// <param name="budget"></param>
        /// <param name="timeout">in seconds</param>
        /// <param name="subnetTag"></param>
        /// <param name="marketStrategy"></param>
        public Executor(IPackage package, int maxWorkers, decimal budget, int timeout, string subnetTag, ApiConfiguration config = default, IMarketStrategy marketStrategy = null)
        {
            var apiConfig = config ?? new ApiConfiguration();
            var apiFactory = new ApiFactory(apiConfig);
            var mapper = Mapper.MapConfig.Config.CreateMapper();

            this.Package = package;

            this.MarketRepository = new MarketRepository(apiFactory.GetMarketRequestorApi(), mapper);
            this.ActivityRepository = new ActivityRepository(apiFactory.GetActivityRequestorControlApi(), mapper);
            this.PaymentRepository = new PaymentRepository(apiFactory.GetPaymentRequestorApi(), mapper);

            this.MarketStrategy = marketStrategy ?? new DummyMarketStrategy(this.MarketRepository, new MarketStrategyConditions() );

            this.StorageProvider = new GftpProvider();
            this.AgreementPool = new AgreementPool();

            this.Budget = budget;
            this.Expires = DateTime.UtcNow.AddSeconds(timeout);

            this.cancellationTokenSource = new CancellationTokenSource();

            this.maxWorkers = maxWorkers;

            // TODO setup event handlers for all the above
        }

        public async IAsyncEnumerable<GolemTask<TData, TResult>> Submit<TData, TResult>(Func<WorkContext, IAsyncEnumerable<GolemTask<TData, TResult>>, IAsyncEnumerable<WorkItem>> worker, IEnumerable<GolemTask<TData, TResult>> data)
        {

            // 1. Create Allocations
            var allocations = await this.CreateAllocationsAsync();

            if (!allocations.Any())
                throw new Exception("No payment accounts. Did you forget to run 'yagna payment init --sender'?");

            foreach (var alloc in allocations)
            {
                this.Allocations.Add(alloc);
            }

            // 1.1. Update market strategy settings with accepted payment platforms
            this.MarketStrategy.Conditions.PaymentPlatforms = allocations.Select(alloc => alloc.PaymentPlatform).ToList();

            // 2. Build Demand

            DemandBuilder demand = new DemandBuilder();

            demand.Add(Properties.SRV_COMP_EXPIRATION, this.Expires);
            demand.Add(Properties.SRV_COMP_CAPS_MULTI_ACTIVITY, true);
            // make sure to include subnet
            if (this.SubnetTag != null)
            {
                demand.Add(Properties.NODE_DEBUG_SUBNET, this.SubnetTag);
                demand.Ensure($"({Properties.NODE_DEBUG_SUBNET}={this.SubnetTag})");
            }

            this.Package.DecorateDemand(demand);
            await this.PaymentRepository.DecorateDemandAsync(allocations, demand);
            // note MarketStrategy does its own decorations on top of the above

            // 3. Create GFTP storage provider
            this.StorageProvider = new GftpProvider();

            // 4. Start FindOffers thread (with cancellation token!)
            var findOffersTask = Task.Run(async () => FindOffersAsync(demand, this.cancellationTokenSource.Token));

            // 5. Start ProcessInvoices thread (with cancellation token!)
            var processInvoicesTask = Task.Run(async () => ProcessInvoicesAsync(this.cancellationTokenSource.Token));

            // 6. Start the Worker Starter thread (with cancellation token!)
            var workerStarterTask = Task.Run(async () => WorkerStarter(worker, data, this.cancellationTokenSource.Token));

            try
            {
                yield return null;

            }
            finally
            {

            }

            throw new NotImplementedException();

        }

        protected async Task<IEnumerable<AllocationEntity>> CreateAllocationsAsync()
        {
            var result = new List<AllocationEntity>();

            var accounts = await this.PaymentRepository.GetAccountsAsync();

            foreach(var account in accounts)
            {
                var allocation = await this.PaymentRepository.CreateAllocationAsync(
                    account.Address,
                    account.Platform,
                    this.Budget, 
                    this.Expires.AddSeconds(this.Configuration.InvoiceTimeout));

                result.Add(allocation);
            }

            return result;
        }

        /// <summary>
        /// Logic to subscribe the Demand on the market and process incoming Offer proposals
        /// NOTE: this also includes "negotiation" of the payment platform.
        /// </summary>
        /// <param name="repo"></param>
        /// <param name="demand"></param>
        /// <returns></returns>
        protected async Task FindOffersAsync(DemandBuilder demand, CancellationToken cancellationToken /* TODO pass objects to collect stats */)
        {
            try
            {
                await foreach(var prop in this.MarketStrategy.FindOffersAsync(demand, cancellationToken))
                {
                    // Add to AgreementPool
                    this.AgreementPool.AddProposal(prop.Item1, prop.Item2);
                }
            }
            catch(Exception exc)
            {

            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected async Task WorkerStarter<TData, TResult>(Func<WorkContext, IAsyncEnumerable<GolemTask<TData, TResult>>, IAsyncEnumerable<WorkItem>> worker, IEnumerable<GolemTask<TData, TResult>> data, CancellationToken cancellationToken)
        {
            while(!cancellationToken.IsCancellationRequested)
            {
                if(this.Workers.Count < this.maxWorkers /* && TODO add logic to detect if there is still owrk to be done*/ )
                {
                    Task newTask = null;

                    try
                    {
                        newTask = await this.AgreementPool.UseAgreementAsync(bufferedAgreement =>
                            this.StartWorker(bufferedAgreement, worker, data)
                            );
                        this.Workers.Add(newTask);
                    }
                    catch(Exception exc)
                    {
                        // TODO should we do something to abort the new task?
                    }
                }
            }
        }

        protected async Task StartWorker<TData, TResult>(BufferedAgreement bufferedAgreement, Func<WorkContext, IAsyncEnumerable<GolemTask<TData, TResult>>, IAsyncEnumerable<WorkItem>> worker, IEnumerable<GolemTask<TData, TResult>> data)
        {
            // TODO raise WorkerStarted event

            ActivityEntity activity = null;
            try
            {
                activity = await this.ActivityRepository.CreateActivityAsync(bufferedAgreement.Agreement);
            }
            catch(Exception exc)
            {
                // TODO raise ActivityCreateFailed event
                // TODO raise WorkerFinished event
                throw;
            }

            // TODO raise ActivityCreated event

            var workContext = new WorkContext($"worker-{this.workerId++}", this.StorageProvider /* , TODO add NodeInfo here */);

            var commandGenerator = worker(workContext, AsyncEnumerable.ToAsyncEnumerable(data));

            await foreach(var batch in commandGenerator)
            {
                // TODO batch deadline logic

                try
                {
                    // TODO in yapapi there is here a weird logic that attempts to correlate 
                    // the batch of commands created by the worker() delegate with its input GolemTask

                    // TODO raise TaskStarted event

                    await batch.Prepare();

                    var commandsBuilder = new ExeScriptBuilder();
                    batch.Register(commandsBuilder);

                    var commands = commandsBuilder.GetCommands();

                    // ...at this point we should have the exescript built by commandBuilder
                    var commandResults = activity.ExecAsync(commands);

                    // TODO raise ScriptSent event

                    await foreach (var result in commandResults)
                    {
                        // TODO raise command executed event
                        if (result.Result == Golem.ActivityApi.Client.Model.ExeScriptCommandResult.ResultEnum.Error)
                            throw new CommandExecutionException(commands[result.Index], result); // hmmm, will this not be immediately caught below???
                    }

                    // TODO raise GettingResults event
                    await batch.Post();
                    // TODO raise ScriptFinished event
                    await this.AcceptPaymentForAgreement(bufferedAgreement.Agreement.AgreementId, true);
                }
                catch(Exception exc)
                {
                    // TODO raise WorkerFinished event
                    return;
                }
            }

            await this.AcceptPaymentForAgreement(bufferedAgreement.Agreement.AgreementId);
            // TODO raise WorkerFinished event
        }

        protected async Task AcceptPaymentForAgreement(string agreementId, bool partial = false)
        {
            // TODO raise PaymentPrepared event
            if(!this.InvoicesByAgreementId.ContainsKey(agreementId))
            {
                this.AgreementsToPay.Add(agreementId);
                // TODO raise PaymentQueued event
                return;
            }
            InvoiceEntity invoice = this.InvoicesByAgreementId[agreementId];
            this.InvoicesByAgreementId.Remove(agreementId);

            var allocation = this.GetAllocationForInvoice(invoice);
            await invoice.AcceptAsync(invoice.Amount, allocation);
            // TODO raise PaymentAccepted event 
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

        protected async Task ProcessInvoicesAsync(CancellationToken cancellationToken /* pass objects to collect stats */)
        {
            await foreach(var invoiceEvent in this.PaymentRepository.GetInvoiceEventsAsync(cancellationToken))
            {
                if(this.AgreementsToPay.Contains(invoiceEvent.Invoice?.AgreementId))
                {
                    // TODO raise InvoiceReceived event
                    var allocation = this.GetAllocationForInvoice(invoiceEvent.Invoice);

                    try
                    {
                        await invoiceEvent.Invoice?.AcceptAsync(invoiceEvent.Invoice.Amount, allocation);

                        this.AgreementsToPay.Remove(invoiceEvent.Invoice?.AgreementId);
                        // TODO raise PaymentAccepted event
                    }
                    catch (Exception exc)
                    {
                        // TODO raise PaymentFailed event
                    }
                }
                else
                {
                    this.InvoicesByAgreementId[invoiceEvent.Invoice.AgreementId] = invoiceEvent.Invoice;
                }
            }
        }

        protected async Task ProcessDebitNotesAsync(CancellationToken cancellationToken /* pass objects to collect stats */)
        {
            // TODO
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
