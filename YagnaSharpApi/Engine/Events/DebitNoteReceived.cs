using System;
using System.Collections.Generic;
using System.Text;
using YagnaSharpApi.Entities;

namespace YagnaSharpApi.Engine.Events
{
    public class DebitNoteReceived : DebitNoteEvent
    {
        public DebitNoteReceived(AgreementEntity agreement, DebitNoteEntity debitNote) : base(agreement, debitNote)
        {
        }
    }
}
