using System;
using System.Collections.Generic;
using System.Text;

namespace YagnaSharpApi.Engine.Events
{
    public class AgreementFailed : AgreementEvent
    {
        public AgreementFailed(string proposalId, Exception exc = null)
        {
            this.ProposalId = proposalId;
            this.Exception = exc;
        }

        public string ProposalId { get; set; }
        public Exception Exception { get; set; }
    }
}
