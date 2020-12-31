using Golem.Common.Client.Client;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace YagnaSharpApi
{
    public class ApiFactory
    {
        public ApiConfiguration Configuration { get; set; }

        private Configuration marketProxyConfig;
        private Configuration activityProxyConfig;
        private Configuration paymentProxyConfig;

        public ApiFactory(ApiConfiguration config)
        {
            this.Configuration = config;
            this.marketProxyConfig = new Configuration(
                new Dictionary<string, string>() { { "Authorization", "Bearer " + config.AppKey } },
                new Dictionary<string, string>(),
                new Dictionary<string, string>(),
                config.MarketApiRoot);
            this.activityProxyConfig = new Configuration(
                new Dictionary<string, string>() { { "Authorization", "Bearer " + config.AppKey } },
                new Dictionary<string, string>(),
                new Dictionary<string, string>(),
                config.ActivityApiRoot);
            this.paymentProxyConfig = new Configuration(
                new Dictionary<string, string>() { { "Authorization", "Bearer " + config.AppKey } },
                new Dictionary<string, string>(),
                new Dictionary<string, string>(),
                config.PaymentApiRoot);
        }

        private Exception ApiExceptionFactory(string methodName, IApiResponse response)
        {
            // TODO add customized ApiException with ErrorMessage structure.

            if(response.RawContent == "Missing application key")
            {
                return new ApiException("AppKey missing.");
            }

            if (response.StatusCode == 0 && !String.IsNullOrEmpty(response.ErrorText))
            {
                return new ApiException($"Method {methodName} returned error: {response.ErrorText}");
            }

            switch (response.StatusCode)
            {
                case System.Net.HttpStatusCode.OK:
                case System.Net.HttpStatusCode.Created:
                case System.Net.HttpStatusCode.Accepted:
                case System.Net.HttpStatusCode.NoContent:
                    return null;
                default:
                    try
                    {
                        // attempt to decode the error message
                        var errorMessage = JsonConvert.DeserializeObject<Golem.PaymentApi.Client.Model.ErrorMessage>(response.RawContent);
                        return new ApiException($"Method {methodName} returned erratic StatusCode", 
                            response.StatusCode, 
                            new ErrorMessage() { Message = errorMessage?.Message });
                    }
                    catch (Exception exc)
                    {
                        return new ApiException($"Method {methodName} returned erratic StatusCode", 
                            response.StatusCode);
                    }

            }
        }

        public Golem.MarketApi.Client.Api.IRequestorApi GetMarketRequestorApi()
        {
            var result = new Golem.MarketApi.Client.Api.RequestorApi(this.marketProxyConfig);

            result.ExceptionFactory = ApiExceptionFactory;

            return result;
        }

        public Golem.ActivityApi.Client.Api.IRequestorControlApi GetActivityRequestorControlApi()
        {
            throw new NotImplementedException();
        }

        public Golem.ActivityApi.Client.Api.IRequestorStateApi GetActivityRequestorStateApi()
        {
            throw new NotImplementedException();
        }

        public Golem.PaymentApi.Client.Api.IRequestorApi GetPaymentRequestorApi()
        {
            var result = new Golem.PaymentApi.Client.Api.RequestorApi(this.paymentProxyConfig);

            result.ExceptionFactory = ApiExceptionFactory;

            return result;
        }

    }
}
