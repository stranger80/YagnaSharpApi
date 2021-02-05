using System;
using System.Collections.Generic;
using System.Text;

namespace YagnaSharpApi.Engine.Events
{
    public class AgreementFailed : AgreementEvent
    {
        public string ProposalId { get; set; }
    }
}
