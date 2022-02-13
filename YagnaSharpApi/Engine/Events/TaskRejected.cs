using System;
using System.Collections.Generic;
using System.Text;

namespace YagnaSharpApi.Engine.Events
{
    public class TaskRejected<TData, TResult> : TaskEvent<TData, TResult>
    {
        public TaskRejected(GolemTask<TData, TResult> task, string reason) : base(task)
        {
            this.Reason = reason;
        }

        public string Reason { get; set; }
    }
}
