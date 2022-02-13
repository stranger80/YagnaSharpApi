using System;
using System.Collections.Generic;
using System.Text;

namespace YagnaSharpApi.Entities.Events
{
    public class DebitNoteEventEntity : EventEntity
    {
        public DebitNoteEntity DebitNote { get; set; }
    }
}
