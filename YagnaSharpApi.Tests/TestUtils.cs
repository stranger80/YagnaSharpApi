using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YagnaSharpApi.Engine.MarketStrategy;
using YagnaSharpApi.Mapper;
using YagnaSharpApi.Repository;

namespace YagnaSharpApi.Tests
{
    public class TestUtils
    {
        static TestUtils()
        {
            MapConfig.Init();
        }

        public MarketRepository CreateMarketRepository(bool withApiKey = true)
        {
            var config = new ApiConfiguration();

            if (!withApiKey)
                config.AppKey = null;
            else
                config.AppKey = config.AppKey ?? "e3f31abc20ac4ea19513d0d7089b79ac";

            var factory = new ApiFactory(config);

            var marketApi = factory.GetMarketRequestorApi();

            return new MarketRepository(marketApi, MapConfig.Config.CreateMapper());

        }

        public ActivityRepository CreateActivityRepository(bool withApiKey = true)
        {
            var config = new ApiConfiguration();

            if (!withApiKey)
                config.AppKey = null;
            else
                config.AppKey = config.AppKey ?? "e3f31abc20ac4ea19513d0d7089b79ac";

            var factory = new ApiFactory(config);

            var controlApi = factory.GetActivityRequestorControlApi();

            return new ActivityRepository(controlApi, MapConfig.Config.CreateMapper());

        }

        public PaymentRepository CreatePaymentRepository(bool withApiKey = true)
        {
            var config = new ApiConfiguration();

            if (!withApiKey)
                config.AppKey = null;
            else
                config.AppKey = config.AppKey ?? "e3f31abc20ac4ea19513d0d7089b79ac";

            var factory = new ApiFactory(config);

            var paymentApi = factory.GetPaymentRequestorApi();

            return new PaymentRepository(paymentApi, MapConfig.Config.CreateMapper());

        }


    }
}
