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
    /// AgreementRejectedEventAllOf
    /// </summary>
    [DataContract]
    public partial class AgreementRejectedEventAllOf :  IEquatable<AgreementRejectedEventAllOf>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AgreementRejectedEventAllOf" /> class.
        /// </summary>
        /// <param name="reason">reason.</param>
        public AgreementRejectedEventAllOf(Reason reason = default(Reason))
        {
            this.Reason = reason;
        }
        
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
            sb.Append("class AgreementRejectedEventAllOf {\n");
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
            return this.Equals(input as AgreementRejectedEventAllOf);
        }

        /// <summary>
        /// Returns true if AgreementRejectedEventAllOf instances are equal
        /// </summary>
        /// <param name="input">Instance of AgreementRejectedEventAllOf to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(AgreementRejectedEventAllOf input)
        {
            if (input == null)
                return false;

            return 
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
                if (this.Reason != null)
                    hashCode = hashCode * 59 + this.Reason.GetHashCode();
                return hashCode;
            }
        }

    }

}
