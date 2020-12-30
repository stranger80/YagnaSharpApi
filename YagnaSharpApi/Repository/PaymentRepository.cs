using AutoMapper;
using Golem.PaymentApi.Client.Api;
using Golem.PaymentApi.Client.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using YagnaSharpApi.Entities;

namespace YagnaSharpApi.Repository
{
    public class PaymentRepository
    {
        public IRequestorApi RequestorApi { get; set; }
        public IMapper Mapper { get; set; }

        public PaymentRepository(IRequestorApi requestorApi, IMapper mapper)
        {
            this.RequestorApi = requestorApi;
            this.Mapper = mapper;
        }

        public async Task<AllocationEntity> CreateAllocationAsync(decimal amount, DateTime? expires, bool makeDeposit = false)
        {
            var allocationTimeout = expires ?? DateTime.Now.AddMinutes(30);
            var allocation = new Allocation(null, null, $"{amount}", allocationTimeout, makeDeposit);

            try
            { 
                var newAllocation = await this.RequestorApi.CreateAllocationAsync(allocation);

                var result = this.Mapper.Map<AllocationEntity>(newAllocation);

                return result;
            }
            catch(Exception exc)
            {
                throw;
            }
        }

    }
}
