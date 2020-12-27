/* 
 * Yagna Activity API
 *
 *  The Activity API can be perceived as controls which a Requestor-side application has to steer the execution of an Activity as specified in an Agreement which has been negotiated via the Market API/Protocol. This defines possible interactions between the Requestor application (via Activity API) and the generic components running on the Provider node, which host the Provider-side application code. The possible interactions imply a logical “execution environment” component, which is the host/container for the “payload” code. The “execution environment” is specified as an ExeUnit, with a generic interface via which a Provider node’s Activity Controller can operate the hosted code. It conforms with capability level 1 of the [Activity API specification] (https://docs.google.com/document/d/1BXaN32ediXdBHljEApmznSfbuudTU8TmvOmHKl0gmQM). 
 *
 * The version of the OpenAPI document: 1.6.1
 * 
 * Generated by: https://github.com/openapitools/openapi-generator.git
 */


using System;
using System.Linq;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using OpenAPIDateConverter = Golem.ActivityApi.Client.Client.OpenAPIDateConverter;

namespace Golem.ActivityApi.Client.Model
{
    /// <summary>
    /// GetActivityUsage
    /// </summary>
    [DataContract]
    public partial class GetActivityUsage : ProviderEvent,  IEquatable<GetActivityUsage>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GetActivityUsage" /> class.
        /// </summary>
        [JsonConstructorAttribute]
        protected GetActivityUsage() { }
        /// <summary>
        /// Initializes a new instance of the <see cref="GetActivityUsage" /> class.
        /// </summary>
        /// <param name="eventType">eventType (required).</param>
        /// <param name="eventDate">eventDate (required).</param>
        /// <param name="activityId">activityId (required).</param>
        /// <param name="agreementId">agreementId (required).</param>
        public GetActivityUsage(string eventType = default(string), DateTime eventDate = default(DateTime), string activityId = default(string), string agreementId = default(string)) : base(eventType, eventDate, activityId, agreementId)
        {
        }
        
        /// <summary>
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class GetActivityUsage {\n");
            sb.Append("  ").Append(base.ToString().Replace("\n", "\n  ")).Append("\n");
            sb.Append("}\n");
            return sb.ToString();
        }
  
        /// <summary>
        /// Returns the JSON string presentation of the object
        /// </summary>
        /// <returns>JSON string presentation of the object</returns>
        public override string ToJson()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }

        /// <summary>
        /// Returns true if objects are equal
        /// </summary>
        /// <param name="input">Object to be compared</param>
        /// <returns>Boolean</returns>
        public override bool Equals(object input)
        {
            return this.Equals(input as GetActivityUsage);
        }

        /// <summary>
        /// Returns true if GetActivityUsage instances are equal
        /// </summary>
        /// <param name="input">Instance of GetActivityUsage to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(GetActivityUsage input)
        {
            if (input == null)
                return false;

            return base.Equals(input);
        }

        /// <summary>
        /// Gets the hash code
        /// </summary>
        /// <returns>Hash code</returns>
        public override int GetHashCode()
        {
            unchecked // Overflow is fine, just wrap
            {
                int hashCode = base.GetHashCode();
                return hashCode;
            }
        }

    }

}
