using System;
using System.Collections.Generic;
using System.Text;

namespace YagnaSharpApi.Utils
{
    public class VmRequestBuilder
    {
        protected const string DEFAULT_REPO_URL = "http://yacn2.dev.golem.network:8000";

        public static IPackage Repo(string imageHash, decimal minMemGiB = 0.5m, decimal minStorageGiB = 2.0m)
        {
            return new VmPackage(DEFAULT_REPO_URL, imageHash, new VmConstraints(minMemGiB, minStorageGiB));
        }
    }
}
