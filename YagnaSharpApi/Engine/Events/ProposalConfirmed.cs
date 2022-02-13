using System;
using System.Collections.Generic;
using System.Text;
using YagnaSharpApi.Entities;

namespace YagnaSharpApi.Engine.Events
{
    public class ProposalConfirmed : ProposalEvent
    {
        public ProposalConfirmed(ProposalEntity proposal) : base(proposal)
        {
        }
    }
}
