using System;
using System.Collections.Generic;
using System.Text;

namespace YagnaSharpApi.Engine.Events
{
    public class AllocationFailed : ExecutorEvent, IPaymentEvent
    {
        public AllocationFailed(Exception exc) : base(exc)
        {
        }
    }
}
