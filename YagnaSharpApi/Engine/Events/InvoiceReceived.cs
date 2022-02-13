using System;
using System.Collections.Generic;
using System.Text;
using YagnaSharpApi.Entities;

namespace YagnaSharpApi.Engine.Events
{
    public class InvoiceReceived : InvoiceEvent
    {
        public InvoiceReceived(AgreementEntity agreement, InvoiceEntity invoice) : base(agreement, invoice)
        {
        }
    }
}
