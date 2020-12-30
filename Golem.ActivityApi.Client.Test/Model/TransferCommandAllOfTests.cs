/* 
 * Yagna Activity API
 *
 *  The Activity API can be perceived as controls which a Requestor-side application has to steer the execution of an Activity as specified in an Agreement which has been negotiated via the Market API/Protocol. This defines possible interactions between the Requestor application (via Activity API) and the generic components running on the Provider node, which host the Provider-side application code. The possible interactions imply a logical “execution environment” component, which is the host/container for the “payload” code. The “execution environment” is specified as an ExeUnit, with a generic interface via which a Provider node’s Activity Controller can operate the hosted code. It conforms with capability level 1 of the [Activity API specification] (https://docs.google.com/document/d/1BXaN32ediXdBHljEApmznSfbuudTU8TmvOmHKl0gmQM). 
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
using Golem.ActivityApi.Client.Api;
using Golem.ActivityApi.Client.Model;
using Golem.ActivityApi.Client.Client;
using System.Reflection;
using Newtonsoft.Json;

namespace Golem.ActivityApi.Client.Test
{
    /// <summary>
    ///  Class for testing TransferCommandAllOf
    /// </summary>
    /// <remarks>
    /// This file is automatically generated by OpenAPI Generator (https://openapi-generator.tech).
    /// Please update the test case below to test the model.
    /// </remarks>
    public class TransferCommandAllOfTests : IDisposable
    {
        // TODO uncomment below to declare an instance variable for TransferCommandAllOf
        //private TransferCommandAllOf instance;

        public TransferCommandAllOfTests()
        {
            // TODO uncomment below to create an instance of TransferCommandAllOf
            //instance = new TransferCommandAllOf();
        }

        public void Dispose()
        {
            // Cleanup when everything is done.
        }

        /// <summary>
        /// Test an instance of TransferCommandAllOf
        /// </summary>
        [Fact]
        public void TransferCommandAllOfInstanceTest()
        {
            // TODO uncomment below to test "IsInstanceOfType" TransferCommandAllOf
            //Assert.IsInstanceOfType<TransferCommandAllOf> (instance, "variable 'instance' is a TransferCommandAllOf");
        }


        /// <summary>
        /// Test the property 'Transfer'
        /// </summary>
        [Fact]
        public void TransferTest()
        {
            // TODO unit test for the property 'Transfer'
        }

    }

}
