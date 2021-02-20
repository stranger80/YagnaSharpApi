using Golem.ActivityApi.Client.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace YagnaSharpApi.Engine.Events
{
    public class ScriptFinished : TaskEvent
    {
        public string AgreementId { get; set; }

        public ScriptFinished(string agreementId, string taskId)
        {
            this.AgreementId = agreementId;
            this.TaskId = taskId;
        }
    }
}
