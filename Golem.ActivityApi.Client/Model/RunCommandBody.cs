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
    /// RunCommandBody
    /// </summary>
    [DataContract]
    public partial class RunCommandBody :  IEquatable<RunCommandBody>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RunCommandBody" /> class.
        /// </summary>
        [JsonConstructorAttribute]
        protected RunCommandBody() { }
        /// <summary>
        /// Initializes a new instance of the <see cref="RunCommandBody" /> class.
        /// </summary>
        /// <param name="entryPoint">entryPoint (required).</param>
        /// <param name="args">args.</param>
        public RunCommandBody(string entryPoint = default(string), List<string> args = default(List<string>))
        {
            // to ensure "entryPoint" is required (not null)
            this.EntryPoint = entryPoint ?? throw new ArgumentNullException("entryPoint is a required property for RunCommandBody and cannot be null");;
            this.Args = args;
        }
        
        /// <summary>
        /// Gets or Sets EntryPoint
        /// </summary>
        [DataMember(Name="entry_point", EmitDefaultValue=false)]
        public string EntryPoint { get; set; }

        /// <summary>
        /// Gets or Sets Args
        /// </summary>
        [DataMember(Name="args", EmitDefaultValue=false)]
        public List<string> Args { get; set; }

        /// <summary>
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class RunCommandBody {\n");
            sb.Append("  EntryPoint: ").Append(EntryPoint).Append("\n");
            sb.Append("  Args: ").Append(Args).Append("\n");
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
            return this.Equals(input as RunCommandBody);
        }

        /// <summary>
        /// Returns true if RunCommandBody instances are equal
        /// </summary>
        /// <param name="input">Instance of RunCommandBody to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(RunCommandBody input)
        {
            if (input == null)
                return false;

            return 
                (
                    this.EntryPoint == input.EntryPoint ||
                    (this.EntryPoint != null &&
                    this.EntryPoint.Equals(input.EntryPoint))
                ) && 
                (
                    this.Args == input.Args ||
                    this.Args != null &&
                    input.Args != null &&
                    this.Args.SequenceEqual(input.Args)
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
                if (this.EntryPoint != null)
                    hashCode = hashCode * 59 + this.EntryPoint.GetHashCode();
                if (this.Args != null)
                    hashCode = hashCode * 59 + this.Args.GetHashCode();
                return hashCode;
            }
        }

    }

}
