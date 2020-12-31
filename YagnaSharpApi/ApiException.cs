using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace YagnaSharpApi
{
    public class ApiException : Exception
    {
        public HttpStatusCode? StatusCode { get; protected set; }
        public ErrorMessage ErrorMessage { get; protected set; }

        public ApiException(string message, HttpStatusCode? statusCode = null, ErrorMessage errorMessage = null)
            : base(message)
        {
            this.StatusCode = statusCode;
            this.ErrorMessage = errorMessage;
        }


        public override string ToString()
        {
            return $"{base.ToString()}\n\rStatusCode: {this.StatusCode}\n\rError message: {this.ErrorMessage?.Message}";
        }


    }

    public class ErrorMessage
    {
        public string Message { get; set; }
    }
}
