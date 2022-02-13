using System;
using System.Collections.Generic;
using System.Text;
using YagnaSharpApi.Entities;

namespace YagnaSharpApi.Engine.Events
{
    public class AgreementEvent : ExecutorEvent // note that AgreementEvent consciously isn't marked as IMarketEvent, as it has descendantsfrom both Market and Payment areas
    {
        public AgreementEvent(AgreementEntity agreement, Exception exc = null) : base(exc)
        {
            this.Agreement = agreement;
        }

        public AgreementEntity Agreement { get; set; }
    }
}
