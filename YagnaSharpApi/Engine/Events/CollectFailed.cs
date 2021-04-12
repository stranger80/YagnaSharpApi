using System;
using System.Collections.Generic;
using System.Text;

namespace YagnaSharpApi.Engine.Events
{
    public class CollectFailed : MarketEvent
    {
        public string SubscriptionId { get; set; }
        public string Reason { get; set; }
    }
}
