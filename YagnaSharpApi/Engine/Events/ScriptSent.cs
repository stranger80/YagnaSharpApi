using Golem.ActivityApi.Client.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace YagnaSharpApi.Engine.Events
{
    public class ScriptSent : TaskEvent
    {
        public string AgreementId { get; set; }
        public IEnumerable<ExeScriptCommand> Commands { get; set; }

        public ScriptSent(string agreementId, string taskId, IEnumerable<ExeScriptCommand> commands)
        {
            this.AgreementId = agreementId;
            this.TaskId = taskId;
            this.Commands = commands;
        }
    }
}
