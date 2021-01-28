using System;
using System.Collections.Generic;
using System.Text;
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
        public IMarketStrategy MarketStrategy { get; set; }
        public StorageProvider StorageProvider { get; set; }
        public AgreementPool AgreementPool { get; set; }
        public IMarketRepository MarketRepository { get; set; }
        public IPaymentRepository PaymentRepository { get; set; }

        public Executor(IPackage package, int maxWorkers, decimal budget, int timeout, string subnetTag, IMarketStrategy marketStrategy = null)
        {
            var apiConfig = new ApiConfiguration();
            var apiFactory = new ApiFactory(apiConfig);
            var mapper = Mapper.MapConfig.Config.CreateMapper();

            this.MarketRepository = new MarketRepository(apiFactory.GetMarketRequestorApi(), mapper);
            this.PaymentRepository = new PaymentRepository(apiFactory.GetPaymentRequestorApi(), mapper);

            this.MarketStrategy = marketStrategy ?? new DummyMarketStrategy(this.MarketRepository);
            this.StorageProvider = new GftpProvider();
            this.AgreementPool = new AgreementPool();

            // TODO setup event handlers for all the above
        }

        public IAsyncEnumerable<GolemTask<TData, TResult>> Submit<TData, TResult>(Func<WorkContext, IAsyncEnumerable<GolemTask<TData, TResult>>, IAsyncEnumerable<WorkItem>> worker, IEnumerable<GolemTask<TData, TResult>> data)
        {
            throw new NotImplementedException();

            // 1. Create Allocations

            // 2. Build Demand
            //    - make sure to include subnet

            // 3. Create GFTP storage provider

            // 4. Start FindOffers thread

            // 5. Start ProcessInvoices thread

            // 6. 


        }

        protected async Task CreateAllocationsAsync()
        {

        }

        /// <summary>
        /// Logic to subscribe the Demand on the market and process incoming Offer proposals
        /// NOTE: this also includes "negotiation" of the payment platform.
        /// </summary>
        /// <param name="repo"></param>
        /// <param name="demand"></param>
        /// <returns></returns>
        protected async Task FindOffersAsync(DemandBuilder demand /* TODO pass objects to collect stats */)
        {
            try
            {
                await foreach(var prop in this.MarketStrategy.FindOffersAsync(demand))
                {
                    // Add to AgreementPool
                    this.AgreementPool.AddProposal(prop.Item1, prop.Item2);
                }
            }
            catch(Exception exc)
            {

            }
        }

        protected async Task ProcessInvoicesAsync(PaymentRepository repo /* pass objects to collect stats */)
        {
            await foreach(var invoice in repo.GetInvoiceEventsAsync())
            {

            }
        }

        #region Dispose

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    this.MarketRepository.Dispose();
                    this.StorageProvider.Dispose();
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
