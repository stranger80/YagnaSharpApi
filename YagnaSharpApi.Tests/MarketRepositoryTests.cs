using Golem.Common.Client.Client;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using YagnaSharpApi.Entities.Events;
using YagnaSharpApi.Mapper;
using YagnaSharpApi.Repository;
using YagnaSharpApi.Utils;

namespace YagnaSharpApi.Tests
{
    [TestClass]
    public class MarketRepositoryTests
    {
        static MarketRepositoryTests()
        {
            MapConfig.Init();
        }

        public MarketRepository CreateMarketRepository(bool withApiKey = true)
        {
            var config = new ApiConfiguration();

            if(withApiKey)
                config.AppKey = Environment.GetEnvironmentVariable("YAGNA_APPKEY") ?? "e3f31abc20ac4ea19513d0d7089b79ac";

            var factory = new ApiFactory(config);

            var marketApi = factory.GetMarketRequestorApi();

            return new MarketRepository(marketApi, MapConfig.Config.CreateMapper());

        }

        [TestMethod]

        public async Task MarketRepository_SubscribeUnsubscribe_Succeeds()
        {
            var repo = this.CreateMarketRepository(true);

            var props = new Dictionary<string, object>()
                {
                { "golem.node.id.name", "test1" },
                { "golem.srv.comp.expiration", 1608556352458 },
                { "golem.srv.comp.task_package",
                    "hash://sha3:d5e31b2eed628572a5898bf8c34447644bfc4b5130cfc1e4f10aeaa1:http://172.19.0.1:8080/rust-wasi-tutorial.zip"},
            };

            var constraints = "(&(golem.inf.mem.gib>0.5)(golem.inf.storage.gib>1)(golem.com.pricing.model=linear))";

            var subscription = await repo.SubscribeDemandAsync(props, constraints);

            Assert.IsNotNull(subscription);
            Assert.IsNotNull(subscription.SubscriptionId);

            await repo.UnsubscribeDemandAsync(subscription.SubscriptionId);
        }

        [TestMethod]

        public async Task MarketRepository_CollectOfferProposals_Succeeds()
        {
            var repo = this.CreateMarketRepository(true);

            var props = new Dictionary<string, object>()
                {
                { "golem.node.debug.subnet", TestConstants.SUBNET_TAG },
                { "golem.node.id.name", "yasharptest" },
                { "golem.srv.comp.expiration", DateHelper.GetJavascriptTimestamp(DateTime.Now.AddMinutes(10)) },
                { "golem.srv.comp.task_package",
                    TestConstants.VM_TASK_PACKAGE},
            };

            var constraints = "(&(golem.inf.mem.gib>0.5)(golem.inf.storage.gib>1)(golem.com.pricing.model=linear))";

            using (var subscription = await repo.SubscribeDemandAsync(props, constraints))
            {

                Assert.IsNotNull(subscription);
                Assert.IsNotNull(subscription.SubscriptionId);

                var offersAsync = subscription.CollectOffersAsync(10);

                int cnt = 0;
                await foreach (var ev in offersAsync)
                {
                    switch (ev)
                    {
                        case ProposalEventEntity proposalEvent:
                            Assert.IsNotNull(proposalEvent.Proposal);
                            cnt++;
                            break;
                        case PropertyQueryEventEntity propQueryEvent:
                            break;
                        case ProposalRejectedEventEntity propRejectedEvent:
                            break;
                    };
                }

                Assert.IsTrue(cnt > 0);
            }
        }

        [TestMethod]

        public async Task MarketRepository_CollectOfferProposals_FailsGentlyForInvalidId()
        {
            var repo = this.CreateMarketRepository(true);

            try
            {
                var offersAsync = repo.CollectOffersAsync("e2d257c7ee364579b94eab17dddc5428-1400c219dcfefd119a75bdfd2ba5d9374cf0f32be89b3074b562d905ee7c0031", 10);

                await foreach (var ev in offersAsync) ;

                Assert.Fail("Exception expected...");
            }
            catch(ApiException exc)
            {

            }
        }


        [TestMethod]

        public async Task MarketRepository_CollectOfferProposals_CancelsGently()
        {
            var repo = this.CreateMarketRepository(true);

            var props = new Dictionary<string, object>()
                {
                { "golem.node.id.name", "yasharptest" },
                { "golem.srv.comp.expiration", DateHelper.GetJavascriptTimestamp(DateTime.Now.AddMinutes(10)) },
                { "golem.srv.comp.task_package",
                    TestConstants.VM_TASK_PACKAGE},
            };

            // set impossible constraint to make sure no offer arrives
            var constraints = "(&(golem.inf.mem.gib>0.5)(golem.inf.storage.gib>1500)(golem.com.pricing.model=linear))";

            var subscription = await repo.SubscribeDemandAsync(props, constraints);

            Assert.IsNotNull(subscription);
            Assert.IsNotNull(subscription.SubscriptionId);


            using (var cts = new CancellationTokenSource())
            {
                var stopWatch = new Stopwatch();

                Task.Run(() =>
                {
                    Thread.Sleep(200);
                    // Request cancellation.
                    cts.Cancel();
                });

                stopWatch.Start();

                try
                {
                    await Task.Run(async () =>
                    {
                        var offersAsync = repo.CollectOffersAsync(subscription.SubscriptionId, 30, cts.Token);

                        await foreach (var ev in offersAsync);
                    });

                    Assert.Fail(); // not expected to finish before cancel
                }
                catch(TaskCanceledException exc)
                {
                    stopWatch.Stop();

                    Assert.IsTrue(stopWatch.ElapsedMilliseconds < 1000);
                }
                finally
                {
                    await repo.UnsubscribeDemandAsync(subscription.SubscriptionId);
                }
            }


        }

    }
}
