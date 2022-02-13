using System;
using System.Collections.Generic;
using System.Text;

namespace YagnaSharpApi.Engine.Events
{
    public class SubscriptionFailed : ExecutorEvent, IMarketEvent
    {
        public SubscriptionFailed(Exception exc = null) : base(exc)
        {
            this.Reason = exc?.Message;
        }

        public string Reason { get; set; }
    }
}
