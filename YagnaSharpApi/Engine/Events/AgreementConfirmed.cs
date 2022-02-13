using System;
using System.Collections.Generic;
using System.Text;
using YagnaSharpApi.Entities;

namespace YagnaSharpApi.Engine.Events
{
    public class AgreementConfirmed : AgreementEvent, IMarketEvent
    {
        public  AgreementConfirmed(AgreementEntity agreement) : base(agreement)
        {
        }
    }
}
