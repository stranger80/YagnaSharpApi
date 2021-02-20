using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace YagnaSharpApi.Engine
{



    public class SmartQueue<TData, TResult> : IDisposable
    {
        public class TaskContext
        {
            public GolemTask<TData, TResult> Task { get; set; }
            public int RetryCount { get; set; }
        }


        public ConcurrentQueue<TaskContext> QueuedTasks { get; set; } = new ConcurrentQueue<TaskContext>();
        public Dictionary<string, TaskContext> InProgressTasks { get; set; } = new Dictionary<string, TaskContext>();
        public List<TaskContext> DoneTasks { get; set; } = new List<TaskContext>();
        public List<TaskContext> FailedTasks { get; set; } = new List<TaskContext>();

        public int MaxRetryCount { get; set; }
        private int taskCounter = 0;
        private EventWaitHandle eof = new EventWaitHandle(false, EventResetMode.AutoReset);
        private bool started = false; // marker to indicate that the queue started execution (this is to prevent WaitUntilDone() from returning before we start queueing items)
        private bool disposedValue;

        public SmartQueue(int maxRetryCount)
        {
            this.MaxRetryCount = maxRetryCount;
        }

        public bool AreAllTasksProcessed()
        {
            return taskCounter == (this.DoneTasks.Count + this.FailedTasks.Count );
        }

        public bool AreSomeTasksUnassigned()
        {
            return !this.QueuedTasks.IsEmpty;
        }

        public void QueueTask(GolemTask<TData, TResult> task)
        {
            started = true;
            this.QueuedTasks.Enqueue(new TaskContext() { Task = task, RetryCount = 0 });
            taskCounter++;
        }

        public async IAsyncEnumerable<GolemTask<TData, TResult>> GetTaskForExecutionAsync()
        {
            do
            {
                while(QueuedTasks.TryDequeue(out TaskContext taskContext))
                {
                    InProgressTasks[taskContext.Task.Id] = taskContext;
                    yield return taskContext.Task;
                }

                if(InProgressTasks.Any())
                {
                    await Task.Delay(2000);
                }
            }
            while (!AreAllTasksProcessed());
        }

        public void MarkDone(GolemTask<TData, TResult> task)
        {
            if(this.InProgressTasks.ContainsKey(task.Id))
            {
                DoneTasks.Add(this.InProgressTasks[task.Id]);
                this.InProgressTasks.Remove(task.Id);
                this.eof.Set();
            }
        }

        public void Reschedule(GolemTask<TData, TResult> task)
        {
            if (this.InProgressTasks.ContainsKey(task.Id))
            {
                var taskContext = this.InProgressTasks[task.Id];
                if (taskContext.RetryCount < this.MaxRetryCount)
                {
                    taskContext.RetryCount++;
                    this.QueuedTasks.Enqueue(taskContext);
                }
                else
                {
                    this.FailedTasks.Add(taskContext);
                }
                this.InProgressTasks.Remove(task.Id);
            }
        }

        public async void WaitUntilDone()
        {
            while(!this.AreAllTasksProcessed() || !started)
            {
                this.eof.WaitOne();
            }

        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    this.eof.Dispose();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~SmartQueue()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
