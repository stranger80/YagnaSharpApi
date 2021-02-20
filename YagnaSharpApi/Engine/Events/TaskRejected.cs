using System;
using System.Collections.Generic;
using System.Text;

namespace YagnaSharpApi.Engine.Events
{
    public class TaskRejected : TaskEvent
    {
        public TaskRejected(string taskId, string reason)
        {
            this.TaskId = taskId;
            this.Reason = reason;
        }

        public string Reason { get; set; }
    }
}
