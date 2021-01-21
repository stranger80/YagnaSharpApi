using System;
using System.Collections.Generic;
using System.Text;

namespace YagnaSharpApi.Entities
{
    public class OfferEntity : DemandOfferBaseEntity
    {
        public string OfferId { get; set; }

        public string ProviderId { get; set; }

        public DateTime Timestamp { get; set; }

    }
}
