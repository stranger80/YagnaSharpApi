using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using YagnaSharpApi.Storage;

namespace YagnaSharpApi.Tests
{
    [TestClass]
    public class GftpTests
    {
        [TestMethod]
        public async Task GftpWrapper_StartsGftpClient_Correctly_Async()
        {
            using (var wrapper = new GftpWrapper())
            {
                var ver = await wrapper.VersionAsync();

                Assert.IsNotNull(ver);
            }

        }

        [TestMethod]
        public async Task GftpWrapper_ShutsdownGftpClient_Correctly_Async()
        {
            using (var wrapper = new GftpWrapper())
            {
                var ver = await wrapper.ShutdownAsync();

                Assert.IsNotNull(ver);
            }

        }

        [TestMethod]
        public async Task GftpWrapper_PublishFile_WorksCorrectly_Async()
        {
            var tempFileName = Path.GetTempFileName();
            try
            {
                using (var wrapper = new GftpWrapper())
                {

                    File.WriteAllText(tempFileName, "sample content");

                    try
                    {

                        var files = await wrapper.PublishAsync(new string[] { tempFileName });

                        Assert.IsNotNull(files);

                        var closeResult = await wrapper.CloseAsync(files.Select(file => file.Url));

                        Assert.AreEqual(CommandStatus.Ok, closeResult.First());

                    }
                    catch(Exception exc)
                    {
                        Assert.Fail(exc.Message);
                    }
                }

            }
            finally
            {
                File.Delete(tempFileName);
            }
        }

    }
}
