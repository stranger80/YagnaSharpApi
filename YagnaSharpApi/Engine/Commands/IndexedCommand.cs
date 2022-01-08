using Golem.ActivityApi.Client.Model;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace YagnaSharpApi.Engine.Commands
{
    /// <summary>
    /// Awaitable workitem.
    /// 
    /// </summary>
    public abstract class IndexedCommand : Command
    {
        public int CommandIndex { get; protected set; }

        private TaskCompletionSource<ExeScriptCommandResult> resultTaskCompletionSource;

        private ExeScriptCommandResult Result;

        public IndexedCommand()
        {
            this.resultTaskCompletionSource = new TaskCompletionSource<ExeScriptCommandResult>(TaskCreationOptions.RunContinuationsAsynchronously);
        }

        public TaskAwaiter<ExeScriptCommandResult> GetAwaiter()
        {
            return this.resultTaskCompletionSource.Task.GetAwaiter();
        }

        public override void StoreResult(ExeScriptCommandResult result)
        {
            if(this.CommandIndex == result.Index)
            {
                this.Result = result;
                if (this.resultTaskCompletionSource != null)
                {
                    this.resultTaskCompletionSource.TrySetResult(result);
                }
            }
        }

        public override IEnumerable<ExeScriptCommandResult> GetResults()
        {
            yield return this.Result;
        }
    }
}
