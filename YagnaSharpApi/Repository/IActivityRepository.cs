using AutoMapper;
using Golem.ActivityApi.Client.Model;
using Golem.PaymentApi.Client.Api;
using Golem.PaymentApi.Client.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using YagnaSharpApi.Entities;
using YagnaSharpApi.Entities.Events;
using YagnaSharpApi.Utils;

namespace YagnaSharpApi.Repository
{
    public interface IActivityRepository : IDisposable
    {
        Task<ActivityEntity> CreateActivityAsync(AgreementEntity agreement, float? timeout = 0.0f);
        
        Task<ActivityEntity> GetActivityAsync(string activityId);

        Task<ExeScriptBatchEntity> ExecAsync(ActivityEntity activity, IEnumerable<ExeScriptCommand> commands);

        IAsyncEnumerable<ExeScriptCommandResult> GetBatchEventsAsync(ExeScriptBatchEntity batch, CancellationToken cancellationToken = default);

        Task DestroyActivityAsync(string activityId);

    }
}
