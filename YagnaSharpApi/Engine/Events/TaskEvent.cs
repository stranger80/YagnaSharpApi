using System;
using System.Collections.Generic;
using System.Text;
using YagnaSharpApi.Entities;

namespace YagnaSharpApi.Engine.Events
{
    public class TaskEvent<TData, TResult> : ActivityEvent
    {
        public TaskEvent(GolemTask<TData, TResult> task) : base(task.Agreement, task.Activity)
        {
            this.Task = task;
        }

        public GolemTask<TData, TResult> Task { get; set; }
    }
}
