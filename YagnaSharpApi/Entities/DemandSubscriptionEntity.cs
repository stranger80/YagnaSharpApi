using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using YagnaSharpApi.Entities.Events;
using YagnaSharpApi.Repository;

namespace YagnaSharpApi.Entities
{
    public class DemandSubscriptionEntity : SubscriptionEntity
    {

        public string DemandId { get; set; }
        public string RequestorId { get; set; }
        public IDictionary<string, object> Properties { get; set; }
        public string Constraints { get; set; }

        public DemandSubscriptionEntity(IMarketRepository repository) : base(repository)
        { }

        public async IAsyncEnumerable<EventEntity> CollectOffersAsync(decimal timeout, CancellationToken token = default(CancellationToken))
        {
            await foreach (var prop in this.repository.CollectOffersAsync(this.SubscriptionId, timeout, token))
            {
                switch(prop)
                {
                    case ProposalEventEntity propEvent:
                        propEvent.Proposal.Subscription = this;
                        break;
                    default:
                        break;
                }

                yield return prop;
            };
        }

        protected Task UnsubscribeAsync()
        {
            return this.repository.UnsubscribeDemandAsync(this.SubscriptionId);
        }

        protected async override void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    try
                    {
                        await this.UnsubscribeAsync();
                    }
                    catch(Exception exc)
                    {
                        // TODO: log the exception for diagnostic purposes
                    }
                    finally 
                    { 
                        base.Dispose(disposing);
                    }
                }

                disposedValue = true;
            }
        }


    }
}
