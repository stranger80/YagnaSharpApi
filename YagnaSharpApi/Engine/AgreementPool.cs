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

        private SemaphoreSlim lockObject = new SemaphoreSlim(1, 1);

        public event EventHandler<Events.AgreementEvent> OnAgreementEvent;

        public AgreementPool()
        {
            this.Agreements = new ConcurrentDictionary<string, BufferedAgreement>();
            this.OfferBuffer = new ConcurrentDictionary<string, BufferedProposal>();
        }

        public void AddProposal(float score, ProposalEntity proposal)
        {
            this.OfferBuffer[proposal.IssuerId] =  
                new BufferedProposal() 
                { 
                    Proposal = proposal, 
                    Score = score, 
                    Timestamp = DateTime.Now 
                };
        }

        public async Task<Task> UseAgreementAsync(Func<AgreementEntity, Task> job)  // ok this is weird, an async method returning a Task...
        {
            await this.lockObject.WaitAsync();
            try
            {
                var agreement = await GetAgreementAsync();

                if (agreement == null)
                    return null;

                var task = job(agreement);
                await SetWorkerAsync(agreement.AgreementId, task);
                return task;
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

        protected BufferedProposal GetRandomBestOffer()
        {
            var maxScore = this.OfferBuffer
                .Max(ofr => ofr.Value.Score);

            var offers = this.OfferBuffer
                .Where(ofr => ofr.Value.Score == maxScore)
                .Select(ofr => ofr.Value)
                .ToList();

            return this.RandomElement(offers);

        }

        protected async Task<AgreementEntity> GetAgreementAsync()
        {
            // try to reuse a known available agreement
            Random random = new Random();

            AgreementEntity bufferedAgreement = null;

            var agreements = this.Agreements.Values
                .Where(a => a.WorkerTask == null)
                .Select(a => a.Agreement)
                .ToArray();

            if (agreements.Length > 0)
            {
                int ind = random.Next(0, agreements.Length);
                bufferedAgreement = agreements[ind];
                return bufferedAgreement;
            }

            // no existing agreements found - create agreement from a randomly selected offer having maximum score
            var offer = this.GetRandomBestOffer();

            AgreementEntity agreement = null;

            try
            {
                agreement = await offer.Proposal.CreateAgreementAsync();
                // TODO extract requestor activity properties
                // TODO extract provider Activity properties
                // TODO extract provider nodeinfo properties
                // raise AgreementCreated event
                this.OnAgreementEvent?.Invoke(this, new AgreementCreated() { AgreementId = agreement.AgreementId, ProviderId = agreement.Offer.ProviderId });

            }
            catch (ApiException exc)
            {
                this.OnAgreementEvent?.Invoke(this, new AgreementFailed() { ProposalId = offer.Proposal.ProposalId });
            }

            if (!await agreement.ConfirmAsync())
            {
                this.OnAgreementEvent?.Invoke(this, new AgreementRejected() { AgreementId = agreement.AgreementId });
            }

            this.Agreements[agreement.AgreementId] = new BufferedAgreement()
            {
                Agreement = agreement,
                WorkerTask = null,
                // HasMultiActivity = providerMultiActivity & requestorMultiActivity // TODO 
            };

            // raise AgreementConfirmed event
            this.OnAgreementEvent?.Invoke(this, new AgreementConfirmed() { AgreementId = agreement.AgreementId });
            
            // TODO stats - agreement counter increment

            return agreement;
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
            throw new NotImplementedException();
        }
    }
}
