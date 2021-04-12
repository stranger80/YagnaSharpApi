using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using YagnaSharpApi.Engine.Events;
using YagnaSharpApi.Entities;

namespace YagnaSharpApi.Engine
{
    public class BufferedAgreement
    {
        public AgreementEntity Agreement { get; set; }
        public Task WorkerTask { get; set; } // should this be a task, or a Func delegate?
        public bool HasMultiActivity { get; set; }
    }

    public class BufferedProposal
    {
        public DateTime Timestamp { get; set; }
        public float Score { get; set; }
        public ProposalEntity Proposal { get; set; }
    }

    public class AgreementPool
    {
        public IDictionary<string, BufferedAgreement> Agreements { get; set; }
        public IDictionary<string, BufferedProposal> OfferBuffer { get; set; }

        private BlockingCollection<BufferedProposal> OfferPipeline { get; set; }

        private SemaphoreSlim lockObject = new SemaphoreSlim(1, 1);

        private int proposalCount = 0;

        public event EventHandler<Events.AgreementEvent> OnAgreementEvent;

        public AgreementPool()
        {
            this.Agreements = new ConcurrentDictionary<string, BufferedAgreement>();
            this.OfferBuffer = new ConcurrentDictionary<string, BufferedProposal>();

            this.OfferPipeline = new BlockingCollection<BufferedProposal>();
        }

        public void AddProposal(float score, ProposalEntity proposal)
        {
            //this.lockObject.Wait();
            var bufferedProposal = new BufferedProposal()
            {
                Proposal = proposal,
                Score = score,
                Timestamp = DateTime.Now
            };

            this.OfferBuffer[proposal.IssuerId] = bufferedProposal;
            this.OfferPipeline.Add(bufferedProposal);
            proposalCount++;

            //this.lockObject.Release();
        }

        public async Task<Task> UseAgreementAsync(Func<BufferedAgreement, Task> job)  // ok this is weird, an async method returning a Task...
        {
            await this.lockObject.WaitAsync();
            try
            {
                var bufferedAgreement = await GetAgreementAsync();

                if (bufferedAgreement == null)
                    return null;

                var task = job(bufferedAgreement);
                await SetWorkerAsync(bufferedAgreement.Agreement.AgreementId, task);
                return task;
            }
            catch(Exception exc)
            {
                // TODO will this catch exception in StartWorker?
                throw;
            }
            finally
            {
                this.lockObject.Release();
            }
        }

        protected T RandomElement<T>(IList<T> list)
        {
            var random = new Random();
            int index = random.Next(list.Count);
            return list[index];
        }

        protected BufferedProposal TakeRandomBestOffer()
        {
            // ok, this is a bit naive sync mechanism - to prevent trying to find random candidate 
            // - wait on a blocking collection, until at least one offer arrives. Once it arrives - select the candidate offer, etc, 
            // do not put them back.

            var firstOffer = this.OfferPipeline.Take();

            //this.OfferPipeline.Add(firstOffer);

            var maxScore = this.OfferBuffer
                .Max(ofr => ofr.Value.Score);

            var offers = this.OfferBuffer
                .Where(ofr => ofr.Value.Score == maxScore)
                .Select(ofr => ofr.Value)
                .ToList();

            var returnedOffer = this.RandomElement(offers);

            this.OfferBuffer.Remove(returnedOffer.Proposal.IssuerId);

            return returnedOffer;


        }

        protected async Task<BufferedAgreement> GetAgreementAsync()
        {
            // try to reuse a known available agreement
            Random random = new Random();

            BufferedAgreement bufferedAgreement = null;

            var bufferedAgreements = this.Agreements.Values
                .Where(a => a.WorkerTask == null)
                .ToArray();

            if (bufferedAgreements.Length > 0)
            {
                int ind = random.Next(0, bufferedAgreements.Length);
                bufferedAgreement = bufferedAgreements[ind];
                return bufferedAgreement;
            }

            // no existing agreements found - create agreement from a randomly selected offer having maximum score
            var offer = this.TakeRandomBestOffer();

            AgreementEntity agreement = null;

            try
            {
                agreement = await offer.Proposal.CreateAgreementAsync();
                // TODO extract requestor activity properties
                // TODO extract provider Activity properties
                // TODO extract provider nodeinfo properties
                // raise AgreementCreated event
                this.OnAgreementEvent?.Invoke(this, new AgreementCreated(agreement, offer.Proposal));

            }
            catch (ApiException exc)
            {
                this.OnAgreementEvent?.Invoke(this, new AgreementFailed(offer.Proposal.ProposalId, exc));
                throw;
            }

            if (!await agreement.ConfirmAsync())
            {
                this.OnAgreementEvent?.Invoke(this, new AgreementRejected(agreement));
                return null;
            }

            this.Agreements[agreement.AgreementId] = new BufferedAgreement()
            {
                Agreement = agreement,
                WorkerTask = null,
                // HasMultiActivity = providerMultiActivity & requestorMultiActivity // TODO 
            };

            // raise AgreementConfirmed event
            this.OnAgreementEvent?.Invoke(this, new AgreementConfirmed(agreement));
            
            // TODO stats - agreement counter increment

            return this.Agreements[agreement.AgreementId];
        }

        protected async Task SetWorkerAsync(string agreementId, Task job)
        {
            if (this.Agreements.ContainsKey(agreementId))
            {
                var bufferedAgreement = this.Agreements[agreementId];
                Contract.Assume(bufferedAgreement.WorkerTask == null);
                bufferedAgreement.WorkerTask = job;
            }
        }

        /// <summary>
        /// Terminate all agreements in pool.
        /// </summary>
        /// <param name="reason"></param>
        /// <returns></returns>
        public async Task TerminateAsync(ReasonEntity reason)
        {
            foreach(var agreement in this.Agreements)
            {
                await agreement.Value.Agreement.TerminateAsync(reason);
            }

            this.Agreements.Clear();
        }
    }
}
