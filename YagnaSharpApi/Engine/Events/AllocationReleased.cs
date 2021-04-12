using System;
using System.Collections.Generic;
using System.Text;

namespace YagnaSharpApi.Engine.Events
{
    public class AllocationReleased : PaymentEvent
    {
        public AllocationReleased(string allocationId)
        {
            this.AllocationId = allocationId;
        }

        public string AllocationId { get; set; }
    }
}
