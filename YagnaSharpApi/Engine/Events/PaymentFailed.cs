using System;
using System.Collections.Generic;
using System.Text;
using YagnaSharpApi.Entities;

namespace YagnaSharpApi.Engine.Events
{
    public class PaymentFailed : AgreementEvent, IPaymentEvent
    {
        public PaymentFailed(AgreementEntity agreement, Exception exc) : base(agreement, exc)
        {
        }
    }
}
