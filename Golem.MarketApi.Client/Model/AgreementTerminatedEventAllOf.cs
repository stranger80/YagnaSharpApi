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
    /// AgreementTerminatedEventAllOf
    /// </summary>
    [DataContract]
    public partial class AgreementTerminatedEventAllOf :  IEquatable<AgreementTerminatedEventAllOf>
    {
        /// <summary>
        /// Defines Terminator
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public enum TerminatorEnum
        {
            /// <summary>
            /// Enum Requestor for value: Requestor
            /// </summary>
            [EnumMember(Value = "Requestor")]
            Requestor = 1,

            /// <summary>
            /// Enum Provider for value: Provider
            /// </summary>
            [EnumMember(Value = "Provider")]
            Provider = 2

        }

        /// <summary>
        /// Gets or Sets Terminator
        /// </summary>
        [DataMember(Name="terminator", EmitDefaultValue=false)]
        public TerminatorEnum Terminator { get; set; }
        /// <summary>
        /// Initializes a new instance of the <see cref="AgreementTerminatedEventAllOf" /> class.
        /// </summary>
        [JsonConstructorAttribute]
        protected AgreementTerminatedEventAllOf() { }
        /// <summary>
        /// Initializes a new instance of the <see cref="AgreementTerminatedEventAllOf" /> class.
        /// </summary>
        /// <param name="terminator">terminator (required).</param>
        /// <param name="signature">signature (required).</param>
        /// <param name="reason">reason.</param>
        public AgreementTerminatedEventAllOf(TerminatorEnum terminator = default(TerminatorEnum), string signature = default(string), Reason reason = default(Reason))
        {
            this.Terminator = terminator;
            // to ensure "signature" is required (not null)
            this.Signature = signature ?? throw new ArgumentNullException("signature is a required property for AgreementTerminatedEventAllOf and cannot be null");;
            this.Reason = reason;
        }
        
        /// <summary>
        /// Gets or Sets Signature
        /// </summary>
        [DataMember(Name="signature", EmitDefaultValue=false)]
        public string Signature { get; set; }

        /// <summary>
        /// Gets or Sets Reason
        /// </summary>
        [DataMember(Name="reason", EmitDefaultValue=false)]
        public Reason Reason { get; set; }

        /// <summary>
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class AgreementTerminatedEventAllOf {\n");
            sb.Append("  Terminator: ").Append(Terminator).Append("\n");
            sb.Append("  Signature: ").Append(Signature).Append("\n");
            sb.Append("  Reason: ").Append(Reason).Append("\n");
            sb.Append("}\n");
            return sb.ToString();
        }
  
        /// <summary>
        /// Returns the JSON string presentation of the object
        /// </summary>
        /// <returns>JSON string presentation of the object</returns>
        public virtual string ToJson()
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
            return this.Equals(input as AgreementTerminatedEventAllOf);
        }

        /// <summary>
        /// Returns true if AgreementTerminatedEventAllOf instances are equal
        /// </summary>
        /// <param name="input">Instance of AgreementTerminatedEventAllOf to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(AgreementTerminatedEventAllOf input)
        {
            if (input == null)
                return false;

            return 
                (
                    this.Terminator == input.Terminator ||
                    this.Terminator.Equals(input.Terminator)
                ) && 
                (
                    this.Signature == input.Signature ||
                    (this.Signature != null &&
                    this.Signature.Equals(input.Signature))
                ) && 
                (
                    this.Reason == input.Reason ||
                    (this.Reason != null &&
                    this.Reason.Equals(input.Reason))
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
                int hashCode = 41;
                hashCode = hashCode * 59 + this.Terminator.GetHashCode();
                if (this.Signature != null)
                    hashCode = hashCode * 59 + this.Signature.GetHashCode();
                if (this.Reason != null)
                    hashCode = hashCode * 59 + this.Reason.GetHashCode();
                return hashCode;
            }
        }

    }

}
