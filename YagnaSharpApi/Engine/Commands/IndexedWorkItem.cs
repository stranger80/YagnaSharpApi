using Golem.ActivityApi.Client.Model;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace YagnaSharpApi.Engine.Commands
{
    public abstract class IndexedWorkItem : WorkItem
    {
        public int CommandIndex { get; protected set; }

        private TaskCompletionSource<ExeScriptCommandResult> resultTaskCompletionSource;

        private ExeScriptCommandResult Result;


        public TaskAwaiter<ExeScriptCommandResult> GetAwaiter()
        {
            if (this.resultTaskCompletionSource == null)
            {
                this.resultTaskCompletionSource = new TaskCompletionSource<ExeScriptCommandResult>(TaskCreationOptions.RunContinuationsAsynchronously);
            }

            return this.resultTaskCompletionSource.Task.GetAwaiter();
        }

        public override void StoreResult(ExeScriptCommandResult result)
        {
            if(this.CommandIndex == result.Index)
            {
                this.Result = result;
                if (this.resultTaskCompletionSource != null)
                {
                    this.resultTaskCompletionSource.SetResult(result);
                }
            }
        }

        public override IEnumerable<ExeScriptCommandResult> GetResults()
        {
            yield return this.Result;
        }
    }
}
