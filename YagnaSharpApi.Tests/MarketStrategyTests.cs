using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YagnaSharpApi.Engine.MarketStrategy;
using YagnaSharpApi.Mapper;
using YagnaSharpApi.Repository;
using YagnaSharpApi.Utils;

namespace YagnaSharpApi.Tests
{
    [TestClass]
    public class MarketStrategyTests
    {
        static MarketStrategyTests()
        {
            MapConfig.Init();
        }

        public MarketRepository CreateMarketRepository(bool withApiKey = true)
        {
            var config = new ApiConfiguration();

            if (withApiKey)
                config.AppKey = Environment.GetEnvironmentVariable("YAGNA_APP_KEY") ?? "e3f31abc20ac4ea19513d0d7089b79ac";

            var factory = new ApiFactory(config);

            var marketApi = factory.GetMarketRequestorApi();

            return new MarketRepository(marketApi, MapConfig.Config.CreateMapper());

        }

        [TestMethod]
        public async Task DummyMarketStrategy_GracefullyReturns_First3Proposals()
        {
            using (var repo = this.CreateMarketRepository())
            {
                var marketStrategy = new DummyMarketStrategy(repo);


                var demand = new DemandBuilder();

                demand.Add(new Dictionary<string, object>()
                    {
                        { "golem.node.debug.subnet", "community.3" },
                        { "golem.node.id.name", "yasharptest" },
                        { "golem.srv.comp.expiration", DateHelper.GetJavascriptTimestamp(DateTime.Now.AddMinutes(10)) },
                        { "golem.srv.comp.task_package",
                            "hash://sha3:9a3b5d67b0b27746283cb5f287c13eab1beaa12d92a9f536b747c7ae:http://3.249.139.167:8000/local-image-c76719083b.gvmi"},
                    });

                demand.Ensure("(&(golem.inf.mem.gib>0.5)(golem.inf.storage.gib>1)(golem.com.pricing.model=linear))");

                var offers = marketStrategy.FindOffersAsync(demand);
                int i = 0;
                await foreach (var offer in offers)
                {
                    i++;

                    if (i >= 3)
                        break;
                }

                Assert.AreEqual(3, i);                

            }
        }
    }
}
