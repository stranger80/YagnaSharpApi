using System;
using System.Collections.Generic;
using System.Text;

namespace YagnaSharpApi.Storage
{
    public class GftpSource : ISource
    {
        public long Length { get; protected set; }
        public PubLink Link { get; protected set; }

        public GftpSource(long length, PubLink link)
        {
            this.Length = length;
            this.Link = link;
        }

        public long ContentLength()
        {
            return this.Length;
        }

        public string DownloadUrl()
        {
            return this.Link.Url;
        }
    }
}
