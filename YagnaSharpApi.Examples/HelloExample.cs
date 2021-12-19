using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YagnaSharpApi.Engine;
using YagnaSharpApi.Engine.Commands;
using YagnaSharpApi.Utils;

namespace YagnaSharpApi.Examples
{
    public class HelloExample : IGolemExample
    {

        public async Task RunExampleAsync()
        {
            var payload = VmRequestBuilder.Repo("9a3b5d67b0b27746283cb5f287c13eab1beaa12d92a9f536b747c7ae");

            async IAsyncEnumerable<Script> Worker(WorkContext ctx, IAsyncEnumerable<GolemTask<object, string>> tasks)
            {
                await foreach (var task in tasks)
                {
                    var script = ctx.NewScript();
                    var command = script.Run("/bin/sh", "-c", "date");

                    yield return script;

                    var results = await command;

                    task.AcceptTask(results?.Stdout);
                }

            }


            var tasks = Enumerable.Range(0, 1).Select(item => new GolemTask<object, string>(null));

            using (var golem = new Engine.Golem(1.0m, ExampleConstants.SUBNET_TAG))
            {
                await foreach (var task in golem.ExecuteTasksAsync(Worker, tasks, payload))
                {
                    Console.WriteLine($"{task.Result}");
                }
            }

        }
    }
}
