﻿using Golem.ActivityApi.Client.Model;
using System;
using System.Collections.Generic;
using System.Text;
using YagnaSharpApi.Entities;

namespace YagnaSharpApi.Engine.Events
{
    public class ScriptEvent : ActivityEvent
    {
        public ScriptEvent(AgreementEntity agreement, ActivityEntity activity, Script script) : base(agreement, activity)
        {
            this.Script = script;
        }

        public Script Script { get; set; }
    }
}
