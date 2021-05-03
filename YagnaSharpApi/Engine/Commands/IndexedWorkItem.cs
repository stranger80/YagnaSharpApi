using Golem.ActivityApi.Client.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace YagnaSharpApi.Engine.Commands
{
    public abstract class IndexedWorkItem : WorkItem
    {
        public int CommandIndex { get; protected set; }
        public ExeScriptCommandResult Result { get; set; }

        public override void StoreResult(ExeScriptCommandResult result)
        {
            if(this.CommandIndex == result.Index)
            {
                this.Result = result;
            }
        }

        public override IEnumerable<ExeScriptCommandResult> GetResults()
        {
            yield return this.Result;
        }
    }
}
