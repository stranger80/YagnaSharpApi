using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YagnaSharpApi.Engine.Events;
using YagnaSharpApi.Entities;
using YagnaSharpApi.Entities.Events;
using YagnaSharpApi.Repository;
using YagnaSharpApi.Utils;
using YagnaSharpApi.Utils.PropertyModel;

namespace YagnaSharpApi.Engine.MarketStrategy
{
    public abstract class MarketStrategyBase : IMarketStrategy
    {

        public IMarketRepository Repository { get; set; }

        public event EventHandler<Events.Event> OnMarketEvent;

        public async IAsyncEnumerable<(float, ProposalEntity)> FindOffersAsync(DemandBuilder demand)
        {
            DemandSubscriptionEntity subscription;

            try
            {
                subscription = await this.Repository.SubscribeDemandAsync(demand.Properties, demand.Constraints);
                this.OnMarketEvent?.Invoke(this, new SubscriptionCreated() { SubscriptionId = subscription.SubscriptionId });

            }
            catch (Exception exc)
            {
                this.OnMarketEvent?.Invoke(this, new SubscriptionFailed() { Reason = exc.ToString() });
                throw;
            }

            IAsyncEnumerable<EventEntity> events;
            

            try
            {
                events = subscription.CollectOffersAsync(30.0m);

            }
            catch(Exception exc)
            {
                this.OnMarketEvent?.Invoke(this, new CollectFailed() { Reason = exc.ToString() });
                throw;
            }

            await using var enumerator = events.GetAsyncEnumerator(); // TODO cancellationToken here
            {

                bool more = false;
                do
                {
                    try
                    {
                        more = await enumerator.MoveNextAsync();
                    }
                    catch (Exception exc)
                    {
                        this.OnMarketEvent?.Invoke(this, new CollectFailed() { Reason = exc.ToString() });
                        throw;
                    }

                    if (more)
                    {
                        var ev = enumerator.Current;
                        switch (ev)
                        {
                            case ProposalEventEntity proposalEvent:
                                float score = 0;
                                this.OnMarketEvent?.Invoke(this, new ProposalReceived() { 
                                    ProposalId = proposalEvent.Proposal.ProposalId, 
                                    ProviderId = proposalEvent.Proposal.IssuerId });
                                try
                                {
                                    score = await this.ScoreOfferAsync(proposalEvent.Proposal);
                                }
                                catch (Exception exc)
                                {
                                    this.OnMarketEvent?.Invoke(this, new ProposalRejected() { 
                                        ProposalId = proposalEvent.Proposal.ProposalId });
                                }

                                if (score < MarketStrategyConsts.SCORE_NEUTRAL)
                                {
                                    // reject proposal and raise event
                                    await proposalEvent.Proposal.RejectAsync();
                                    this.OnMarketEvent?.Invoke(this, new ProposalRejected() { 
                                        ProposalId = proposalEvent.Proposal.ProposalId });
                                }
                                else if (proposalEvent.Proposal.State != ProposalState.Draft)
                                {
                                    try
                                    {
                                        // TODO at this point decide on the payment platform
                                        // ...see if any common payment platforms

                                        // if no common platofmrs - reject
                                        await proposalEvent.Proposal.RejectAsync();
                                        this.OnMarketEvent?.Invoke(this, new ProposalRejected() { 
                                            ProposalId = proposalEvent.Proposal.ProposalId });

                                        // if common platforms found, set the chosen-platform property
                                        // TODO demand.Add( "chosen-platform", common platform)
                                        // and send the response proposal
                                        await proposalEvent.Proposal.RespondAsync(demand.Properties, demand.Constraints);
                                        this.OnMarketEvent?.Invoke(this, new ProposalResponded() { 
                                            ProposalId = proposalEvent.Proposal.ProposalId });
                                    }
                                    catch(Exception exc)
                                    {
                                        this.OnMarketEvent?.Invoke(this, new ProposalFailed() { 
                                            ProposalId = proposalEvent.Proposal.ProposalId,
                                            Exception = exc
                                        });
                                    }
                                }
                                else
                                {
                                    // a confirmed proposal has been received - return the received proposal
                                    // so that it can be added to "AgreementCandidate pool"
                                    yield return (score, proposalEvent.Proposal);
                                }
                                break;
                            case PropertyQueryEventEntity propQueryEvent:
                                break;
                            case ProposalRejectedEventEntity propRejectedEvent:
                                break;
                        }

                    }
                }
                while (more);
            }
        }



        protected abstract Task DecorateDemandAsync(DemandBuilder demand);

        public abstract Task<float> ScoreOfferAsync(ProposalEntity offer);
    }
}
