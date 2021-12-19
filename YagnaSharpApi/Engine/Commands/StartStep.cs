using System;
using System.Collections.Generic;
using System.Text;

namespace YagnaSharpApi.Engine.Commands
{
    public class StartStep : IndexedCommand
    {
        public override void Evaluate(ExeScriptBuilder commands)
        {
            this.CommandIndex = commands.Start();
        }
    }
}
