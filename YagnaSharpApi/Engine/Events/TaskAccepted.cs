using System;
using System.Collections.Generic;
using System.Text;

namespace YagnaSharpApi.Engine.Events
{
    public class TaskAccepted<TResult> : TaskEvent
    {
        public TaskAccepted(string taskId, TResult result)
        {
            this.TaskId = taskId;
            this.Result = result;
        }

        public Type GetResultType()
        {
            return typeof(TResult);
        }

        public TResult Result { get; set; }
    }
}
