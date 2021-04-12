using System;
using System.Collections.Generic;
using System.Text;
using YagnaSharpApi.Entities;

namespace YagnaSharpApi.Engine.Events
{
    public class SubscriptionCreated : MarketEvent
    {
        public SubscriptionCreated(SubscriptionEntity subscription)
        {
            this.Subscription = subscription;
        }

        public SubscriptionEntity Subscription { get; set; }
    }
}
