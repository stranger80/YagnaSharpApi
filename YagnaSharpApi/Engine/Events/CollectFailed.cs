using System;
using System.Collections.Generic;
using System.Text;
using YagnaSharpApi.Entities;

namespace YagnaSharpApi.Engine.Events
{
    public class CollectFailed : SubscriptionEvent
    {
        public CollectFailed(SubscriptionEntity subscription, string reason) : base(subscription)
        {
            this.Reason = reason;
        }

        public CollectFailed(SubscriptionEntity subscription, Exception exc) : base(subscription, exc)
        {
            this.Reason = exc?.Message;
        }

        public string Reason { get; set; }
    }
}
