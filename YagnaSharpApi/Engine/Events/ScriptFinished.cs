using Golem.ActivityApi.Client.Model;
using System;
using System.Collections.Generic;
using System.Text;
using YagnaSharpApi.Engine.Commands;

namespace YagnaSharpApi.Engine.Events
{
    public class ScriptFinished : TaskEvent
    {
        public string AgreementId { get; set; }

        public Script ScriptBatch { get; set; }

        public ScriptFinished(string agreementId, string taskId, Script commandBatch = default(Script))
        {
            this.AgreementId = agreementId;
            this.TaskId = taskId;
            this.ScriptBatch = commandBatch;
        }
    }
}
