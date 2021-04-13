using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace YagnaSharpApi.Utils
{
    public class VmPackage : IPackage
    {
        public const string GVMKIT_SQUASH = "gvmkit-squash";
        public string RepoUrl { get; set; }
        public string ImageHash { get; set; }
        public VmConstraints Constraints { get; set; }

        public VmPackage(string repoUrl, string imageHash, VmConstraints constraints)
        {
            this.RepoUrl = repoUrl;
            this.ImageHash = imageHash;
            this.Constraints = constraints;
        }

        public async Task DecorateDemandAsync(DemandBuilder builder)
        {
            var imageUrl = await this.ResolveUrlAsync();
            builder.Ensure(this.Constraints.ToString());
            builder.Add(new VmRequest(imageUrl, GVMKIT_SQUASH).ToPropertiesDictionary());
        }

        public async Task<string> ResolveUrlAsync()
        {
            using (var client = new HttpClient())
            {
                var linkUrl = $"{this.RepoUrl}/image.{this.ImageHash}.link";
                var resp = await client.GetAsync(linkUrl);

                if(resp.IsSuccessStatusCode)
                {
                    var imageUrl = await resp.Content.ReadAsStringAsync();
                    return $"hash:sha3:{this.ImageHash}:{imageUrl?.TrimEnd()}";
                }
                else
                {
                    throw new Exception($"Unable to get package link from url: [{linkUrl}]");
                }
            }
        }
    }
}
