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
    ///  Class for testing InvoiceAcceptedEvent
    /// </summary>
    /// <remarks>
    /// This file is automatically generated by OpenAPI Generator (https://openapi-generator.tech).
    /// Please update the test case below to test the model.
    /// </remarks>
    public class InvoiceAcceptedEventTests : IDisposable
    {
        // TODO uncomment below to declare an instance variable for InvoiceAcceptedEvent
        //private InvoiceAcceptedEvent instance;

        public InvoiceAcceptedEventTests()
        {
            // TODO uncomment below to create an instance of InvoiceAcceptedEvent
            //instance = new InvoiceAcceptedEvent();
        }

        public void Dispose()
        {
            // Cleanup when everything is done.
        }

        /// <summary>
        /// Test an instance of InvoiceAcceptedEvent
        /// </summary>
        [Fact]
        public void InvoiceAcceptedEventInstanceTest()
        {
            // TODO uncomment below to test "IsInstanceOfType" InvoiceAcceptedEvent
            //Assert.IsInstanceOfType<InvoiceAcceptedEvent> (instance, "variable 'instance' is a InvoiceAcceptedEvent");
        }


        /// <summary>
        /// Test the property 'InvoiceId'
        /// </summary>
        [Fact]
        public void InvoiceIdTest()
        {
            // TODO unit test for the property 'InvoiceId'
        }

    }

}
