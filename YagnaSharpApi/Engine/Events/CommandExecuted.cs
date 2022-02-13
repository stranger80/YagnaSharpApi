using Golem.ActivityApi.Client.Model;
using System;
using System.Collections.Generic;
using System.Text;
using YagnaSharpApi.Entities;

namespace YagnaSharpApi.Engine.Events
{
    public class CommandExecuted : CommandEvent
    {
        public CommandExecuted(AgreementEntity agreement, ActivityEntity activity, Script script) : base(agreement, activity, script)
        {
        }

        public ExeScriptCommand Command { get; set; }
        public bool Success { get; set; }
        public string Message { get; set; }
        public string StdOut { get; set; }
        public string StdErr { get; set; }

    }
}
