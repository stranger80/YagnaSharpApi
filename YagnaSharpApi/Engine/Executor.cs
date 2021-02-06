using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using YagnaSharpApi.Engine.Commands;
using YagnaSharpApi.Engine.MarketStrategy;
using YagnaSharpApi.Entities;
using YagnaSharpApi.Entities.Events;
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
        public IPaymentRepository PaymentRepository { get; set; }

        public ApiConfiguration Configuration { get; set; }

        public decimal Budget { get; set; }
        public DateTime Expires { get; set; }
        public string SubnetTag { get; set; }

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
            this.PaymentRepository = new PaymentRepository(apiFactory.GetPaymentRequestorApi(), mapper);

            this.MarketStrategy = marketStrategy ?? new DummyMarketStrategy(this.MarketRepository, new MarketStrategyConditions() );

            this.StorageProvider = new GftpProvider();
            this.AgreementPool = new AgreementPool();

            this.Budget = budget;
            this.Expires = DateTime.UtcNow.AddSeconds(timeout);

            this.cancellationTokenSource = new CancellationTokenSource();

            // TODO setup event handlers for all the above
        }

        public async IAsyncEnumerable<GolemTask<TData, TResult>> Submit<TData, TResult>(Func<WorkContext, IAsyncEnumerable<GolemTask<TData, TResult>>, IAsyncEnumerable<WorkItem>> worker, IEnumerable<GolemTask<TData, TResult>> data)
        {

            // 1. Create Allocations
            var allocations = await this.CreateAllocationsAsync();

            if (!allocations.Any())
                throw new Exception("No payment accounts. Did you forget to run 'yagna payment init --sender'?");

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
            Task.Run(async () => FindOffersAsync(demand, this.cancellationTokenSource.Token));

            // 5. Start ProcessInvoices thread (with cancellation token!)
            Task.Run(async () => ProcessInvoicesAsync(this.cancellationTokenSource.Token));

            // 6. 

            yield return null;

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

        protected async Task ProcessInvoicesAsync(CancellationToken cancellationToken /* pass objects to collect stats */)
        {
            await foreach(var invoice in this.PaymentRepository.GetInvoiceEventsAsync(cancellationToken))
            {
                // TODO write invoice accept logic
            }
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
