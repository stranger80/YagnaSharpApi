using System;
using System.Collections.Generic;
using System.Text;
using YagnaSharpApi.Entities;

namespace YagnaSharpApi.Engine.Events
{
    public class ProposalResponded : ProposalEvent
    {
        public ProposalResponded(ProposalEntity prop, ProposalEntity counterProp) : base(prop)
        {
            this.CounterProposal = counterProp;
        }
        public ProposalEntity CounterProposal { get; set; }

    }
}
