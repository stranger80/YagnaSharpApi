using Golem.ActivityApi.Client.Model;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using YagnaSharpApi.Repository;

namespace YagnaSharpApi.Entities
{
    public class ActivityEntity : IDisposable
    {
        private bool disposedValue;

        protected IActivityRepository Repository { get; set; }


        public string ActivityId { get; set; }

        public ActivityEntity(IActivityRepository repo)
        {
            this.Repository = repo;
        }

        public async IAsyncEnumerable<ExeScriptCommandResult> ExecAsync(List<ExeScriptCommand> commands, [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            var batch = await this.Repository.ExecAsync(this, commands);

            await foreach (var result in this.Repository.GetBatchEventsAsync(batch, cancellationToken))
            {
                yield return result;
            };
        }

        protected async virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    try
                    {
                        await this.Repository.DestroyActivityAsync(this.ActivityId);
                    }
                    catch(Exception exc)
                    {
                        // TODO log warning
                    }
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
    }
}
