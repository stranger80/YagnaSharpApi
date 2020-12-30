using System;
using System.Collections.Generic;
using System.Text;

namespace YagnaSharpApi
{
    public class ApiConfiguration
    {

        public const string DEFAULT_API_URL = "http://127.0.0.1:7465";
        public const string DEFAULT_MARKET_URL = "/market-api/v1";
        public const string DEFAULT_ACTIVITY_URL = "/activity-api/v1";
        public const string DEFAULT_PAYMENT_URL = "/payment-api/v1";
        public string MarketApiRoot { get; set; }
        public string ActivityApiRoot { get; set; }
        public string PaymentApiRoot { get; set; }

        public string AppKey { get; set; }

        public ApiConfiguration()
        {
            this.MarketApiRoot = DEFAULT_API_URL + DEFAULT_MARKET_URL;
            this.ActivityApiRoot = DEFAULT_API_URL + DEFAULT_ACTIVITY_URL;
            this.PaymentApiRoot = DEFAULT_API_URL + DEFAULT_PAYMENT_URL;
        }

    }
}
