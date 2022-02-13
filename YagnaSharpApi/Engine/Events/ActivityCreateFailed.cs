using System;
using System.Collections.Generic;
using System.Text;
using YagnaSharpApi.Entities;

namespace YagnaSharpApi.Engine.Events
{
    public class ActivityCreateFailed : AgreementEvent, IActivityEvent
    {
        public ActivityCreateFailed(AgreementEntity agreement, Exception exc = null) : base(agreement, exc)
        {
        }

    }
}
