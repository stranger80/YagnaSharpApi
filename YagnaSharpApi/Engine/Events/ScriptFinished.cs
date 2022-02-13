using Golem.ActivityApi.Client.Model;
using System;
using System.Collections.Generic;
using System.Text;
using YagnaSharpApi.Engine.Commands;
using YagnaSharpApi.Entities;

namespace YagnaSharpApi.Engine.Events
{
    public class ScriptFinished : ScriptEvent
    {
        public ScriptFinished(AgreementEntity agreement, ActivityEntity activity, Script script = default(Script)) : base(agreement, activity, script)
        {
        }
    }
}
