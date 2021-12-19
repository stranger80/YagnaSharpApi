using System;
using System.Collections.Generic;
using System.Text;

namespace YagnaSharpApi.Engine.Commands
{
    public class DeployStep : IndexedCommand
    {
        public override void Evaluate(ExeScriptBuilder commands)
        {
            this.CommandIndex = commands.Deploy();
        }
    }
}
