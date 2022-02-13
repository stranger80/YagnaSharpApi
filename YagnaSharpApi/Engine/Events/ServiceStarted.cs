using System;
using System.Collections.Generic;
using System.Text;
using YagnaSharpApi.Entities;

namespace YagnaSharpApi.Engine.Events
{
    public class ServiceStarted : ServiceEvent
    {
        public ServiceStarted(AgreementEntity agreement, ActivityEntity activity, ServiceBase service) : base(agreement, activity, service)
        {
        }
    }
}
