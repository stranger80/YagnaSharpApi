using AutoMapper;
using Golem.MarketApi.Client.Api;
using Golem.MarketApi.Client.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using YagnaSharpApi.Entities;
using YagnaSharpApi.Entities.Events;

namespace YagnaSharpApi.Repository
{
    public class MarketRepository : IMarketRepository
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

                var result = new DemandSubscriptionEntity(this)
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


        public async IAsyncEnumerable<EventEntity> CollectOffersAsync(string subscriptionId, decimal timeout, CancellationToken token = default(CancellationToken))
        {
            List<Event> events;
            try
            {
                events = await this.RequestorApi.CollectOffersAsync(subscriptionId, (float)timeout, null, token);
            }
            catch (Exception exc)
            {
                throw;
            }

            foreach (var ev in events)
            {
                var evEntity = Mapper.Map<EventEntity>(ev);
                
                // Add reference to repository to the Proposal entity
                switch(evEntity)
                {
                    case ProposalEventEntity propEntity:
                        propEntity.Proposal.Repository = this;
                        break;
                    default:
                        break;
                }

                yield return evEntity;
            }

        }

        public async Task<ProposalEntity> CounterProposalDemandAsync(string subscriptionId, string proposalId, IDictionary<string, object> properties, string constraints)
        {
            var demand = new DemandOfferBase(properties, constraints);

            try
            {
                var newProposalIdText = await this.RequestorApi.CounterProposalDemandAsync(subscriptionId, proposalId, demand);

                var newProposalId = JsonConvert.DeserializeObject<string>(newProposalIdText);

                var result = new ProposalEntity
                {
                    PrevProposalId = proposalId,
                    ProposalId = newProposalId,
                    Constraints = constraints,
                    Properties = properties,
                    // IssuerId - TODO!!!
                };

                return result;
            }
            catch (Exception exc)
            {
                throw;
            }

        }

        public async Task RejectProposalOfferAsync(string subscriptionId, string proposalId)
        {
            try
            {
                await this.RequestorApi.RejectProposalOfferAsync(subscriptionId, proposalId);
            }
            catch (Exception exc)
            {
                throw;
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

        public async Task<AgreementEntity> CreateAgreementAsync(ProposalEntity proposal, decimal timeout = 1000m)
        {
            try
            {
                var agreementProposal = new AgreementProposal()
                {
                    ProposalId = proposal.ProposalId,
                    ValidTo = DateTime.UtcNow.AddSeconds((double)timeout)
                };

                var agreementId = await this.RequestorApi.CreateAgreementAsync(agreementProposal);

                var result = new AgreementEntity()
                {
                    Id = agreementId,
                    Proposal = proposal,
                    Repository = this
                };

                return result;
            }
            catch (Exception exc)
            {
                throw;
            }

        }

    }
}
