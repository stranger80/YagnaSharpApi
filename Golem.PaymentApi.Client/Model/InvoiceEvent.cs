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
using JsonSubTypes;
using OpenAPIDateConverter = Golem.Common.Client.Client.OpenAPIDateConverter;

namespace Golem.PaymentApi.Client.Model
{
    /// <summary>
    /// InvoiceEvent
    /// </summary>
    [DataContract]
    [JsonConverter(typeof(JsonSubtypes), "EventType")]
    [JsonSubtypes.KnownSubType(typeof(InvoiceRejectedEvent), "InvoiceRejectedEvent")]
    [JsonSubtypes.KnownSubType(typeof(InvoiceFailedEvent), "InvoiceFailedEvent")]
    [JsonSubtypes.KnownSubType(typeof(InvoiceSettledEvent), "InvoiceSettledEvent")]
    [JsonSubtypes.KnownSubType(typeof(InvoiceCancelledEvent), "InvoiceCancelledEvent")]
    [JsonSubtypes.KnownSubType(typeof(InvoiceAcceptedEvent), "InvoiceAcceptedEvent")]
    [JsonSubtypes.KnownSubType(typeof(InvoiceReceivedEvent), "InvoiceReceivedEvent")]
    [JsonSubtypes.KnownSubType(typeof(PaymentReceivedEvent), "PaymentReceivedEvent")]
    [JsonSubtypes.KnownSubType(typeof(InvoiceReceivedEvent), "InvoiceReceivedEvent")]
    [JsonSubtypes.KnownSubType(typeof(InvoiceAcceptedEvent), "InvoiceAcceptedEvent")]
    [JsonSubtypes.KnownSubType(typeof(InvoiceRejectedEvent), "InvoiceRejectedEvent")]
    [JsonSubtypes.KnownSubType(typeof(InvoiceFailedEvent), "InvoiceFailedEvent")]
    [JsonSubtypes.KnownSubType(typeof(InvoiceSettledEvent), "InvoiceSettledEvent")]
    [JsonSubtypes.KnownSubType(typeof(InvoiceCancelledEvent), "InvoiceCancelledEvent")]
    [JsonSubtypes.KnownSubType(typeof(PaymentReceivedEvent), "PaymentReceivedEvent")]
    public partial class InvoiceEvent :  IEquatable<InvoiceEvent>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InvoiceEvent" /> class.
        /// </summary>
        [JsonConstructorAttribute]
        protected InvoiceEvent() { }
        /// <summary>
        /// Initializes a new instance of the <see cref="InvoiceEvent" /> class.
        /// </summary>
        /// <param name="eventType">eventType (required).</param>
        /// <param name="eventDate">eventDate (required).</param>
        public InvoiceEvent(string eventType = default(string), DateTime eventDate = default(DateTime))
        {
            // to ensure "eventType" is required (not null)
            this.EventType = eventType ?? throw new ArgumentNullException("eventType is a required property for InvoiceEvent and cannot be null");;
            this.EventDate = eventDate;
        }
        
        /// <summary>
        /// Gets or Sets EventType
        /// </summary>
        [DataMember(Name="eventType", EmitDefaultValue=false)]
        public string EventType { get; set; }

        /// <summary>
        /// Gets or Sets EventDate
        /// </summary>
        [DataMember(Name="eventDate", EmitDefaultValue=false)]
        public DateTime EventDate { get; set; }

        /// <summary>
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class InvoiceEvent {\n");
            sb.Append("  EventType: ").Append(EventType).Append("\n");
            sb.Append("  EventDate: ").Append(EventDate).Append("\n");
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
            return this.Equals(input as InvoiceEvent);
        }

        /// <summary>
        /// Returns true if InvoiceEvent instances are equal
        /// </summary>
        /// <param name="input">Instance of InvoiceEvent to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(InvoiceEvent input)
        {
            if (input == null)
                return false;

            return 
                (
                    this.EventType == input.EventType ||
                    (this.EventType != null &&
                    this.EventType.Equals(input.EventType))
                ) && 
                (
                    this.EventDate == input.EventDate ||
                    (this.EventDate != null &&
                    this.EventDate.Equals(input.EventDate))
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
                if (this.EventType != null)
                    hashCode = hashCode * 59 + this.EventType.GetHashCode();
                if (this.EventDate != null)
                    hashCode = hashCode * 59 + this.EventDate.GetHashCode();
                return hashCode;
            }
        }

    }

}
