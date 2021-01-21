using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using YagnaSharpApi.Repository;

namespace YagnaSharpApi.Entities
{

    public enum ProposalState
    {
        /// <summary>
        /// Enum Initial for value: Initial
        /// </summary>
        Initial = 1,

        /// <summary>
        /// Enum Draft for value: Draft
        /// </summary>
        Draft = 2,

        /// <summary>
        /// Enum Rejected for value: Rejected
        /// </summary>
        Rejected = 3,

        /// <summary>
        /// Enum Accepted for value: Accepted
        /// </summary>
        Accepted = 4,

        /// <summary>
        /// Enum Expired for value: Expired
        /// </summary>
        Expired = 5

    }

    public class ProposalEntity
    {
        public string ProposalId { get; set; }
        public string IssuerId { get; set; }
        public DateTime Timestamp { get; set; }
        public ProposalState State { get; set; }
        public string PrevProposalId { get; set; }
        public IDictionary<string, object> Properties { get; set; }
        public string Constraints { get; set; }

        public SubscriptionEntity Subscription { get; set; }

        public IMarketRepository Repository { get; set; }

        public async Task<ProposalEntity> RespondAsync(IDictionary<string, object> properties, string constraints)
        {
            var prop = await this.Repository.CounterProposalDemandAsync(this.Subscription.SubscriptionId, this.ProposalId, properties, constraints);

            prop.Subscription = this.Subscription;

            return prop;
        }

        public Task RejectAsync()
        {
            return this.Repository.RejectProposalOfferAsync(this.Subscription.SubscriptionId, this.ProposalId);
        }

        public async Task<AgreementEntity> CreateAgreementAsync()
        {
            var agreement = await this.Repository.CreateAgreementAsync(this, 3600); // TODO change default timeout const into a proper config param

            return agreement;
        }
    }
}
