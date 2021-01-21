using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YagnaSharpApi.Entities;
using YagnaSharpApi.Entities.Events;
using YagnaSharpApi.Repository;
using YagnaSharpApi.Utils;
using YagnaSharpApi.Utils.PropertyModel;

namespace YagnaSharpApi.Engine.MarketStrategy
{
    public class DummyMarketStrategy : MarketStrategyBase
    {

        private IDictionary<string, decimal> maxForCounter = new Dictionary<string, decimal>()
        {
            { Com.ComLinear.FIXED, 0.05m },
            { Counters.DURATION_SEC, 0.002m },
            { Counters.CPU_SEC, 0.002m },
        };

        public DummyMarketStrategy(IMarketRepository repo) : base(repo)
        {
        }

        protected override async Task DecorateDemandAsync(DemandBuilder demand)
        {
            demand.Ensure($"({Properties.COM_PRICING_MODEL}={PropertyValues.COM_PRICING_MODEL_LINEAR})");
        }

        public override async Task<float> ScoreOfferAsync(ProposalEntity offer)
        {
            Com com = Com.FromProperties(offer.Properties);

            if(com.Scheme != PropertyValues.COM_SCHEME_PAYU)
            {
                return MarketStrategyConsts.SCORE_REJECTED;
            }

            var coeffs = com.Linear.Coeffs;

            foreach(var (counter, price) in coeffs.Keys.Select(key => (key, coeffs[key])))
            {
                if(! this.maxForCounter.ContainsKey(counter))
                {
                    return MarketStrategyConsts.SCORE_REJECTED;
                }
                if(price > this.maxForCounter[counter])
                {
                    return MarketStrategyConsts.SCORE_REJECTED;
                }
            }

            return MarketStrategyConsts.SCORE_NEUTRAL;
        }
    }
}
