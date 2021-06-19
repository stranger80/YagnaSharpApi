using System;
using System.Collections.Generic;
using System.Text;

namespace YagnaSharpApi.Engine.Commands
{
    public class StartStep : IndexedWorkItem
    {
        public override void Register(ExeScriptBuilder commands)
        {
            this.CommandIndex = commands.Deploy();
        }
    }
}
