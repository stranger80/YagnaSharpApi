using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace YagnaSharpApi.Storage
{
    public interface IInputStorageProvider
    {
        Task<ISource> UploadStream(IAsyncEnumerable<byte> stream);
        Task<ISource> UploadBytes(byte[] data);
        Task<ISource> UploadFile(string file);
    }


}
