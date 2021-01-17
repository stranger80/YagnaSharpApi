using System;
using System.Collections.Generic;
using System.Text;

namespace YagnaSharpApi.Engine.Events
{
    public class AgreementTerminated : AgreementEvent
    {
        public string Reason { get; set; }
    }
}
