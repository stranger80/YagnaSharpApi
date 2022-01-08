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

    }
}
