using System;
using System.Collections.Generic;
using System.Text;

namespace YagnaSharpApi.Engine.Events
{
    public class NoProposalsConfirmed : Event
    {
        public int NumOffers { get; set; }
        public DateTime Timeout { get; set; }
    }
}
