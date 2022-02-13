using System;
using System.Collections.Generic;
using System.Text;
using YagnaSharpApi.Entities;

namespace YagnaSharpApi.Engine.Events
{
    public class ProposalFailed : ProposalEvent
    {
        public ProposalFailed(ProposalEntity prop, Exception exc = null) : base(prop, exc)
        {
        }
    }
}
