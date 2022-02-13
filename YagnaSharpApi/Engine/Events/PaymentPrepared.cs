using System;
using System.Collections.Generic;
using System.Text;
using YagnaSharpApi.Entities;

namespace YagnaSharpApi.Engine.Events
{
    public class PaymentPrepared : AgreementEvent, IPaymentEvent
    {
        public PaymentPrepared(AgreementEntity agreement): base(agreement)
        {
        }
    }
}
