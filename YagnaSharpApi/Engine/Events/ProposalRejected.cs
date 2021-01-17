using System;
using System.Collections.Generic;
using System.Text;

namespace YagnaSharpApi.Engine.Events
{
    public class ProposalRejected : ProposalEvent
    {
        public string Reason { get; set; }
    }
}
