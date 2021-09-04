using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using YagnaSharpApi.Engine.Commands;

namespace YagnaSharpApi.Tests
{
    [TestClass]
    public class IndexedWorkItemTests
    {
        [TestMethod]
        public async Task IndexedWorkItem_AwaitableWorks()
        {
            var iwi = new DeployStep();
            

            Task.Run(async () =>
            {
                await Task.Delay(200);
                iwi.StoreResult(new Golem.ActivityApi.Client.Model.ExeScriptCommandResult() { 
                    Result = Golem.ActivityApi.Client.Model.ExeScriptCommandResult.ResultEnum.Ok,
                    Message = "Result arrived"                
                });
            });
            
            var result = await iwi;

            Assert.IsNotNull(result);
            Assert.AreEqual("Result arrived", result.Message);

        }
    }
}
