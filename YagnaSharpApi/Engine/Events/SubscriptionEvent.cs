using System;
using System.Collections.Generic;
using System.Text;
using YagnaSharpApi.Entities;

namespace YagnaSharpApi.Engine.Events
{
    public class SubscriptionEvent : ExecutorEvent, IMarketEvent
    {
        public SubscriptionEvent(SubscriptionEntity subscription, Exception exc = default(Exception)) : base(exc)
        {
            this.Subscription = subscription;
        }

        public SubscriptionEntity Subscription { get; set; }
    }
}
