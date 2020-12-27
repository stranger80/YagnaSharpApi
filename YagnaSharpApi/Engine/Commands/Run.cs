using System;
using System.Collections.Generic;
using System.Text;

namespace YagnaSharpApi.Engine.Commands
{
    public class Run : WorkItem
    {
        public int Index { get; private set; }
        private string cmd;
        private string[] args;

        public Run(string cmd, string[] args)
        {
            this.cmd = cmd;
            this.args = args;
        }

        public override void Register(ExeScriptBuilder commands)
        {
            this.Index = commands.Run(cmd, args);
        }
    }
}
