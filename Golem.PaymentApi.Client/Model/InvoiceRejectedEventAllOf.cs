/* 
 * Yagna Payment API
 *
 *  Invoicing and Payments is a fundamental area of Yagna Ecosystem functionality. It includes aspects of communication between Requestor, Provider and a selected Payment Platform, which becomes crucial when Activities are executed in the context of negotiated Agreements. Yagna applications must be able to exercise various payment models, and the Invoicing/Payment-related communication is happening in parallel to Activity control communication. To define functional patterns of Requestor/Provider interaction in this area, Payment API is specified.  An important principle of the Yagna Payment API is that the actual payment transactions are hidden behind the Invoice flow. In other words, a Yagna Application on Requestor side isn’t expected to trigger actual payment transactions. Instead it is expected to receive and accept Invoices raised by the Provider - based on Application’s Invoice Accept notifications, the Payment API implementation orchestrates the payment via a configured Payment platform.  **NOTE: This specification is work-in-progress.** 
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
using OpenAPIDateConverter = Golem.PaymentApi.Client.Client.OpenAPIDateConverter;

namespace Golem.PaymentApi.Client.Model
{
    /// <summary>
    /// InvoiceRejectedEventAllOf
    /// </summary>
    [DataContract]
    public partial class InvoiceRejectedEventAllOf :  IEquatable<InvoiceRejectedEventAllOf>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InvoiceRejectedEventAllOf" /> class.
        /// </summary>
        /// <param name="invoiceId">invoiceId.</param>
        /// <param name="rejection">rejection.</param>
        public InvoiceRejectedEventAllOf(string invoiceId = default(string), Rejection rejection = default(Rejection))
        {
            this.InvoiceId = invoiceId;
            this.Rejection = rejection;
        }
        
        /// <summary>
        /// Gets or Sets InvoiceId
        /// </summary>
        [DataMember(Name="invoiceId", EmitDefaultValue=false)]
        public string InvoiceId { get; set; }

        /// <summary>
        /// Gets or Sets Rejection
        /// </summary>
        [DataMember(Name="rejection", EmitDefaultValue=false)]
        public Rejection Rejection { get; set; }

        /// <summary>
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class InvoiceRejectedEventAllOf {\n");
            sb.Append("  InvoiceId: ").Append(InvoiceId).Append("\n");
            sb.Append("  Rejection: ").Append(Rejection).Append("\n");
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
            return this.Equals(input as InvoiceRejectedEventAllOf);
        }

        /// <summary>
        /// Returns true if InvoiceRejectedEventAllOf instances are equal
        /// </summary>
        /// <param name="input">Instance of InvoiceRejectedEventAllOf to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(InvoiceRejectedEventAllOf input)
        {
            if (input == null)
                return false;

            return 
                (
                    this.InvoiceId == input.InvoiceId ||
                    (this.InvoiceId != null &&
                    this.InvoiceId.Equals(input.InvoiceId))
                ) && 
                (
                    this.Rejection == input.Rejection ||
                    (this.Rejection != null &&
                    this.Rejection.Equals(input.Rejection))
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
                if (this.InvoiceId != null)
                    hashCode = hashCode * 59 + this.InvoiceId.GetHashCode();
                if (this.Rejection != null)
                    hashCode = hashCode * 59 + this.Rejection.GetHashCode();
                return hashCode;
            }
        }

    }

}
