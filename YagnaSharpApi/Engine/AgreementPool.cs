using System;
using System.Collections.Generic;
using System.Text;
using YagnaSharpApi.Entities;

namespace YagnaSharpApi.Engine
{
    public class AgreementPool
    {
        public void AddProposal(float score, ProposalEntity proposal)
        {

        }

        public void UseAgreement(Action<AgreementEntity> job)
        {

        }

        protected AgreementEntity GetAgreement()
        {
            throw new NotImplementedException();
        }
    }
}
