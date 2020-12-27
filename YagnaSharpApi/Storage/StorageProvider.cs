using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace YagnaSharpApi.Storage
{
    public abstract class StorageProvider : IInputStorageProvider, IOutputStorageProvider
    {
        public abstract Task<IDestination> NewDestination(string destinationFile = null);

        public Task<ISource> UploadBytes(byte[] data)
        {
            using (var stream = new MemoryStream(data))
            {
                return UploadStream(stream);
            }
        }

        public Task<ISource> UploadFile(string file)
        {
            using (var stream = new FileStream(file, FileMode.Open))
            {
                return UploadStream(stream);
            }
        }

        public abstract Task<ISource> UploadStream(Stream stream);
    }
}
