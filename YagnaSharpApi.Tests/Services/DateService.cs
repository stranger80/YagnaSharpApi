using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using YagnaSharpApi.Engine;
using YagnaSharpApi.Engine.Commands;
using YagnaSharpApi.Utils;

namespace YagnaSharpApi.Tests.Services
{
    public class DateService : ServiceBase
    {
        public const string DATE_OUTPUT_PATH = "/golem/work/date.txt";
        public const int REFRESH_INTERVAL_SEC = 5;

        public override IPackage GetPayload()
        {
            return VmRequestBuilder.Repo("d646d7b93083d817846c2ae5c62c72ca0507782385a2e29291a3d376"); 
        }

        public async override IAsyncEnumerable<WorkItem> OnStartupAsync(WorkContext ctx)
        {
            ctx.Run("/bin/sh",
            "-c",
            $"while true; do date > {DATE_OUTPUT_PATH}; sleep {REFRESH_INTERVAL_SEC}; done &");

            yield return ctx.Commit();
        }

        public async override IAsyncEnumerable<WorkItem> OnRunAsync(WorkContext ctx, [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            while (true)
            {
                Thread.Sleep(REFRESH_INTERVAL_SEC * 1000);

                var runWork = ctx.Run(
                    "/bin/sh",
                    "-c",
                    $"cat {DATE_OUTPUT_PATH}"
                    );

                var batch = ctx.Commit();
                yield return batch;

                var results = await batch;

                Debug.WriteLine($"Command returned: {results[^1].Stdout}");
            }
        }

        public async override IAsyncEnumerable<WorkItem> OnShutdownAsync(WorkContext ctx, Exception error = null)
        {
            yield break;
        }

    }
}
