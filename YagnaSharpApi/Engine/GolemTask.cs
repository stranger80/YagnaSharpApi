using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using YagnaSharpApi.Engine.Events;
using YagnaSharpApi.Entities;

namespace YagnaSharpApi.Engine
{
    public enum GolemTaskState
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
        public GolemTaskState State { get; set; }
        public TData Data { get; protected set; }
        public TResult Result { get; protected set; }
        
        [JsonIgnore]
        public SmartQueue<TData, TResult> Queue { get; set; }
        public AgreementEntity Agreement { get; set; }
        public ActivityEntity Activity { get; set; }

        private object lockObject = new object();


        public GolemTask(TData data, DateTime? expires = null, int timeout = 0)
        {
            this.Id = $"{nextTaskId++}";
            this.State = GolemTaskState.Waiting;
            this.Data = data;
        }

        public event EventHandler<TaskEvent<TData, TResult>> OnTaskComplete;

        public void Start(AgreementEntity agreement, ActivityEntity activity)
        {
            lock(lockObject)
            {
                this.Agreement = agreement;
                this.Activity = activity;
                this.Started = DateTime.UtcNow;
                this.State = GolemTaskState.Running;
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

        public void AcceptTask(TResult result)
        {
            lock (lockObject)
            {
                if (this.State != GolemTaskState.Running)
                    throw new Exception("Accepted task not in Running state!");
                this.State = GolemTaskState.Accepted;
                this.OnTaskComplete?.Invoke(this, new TaskAccepted<TData, TResult>(this, result));
                this.Result = result;
                this.DoStop();
            }
        }

        public void RejectTask(string reason, bool retry = false)
        {
            lock (lockObject)
            {
                if (this.State != GolemTaskState.Running)
                    throw new Exception("Accepted task not in Running state!");
                this.State = GolemTaskState.Rejected;
                this.OnTaskComplete?.Invoke(this, new TaskRejected<TData, TResult>(this, reason));
                this.DoStop(retry);
            }
        }
    }
}
