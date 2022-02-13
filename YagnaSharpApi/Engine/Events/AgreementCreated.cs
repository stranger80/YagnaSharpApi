using System;
using System.Collections.Generic;
using System.Text;
using YagnaSharpApi.Entities;

namespace YagnaSharpApi.Engine.Events
{
    public class AgreementCreated : AgreementEvent, IMarketEvent
    {
        public AgreementCreated(AgreementEntity agreement, ProposalEntity proposal) : base(agreement)
        {
            this.OfferProposal = proposal;
        }

        public ProposalEntity OfferProposal { get; set; }
    }
}
