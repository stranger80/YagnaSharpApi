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
        public MarketStrategyTests MarketStrategyTests { get; set; } = new MarketStrategyTests();
        public AgreementPoolTests AgreementPoolTests { get; set; } = new AgreementPoolTests();

        static ActivityRepositoryTests()
        {
            MapConfig.Init();
        }

        public ActivityRepository CreateActivityRepository(bool withApiKey = true)
        {
            var config = new ApiConfiguration();

            if (withApiKey)
                config.AppKey = Environment.GetEnvironmentVariable("YAGNA_APP_KEY") ?? "e3f31abc20ac4ea19513d0d7089b79ac";

            var factory = new ApiFactory(config);

            var controlApi = factory.GetActivityRequestorControlApi();

            return new ActivityRepository(controlApi, MapConfig.Config.CreateMapper());

        }


        [TestMethod]
        public async Task ActivityRepository_ExecDeployCommand()
        {
            using (var activityRepo = this.CreateActivityRepository())
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

                        var activity = await activityRepo.CreateActivityAsync(agreement);
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
