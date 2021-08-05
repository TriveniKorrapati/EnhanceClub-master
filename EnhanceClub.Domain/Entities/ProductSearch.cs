using System.Collections.Generic;
using System.Linq;
using System.Web;
using EnhanceClub.Domain.Concrete;

namespace EnhanceClub.Domain.Entities
{
    // this class is used to get product search data 
    public class ProductSearch
    {
        public string ProductStoreFrontRealName { get; set; }
        public int       ProductId              { get; set; }
        public string    ProductName            { get; set; }
        public string    ProductSafeUrlName     { get; set; }
        public string    ProductWarning         { get; set; }
        public string    ProductCautions        { get; set; }
        public string    ProductSideEffects     { get; set; }
        public string    ProductDescription     { get; set; }
        public string    ProductDirections      { get; set; }
        public string    ProductIngredients     { get; set; }
        public string    ProductUkName          { get; set; }
        public bool      ProductFrontEndNa      { get; set; }
        public bool      ProductOtcRxRequired   { get; set; }
        public bool      ProductBlockCndIp      { get; set; }
        public string    ProductRxOtc           { get; set; }
        public int       ProductTypeFk          { get; set; }
        public bool      PetProduct { get; set; }

        public decimal ProductSizeStoreFrontPrice       { get; set; }

        public string ProductStoreFrontMetaTitle        { get; set; }
        public string ProductStoreFrontMetaDescription  { get; set; }
        public string ProductStoreFrontMetaKeyWords     { get; set; }
        public bool   ProductStoreFrontImage            { get; set; }
        public string ProductStoreFrontSafeUrlName      { get; set; }
        public string ProductStoreFrontDescription      { get; set; }
        public string ProductStoreFrontDirections       { get; set; }
		public string ProductStoreFrontIngredients      { get; set; }
		public string ProductStoreFrontCautions        { get; set; }
        public string ProductStoreFrontSideEffects      { get; set; }

        public int ProductUnitId                        { get; set; }
        public string ProductUnitNamePk                 { get; set; }
        public string ManufacturerNamePk                { get; set; }

        public int ProductSearchTermSortOrder           { get; set; }
        
        public List<ProductSize> ProductSizeList { get; set; }
        
        public IEnumerable<FeaturedProduct> RelatedProducts { get; set; } // used to create structure data in ProductStructureData.cshtml

        // Flag used to send user to template page
        public bool? ProductDataCollect { get; set; }

        // Property to display message for unavailability of product (for which DataCollect is set to true)
        public string ProductDataCollectMessage { get; set; }

        // Property to chose display color for Product 
        public string ProductStoreFrontDisplayColor { get; set; }

        public string FeaturedProductDisplayName { get; set; }

        // property to add search letter to top selction "Quick Jump" in case of search by Alphabet
        public string LetterSearched { get; set; }

        // property to add url section of either prescription or otc for UrlAlphabet
        public string ProductClass{ get; set; }
        
        public string ProductGenericName { get; set; }

        // property to return product detail url for Alphabet Search
        public string UrlAlphabet
        {
            get
            {
                string urlAction = "http://" + HttpContext.Current.Request.Url.Authority;

                if (ProductName.Contains("Quick Jump"))
                {
                    // in this case send back to prescription page for searchLetter typed
                    urlAction += "/" + ProductClass + "/" + LetterSearched.Trim();
                }
                else
                {

                    urlAction += "/products/" + ProductStoreFrontSafeUrlName;
                }


                return urlAction;
            }
        }
        // get count of generic product size

        public int ProductSizeGenericCount
        {
            get
            {
                if (ProductSizeList == null)
                {
                    // log no product size found
                    // log no product size found
                    var actionLog = new GlobalFunctions();
                    actionLog.AddLogUnexpected("ProductSizeList is null for this product", "ProductSearch.cs", ProductName ,0, 0, 0, ProductId );
                    return 0;
                    
                }
                else
                {
                    return  ProductSizeList.Count(x => x.ProductSizeGeneric == true);    
                }
                
            }
        }

        // get count of brand product size

        public int ProductSizeBrandCount
        {
            get
            {
                if (ProductSizeList == null)
                {
                    // log no product size found
                    var actionLog = new GlobalFunctions( );
                    actionLog.AddLogUnexpected("ProductSizeList is null for this product", "ProductSearch.cs", ProductName, 0, 0, 0, ProductId);
                    return 0;
                }
                else
                {
                    return ProductSizeList.Count(x => x.ProductSizeGeneric == false);
                }
            }
        }

        public string ProductUnits { get; set; } // list of all product units available for product

        public string ProductGenericImagePath
        {
            get
            {
                return "/content/images/products/product-item-generic-" + ProductId + ".png";
            }
        }

        public string ProductBrandImagePath
        {
            get
            {
                return "/content/images/products/product-item-brand-" + ProductId + ".png";
            }
        }

       
    }
}
