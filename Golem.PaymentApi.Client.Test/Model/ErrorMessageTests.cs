/* 
 * Yagna Payment API
 *
 *  Invoicing and Payments is a fundamental area of Yagna Ecosystem functionality. It includes aspects of communication between Requestor, Provider and a selected Payment Platform, which becomes crucial when Activities are executed in the context of negotiated Agreements. Yagna applications must be able to exercise various payment models, and the Invoicing/Payment-related communication is happening in parallel to Activity control communication. To define functional patterns of Requestor/Provider interaction in this area, Payment API is specified.  An important principle of the Yagna Payment API is that the actual payment transactions are hidden behind the Invoice flow. In other words, a Yagna Application on Requestor side isn’t expected to trigger actual payment transactions. Instead it is expected to receive and accept Invoices raised by the Provider - based on Application’s Invoice Accept notifications, the Payment API implementation orchestrates the payment via a configured Payment platform.  **NOTE: This specification is work-in-progress.** 
 *
 * The version of the OpenAPI document: 1.6.1
 * 
 * Generated by: https://github.com/openapitools/openapi-generator.git
 */


using Xunit;

using System;
using System.Linq;
using System.IO;
using System.Collections.Generic;
using Golem.PaymentApi.Client.Api;
using Golem.PaymentApi.Client.Model;
using Golem.PaymentApi.Client.Client;
using System.Reflection;
using Newtonsoft.Json;

namespace Golem.PaymentApi.Client.Test
{
    /// <summary>
    ///  Class for testing ErrorMessage
    /// </summary>
    /// <remarks>
    /// This file is automatically generated by OpenAPI Generator (https://openapi-generator.tech).
    /// Please update the test case below to test the model.
    /// </remarks>
    public class ErrorMessageTests : IDisposable
    {
        // TODO uncomment below to declare an instance variable for ErrorMessage
        //private ErrorMessage instance;

        public ErrorMessageTests()
        {
            // TODO uncomment below to create an instance of ErrorMessage
            //instance = new ErrorMessage();
        }

        public void Dispose()
        {
            // Cleanup when everything is done.
        }

        /// <summary>
        /// Test an instance of ErrorMessage
        /// </summary>
        [Fact]
        public void ErrorMessageInstanceTest()
        {
            // TODO uncomment below to test "IsInstanceOfType" ErrorMessage
            //Assert.IsInstanceOfType<ErrorMessage> (instance, "variable 'instance' is a ErrorMessage");
        }


        /// <summary>
        /// Test the property 'Message'
        /// </summary>
        [Fact]
        public void MessageTest()
        {
            // TODO unit test for the property 'Message'
        }

    }

}
