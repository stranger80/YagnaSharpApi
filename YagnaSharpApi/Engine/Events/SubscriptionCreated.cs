using System;
using System.Collections.Generic;
using System.Text;

namespace YagnaSharpApi.Engine.Events
{
    public class SubscriptionCreated : Event
    {
        public string SubscriptionId { get; set; }
    }
}
