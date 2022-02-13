using System;
using System.Collections.Generic;
using System.Text;

namespace YagnaSharpApi.Engine.Events
{
    public class Event
    {
        public Event() 
        { 
        }

        public Event(Exception exc)
        {
            this.Exception = exc;
        }

        public DateTime EventDate
        {
            get;
        } = DateTime.Now;

        public Exception Exception { get; protected set; }
    }
}
