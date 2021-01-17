using System;
using System.Collections.Generic;
using System.Text;

namespace YagnaSharpApi.Storage
{
    public class PubLink
    {
        /// <summary>
        /// GFTP Url at which local file is exposed. 
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// Path to file on local filesystem.
        /// </summary>
        public string File { get; set; }
    }
}
