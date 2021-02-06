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
    public class AgreementPoolTests
    {
        public MarketStrategyTests MarketStrategyTests { get; set; } = new MarketStrategyTests();

        static AgreementPoolTests()
        {
            MapConfig.Init();
        }

        public async Task DoWithDefaultAgreementPool(Func<AgreementPool, Task> work)
        {
            using (var marketRepo = this.MarketStrategyTests.CreateMarketRepository())
            using (var paymentRepo = this.MarketStrategyTests.CreatePaymentRepository())
            {
                var marketStrategy = await this.MarketStrategyTests.CreateDummyMarketStrategyAsync(marketRepo, paymentRepo);

                var demand = new DemandBuilder();

                demand.Add(new Dictionary<string, object>()
                    {
                        { "golem.node.debug.subnet", "devnet-alpha.4" },
                        { "golem.node.id.name", "yasharptest" },
                        { "golem.srv.comp.expiration", DateHelper.GetJavascriptTimestamp(DateTime.Now.AddMinutes(10)) },
                        { "golem.srv.comp.task_package",
                            "hash://sha3:9a3b5d67b0b27746283cb5f287c13eab1beaa12d92a9f536b747c7ae:http://3.249.139.167:8000/local-image-c76719083b.gvmi"},
                    });

                demand.Ensure("(&(golem.inf.mem.gib>0.5)(golem.inf.storage.gib>1)(golem.com.pricing.model=linear))");

                var agreementPool = new AgreementPool();

                // Set the event handler and watch for events
                bool isAgreementConfirmedEventFired = false;
                bool isAgreementCreatedEventFired = false;

                agreementPool.OnAgreementEvent += (sender, ev) =>
                {
                    Debug.WriteLine($"Agreement Event {ev} received...");
                    switch (ev)
                    {
                        case AgreementCreated acev:
                            isAgreementCreatedEventFired = true;
                            break;
                        case AgreementConfirmed acev:
                            isAgreementConfirmedEventFired = true;
                            break;
                    }
                };

                // Gather proposals

                var offers = marketStrategy.FindOffersAsync(demand);
                int i = 0;
                await foreach (var offer in offers)
                {
                    agreementPool.AddProposal(offer.Item1, offer.Item2);

                    i++;

                    if (i >= 3)
                        break;
                }

                await work(agreementPool);
            }

        }


        [TestMethod]
        public async Task AgreementPool_NegotiatesAgreements()
        {

            // Attempt to negotiate Agreement and execute a dummy job using this agreement
            bool isAgreementConfirmedEventFired = false;
            bool isAgreementCreatedEventFired = false;
            var agreementConfirmed = false;

            await this.DoWithDefaultAgreementPool(async agreementPool =>
            {
                // Hook into events to observe and validate
                agreementPool.OnAgreementEvent += (sender, ev) =>
                {
                    Debug.WriteLine($"Agreement Event {ev} received...");
                    switch (ev)
                    {
                        case AgreementCreated acev:
                            isAgreementCreatedEventFired = true;
                            break;
                        case AgreementConfirmed acev:
                            isAgreementConfirmedEventFired = true;
                            break;
                    }
                };

                // Trigger agreement negotiation
                await agreementPool.UseAgreementAsync(async agreement =>
                    {
                        Assert.IsNotNull(agreement);
                        agreementConfirmed = true;
                    });
            });

            Assert.IsTrue(agreementConfirmed);
            Assert.IsTrue(isAgreementCreatedEventFired);
            Assert.IsTrue(isAgreementConfirmedEventFired);
        }

    }
}
