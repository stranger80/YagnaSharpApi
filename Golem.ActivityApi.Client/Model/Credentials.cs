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
    /// Credentials
    /// </summary>
    [DataContract]
    public partial class Credentials :  IEquatable<Credentials>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Credentials" /> class.
        /// </summary>
        [JsonConstructorAttribute]
        protected Credentials() { }
        /// <summary>
        /// Initializes a new instance of the <see cref="Credentials" /> class.
        /// </summary>
        /// <param name="sgx">sgx (required).</param>
        public Credentials(SgxCredentials sgx = default(SgxCredentials))
        {
            // to ensure "sgx" is required (not null)
            this.Sgx = sgx ?? throw new ArgumentNullException("sgx is a required property for Credentials and cannot be null");;
        }
        
        /// <summary>
        /// Gets or Sets Sgx
        /// </summary>
        [DataMember(Name="sgx", EmitDefaultValue=false)]
        public SgxCredentials Sgx { get; set; }

        /// <summary>
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class Credentials {\n");
            sb.Append("  Sgx: ").Append(Sgx).Append("\n");
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
            return this.Equals(input as Credentials);
        }

        /// <summary>
        /// Returns true if Credentials instances are equal
        /// </summary>
        /// <param name="input">Instance of Credentials to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(Credentials input)
        {
            if (input == null)
                return false;

            return 
                (
                    this.Sgx == input.Sgx ||
                    (this.Sgx != null &&
                    this.Sgx.Equals(input.Sgx))
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
                if (this.Sgx != null)
                    hashCode = hashCode * 59 + this.Sgx.GetHashCode();
                return hashCode;
            }
        }

    }

}
