using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using YagnaSharpApi.Utils;

namespace YagnaSharpApi.Tests
{
    [TestClass]
    public class VmRequestBuilderTests
    {
        [TestMethod]
        public async Task VmRequestBuilder_AllwosImageUrlOverride()
        {
            var package = VmRequestBuilder.Repo("imageHash", 0.5m, 2.0m, "https://www.dropbox.com/myImage.gvmi");

            var image = await package.ResolveUrlAsync();

            Assert.AreEqual("hash:sha3:imageHash:https://www.dropbox.com/myImage.gvmi", image);
        }
    }
}
