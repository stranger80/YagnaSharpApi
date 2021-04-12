using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YagnaSharpApi.Engine.Events;
using YagnaSharpApi.Entities;
using YagnaSharpApi.Utils;

namespace YagnaSharpApi.Studio.Model
{
    public class OfferModel
    {

        public string Id 
        { 
            get 
            {
                return this.OfferProposal.ProposalId;
            }
        }

        public string IssuerName
        {
            get
            {
                return this.OfferProposal.Properties[Properties.NODE_ID_NAME]?.ToString();
            }
        }

        public string State { 
            get
            {
                return Events.LastOrDefault()?.GetType().Name;
            }
        }

        public int EventCount
        {
            get
            {
                return this.Events.Count;
            }
        }

        public ProposalEntity OfferProposal { get; set; }

        public List<Event> Events { get; set; } = new List<Event>();

        public OfferModel(ProposalEntity offerProposal)
        {
            this.OfferProposal = offerProposal;
        }

    }
}
