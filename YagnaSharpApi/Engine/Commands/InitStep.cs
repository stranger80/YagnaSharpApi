using Golem.ActivityApi.Client.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace YagnaSharpApi.Engine.Commands
{
    public class InitStep : Command
    {
        private int deployIndex;
        private int startIndex;

        public ExeScriptCommandResult DeployResult { get; set; }
        public ExeScriptCommandResult StartResult { get; set; }

        public override void Evaluate(ExeScriptBuilder commands)
        {
            this.deployIndex = commands.Deploy();
            this.startIndex = commands.Start();
        }

        public override void StoreResult(ExeScriptCommandResult result)
        {
            if(result.Index == deployIndex)
            {
                this.DeployResult = result;
            }

            if (result.Index == startIndex)
            {
                this.StartResult = result;
            }
        }

        public override IEnumerable<ExeScriptCommandResult> GetResults()
        {
            yield return this.DeployResult;
            yield return this.StartResult;
        }
    }
}
