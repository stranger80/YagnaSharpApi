using System;
using System.Collections.Generic;
using System.Text;

namespace YagnaSharpApi.Engine.Events
{
    public class ActivityCreateFailed : ExecutorEvent
    {
        public ActivityCreateFailed(string agreementId, Exception exc = null)
        {
            this.AgreementId = agreementId;
            this.Exception = exc;
        }

        public string AgreementId { get; set; }
        public Exception Exception { get; set; }
    }
}
