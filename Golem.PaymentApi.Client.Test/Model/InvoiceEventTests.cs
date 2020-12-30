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
    ///  Class for testing InvoiceEvent
    /// </summary>
    /// <remarks>
    /// This file is automatically generated by OpenAPI Generator (https://openapi-generator.tech).
    /// Please update the test case below to test the model.
    /// </remarks>
    public class InvoiceEventTests : IDisposable
    {
        // TODO uncomment below to declare an instance variable for InvoiceEvent
        //private InvoiceEvent instance;

        public InvoiceEventTests()
        {
            // TODO uncomment below to create an instance of InvoiceEvent
            //instance = new InvoiceEvent();
        }

        public void Dispose()
        {
            // Cleanup when everything is done.
        }

        /// <summary>
        /// Test an instance of InvoiceEvent
        /// </summary>
        [Fact]
        public void InvoiceEventInstanceTest()
        {
            // TODO uncomment below to test "IsInstanceOfType" InvoiceEvent
            //Assert.IsInstanceOfType<InvoiceEvent> (instance, "variable 'instance' is a InvoiceEvent");
        }

        /// <summary>
        /// Test deserialize a InvoiceRejectedEvent from type InvoiceEvent
        /// </summary>
        [Fact]
        public void InvoiceRejectedEventDeserializeFromInvoiceEventTest()
        {
            // TODO uncomment below to test deserialize a InvoiceRejectedEvent from type InvoiceEvent
            //Assert.IsInstanceOf<InvoiceEvent>(JsonConvert.DeserializeObject<InvoiceEvent>(new InvoiceRejectedEvent().ToJson()));
        }
        /// <summary>
        /// Test deserialize a InvoiceFailedEvent from type InvoiceEvent
        /// </summary>
        [Fact]
        public void InvoiceFailedEventDeserializeFromInvoiceEventTest()
        {
            // TODO uncomment below to test deserialize a InvoiceFailedEvent from type InvoiceEvent
            //Assert.IsInstanceOf<InvoiceEvent>(JsonConvert.DeserializeObject<InvoiceEvent>(new InvoiceFailedEvent().ToJson()));
        }
        /// <summary>
        /// Test deserialize a InvoiceSettledEvent from type InvoiceEvent
        /// </summary>
        [Fact]
        public void InvoiceSettledEventDeserializeFromInvoiceEventTest()
        {
            // TODO uncomment below to test deserialize a InvoiceSettledEvent from type InvoiceEvent
            //Assert.IsInstanceOf<InvoiceEvent>(JsonConvert.DeserializeObject<InvoiceEvent>(new InvoiceSettledEvent().ToJson()));
        }
        /// <summary>
        /// Test deserialize a InvoiceCancelledEvent from type InvoiceEvent
        /// </summary>
        [Fact]
        public void InvoiceCancelledEventDeserializeFromInvoiceEventTest()
        {
            // TODO uncomment below to test deserialize a InvoiceCancelledEvent from type InvoiceEvent
            //Assert.IsInstanceOf<InvoiceEvent>(JsonConvert.DeserializeObject<InvoiceEvent>(new InvoiceCancelledEvent().ToJson()));
        }
        /// <summary>
        /// Test deserialize a InvoiceAcceptedEvent from type InvoiceEvent
        /// </summary>
        [Fact]
        public void InvoiceAcceptedEventDeserializeFromInvoiceEventTest()
        {
            // TODO uncomment below to test deserialize a InvoiceAcceptedEvent from type InvoiceEvent
            //Assert.IsInstanceOf<InvoiceEvent>(JsonConvert.DeserializeObject<InvoiceEvent>(new InvoiceAcceptedEvent().ToJson()));
        }
        /// <summary>
        /// Test deserialize a InvoiceReceivedEvent from type InvoiceEvent
        /// </summary>
        [Fact]
        public void InvoiceReceivedEventDeserializeFromInvoiceEventTest()
        {
            // TODO uncomment below to test deserialize a InvoiceReceivedEvent from type InvoiceEvent
            //Assert.IsInstanceOf<InvoiceEvent>(JsonConvert.DeserializeObject<InvoiceEvent>(new InvoiceReceivedEvent().ToJson()));
        }
        /// <summary>
        /// Test deserialize a PaymentReceivedEvent from type InvoiceEvent
        /// </summary>
        [Fact]
        public void PaymentReceivedEventDeserializeFromInvoiceEventTest()
        {
            // TODO uncomment below to test deserialize a PaymentReceivedEvent from type InvoiceEvent
            //Assert.IsInstanceOf<InvoiceEvent>(JsonConvert.DeserializeObject<InvoiceEvent>(new PaymentReceivedEvent().ToJson()));
        }

        /// <summary>
        /// Test the property 'EventType'
        /// </summary>
        [Fact]
        public void EventTypeTest()
        {
            // TODO unit test for the property 'EventType'
        }
        /// <summary>
        /// Test the property 'EventDate'
        /// </summary>
        [Fact]
        public void EventDateTest()
        {
            // TODO unit test for the property 'EventDate'
        }

    }

}
