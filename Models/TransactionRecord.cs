using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Cascade.WebShop.Models
{
    public class TransactionRecord
    {
        /// <summary>
        /// Primary Id
        /// </summary>
        public virtual int Id { get; set; }

        /// <summary>
        /// Related Order Id
        /// </summary>
        public virtual int OrderRecord_Id { get; set; }
        /// <summary>
        /// Paypal method: SetExpressCheckout, GetExpressCheckout, DoExpressCheckout
        /// </summary>
        public virtual string Method { get; set; }

        /// <summary>
        /// The returned Paypal token
        /// </summary>
        public virtual string Token { get; set; }

        /// <summary>
        /// The returned response for the method: Success, Failure, Cancel
        /// </summary>
        public virtual string Ack { get; set; }

        /// <summary>
        /// Paypal payer ID
        /// </summary>
        public virtual string PayerId { get; set; }

        /// <summary>
        /// For DoExpressCheckout only, the first request TransactionId (should only be one)
        /// </summary>
        public virtual string RequestTransactionId { get; set; }

        /// <summary>
        /// For DoExpressCheckout only, the first SecureMerchantAccountId (should only be one)
        /// </summary>
        public virtual string RequestSecureMerchantAccountId { get; set; }

        /// <summary>
        /// For DoExpressCheckout only, the Request status (should only be one : Success, Failure, Cancel)
        /// </summary>
        public virtual string RequestAck { get; set; }

        public virtual DateTime DateTime { get; set; }

        //////////////////// Error info ///////////////////////////
        public virtual DateTime Timestamp { get; set; }
        public virtual string ErrorCodes { get; set; }
        public virtual string ShortMessages { get; set; }
        public virtual string LongMessages { get; set; }
        public virtual string CorrelationId { get; set; }

    }
}