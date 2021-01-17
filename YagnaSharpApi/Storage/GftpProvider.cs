using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YagnaSharpApi.Storage
{
    public class GftpProvider : StorageProvider, IDisposable
    {
        private string tempDir;
        private bool disposedValue;
        private GftpWrapper wrapper;

        public GftpProvider(string tempDir = null)
        {
            this.tempDir = tempDir ?? Path.GetTempPath();
            this.wrapper = new GftpWrapper();
        }

        private string NewFile()
        {
            return Path.Combine(tempDir, Path.GetRandomFileName());
        }

        public async override Task<IDestination> NewDestination(string destinationFile = null)
        {
            var outputFile = destinationFile ?? this.NewFile();

            var link = await this.wrapper.ReceiveAsync(outputFile);

            return new GftpDestination(link);
        }

        public async override Task<ISource> UploadStream(IAsyncEnumerable<byte> stream)
        {
            var fileName = this.NewFile();

            var file = File.OpenWrite(fileName);

            await foreach(var b in stream)
            {
                file.WriteByte(b);
            }

            file.Close();

            var links = await wrapper.PublishAsync(new string[] { fileName });

            if(links.Count() != 1)
            {
                throw new Exception("Invalid gftp publish response");
            }

            return new GftpSource(file.Length, links.First());
        }

        protected override void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    this.wrapper.Dispose();
                }

                disposedValue = true;
            }
        }

    }
}
