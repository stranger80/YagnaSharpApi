using Golem.ActivityApi.Client.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace YagnaSharpApi.Engine.Commands
{
    public class Steps : WorkItem
    {
        protected IEnumerable<WorkItem> steps;

        public Steps(IEnumerable<WorkItem> steps)
        {
            this.steps = steps;
        }

        public async override Task Prepare()
        {
            foreach(var step in steps)
            {
                await step.Prepare();
            }
        }

        public override void Register(ExeScriptBuilder commands)
        {
            foreach (var step in steps)
            {
                step.Register(commands);
            }
        }

        public async override Task Post()
        {
            foreach (var step in steps)
            {
                await step.Post();
            }
        }

        public override void StoreResult(ExeScriptCommandResult result)
        {
            foreach (var step in steps)
            {
                step.StoreResult(result);
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
