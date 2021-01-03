using AutoMapper;
using Golem.PaymentApi.Client.Api;
using Golem.PaymentApi.Client.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using YagnaSharpApi.Entities;
using YagnaSharpApi.Entities.Events;

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

        public async Task<AllocationEntity> GetAllocationAsync(string allocationId)
        {
            try
            {
                var existingAllocation = await this.RequestorApi.GetAllocationAsync(allocationId);

                var result = this.Mapper.Map<AllocationEntity>(existingAllocation);

                return result;
            }
            catch (Exception exc)
            {
                throw;
            }
        }

        public async Task ReleaseAllocationAsync(string allocationId)
        {
            try
            {
                await this.RequestorApi.ReleaseAllocationAsync(allocationId);
            }
            catch (Exception exc)
            {
                throw;
            }
        }

        public async IAsyncEnumerable<InvoiceEventEntity> GetInvoiceEventsAsync()
        {
            // implement ongoing listening of the Invoice Event flow, 
            // decode incoming events
            // for new Invoices - call GetInvoice(invoiceId) to retrieve the Invoice details from payment API

            yield return null;

            throw new NotImplementedException();
        }

    }
}
