using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace YagnaSharpApi.Storage
{
    public interface IOutputStorageProvider
    {
        Task<IDestination> NewDestination(string destinationFile = null);
    }
}
