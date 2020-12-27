using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using YagnaSharpApi.Storage;

namespace YagnaSharpApi.Engine.Commands
{
    public class SendFile : SendWork
    {
        protected string srcPath;

        public SendFile(IInputStorageProvider storage, string srcPath, string destPath) : base(storage, destPath)
        {
            this.srcPath = srcPath;
        }

        protected async override Task<ISource> DoUpload()
        {
            this.src = await this.storage.UploadFile(this.srcPath);
            return src;
        }
    }
}
