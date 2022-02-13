using System;
using System.Collections.Generic;
using System.Text;
using YagnaSharpApi.Entities;

namespace YagnaSharpApi.Engine.Events
{
    public class AgreementTerminated : AgreementEvent, IMarketEvent
    {
        public AgreementTerminated(AgreementEntity agreement) : base(agreement)
        { }

        public string Reason { get; set; }
    }
}
