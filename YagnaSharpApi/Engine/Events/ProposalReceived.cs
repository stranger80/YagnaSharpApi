using System;
using System.Collections.Generic;
using System.Text;

namespace YagnaSharpApi.Engine.Events
{
    public class ProposalReceived : ProposalEvent
    {
        public string ProviderId { get; set; }
    }
}
