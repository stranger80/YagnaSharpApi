﻿using System;
using System.Collections.Generic;
using System.Text;

namespace YagnaSharpApi.Entities.Events
{
    public class InvoiceEventEntity : EventEntity
    {
        public InvoiceEntity Invoice { get; set; }
    }
}
