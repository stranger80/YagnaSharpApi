using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using YagnaSharpApi.Entities.Events;
using YagnaSharpApi.Repository;

namespace YagnaSharpApi.Entities
{
    public class SubscriptionEntity
    {
        protected IMarketRepository repository;

        public string SubscriptionId { get; set; }

        public SubscriptionEntity(IMarketRepository repository)
        {
            this.repository = repository;
        }
    }
}
