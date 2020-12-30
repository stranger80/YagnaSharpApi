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
    /// RuntimeEventKindFinished
    /// </summary>
    [DataContract]
    public partial class RuntimeEventKindFinished : RuntimeEventKind,  IEquatable<RuntimeEventKindFinished>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RuntimeEventKindFinished" /> class.
        /// </summary>
        [JsonConstructorAttribute]
        protected RuntimeEventKindFinished() { }
        /// <summary>
        /// Initializes a new instance of the <see cref="RuntimeEventKindFinished" /> class.
        /// </summary>
        /// <param name="finished">finished (required).</param>
        public RuntimeEventKindFinished(RuntimeEventKindFinishedBody finished = default(RuntimeEventKindFinishedBody)) : base()
        {
            // to ensure "finished" is required (not null)
            this.Finished = finished ?? throw new ArgumentNullException("finished is a required property for RuntimeEventKindFinished and cannot be null");;
        }
        
        /// <summary>
        /// Gets or Sets Finished
        /// </summary>
        [DataMember(Name="finished", EmitDefaultValue=false)]
        public RuntimeEventKindFinishedBody Finished { get; set; }

        /// <summary>
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class RuntimeEventKindFinished {\n");
            sb.Append("  ").Append(base.ToString().Replace("\n", "\n  ")).Append("\n");
            sb.Append("  Finished: ").Append(Finished).Append("\n");
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
            return this.Equals(input as RuntimeEventKindFinished);
        }

        /// <summary>
        /// Returns true if RuntimeEventKindFinished instances are equal
        /// </summary>
        /// <param name="input">Instance of RuntimeEventKindFinished to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(RuntimeEventKindFinished input)
        {
            if (input == null)
                return false;

            return base.Equals(input) && 
                (
                    this.Finished == input.Finished ||
                    (this.Finished != null &&
                    this.Finished.Equals(input.Finished))
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
                if (this.Finished != null)
                    hashCode = hashCode * 59 + this.Finished.GetHashCode();
                return hashCode;
            }
        }

    }

}
