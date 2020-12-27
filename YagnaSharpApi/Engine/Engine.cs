using System;
using System.Collections.Generic;
using System.Text;
using YagnaSharpApi.Engine.Commands;
using YagnaSharpApi.Utils;

namespace YagnaSharpApi.Engine
{
    public class Engine : IDisposable
    {
        private bool disposedValue;

        public Engine(IPackage package, int maxWorkers, decimal budget, int timeout, string subnetTag)
        {

        }

        public IAsyncEnumerable<GolemTask<TData, TResult>> Map<TData, TResult>(Func<WorkContext, IAsyncEnumerable<GolemTask<TData, TResult>>, IAsyncEnumerable<WorkItem>> worker, IEnumerable<GolemTask<TData, TResult>> data)
        {
            throw new NotImplementedException();

            // 1. Create Allocation

            // 2. Build Demand

            // 3. Create GFTP storage provider

            // 4. Start FindOffers thread

            // 5. Start ProcessInvoices thread

            // 6. 


        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~Engine()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
