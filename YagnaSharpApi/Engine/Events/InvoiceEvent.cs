using System;
using System.Collections.Generic;
using System.Text;
using YagnaSharpApi.Entities;

namespace YagnaSharpApi.Engine.Events
{
    public class InvoiceEvent : AgreementEvent, IPaymentEvent
    {
        public InvoiceEvent(AgreementEntity agreement, InvoiceEntity invoice) : base(agreement)
        {
            this.Invoice = invoice;
        }

        public InvoiceEntity Invoice { get; set; }
    }
}
