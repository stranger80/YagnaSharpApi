﻿using System;
using System.Collections.Generic;
using System.Text;

namespace YagnaSharpApi.Engine.Events
{
    public class SubscriptionFailed : MarketEvent
    {
        public string Reason { get; set; }
    }
}
