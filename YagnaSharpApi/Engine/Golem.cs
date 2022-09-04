using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using YagnaSharpApi.Engine.Commands;
using YagnaSharpApi.Engine.Events;
using YagnaSharpApi.Engine.MarketStrategy;
using YagnaSharpApi.Entities;
using YagnaSharpApi.Utils;

namespace YagnaSharpApi.Engine
{
    public class Golem : Engine, IDisposable
    {
        public Golem(decimal budget, string subnetTag, ApiConfiguration config = default, IMarketStrategy marketStrategy = null) :
            base(budget, subnetTag, config, marketStrategy)
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TData"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="worker"></param>
        /// <param name="data"></param>
        /// <param name="payload"></param>
        /// <param name="maxWorkers"></param>
        /// <param name="timeout"></param>
        /// <param name="budget"></param>
        /// <returns></returns>        
        public async IAsyncEnumerable<GolemTask<TData, TResult>> ExecuteTasksAsync<TData, TResult>(
            Func<WorkContext, IAsyncEnumerable<GolemTask<TData, TResult>>, IAsyncEnumerable<Script>> worker,
            IEnumerable<GolemTask<TData, TResult>> data,
            IPackage payload,
            int? maxWorkers = null,
            int? timeout = null,
            decimal? budget = null)
        {
            using (var executor = new Executor(payload, maxWorkers ?? 1, budget ?? this.Budget, timeout ?? 360, this.SubnetTag, this.Configuration, this.MarketStrategy, this))
            {
                executor.OnExecutorEvent += Executor_OnExecutorEvent;
                try
                {
                    await foreach (var result in executor.SubmitAsync(worker, data))
                        yield return result;
                }
                finally
                {
                    executor.OnExecutorEvent -= Executor_OnExecutorEvent;
                }

            }
        }

        public async Task<Cluster<TService>> RunServicesAsync<TService>(int numInstances = 1, DateTime? expiration = null)
            where TService : ServiceBase, new()
        {
            // 1. Create allocations
            var allocations = await this.CreateAllocationsAsync();
            foreach (var allocation in allocations)
            {
                this.Allocations.Add(allocation);
            }

            // 1.1. Update market strategy settings with accepted payment platforms
            this.MarketStrategy.Conditions.PaymentPlatforms = allocations.Select(alloc => alloc.PaymentPlatform).ToList();

            // 2. Create new cluster
            var cluster = new Cluster<TService>(this, numInstances, expiration);

            var dummyInstance = new TService();

            var payload = dummyInstance.GetPayload();

            var builder = await this.CreateDemandBuilderAsync(cluster.Expiration, payload);

            // 3. Start the FindOffers thread
            var findOffersTask = Task.Run(() => this.FindOffersAsync(builder, this.cancellationTokenSource.Token));

            // 4. Start ProcessInvoices thread (with cancellation token!)
            var processDebitNotesTask = Task.Run(() => this.ProcessDebitNotesAsync(this.cancellationTokenSource.Token));

            // 5. Start ProcessInvoices thread (with cancellation token!)
            var processInvoicesTask = Task.Run(() => this.ProcessInvoicesAsync(this.cancellationTokenSource.Token));

            cluster.SpawnInstances();

            return cluster;
        }

        public async Task<(AgreementEntity, ActivityEntity)> RunPayloadAsync(IPackage payload, DateTime? expiration = null)
        {
            // 1. Create allocations
            var allocations = await this.CreateAllocationsAsync();
            foreach (var allocation in allocations)
            {
                this.Allocations.Add(allocation);
            }

            // 1.1. Update market strategy settings with accepted payment platforms
            this.MarketStrategy.Conditions.PaymentPlatforms = allocations.Select(alloc => alloc.PaymentPlatform).ToList();

            // 2. Create demand builder
            var builder = await this.CreateDemandBuilderAsync(expiration.Value, payload);

            // 3. Start the FindOffers thread
            var findOffersTask = Task.Run(() => this.FindOffersAsync(builder, this.cancellationTokenSource.Token));

            // 4. Start ProcessInvoices thread (with cancellation token!)
            var processDebitNotesTask = Task.Run(() => this.ProcessDebitNotesAsync(this.cancellationTokenSource.Token));

            // 5. Start ProcessInvoices thread (with cancellation token!)
            var processInvoicesTask = Task.Run(() => this.ProcessInvoicesAsync(this.cancellationTokenSource.Token));

            // 6. Obtain Agreement and start Activity
            AgreementEntity agreement = null;
            ActivityEntity activity = null;
            bool spawned = false;

            // This is the worker code which will be executed within the AgreementPool
            // its main purpose is to "extract" the agreement and activity entities...
            async Task StartWorker(AgreementEntity agreementParam)
            {
                agreement = agreementParam;

                try
                {
                    activity = await this.CreateActivityAsync(agreement);
                }
                catch (Exception exc)
                {
                    this.Executor_OnExecutorEvent(this, new ActivityCreateFailed(agreement, exc));
                    throw;
                }

                this.Executor_OnExecutorEvent(this, new ActivityCreated(agreement, activity));

                // TODO rethink how the Payment for Agreement should be accepted
                this.AcceptDebitNotesForAgreement(agreement.AgreementId);
                await this.AcceptPaymentForAgreement(agreement.AgreementId);

                spawned = true;

            }

            var task = await this.AgreementPool.UseAgreementAsync(bufferedAgreement =>
                             StartWorker(bufferedAgreement.Agreement));

            // execute the StartWorker() within the AgreementPool context...
            Task.Run(async () =>
                        {
                            try
                            {
                                await task;
                            }
                            catch (Exception exc)
                            {
                                if (agreement != null)
                                {
                                    this.Executor_OnExecutorEvent(this, new WorkerFinished(agreement, null, exc));
                                }
                            }
                        }
            );

            while (!spawned)
            {
                Thread.Sleep(1000);
            }

            return (agreement, activity);
        }


        private void Executor_OnExecutorEvent(object sender, Events.Event e)
        {
            this.ForwardExecutorEvent(sender, e);
        }

        protected override async void Dispose(bool disposing)
        {
            try
            {
                foreach (var alloc in this.Allocations)
                {
                    await this.PaymentRepository.ReleaseAllocationAsync(alloc.AllocationId);
                    this.ForwardExecutorEvent(this, new AllocationReleased(alloc.AllocationId));

                }
            }
            finally
            {
                base.Dispose(disposing);
            }
        }
    }
}
