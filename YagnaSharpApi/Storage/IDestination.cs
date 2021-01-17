using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace YagnaSharpApi.Storage
{
    public interface IDestination
    {
        string UploadUrl();
        Content DownloadStream();
        Task DownloadFile(string destinationFile);

    }
}
