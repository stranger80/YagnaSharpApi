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
    public class PaymentRepository : IPaymentRepository, IDisposable
    {
        private bool disposedValue;

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

        public async Task<IEnumerable<AccountEntity>> GetAccountsAsync()
        {
            try
            {
                var accounts = await this.RequestorApi.GetRequestorAccountsAsync();

                var result = this.Mapper.Map<IEnumerable<AccountEntity>>(accounts);

                return result;
            }
            catch (Exception exc)
            {
                throw;
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~PaymentRepository()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
