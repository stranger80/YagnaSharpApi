using System;
using System.Collections.Generic;
using System.Text;
using YagnaSharpApi.Entities;

namespace YagnaSharpApi.Engine.Events
{
    public class DebitNoteAccepted : DebitNoteEvent
    {
        public DebitNoteAccepted(AgreementEntity agreement, DebitNoteEntity debitNote) : base(agreement, debitNote)
        {
        }
    }
}
