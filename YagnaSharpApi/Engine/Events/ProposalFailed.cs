using System;
using System.Collections.Generic;
using System.Text;
using YagnaSharpApi.Entities;

namespace YagnaSharpApi.Engine.Events
{
    public class ProposalFailed : ProposalEvent
    {
        public ProposalFailed(ProposalEntity prop, Exception exc = null)
        {
            this.Proposal = prop;
            this.Exception = exc;
        }

        public Exception Exception { get; set; }
    }
}
