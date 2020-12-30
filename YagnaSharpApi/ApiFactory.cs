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

        private Configuration proxyConfig;

        public ApiFactory(ApiConfiguration config)
        {
            this.Configuration = config;
            this.proxyConfig = new Configuration(
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
                return new Exception("AppKey missing.");
            }

            if (!String.IsNullOrEmpty(response.ErrorText))
            {
                return new Exception($"Method {methodName} returned error: {response.ErrorText}");
            }

            switch (response.StatusCode)
            {
                case System.Net.HttpStatusCode.OK:
                case System.Net.HttpStatusCode.Created:
                    return null;
                default:
                    try
                    {
                        // attempt to decode the error message
                        var errorMessage = JsonConvert.DeserializeObject<Golem.PaymentApi.Client.Model.ErrorMessage>(response.RawContent);
                        return new Exception($"Method {methodName} returned HTTP status {response.StatusCode} with error message: {errorMessage.Message}");
                    }
                    catch (Exception exc)
                    {
                        return new Exception($"Method {methodName} returned HTTP status {response.StatusCode}");
                    }

            }
        }

        public Golem.MarketApi.Client.Api.IRequestorApi GetMarketRequestorApi()
        {
            throw new NotImplementedException();
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
            var result = new Golem.PaymentApi.Client.Api.RequestorApi(this.proxyConfig);

            result.ExceptionFactory = ApiExceptionFactory;

            return result;
        }

    }
}
