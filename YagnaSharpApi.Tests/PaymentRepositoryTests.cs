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

        public TestUtils Utils { get; set; } = new TestUtils();

        [TestMethod]
        
        public async Task PaymentRepository_CreateAllocation_FailsWithNoAppKey()
        {
            var repo = this.Utils.CreatePaymentRepository(false);

            await Assert.ThrowsExceptionAsync<ApiException>(async () => await repo.CreateAllocationAsync(null, null, 0.1m, DateTime.Now.AddMinutes(5), false));
        }

        [TestMethod]
        public async Task PaymentRepository_CreateAllocation_Succeeds()
        {
            var repo = this.Utils.CreatePaymentRepository();

            var allocation = await repo.CreateAllocationAsync(null, null, 0.1m, DateTime.Now.AddMinutes(5), false);

            Assert.IsNotNull(allocation);
            Assert.IsNotNull(allocation.AllocationId);

        }

        [TestMethod]
        public async Task PaymentRepository_GetAndReleaseAllocation_Succeeds()
        {
            var repo = this.Utils.CreatePaymentRepository();

            var allocation = await repo.CreateAllocationAsync(null, null, 0.1m, DateTime.Now.AddMinutes(5), false);

            Assert.IsNotNull(allocation);
            Assert.IsNotNull(allocation.AllocationId);

            var allocation2 = await repo.GetAllocationAsync(allocation.AllocationId);

            Assert.AreEqual(allocation.AllocationId, allocation2.AllocationId);

            await repo.ReleaseAllocationAsync(allocation.AllocationId);

            await Assert.ThrowsExceptionAsync<ApiException>(async () => await repo.GetAllocationAsync(allocation.AllocationId));

        }

        [TestMethod]
        public async Task PaymentRepository_ReleaseAllAllocations_Succeeds()
        {
            var repo = this.Utils.CreatePaymentRepository();

            var allocations = await repo.GetAllocationsAsync();

            Assert.IsNotNull(allocations);

            foreach (var allocation in allocations)
            {
                await repo.ReleaseAllocationAsync(allocation.AllocationId);
            }

        }

    }
}
