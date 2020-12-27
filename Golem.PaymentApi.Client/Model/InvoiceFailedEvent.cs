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
    /// InvoiceFailedEvent
    /// </summary>
    [DataContract]
    public partial class InvoiceFailedEvent : InvoiceEvent,  IEquatable<InvoiceFailedEvent>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InvoiceFailedEvent" /> class.
        /// </summary>
        [JsonConstructorAttribute]
        protected InvoiceFailedEvent() { }
        /// <summary>
        /// Initializes a new instance of the <see cref="InvoiceFailedEvent" /> class.
        /// </summary>
        /// <param name="invoiceId">invoiceId.</param>
        /// <param name="eventType">eventType (required).</param>
        /// <param name="eventDate">eventDate (required).</param>
        public InvoiceFailedEvent(string invoiceId = default(string), string eventType = default(string), DateTime eventDate = default(DateTime)) : base(eventType, eventDate)
        {
            this.InvoiceId = invoiceId;
        }
        
        /// <summary>
        /// Gets or Sets InvoiceId
        /// </summary>
        [DataMember(Name="invoiceId", EmitDefaultValue=false)]
        public string InvoiceId { get; set; }

        /// <summary>
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class InvoiceFailedEvent {\n");
            sb.Append("  ").Append(base.ToString().Replace("\n", "\n  ")).Append("\n");
            sb.Append("  InvoiceId: ").Append(InvoiceId).Append("\n");
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
            return this.Equals(input as InvoiceFailedEvent);
        }

        /// <summary>
        /// Returns true if InvoiceFailedEvent instances are equal
        /// </summary>
        /// <param name="input">Instance of InvoiceFailedEvent to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(InvoiceFailedEvent input)
        {
            if (input == null)
                return false;

            return base.Equals(input) && 
                (
                    this.InvoiceId == input.InvoiceId ||
                    (this.InvoiceId != null &&
                    this.InvoiceId.Equals(input.InvoiceId))
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
                if (this.InvoiceId != null)
                    hashCode = hashCode * 59 + this.InvoiceId.GetHashCode();
                return hashCode;
            }
        }

    }

}
