using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace YagnaSharpApi.Storage
{
    public abstract class StorageProvider : IInputStorageProvider, IOutputStorageProvider, IDisposable
    {
        private bool disposedValue;

        public abstract Task<IDestination> NewDestination(string destinationFile = null);

        public Task<ISource> UploadBytes(byte[] data)
        {
            using (var stream = new MemoryStream(data))
            {
                async IAsyncEnumerable<byte> GetBytes()
                {
                    int b = 0;
                    while((b = stream.ReadByte()) != -1)
                        yield return (byte)b;
                }

                return UploadStream(GetBytes());
            }
        }

        public Task<ISource> UploadFile(string file)
        {
            using (var stream = new FileStream(file, FileMode.Open))
            {
                async IAsyncEnumerable<byte> GetBytes()
                {
                    int b = 0;
                    while ((b = stream.ReadByte()) != -1)
                        yield return (byte)b;
                }

                return UploadStream(GetBytes());
            }
        }

        public abstract Task<ISource> UploadStream(IAsyncEnumerable<byte> stream);

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
