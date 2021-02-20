using System;
using System.Collections.Generic;
using System.Text;

namespace YagnaSharpApi.Engine.Events
{
    public class ActivityCreated : ExecutorEvent
    {
        public ActivityCreated(string agreementId, string activityId)
        {
            this.AgreementId = agreementId;
            this.ActivityId = activityId;
        }

        public string AgreementId { get; set; }
        public string ActivityId { get; set; }
    }
}
