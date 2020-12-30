using Golem.Common.Client.Client;
using Golem.PaymentApi.Client.Api;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using YagnaSharpApi.Mapper;
using YagnaSharpApi.Repository;

namespace YagnaSharpApi.Tests
{
    [TestClass]
    public class PaymentRepositoryTests
    {
        [TestMethod]
        
        public async Task PaymentRepository_CreateAllocation_FailsWithNoAppKey()
        {
            MapConfig.Init();

            var config = new ApiConfiguration();

            var factory = new ApiFactory(config);

            var paymentApi = factory.GetPaymentRequestorApi();

            var repo = new PaymentRepository(paymentApi, MapConfig.Config.CreateMapper());

            await Assert.ThrowsExceptionAsync<Exception>(async () => await repo.CreateAllocationAsync(0.1m, DateTime.Now.AddMinutes(5), false));
        }

        [TestMethod]
        public async Task PaymentRepository_CreateAllocation_Succeeds()
        {
            MapConfig.Init();

            var config = new ApiConfiguration();

            config.AppKey = "e3f31abc20ac4ea19513d0d7089b79ac";

            var factory = new ApiFactory(config);

            var paymentApi = factory.GetPaymentRequestorApi();

            var repo = new PaymentRepository(paymentApi, MapConfig.Config.CreateMapper());

            var allocation = await repo.CreateAllocationAsync(0.1m, DateTime.Now.AddMinutes(5), false);

            Assert.IsNotNull(allocation);
            Assert.IsNotNull(allocation.AllocationId);

        }

    }
}
