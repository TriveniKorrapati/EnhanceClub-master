using System;

namespace EnhanceClub.Domain.Entities
{
    public class CartItem
    {
        public ProductCart ProductCart { get; set; }
        public int Quantity { get; set; }

        public decimal CartLineTotal
        {
            get { return ProductCart.ProductSizeStoreFrontPrice*Quantity; }
        }
        // these properties are assigned during checkout step 2 under order review
        public bool CartRefill { get; set; }
        public int CartProfileFk { get; set; }

        public int CartItemOrder { get; set; }

        public bool IsRefillCartItem { get; set; }

        public int CartItemShipInvoiceFk { get; set; }

        public int NumberOfRefillsAllowed { get; set; }

        public int NumberOfRefillsAvailable { get; set; }

        public bool AutoRefillCompleted { get; set; }

        public DateTime? AutoRefillStartDate { get; set; }

        public DateTime? AutoRefillEndDate { get; set; }

        public string RefillMessage { get; set; }

        public int LogRefillId { get; set; }


    }
}
