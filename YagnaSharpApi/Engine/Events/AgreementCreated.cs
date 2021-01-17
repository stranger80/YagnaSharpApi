using System;
using System.Collections.Generic;
using System.Text;

namespace YagnaSharpApi.Engine.Events
{
    public class AgreementCreated : AgreementEvent
    {
        public string ProviderId { get; set; }
    }
}
