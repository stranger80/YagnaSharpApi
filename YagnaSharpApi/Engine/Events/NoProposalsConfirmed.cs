using System;
using System.Collections.Generic;
using System.Text;

namespace YagnaSharpApi.Engine.Events
{
    public class NoProposalsConfirmed : ExecutorEvent
    {
        public NoProposalsConfirmed(int numOffers, DateTime timeout)
        {
            this.NumOffers = numOffers;
            this.Timeout = timeout;
        }

        public int NumOffers { get; set; }
        public DateTime Timeout { get; set; }
    }
}
