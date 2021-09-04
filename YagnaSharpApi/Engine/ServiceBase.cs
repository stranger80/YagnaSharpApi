using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using YagnaSharpApi.Engine.Commands;
using YagnaSharpApi.Entities;
using YagnaSharpApi.Utils;

namespace YagnaSharpApi.Engine
{
    public abstract class ServiceBase : IDisposable
    {
        private static int ids = 0;

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

        public enum ServiceControlSignal
        {
            Stop
        }

        public CancellationToken CancellationToken { get; set; }
        public CancellationTokenSource CancellationTokenSource { get; set; }

        protected AsyncQueue<ServiceControlSignal> ControlQueue { get; } = new AsyncQueue<ServiceControlSignal>();

        public ServiceBase()
        {
            this.State = ServiceStateEnum.New;

            this.CancellationTokenSource = new CancellationTokenSource();
            this.CancellationToken = this.CancellationTokenSource.Token;
        }

        public ServiceStateEnum State { get; set; }

        protected object stateLock = new object();

        public int Id { get; } = ++ids;
        public string ProviderName { get; set; }

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
        /// This method executes the service lifecycle
        /// </summary>
        /// <returns></returns>
        public async Task ExecuteLifecycleAsync(Engine engine, AgreementEntity agreement, ActivityEntity activity, WorkContext ctx, string taskId)
        {
            var processControlQueueTask = Task.Run(() => ProcessControlQueue());

            while (!this.IsTerminalState())
            {
                switch (this.State)
                {
                    case ServiceStateEnum.New:
                        this.MoveToState(ServiceStateEnum.Starting);
                        break;

                    case ServiceStateEnum.Starting:
                        try
                        {
                            await this.ExecuteLifecycleStepAsync(engine, agreement, activity, ctx, taskId, () => OnStartupAsync(ctx));
                            this.MoveToState(ServiceStateEnum.Running);
                        }
                        catch(Exception exc)
                        {
                            // on error move to Error state
                            this.MoveToState(ServiceStateEnum.Error);
                        }

                        break;

                    case ServiceStateEnum.Running:
                        try
                        {
                            await this.ExecuteLifecycleStepAsync(engine, agreement, activity, ctx, taskId, () => OnRunAsync(ctx, this.CancellationToken));
                            this.MoveToState(ServiceStateEnum.ShuttingDown);
                        }
                        catch (Exception exc)
                        {
                            // on error move to Error state
                            this.MoveToState(ServiceStateEnum.Error);
                        }

                        break;

                    case ServiceStateEnum.ShuttingDown:
                        try
                        {
                            await this.ExecuteLifecycleStepAsync(engine, agreement, activity, ctx, taskId, () => OnShutdownAsync(ctx));
                            this.MoveToState(ServiceStateEnum.Finished);
                        }
                        catch (Exception exc)
                        {
                            // on error move to Error state
                            this.MoveToState(ServiceStateEnum.Error);
                        }

                        break;
                }
            }
        }

        protected async Task ExecuteLifecycleStepAsync(Engine engine, AgreementEntity agreement, ActivityEntity activity, WorkContext ctx, string taskId, Func<IAsyncEnumerable<WorkItem>> commandGenerator)
        {
            await engine.ProcessBatchesAsync(agreement, activity, commandGenerator(), taskId);
            // TODO this should be a handled Cancelled exception - from cancellation token???
        }

        protected bool MoveToState(ServiceStateEnum newState)
        {
            lock(this.stateLock)
            {
                switch(newState)
                {
                    case ServiceStateEnum.Error: // any state may move to Error
                        this.State = newState;
                        return true;

                    case ServiceStateEnum.Starting: // only New can move to Starting
                        if(this.State == ServiceStateEnum.New)
                        {
                            this.State = newState;
                            return true;
                        }
                        return false;

                    case ServiceStateEnum.Running: // Only Starting can move to Running (for now)
                        if (this.State == ServiceStateEnum.Starting)
                        {
                            this.State = newState;
                            return true;
                        }
                        return false;

                    case ServiceStateEnum.ShuttingDown: // Only Running can move to ShuttingDown (for now)
                        if (this.State == ServiceStateEnum.Running)
                        {
                            this.State = newState;
                            return true;
                        }
                        return false;

                    case ServiceStateEnum.Finished: // Only ShuttingDown can move to Finished (for now)
                        if (this.State == ServiceStateEnum.ShuttingDown)
                        {
                            this.State = newState;
                            return true;
                        }
                        return false;

                    default:
                        return false;
                }
            }
        }

        protected bool IsTerminalState()
        {
            return this.State == ServiceStateEnum.Finished || this.State == ServiceStateEnum.Error;
        }

        /// <summary>
        /// Listen on the service control queue and react accordingly
        /// </summary>
        protected async Task ProcessControlQueue()
        {
            await foreach(var signal in this.ControlQueue)
            {
                switch(signal)
                {
                    case ServiceControlSignal.Stop:
                        // TODO implement service shutdown
                        // then exit = stop listening to control queue
                        return;
                }
            }
        }

        /// <summary>
        /// Request service shutdown
        /// </summary>
        public void Shutdown()
        {
            this.ControlQueue.Enqueue(ServiceControlSignal.Stop);
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
