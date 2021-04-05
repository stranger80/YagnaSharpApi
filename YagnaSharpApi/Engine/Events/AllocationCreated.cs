﻿using System;
using System.Collections.Generic;
using System.Text;

namespace YagnaSharpApi.Engine.Events
{
    public class AllocationCreated : Event
    {
        public AllocationCreated(string allocationId)
        {
            this.AllocationId = allocationId;
        }

        public string AllocationId { get; set; }
    }
}
