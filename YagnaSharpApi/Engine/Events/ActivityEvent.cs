using System;
using System.Collections.Generic;
using System.Text;
using YagnaSharpApi.Entities;

namespace YagnaSharpApi.Engine.Events
{
    public class ActivityEvent : AgreementEvent, IActivityEvent
    {
        public ActivityEvent(AgreementEntity agreement, ActivityEntity activity, Exception exc = default(Exception)) : base(agreement, exc)
        {
            this.Activity = activity;
        }

        public ActivityEntity Activity { get; set; }
    }
}
