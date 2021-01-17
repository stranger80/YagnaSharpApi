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

        public Executor(IPackage package, int maxWorkers, decimal budget, int timeout, string subnetTag, IMarketStrategy marketStrategy = null)
        {
            this.MarketStrategy = marketStrategy ?? new DummyMarketStrategy();
            this.StorageProvider = new GftpProvider();

        }

        public IAsyncEnumerable<GolemTask<TData, TResult>> Submit<TData, TResult>(Func<WorkContext, IAsyncEnumerable<GolemTask<TData, TResult>>, IAsyncEnumerable<WorkItem>> worker, IEnumerable<GolemTask<TData, TResult>> data)
        {
            throw new NotImplementedException();

            // 1. Create Allocation

            // 2. Build Demand
            //    - make sure to include subnet

            // 3. Create GFTP storage provider

            // 4. Start FindOffers thread

            // 5. Start ProcessInvoices thread

            // 6. 


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
                    // TODO add to AgreementPool
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
