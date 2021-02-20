using System;
using System.Collections.Generic;
using System.Text;

namespace YagnaSharpApi.Engine.Events
{
    public class InvoiceAccepted : ExecutorEvent
    {
        public InvoiceAccepted(string agreementId, string invoiceId, string amount)
        {
            this.AgreementId = agreementId;
            this.InvoiceId = invoiceId;
            this.Amount = amount;
        }

        public string AgreementId { get; set; }
        public string InvoiceId { get; set; }
        public string Amount { get; set; }
    }
}
