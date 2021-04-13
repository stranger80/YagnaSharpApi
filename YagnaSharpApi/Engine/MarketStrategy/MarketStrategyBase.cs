using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
        private bool disposedValue;

        public IMarketRepository Repository { get; private set; }

        public MarketStrategyConditions Conditions { get; set; }

        public event EventHandler<Events.Event> OnMarketEvent;

        public MarketStrategyBase(IMarketRepository repo, MarketStrategyConditions conditions)
        {
            this.Repository = repo;
            this.Conditions = conditions;
        }

        public async IAsyncEnumerable<(float, ProposalEntity)> FindOffersAsync(DemandBuilder demand, CancellationToken cancellationToken = default)
        {
            DemandSubscriptionEntity subscription;

            try
            {
                // decorate demand with additional props or constraints necessary to execute the strategy
                await this.DecorateDemandAsync(demand);
                subscription = await this.Repository.SubscribeDemandAsync(demand.Properties, demand.Constraints);
                this.OnMarketEvent?.Invoke(this, new SubscriptionCreated(subscription));

            }
            catch (Exception exc)
            {
                this.OnMarketEvent?.Invoke(this, new SubscriptionFailed() { Reason = exc.ToString() });
                throw;
            }

            IAsyncEnumerable<EventEntity> events;

            do
            {
                try
                {
                    events = subscription.CollectOffersAsync(30.0m, cancellationToken);

                }
                catch (Exception exc)
                {
                    this.OnMarketEvent?.Invoke(this, new CollectFailed() { Reason = exc.ToString() });
                    throw;
                }

                await using var enumerator = events.GetAsyncEnumerator(cancellationToken);
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
                            subscription.Dispose();

                            throw;
                        }

                        if (more)
                        {
                            var ev = enumerator.Current;
                            switch (ev)
                            {
                                case ProposalEventEntity proposalEvent:
                                    float score = 0;
                                    this.OnMarketEvent?.Invoke(this, new ProposalReceived(proposalEvent.Proposal));
                                    try
                                    {
                                        score = await this.ScoreOfferAsync(proposalEvent.Proposal);
                                    }
                                    catch (Exception exc)
                                    {
                                        this.OnMarketEvent?.Invoke(this, new ProposalRejected(proposalEvent.Proposal, exc));
                                    }

                                    if (score < MarketStrategyConsts.SCORE_NEUTRAL)
                                    {
                                        var reason = new ReasonEntity() { Message = "Score too low" };

                                        // reject proposal and raise event
                                        try
                                        {
                                            await proposalEvent.Proposal.RejectAsync(reason);
                                        }
                                        catch(Exception exc)
                                        {
                                            // TODO log warning
                                        }

                                        this.OnMarketEvent?.Invoke(this, new ProposalRejected(proposalEvent.Proposal, reason));
                                    }
                                    else if (proposalEvent.Proposal.State != ProposalState.Draft)
                                    {
                                        try
                                        {
                                            // TODO at this point decide on the payment platform
                                            // ...see if any common payment platforms

                                            var commonPlatforms = this.GetCommonPaymentPlatforms(proposalEvent.Proposal);

                                            // if common platforms found, set the chosen-platform property
                                            if (commonPlatforms.Any())
                                            {
                                                demand.Add(Properties.COM_PAYMENT_CHOSEN_PLATFORM, commonPlatforms.First());
                                            }
                                            else // if no common platofmrs - reject
                                            {
                                                var reason = new ReasonEntity() { Message = "No common payment platforms" };
                                                await proposalEvent.Proposal.RejectAsync(reason);
                                                this.OnMarketEvent?.Invoke(this, new ProposalRejected(proposalEvent.Proposal, reason));
                                            }

                                            // TODO handle the ACCEPT TIMEOUT (Debit Note heartbeat)

                                            // ...and send the response proposal
                                            var counterProposal = await proposalEvent.Proposal.RespondAsync(demand.Properties, demand.Constraints);
                                            this.OnMarketEvent?.Invoke(this, new ProposalResponded(proposalEvent.Proposal, counterProposal));


                                        }
                                        catch (Exception exc)
                                        {
                                            this.OnMarketEvent?.Invoke(this, new ProposalFailed(proposalEvent.Proposal, exc));
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
            while (!cancellationToken.IsCancellationRequested);
        }

        protected virtual IEnumerable<string> GetCommonPaymentPlatforms(ProposalEntity proposal)
        {
            var provPlatforms = proposal.Properties.Keys
                .Where(key => key.StartsWith(Properties.COM_PAYMENT_PLATFORM_))
                .Select(key => key.Split(".")[4]).Distinct();

            if(!provPlatforms.Any())
            {
                provPlatforms = new string[] { "NGNT" };
            }

            return this.Conditions?.PaymentPlatforms.Intersect(provPlatforms).ToList();

        }

        protected abstract Task DecorateDemandAsync(DemandBuilder demand);

        public abstract Task<float> ScoreOfferAsync(ProposalEntity offer);

    }
}
