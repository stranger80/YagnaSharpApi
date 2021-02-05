using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
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

    public class MarketStrategyConditions
    {
        public IEnumerable<string> PaymentPlatforms { get; set; }
    }

    public interface IMarketStrategy
    {
        /// <summary>
        /// Market Strategy conditions and parameters
        /// </summary>
        MarketStrategyConditions Conditions { get; set; }

        /// <summary>
        /// Perform the market subscription and negotiation for a given demand
        /// </summary>
        /// <param name="demand"></param>
        /// <returns></returns>
        IAsyncEnumerable<(float, ProposalEntity)> FindOffersAsync(DemandBuilder demand, CancellationToken cancellationToken = default);
        event EventHandler<Events.Event> OnMarketEvent;
    }
}
