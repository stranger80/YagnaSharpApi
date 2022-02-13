using System;
using System.Collections.Generic;
using System.Text;

namespace YagnaSharpApi.Engine.Events
{
    public class ExecutorEvent : Event
    {
        public ExecutorEvent() : base()
        {

        }

        public ExecutorEvent(Exception exc) : base(exc)
        {

        }
    }
}
