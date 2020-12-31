using Golem.Common.Client.Client;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
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
                config.AppKey = "e3f31abc20ac4ea19513d0d7089b79ac";

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
                { "golem.node.debug.subnet", "community.3" },
                { "golem.node.id.name", "yasharptest" },
                { "golem.srv.comp.expiration", DateHelper.GetJavascriptTimestamp(DateTime.Now.AddMinutes(10)) },
                { "golem.srv.comp.task_package",
                    "hash://sha3:9a3b5d67b0b27746283cb5f287c13eab1beaa12d92a9f536b747c7ae:http://3.249.139.167:8000/local-image-c76719083b.gvmi"},
            };

            var constraints = "(&(golem.inf.mem.gib>0.5)(golem.inf.storage.gib>1)(golem.com.pricing.model=linear))";

            var subscription = await repo.SubscribeDemandAsync(props, constraints);

            Assert.IsNotNull(subscription);
            Assert.IsNotNull(subscription.SubscriptionId);

            try
            {
                var offersAsync = repo.CollectOffersAsync(subscription.SubscriptionId, 10);

                await foreach(var ev in offersAsync)
                {
                    switch(ev)
                    {
                        case ProposalEventEntity proposalEvent:
                            Assert.IsNotNull(proposalEvent.Proposal);
                            break;
                        case PropertyQueryEventEntity propQueryEvent:
                            break;
                        case ProposalRejectedEventEntity propRejectedEvent:
                            break;
                    };
                }
            }
            finally
            {
                await repo.UnsubscribeDemandAsync(subscription.SubscriptionId);
            }
        }


    }
}
