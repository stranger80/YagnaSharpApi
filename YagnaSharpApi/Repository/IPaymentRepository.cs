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
    public interface IPaymentRepository
    {
        Task<AllocationEntity> CreateAllocationAsync(decimal amount, DateTime? expires, bool makeDeposit = false);
        Task<AllocationEntity> GetAllocationAsync(string allocationId);
        Task ReleaseAllocationAsync(string allocationId);
        IAsyncEnumerable<InvoiceEventEntity> GetInvoiceEventsAsync();

    }
}
