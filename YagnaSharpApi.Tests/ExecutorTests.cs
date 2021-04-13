using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using YagnaSharpApi.Engine;
using YagnaSharpApi.Engine.Commands;
using YagnaSharpApi.Engine.Events;
using YagnaSharpApi.Engine.MarketStrategy;
using YagnaSharpApi.Mapper;
using YagnaSharpApi.Repository;
using YagnaSharpApi.Utils;

namespace YagnaSharpApi.Tests
{
    [TestClass]
    public class ExecutorTests
    {
        public TestUtils Utils { get; set; } = new TestUtils();
        public MarketStrategyTests MarketStrategyTests { get; set; } = new MarketStrategyTests();

        public List<Event> EventList { get; set; } = new List<Event>();

        static ExecutorTests()
        {
            MapConfig.Init();
        }

        protected async Task DoExecutorRunAsync<Input, Output>(
            Func<WorkContext, IAsyncEnumerable<GolemTask<Input, Output>>, IAsyncEnumerable<WorkItem>> workerFunc,
            IEnumerable<GolemTask<Input, Output>> data,
            Action assertions,
            int timeoutSeconds = 360)
        {
            var package = VmRequestBuilder.Repo(
                "9a3b5d67b0b27746283cb5f287c13eab1beaa12d92a9f536b747c7ae",
                0.5m,
                2.0m
                );


            using (var executor = new Engine.Executor(
                package,
                1,
                data.Count() * 5,
                timeoutSeconds, 
                TestConstants.SUBNET_TAG))
            {
                executor.OnExecutorEvent += Executor_OnExecutorEvent;
                var inputTasks = data.ToList();

                await foreach (var task in executor.SubmitAsync(workerFunc, inputTasks))
                {
                    Console.WriteLine($"{TextColorConstants.TEXT_COLOR_CYAN}Task computed: {task}, result: {task.Result}{TextColorConstants.TEXT_COLOR_DEFAULT}");
                }

                assertions();
            }

        }

        [TestMethod]
        public async Task Executor_RunsOneWorkerNoActionTask()
        {
            List<GolemTask<int, string>> acceptedTasks = new List<GolemTask<int, string>>();

            async IAsyncEnumerable<WorkItem> ProcessGolemTasksAsync(WorkContext ctx, IAsyncEnumerable<GolemTask<int, string>> tasks)
            {
                await foreach (var task in tasks)
                {
                    System.Diagnostics.Debug.WriteLine("Starting Task Body...");
                    ctx.Prepare();  // force the init steps to be added to exescript (DEPLOY/START)
                    yield return ctx.Commit();
                    // always accept
                    System.Diagnostics.Debug.WriteLine("Accepting Task results...");
                    task.AcceptTask("dummy");
                    acceptedTasks.Add(task);
                    System.Diagnostics.Debug.WriteLine("Accepted Task results...");
                }

            }

            // one data set
            var data = Enumerable.Range(0, 1).Select(item => new GolemTask<int, string>(item * 10)).ToList();

            await this.DoExecutorRunAsync(ProcessGolemTasksAsync, data, () =>
            {
                Assert.AreEqual(data.Count(), acceptedTasks.Count);
            });
        }

        [TestMethod]
        public async Task Executor_RunsOneWorkerFileTransferInOutTask()
        {
            List<GolemTask<int, string>> acceptedTasks = new List<GolemTask<int, string>>();

            async IAsyncEnumerable<WorkItem> ProcessGolemTasksAsync(WorkContext ctx, IAsyncEnumerable<GolemTask<int, string>> tasks)
            {
                await foreach (var task in tasks)
                {
                    System.Diagnostics.Debug.WriteLine("Starting Task Body...");
                    ctx.SendFile("Assets/cubes.blend", "/golem/resource/scene.blend");
                    ctx.DownloadFile($"/golem/resource/scene.blend", "output.file");
                    yield return ctx.Commit();
                    // always accept
                    System.Diagnostics.Debug.WriteLine("Accepting Task results...");
                    task.AcceptTask("dummy");
                    acceptedTasks.Add(task);
                    System.Diagnostics.Debug.WriteLine("Accepted Task results...");
                }

            }

            // one data set
            var data = Enumerable.Range(0, 1).Select(item => new GolemTask<int, string>(item * 10)).ToList();

            await this.DoExecutorRunAsync(ProcessGolemTasksAsync, data, () =>
            {
                Assert.AreEqual(data.Count(), acceptedTasks.Count);
                Assert.IsTrue(File.Exists("output.file"));
                Assert.AreEqual(new FileInfo("Assets\\cubes.blend").Length, new FileInfo("output.file").Length);

            });
        }

        [TestMethod]
        public async Task Executor_RunsOneWorkerBlenderFrameTask()
        {
            List<GolemTask<int, string>> acceptedTasks = new List<GolemTask<int, string>>();

            async IAsyncEnumerable<WorkItem> ProcessGolemTasksAsync(WorkContext ctx, IAsyncEnumerable<GolemTask<int, string>> tasks)
            {
                var scenePath = "Assets/cubes.blend";
                ctx.SendFile(scenePath, "/golem/resource/scene.blend");
                await foreach (var task in tasks)
                {
                    var frame = task.Data;
                    ctx.SendJson("/golem/work/params.json",
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
                        });

                    ctx.Run("/golem/entrypoints/run-blender.sh");
                    var outputFile = $"output_{frame}.png";
                    ctx.DownloadFile($"/golem/output/out{frame:d4}.png", outputFile);
                    yield return ctx.Commit();
                    // TODO check if results are valid
                    task.AcceptTask(outputFile);
                    acceptedTasks.Add(task);
                }

            }

            // one data set
            var data = Enumerable.Range(0, 1).Select(item => new GolemTask<int, string>(item * 10)).ToList();

            await this.DoExecutorRunAsync(ProcessGolemTasksAsync, data, () =>
            {
                Assert.AreEqual(data.Count(), acceptedTasks.Count);
                Assert.IsTrue(File.Exists("output_0.png"));
            },
            600);
        }


        [TestMethod]
        public async Task Executor_FailsGracefullyForExeScriptLocalError()
        {
            List<GolemTask<int, string>> acceptedTasks = new List<GolemTask<int, string>>();

            async IAsyncEnumerable<WorkItem> ProcessGolemTasksAsync(WorkContext ctx, IAsyncEnumerable<GolemTask<int, string>> tasks)
            {
                var scenePath = "nonexistent.path";
                ctx.SendFile(scenePath, "/golem/resource/scene.blend");
                await foreach (var task in tasks)
                {
                    var frame = task.Data;
                    ctx.SendJson("/golem/work/params.json",
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
                        });

                    ctx.Run("/golem/entrypoints/run-blender.sh");
                    var outputFile = $"output_{frame}.png";
                    ctx.DownloadFile($"/golem/output/out{frame:d4}.png", outputFile);
                    yield return ctx.Commit();
                    // TODO check if results are valid
                    task.AcceptTask(outputFile);
                    acceptedTasks.Add(task);
                }

            }

            // one data set
            var data = Enumerable.Range(0, 1).Select(item => new GolemTask<int, string>(item * 10)).ToList();

            await this.DoExecutorRunAsync(ProcessGolemTasksAsync, data, () =>
            {
                Assert.AreEqual(0, acceptedTasks.Count);
                
                // assertion to verify that a WorkerFinished event was observed, which had FileNotFoundException recorded
                var workerFinishedEvent = this.EventList.LastOrDefault(ev => ev is WorkerFinished) as WorkerFinished;

                Assert.IsNotNull(workerFinishedEvent);
                Assert.IsNotNull(workerFinishedEvent.Exception);
                Assert.IsTrue(workerFinishedEvent.Exception is FileNotFoundException);
            },
            600);
        }

        private void Executor_OnExecutorEvent(object sender, Event e)
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
