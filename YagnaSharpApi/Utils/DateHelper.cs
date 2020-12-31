using System;
using System.Collections.Generic;
using System.Text;

namespace YagnaSharpApi.Utils
{
    public static class DateHelper
    {
        public static long GetJavascriptTimestamp(DateTime timestamp)
        {
            DateTime unixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return (long)(timestamp.ToUniversalTime() - unixEpoch).TotalMilliseconds;
        }
    }
}
