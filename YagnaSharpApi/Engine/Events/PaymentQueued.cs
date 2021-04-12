using System;
using System.Collections.Generic;
using System.Text;

namespace YagnaSharpApi.Engine.Events
{
    public class PaymentQueued : PaymentEvent
    {
        public PaymentQueued(string agreementId)
        {
            this.AgreementId = agreementId;
        }

        public string AgreementId { get; set; }
    }
}
