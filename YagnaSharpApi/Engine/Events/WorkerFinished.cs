using System;
using System.Collections.Generic;
using System.Text;
using YagnaSharpApi.Entities;

namespace YagnaSharpApi.Engine.Events
{
    public class WorkerFinished : ActivityEvent
    {
        public WorkerFinished(AgreementEntity agreement, ActivityEntity activity, Exception exc = null) : base(agreement, activity, exc)
        {
        }


    }
}
