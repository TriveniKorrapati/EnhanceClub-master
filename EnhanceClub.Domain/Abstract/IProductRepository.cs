using System;
using System.Collections.Generic;
using EnhanceClub.Domain.Entities;

namespace EnhanceClub.Domain.Abstract
{
    public interface IProductRepository
    {
        IEnumerable<ProductCart> Products { get; }
        
        void AddLogSearchTerm(string productNameFilter, int affiliateId, DateTime searchTimeStamp);
        
        // get search terms
        IEnumerable<string> GetProductSearchTermList(string searchTerm, int affiliateStoreFrontFk, bool onlyActive);
   

        // get product based on search term
        IEnumerable<ProductSearch> GetProductBasedOnSearchTerm(IEnumerable<string> searchTermIdList,
                                                               int affiliateStoreFrontFk,
                                                               bool onlyActiveProduct,
                                                               bool onlyActiveProductSize, 
                                                               bool onlyActiveProductStoreFront, 
                                                               bool onlyActiveProductSizeStorefront );

        // get products based on name filter
        IEnumerable<ProductSearch> GetProductBasedOnNameFilter(string searchTerm, 
                                                               int affiliateStoreFrontFk,
                                                               string modifySearchRxOtc,
                                                               int productType,
                                                               bool onlyActiveProduct,
                                                               bool onlyActiveProductSize,
                                                               bool onlyActiveProductStoreFront,
                                                               bool onlyActiveProductSizeStorefront);

        // get products based on url safe name 
        ProductSearch GetProductBasedOnUrlName(string searchTerm,
                                               string strength,
                                               int affiliateStoreFrontFk,
                                               string modifySearchRxOtc,
                                               int productType,
                                               bool onlyActiveProduct,
                                               bool onlyActiveProductSize,
                                               bool onlyActiveProductStoreFront,
                                               bool onlyActiveProductSizeStorefront);

        // get products based on letter filter
        IEnumerable<ProductSearch> GetProductBasedOnLetterFilter(
                                                                 string searchLetter,
                                                                 int affiliateStoreFrontFk, string modifySearchRxOtc,
                                                                 int productType,
                                                                 bool onlyActiveProduct,
                                                                 bool onlyActiveProductSize,
                                                                 bool onlyActiveProductStoreFront,
                                                                 bool onlyActiveProductSizeStorefront);



        // get products based on letter filter
        IEnumerable<ProductSearch> GetPetProductBasedOnLetterFilter(
                                                                 string searchLetter,
                                                                 int affiliateStoreFrontFk, string modifySearchRxOtc,
                                                                 int productType,
                                                                 bool onlyActiveProduct,
                                                                 bool onlyActiveProductSize,
                                                                 bool onlyActiveProductStoreFront,
                                                                 bool onlyActiveProductSizeStorefront);

        // used to get product info for product size added to cart for cart view
        ProductCart GetProductSizeInfoForCart( int productSizeId , int storefrontFk, int frontendVisibleProductSizeFk);

        // used to update limited qty 
        void UpdateLimitedQty(int productSizeId, int quantity, int orderInvoiceId , DateTime now);

        // gets related products f`or a product, used in product detail page
        IEnumerable<FeaturedProduct> GetRelatedProducts(int productId, int storeFrontId,string sortBy);

        // get count of search result for every letter to disable letters that don't have products
        IEnumerable<ProductAlphabet> GetProductCountByAlphabet(int storeFrontFk, int productType);

        // get count of search result for every letter to disable letters that don't have products
        IEnumerable<ProductAlphabet> GetPetProductCountByAlphabet(int affiliateInfoAffiliateStoreFrontFk);

        // get matching products for auto complete search
        List<ProductSearchBox> GetMatchingProducts(string searchTerm,
            int storeFrontFk,
            bool activeProduct,
            bool activeProductStorefront);

         // Save product enquiry details for the unavailable brand/generic for a product
        int AddProductEnquiry(int productFk, string email, int brandGeneric, int customerFk, int storefrontFk, int dataCollect, DateTime dateCreated);

        // Gets products for home page 
        IEnumerable<TopFeaturedProduct> GetTopMedications(int storeFrontId, string productType);

        //Get All products for  general product page
        IEnumerable<FeaturedProduct> GetAllProducts(int storeFrontId);

        List<ProductSize> GetProductSizeForProduct(int productId, int storeFrontId, int storeFrontType, string strength, bool productSizeVisibleFrontEnd);

        IEnumerable<VolumeDiscountProductSize> GetVolumeDiscountProductSize(int productSizeFk);

    }
}
