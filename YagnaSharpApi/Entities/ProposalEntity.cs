using System;
using System.Collections.Generic;
using System.Text;

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
    }
}
