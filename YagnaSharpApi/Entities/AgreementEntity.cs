using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using YagnaSharpApi.Repository;

namespace YagnaSharpApi.Entities
{
    public class AgreementEntity
    {
        public IMarketRepository Repository { get; set; }

        public ProposalEntity Proposal { get; set; }

        public string Id { get; set; }

        public async Task<bool> ConfirmAsync()
        {
            // TODO consider moving all the below into the repository, and only handle exceptions properly
            await this.Repository.ConfirmAgreementAsync(this.Id);

            try
            {
                await this.Repository.WaitForApprovalAsync(this.Id, 90, 100);
                return true;
            }
            catch (ApiException exc)
            {
                // TODO log exception details
                return false;
            }
        }

        public async Task TerminateAsync(ReasonEntity reason)
        {
            // TODO
        }
    }
}
