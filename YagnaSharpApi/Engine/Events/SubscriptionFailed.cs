using System;
using System.Collections.Generic;
using System.Text;

namespace YagnaSharpApi.Engine.Events
{
    public class SubscriptionFailed : Event
    {
        public string Reason { get; set; }
    }
}
