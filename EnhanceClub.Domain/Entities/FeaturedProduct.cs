namespace EnhanceClub.Domain.Entities
{
    public class FeaturedProduct
    {
        public string FeaturedProductName { get; set; }
        public string FeaturedProductGenericName { get; set; }
        public int FeaturedProductId { get; set; }
        public string ProductUrl { get; set; }
       
        // used for top 10  
        public string Category { get; set; }

        // used for Rx or Otc
        public string ProductType { get; set; }
        public string FeaturedProductDisplayName { get; set; }

        // how many active product size are for storefront, used to filter products that don't have any active product size
        // it was done for seo, so that 404 errors are avoided for products that don't have ant active product size
        public int ProductSizeCount { get; set; }

    }
}
