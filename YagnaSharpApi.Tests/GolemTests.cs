using Golem.ActivityApi.Client.Model;
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

        [TestCleanup]
        public void TestCleanup()
        {
            this.EventList.Clear();
        }

        protected async Task DoGolemRunTaskAsync<Input, Output>(
            Func<WorkContext, IAsyncEnumerable<GolemTask<Input, Output>>, IAsyncEnumerable<Script>> workerFunc,
            IEnumerable<GolemTask<Input, Output>> data,
            Action assertions, bool waitForInvoiceAcceptance = true,
            int timeoutSeconds = 360)
        {
            var payload = VmRequestBuilder.Repo(
                TestConstants.VM_TASK_DEFAULT_PAYLOAD_HASH,
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

                if (waitForInvoiceAcceptance)
                {
                    // Wait until invoice paid
                    while (!this.EventList.Any(ev => ev is InvoiceAccepted))
                    {
                        Thread.Sleep(500);
                    }
                }

            }

        }

        [TestMethod]
        public async Task Golem_RunsOneWorkerOneActionTask()
        {
            var acceptedTasks = new List<GolemTask<object, string>>();

            async IAsyncEnumerable<Script> ProcessGolemTasksAsync(WorkContext ctx, IAsyncEnumerable<GolemTask<object, string>> tasks)
            {
                await foreach (var task in tasks)
                {
                    System.Diagnostics.Debug.WriteLine("Starting Task Body...");
                    var script = ctx.NewScript();

                    var workItem = script.Run("/bin/sh", "-c", "date");

                    yield return script;

                    var results = await workItem;

                    // always accept
                    System.Diagnostics.Debug.WriteLine("Accepting Task results...");
                    
                    task.AcceptTask(results?.Stdout);
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
        public async Task Golem_RunsOneWorkerNoActionTask()
        {
            List<GolemTask<int, string>> acceptedTasks = new List<GolemTask<int, string>>();

            async IAsyncEnumerable<Script> ProcessGolemTasksAsync(WorkContext ctx, IAsyncEnumerable<GolemTask<int, string>> tasks)
            {
                await foreach (var task in tasks)
                {
                    System.Diagnostics.Debug.WriteLine("Starting Task Body...");
                    var script = ctx.NewScript();

                    yield return script;

                    // always accept
                    System.Diagnostics.Debug.WriteLine("Accepting Task results...");
                    task.AcceptTask("dummy");
                    acceptedTasks.Add(task);
                    System.Diagnostics.Debug.WriteLine("Accepted Task results...");
                }

            }

            // one data set
            var data = Enumerable.Range(0, 1).Select(item => new GolemTask<int, string>(item * 10)).ToList();

            await this.DoGolemRunTaskAsync(ProcessGolemTasksAsync, data, () =>
            {
                Assert.AreEqual(data.Count(), acceptedTasks.Count);
            }, 
            false);
        }

        [TestMethod]
        public async Task Golem_RunsOneWorkerTaskFileTransferInOutTask()
        {
            List<GolemTask<int, string>> acceptedTasks = new List<GolemTask<int, string>>();

            async IAsyncEnumerable<Script> ProcessGolemTasksAsync(WorkContext ctx, IAsyncEnumerable<GolemTask<int, string>> tasks)
            {
                await foreach (var task in tasks)
                {
                    System.Diagnostics.Debug.WriteLine("Starting Task Body...");

                    var script = ctx.NewScript();

                    script.SendFile("Assets/cubes.blend", "/golem/resource/scene.blend");
                    script.DownloadFile($"/golem/resource/scene.blend", "output.file");
                    
                    yield return script;
                    // always accept
                    System.Diagnostics.Debug.WriteLine("Accepting Task results...");
                    task.AcceptTask("dummy");
                    acceptedTasks.Add(task);
                    System.Diagnostics.Debug.WriteLine("Accepted Task results...");
                }

            }

            // one data set
            var data = Enumerable.Range(0, 1).Select(item => new GolemTask<int, string>(item * 10)).ToList();

            await this.DoGolemRunTaskAsync(ProcessGolemTasksAsync, data, () =>
            {
                Assert.AreEqual(data.Count(), acceptedTasks.Count);
                Assert.IsTrue(File.Exists("output.file"));
                Assert.AreEqual(new FileInfo("Assets\\cubes.blend").Length, new FileInfo("output.file").Length);

            });
        }

        [TestMethod]
        public async Task Golem_RunsOneWorkerTaskBlenderFrameTask()
        {
            List<GolemTask<int, string>> acceptedTasks = new List<GolemTask<int, string>>();

            async IAsyncEnumerable<Script> ProcessGolemTasksAsync(WorkContext ctx, IAsyncEnumerable<GolemTask<int, string>> tasks)
            {
                var scenePath = "Assets/cubes.blend";
                var script = ctx.NewScript();

                script.SendFile(scenePath, "/golem/resource/scene.blend");
                await foreach (var task in tasks)
                {
                    var frame = task.Data;
                    script.SendJson(
                        new
                        {
                            scene_file = "/golem/resource/scene.blend",
                            resolution = new[] { 40, 30 },
                            use_compositing = false,
                            crops = new[] { new { outfilebasename = "out", borders_x = new[] { 0.0, 1.0 }, borders_y = new[] { 0.0, 1.0 } } },
                            samples = 100,
                            frames = new[] { frame },
                            output_format = "PNG",
                            RESOURCES_DIR = "/golem/resources",
                            WORK_DIR = "/golem/work",
                            OUTPUT_DIR = "/golem/output"
                        },
                        "/golem/work/params.json");

                    script.Run("/golem/entrypoints/run-blender.sh");
                    var outputFile = $"output_{frame}.png";
                    script.DownloadFile($"/golem/output/out{frame:d4}.png", outputFile);
                    
                    yield return script;
                    // TODO check if results are valid
                    task.AcceptTask(outputFile);
                    acceptedTasks.Add(task);
                }

            }

            // one data set
            var data = Enumerable.Range(0, 1).Select(item => new GolemTask<int, string>(item * 10)).ToList();

            await this.DoGolemRunTaskAsync(ProcessGolemTasksAsync, data, () =>
            {
                Assert.AreEqual(data.Count(), acceptedTasks.Count);
                Assert.IsTrue(File.Exists("output_0.png"));
            },
            true,
            600);
        }


        [TestMethod]
        public async Task Golem_FailsTaskGracefullyForExeScriptLocalError()
        {
            List<GolemTask<int, string>> acceptedTasks = new List<GolemTask<int, string>>();

            async IAsyncEnumerable<Script> ProcessGolemTasksAsync(WorkContext ctx, IAsyncEnumerable<GolemTask<int, string>> tasks)
            {
                var scenePath = "nonexistent.path";
                var script = ctx.NewScript();

                script.SendFile(scenePath, "/golem/resource/scene.blend");
                await foreach (var task in tasks)
                {
                    var frame = task.Data;
                    script.SendJson(
                        new
                        {
                            scene_file = "/golem/resource/scene.blend",
                            resolution = new[] { 40, 30 },
                            use_compositing = false,
                            crops = new[] { new { outfilebasename = "out", borders_x = new[] { 0.0, 1.0 }, borders_y = new[] { 0.0, 1.0 } } },
                            samples = 100,
                            frames = new[] { frame },
                            output_format = "PNG",
                            RESOURCES_DIR = "/golem/resources",
                            WORK_DIR = "/golem/work",
                            OUTPUT_DIR = "/golem/output"
                        },
                        "/golem/work/params.json");

                    script.Run("/golem/entrypoints/run-blender.sh");
                    var outputFile = $"output_{frame}.png";
                    script.DownloadFile($"/golem/output/out{frame:d4}.png", outputFile);
                    yield return script;
                    // TODO check if results are valid
                    task.AcceptTask(outputFile);
                    acceptedTasks.Add(task);
                }

            }

            // one data set
            var data = Enumerable.Range(0, 1).Select(item => new GolemTask<int, string>(item * 10)).ToList();

            await this.DoGolemRunTaskAsync(ProcessGolemTasksAsync, data, () =>
            {
                Assert.AreEqual(0, acceptedTasks.Count);

                // assertion to verify that a WorkerFinished event was observed, which had FileNotFoundException recorded
                var workerFinishedEvent = this.EventList.LastOrDefault(ev => ev is WorkerFinished) as WorkerFinished;

                Assert.IsNotNull(workerFinishedEvent);
                Assert.IsNotNull(workerFinishedEvent.Exception);
                Assert.IsTrue(workerFinishedEvent.Exception is FileNotFoundException);
            },
            false,
            600);
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
                    .Script
                    .GetResults()
                    .LastOrDefault()?
                    .Stdout != null);

                // Shutdown service instance...
                cluster.Instances[0].Shutdown();


                // Wait until invoice paid
                while(!events.Any(ev => ev is InvoiceAccepted))
                {
                    Thread.Sleep(500);
                }

            }

        }


        [TestMethod]
        public async Task Golem_RunsSimplePayload()
        {
            const string DATE_OUTPUT_PATH = "/golem/work/date.txt";
            const int REFRESH_INTERVAL_SEC = 5;
            
            var payload = VmRequestBuilder.Repo(
                TestConstants.VM_TASK_DEFAULT_PAYLOAD_HASH,
                0.5m,
                2.0m
                );


            using (var golem = new Engine.Golem(
                1.0m,
                TestConstants.SUBNET_TAG))
            {
                golem.OnExecutorEvent += Golem_OnExecutorEvent;

                var (agreement, activity) = await golem.RunPayloadAsync(payload, DateTime.Now.AddMinutes(7.0));

                Assert.IsNotNull(agreement);
                Assert.IsNotNull(activity);

                await foreach(var result in activity.ExecAsync(new List<ExeScriptCommand>()
                    {
                        new DeployCommand(new object()),
                        new StartCommand(new StartCommandBody(new List<string>())),
                        new RunCommand(new RunCommandBody("/bin/sh",
                            new List<string>() {"-c", $"while true; do date > {DATE_OUTPUT_PATH}; sleep {REFRESH_INTERVAL_SEC}; done &" },
                            new Capture()  // default settings of stdout/stderr
                            {
                                Stdout = new CaptureMode()
                                {
                                    Stream = new CaptureStreamBody()
                                },
                                Stderr = new CaptureMode()
                                {
                                    Stream = new CaptureStreamBody()
                                }
                            }
                        )),
                        new RunCommand(new RunCommandBody("/bin/sh",
                            new List<string>() { "-c", $"cat {DATE_OUTPUT_PATH}" },
                            new Capture()  // default settings of stdout/stderr
                            {
                                Stdout = new CaptureMode()
                                {
                                    Stream = new CaptureStreamBody()
                                },
                                Stderr = new CaptureMode()
                                {
                                    Stream = new CaptureStreamBody()
                                }
                            }))
                        }))
                {
                    Assert.AreEqual(ExeScriptCommandResult.ResultEnum.Ok, result.Result);
                }


                for(int i=0; i< 10; i++)
                {
                    Thread.Sleep(5000);
                    await foreach (var result in activity.ExecAsync(new List<ExeScriptCommand>()
                    {
                        new RunCommand(new RunCommandBody("/bin/sh",
                            new List<string>() { "-c", $"cat {DATE_OUTPUT_PATH}" },
                            new Capture()  // default settings of stdout/stderr
                            {
                                Stdout = new CaptureMode()
                                {
                                    Stream = new CaptureStreamBody()
                                },
                                Stderr = new CaptureMode()
                                {
                                    Stream = new CaptureStreamBody()
                                }
                            }))
                        }))
                    {
                        Assert.AreEqual(ExeScriptCommandResult.ResultEnum.Ok, result.Result);
                        Assert.IsNotNull(result.Stdout);
                    }

                }



                await agreement.TerminateAsync(new Entities.ReasonEntity() { Message = "Terminated" });


                Thread.Sleep(10000); // allow for invoices to be paid, etc.


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
