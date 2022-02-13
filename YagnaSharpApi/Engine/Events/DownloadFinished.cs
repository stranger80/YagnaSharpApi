using Golem.ActivityApi.Client.Model;
using System;
using System.Collections.Generic;
using System.Text;
using YagnaSharpApi.Entities;

namespace YagnaSharpApi.Engine.Events
{
    public class DownloadFinished : CommandEvent
    {
        public DownloadFinished(AgreementEntity agreement, ActivityEntity activity, Script script) : base(agreement, activity, script)
        {
        }
    }
}
