using System;
using System.Collections.Generic;
using System.Text;

namespace YagnaSharpApi.Engine.Events
{
    public class TaskAccepted<TData, TResult> : TaskEvent<TData, TResult>
    {
        public TaskAccepted(GolemTask<TData, TResult> task, TResult result) : base(task)
        {
            this.Result = result;
        }

        public Type GetResultType()
        {
            return typeof(TResult);
        }

        public TResult Result { get; set; }
    }
}
