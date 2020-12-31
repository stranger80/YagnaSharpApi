using System;
using System.Collections.Generic;
using System.Text;

namespace YagnaSharpApi.Entities
{
    public class DemandSubscriptionEntity : SubscriptionEntity
    {
        public string DemandId { get; set; }
        public string RequestorId { get; set; }
        public IDictionary<string, object> Properties { get; set; }
        public string Constraints { get; set; }
    }
}
