using System;
using System.Collections.Generic;
using System.Text;
using YagnaSharpApi.Entities;

namespace YagnaSharpApi.Engine.Events
{
    public class ProposalEvent : ExecutorEvent, IMarketEvent
    {
        public ProposalEvent(ProposalEntity proposal, Exception exc = null) : base(exc)
        {
            this.Proposal = proposal;
        }

        public ProposalEntity Proposal { get; set; }
    }
}
