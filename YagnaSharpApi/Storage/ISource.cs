using System;
using System.Collections.Generic;
using System.Text;

namespace YagnaSharpApi.Storage
{
    public interface ISource
    {
        string DownloadUrl();

        long ContentLength();
    }
}
