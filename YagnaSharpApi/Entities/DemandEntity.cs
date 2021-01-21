using System;
using System.Collections.Generic;
using System.Text;

namespace YagnaSharpApi.Entities
{
    public class DemandEntity : DemandOfferBaseEntity
    {
        public string DemandId { get; set; }

        public string RequestorId { get; set; }

        public DateTime Timestamp { get; set; }

    }
}
