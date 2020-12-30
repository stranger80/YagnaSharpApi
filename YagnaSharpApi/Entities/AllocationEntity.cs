using System;
using System.Collections.Generic;
using System.Text;

namespace YagnaSharpApi.Entities
{
    public class AllocationEntity
    {
        /// <summary>
        /// Gets or Sets AllocationId
        /// </summary>
        public string AllocationId { get; set; }

        /// <summary>
        /// Gets or Sets Timestamp
        /// </summary>
        public DateTime Timestamp { get; set; }

        /// <summary>
        /// Gets or Sets Address
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// Gets or Sets PaymentPlatform
        /// </summary>
        public string PaymentPlatform { get; set; }

        /// <summary>
        /// Gets or Sets TotalAmount
        /// </summary>
        public string TotalAmount { get; set; }

        /// <summary>
        /// Gets or Sets SpentAmount
        /// </summary>
        public string SpentAmount { get; set; }

        /// <summary>
        /// Gets or Sets RemainingAmount
        /// </summary>
        public string RemainingAmount { get; set; }

        /// <summary>
        /// Gets or Sets Timeout
        /// </summary>
        public DateTime Timeout { get; set; }

        /// <summary>
        /// Gets or Sets MakeDeposit
        /// </summary>
        public bool MakeDeposit { get; set; }

    }
}
