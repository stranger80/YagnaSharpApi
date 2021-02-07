using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using YagnaSharpApi.Repository;

namespace YagnaSharpApi.Entities
{
    public class InvoiceEntity
    {

        public IPaymentRepository Repository { get; protected set; }


        public InvoiceEntity(IPaymentRepository repo)
        {
            this.Repository = repo;
        }

        #region Fields

        /// <summary>
        /// Gets or Sets InvoiceId
        /// </summary>
        public string InvoiceId { get; private set; }

        /// <summary>
        /// Gets or Sets IssuerId
        /// </summary>
        public string IssuerId { get; private set; }

        /// <summary>
        /// Gets or Sets RecipientId
        /// </summary>
        public string RecipientId { get; private set; }

        /// <summary>
        /// Gets or Sets PayeeAddr
        /// </summary>
        public string PayeeAddr { get; private set; }

        /// <summary>
        /// Gets or Sets PayerAddr
        /// </summary>
        public string PayerAddr { get; private set; }

        /// <summary>
        /// Gets or Sets PaymentPlatform
        /// </summary>
        public string PaymentPlatform { get; private set; }

        /// <summary>
        /// Gets or Sets LastDebitNoteId
        /// </summary>
        public string LastDebitNoteId { get; private set; }

        /// <summary>
        /// Gets or Sets Timestamp
        /// </summary>
        public DateTime Timestamp { get; private set; }

        /// <summary>
        /// Gets or Sets AgreementId
        /// </summary>
        public string AgreementId { get; set; }

        /// <summary>
        /// Gets or Sets ActivityIds
        /// </summary>
        public List<string> ActivityIds { get; set; }

        /// <summary>
        /// Gets or Sets Amount
        /// </summary>
        public string Amount { get; set; }

        /// <summary>
        /// Gets or Sets PaymentDueDate
        /// </summary>
        public DateTime PaymentDueDate { get; set; }

        #endregion

        public async Task AcceptAsync(string amount, AllocationEntity allocation)
        {

        }
    }
}
