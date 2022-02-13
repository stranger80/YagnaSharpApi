using System;
using System.Collections.Generic;
using System.Text;
using YagnaSharpApi.Entities;

namespace YagnaSharpApi.Engine.Events
{
    public class WorkerStarted : AgreementEvent
    {
        public WorkerStarted(AgreementEntity agreement) : base(agreement)
        {
        }
    }
}
