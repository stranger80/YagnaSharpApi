using System;
using System.Collections.Generic;
using System.Text;

namespace YagnaSharpApi.Engine.Events
{
    public class ProposalFailed : ProposalEvent
    {
        public Exception Exception { get; set; }
    }
}
