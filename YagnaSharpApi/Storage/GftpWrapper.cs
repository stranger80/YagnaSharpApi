using Nerdbank.Streams;
using StreamJsonRpc;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace YagnaSharpApi.Storage
{
    public interface IGftpDriver
    {
        Task<string> version(object dummy);
        Task<IEnumerable<PubLink>> publish(IEnumerable<string> files);
    }

    public enum CommandStatus
    {
        [EnumMember(Value = "ok")]
        Ok,
        [EnumMember(Value = "error")]
        Error
    }

    /// <summary>
    /// Wrapper class for the GFTP process which is a client used to publish and download files.
    /// </summary>
    public class GftpWrapper : IDisposable
    {
        private bool disposedValue;
        private Process process;
        private JsonRpc jsonRpc;
       
        public GftpWrapper()
        {
            this.process = StartProcess();
            var formatter = new JsonMessageFormatter(new UTF8Encoding(false));

            var handler = new DebugMessageHandler(
                    process.StandardInput.BaseStream,
                    process.StandardOutput.BaseStream,
                    formatter)
            {
                NewLine = DebugMessageHandler.NewLineStyle.Lf
            };

            //var handler = new NewLineDelimitedMessageHandler(
            //    process.StandardInput.BaseStream,
            //    process.StandardOutput.BaseStream,
            //    formatter)
            //{
            //    NewLine = NewLineDelimitedMessageHandler.NewLineStyle.Lf
            //};



            jsonRpc = new JsonRpc(handler);

            this.Driver = jsonRpc.Attach<IGftpDriver>();

            jsonRpc.StartListening();

        }


        private Process StartProcess()
        {
            var startInfo = new ProcessStartInfo("gftp")
            {
                CreateNoWindow = true,
                StandardInputEncoding = new UTF8Encoding(false), // no BOM
                StandardOutputEncoding = new UTF8Encoding(false),  // no BOM
                RedirectStandardInput = true,
                RedirectStandardOutput = true
            };
            startInfo.ArgumentList.Add("server");

            var process = Process.Start(startInfo);

            return process;
        }

        public IGftpDriver Driver { get; private set; }
        

        public async Task<string> VersionAsync()
        {
            return await jsonRpc.InvokeWithParameterObjectAsync<string>("version", new object());
        }

        public async Task<IEnumerable<PubLink>> PublishAsync(IEnumerable<string> files)
        {
            return await jsonRpc.InvokeWithParameterObjectAsync<IEnumerable<PubLink>>("publish", new { files = files });
        }

        public async Task<IEnumerable<CommandStatus>> CloseAsync(IEnumerable<string> urls)
        {
            return await jsonRpc.InvokeWithParameterObjectAsync<IEnumerable<CommandStatus>>("close", new { urls = urls });
        }

        public async Task<PubLink> ReceiveAsync(string outputFile)
        {
            return await jsonRpc.InvokeWithParameterObjectAsync<PubLink>("receive", new { output_file = outputFile });
        }

        public async Task UploadAsync(string file, string url)
        {
            await jsonRpc.InvokeWithParameterObjectAsync<PubLink>("upload", new { file = file, url = url });
        }

        public async Task<CommandStatus> ShutdownAsync()
        {
            return await jsonRpc.InvokeWithParameterObjectAsync<CommandStatus>("shutdown", new { });
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    var shutdownResult = this.ShutdownAsync().Result;

                    if(shutdownResult != CommandStatus.Ok && !this.process.HasExited)
                    {
                        this.process.Kill();
                    }

                    this.process.Dispose();
                    jsonRpc.Dispose();
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
