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

        public void Prepare()
        {
            if(! this.started)
            {
                this.pendingSteps.Add(new InitStep());
                this.started = true;
            }
        }

        public void SendJson(string destPath, object data)
        {
            this.Prepare();
            this.pendingSteps.Add(new SendJson(this.storage, data, destPath));
        }

        public void SendFile(string srcPath, string destPath)
        {
            this.Prepare();
            this.pendingSteps.Add(new SendFile(this.storage, srcPath, destPath));
        }

        public void Run(string cmd, params string[] args)
        {
            this.Prepare();
            this.pendingSteps.Add(new Run(cmd, args));
        }

        public void DownloadFile(string srcPath, string destPath)
        {
            this.Prepare();
            this.pendingSteps.Add(new RecvFile(this.storage, srcPath, destPath));
        }

        public WorkItem Commit()
        {
            var result = new Steps(this.pendingSteps);
            this.pendingSteps = new List<WorkItem>();
            return result;
        }
    }
}
