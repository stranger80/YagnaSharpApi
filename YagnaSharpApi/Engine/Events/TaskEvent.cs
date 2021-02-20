using System;
using System.Collections.Generic;
using System.Text;

namespace YagnaSharpApi.Engine.Events
{
    public class TaskEvent : ExecutorEvent
    {
        public string TaskId { get; set; }
    }
}
