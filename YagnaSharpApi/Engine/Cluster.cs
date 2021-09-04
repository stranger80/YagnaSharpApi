using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using YagnaSharpApi.Engine.Commands;
using YagnaSharpApi.Engine.Events;
using YagnaSharpApi.Entities;

namespace YagnaSharpApi.Engine
{
    public class Cluster<TService> where TService : ServiceBase, new()
    {
        public const int DEFAULT_SERVICE_EXPIRATION = 20; // in minutes

        /// <summary>
        /// Counter of Cluster Id values
        /// </summary>
        protected static int clusterIds = 0;

        /// <summary>
        /// Counter of Task Ids
        /// </summary>
        protected static int taskIds = 0;

        public int Id { get; set; } = clusterIds++; //Assign cluster Id

        public Engine Engine { get; set; }
        public int NumInstances { get; set; }
        public DateTime Expiration { get; set; }

        public IList<Task> InstanceTasks { get; set; } = new List<Task>();

        /// <summary>
        /// Collection of service instances
        /// </summary>
        public IList<TService> Instances { get; set; } = new List<TService>();

        public event EventHandler<Event> OnClusterEvent;


        public Cluster(Engine engine, int numInstances, DateTime? expiration = null)
        {
            this.Engine = engine;
            this.NumInstances = numInstances;
            this.Expiration = expiration ?? DateTime.Now.AddMinutes(DEFAULT_SERVICE_EXPIRATION);
        }

        public void SpawnInstances()
        {
            for (int i = 0; i < this.NumInstances; i++)
            {
                var task = Task.Run(async () => await this.SpawnInstanceAsync());
                this.InstanceTasks.Add(task);
            }
        }

        protected async Task SpawnInstanceAsync()
        {
            bool spawned = false;
            string agreementId = null;

            async Task StartWorker(AgreementEntity agreement)
            {
                ActivityEntity activity = null;

                agreementId = agreement.AgreementId;

                this.OnClusterEvent?.Invoke(this, new WorkerStarted(agreement.AgreementId));
                try
                {
                    activity = await this.Engine.CreateActivityAsync(agreement);
                }
                catch(Exception exc)
                {
                    this.OnClusterEvent?.Invoke(this, new ActivityCreateFailed(agreement.AgreementId, exc));
                    throw;
                }

                spawned = true;

                this.OnClusterEvent?.Invoke(this, new ActivityCreated(agreement.AgreementId, activity.ActivityId));

                this.Engine.AcceptDebitNotesForAgreement(agreementId);

                var workContext = new WorkContext($"worker-{activity.ActivityId}", this.Engine.StorageProvider, new NodeInfo(agreement));

                var taskId = $"{this.Id}:{Cluster<TService>.taskIds++}";

                this.OnClusterEvent?.Invoke(this, new TaskStarted<object>(agreement.AgreementId, taskId, null));

                try
                {
                    var instance = new TService();
                    instance.ProviderName = workContext.ProviderName;
                    this.Instances.Add(instance);

                    await instance.ExecuteLifecycleAsync(this.Engine, agreement, activity, workContext, taskId);

                    // TODO TaskFinished event
                    this.OnClusterEvent?.Invoke(this, new WorkerFinished(agreement.AgreementId));
                }
                finally
                {
                    await this.Engine.AcceptPaymentForAgreement(agreementId);
                    // TODO await this.Engine.AgreementPool.ReleaseAgreement(agreementId);
                }

            }
        
            while(!spawned)
            {
                // TODO not sure why this wait was put here???
                Thread.Sleep(1000);

                var task = await this.Engine.AgreementPool.UseAgreementAsync(bufferedAgreement =>
                    StartWorker(bufferedAgreement.Agreement));

                if(task == null)
                {
                    continue;
                }

                try
                {
                    await task;
                }
                catch(Exception exc)
                {
                    if (agreementId != null)
                    {
                        this.OnClusterEvent?.Invoke(this, new WorkerFinished(agreementId, exc));
                    }
                    else
                    {
                        // TODO log - this should not happen
                        return;
                    }
                }
            }
        }
    
    }
}
