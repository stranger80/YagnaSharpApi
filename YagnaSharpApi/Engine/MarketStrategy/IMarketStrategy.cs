using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using YagnaSharpApi.Entities;
using YagnaSharpApi.Utils;

namespace YagnaSharpApi.Engine.MarketStrategy
{
    public static class MarketStrategyConsts
    {
        public const float SCORE_REJECTED = -1.0f;
        public const float SCORE_NEUTRAL = 0.0f;
        public const float SCORE_TRUSTED = 100.0f;
    }

    public interface IMarketStrategy
    {
        Task DecorateDemandAsync(DemandBuilder demand);
        Task<float> ScoreOfferAsync(ProposalEntity offer);
    }
}
