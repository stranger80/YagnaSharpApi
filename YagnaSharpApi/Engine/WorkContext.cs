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
        public StorageProvider Storage { get; protected set; }
        protected bool started = false;
        protected List<Command> pendingSteps = new List<Command>();
        public NodeInfo NodeInfo { get; set; }

        public string ProviderName { get => this.NodeInfo?.Name; } 

        public WorkContext(string ctxId, StorageProvider storage, NodeInfo nodeInfo)
        {
            this.CtxId = ctxId;
            this.Storage = storage;
            this.NodeInfo = nodeInfo;
        }

        public Script NewScript()
        {
            return new Script(this);
        }

        /// <summary>
        /// Adds an InitStep to the batch, if the batch hasn't yet included a starting command.
        /// Otherwise it returns null.
        /// </summary>
        /// <returns></returns>
        [Obsolete]
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
        [Obsolete]
        public IndexedCommand Deploy()
        {
            if (!this.started)
            {
                var deployStep = new DeployStep();
                this.pendingSteps.Add(deployStep);
                return deployStep;
            }
            return null;
        }

        [Obsolete]
        public IndexedCommand Start()
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

        [Obsolete]
        public IndexedCommand SendJson(string destPath, object data)
        {
            this.Prepare();
            var sendJson = new SendJson(this.Storage, data, destPath);
            this.pendingSteps.Add(sendJson);
            return sendJson;
        }

        [Obsolete]
        public IndexedCommand SendFile(string srcPath, string destPath)
        {
            this.Prepare();
            var sendFile = new SendFile(this.Storage, srcPath, destPath);
            this.pendingSteps.Add(sendFile);
            return sendFile;
        }

        [Obsolete]
        public IndexedCommand Run(string cmd, params string[] args)
        {
            this.Prepare();
            var run = new Run(cmd, args);
            this.pendingSteps.Add(run);
            return run;
        }

        [Obsolete]
        public IndexedCommand DownloadFile(string srcPath, string destPath)
        {
            this.Prepare();
            var recvFile = new RecvFile(this.Storage, srcPath, destPath);
            this.pendingSteps.Add(recvFile);
            return recvFile;
        }

        [Obsolete]
        public BatchCommand Commit()
        {
            var result = new BatchCommand(this.pendingSteps);
            this.pendingSteps = new List<Command>();
            return result;
        }
    }
}
