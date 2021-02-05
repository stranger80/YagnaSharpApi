using AutoMapper;
using Golem.PaymentApi.Client.Api;
using Golem.PaymentApi.Client.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using YagnaSharpApi.Entities;
using YagnaSharpApi.Entities.Events;
using YagnaSharpApi.Utils;

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

        public async Task<AllocationEntity> CreateAllocationAsync(string address, string platform, decimal amount, DateTime? expires, bool makeDeposit = false)
        {
            var allocationTimeout = expires ?? DateTime.Now.AddMinutes(30);
            var allocation = new Allocation(address, platform, $"{amount}", allocationTimeout, makeDeposit);

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

        public async Task DecorateDemandAsync(IEnumerable<AllocationEntity> allocations, DemandBuilder demand)
        {
            try
            {
                var decorations = await this.RequestorApi.GetDemandDecorationsAsync(allocations.Select(alloc => alloc.AllocationId).ToList());

                var props = decorations.Properties.ToDictionary(prop => prop.Key, prop => (object)prop.Value);

                demand.Add(props);
                foreach(var cons in decorations.Constraints)
                    demand.Ensure(cons);

            }
            catch (Exception exc)
            {
                throw;
            }

        }


        public async IAsyncEnumerable<InvoiceEventEntity> GetInvoiceEventsAsync([EnumeratorCancellation]CancellationToken cancellationToken)
        {
            // implement ongoing listening of the Invoice Event flow, 
            // decode incoming events
            // for new Invoices - call GetInvoice(invoiceId) to retrieve the Invoice details from payment API

            DateTime afterTimestamp = DateTime.Now;

            while(!cancellationToken.IsCancellationRequested)
            {
                var events = await this.RequestorApi.GetInvoiceEventsAsync(5.0f, afterTimestamp, null, null, cancellationToken);

                foreach(var ev in events)
                {
                    if(ev.EventDate > afterTimestamp) // move the afterTimestamp pointer
                    {
                        afterTimestamp = ev.EventDate;
                    }

                    var eventEntity = this.Mapper.Map<InvoiceEventEntity>(ev);

                    switch(ev)
                    {
                        case InvoiceReceivedEvent irev:
                            eventEntity.Invoice = await this.GetInvoiceAsync(irev.InvoiceId);
                            break;
                    }

                    yield return eventEntity;
                }
            }
        }

        public async Task<InvoiceEntity> GetInvoiceAsync(string invoiceId)
        {
            try
            {
                var existingInvoice = await this.RequestorApi.GetInvoiceAsync(invoiceId);

                var result = this.Mapper.Map<InvoiceEntity>(existingInvoice);

                return result;
            }
            catch (Exception exc)
            {
                throw;
            }

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
