using System;
using System.Collections.Generic;
using System.Text;
using YagnaSharpApi.Entities;

namespace YagnaSharpApi.Engine.Events
{
    public class ProposalRejected : ProposalEvent
    {
        public ProposalRejected(ProposalEntity prop, Exception exc)
        {
            this.Proposal = prop;
            this.Exception = exc;
        }

        public ProposalRejected(ProposalEntity prop, ReasonEntity reason)
        {
            this.Proposal = prop;
            this.Reason = reason;
        }

        public Exception Exception { get; set; }
        public ReasonEntity Reason { get; set; }
    }
}
