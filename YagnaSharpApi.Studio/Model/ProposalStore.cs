using System;
using System.Collections.Generic;
using System.Text;
using YagnaSharpApi.Entities;

namespace YagnaSharpApi.Studio.Model
{
    class ProposalStore
    {

        protected Dictionary<string, ProposalEntity> proposalsById = new Dictionary<string, ProposalEntity>();

        public ProposalStore()
        {

        }

        public ProposalEntity GetRootProposal(ProposalEntity proposal)
        {
            var curProposal = proposal;
            string curPrevProId = proposal.PrevProposalId;

            while (curPrevProId != null)
            {
                if (this.proposalsById.ContainsKey(curPrevProId))
                {
                    curProposal = this.proposalsById[curPrevProId];
                    curPrevProId = curProposal?.PrevProposalId;
                }
                else
                    return null;
            }

            return curProposal;
        }

        public void AddProposal(ProposalEntity proposal)
        {
            proposalsById[proposal.ProposalId] = proposal;
        }

    }
}
