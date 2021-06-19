using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YagnaSharpApi.Engine.Commands;
using YagnaSharpApi.Storage;

namespace YagnaSharpApi.Engine
{
    public class WorkContext
    {
        public string CtxId { get; private set; }
        protected StorageProvider storage;
        protected bool started = false;
        protected List<WorkItem> pendingSteps = new List<WorkItem>();

        public WorkContext(string ctxId, StorageProvider storage)
        {
            this.CtxId = ctxId;
            this.storage = storage;
        }

        /// <summary>
        /// Adds an InitStep to the batch, if the batch hasn't yet included a starting command.
        /// Otherwise it returns null.
        /// </summary>
        /// <returns></returns>
        public InitStep Prepare()
        {
            if(! this.started)
            {
                var initStep = new InitStep();
                this.pendingSteps.Add(initStep);
                this.started = true;
                return initStep;
            }
            return null;
        }

        /// <summary>
        /// Adds DeployStep to the batch (if not started yet).
        /// </summary>
        public IndexedWorkItem Deploy()
        {
            if (!this.started)
            {
                var deployStep = new DeployStep();
                this.pendingSteps.Add(deployStep);
                return deployStep;
            }
            return null;
        }

        public IndexedWorkItem Start()
        {
            if (!this.started)
            {
                var startStep = new StartStep();
                this.pendingSteps.Add(startStep);
                this.started = true;
                return startStep;
            }
            return null;
        }

        public IndexedWorkItem SendJson(string destPath, object data)
        {
            this.Prepare();
            var sendJson = new SendJson(this.storage, data, destPath);
            this.pendingSteps.Add(sendJson);
            return sendJson;
        }

        public IndexedWorkItem SendFile(string srcPath, string destPath)
        {
            this.Prepare();
            var sendFile = new SendFile(this.storage, srcPath, destPath);
            this.pendingSteps.Add(sendFile);
            return sendFile;
        }

        public IndexedWorkItem Run(string cmd, params string[] args)
        {
            this.Prepare();
            var run = new Run(cmd, args);
            this.pendingSteps.Add(run);
            return run;
        }

        public IndexedWorkItem DownloadFile(string srcPath, string destPath)
        {
            this.Prepare();
            var recvFile = new RecvFile(this.storage, srcPath, destPath);
            this.pendingSteps.Add(recvFile);
            return recvFile;
        }

        public WorkItem Commit()
        {
            var result = new Steps(this.pendingSteps);
            this.pendingSteps = new List<WorkItem>();
            return result;
        }
    }
}
