/* 
 * Yagna Market API
 *
 *  ## Yagna Market The Yagna Market is a core component of the Yagna Network, which enables computational Offers and Demands circulation. The Market is open for all entities willing to buy computations (Demands) or monetize computational resources (Offers). ## Yagna Market API The Yagna Market API is the entry to the Yagna Market through which Requestors and Providers can publish their Demands and Offers respectively, find matching counterparty, conduct negotiations and make an agreement.  This version of Market API conforms with capability level 1 of the <a href=\"https://docs.google.com/document/d/1Zny_vfgWV-hcsKS7P-Kdr3Fb0dwfl-6T_cYKVQ9mkNg\"> Market API specification</a>.  Market API contains two roles: Requestors and Providers which are symmetrical most of the time (excluding agreement phase). 
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
using OpenAPIDateConverter = Golem.Common.Client.Client.OpenAPIDateConverter;

namespace Golem.MarketApi.Client.Model
{
    /// <summary>
    /// AgreementEvent
    /// </summary>
    [DataContract]
    public partial class AgreementEvent : Event,  IEquatable<AgreementEvent>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AgreementEvent" /> class.
        /// </summary>
        [JsonConstructorAttribute]
        protected AgreementEvent() { }
        /// <summary>
        /// Initializes a new instance of the <see cref="AgreementEvent" /> class.
        /// </summary>
        /// <param name="agreement">agreement (required).</param>
        /// <param name="eventType">eventType (required).</param>
        /// <param name="eventDate">eventDate (required).</param>
        public AgreementEvent(Agreement agreement = default(Agreement), string eventType = default(string), DateTime eventDate = default(DateTime)) : base(eventType, eventDate)
        {
            // to ensure "agreement" is required (not null)
            this.Agreement = agreement ?? throw new ArgumentNullException("agreement is a required property for AgreementEvent and cannot be null");;
        }
        
        /// <summary>
        /// Gets or Sets Agreement
        /// </summary>
        [DataMember(Name="agreement", EmitDefaultValue=false)]
        public Agreement Agreement { get; set; }

        /// <summary>
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class AgreementEvent {\n");
            sb.Append("  ").Append(base.ToString().Replace("\n", "\n  ")).Append("\n");
            sb.Append("  Agreement: ").Append(Agreement).Append("\n");
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
            return this.Equals(input as AgreementEvent);
        }

        /// <summary>
        /// Returns true if AgreementEvent instances are equal
        /// </summary>
        /// <param name="input">Instance of AgreementEvent to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(AgreementEvent input)
        {
            if (input == null)
                return false;

            return base.Equals(input) && 
                (
                    this.Agreement == input.Agreement ||
                    (this.Agreement != null &&
                    this.Agreement.Equals(input.Agreement))
                );
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
                if (this.Agreement != null)
                    hashCode = hashCode * 59 + this.Agreement.GetHashCode();
                return hashCode;
            }
        }

    }

}
