using System;
using System.Collections.Generic;
using System.Text;

namespace YagnaSharpApi.Storage
{
    public class Content
    {
        public long Length { get; set; }
        public IAsyncEnumerable<byte> ContentBytes { get; set; }
    }
}
