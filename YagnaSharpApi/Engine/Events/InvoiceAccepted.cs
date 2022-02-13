using System;
using System.Collections.Generic;
using System.Text;
using YagnaSharpApi.Entities;

namespace YagnaSharpApi.Engine.Events
{
    public class InvoiceAccepted : InvoiceEvent
    {
        public InvoiceAccepted(AgreementEntity agreement, InvoiceEntity invoice) : base(agreement, invoice)
        {
        }
    }
}
