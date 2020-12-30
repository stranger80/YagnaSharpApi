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
    /// RuntimeEventKindStdOut
    /// </summary>
    [DataContract]
    public partial class RuntimeEventKindStdOut : RuntimeEventKind,  IEquatable<RuntimeEventKindStdOut>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RuntimeEventKindStdOut" /> class.
        /// </summary>
        [JsonConstructorAttribute]
        protected RuntimeEventKindStdOut() { }
        /// <summary>
        /// Initializes a new instance of the <see cref="RuntimeEventKindStdOut" /> class.
        /// </summary>
        /// <param name="stdout">stdout (required).</param>
        public RuntimeEventKindStdOut(Object stdout = default(Object)) : base()
        {
            // to ensure "stdout" is required (not null)
            this.Stdout = stdout ?? throw new ArgumentNullException("stdout is a required property for RuntimeEventKindStdOut and cannot be null");;
        }
        
        /// <summary>
        /// Gets or Sets Stdout
        /// </summary>
        [DataMember(Name="stdout", EmitDefaultValue=false)]
        public Object Stdout { get; set; }

        /// <summary>
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class RuntimeEventKindStdOut {\n");
            sb.Append("  ").Append(base.ToString().Replace("\n", "\n  ")).Append("\n");
            sb.Append("  Stdout: ").Append(Stdout).Append("\n");
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
            return this.Equals(input as RuntimeEventKindStdOut);
        }

        /// <summary>
        /// Returns true if RuntimeEventKindStdOut instances are equal
        /// </summary>
        /// <param name="input">Instance of RuntimeEventKindStdOut to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(RuntimeEventKindStdOut input)
        {
            if (input == null)
                return false;

            return base.Equals(input) && 
                (
                    this.Stdout == input.Stdout ||
                    (this.Stdout != null &&
                    this.Stdout.Equals(input.Stdout))
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
                if (this.Stdout != null)
                    hashCode = hashCode * 59 + this.Stdout.GetHashCode();
                return hashCode;
            }
        }

    }

}
