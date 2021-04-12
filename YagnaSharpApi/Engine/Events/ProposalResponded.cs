using System;
using System.Collections.Generic;
using System.Text;
using YagnaSharpApi.Entities;

namespace YagnaSharpApi.Engine.Events
{
    public class ProposalResponded : ProposalEvent
    {
        public ProposalEntity CounterProposal { get; set; }

        public ProposalResponded(ProposalEntity prop, ProposalEntity counterProp)
        {
            this.Proposal = prop;
            this.CounterProposal = counterProp;
        }
    }
}
