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
                return this.Event.GetType().Name;
            }
        }
        public string Timestamp { 
            get
            {
                return this.Event.EventDate.ToString("yyyy-MM-dd HH:mm:ss.fff");
            }
        }
        public Event Event { get; set; }

        public EventModel(Event e)
        {
            this.Event = e;
        }

    }
}
