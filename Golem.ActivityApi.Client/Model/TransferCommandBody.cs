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
    /// TransferCommandBody
    /// </summary>
    [DataContract]
    public partial class TransferCommandBody :  IEquatable<TransferCommandBody>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TransferCommandBody" /> class.
        /// </summary>
        [JsonConstructorAttribute]
        protected TransferCommandBody() { }
        /// <summary>
        /// Initializes a new instance of the <see cref="TransferCommandBody" /> class.
        /// </summary>
        /// <param name="from">from (required).</param>
        /// <param name="to">to (required).</param>
        /// <param name="format">format.</param>
        /// <param name="depth">depth.</param>
        /// <param name="fileset">fileset.</param>
        public TransferCommandBody(string from = default(string), string to = default(string), string format = default(string), decimal depth = default(decimal), List<FileSet> fileset = default(List<FileSet>))
        {
            // to ensure "from" is required (not null)
            this.From = from ?? throw new ArgumentNullException("from is a required property for TransferCommandBody and cannot be null");;
            // to ensure "to" is required (not null)
            this.To = to ?? throw new ArgumentNullException("to is a required property for TransferCommandBody and cannot be null");;
            this.Format = format;
            this.Depth = depth;
            this.Fileset = fileset;
        }
        
        /// <summary>
        /// Gets or Sets From
        /// </summary>
        [DataMember(Name="from", EmitDefaultValue=false)]
        public string From { get; set; }

        /// <summary>
        /// Gets or Sets To
        /// </summary>
        [DataMember(Name="to", EmitDefaultValue=false)]
        public string To { get; set; }

        /// <summary>
        /// Gets or Sets Format
        /// </summary>
        [DataMember(Name="format", EmitDefaultValue=false)]
        public string Format { get; set; }

        /// <summary>
        /// Gets or Sets Depth
        /// </summary>
        [DataMember(Name="depth", EmitDefaultValue=false)]
        public decimal Depth { get; set; }

        /// <summary>
        /// Gets or Sets Fileset
        /// </summary>
        [DataMember(Name="fileset", EmitDefaultValue=false)]
        public List<FileSet> Fileset { get; set; }

        /// <summary>
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class TransferCommandBody {\n");
            sb.Append("  From: ").Append(From).Append("\n");
            sb.Append("  To: ").Append(To).Append("\n");
            sb.Append("  Format: ").Append(Format).Append("\n");
            sb.Append("  Depth: ").Append(Depth).Append("\n");
            sb.Append("  Fileset: ").Append(Fileset).Append("\n");
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
            return this.Equals(input as TransferCommandBody);
        }

        /// <summary>
        /// Returns true if TransferCommandBody instances are equal
        /// </summary>
        /// <param name="input">Instance of TransferCommandBody to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(TransferCommandBody input)
        {
            if (input == null)
                return false;

            return 
                (
                    this.From == input.From ||
                    (this.From != null &&
                    this.From.Equals(input.From))
                ) && 
                (
                    this.To == input.To ||
                    (this.To != null &&
                    this.To.Equals(input.To))
                ) && 
                (
                    this.Format == input.Format ||
                    (this.Format != null &&
                    this.Format.Equals(input.Format))
                ) && 
                (
                    this.Depth == input.Depth ||
                    this.Depth.Equals(input.Depth)
                ) && 
                (
                    this.Fileset == input.Fileset ||
                    this.Fileset != null &&
                    input.Fileset != null &&
                    this.Fileset.SequenceEqual(input.Fileset)
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
                if (this.From != null)
                    hashCode = hashCode * 59 + this.From.GetHashCode();
                if (this.To != null)
                    hashCode = hashCode * 59 + this.To.GetHashCode();
                if (this.Format != null)
                    hashCode = hashCode * 59 + this.Format.GetHashCode();
                hashCode = hashCode * 59 + this.Depth.GetHashCode();
                if (this.Fileset != null)
                    hashCode = hashCode * 59 + this.Fileset.GetHashCode();
                return hashCode;
            }
        }

    }

}
