/* 
 * Yagna Market API
 *
 *  ## Yagna Market The Yagna Market is a core component of the Yagna Network, which enables computational Offers and Demands circulation. The Market is open for all entities willing to buy computations (Demands) or monetize computational resources (Offers). ## Yagna Market API The Yagna Market API is the entry to the Yagna Market through which Requestors and Providers can publish their Demands and Offers respectively, find matching counterparty, conduct negotiations and make an agreement.  This version of Market API conforms with capability level 1 of the <a href=\"https://docs.google.com/document/d/1Zny_vfgWV-hcsKS7P-Kdr3Fb0dwfl-6T_cYKVQ9mkNg\"> Market API specification</a>.  Market API contains two roles: Requestors and Providers which are symmetrical most of the time (excluding agreement phase). 
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
using Golem.MarketApi.Client.Api;
using Golem.MarketApi.Client.Model;
using Golem.MarketApi.Client.Client;
using System.Reflection;
using Newtonsoft.Json;

namespace Golem.MarketApi.Client.Test
{
    /// <summary>
    ///  Class for testing AgreementRejectedEvent
    /// </summary>
    /// <remarks>
    /// This file is automatically generated by OpenAPI Generator (https://openapi-generator.tech).
    /// Please update the test case below to test the model.
    /// </remarks>
    public class AgreementRejectedEventTests : IDisposable
    {
        // TODO uncomment below to declare an instance variable for AgreementRejectedEvent
        //private AgreementRejectedEvent instance;

        public AgreementRejectedEventTests()
        {
            // TODO uncomment below to create an instance of AgreementRejectedEvent
            //instance = new AgreementRejectedEvent();
        }

        public void Dispose()
        {
            // Cleanup when everything is done.
        }

        /// <summary>
        /// Test an instance of AgreementRejectedEvent
        /// </summary>
        [Fact]
        public void AgreementRejectedEventInstanceTest()
        {
            // TODO uncomment below to test "IsInstanceOfType" AgreementRejectedEvent
            //Assert.IsInstanceOfType<AgreementRejectedEvent> (instance, "variable 'instance' is a AgreementRejectedEvent");
        }


        /// <summary>
        /// Test the property 'Reason'
        /// </summary>
        [Fact]
        public void ReasonTest()
        {
            // TODO unit test for the property 'Reason'
        }

    }

}
