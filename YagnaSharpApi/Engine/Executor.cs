using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using YagnaSharpApi.Engine.Commands;
using YagnaSharpApi.Engine.Events;
using YagnaSharpApi.Engine.MarketStrategy;
using YagnaSharpApi.Entities;
using YagnaSharpApi.Entities.Events;
using YagnaSharpApi.Exceptions;
using YagnaSharpApi.Mapper;
using YagnaSharpApi.Repository;
using YagnaSharpApi.Storage;
using YagnaSharpApi.Utils;
using Golem.ActivityApi.Client.Model;

namespace YagnaSharpApi.Engine
{
    [Obsolete]
    public class Executor : IDisposable
    {
        private bool disposedValue;
        private CancellationTokenSource cancellationTokenSource;
                
        public IPackage Package { get; set; }
        public IMarketStrategy MarketStrategy { get; set; }
        public StorageProvider StorageProvider { get; set; }
        public AgreementPool AgreementPool { get; set; }
        public IMarketRepository MarketRepository { get; set; }
        public IActivityRepository ActivityRepository { get; set; }
        public IPaymentRepository PaymentRepository { get; set; }

        protected Engine Engine;
        /// <summary>
        /// Indicates if the Engine has been created by Executor (in which case it must also be disposed by Executor)
        /// </summary>
        protected bool shouldDisposeEngine = false; 
        protected IDictionary<string, InvoiceEntity> InvoicesByAgreementId = new ConcurrentDictionary<string, InvoiceEntity>();
        protected HashSet<string> AgreementsToPay = new HashSet<string>();
        protected ConcurrentBag<AllocationEntity> Allocations = new ConcurrentBag<AllocationEntity>();
        protected List<Task> Workers = new List<Task>();

        public ApiConfiguration Configuration { get; set; }

        public decimal Budget { get; set; }
        public DateTime Expires { get; set; }
        public string SubnetTag { get; set; }

        public event EventHandler<Event> OnExecutorEvent;


        private int workerId = 0;
        private int maxWorkers = 0;


        static Executor()
        {
            MapConfig.Init();
        }




        public Executor(Engine engine, IPackage package, int maxWorkers, decimal budget, int timeout, string subnetTag, ApiConfiguration config = default, IMarketStrategy marketStrategy = null)
            : this(package, maxWorkers, budget, timeout, subnetTag, config, marketStrategy, engine)
        {
            
        }


        /// <summary>
        /// Obsolete constructor
        /// </summary>
        /// <param name="package"></param>
        /// <param name="maxWorkers"></param>
        /// <param name="budget"></param>
        /// <param name="timeout">in seconds</param>
        /// <param name="subnetTag"></param>
        /// <param name="marketStrategy"></param>
        public Executor(IPackage package, int maxWorkers, decimal budget, int timeout, string subnetTag, ApiConfiguration config = default, IMarketStrategy marketStrategy = null, Engine engine = null)
        {
            this.Configuration = config ?? new ApiConfiguration();
            var apiFactory = new ApiFactory(this.Configuration);
            var mapper = Mapper.MapConfig.Config.CreateMapper();

            this.Package = package;

            this.MarketRepository = new MarketRepository(apiFactory.GetMarketRequestorApi(), mapper);
            this.ActivityRepository = new ActivityRepository(apiFactory.GetActivityRequestorControlApi(), mapper);
            this.PaymentRepository = new PaymentRepository(apiFactory.GetPaymentRequestorApi(), mapper);

            this.MarketStrategy = marketStrategy ?? new DummyMarketStrategy(this.MarketRepository, new MarketStrategyConditions() );

            this.StorageProvider = new GftpProvider();
            this.AgreementPool = new AgreementPool();

            this.Budget = budget;
            this.Expires = DateTime.UtcNow.AddSeconds(timeout);

            this.cancellationTokenSource = new CancellationTokenSource();

            this.maxWorkers = maxWorkers;
            this.SubnetTag = subnetTag;

            if (engine == null)
            {
                // TODO it is deprecated to use the Executor as standalone
                this.Engine = new Engine(budget, subnetTag, config, marketStrategy);
            }
            else
            {
                this.Engine = engine;
            }

            // TODO setup event handlers for all the above
            this.MarketStrategy.OnMarketEvent += MarketStrategy_OnMarketEvent;
            this.AgreementPool.OnAgreementEvent += AgreementPool_OnAgreementEvent; 
        }

        private void AgreementPool_OnAgreementEvent(object sender, AgreementEvent e)
        {
            this.OnExecutorEvent?.Invoke(this, e);
        }

        private void MarketStrategy_OnMarketEvent(object sender, Event e)
        {
            this.OnExecutorEvent?.Invoke(this, e);
        }

        public async IAsyncEnumerable<GolemTask<TData, TResult>> SubmitAsync<TData, TResult>(Func<WorkContext, 
                                                                                                  IAsyncEnumerable<GolemTask<TData, TResult>>, 
                                                                                                  IAsyncEnumerable<Script>> worker, 
                                                                                             IEnumerable<GolemTask<TData, TResult>> data)
        {
            this.OnExecutorEvent?.Invoke(this, new SubmitStarted());

            using (var smartQueue = new SmartQueue<TData, TResult>(2)) // TODO make the retry count configurable
            using (var doneTasks = new BlockingCollection<GolemTask<TData, TResult>>())
            {
                var inputTasks = new List<GolemTask<TData, TResult>>();

                foreach (var inputTask in data)
                {
                    // queue task in smart queue
                    smartQueue.QueueTask(inputTask);
                    inputTask.Queue = smartQueue;

                    inputTask.OnTaskComplete += (sender, ev) => {
                        if (ev is TaskAccepted<TResult>)
                        {
                            doneTasks.Add((GolemTask<TData, TResult>)sender);
                        }
                        this.OnExecutorEvent?.Invoke(sender, ev);
                    };

                    inputTasks.Add(inputTask);
                }

                // 1. Create Allocations
                var allocations = await this.Engine.CreateAllocationsAsync();

                if (!allocations.Any())
                    throw new Exception("No payment accounts. Did you forget to run 'yagna payment init --sender'?");

                foreach (var alloc in allocations)
                {
                    this.Allocations.Add(alloc);
                }

                // 1.1. Update market strategy settings with accepted payment platforms
                this.MarketStrategy.Conditions.PaymentPlatforms = allocations.Select(alloc => alloc.PaymentPlatform).ToList();

                // 2. Build Demand

                DemandBuilder demand = new DemandBuilder();

                demand.Add(Properties.SRV_COMP_EXPIRATION, this.Expires);
                demand.Add(Properties.SRV_COMP_CAPS_MULTI_ACTIVITY, true);
                // make sure to include subnet
                if (this.SubnetTag != null)
                {
                    demand.Add(Properties.NODE_DEBUG_SUBNET, this.SubnetTag);
                    demand.Ensure($"({Properties.NODE_DEBUG_SUBNET}={this.SubnetTag})");
                }

                await this.Package.DecorateDemandAsync(demand);
                await this.PaymentRepository.DecorateDemandAsync(allocations, demand);

                // note MarketStrategy does its own decorations on top of the above

                // 3. Create GFTP storage provider
                this.StorageProvider = new GftpProvider();

                // 4. Start FindOffers thread (with cancellation token!)
                var findOffersTask = Task.Run(() => FindOffersAsync(demand, this.cancellationTokenSource.Token));

                // 5. Start ProcessInvoices thread (with cancellation token!)
                var processInvoicesTask = Task.Run(() => this.Engine.ProcessInvoicesAsync(this.cancellationTokenSource.Token));

                // 6. Start the Worker Starter thread on input task collection (wrapped so that a completed task adds itself to the "doneQueue") 
                // (with cancellation token!)
                var workerStarterTask = Task.Run(() => WorkerStarter(worker, inputTasks, smartQueue, this.cancellationTokenSource.Token));

                var waitUntilDoneTask = Task.Run(() => smartQueue.WaitUntilDone());

                var runningTasks = new List<Task>()
                {
                    findOffersTask,
                    processInvoicesTask,
                    workerStarterTask,
                    waitUntilDoneTask,
                    // TODO processDebitNotesTask
                };

                try
                {
                    Task<GolemTask<TData, TResult>> getDoneTask = null;

                    while (!waitUntilDoneTask.IsCompleted || !(doneTasks.Count == 0))
                    {
                        var now = DateTime.UtcNow;

                        if (now > this.Expires)
                        {
                            throw new Exception($"task timeout exceeded. timeout={this.Configuration.TaskTimeout}");
                        }
                        //if(now > GetOffersDeadline && proposalsConfirmed == 0)
                        //{
                        //    // TODO raise NoProposalsConfirmed event
                        //}

                        if (getDoneTask == null)
                        {
                            getDoneTask = Task.Run(() => doneTasks.Take(this.cancellationTokenSource.Token));
                            runningTasks.Add(getDoneTask);
                        }

                        var runningTasksArray = runningTasks.Union(this.Workers).Where(item => item != null).ToArray();

                        var completedTaskIndex = Task.WaitAny(runningTasksArray, 1000, this.cancellationTokenSource.Token);

                        if (completedTaskIndex != -1)
                        {
                            var completedTasks = runningTasksArray.Where(task => task.IsCompleted);

                            foreach (var completedTask in completedTasks)
                            {
                                if (this.Workers.Contains(completedTask))
                                {
                                    try
                                    {
                                        System.Diagnostics.Debug.WriteLine($"Waiting to complete Worker task: {completedTask.Id}");
                                        await completedTask;
                                        System.Diagnostics.Debug.WriteLine($"Completed Worker task: {completedTask.Id}");
                                    }
                                    catch (Exception exc)
                                    {
                                        // This is where exceptions raised in StartWorker() will be caught
                                        System.Diagnostics.Debug.WriteLine(exc);
                                    }
                                }

                                System.Diagnostics.Debug.WriteLine($"Removing task: {completedTask.Id}");
                                this.Workers.Remove(completedTask);
                                runningTasks.Remove(completedTask);
                            }

                            if (getDoneTask.IsCompleted)
                            {
                                yield return getDoneTask.Result;
                                getDoneTask = null;
                            }
                        }

                    }

                    this.OnExecutorEvent?.Invoke(this, new ComputationFinished());

                    try
                    {
                        await this.AgreementPool.TerminateAsync(new ReasonEntity() { Message = "Successfully finished all work" });
                    }
                    catch (Exception exc)
                    {
                        System.Diagnostics.Debug.WriteLine(exc);
                    }

                    // TODO wait until all work has completed (eg. all invoices paid)

                    while (this.AgreementsToPay.Any())
                    {
                        await Task.Delay(1000);
                    }

                    System.Diagnostics.Debug.WriteLine("All agreements paid...");
                }
                finally
                {
                    foreach (var alloc in this.Allocations)
                    {
                        await this.PaymentRepository.ReleaseAllocationAsync(alloc.AllocationId);
                        this.OnExecutorEvent?.Invoke(this, new AllocationReleased(alloc.AllocationId));

                    }

                }
            }

            this.OnExecutorEvent?.Invoke(this, new SubmitFinished());
        }

        /// <summary>
        /// Runs a service of type specified by generic parameter, 
        /// in a number of instances indicated by numInstances param.
        /// 
        /// 
        /// </summary>
        /// <typeparam name="Service"></typeparam>
        /// <param name="numInstances"></param>
        /// <returns>Collection of Instances of the Service class which had been launched.</returns>
        public async IAsyncEnumerable<Service> RunServiceAsync<Service>(int numInstances = 1) 
            where Service : ServiceBase, new()
        {
            // TODO : handle the long running service execution
            for (int i = 0; i < numInstances; i++)
            {
                var service = new Service();

                yield return service;
            }
        }

        /// <summary>
        /// Logic to subscribe the Demand on the market and process incoming Offer proposals
        /// NOTE: this also includes "negotiation" of the payment platform.
        /// </summary>
        /// <param name="repo"></param>
        /// <param name="demand"></param>
        /// <returns></returns>
        protected async Task FindOffersAsync(DemandBuilder demand, CancellationToken cancellationToken /* TODO pass objects to collect stats */)
        {
            int iterCount = 0;
            
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    System.Diagnostics.Debug.WriteLine($"Starting {iterCount++} Demand Subscription...");

                    await foreach (var prop in this.MarketStrategy.FindOffersAsync(demand, cancellationToken))
                    {
                        System.Diagnostics.Debug.WriteLine("Adding Proposal to AgreementPool...");
                        // Add to AgreementPool
                        this.AgreementPool.AddProposal(prop.Item1, prop.Item2);
                        System.Diagnostics.Debug.WriteLine("Added Proposal to AgreementPool...");
                    }
                }
                catch (Exception exc)
                {
                    // TODO how to handle this???
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected async Task WorkerStarter<TData, TResult>(Func<WorkContext, IAsyncEnumerable<GolemTask<TData, TResult>>, IAsyncEnumerable<Script>> worker, IEnumerable<GolemTask<TData, TResult>> data, SmartQueue<TData, TResult> smartQueue, CancellationToken cancellationToken)
        {
            System.Diagnostics.Debug.WriteLine("Starting WorkerStarter...");
            do
            {
                while (this.Workers.Count < this.maxWorkers)
                {
                    System.Diagnostics.Debug.WriteLine("Queueing new Task for exec...");
                    Task newTask = null;

                    try
                    {
                        newTask = await this.AgreementPool.UseAgreementAsync(bufferedAgreement =>
                            this.DoWork(bufferedAgreement, worker, data, smartQueue)
                            );

                        if (newTask != null)
                            this.Workers.Add(newTask);
                    }
                    catch (Exception exc)
                    {
                        // TODO should we do something to abort the new task?

                    }

                }
            }
            while (!cancellationToken.IsCancellationRequested && !smartQueue.AreAllTasksProcessed());

            System.Diagnostics.Debug.WriteLine("Finishing WorkerStarter...");
        }

        /// <summary>
        /// 'Container' class required to track the 'last task'
        /// </summary>
        /// <typeparam name="TObject"></typeparam>
        protected class Box<TObject>
        {
            public TObject Item { get; set; } = default(TObject);
        }

        protected async Task DoWork<TData, TResult>(BufferedAgreement bufferedAgreement, Func<WorkContext, IAsyncEnumerable<GolemTask<TData, TResult>>, IAsyncEnumerable<Script>> worker, IEnumerable<GolemTask<TData, TResult>> data, SmartQueue<TData, TResult> smartQueue)
        {
            this.OnExecutorEvent?.Invoke(this, new WorkerStarted(bufferedAgreement.Agreement.AgreementId));

            ActivityEntity activity = null;
            try
            {
                activity = await this.Engine.CreateActivityAsync(bufferedAgreement.Agreement);
            }
            catch (Exception exc)
            {
                this.OnExecutorEvent?.Invoke(this, new ActivityCreateFailed(bufferedAgreement.Agreement.AgreementId, exc));
                this.OnExecutorEvent?.Invoke(this, new WorkerFinished(bufferedAgreement.Agreement.AgreementId));
                throw;
            }

            this.OnExecutorEvent?.Invoke(this, new ActivityCreated(bufferedAgreement.Agreement.AgreementId, activity.ActivityId));

            var workContext = new WorkContext($"worker-{this.workerId++}", this.StorageProvider, new NodeInfo(bufferedAgreement.Agreement));

            // "wrap" the data task collection with logic that for each fetched task will put the task into the queue 
            // that monitors task execution, handles retries, etc. 
            // (Also, record the 'lastTask' so that one can correlate WorkItems with GolemTasks)

            Box<GolemTask<TData, TResult>> lastTask = new Box<GolemTask<TData, TResult>>();

            async IAsyncEnumerable<GolemTask<TData, TResult>> QueueTasks(IEnumerable<GolemTask<TData, TResult>> data, SmartQueue<TData, TResult> queue)
            {
                // now get tasks from smartqueue - it will return next task, 
                // selecting one from either newly queued, or queued for reschedule
                await foreach (var task in queue.GetTaskForExecutionAsync())
                {
                    lastTask.Item = task;
                    yield return task;
                }
            }

            var commandGenerator = worker(workContext, QueueTasks(data, smartQueue));

            if (!await this.ProcessBatchesAsync<TData, TResult>(commandGenerator, bufferedAgreement, activity, workContext, lastTask))
                return;

            System.Diagnostics.Debug.WriteLine($"finishing Worker Task {Task.CurrentId}..");
            await this.Engine.AcceptPaymentForAgreement(bufferedAgreement.Agreement.AgreementId);
            System.Diagnostics.Debug.WriteLine($"Accepted payment in Worker Task {Task.CurrentId}..");

            this.OnExecutorEvent?.Invoke(this, new WorkerFinished(bufferedAgreement.Agreement.AgreementId));
            System.Diagnostics.Debug.WriteLine($"finished Worker Task {Task.CurrentId}..");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TData"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="commandGenerator"></param>
        /// <param name="bufferedAgreement"></param>
        /// <param name="activity"></param>
        /// <param name="lastTaskBox"></param>
        /// <returns>true if finished successfully</returns>
        protected async Task<bool> ProcessBatchesAsync<TData, TResult>(IAsyncEnumerable<Script> commandGenerator, 
            BufferedAgreement bufferedAgreement, ActivityEntity activity, WorkContext workContext, Box<GolemTask<TData, TResult>> lastTaskBox = null)
        {
            bool initializationDone = false;

            await foreach (var batch in commandGenerator)
            {
                // TODO batch deadline logic

                // TODO in yapapi there is here a weird logic that attempts to correlate 
                // the batch of commands created by the worker() delegate with its input GolemTask

                var currentTask = lastTaskBox?.Item;

                try
                {
                    currentTask?.Start();

                    this.OnExecutorEvent?.Invoke(this, new TaskStarted<TData>(bufferedAgreement.Agreement.AgreementId, currentTask?.Id, currentTask != null ? currentTask.Data : default(TData)));

                    // The first batch must either incldue initialization block, or we must initialize
                    if (!initializationDone)
                    {
                        if (!batch.IsInitialized)
                        {
                            await this.PerformImplicitInitAsync(workContext, bufferedAgreement, activity);
                        }
                        initializationDone = true;
                    }

                    await batch.BeforeAsync();

                    var commandsBuilder = new ExeScriptBuilder();
                    batch.Evaluate(commandsBuilder);

                    var commands = commandsBuilder.GetCommands();

                    // If there are any commands to execute...
                    if (commands.Any())
                    {
                        // ...at this point we should have the exescript built by commandBuilder
                        var commandResults = activity.ExecAsync(commands);

                        this.OnExecutorEvent?.Invoke(this, new ScriptSent(bufferedAgreement.Agreement.AgreementId, currentTask?.Id, commands));

                        await foreach (var result in commandResults)
                        {
                            batch.StoreResult(result);

                            // TODO raise command executed event
                            if (result.Result == ExeScriptCommandResult.ResultEnum.Error)
                                throw new CommandExecutionException(commands[result.Index], result);
                        }

                        this.OnExecutorEvent?.Invoke(this, new GettingResults(bufferedAgreement.Agreement.AgreementId, currentTask?.Id));
                    }

                    await batch.AfterAsync();
                    this.OnExecutorEvent?.Invoke(this, new ScriptFinished(bufferedAgreement.Agreement.AgreementId, currentTask?.Id));

                    await this.Engine.AcceptPaymentForAgreement(bufferedAgreement.Agreement.AgreementId, true);

                }
                catch (CommandExecutionException exc) // in case of command error - throw the exception up...
                {
                    currentTask?.Stop(true); // reschedule failed task
                    this.OnExecutorEvent?.Invoke(this, new WorkerFinished(bufferedAgreement.Agreement.AgreementId, exc));
                    throw;
                }
                catch (Exception exc)
                {
                    currentTask?.Stop(true); // reschedule failed task
                    this.OnExecutorEvent?.Invoke(this, new WorkerFinished(bufferedAgreement.Agreement.AgreementId, exc));
                    return false;
                }

                System.Diagnostics.Debug.WriteLine($"Finished executing batch in Worker Task {Task.CurrentId}..");

            }
            return true;

        }

        protected async Task PerformImplicitInitAsync(WorkContext ctx, BufferedAgreement bufferedAgreement, ActivityEntity activity)
        {
            async IAsyncEnumerable<Script> ImplicitInitAsync()
            {
                var script = ctx.NewScript();
                script.Append(new DeployStep());
                script.Append(new StartStep());
                yield return script;
            }
            await this.ProcessBatchesAsync<object, string>(ImplicitInitAsync(), bufferedAgreement, activity, ctx);

        }

        protected AllocationEntity GetAllocationForInvoice(InvoiceEntity invoice)
        {
            var result = this.Allocations
                .Where(alloc => alloc.Address == invoice.PayerAddr && alloc.PaymentPlatform == invoice.PaymentPlatform)
                .FirstOrDefault();

            if (result == null)
                throw new Exception($"No allocation for {invoice.PayerAddr} {invoice.PaymentPlatform}");

            return result;
        }



        #region Dispose

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    if (this.shouldDisposeEngine)
                        this.Engine.Dispose();

                    this.cancellationTokenSource.Cancel();

                    this.MarketRepository.Dispose();
                    this.StorageProvider.Dispose();

                    this.cancellationTokenSource.Dispose();
                }
                disposedValue = true;
            }
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
