using System;
using System.Collections.Generic;
using System.Text;
using YagnaSharpApi.Entities;

namespace YagnaSharpApi.Engine.Events
{
    public class AgreementCreated : AgreementEvent
    {
        public ProposalEntity OfferProposal { get; set; }

        public AgreementCreated(AgreementEntity agreement, ProposalEntity proposal)
        {
            this.Agreement = agreement;
            this.OfferProposal = proposal;
        }

    }
}
