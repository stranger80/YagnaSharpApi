using System;
using System.Collections.Generic;
using System.Text;
using YagnaSharpApi.Entities;

namespace YagnaSharpApi.Engine.Events
{
    public class AgreementEvent : MarketEvent
    {
        public AgreementEntity Agreement { get; set; }
    }
}
