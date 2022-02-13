using System;
using System.Collections.Generic;
using System.Text;
using YagnaSharpApi.Entities;

namespace YagnaSharpApi.Engine.Events
{
    public class ProposalReceived : ProposalEvent
    {
        public ProposalReceived(ProposalEntity prop) : base(prop)
        {
        }
    }
}
