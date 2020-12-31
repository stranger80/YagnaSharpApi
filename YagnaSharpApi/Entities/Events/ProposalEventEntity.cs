using System;
using System.Collections.Generic;
using System.Text;

namespace YagnaSharpApi.Entities.Events
{
    public class ProposalEventEntity : EventEntity
    {
        public ProposalEntity Proposal { get; set; }
    }
}
