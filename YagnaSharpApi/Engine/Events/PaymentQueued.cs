using System;
using System.Collections.Generic;
using System.Text;
using YagnaSharpApi.Entities;

namespace YagnaSharpApi.Engine.Events
{
    public class PaymentQueued : AgreementEvent, IPaymentEvent
    {
        public PaymentQueued(AgreementEntity agreement) : base(agreement)
        {
        }
    }
}
