using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using YagnaSharpApi.Engine.Commands;
using YagnaSharpApi.Utils;

namespace YagnaSharpApi.Engine
{
    public abstract class ServiceBase : IDisposable
    {
        private bool disposedValue;

        public enum ServiceStateEnum
        {
            New,
            Starting,
            Running,
            Unresponsive,
            ShuttingDown,
            Finished,
            Error
        }

        public CancellationToken CancellationToken { get; set; }
        public CancellationTokenSource CancellationTokenSource { get; set; }

        public ServiceBase()
        {
            this.CancellationTokenSource = new CancellationTokenSource();
            this.CancellationToken = this.CancellationTokenSource.Token;
        }

        public ServiceStateEnum State { get; set; }

        /// <summary>
        /// Returns the definition of service payload.
        /// </summary>
        /// <returns></returns>
        public abstract IPackage GetPayload();

        /// <summary>
        /// Specifies actions to be executed on Provider when the service is being launched.
        /// </summary>
        /// <param name="ctx"></param>
        /// <returns></returns>
        public abstract IAsyncEnumerable<WorkItem> OnStartupAsync(WorkContext ctx);
        
        /// <summary>
        /// Specifies actions which are to be executed on Provider while the service is running.
        /// </summary>
        /// <param name="ctx"></param>
        /// <returns></returns>
        public abstract IAsyncEnumerable<WorkItem> OnRunAsync(WorkContext ctx, CancellationToken cancellationToken);
        
        /// <summary>
        /// Specifies actions to be executed on Provider while the service is being shut down.
        /// </summary>
        /// <param name="ctx"></param>
        /// <returns></returns>
        public abstract IAsyncEnumerable<WorkItem> OnShutdownAsync(WorkContext ctx, Exception error = null);

        /// <summary>
        /// Request service shutdown
        /// </summary>
        public void Shutdown()
        {

        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    this.CancellationTokenSource.Dispose();
                }
                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~ServiceBase()
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
