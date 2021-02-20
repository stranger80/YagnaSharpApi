using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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

        static ExecutorTests()
        {
            MapConfig.Init();
        }

        static async IAsyncEnumerable<WorkItem> SimpleDeployWorker(WorkContext ctx, IAsyncEnumerable<GolemTask<int, string>> tasks)
        {
            await foreach (var task in tasks)
            {
                ctx.Prepare();  // force the init steps to be added to exescript (DEPLOY/START)
                yield return ctx.Commit();
                // always accept
                task.AcceptTask("dummy");
            }

        }


        [TestMethod]
        public async Task Executor_RunsOneWorkerTask()
        {
            var package = VmRequestBuilder.Repo(
                "9a3b5d67b0b27746283cb5f287c13eab1beaa12d92a9f536b747c7ae",
                0.5m,
                2.0m
                );

            // one data set
            var data = Enumerable.Range(0, 1);

            using (var executor = new Engine.Executor(
                package,
                1,
                data.Count() * 5,
                360, // 6 mins timeout as providers will not accept less
                TestConstants.SUBNET_TAG))
            {
                executor.OnExecutorEvent += Executor_OnExecutorEvent;

                await foreach (var task in executor.Submit(SimpleDeployWorker, data.Select(item => new GolemTask<int, string>(item * 10))))
                {
                    Console.WriteLine($"{TextColorConstants.TEXT_COLOR_CYAN}Task computed: {task}, result: {task.Result}{TextColorConstants.TEXT_COLOR_DEFAULT}");
                }
            }





        }

        private void Executor_OnExecutorEvent(object sender, Event e)
        {
            var text = JsonConvert.SerializeObject(e, Formatting.Indented);

            Debug.WriteLine(e);
            Debug.WriteLine(text);
        }
    }
}
