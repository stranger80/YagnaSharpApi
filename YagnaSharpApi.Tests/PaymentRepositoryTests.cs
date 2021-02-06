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
        static PaymentRepositoryTests()
        {
            MapConfig.Init();
        }

        public PaymentRepository CreatePaymentRepository(bool withApiKey = true)
        {
            var config = new ApiConfiguration();

            if(withApiKey)
                config.AppKey = "e3f31abc20ac4ea19513d0d7089b79ac";

            var factory = new ApiFactory(config);

            var paymentApi = factory.GetPaymentRequestorApi();

            return new PaymentRepository(paymentApi, MapConfig.Config.CreateMapper());

        }

        [TestMethod]
        
        public async Task PaymentRepository_CreateAllocation_FailsWithNoAppKey()
        {
            var repo = this.CreatePaymentRepository(false);

            await Assert.ThrowsExceptionAsync<ApiException>(async () => await repo.CreateAllocationAsync(null, null, 0.1m, DateTime.Now.AddMinutes(5), false));
        }

        [TestMethod]
        public async Task PaymentRepository_CreateAllocation_Succeeds()
        {
            var repo = this.CreatePaymentRepository();

            var allocation = await repo.CreateAllocationAsync(null, null, 0.1m, DateTime.Now.AddMinutes(5), false);

            Assert.IsNotNull(allocation);
            Assert.IsNotNull(allocation.AllocationId);

        }

        [TestMethod]
        public async Task PaymentRepository_GetAndReleaseAllocation_Succeeds()
        {
            var repo = this.CreatePaymentRepository();

            var allocation = await repo.CreateAllocationAsync(null, null, 0.1m, DateTime.Now.AddMinutes(5), false);

            Assert.IsNotNull(allocation);
            Assert.IsNotNull(allocation.AllocationId);

            var allocation2 = await repo.GetAllocationAsync(allocation.AllocationId);

            Assert.AreEqual(allocation.AllocationId, allocation2.AllocationId);

            await repo.ReleaseAllocationAsync(allocation.AllocationId);

            await Assert.ThrowsExceptionAsync<ApiException>(async () => await repo.GetAllocationAsync(allocation.AllocationId));

        }

    }
}
