using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YagnaSharpApi.Engine.Events;
using YagnaSharpApi.Entities;
using YagnaSharpApi.Utils;

namespace YagnaSharpApi.Studio.Model
{
    public class AgreementModel
    {

        public string Id 
        { 
            get 
            {
                return this.Agreement.AgreementId;
            }
        }

        public string IssuerName
        {
            get
            {
                return this.Agreement.Offer.Properties[Properties.NODE_ID_NAME]?.ToString();
            }
        }

        public string State { 
            get
            {
                return this.Agreement.State.ToString(); // Events.LastOrDefault()?.GetType().Name;
            }
        }

        public int EventCount
        {
            get
            {
                return this.Events.Count;
            }
        }

        public AgreementEntity Agreement { get; set; }

        public List<Event> Events { get; set; } = new List<Event>();

        public AgreementModel(AgreementEntity agreement)
        {
            this.Agreement = agreement;
        }

    }
}
