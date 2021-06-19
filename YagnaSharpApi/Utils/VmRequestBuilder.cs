using DnsClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YagnaSharpApi.Utils
{
    public class VmRequestBuilder
    {
        protected const string DEFAULT_REPO_URL = "_girepo._tcp.dev.golem.network";
        protected const string FALLBACK_REPO_URL = "http://yacn2.dev.golem.network:8000";

        public static IPackage Repo(string imageHash, decimal minMemGiB = 0.5m, decimal minStorageGiB = 2.0m)
        {

            return new VmPackage(ResolveRepoUrl(DEFAULT_REPO_URL), imageHash, new VmConstraints(minMemGiB, minStorageGiB));
        }

        public static string ResolveRepoUrl(string address)
        {
            try
            {
                var client = new LookupClient();

                var result = client.Query(address, QueryType.SRV);

                foreach (var srvRecord in client.Query(address, QueryType.SRV).Answers.SrvRecords())
                {
                    return $"http://{srvRecord.Target}:{srvRecord.Port}";
                }
            }
            catch(Exception exc)
            {
                // TODO log warning
            }

            return FALLBACK_REPO_URL;
        }
    }
}
