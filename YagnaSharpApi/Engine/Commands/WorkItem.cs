using Golem.ActivityApi.Client.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace YagnaSharpApi.Engine.Commands
{
    public abstract class WorkItem
    {
        public virtual async Task Prepare()
        {

        }

        public abstract void Register(ExeScriptBuilder commands);

        public virtual async Task Post()
        {

        }

        /// <summary>
        /// Once a single ExeScript command result is received - record it against a respective workitem.
        /// Note that in reality only the IndexedWorkItem subclasses can reliably record the results...
        /// </summary>
        /// <param name="result"></param>
        public abstract void StoreResult(ExeScriptCommandResult result);

        /// <summary>
        /// Extract the results received by this workitem
        /// </summary>
        /// <returns></returns>
        public abstract IEnumerable<ExeScriptCommandResult> GetResults();
    }
}
