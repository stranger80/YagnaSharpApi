using AutoMapper;
using Golem.MarketApi.Client.Api;
using Golem.MarketApi.Client.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using YagnaSharpApi.Entities;
using YagnaSharpApi.Entities.Events;

namespace YagnaSharpApi.Repository
{
    public class MarketRepository
    {
        public IRequestorApi RequestorApi { get; set; }
        public IMapper Mapper { get; set; }

        public MarketRepository(IRequestorApi requestorApi, IMapper mapper)
        {
            this.RequestorApi = requestorApi;
            this.Mapper = mapper;
        }

        public async Task<DemandSubscriptionEntity> SubscribeDemandAsync(IDictionary<string, object> properties, string constraints)
        {
            var demand = new DemandOfferBase(properties, constraints);

            try
            {
                var newSubscriptionIdText = await this.RequestorApi.SubscribeDemandAsync(demand);

                var newSubscriptionId = JsonConvert.DeserializeObject<string>(newSubscriptionIdText);

                var result = new DemandSubscriptionEntity()
                {
                    SubscriptionId = newSubscriptionId,
                    DemandId = newSubscriptionId,
                    Constraints = constraints,
                    Properties = properties,
                    // RequestorId - TODO!!!
                };

                return result;
            }
            catch (Exception exc)
            {
                throw;
            }
        }


        public async IAsyncEnumerable<EventEntity> CollectOffersAsync(string subscriptionId, decimal timeout)
        {
            List<Event> events;
            try
            {
                events = await this.RequestorApi.CollectOffersAsync(subscriptionId, (float)timeout);
            }
            catch (Exception exc)
            {
                throw;
            }

            foreach (var ev in events)
            {
                var evEntity = Mapper.Map<EventEntity>(ev);
                yield return evEntity;
            }

        }

        public async Task UnsubscribeDemandAsync(string subscriptionId)
        {
            try
            {
                await this.RequestorApi.UnsubscribeDemandAsync(subscriptionId);
            }
            catch (Exception exc)
            {
                throw;
            }
        }

    }
}
