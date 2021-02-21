using System;
using System.Collections.Generic;
using System.Text;
using YagnaSharpApi.Engine.Events;

namespace YagnaSharpApi.Engine
{
    public enum GolemTastState
    {
        Waiting,
        Running,
        Accepted,
        Rejected
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TData">type of golem task input data</typeparam>
    /// <typeparam name="TResult">type of golem task result data</typeparam>
    public class GolemTask<TData, TResult>
    {
        private static int nextTaskId = 1;

        public string Id { get; set; }
        public DateTime? Started { get; set; }
        public DateTime? Finished { get; set; }
        public GolemTastState State { get; set; }
        public TData Data { get; protected set; }
        public TResult Result { get; protected set; }
        public SmartQueue<TData, TResult> Queue { get; set; }

        private object lockObject = new object();


        public GolemTask(TData data, DateTime? expires = null, int timeout = 0)
        {
            this.Id = $"{nextTaskId++}";
            this.State = GolemTastState.Waiting;
            this.Data = data;
        }

        public event EventHandler<TaskEvent> OnTaskComplete;

        public void Start()
        {
            lock(lockObject)
            {
                this.Started = DateTime.UtcNow;
                this.State = GolemTastState.Running;
            }
        }

        public void Stop(bool retry = false)
        {
            lock (lockObject)
            {
                this.DoStop(retry);
            }
        }

        private void DoStop(bool retry = false)
        {
            this.Finished = DateTime.UtcNow;

            // add logic to reschedule
            if(this.Queue != null)
            {
                if (retry)
                    this.Queue.Reschedule(this);
                else
                    this.Queue.MarkDone(this);
            }

        }

        // DO NOT USE this is just for convenience when porting from python
        public static GolemTask<TData, TResult> QueueTask(GolemTask<TData, TResult> task /* , queue*/)
        {
            // record the task "handle" and queue reference...???
            task.Start();
            return task;
        }

        public void AcceptTask(TResult result)
        {
            lock (lockObject)
            {
                if (this.State != GolemTastState.Running)
                    throw new Exception("Accepted task not in Running state!");
                this.State = GolemTastState.Accepted;
                this.OnTaskComplete?.Invoke(this, new TaskAccepted<TResult>(this.Id, result));
                this.Result = result;
                this.DoStop();
            }
        }

        public void RejectTask(string reason, bool retry = false)
        {
            lock (lockObject)
            {
                if (this.State != GolemTastState.Running)
                    throw new Exception("Accepted task not in Running state!");
                this.State = GolemTastState.Rejected;
                this.OnTaskComplete?.Invoke(this, new TaskRejected(this.Id, reason));
                this.DoStop(retry);
            }
        }
    }
}
