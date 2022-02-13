using System;
using System.Collections.Generic;
using System.Text;
using YagnaSharpApi.Entities;

namespace YagnaSharpApi.Engine.Events
{
    public class AgreementRejected : AgreementEvent, IMarketEvent
    {
        public AgreementRejected(AgreementEntity agreement) : base(agreement)
        {
        }
    }
}
