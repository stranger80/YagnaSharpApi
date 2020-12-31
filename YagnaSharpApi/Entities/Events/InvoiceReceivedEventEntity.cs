using System;
using System.Collections.Generic;
using System.Text;

namespace YagnaSharpApi.Entities.Events
{
    public class InvoiceReceivedEventEntity : InvoiceEntity
    {
        public InvoiceEntity Invoice { get; set; }
    }
}
