using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace YagnaSharpApi.Storage
{
    public abstract class DestinationBase : IDestination
    {
        public virtual async Task DownloadFile(string destinationFile)
        {
            var content = this.DownloadStream();

            using(var file = File.OpenWrite(destinationFile))
            {
                await foreach (var b in content.ContentBytes)
                {
                    file.WriteByte(b);
                }
                file.Close();
            }
        }

        public abstract Content DownloadStream();

        public abstract string UploadUrl();
    }
}
