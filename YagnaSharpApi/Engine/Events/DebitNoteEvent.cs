using System;
using System.Collections.Generic;
using System.Text;
using YagnaSharpApi.Entities;

namespace YagnaSharpApi.Engine.Events
{
    public class DebitNoteEvent : AgreementEvent, IPaymentEvent
    {
        public DebitNoteEvent(AgreementEntity agreement, DebitNoteEntity debitNote) : base(agreement)
        {
            this.DebitNote = debitNote;
        }

        public DebitNoteEntity DebitNote { get; set; }
    }
}
