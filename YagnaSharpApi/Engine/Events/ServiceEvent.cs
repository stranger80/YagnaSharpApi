using System;
using System.Collections.Generic;
using System.Text;
using YagnaSharpApi.Entities;

namespace YagnaSharpApi.Engine.Events
{
    public class ServiceEvent : ActivityEvent
    {
        public ServiceEvent(AgreementEntity agreement, ActivityEntity activity, ServiceBase service) : base(agreement, activity)
        {
            this.Service = service;
        }

        public ServiceBase Service { get; set; }
    }
}
