using AutoMapper;
using Golem.ActivityApi.Client.Api;
using Golem.ActivityApi.Client.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using YagnaSharpApi.Entities;
using YagnaSharpApi.Entities.Events;
using YagnaSharpApi.Utils;

namespace YagnaSharpApi.Repository
{
    public class ActivityRepository : IActivityRepository
    {
        private bool disposedValue;

        protected Dictionary<string, ActivityEntity> ActivitiesById = new Dictionary<string, ActivityEntity>();
        public IRequestorControlApi RequestorControlApi { get; set; }
        public IRequestorStateApi RequestorStateApi { get; set; }
        public IMapper Mapper { get; set; }

        public ActivityRepository(IRequestorControlApi requestorControlApi, IRequestorStateApi requestorStateApi, IMapper mapper)
        {
            this.RequestorControlApi = requestorControlApi;
            this.RequestorStateApi = requestorStateApi;
            this.Mapper = mapper;
        }

        public async Task<ActivityEntity> CreateActivityAsync(AgreementEntity agreement, float? timeout = 0.0f)
        {
            try
            {
                var request = new CreateActivityRequest(agreement.AgreementId);

                var response = await this.RequestorControlApi.CreateActivityAsync(request, timeout);

                var result = new ActivityEntity(this)
                {
                    ActivityId = response.ActivityId
                    // TODO Credentials, requird for SGX activities
                };

                return result;
            }
            catch (Exception exc)
            {
                System.Diagnostics.Debug.WriteLine($"CreateActivity failed for provider: {agreement.Offer.ProviderId}");
                throw;
            }
        }

        public async Task<ActivityEntity> GetActivityAsync(string activityId)
        {
            if (this.ActivitiesById.ContainsKey(activityId))
            {
                return this.ActivitiesById[activityId];
            }

            try
            {
                var activityState = await this.RequestorStateApi.GetActivityStateAsync(activityId);

                var result = new ActivityEntity(this)
                {
                    ActivityId = activityId,
                    State = (YagnaSharpApi.Entities.ActivityEntity.StateEnum)activityState.State[0],
                };

                this.ActivitiesById.Add(activityId, result);

                return result;
            }
            catch (Exception exc)
            {
                throw;
            }
        }

        public async Task<ExeScriptBatchEntity> ExecAsync(ActivityEntity activity, IEnumerable<ExeScriptCommand> commands)
        {
            try
            {
                var request = new ExeScriptRequest(JsonConvert.SerializeObject(commands));

                var batchIdText = await this.RequestorControlApi.ExecAsync(activity.ActivityId, request);

                var batchId = JsonConvert.DeserializeObject<string>(batchIdText);

                var result = new ExeScriptBatchEntity(activity, batchId);

                return result;
            }
            catch (Exception exc)
            {
                throw;
            }
        }

        public async IAsyncEnumerable<ExeScriptCommandResult> GetBatchEventsAsync(ExeScriptBatchEntity batch, [EnumeratorCancellation]CancellationToken cancellationToken = default)
        {
            bool stop = false;
            do
            {
                List<ExeScriptCommandResult> resultList = null;

                try
                {
                    resultList = await this.RequestorControlApi.GetExecBatchResultsAsync(batch.Activity.ActivityId, batch.BatchId);
                }
                catch (Exception exc)
                {
                    throw;
                }

                foreach (var result in resultList)
                {
                    if(result.IsBatchFinished)
                    {
                        stop = true;
                    }

                    yield return result;
                }
            }
            while (!cancellationToken.IsCancellationRequested && !stop);

        }

        public async Task DestroyActivityAsync(string activityId)
        {
            try
            {
                await this.RequestorControlApi.DestroyActivityAsync(activityId);
            }
            catch (Exception exc)
            {
                throw;
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

    }
}
