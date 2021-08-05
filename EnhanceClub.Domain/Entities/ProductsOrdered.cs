namespace EnhanceClub.Domain.Entities
{
    public class ProductsOrdered
    {
        //--In addition to cart controller this class is used in order checkout step 3 to pass "profileFk" and "refill" fileds to cart

        public int CartProductSizeFk { get; set; }                    //-- changing name will fail cart recalculate
        public string ProductStoreFrontRealName { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public bool ProductActive { get; set; }
        public int ProductSizeId { get; set; }
        public decimal ProductSizeQty { get; set; }
        public string ProductSizeHeader { get; set; }
        public string ProductSizeStrength { get; set; }

        public bool ProductSizeAdd { get; set; }   // use to select product to be added to cart from order history
        public int ProductSizeAddQty { get; set; }  // used for quantity to be added to Cart // also used by recalculate cart button //-- changing name will fail recalculate

        // these properties are assigned during checkout step 2 under order review
        public bool CartRefill { get; set; }
        public int CartProfileFk { get; set; }

    }
}
