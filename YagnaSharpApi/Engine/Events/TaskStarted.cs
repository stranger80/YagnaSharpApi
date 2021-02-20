using System;
using System.Collections.Generic;
using System.Text;

namespace YagnaSharpApi.Engine.Events
{
    public class TaskStarted<TData> : TaskEvent
    {
        public string AgreementId { get; set; }
        public TData Data { get; set; }

        public TaskStarted(string agreementId, string taskId, TData data)
        {
            this.AgreementId = agreementId;
            this.TaskId = taskId;
            this.Data = data;
        }

        public Type GetDataType()
        {
            return typeof(TData);
        }

    }
}
