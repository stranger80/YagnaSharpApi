using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using YagnaSharpApi.Storage;

namespace YagnaSharpApi.Engine.Commands
{
    public class RecvFile : IndexedWorkItem
    {
        protected IOutputStorageProvider storage;
        protected string srcPath;
        protected string destPath;
        protected IDestination destSlot;

        public RecvFile(IOutputStorageProvider storage, string srcPath, string destPath)
        {
            this.storage = storage;
            this.destPath = destPath;
            this.srcPath = srcPath;
        }

        public async override Task Prepare()
        {
            this.destSlot = await this.storage.NewDestination(destPath);
        }

        public override void Register(ExeScriptBuilder commands)
        {
            this.CommandIndex = commands.Transfer($"container:{this.srcPath}", this.destSlot.UploadUrl());
        }

        public async override Task Post()
        {
            if (this.destSlot == null)
                throw new Exception("RecvFile Post() called without Prepare()");

            await this.destSlot.DownloadFile(this.destPath);
        }
    }
}
