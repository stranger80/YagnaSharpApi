using Golem.ActivityApi.Client.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace YagnaSharpApi.Engine.Commands
{
    public class BatchCommand : Command
    {
        protected IEnumerable<Command> steps;

        private TaskCompletionSource<ExeScriptCommandResult[]> resultTaskCompletionSource;

        private ExeScriptCommandResult[] Results;

        public BatchCommand(IEnumerable<Command> steps)
        {
            this.steps = steps;
            this.Results = new ExeScriptCommandResult[steps.Count()];

            if (this.resultTaskCompletionSource == null)
            {
                this.resultTaskCompletionSource = new TaskCompletionSource<ExeScriptCommandResult[]>(TaskCreationOptions.RunContinuationsAsynchronously);
            }
        }



        public TaskAwaiter<ExeScriptCommandResult[]> GetAwaiter()
        {
            return this.resultTaskCompletionSource.Task.GetAwaiter();
        }


        public async override Task BeforeAsync()
        {
            foreach(var step in steps)
            {
                await step.BeforeAsync();
            }
        }

        public override void Evaluate(ExeScriptBuilder commands)
        {
            foreach (var step in steps)
            {
                step.Evaluate(commands);
            }
        }

        public async override Task AfterAsync()
        {
            foreach (var step in steps)
            {
                await step.AfterAsync();
            }
        }

        public override void StoreResult(ExeScriptCommandResult result)
        {
            foreach (var step in steps)
            {
                step.StoreResult(result);
            }

            if (result.Index < 0 || result.Index >= this.Results.Length)
            {
                // TODO warning, command batch returned command index outside of the expected index range
                return;
            }

            this.Results[result.Index] = result;

            // Check if all commands now have results - if yes, mark task ac completed with result
            if (this.Results.All(item => item != null))
            {
                if (this.resultTaskCompletionSource != null)
                {
                    this.resultTaskCompletionSource.TrySetResult(this.Results); // use TrySetResult: if the whole object isn't awaited on - SetResult() throws exception...
                }
            }

        }

        public override IEnumerable<ExeScriptCommandResult> GetResults()
        {
            foreach(var step in this.steps)
            {
                foreach(var result in step.GetResults())
                {
                    yield return result;
                }
            }
        }
    }
}
