using System;
using System.Collections.Generic;
using System.Text;
using YagnaSharpApi.Entities;

namespace YagnaSharpApi.Engine.Events
{
    public class ProposalEvent : MarketEvent
    {
        public ProposalEntity Proposal { get; set; }
    }
}
