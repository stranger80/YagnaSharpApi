using System;
using System.Collections.Generic;
using System.Text;

namespace YagnaSharpApi.Engine.Events
{
    public class AllocationFailed : Event
    {
        public AllocationFailed(Exception exc)
        {
            this.Exception = exc;
        }

        public Exception Exception { get; private set; }
    }
}
