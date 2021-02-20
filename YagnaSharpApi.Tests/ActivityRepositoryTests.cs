using Golem.ActivityApi.Client.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using YagnaSharpApi.Engine;
using YagnaSharpApi.Engine.Events;
using YagnaSharpApi.Engine.MarketStrategy;
using YagnaSharpApi.Mapper;
using YagnaSharpApi.Repository;
using YagnaSharpApi.Utils;

namespace YagnaSharpApi.Tests
{
    [TestClass]
    public class ActivityRepositoryTests
    {
        public TestUtils Utils { get; set; } = new TestUtils();

        public MarketStrategyTests MarketStrategyTests { get; set; } = new MarketStrategyTests();
        public AgreementPoolTests AgreementPoolTests { get; set; } = new AgreementPoolTests();

        static ActivityRepositoryTests()
        {
            MapConfig.Init();
        }

        [TestMethod]
        public async Task ActivityRepository_ExecDeployCommand()
        {
            using (var activityRepo = this.Utils.CreateActivityRepository())
            {
                // Cretae activity using agreement and run Deploy command. Make sure it completes and succeeds.
                var commandSuccessful = false;

                await this.AgreementPoolTests.DoWithDefaultAgreementPool(async agreementPool =>
                {
                    await await agreementPool.UseAgreementAsync(async agreement =>
                    {
                        var commands = new List<ExeScriptCommand>()
                            {
                                new DeployCommand(new object())
                            };

                        var activity = await activityRepo.CreateActivityAsync(agreement.Agreement);
                        {
                            await foreach (var result in activity.ExecAsync(commands))
                            {
                                Assert.IsNotNull(result);
                                if (result.Result == ExeScriptCommandResult.ResultEnum.Ok)
                                {
                                    commandSuccessful = true;
                                }
                                else
                                {
                                    Assert.Fail($"ExeScript command failed: [{result.Message}] {result.Stderr}");
                                }
                            }
                        }
                        activity.Dispose();
                    });
                });

                Assert.IsTrue(commandSuccessful);

            }
        }

    }
}
