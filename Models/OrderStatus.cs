namespace Cascade.WebShop.Models
{
    public enum OrderStatus
    {
        /// <summary>
        /// The order is new and is yet to be paid for
        /// </summary>
        New,

        /// <summary>
        /// The order has been paid for, so it's eligable for shipping
        /// </summary>
        Paid,

        /// <summary>
        /// The order has shipped
        /// </summary>
        Completed,

        /// <summary>
        /// The order was cancelled
        /// </summary>
        Cancelled
    }
}