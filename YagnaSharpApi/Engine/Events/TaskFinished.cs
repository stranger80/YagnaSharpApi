using System;
using System.Collections.Generic;
using System.Text;

namespace YagnaSharpApi.Engine.Events
{
    public class TaskFinished<TData, TResult> : TaskEvent<TData, TResult>
    {
        public TaskFinished(GolemTask<TData, TResult> task) : base(task)
        {
        }

    }
}
