using System;
using System.Collections.Generic;
using System.Text;

namespace YagnaSharpApi.Engine.Events
{
    public class WorkerStarted : ExecutorEvent
    {
        public WorkerStarted(string agreementId)
        {
            this.AgreementId = agreementId;
        }

        public string AgreementId { get; set; }
    }
}
