using System;
using System.Collections.Generic;
using System.Text;
using YagnaSharpApi.Entities;

namespace YagnaSharpApi.Engine.Events
{
    public class SubscriptionCreated : SubscriptionEvent
    {
        public SubscriptionCreated(SubscriptionEntity subscription) : base(subscription)
        {
        }
    }
}
