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
using OpenAPIDateConverter = Golem.Common.Client.Client.OpenAPIDateConverter;

namespace Golem.ActivityApi.Client.Model
{
    /// <summary>
    /// RuntimeEventKindStdErrAllOf
    /// </summary>
    [DataContract]
    public partial class RuntimeEventKindStdErrAllOf :  IEquatable<RuntimeEventKindStdErrAllOf>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RuntimeEventKindStdErrAllOf" /> class.
        /// </summary>
        [JsonConstructorAttribute]
        protected RuntimeEventKindStdErrAllOf() { }
        /// <summary>
        /// Initializes a new instance of the <see cref="RuntimeEventKindStdErrAllOf" /> class.
        /// </summary>
        /// <param name="stderr">stderr (required).</param>
        public RuntimeEventKindStdErrAllOf(Object stderr = default(Object))
        {
            // to ensure "stderr" is required (not null)
            this.Stderr = stderr ?? throw new ArgumentNullException("stderr is a required property for RuntimeEventKindStdErrAllOf and cannot be null");;
        }
        
        /// <summary>
        /// Gets or Sets Stderr
        /// </summary>
        [DataMember(Name="stderr", EmitDefaultValue=false)]
        public Object Stderr { get; set; }

        /// <summary>
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class RuntimeEventKindStdErrAllOf {\n");
            sb.Append("  Stderr: ").Append(Stderr).Append("\n");
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
            return this.Equals(input as RuntimeEventKindStdErrAllOf);
        }

        /// <summary>
        /// Returns true if RuntimeEventKindStdErrAllOf instances are equal
        /// </summary>
        /// <param name="input">Instance of RuntimeEventKindStdErrAllOf to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(RuntimeEventKindStdErrAllOf input)
        {
            if (input == null)
                return false;

            return 
                (
                    this.Stderr == input.Stderr ||
                    (this.Stderr != null &&
                    this.Stderr.Equals(input.Stderr))
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
                if (this.Stderr != null)
                    hashCode = hashCode * 59 + this.Stderr.GetHashCode();
                return hashCode;
            }
        }

    }

}
