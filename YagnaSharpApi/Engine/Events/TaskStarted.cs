using System;
using System.Collections.Generic;
using System.Text;

namespace YagnaSharpApi.Engine.Events
{
    public class TaskStarted<TData, TResult> : TaskEvent<TData, TResult>
    {
        public TData Data { get; set; }

        public TaskStarted(GolemTask<TData, TResult> task, TData data) : base(task)
        {
            this.Data = data;
        }

        public Type GetDataType()
        {
            return typeof(TData);
        }

    }
}
