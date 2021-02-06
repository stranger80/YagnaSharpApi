using System;
using System.Collections.Generic;
using System.Text;

namespace YagnaSharpApi.Entities
{
    public class ExeScriptBatchEntity
    {
        public ExeScriptBatchEntity(ActivityEntity activity, string batchId)
        {
            this.Activity = activity;
            this.BatchId = batchId;
        }

        public ActivityEntity Activity { get; protected set; }
        public string BatchId { get; protected set; }
    }
}
