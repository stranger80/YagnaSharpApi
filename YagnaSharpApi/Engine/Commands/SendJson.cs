using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using YagnaSharpApi.Storage;

namespace YagnaSharpApi.Engine.Commands
{
    public class SendJson : SendWork
    {
        protected string json;

        public SendJson(IInputStorageProvider storage, object data, string destPath) : base(storage, destPath)
        {
            this.json = JsonConvert.SerializeObject(data);
        }

        protected async override Task<ISource> DoUpload()
        {
            if (this.json == null)
                throw new Exception("JSON buffer not initialized...");

            this.src = await this.storage.UploadBytes(UTF8Encoding.UTF8.GetBytes(this.json));
            this.json = null;
            return src;
        }
    }
}
