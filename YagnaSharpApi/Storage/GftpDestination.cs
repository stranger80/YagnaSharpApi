using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace YagnaSharpApi.Storage
{
    public class GftpDestination : DestinationBase, IDisposable
    {
        private bool disposedValue;
        private Stream downloadStream;

        public PubLink Link { get; protected set; }

        public GftpDestination(PubLink link)
        {
            this.Link = link;
        }
        public override async Task DownloadFile(string destinationFile)
        {
            if (destinationFile == this.Link.File)
                return;

            await base.DownloadFile(destinationFile);
        }

        public override Content DownloadStream()
        {
            var path = new FileInfo(this.Link.File);
            if(path.Exists)
            {
                var length = path.Length;
                
                async IAsyncEnumerable<byte> Chunks()
                {
                    using(var file = path.OpenRead())
                    {
                        var buffer = new byte[30000];
                        int bytesRead = 0;
                        while((bytesRead = await file.ReadAsync(buffer, 0, buffer.Length)) > 0)
                        {
                            for(int i = 0; i < bytesRead; i++)
                            {
                                yield return buffer[i];
                            }
                        }
                    }
                }

                return new Content() { Length = length, ContentBytes = Chunks() };
            }

            return null;
        }

        public override string UploadUrl()
        {
            return this.Link.Url;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    if(this.downloadStream != null)
                    {
                        this.downloadStream.Dispose();
                    }
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
