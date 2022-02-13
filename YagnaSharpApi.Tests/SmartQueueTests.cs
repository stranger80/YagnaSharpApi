using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YagnaSharpApi.Engine;
using YagnaSharpApi.Engine.Commands;

namespace YagnaSharpApi.Tests
{
    [TestClass]
    public class SmartQueueTests
    {


        async IAsyncEnumerable<Command> TestWorkerAllSuccessful(IAsyncEnumerable<GolemTask<int, int>> tasks)
        {
            await foreach (var task in tasks)
            {

                yield return new InitStep();
                task.AcceptTask(task.Data);
            }

        }

        async IAsyncEnumerable<Command> TestWorkerRejectOddItems(IAsyncEnumerable<GolemTask<int, int>> tasks)
        {
            await foreach (var task in tasks)
            {

                yield return new InitStep();
                if (task.Data % 2 == 0)
                {
                    task.AcceptTask(task.Data);
                }
                else
                {
                    task.RejectTask("task rejected", true);
                }
            }

        }


        [TestMethod]
        public async Task SmartQueue_HandlesMultipleTasks()
        {
            using (var queue = new SmartQueue<int, int>(0)) // no retries
            {
                var taskData = Enumerable.Range(1, 3);

                GolemTask<int, int> lastTask = null;

                // "wrap" the data task collection with logic that for each fetched task will put the task into the queue 
                // that monitors task execution, handles retries, etc.
                async IAsyncEnumerable<GolemTask<int, int>> QueueTasks(IEnumerable<GolemTask<int, int>> data, SmartQueue<int, int> queue)
                {
                    // queue task in smart queue
                    foreach (var task in data)
                    {
                        queue.QueueTask(task);
                        task.Queue = queue;
                    }

                    // now get tasks from smartqueue - it will return next task, 
                    // selecting one from either newly queued, or queued for reschedule
                    await foreach (var task in queue.GetTaskForExecutionAsync())
                    {
                        task.Start(null, null);
                        lastTask = task;
                        yield return task;
                    }
                }




                var commandGenerator = this.TestWorkerAllSuccessful(QueueTasks(taskData.Select(item => new GolemTask<int, int>(item)), queue));

                var processedTasks = new List<GolemTask<int, int>>();

                Task.Run(async () =>
                {
                    await foreach (var batch in commandGenerator)
                    {
                        try
                        {
                        // how do I get the GolemTask that is executing this batch???
                        var currentTask = lastTask;

                            processedTasks.Add(currentTask);

                            await Task.Delay(100); // this is where we simulate the duration if task execution
                    }
                        catch (Exception exc)
                        {
                        // TODO raise WorkerFinished event
                        return;
                        }
                    }
                });

                queue.WaitUntilDone();

                Assert.IsTrue(!queue.QueuedTasks.Any());
                Assert.AreEqual(taskData.Count(), processedTasks.Count());
            }

        }


        [TestMethod]
        public async Task SmartQueue_HandlesRetries()
        {
            using (var queue = new SmartQueue<int, int>(3))  // 3 retries per failed task
            {
                var taskData = Enumerable.Range(1, 3);

                GolemTask<int, int> lastTask = null;

                // "wrap" the data task collection with logic that for each fetched task will put the task into the queue 
                // that monitors task execution, handles retries, etc.
                async IAsyncEnumerable<GolemTask<int, int>> QueueTasks(IEnumerable<GolemTask<int, int>> data, SmartQueue<int, int> queue)
                {
                    // queue task in smart queue
                    foreach (var task in data)
                    {
                        queue.QueueTask(task);
                        task.Queue = queue;
                    }

                    // now get tasks from smartqueue - it will return next task, 
                    // selecting one from either newly queued, or queued for reschedule
                    await foreach (var task in queue.GetTaskForExecutionAsync())
                    {
                        task.Start(null, null);
                        lastTask = task;
                        yield return task;
                    }
                }


                var commandGenerator = this.TestWorkerRejectOddItems(QueueTasks(taskData.Select(item => new GolemTask<int, int>(item)), queue));

                var processedTasks = new List<GolemTask<int, int>>();

                await foreach (var batch in commandGenerator)
                {
                    try
                    {
                        // how do I get the GolemTask that is executing this batch???
                        var currentTask = lastTask;

                        processedTasks.Add(currentTask);

                        await Task.Delay(100); // this is where we simulate the duration if task execution
                    }
                    catch (Exception exc)
                    {
                        // TODO raise WorkerFinished event
                        return;
                    }
                }

                Assert.IsTrue(queue.AreAllTasksProcessed());
                Assert.AreEqual(taskData.Count(), processedTasks.Distinct().Count());
                Assert.AreEqual(2, queue.FailedTasks.Count);
                Assert.AreEqual(1, queue.DoneTasks.Count);
            }

        }


    }
}
