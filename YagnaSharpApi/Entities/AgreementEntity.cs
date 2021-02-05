using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using YagnaSharpApi.Entities.Events;
using YagnaSharpApi.Repository;

namespace YagnaSharpApi.Entities
{
    public class AgreementEntity : IDisposable
    {
        public enum StateEnum
        {
            Proposal = 1,
            Pending = 2,
            Cancelled = 3,
            Rejected = 4,
            Approved = 5,
            Expired = 6,
            Terminated = 7
        }

        private bool disposedValue;

        private IMarketRepository repository;

        public IMarketRepository Repository { 
            get { 
                return repository;  
            } 
            set {
                // Handle the event handler attach/detach
                if(repository != null)
                {
                    repository.OnAgreementEvent -= Repository_OnAgreementEvent;
                }
                this.repository = value;

                this.repository.OnAgreementEvent += Repository_OnAgreementEvent;
            } 
        }

        public IList<AgreementEventEntity> Events { get; private set; } = new List<AgreementEventEntity>();

        #region Fields

        public string AgreementId { get; set; }
        public DemandEntity Demand { get; set; }
        public OfferEntity Offer { get; set; }
        public StateEnum State { get; protected set; }

        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="repo"></param>
        public AgreementEntity()
        {
        }

        private void Repository_OnAgreementEvent(object sender, Events.AgreementEventEntity e)
        {
            if(e.Agreement?.AgreementId == this.AgreementId)
            {
                this.HandleAgreementEvent(e);
            }
        }

        protected void HandleAgreementEvent(Events.AgreementEventEntity ev)
        {
            this.Events.Add(ev);

            // TODO finish for whole event type hierarchy
            switch (ev)
            {
                case AgreementTerminatedEventEntity e:
                    this.State = StateEnum.Terminated;
                    break;
            }
        }

        public async Task<bool> ConfirmAsync()
        {
            // TODO consider moving all the below into the repository, and only handle exceptions properly
            await this.Repository.ConfirmAgreementAsync(this);

            try
            {
                this.State = await this.Repository.WaitForApprovalAsync(this, 90);
                this.State = StateEnum.Approved;
                return true;
            }
            catch (ApiException exc)
            {
                // TODO log exception details
                // TODO tentative - mark as rejected, need to handle the timeout and canceled case!
                this.State = StateEnum.Rejected; 
                return false;
            }
        }

        public async Task TerminateAsync(ReasonEntity reason)
        {
            // TODO
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    this.Repository.OnAgreementEvent -= Repository_OnAgreementEvent;
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
