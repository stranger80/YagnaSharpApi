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

        public WorkItem CommandBatch { get; set; }

        public ScriptFinished(string agreementId, string taskId, WorkItem commandBatch = default(WorkItem))
        {
            this.AgreementId = agreementId;
            this.TaskId = taskId;
            this.CommandBatch = commandBatch;
        }
    }
}
