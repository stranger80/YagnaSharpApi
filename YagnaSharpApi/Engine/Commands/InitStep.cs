using System;
using System.Collections.Generic;
using System.Text;

namespace YagnaSharpApi.Engine.Commands
{
    public class InitStep : WorkItem
    {
        public override void Register(ExeScriptBuilder commands)
        {
            commands.Deploy();
            commands.Start();
        }
    }
}
