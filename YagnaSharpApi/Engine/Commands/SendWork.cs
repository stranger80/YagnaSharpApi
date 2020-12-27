using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using YagnaSharpApi.Storage;

namespace YagnaSharpApi.Engine.Commands
{
    public abstract class SendWork : WorkItem
    {
        public int Index { get; set; }
        protected ISource src;
        protected string destPath;
        protected IInputStorageProvider storage;

        public SendWork(IInputStorageProvider storage, string destPath)
        {
            this.destPath = destPath;
            this.storage = storage;
        }

        public SendWork(string destPath)
        {
            this.destPath = destPath;
        }

        public async override Task Prepare()
        {
            src = await this.DoUpload();
        }

        protected abstract Task<ISource> DoUpload();

        public override void Register(ExeScriptBuilder commands)
        {
            this.Index = commands.Transfer(this.src.DownloadUrl(), $"container:{this.destPath}");
        }
    }
}
