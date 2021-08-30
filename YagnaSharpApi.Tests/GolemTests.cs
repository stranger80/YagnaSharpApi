using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using YagnaSharpApi.Engine;
using YagnaSharpApi.Engine.Commands;
using YagnaSharpApi.Engine.Events;
using YagnaSharpApi.Engine.MarketStrategy;
using YagnaSharpApi.Mapper;
using YagnaSharpApi.Repository;
using YagnaSharpApi.Tests.Services;
using YagnaSharpApi.Utils;

namespace YagnaSharpApi.Tests
{
    [TestClass]
    public class GolemTests
    {
        public TestUtils Utils { get; set; } = new TestUtils();
        public MarketStrategyTests MarketStrategyTests { get; set; } = new MarketStrategyTests();

        public List<Event> EventList { get; set; } = new List<Event>();

        static GolemTests()
        {
            MapConfig.Init();
        }

        protected async Task DoGolemRunTaskAsync<Input, Output>(
            Func<WorkContext, IAsyncEnumerable<GolemTask<Input, Output>>, IAsyncEnumerable<WorkItem>> workerFunc,
            IEnumerable<GolemTask<Input, Output>> data,
            Action assertions,
            int timeoutSeconds = 360)
        {
            var payload = VmRequestBuilder.Repo(
                "d646d7b93083d817846c2ae5c62c72ca0507782385a2e29291a3d376",
                0.5m,
                2.0m
                );


            using (var golem = new Engine.Golem(
                1.0m, 
                TestConstants.SUBNET_TAG))
            {
                golem.OnExecutorEvent += Golem_OnExecutorEvent;
                var inputTasks = data.ToList();

                await foreach (var task in golem.ExecuteTasksAsync(workerFunc, inputTasks, payload))
                {
                    Console.WriteLine($"{TextColorConstants.TEXT_COLOR_CYAN}Task computed: {task}, result: {task.Result}{TextColorConstants.TEXT_COLOR_DEFAULT}");
                }

                assertions();
            }

        }

        [TestMethod]
        public async Task Golem_RunsOneWorkerOneActionTask()
        {
            var acceptedTasks = new List<GolemTask<object, string>>();

            async IAsyncEnumerable<WorkItem> ProcessGolemTasksAsync(WorkContext ctx, IAsyncEnumerable<GolemTask<object, string>> tasks)
            {
                await foreach (var task in tasks)
                {
                    System.Diagnostics.Debug.WriteLine("Starting Task Body...");

                    ctx.Run("/bin/sh", "-c", "date");
                    var workItem = ctx.Commit();
                    yield return workItem;

                    var results = await workItem;

                    // always accept
                    System.Diagnostics.Debug.WriteLine("Accepting Task results...");
                    
                    task.AcceptTask(results[^1]?.Stdout);
                    acceptedTasks.Add(task);
                    System.Diagnostics.Debug.WriteLine("Accepted Task results...");
                }

            }

            // one data set
            var data = Enumerable.Range(0, 1).Select(item => new GolemTask<object, string>(null)).ToList();

            await this.DoGolemRunTaskAsync(ProcessGolemTasksAsync, data, () =>
            {
                Assert.AreEqual(data.Count(), acceptedTasks.Count);
            });
        }

        [TestMethod]
        public async Task Golem_RunsOneService()
        {

            using (var golem = new Engine.Golem(
                1.0m,
                TestConstants.SUBNET_TAG))
            {
                var events = new List<Event>();

                // add event listener to collect events - we will be making assertions on events later
                golem.OnExecutorEvent += (sender, e) => { events.Add(e); };

                var cluster = await golem.RunServicesAsync<DateService>();

                var startDate = DateTime.Now;

                while (DateTime.Now < startDate.AddSeconds(30))
                {
                    foreach (var instance in cluster.Instances)
                        Debug.WriteLine($"Instance {instance.Id} is {instance.State} on {instance.ProviderName}");
                    Thread.Sleep(5000);
                }

                Assert.IsTrue(events.Any(ev => ev is ScriptFinished));
                Assert.IsTrue(
                    (events.LastOrDefault(ev => ev is ScriptFinished) as ScriptFinished)?
                    .CommandBatch
                    .GetResults()
                    .LastOrDefault()?
                    .Stdout != null);

            }

        }


        private void Golem_OnExecutorEvent(object sender, Event e)
        {
            switch(e)
            {
                case ProposalReceived rec:
                case ProposalRejected rej:
                case ProposalResponded resp:
                    break;
                default:
                    var text = JsonConvert.SerializeObject(e, Formatting.Indented);

                    Debug.Write(DateTime.UtcNow + " ");
                    Debug.WriteLine(e);
                    Debug.WriteLine(text);
                    break;
            }

            this.EventList.Add(e);
        }
    }
}
