using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using YagnaSharpApi.Engine.Commands;
using YagnaSharpApi.Engine.MarketStrategy;
using YagnaSharpApi.Entities;
using YagnaSharpApi.Entities.Events;
using YagnaSharpApi.Repository;
using YagnaSharpApi.Utils;

namespace YagnaSharpApi.Engine
{
    public class Executor : IDisposable
    {
        private bool disposedValue;
        public IMarketStrategy MarketStrategy { get; set; }

        public Executor(IPackage package, int maxWorkers, decimal budget, int timeout, string subnetTag, IMarketStrategy marketStrategy = null)
        {
            this.MarketStrategy = marketStrategy ?? new DummyMarketStrategy();


        }

        public IAsyncEnumerable<GolemTask<TData, TResult>> Submit<TData, TResult>(Func<WorkContext, IAsyncEnumerable<GolemTask<TData, TResult>>, IAsyncEnumerable<WorkItem>> worker, IEnumerable<GolemTask<TData, TResult>> data)
        {
            throw new NotImplementedException();

            // 1. Create Allocation

            // 2. Build Demand
            //    - make sure to include subnet

            // 3. Create GFTP storage provider

            // 4. Start FindOffers thread

            // 5. Start ProcessInvoices thread

            // 6. 


        }


        /// <summary>
        /// Logic to subscribe the Demand on the market and process incoming Offer proposals
        /// NOTE: this also includes "negotiation" of the payment platform.
        /// </summary>
        /// <param name="repo"></param>
        /// <param name="demand"></param>
        /// <returns></returns>
        protected async Task FindOffersAsync(MarketRepository repo, DemandBuilder demand /* TODO pass objects to collect stats */)
        {
            DemandSubscriptionEntity subscription;

            try
            {
                subscription = await repo.SubscribeDemandAsync(demand.Properties, demand.Constraints);

            }
            catch(Exception exc)
            {
                // TODO log, raise event
                throw;
            }

            var events = repo.CollectOffersAsync(subscription.SubscriptionId, 30.0m);

            try
            {
                await foreach (var ev in events)
                {
                    switch(ev)
                    {
                        case ProposalEventEntity proposalEvent:
                            float score = 0;
                            try
                            {
                                score = await this.MarketStrategy.ScoreOfferAsync(proposalEvent.Proposal);
                            }
                            catch(Exception exc)
                            {

                            }

                            if(score < MarketStrategyConsts.SCORE_NEUTRAL)
                            {
                                // TODO reject proposal and raise event
                            }
                            else if(proposalEvent.Proposal.State != ProposalState.Draft)
                            {
                                // TODO at this point decide on the payment platform
                                // ...see if any common payment platforms

                                // if no common platofmrs - reject
                                // await proposalEvent.Proposal.RejectAsync();
                                // send event

                                // if common platforms found, set the chosen-platform property
                                // demand.Add( "chosen-platform", common platform)
                                // and send the response proposal
                                // await proposalEvent.Proposal.RespondAsync(demand.Properties, demand.Constraints);
                                // send event

                            }
                            else
                            {
                                // a confirmed proposal has been received - add the received proposal to "AgreementCandidate pool"
                            }
                            break;
                        case PropertyQueryEventEntity propQueryEvent:
                            break;
                        case ProposalRejectedEventEntity propRejectedEvent:
                            break;
                    }
                }
            }
            catch (Exception exc)
            {
                // TODO log, raise event
                throw;
            }

        }

        protected async Task ProcessInvoicesAsync(PaymentRepository repo /* pass objects to collect stats */)
        {
            await foreach(var invoice in repo.GetInvoiceEventsAsync())
            {

            }
        }


        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~Engine()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
