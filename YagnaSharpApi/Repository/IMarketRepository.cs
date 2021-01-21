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
    public interface IMarketRepository
    {
        Task<DemandSubscriptionEntity> SubscribeDemandAsync(IDictionary<string, object> properties, string constraints);
        IAsyncEnumerable<EventEntity> CollectOffersAsync(string subscriptionId, decimal timeout, CancellationToken token = default(CancellationToken));
        Task<ProposalEntity> CounterProposalDemandAsync(string subscriptionId, string proposalId, IDictionary<string, object> properties, string constraints);
        Task RejectProposalOfferAsync(string subscriptionId, string proposalId);
        Task UnsubscribeDemandAsync(string subscriptionId);
        Task<AgreementEntity> CreateAgreementAsync(ProposalEntity proposal, decimal timeout);

    }
}
