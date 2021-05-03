using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YagnaSharpApi.Engine;
using YagnaSharpApi.Engine.Commands;
using YagnaSharpApi.Utils;

namespace YagnaSharpApi.Examples
{


    class BlenderExample
    {

        static async IAsyncEnumerable<WorkItem> Worker(WorkContext ctx, IAsyncEnumerable<GolemTask<int, string>> tasks)
        {
            var scenePath = /* scriptDir + */ "/cubes.blend";
            ctx.SendFile(scenePath, "/golem/resource/scene.blend");
            await foreach (var task in tasks)
            {
                var frame = task.Data;
                ctx.SendJson("/golem/work/params.json",
                    new
                    {
                        scene_file = "/golem/resource/scene.blend",
                        resolution = (40, 30),
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
                ctx.DownloadFile($"/golem/output/out{frame:04d}.png", outputFile);
                yield return ctx.Commit();
                // TODO check if results are valid
                task.AcceptTask(outputFile);
            }

        }

        public static async Task MainAsync(string subnetTag)
        { 
            var package = VmRequestBuilder.Repo(
                "9a3b5d67b0b27746283cb5f287c13eab1beaa12d92a9f536b747c7ae",
                0.5m,
                2.0m
                );

            // iterator over the frame indices that we want to render
            var frames = Enumerable.Range(0,6);
            // TODO make this dynamic, e.g. depending on the size of files to transfer
            // worst-case time overhead for initialization, e.g. negotiation, file transfer etc.
            var initOverhead = 3 * 60m; // 3 minutes

            using (var executor = new Engine.Executor(
                package, 
                3, 
                initOverhead + frames.Count() * 2,
                120, 
                subnetTag))
            {
                await foreach(var task in executor.SubmitAsync(Worker, frames.Select(frame => new GolemTask<int, string>(frame*10))))
                {
                    Console.WriteLine($"{TextColorConstants.TEXT_COLOR_CYAN}Task computed: {task}, result: {task.Result}{TextColorConstants.TEXT_COLOR_DEFAULT}");
                }
            }
        }
    }
}
