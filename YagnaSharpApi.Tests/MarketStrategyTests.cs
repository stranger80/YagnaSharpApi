using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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

        public TestUtils Utils { get; set; } = new TestUtils();

        public async Task<DummyMarketStrategy> CreateDummyMarketStrategyAsync(IMarketRepository marketRepo, IPaymentRepository paymentRepo)
        {
            var accounts = await paymentRepo.GetAccountsAsync();

            if (!accounts.Any())
            {
                throw new Exception("No sender accounts found, make sure to run 'yagna payment init --sender'...");
            }

            var settings = new MarketStrategyConditions()
            {
                PaymentPlatforms = accounts.Select(acc => acc.Platform).ToList()
            };


            var marketStrategy = new DummyMarketStrategy(marketRepo, settings);

            marketStrategy.OnMarketEvent += MarketStrategy_OnMarketEvent;

            return marketStrategy;
        }

        [TestMethod]
        public async Task DummyMarketStrategy_GracefullyReturns_First3Proposals()
        {
            using (var marketRepo = this.Utils.CreateMarketRepository())
            using (var paymentRepo = this.Utils.CreatePaymentRepository())
            {
                var marketStrategy = await CreateDummyMarketStrategyAsync(marketRepo, paymentRepo);

                var demand = new DemandBuilder();

                demand.Add(new Dictionary<string, object>()
                    {
                        { "golem.node.debug.subnet", TestConstants.SUBNET_TAG },
                        { "golem.node.id.name", "yasharptest" },
                        { "golem.srv.comp.expiration", DateHelper.GetJavascriptTimestamp(DateTime.Now.AddMinutes(10)) },
                        { "golem.srv.comp.task_package",
                            TestConstants.VM_TASK_PACKAGE},
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

        private void MarketStrategy_OnMarketEvent(object sender, Engine.Events.Event e)
        {
            Debug.WriteLine($"Market Event {e} received...");
        }
    }
}
