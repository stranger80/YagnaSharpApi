using System;
using System.Collections.Generic;
using System.Text;

namespace YagnaSharpApi.Utils
{
    public class VmRequest
    {
        public string PackageUrl { get; set; }
        public string PackageFormat { get; set; }

        public VmRequest(string packageUrl, string packageFormat)
        {
            this.PackageUrl = packageUrl;
            this.PackageFormat = packageFormat;
        }

        public IDictionary<string, object> ToPropertiesDictionary()
        {
            var result = new Dictionary<string, object>();
            result[Properties.SRV_COMP_TASK_PACKAGE] = this.PackageUrl;
            result[Properties.SRV_COMP_VM_PACKAGE_FORMAT] = this.PackageFormat;

            return result;
        }
    }
}
