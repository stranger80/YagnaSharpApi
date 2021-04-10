using System;
using System.Collections.Generic;
using System.Text;
using YagnaSharpApi.Engine.Events;

namespace YagnaSharpApi.Studio.Model
{
    public class EventModel
    {

        public string Text 
        { 
            get 
            {
                return this.Event.ToString();
            }
        }
        public DateTime Timestamp { 
            get
            {
                return this.Event.EventDate;
            }
        }
        public Event Event { get; set; }

        public EventModel(Event e)
        {
            this.Event = e;
        }

    }
}
