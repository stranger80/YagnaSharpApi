using System;
using System.Collections.Generic;
using System.Text;

namespace YagnaSharpApi.Engine.Events
{
    public class PaymentFailed : ExecutorEvent
    {
        public PaymentFailed(string agreementId, Exception exc)
        {
            this.AgreementId = agreementId;
            this.Exception = exc;
        }

        public string AgreementId { get; set; }
        public Exception Exception { get; set; }
    }
}
