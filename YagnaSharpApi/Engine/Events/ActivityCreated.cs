using System;
using System.Collections.Generic;
using System.Text;
using YagnaSharpApi.Entities;

namespace YagnaSharpApi.Engine.Events
{
    public class ActivityCreated : ActivityEvent
    {
        public ActivityCreated(AgreementEntity agreement, ActivityEntity activity) : base(agreement, activity)
        {
        }
    }
}
