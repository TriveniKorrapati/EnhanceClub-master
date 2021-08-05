namespace EnhanceClub.Domain.Entities
{

    // this class is used to add previously ordered products to cart
 
    public class CartProduct
    {
        public int CartProductSizeFk { get; set; }
        
        public bool ProductSizeAdd { get; set; }   // use to select product to be added to cart from order history

        public int ProductSizeAddQty { get; set; }   // used for quantity to be added to Cart

        public bool Subscription { get; set; }
    }
}