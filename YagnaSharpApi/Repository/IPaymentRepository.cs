using AutoMapper;
using Golem.PaymentApi.Client.Api;
using Golem.PaymentApi.Client.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using YagnaSharpApi.Entities;
using YagnaSharpApi.Entities.Events;
using YagnaSharpApi.Utils;

namespace YagnaSharpApi.Repository
{
    public interface IPaymentRepository
    {
        Task<IEnumerable<AccountEntity>> GetAccountsAsync();
        Task<AllocationEntity> CreateAllocationAsync(string address, string platform, decimal amount, DateTime? expires, bool makeDeposit = false);
        Task<AllocationEntity> GetAllocationAsync(string allocationId);
        Task ReleaseAllocationAsync(string allocationId);

        Task DecorateDemandAsync(IEnumerable<AllocationEntity> allocations, DemandBuilder demand);

        IAsyncEnumerable<InvoiceEventEntity> GetInvoiceEventsAsync(CancellationToken cancellationToken = default);
        Task AcceptInvoiceAsync(InvoiceEntity invoice, string amount, AllocationEntity allocation);

    }
}
