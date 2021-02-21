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
    public class MarketRepository : IMarketRepository, IDisposable
    {
        public IRequestorApi RequestorApi { get; set; }
        public IMapper Mapper { get; set; }

        protected Dictionary<string, AgreementEntity> AgreementsById = new Dictionary<string, AgreementEntity>();
        private bool disposedValue;
        private CancellationTokenSource cancellationTokenSource;

        public MarketRepository(IRequestorApi requestorApi, IMapper mapper)
        {
            this.RequestorApi = requestorApi;
            this.Mapper = mapper;
            this.cancellationTokenSource = new CancellationTokenSource();

            var token = cancellationTokenSource.Token;

            // launch the agreement listening thread
            Task.Run(async () => { await this.ListenAgreementEvents(token); });
        }

        public event EventHandler<AgreementEventEntity> OnAgreementEvent;

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

        public async Task RejectProposalOfferAsync(string subscriptionId, string proposalId, ReasonEntity reason)
        {
            try
            {
                var reasonEntity = Mapper.Map<Reason>(reason);
                await this.RequestorApi.RejectProposalOfferAsync(subscriptionId, proposalId, reasonEntity);
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
                var agreementProposal = new AgreementProposal(proposal.ProposalId, DateTime.UtcNow.AddSeconds((double)timeout));

                var agreementIdText = await this.RequestorApi.CreateAgreementAsync(agreementProposal);

                var agreementId = JsonConvert.DeserializeObject<string>(agreementIdText);

                // immediately fetch whole Agreement object
                var agreement = await this.RequestorApi.GetAgreementAsync(agreementId);

                var result = this.Mapper.Map<AgreementEntity>(agreement);

                result.Repository = this;

                this.AgreementsById.Add(agreementId, result);

                return result;
            }
            catch (Exception exc)
            {
                throw;
            }

        }

        public async Task<AgreementEntity> GetAgreement(string agreementId)
        {
            if(this.AgreementsById.ContainsKey(agreementId))
            {
                return this.AgreementsById[agreementId];
            }

            try
            {
                var agreement = await this.RequestorApi.GetAgreementAsync(agreementId);

                var result = this.Mapper.Map<AgreementEntity>(agreement);

                result.Repository = this;

                this.AgreementsById.Add(agreementId, result);

                return result;
            }
            catch (Exception exc)
            {
                throw;
            }
        }

        public async Task ConfirmAgreementAsync(AgreementEntity agreement)
        {
            try
            {
                await this.RequestorApi.ConfirmAgreementAsync(agreement.AgreementId);
            }
            catch (Exception exc)
            {
                throw;
            }
        }

        public async Task<AgreementEntity.StateEnum> WaitForApprovalAsync(AgreementEntity agreement, decimal timeout)
        {
            try
            {
                var response = await this.RequestorApi.WaitForApprovalAsyncWithHttpInfo(agreement.AgreementId, (float)timeout);

                switch (response.StatusCode)
                {
                    case System.Net.HttpStatusCode.NoContent:
                        return AgreementEntity.StateEnum.Approved;
                    case System.Net.HttpStatusCode.RequestTimeout:
                        return AgreementEntity.StateEnum.Pending;
                    case System.Net.HttpStatusCode.Gone:
                        return AgreementEntity.StateEnum.Rejected;
                }

                throw new ApiException("WaitForApproval error: ", response.StatusCode, new ErrorMessage() { Message = response.ErrorText });
            }
            catch (Exception exc)
            {
                throw;
            }
        }

        protected async Task ListenAgreementEvents(CancellationToken token)
        {
            var afterTimestamp = DateTime.UtcNow;

            while(!token.IsCancellationRequested)
            {
                var events = await this.RequestorApi.CollectAgreementEventsAsync(30, afterTimestamp, token: token);

                foreach(var ev in events)
                {
                    if (ev.EventDate > afterTimestamp) // move the afterTimestamp pointer
                    {
                        afterTimestamp = ev.EventDate;
                    }

                    try
                    {
                        var evEntity = Mapper.Map<AgreementEventEntity>(ev);

                        this.OnAgreementEvent?.Invoke(this, evEntity);
                    }
                    catch(Exception exc)
                    {
                        throw;
                    }
                }
            }
        }

        public async Task TerminateAgreementAsync(AgreementEntity agreement, ReasonEntity reason)
        {
            try
            {
                var reasonDict = new Dictionary<string, object>()
                {
                    { "message", reason.Message }
                };

                await this.RequestorApi.TerminateAgreementAsync(agreement.AgreementId, reasonDict);
            }
            catch (Exception exc)
            {
                throw;
            }
        }



        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    this.cancellationTokenSource.Cancel();

                    foreach(var agreement in this.AgreementsById.Values)
                    {
                        agreement.Dispose();
                    }

                    this.cancellationTokenSource.Dispose();
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

    }
}
