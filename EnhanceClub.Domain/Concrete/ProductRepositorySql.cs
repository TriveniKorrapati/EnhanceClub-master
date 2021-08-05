using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using EnhanceClub.Domain.Abstract;
using EnhanceClub.Domain.Entities;
using EnhanceClub.Domain.Helpers;

namespace EnhanceClub.Domain.Concrete
{
    public class ProductRepositorySql : IProductRepository
    {
        private readonly ProductDbLayer _productDbl = new ProductDbLayer();

        public IEnumerable<ProductCart> Products { get; private set; }

        // Add Product Name Searched  to Log Table
        public void AddLogSearchTerm(string productNameFilter, int affiliateId, DateTime searchTimeStamp)
        {
            var newLogSearchTermId = _productDbl.AddLogSearchTerm(productNameFilter, affiliateId, searchTimeStamp);
        }

        // get list of Ids (if any) from ProductSearchTerm_Product table
        public IEnumerable<string> GetProductSearchTermList(string searchTerm, int affiliateStorefontFk, bool onlyActive)
        {
            DataSet dsSearchTerm = _productDbl.GetProductSearchTermList(searchTerm, affiliateStorefontFk, onlyActive);

            if (dsSearchTerm.Tables[0].Rows.Count > 0)
            {

                IEnumerable<string> myList = dsSearchTerm.Tables[0].AsEnumerable()
                    .Select(row => row.Field<int>("ProductSearchTerm_Product_id").ToString()).ToArray();

                return myList;
            }
            else
            {
                return null;
            }

        }

        // get product data based on search term

        public IEnumerable<ProductSearch> GetProductBasedOnSearchTerm(IEnumerable<string> searchTermIdList,
                                                                                  int affiliateStoreFrontFk,
                                                                                  bool onlyActiveProduct,
                                                                                  bool onlyActiveProductSize,
                                                                                  bool onlyActiveProductStoreFront,
                                                                                  bool onlyActiveProductSizeStorefront)
        {
            DataSet dsProductsFound = _productDbl.GetProductBasedOnSearchTerm(searchTermIdList,
                                                                              affiliateStoreFrontFk,
                                                                              onlyActiveProduct,
                                                                              onlyActiveProductSize,
                                                                              onlyActiveProductStoreFront,
                                                                              onlyActiveProductSizeStorefront);

            List<ProductSearch> productList = null;

            if (dsProductsFound.Tables[0].Rows.Count > 0)
            {
                productList =
                   dsProductsFound.Tables[0].AsEnumerable()
                   .Select(row => new ProductSearch
                   {
                       ProductStoreFrontRealName = row.Field<string>("Product_StoreFront_Realname"),
                       ProductId = row.Field<int>("Product_Id"),
                       ProductName = row.Field<string>("Product_Name"),
                       ProductSafeUrlName = row.Field<string>("Product_SafeUrlName"),
                       ProductWarning = row.Field<string>("Product_Warning"),
                       ProductCautions = row.Field<string>("Product_Cautions"),
                       ProductSideEffects = row.Field<string>("Product_SideEffects"),
                       ProductDescription = row.Field<string>("Product_Description"),
                       ProductDirections = row.Field<string>("Product_Directions"),
                       ProductIngredients = row.Field<string>("Product_Ingredients"),
                       ProductUkName = row.Field<string>("Product_UKName"),
                       ProductFrontEndNa = row.Field<bool>("Product_FrontendNA"),
                       ProductOtcRxRequired = row.Field<bool>("Product_OTCRxRequired"),
                       ProductBlockCndIp = row.Field<bool>("Product_Block_CDNIP"),
                       ProductRxOtc = row.Field<string>("Product_RxOtc"),
                       ProductTypeFk = row.Field<int>("Product_Producttype_Fk"),

                       //ProductSizeStoreFrontPrice = row.Field<decimal>("ProductSize_StoreFront_Price"),

                       ProductStoreFrontMetaTitle = row.Field<string>("Product_Storefront_MetaTitle"),
                       ProductStoreFrontMetaDescription = row.Field<string>("Product_Storefront_MetaDescription"),
                       ProductStoreFrontMetaKeyWords = row.Field<string>("Product_Storefront_MetaKeyWords"),
                       ProductStoreFrontImage = row.Field<bool>("Product_Storefront_Image"),

                       // ProductUnitId  = row.Field<int>("ProductUnit_Id"),
                       // ProductUnitNamePk = row.Field<string>("ProductUnit_Name_Pk"),
                       //  ManufacturerNamePk = row.Field<string>("Manufacturer_Name_Pk"),
                       ProductSearchTermSortOrder = row.Field<int>("ProductSearchTerm_Product_SortOrder"),
                       ProductDataCollect = row.Field<object>("Product_DataCollect") != DBNull.Value && Convert.ToBoolean(row.Field<object>("Product_DataCollect")),
                       ProductSizeList = GetProductSizeForProduct(row.Field<int>("Product_Id"), affiliateStoreFrontFk, 1, null, SiteConfigurations.ProductSizeVisibleFrontEndOnly)


                   }).ToList();

            }
            return productList;
        }


        // - get product data based on name search keyword

        public IEnumerable<ProductSearch> GetProductBasedOnNameFilter(string searchTerm,
                                                                      int affiliateStoreFrontFk,
                                                                      string modifySearchRxOtc,
                                                                      int productType,
                                                                      bool onlyActiveProduct,
                                                                      bool onlyActiveProductSize,
                                                                      bool onlyActiveProductStoreFront,
                                                                      bool onlyActiveProductSizeStorefront)
        {
            DataSet dsProductsFound = _productDbl.GetProductBasedOnNameFilter(searchTerm,
                                                                              affiliateStoreFrontFk,
                                                                              modifySearchRxOtc,
                                                                              productType,
                                                                              onlyActiveProduct,
                                                                              onlyActiveProductSize,
                                                                              onlyActiveProductStoreFront,
                                                                              onlyActiveProductSizeStorefront);

            List<ProductSearch> productList = null;

            if (dsProductsFound.Tables[0].Rows.Count > 0)
            {
                productList =
                   dsProductsFound.Tables[0].AsEnumerable()
                   .Select(row => new ProductSearch
                   {
                       ProductStoreFrontRealName = row.Field<string>("Product_StoreFront_Realname"),
                       ProductId = row.Field<int>("Product_Id"),
                       ProductName = row.Field<string>("Product_Name"),
                       ProductSafeUrlName = row.Field<string>("Product_SafeUrlName"),
                       ProductWarning = row.Field<string>("Product_Warning"),
                       ProductCautions = row.Field<string>("Product_Cautions"),
                       ProductSideEffects = row.Field<string>("Product_SideEffects"),
                       ProductDescription = row.Field<string>("Product_Description"),
                       ProductDirections = row.Field<string>("Product_Directions"),
                       ProductIngredients = row.Field<string>("Product_Ingredients"),
                       ProductUkName = row.Field<string>("Product_UKName"),
                       ProductFrontEndNa = row.Field<bool>("Product_FrontendNA"),
                       ProductOtcRxRequired = row.Field<bool>("Product_OTCRxRequired"),
                       ProductBlockCndIp = row.Field<bool>("Product_Block_CDNIP"),
                       ProductRxOtc = row.Field<string>("Product_RxOtc"),
                       ProductTypeFk = row.Field<int>("Product_Producttype_Fk"),


                       ProductStoreFrontMetaTitle = row.Field<string>("Product_Storefront_MetaTitle"),
                       ProductStoreFrontMetaDescription = row.Field<string>("Product_Storefront_MetaDescription"),
                       ProductStoreFrontMetaKeyWords = row.Field<string>("Product_Storefront_MetaKeyWords"),
                       ProductStoreFrontImage = row.Field<bool>("Product_Storefront_Image"),
                       ProductStoreFrontSafeUrlName = row.Field<string>("Product_Storefront_safeurlname"),

                       ManufacturerNamePk = row.Field<string>("Manufacturer_Name_Pk"),
                       ProductDataCollect = row.Field<object>("Product_DataCollect") != DBNull.Value && Convert.ToBoolean(row.Field<object>("Product_DataCollect")),
                       ProductSizeList = GetProductSizeForProduct(row.Field<int>("Product_Id"), affiliateStoreFrontFk, 1, null,SiteConfigurations.ProductSizeVisibleFrontEndOnly)

                   }).ToList();

            }
            return productList;
        }



        // get product based on url safe name

        public ProductSearch GetProductBasedOnUrlName(string searchTerm,
                                                      string strength,
                                                     int affiliateStoreFrontFk,
                                                     string modifySearchRxOtc,
                                                     int productType,
                                                     bool onlyActiveProduct,
                                                     bool onlyActiveProductSize,
                                                     bool onlyActiveProductStoreFront,
                                                     bool onlyActiveProductSizeStorefront
                                                    )
        {
            DataSet dsProductsFound = _productDbl.GetProductBasedOnUrlName(searchTerm,
                                                                             affiliateStoreFrontFk,
                                                                             modifySearchRxOtc,
                                                                             productType,
                                                                             onlyActiveProduct,
                                                                             onlyActiveProductSize,
                                                                             onlyActiveProductStoreFront,
                                                                             onlyActiveProductSizeStorefront);

            ProductSearch productFromUrlSafeName = null;

            if (dsProductsFound.Tables[0].Rows.Count > 0)
            {
                productFromUrlSafeName =
                   dsProductsFound.Tables[0].AsEnumerable()
                   .Select(row => new ProductSearch
                   {
                       ProductStoreFrontRealName = Convert.IsDBNull(row.Field<string>("Product_StoreFront_Realname")) ? String.Empty : row.Field<string>("Product_StoreFront_Realname"),
                       ProductId = row.Field<int>("Product_Id"),
                       ProductName = Convert.IsDBNull(row.Field<string>("Product_Name")) ? String.Empty : row.Field<string>("Product_Name"),
                       ProductSafeUrlName = Convert.IsDBNull(row.Field<string>("Product_SafeUrlName")) ? String.Empty : row.Field<string>("Product_SafeUrlName"),
                       ProductWarning = Convert.IsDBNull(row.Field<string>("Product_Warning")) ? String.Empty : row.Field<string>("Product_Warning"),
                       ProductCautions = Convert.IsDBNull(row.Field<string>("Product_Cautions")) ? String.Empty : row.Field<string>("Product_Cautions"),
                       ProductSideEffects = Convert.IsDBNull(row.Field<string>("Product_SideEffects")) ? String.Empty : row.Field<string>("Product_SideEffects"),
                       ProductDescription = String.IsNullOrEmpty(row.Field<string>("Product_Description")) ? String.Empty : row.Field<string>("Product_Description").Replace("href=\"", "href=http://www.EnhanceClub/").Replace("\"", ""),
                       ProductDirections = Convert.IsDBNull(row.Field<string>("Product_Directions")) ? String.Empty : row.Field<string>("Product_Directions"),
                       ProductIngredients = Convert.IsDBNull(row.Field<string>("Product_Ingredients")) ? String.Empty : row.Field<string>("Product_Ingredients"),
                       ProductUkName = Convert.IsDBNull(row.Field<string>("Product_UKName")) ? String.Empty : row.Field<string>("Product_UKName"),
                       ProductFrontEndNa = row.Field<object>("Product_FrontendNA") != DBNull.Value && Convert.ToBoolean(row.Field<object>("Product_FrontendNA")),

                       ProductOtcRxRequired = row.Field<object>("Product_OTCRxRequired") != DBNull.Value && Convert.ToBoolean(row.Field<object>("Product_OTCRxRequired")),
                       ProductBlockCndIp = row.Field<object>("Product_Block_CDNIP") != DBNull.Value && Convert.ToBoolean(row.Field<object>("Product_Block_CDNIP")),
                       ProductRxOtc = Convert.IsDBNull(row.Field<string>("Product_RxOtc")) ? String.Empty : row.Field<string>("Product_RxOtc"),

                       ProductTypeFk = row.Field<int>("Product_Producttype_Fk"),
                       PetProduct = row.Field<object>("Product_Pet") != DBNull.Value && Convert.ToBoolean(row.Field<object>("Product_Pet")),
                       ProductStoreFrontMetaTitle = Convert.IsDBNull(row.Field<string>("Product_Storefront_MetaTitle")) ? String.Empty : row.Field<string>("Product_Storefront_MetaTitle"),
                       ProductStoreFrontMetaDescription = Convert.IsDBNull(row.Field<string>("Product_Storefront_MetaDescription")) ? String.Empty : row.Field<string>("Product_Storefront_MetaDescription"),
                       ProductStoreFrontMetaKeyWords = Convert.IsDBNull(row.Field<string>("Product_Storefront_MetaKeyWords")) ? String.Empty : row.Field<string>("Product_Storefront_MetaKeyWords"),
                       ProductStoreFrontImage = row.Field<object>("Product_Storefront_Image") != DBNull.Value && Convert.ToBoolean(row.Field<object>("Product_Storefront_Image")),

                       ProductStoreFrontSafeUrlName = Convert.IsDBNull(row.Field<string>("Product_Storefront_SafeUrlName")) ? String.Empty : row.Field<string>("Product_Storefront_SafeUrlName"),

                       //ProductStoreFrontDescription = Convert.IsDBNull(row.Field<string>("Product_StoreFront_Description")) ? String.Empty : row.Field<string>("Product_StoreFront_Description").Replace("href=\"", "href=http://www.EnhanceClub/").Replace(".html\"", ".html"),
                       ProductStoreFrontDescription = String.IsNullOrEmpty(row.Field<string>("Product_StoreFront_Description")) ? String.Empty : row.Field<string>("Product_StoreFront_Description").Replace("href=\"", "href=http://www.EnhanceClub/").Replace("\"", ""),
                       //ProductStoreFrontDescription = Convert.IsDBNull(row.Field<string>("Product_StoreFront_Description")) ? String.Empty : row.Field<string>("Product_StoreFront_Description"),
                       ProductStoreFrontDirections = Convert.IsDBNull(row.Field<string>("Product_StoreFront_Directions")) ? String.Empty : row.Field<string>("Product_StoreFront_Directions"),
                       ProductStoreFrontIngredients = Convert.IsDBNull(row.Field<string>("Product_StoreFront_Ingredients")) ? String.Empty : row.Field<string>("Product_StoreFront_Ingredients"),
                       ProductStoreFrontCautions = Convert.IsDBNull(row.Field<string>("Product_StoreFront_Cautions")) ? String.Empty : row.Field<string>("Product_StoreFront_Cautions"),
                       ProductStoreFrontSideEffects = Convert.IsDBNull(row.Field<string>("Product_StoreFront_SideEffects")) ? String.Empty : row.Field<string>("Product_StoreFront_SideEffects"),
                       ManufacturerNamePk = Convert.IsDBNull(row.Field<string>("Manufacturer_Name_Pk")) ? String.Empty : row.Field<string>("Manufacturer_Name_Pk"),
                       ProductUnits = Convert.IsDBNull(row.Field<string>("ProductUnits")) ? String.Empty : row.Field<string>("ProductUnits"),

                       ProductDataCollect = row.Field<object>("Product_DataCollect") != DBNull.Value && Convert.ToBoolean(row.Field<object>("Product_DataCollect")),
                       ProductDataCollectMessage = Convert.IsDBNull(row.Field<string>("Product_DataCollectMessage")) ? String.Empty : row.Field<string>("Product_DataCollectMessage"),
                       ProductStoreFrontDisplayColor = Convert.IsDBNull(row.Field<string>("Product_StoreFront_DisplayColor")) ? String.Empty : row.Field<string>("Product_StoreFront_DisplayColor"),

                       ProductGenericName = Convert.IsDBNull(row.Field<string>("Product_GenericName")) ? String.Empty : row.Field<string>("Product_GenericName"),

                       ProductSizeList = GetProductSizeForProduct(row.Field<int>("Product_Id"), affiliateStoreFrontFk, 1, null, SiteConfigurations.ProductSizeVisibleFrontEndOnly),
                       RelatedProducts = GetRelatedProducts(row.Field<int>("Product_Id"), affiliateStoreFrontFk, String.Empty),
                       FeaturedProductDisplayName = Convert.IsDBNull(row.Field<string>("Product_Storefront_DisplayName")) ? String.Empty : row.Field<string>("Product_Storefront_DisplayName"),



                   }).ToList().FirstOrDefault();

            }
            return productFromUrlSafeName;
        }

        // get products based on letter filter

        public IEnumerable<ProductSearch> GetProductBasedOnLetterFilter(string searchLetter,
                                                                        int affiliateStoreFrontFk,
                                                                        string modifySearchRxOtc,
                                                                        int productType,
                                                                        bool onlyActiveProduct,
                                                                        bool onlyActiveProductSize,
                                                                        bool onlyActiveProductStoreFront,
                                                                        bool onlyActiveProductSizeStorefront)
        {
            DataSet dsProductsFound = _productDbl.GetProductBasedOnLetterFilter(searchLetter,
                                                                              affiliateStoreFrontFk,
                                                                              modifySearchRxOtc,
                                                                              productType,
                                                                              onlyActiveProduct,
                                                                              onlyActiveProductSize,
                                                                              onlyActiveProductStoreFront,
                                                                              onlyActiveProductSizeStorefront);

            List<ProductSearch> productList = null;

            if (dsProductsFound.Tables[0].Rows.Count > 0)
            {
                productList =
                   dsProductsFound.Tables[0].AsEnumerable()
                   .Select(row => new ProductSearch
                   {
                       ProductStoreFrontRealName = row.Field<string>("Product_StoreFront_Realname"),
                       ProductId = row.Field<int>("Product_Id"),
                       ProductName = row.Field<string>("Product_Name"),
                       ProductSafeUrlName = row.Field<string>("Product_SafeUrlName"),
                       ProductWarning = row.Field<string>("Product_Warning"),
                       ProductCautions = row.Field<string>("Product_Cautions"),
                       ProductSideEffects = row.Field<string>("Product_SideEffects"),
                       ProductDescription = row.Field<string>("Product_Description"),
                       ProductDirections = row.Field<string>("Product_Directions"),
                       ProductIngredients = row.Field<string>("Product_Ingredients"),
                       ProductUkName = row.Field<string>("Product_UKName"),
                       ProductFrontEndNa = row.Field<bool>("Product_FrontendNA"),
                       ProductOtcRxRequired = row.Field<bool>("Product_OTCRxRequired"),
                       ProductBlockCndIp = row.Field<bool>("Product_Block_CDNIP"),
                       ProductRxOtc = row.Field<string>("Product_RxOtc"),
                       ProductTypeFk = row.Field<int>("Product_Producttype_Fk"),

                       ProductStoreFrontMetaTitle = row.Field<string>("Product_Storefront_MetaTitle"),
                       ProductStoreFrontMetaDescription = row.Field<string>("Product_Storefront_MetaDescription"),
                       ProductStoreFrontMetaKeyWords = row.Field<string>("Product_Storefront_MetaKeyWords"),
                       ProductStoreFrontImage = row.Field<bool>("Product_Storefront_Image"),
                       ProductStoreFrontSafeUrlName = row.Field<string>("Product_Storefront_safeurlname"),

                       ManufacturerNamePk = row.Field<string>("Manufacturer_Name_Pk"),

                       ProductSizeList = GetProductSizeForProduct(row.Field<int>("Product_Id"), affiliateStoreFrontFk, 1,null, SiteConfigurations.ProductSizeVisibleFrontEndOnly),
                       ProductDataCollect = row.Field<object>("Product_DataCollect") != DBNull.Value && Convert.ToBoolean(row.Field<object>("Product_DataCollect")),
                       
                   }).ToList();

            }
            return productList;
        }

        // get Pet products based on letter filter

        public IEnumerable<ProductSearch> GetPetProductBasedOnLetterFilter(string searchLetter,
                                                                        int affiliateStorefontFk,
                                                                        string modifySearchRxOtc,
                                                                        int productType,
                                                                        bool onlyActiveProduct,
                                                                        bool onlyActiveProductSize,
                                                                        bool onlyActiveProductStoreFront,
                                                                        bool onlyActiveProductSizeStorefront)
        {
            DataSet dsProductsFound = _productDbl.GetPetProductBasedOnLetterFilter(searchLetter,
                                                                              affiliateStorefontFk,
                                                                              modifySearchRxOtc,
                                                                              productType,
                                                                              onlyActiveProduct,
                                                                              onlyActiveProductSize,
                                                                              onlyActiveProductStoreFront,
                                                                              onlyActiveProductSizeStorefront);

            List<ProductSearch> productList = null;

            if (dsProductsFound.Tables[0].Rows.Count > 0)
            {
                productList =
                   dsProductsFound.Tables[0].AsEnumerable()
                   .Select(row => new ProductSearch
                   {
                       ProductStoreFrontRealName = row.Field<string>("Product_StoreFront_Realname"),
                       ProductId = row.Field<int>("Product_Id"),
                       ProductName = row.Field<string>("Product_Name"),
                       ProductSafeUrlName = row.Field<string>("Product_SafeUrlName"),
                       ProductWarning = row.Field<string>("Product_Warning"),
                       ProductCautions = row.Field<string>("Product_Cautions"),
                       ProductSideEffects = row.Field<string>("Product_SideEffects"),
                       ProductDescription = row.Field<string>("Product_Description"),
                       ProductDirections = row.Field<string>("Product_Directions"),
                       ProductIngredients = row.Field<string>("Product_Ingredients"),
                       ProductUkName = row.Field<string>("Product_UKName"),
                       ProductFrontEndNa = row.Field<bool>("Product_FrontendNA"),
                       ProductOtcRxRequired = row.Field<bool>("Product_OTCRxRequired"),
                       ProductBlockCndIp = row.Field<bool>("Product_Block_CDNIP"),
                       ProductRxOtc = row.Field<string>("Product_RxOtc"),
                       ProductTypeFk = row.Field<int>("Product_Producttype_Fk"),


                       ProductStoreFrontMetaTitle = row.Field<string>("Product_Storefront_MetaTitle"),
                       ProductStoreFrontMetaDescription = row.Field<string>("Product_Storefront_MetaDescription"),
                       ProductStoreFrontMetaKeyWords = row.Field<string>("Product_Storefront_MetaKeyWords"),
                       ProductStoreFrontImage = row.Field<bool>("Product_Storefront_Image"),
                       ProductStoreFrontSafeUrlName = row.Field<string>("Product_Storefront_safeurlname"),

                       ManufacturerNamePk = row.Field<string>("Manufacturer_Name_Pk"),

                       ProductSizeList = GetProductSizeForProduct(row.Field<int>("Product_Id"), affiliateStorefontFk, 1, null,SiteConfigurations.ProductSizeVisibleFrontEndOnly)

                   }).ToList();

            }
            return productList;
        }


        // used to get related product info for product size added to (info for cart for cart view )
        public ProductCart GetProductSizeInfoForCart(int productSizeId, int storeFrontFk, int frontendVisibleProductSizeFk)
        {
            DataSet dsProductInfo = _productDbl.GetProductSizeInfoForCart(productSizeId, storeFrontFk);

            ProductCart productInfoForCart = null;



            if (dsProductInfo != null && dsProductInfo.Tables[0].Rows.Count > 0)
            {
                productInfoForCart =
                    dsProductInfo.Tables[0].AsEnumerable()
                        .Select(row => new ProductCart
                        {
                            ProductId = Convert.IsDBNull(row.Field<object>("Product_Id")) ? 0 : Convert.ToInt32(row.Field<object>("Product_Id")),
                            ProductName = row.Field<string>("Product_Name"),
                            ProductStoreFrontName = row.Field<string>("Product_StoreFront_Realname"),
                            ProductSafeUrlName = row.Field<string>("Product_StoreFront_UrlName"),
                            ProductTypeFk = row.Field<int>("Product_Producttype_Fk"),

                            ProductSizeId = row.Field<int>("ProductSize_Id"),
                            ProductUnitNamePk = row.Field<string>("ProductUnit_Name_Pk"),
                            ProductSizeGeneric = row.Field<bool>("ProductSize_generic"),
                            ProductSizeStrength = row.Field<string>("ProductSize_Strength"),
                            ProductSizeQuantity = row.Field<decimal>("ProductSize_Quantity"),
                            ProductSizeStoreFrontPrice = row.Field<decimal>("ProductSize_StoreFront_Price"),
                            ProductSizeLimitedQty = row.Field<decimal>("ProductSize_LimitedQty"),
                            ProductSizeLimited = row.Field<bool>("ProductSize_Limited"),
                            ProductSizeVisibleFrontEnd = row.Field<object>("Productsize_visiblefrontend") != DBNull.Value &&
                                                            Convert.ToBoolean(row.Field<object>("Productsize_visiblefrontend")),
                            FrontendVisibleProductSizeFk = frontendVisibleProductSizeFk,
                            ProductDisplayName = Convert.IsDBNull(row.Field<object>("Product_Storefront_DisplayName"))?string.Empty 
                                                : Convert.ToString(row.Field<object>("Product_Storefront_DisplayName")),
                            ProductSizeNonDiscountedPrice = Convert.IsDBNull(row.Field<object>("ProductSize_VolumeDiscount_NonDiscountedPrice")) ?
                                                            0 : Convert.ToDecimal(row.Field<object>("ProductSize_VolumeDiscount_NonDiscountedPrice")),
                            ProductSizeOrder = Convert.IsDBNull(row.Field<object>("Productsize_Order"))? 0 : Convert.ToInt32(row.Field<object>("Productsize_Order")),
                            VolumeDiscountProductSize = SiteConfigurations.ProductSizeVisibleFrontEndOnly ? 
                                        GetVolumeDiscountProductSize(frontendVisibleProductSizeFk) : GetVolumeDiscountProductSize(row.Field<int>("ProductSize_Id"))

                        }).ToList().FirstOrDefault();
            }
            else
            {
                // add to log
                var actionLog = new GlobalFunctions();
                actionLog.AddLogUnexpected("product detail for product size in cart not found",
                                            "ProductRepositorySql.cs", "", 0, storeFrontFk, +productSizeId, 0);
            }

            return productInfoForCart;
        }

        // used to update product size limited qty
        public void UpdateLimitedQty(int productSizeId, int quantity, int orderInvoiceId, DateTime dateCreated)
        {
            _productDbl.UpdateLimitedQty(productSizeId, quantity, orderInvoiceId, dateCreated);
        }

        // gets related products for a product, used in product detail page
        public IEnumerable<FeaturedProduct> GetRelatedProducts(int productId, int storeFrontId, string sortBy)
        {
            DataSet dsRelatedProducts = _productDbl.GetRelatedProducts(productId, storeFrontId, sortBy);

            List<FeaturedProduct> productList = new List<FeaturedProduct>();

            if (dsRelatedProducts.Tables.Count > 0 && dsRelatedProducts.Tables[0].Rows.Count > 0)
            {
                productList =
                    dsRelatedProducts.Tables[0].AsEnumerable()
                        .Select(row => new FeaturedProduct
                        {
                            FeaturedProductName = Convert.IsDBNull(row.Field<object>("Product_Name")) ? string.Empty : row.Field<string>("Product_Name"),
                            FeaturedProductGenericName = Convert.IsDBNull(row.Field<object>("Product_GenericName")) ? string.Empty : row.Field<string>("Product_GenericName"),
                            FeaturedProductDisplayName = Convert.IsDBNull(row.Field<object>("Product_Storefront_DisplayName")) ? string.Empty : row.Field<string>("Product_Storefront_DisplayName"),
                            ProductUrl = Convert.IsDBNull(row.Field<object>("Product_Storefront_SafeUrlName")) ? string.Empty : row.Field<string>("Product_Storefront_SafeUrlName"),
                            ProductSizeCount = Convert.IsDBNull(row.Field<int>("ProductSizeCount")) ? 0 : row.Field<int>("ProductSizeCount"),
                            FeaturedProductId = Convert.IsDBNull(row.Field<object>("Product_Id")) ? 0 : row.Field<int>("Product_Id"),
                        }).ToList();

            }
            return productList;
        }

        // get count of search result for every letter to disable letters that don't have products
        public IEnumerable<ProductAlphabet> GetProductCountByAlphabet(int storeFrontFk, int productType)
        {

            DataSet dsProductAlphabet = _productDbl.GetProductCountByAlphabet(storeFrontFk, productType);

            List<ProductAlphabet> productList = null;

            if (dsProductAlphabet.Tables[0].Rows.Count > 0)
            {
                productList =
                    dsProductAlphabet.Tables[0].AsEnumerable()
                        .Select(row => new ProductAlphabet
                        {
                            ProductLetterCount = Convert.IsDBNull(row.Field<int>("ProductLetterCount")) ? 0 : row.Field<int>("ProductLetterCount"),
                            ProductLetter = Convert.IsDBNull(row.Field<string>("ProductLetter")) ? string.Empty : row.Field<string>("ProductLetter")
                        }).ToList();

            }
            return productList;
        }


        // get count of search result for every letter to disable letters that don't have products
        public IEnumerable<ProductAlphabet> GetPetProductCountByAlphabet(int storeFrontFk)
        {
            DataSet dsProductAlphabet = _productDbl.GetPetProductCountByAlphabet(storeFrontFk);

            List<ProductAlphabet> productList = null;

            if (dsProductAlphabet.Tables[0].Rows.Count > 0)
            {
                productList =
                    dsProductAlphabet.Tables[0].AsEnumerable()
                        .Select(row => new ProductAlphabet
                        {
                            ProductLetterCount = Convert.IsDBNull(row.Field<int>("ProductLetterCount")) ? 0 : row.Field<int>("ProductLetterCount"),
                            ProductLetter = Convert.IsDBNull(row.Field<string>("ProductLetter")) ? string.Empty : row.Field<string>("ProductLetter")
                        }).ToList();

            }
            return productList;
        }

        // get matching products for auto complete search
        public List<ProductSearchBox> GetMatchingProducts(string searchTerm,
            int storeFrontFk,
            bool activeProduct,
            bool activeProductStorefront)
        {
            DataSet dsProductsFound = _productDbl.GetMatchingProducts(searchTerm, storeFrontFk, activeProduct, activeProductStorefront);

            List<ProductSearchBox> productList = null;

            if (dsProductsFound.Tables[0].Rows.Count > 0)
            {
                productList =
                    dsProductsFound.Tables[0].AsEnumerable()
                        .Select(row => new ProductSearchBox
                        {
                            ProductName = row.Field<string>("Product_Name"),
                            ProductSafeName = row.Field<string>("Product_Storefront_SafeurlName"),
                        }).ToList();

            }
            return productList;
        }

        // this method is used by all product search methods  above to get list of productsizes for a product

       public List<ProductSize> GetProductSizeForProduct(int productId, int storeFrontId, int storeFrontType, string strength, bool productSizeVisibleFrontEnd)
        {
            DataSet dsProductSize = _productDbl.GetProductSizeForProduct(productId, storeFrontId, storeFrontType, strength, productSizeVisibleFrontEnd);

            List<ProductSize> productSizeList = new List<ProductSize>();

            //var x = dsProductSize.Tables[0].Rows[1]["ProductSize_StoreFront_PriceDefaultWholesale_Active"] ==
            //        DBNull.Value;

            //bool t = dsProductSize.Tables[0].Rows[1]["ProductSize_StoreFront_PriceDefaultWholesale_Active"] ==
            //         DBNull.Value
            //    ? false
            //    : Convert.ToBoolean(dsProductSize.Tables[0].Rows[1]["ProductSize_StoreFront_PriceDefaultWholesale_Active"]);

            if (dsProductSize.Tables[0].Rows.Count > 0)
            {
                try
                {
                    productSizeList =
                        dsProductSize.Tables[0].AsEnumerable()
                            .Select(row => new ProductSize
                            {
                                ProductFk = row.Field<int>("ProductSize_Product_Fk"),
                                ProductSizeId = row.Field<int>("ProductSize_Id"),
                                ProductSizeProductUnitFk = row.Field<int>("ProductSize_ProductUnit_Fk"),
                                ProductSizeOrder = row.Field<int>("ProductSize_Order"),
                                ProductSizeStrength = row.Field<string>("ProductSize_Strength"),
                                ProductSizeQuantity = row.Field<decimal>("ProductSize_Quantity"),
                                //ProductSizeLimitedQty = row.Field<decimal>("ProductSize_LimitedQty"),
                                ProductSizeLimitedQty = Convert.IsDBNull(row.Field<decimal>("ProductSize_LimitedQty")) ? 0m : row.Field<decimal>("ProductSize_LimitedQty"),
                                ProductSizeMaxOrder = Convert.IsDBNull(row.Field<int>("ProductSize_MaxOrder")) ? 0 : row.Field<int>("ProductSize_MaxOrder"),
                                ProductSizePrice = row.Field<decimal>("ProductSize_Price"),
                                ProductSizeBlockCndIp = row.Field<bool>("ProductSize_BlockCNDIP"),
                                ProductSizeFrontEndNa = row.Field<bool>("ProductSize_FrontendNA"),
                                ProductSizeLimited = row.Field<bool>("ProductSize_Limited"),
                                ProductSizeGeneric = row.Field<bool>("ProductSize_generic"),
                                ProductUnitNamePk = row.Field<string>("ProductUnit_Name_Pk"),
                                ProductSizeStoreFrontFrontEndNa = row.Field<bool>("ProductSize_Storefront_FrontendNA"),
                                ProductSizeStoreFrontNotAvailable = row.Field<bool>("Productsize_Storefront_Notavailable"),
                                ProductSizeStoreFrontExcludeWholeSale = row.Field<bool>("ProductSize_StoreFront_Exclude_Wholesale"),
                                ProductSizeStoreFrontPreferText = Convert.IsDBNull(row.Field<string>("Productsize_Storefront_Prefertext")) ? string.Empty : row.Field<string>("Productsize_Storefront_Prefertext"),
                                // ProductSizeStoreFrontPriceDefaultWholeSaleActive = row.Field<bool?>("ProductSize_StoreFront_PriceDefaultWholesale_Active") != null && row.Field<bool>("ProductSize_StoreFront_PriceDefaultWholesale_Active"),
                                ProductSizeStoreFrontPriceDefaultWholeSaleActive = row.Field<object>("ProductSize_StoreFront_PriceDefaultWholesale_Active") != DBNull.Value && Convert.ToBoolean(row.Field<object>("ProductSize_StoreFront_PriceDefaultWholesale_Active")),
                                ProductSizeStoreFrontPriceDefaultWholeSale = row.Field<decimal>("ProductSize_StoreFront_PriceDefaultWholesale"),
                                StoreFrontExchangeRate = Convert.IsDBNull(row.Field<decimal>("StoreFrontExchangeRate")) ? 1.00m : row.Field<decimal>("StoreFrontExchangeRate"),
                                ProductSizeStoreFrontRecommendedDosage = Convert.IsDBNull(row.Field<object>("ProductSize_Storefront_RecommendedDosage")) ? string.Empty : row.Field<string>("ProductSize_Storefront_RecommendedDosage"),
                                ProductSizeRecommendedDosage = Convert.IsDBNull(row.Field<object>("ProductSize_RecommendedDosage")) ? string.Empty : row.Field<string>("ProductSize_RecommendedDosage"),
                                ProductCountryOfOrigin = row.Field<string>("Country_Name"),
                                ProductCountryOriginCode = row.Field<string>("Country_Code"),
                                ProductSizeVisibleFrontEnd = row.Field<object>("Productsize_visiblefrontend") != DBNull.Value && Convert.ToBoolean(row.Field<object>("Productsize_visiblefrontend"))

                            }).ToList();
                }

                catch (Exception ex)
                {
                    // add to log
                    var actionLog = new GlobalFunctions();
                    actionLog.AddLogUnexpected("error trying to create product size list",
                                               "ProductRepositorySql.cs",
                                               ex.InnerException == null ? string.Empty : ex.InnerException.ToString().Substring(1, 100),
                                               0,
                                               storeFrontId,
                                               0,
                                               productId);
                }

            }
            return productSizeList;
        }

        // Save product enquiry details for the unavailable brand/generic for a product
        public int AddProductEnquiry(int productFk, string email, int brandGeneric, int customerFk, int storefrontFk, int dataCollect, DateTime dateCreated)
        {
            return _productDbl.AddProductEnquiry(productFk, email, brandGeneric, customerFk, storefrontFk, dataCollect, dateCreated);
        }

        // gets Products for top medications
        public IEnumerable<TopFeaturedProduct> GetTopMedications(int storeFrontId, string productType)
        {
            DataSet dsRelatedProducts = _productDbl.GetTopMedications(storeFrontId, productType);

            List<TopFeaturedProduct> productList = new List<TopFeaturedProduct>();

            if (dsRelatedProducts.Tables.Count > 0 && dsRelatedProducts.Tables[0].Rows.Count > 0)
            {
                productList =
                    dsRelatedProducts.Tables[0].AsEnumerable()
                        .Select(row => new TopFeaturedProduct
                        {
                            FeaturedProductId = Convert.IsDBNull(row.Field<object>("Product_Id")) ? 0 : Convert.ToInt32(row.Field<object>("Product_Id")),
                            FeaturedProductName = Convert.IsDBNull(row.Field<object>("Product_Name")) ? string.Empty : row.Field<string>("Product_Name"),
                            ProductUrl = Convert.IsDBNull(row.Field<object>("Product_Storefront_Safeurlname")) ? string.Empty : "/products/" + row.Field<string>("Product_Storefront_Safeurlname"),
                            Category = Convert.IsDBNull(row.Field<object>("productTypeHeader")) ? string.Empty : row.Field<string>("productTypeHeader"),
                            ProductStoreFrontDisplayOrder = Convert.IsDBNull(row.Field<object>("Product_Storefront_DisplayOrder"))? 0 : Convert.ToInt32(row.Field<object>("Product_StoreFront_DisplayOrder")),
                            FeaturedProductDisplayName = Convert.IsDBNull(row.Field<object>("Product_Storefront_DisplayName"))?string.Empty : Convert.ToString(row.Field<object>("Product_Storefront_DisplayName")),
                            ProductSearch = GetProductBasedOnUrlName(row.Field<string>("Product_Storefront_Safeurlname"),null,storeFrontId,"rx", 0 ,true,true,true,true)
                            
                        }).ToList();

            }
            return productList;
        }

        public IEnumerable<FeaturedProduct> GetAllProducts(int storeFrontId)
        {
            DataSet dsProducts = _productDbl.GetAllProducts(storeFrontId);

            List<FeaturedProduct> productList = new List<FeaturedProduct>();

            if (dsProducts.Tables.Count > 0 && dsProducts.Tables[0].Rows.Count > 0)
            {
                productList =
                    dsProducts.Tables[0].AsEnumerable()
                        .Select(row => new FeaturedProduct
                        {
                            FeaturedProductName = Convert.IsDBNull(row.Field<object>("Product_Name")) ? string.Empty : row.Field<string>("Product_Name"),
                            FeaturedProductGenericName = Convert.IsDBNull(row.Field<object>("Product_GenericName")) ? string.Empty : row.Field<string>("Product_GenericName"),
                            FeaturedProductDisplayName = Convert.IsDBNull(row.Field<object>("Product_Storefront_DisplayName")) ? string.Empty : row.Field<string>("Product_Storefront_DisplayName"),
                            ProductUrl = Convert.IsDBNull(row.Field<object>("Product_Storefront_SafeUrlName")) ? string.Empty : row.Field<string>("Product_Storefront_SafeUrlName"),
                            FeaturedProductId = Convert.IsDBNull(row.Field<object>("Product_Id")) ? 0 : row.Field<int>("Product_Id"),
                        }).ToList();

            }
            return productList;
        }

        public IEnumerable<VolumeDiscountProductSize> GetVolumeDiscountProductSize(int productSizeFk)
        {
            DataSet dsProducts = _productDbl.GetVolumeDiscountProductSize(productSizeFk);

            List<VolumeDiscountProductSize> productList = new List<VolumeDiscountProductSize>();

            if (dsProducts.Tables.Count > 0 && dsProducts.Tables[0].Rows.Count > 0)
            {
                productList =
                    dsProducts.Tables[0].AsEnumerable()
                        .Select(row => new VolumeDiscountProductSize
                        {
                            ProductSizeFk = Convert.IsDBNull(row.Field<object>("productsize_volumeDiscount_ProductSize_Fk")) ? 0 : row.Field<int>("productsize_volumeDiscount_ProductSize_Fk"),
                            RelatedProductSizeFk = Convert.IsDBNull(row.Field<object>("productsize_volumeDiscount_RelatedProductSize_Fk")) ? 0 : row.Field<int>("productsize_volumeDiscount_RelatedProductSize_Fk"),
                            DiscountMessage = Convert.IsDBNull(row.Field<object>("productsize_volumeDiscount_DiscountMessage")) ? string.Empty : row.Field<string>("productsize_volumeDiscount_DiscountMessage"),
                            DiscountImageName = Convert.IsDBNull(row.Field<object>("productsize_volumeDiscount_DisplayImageName")) ? string.Empty : row.Field<string>("productsize_volumeDiscount_DisplayImageName"),
                            ProductSizePrice = Convert.IsDBNull(row.Field<object>("ProductSize_StoreFront_Price")) ? 0 : Convert.ToDecimal(row.Field<object>("ProductSize_StoreFront_Price")),
                            ProductSizeQuantity = Convert.IsDBNull(row.Field<object>("ProductSize_Quantity")) ? 0 : Convert.ToDecimal(row.Field<object>("ProductSize_Quantity")),
                            ProductSizeUnit = Convert.IsDBNull(row.Field<object>("ProductUnit_Name_Pk")) ? string.Empty : row.Field<string>("ProductUnit_Name_Pk"),
                            ProductSizeNonDiscountedPrice = Convert.IsDBNull(row.Field<object>("ProductSize_VolumeDiscount_NonDiscountedPrice")) ? 
                                                            0 : Convert.ToDecimal(row.Field<object>("ProductSize_VolumeDiscount_NonDiscountedPrice")),
                            FrontendVisibleItemPrice = Convert.IsDBNull(row.Field<object>("FrontendVisibleItemPrice")) ?
                                                            0 : Convert.ToDecimal(row.Field<object>("FrontendVisibleItemPrice")),
                            FrontendVisibleItemQuantity = Convert.IsDBNull(row.Field<object>("FrontendVisibleItemQuantity")) ?
                                                            0 : Convert.ToDecimal(row.Field<object>("FrontendVisibleItemQuantity")),
                            ProductSizeOrder = Convert.IsDBNull(row.Field<object>("Productsize_Order")) ? 0 : row.Field<int>("Productsize_Order"),
                        }).ToList();

            }
            return productList;
        }

    }
}
