using System;
using System.Collections.Generic;
using System.Text;
using YagnaSharpApi.Entities;

namespace YagnaSharpApi.Engine.Events
{
    public class ProposalRejected : ProposalEvent
    {
        public ProposalRejected(ProposalEntity prop, Exception exc) : base(prop, exc)
        {
        }

        public ProposalRejected(ProposalEntity prop, ReasonEntity reason) : base(prop)
        {
            this.Reason = reason;
        }

        public ReasonEntity Reason { get; set; }
    }
}
