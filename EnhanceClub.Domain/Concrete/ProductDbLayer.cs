using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using EnhanceClub.Domain.Helpers;

namespace EnhanceClub.Domain.Concrete
{
    public class ProductDbLayer
    {
        // readonly string _sCon = ConfigurationManager.ConnectionStrings["Connection"].ToString();

        private readonly string _sCon = @SiteConfigurations.SCon;

        DataSet ds;

        // Add to Search term Log
        public int AddLogSearchTerm(string productNameFilter, int affiliateId, DateTime searchTimeStamp)
        {
            try
            {
                SqlParameter[] paramCollection = SqlHelperParameterCache.GetSpParameterSet(_sCon, "usp_addLogSearchTerm", true);

                paramCollection[1].Value = productNameFilter;
                paramCollection[2].Value = affiliateId;
                paramCollection[3].Value = searchTimeStamp;
                paramCollection[4].Value = null; // new Log Id
                paramCollection[5].Value = null;  // message

                SqlHelper.ExecuteNonQuery(_sCon, CommandType.StoredProcedure, "usp_addLogSearchTerm", paramCollection);

                var msg = paramCollection[5].Value;
              
                var value = paramCollection[4].Value;
                if (value != null) return Convert.ToInt32(value.ToString());

                return 0;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        // get list of product search term_Product Id's  from ProductSearchTermProduct
        public DataSet GetProductSearchTermList(string searchTerm, int affiliateStorefontFk, bool onlyActive)
        {
            try
            {
                ds = SqlHelper.ExecuteDataset(_sCon, "usp_getProductSearchTerm", searchTerm, affiliateStorefontFk, onlyActive);
            }
            catch (Exception ex)
            {
                ds = null;
                throw ex;
            }
            return ds;
        }

        // get products based on search term

        public DataSet GetProductBasedOnSearchTerm(IEnumerable<string> searchTermIdList, 
                                                   int affiliateStoreFrontFk, 
                                                   bool onlyActiveProduct,
                                                   bool onlyActiveProductSize,
                                                   bool onlyActiveProductStoreFront,
                                                   bool onlyActiveProductSizeStorefront)
        {
            try
            {
                // Convert IEnumerable to Array
                string[] searchTerm = searchTermIdList.ToArray();

                // Convert Array to Coma separated List
                string searchTermIds = String.Join(",", searchTerm);

                ds = SqlHelper.ExecuteDataset(_sCon, "usp_getProductBasedOnSearchTerm", searchTermIds, 
                                                                                       affiliateStoreFrontFk,
                                                                                       onlyActiveProduct,
                                                                                       onlyActiveProductSize,
                                                                                       onlyActiveProductStoreFront,
                                                                                       onlyActiveProductSizeStorefront);
            }
            catch (Exception ex)
            {
                ds = null;
                throw ex;
            }
            return ds;
        }

        // get products based on product name search keyword

        public DataSet GetProductBasedOnNameFilter(string searchTerm, 
                                                   int affiliateStoreFrontFk, 
                                                   string modifySearchRxOtc,
                                                   int productType,
                                                   bool onlyActiveProduct,
                                                   bool onlyActiveProductSize,
                                                   bool onlyActiveProductStoreFront, 
                                                   bool onlyActiveProductSizeStorefront)
        {
            try
            {
                
                ds = SqlHelper.ExecuteDataset(_sCon, "usp_getProductBasedOnNameFilter", searchTerm,
                                                                                       affiliateStoreFrontFk,
                                                                                       modifySearchRxOtc,
                                                                                       productType,
                                                                                       onlyActiveProduct,
                                                                                       onlyActiveProductSize,
                                                                                       onlyActiveProductStoreFront,
                                                                                       onlyActiveProductSizeStorefront);
            }
            catch (Exception ex)
            {
                ds = null;
                throw ex;
            }
            return ds;
        }

        // get all active productSize for a product
        public DataSet GetProductSizeForProduct(int productId, int storeFrontId, int storeFrontType, string strength, bool productSizeVisibleFrontEnd)
        {
            try
            {
                ds = SqlHelper.ExecuteDataset(_sCon, "usp_getAllProductSizeforProduct", productId, storeFrontId,storeFrontType, strength, productSizeVisibleFrontEnd);
            }
            catch (Exception ex)
            {
                ds = null;
                throw ex;
            }
            return ds;
        }

        // get product based on url safe name
        public DataSet GetProductBasedOnUrlName  (string searchTerm,
                                                  int affiliateStoreFrontFk,
                                                  string modifySearchRxOtc,
                                                  int productType,
                                                  bool onlyActiveProduct,
                                                  bool onlyActiveProductSize,
                                                  bool onlyActiveProductStoreFront,
                                                  bool onlyActiveProductSizeStorefront)
        {
            try
            {

                ds = SqlHelper.ExecuteDataset(_sCon, "usp_getProductBasedOnUrlName", searchTerm,
                                                                                       affiliateStoreFrontFk,
                                                                                       modifySearchRxOtc,
                                                                                       productType,
                                                                                       onlyActiveProduct,
                                                                                       onlyActiveProductSize,
                                                                                       onlyActiveProductStoreFront,
                                                                                       onlyActiveProductSizeStorefront);
            }
            catch (Exception ex)
            {
                ds = null;
                throw ex;
            }
            return ds;
        }

        // get products based on letter filter
        public DataSet GetProductBasedOnLetterFilter(string searchLetter, 
                                                            int affiliateStoreFrontFk,
                                                            string modifySearchRxOtc, 
                                                            int productType, 
                                                            bool onlyActiveProduct, 
                                                            bool onlyActiveProductSize,
                                                            bool onlyActiveProductStoreFront,
                                                            bool onlyActiveProductSizeStorefront)
        {
            try
            {

                ds = SqlHelper.ExecuteDataset(_sCon, "usp_getProductBasedOnLetterFilter", searchLetter,
                                                                                       affiliateStoreFrontFk,
                                                                                       modifySearchRxOtc,
                                                                                       productType,
                                                                                       onlyActiveProduct,
                                                                                       onlyActiveProductSize,
                                                                                       onlyActiveProductStoreFront,
                                                                                       onlyActiveProductSizeStorefront);
            }
            catch (Exception ex)
            {
                ds = null;
                throw ex;
            }
            return ds;
        }

        // get pet products based on letter filter
        public DataSet GetPetProductBasedOnLetterFilter(string searchLetter,
                                                            int affiliateStoreFrontFk,
                                                            string modifySearchRxOtc,
                                                            int productType,
                                                            bool onlyActiveProduct,
                                                            bool onlyActiveProductSize,
                                                            bool onlyActiveProductStoreFront,
                                                            bool onlyActiveProductSizeStorefront)
        {
            try
            {

                ds = SqlHelper.ExecuteDataset(_sCon, "usp_getPetProductBasedOnLetterFilter", searchLetter,
                                                                                       affiliateStoreFrontFk,
                                                                                       modifySearchRxOtc,
                                                                                       productType,
                                                                                       onlyActiveProduct,
                                                                                       onlyActiveProductSize,
                                                                                       onlyActiveProductStoreFront,
                                                                                       onlyActiveProductSizeStorefront);
            }
            catch (Exception ex)
            {
                ds = null;
                throw ex;
            }
            return ds;
        }

        // to get related product size info for  product in cart (cart view purpose)
        public DataSet GetProductSizeInfoForCart(int productSizeId, int storeFrontFk)
        {
            try
            {

                ds = SqlHelper.ExecuteDataset(_sCon, "usp_getPetProductSizeInfoForCart", productSizeId,storeFrontFk);
            }
            catch (Exception ex)
            {
                ds = null;
                throw ex;
            }
            return ds;
        }

        // used to update limited quantity
        public void UpdateLimitedQty(int productSizeId, int cartQty, int orderInvocieId, DateTime dateCreated)
        {
            try
            {
                SqlParameter[] paramCollection = SqlHelperParameterCache.GetSpParameterSet(_sCon, "usp_UpdateLimitedQty", true);

                paramCollection[1].Value = productSizeId;
                paramCollection[2].Value = cartQty;
                paramCollection[3].Value = orderInvocieId;
                paramCollection[4].Value = dateCreated;
                paramCollection[5].Value = null; // UpdateStatus
                paramCollection[6].Value = null; // Message

                SqlHelper.ExecuteNonQuery(_sCon, CommandType.StoredProcedure, "usp_UpdateLimitedQty", paramCollection);

                var value = paramCollection[5].Value;
                if (value != null)
                {
                    var updateStatus = Convert.ToInt32(value.ToString());
                }

                var message = paramCollection[6].Value;
                if (message != null)
                {
                    message = message.ToString();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        // gets related products for a product, used in product detail page
        public DataSet GetRelatedProducts(int productId,int storeFrontId,string sortBy)
        {
            try
            {

                ds = SqlHelper.ExecuteDataset(_sCon, "usp_be_GetRelatedProducts", productId, storeFrontId, sortBy);
            }
            catch (Exception ex)
            {
                ds = null;
                throw ex;
            }
            return ds;
        }
        // get count of search result for every letter to disable letters that don't have products
        public DataSet GetProductCountByAlphabet(int storeFrontFk,int productType)
        {

            try
            {

                ds = SqlHelper.ExecuteDataset(_sCon, "usp_be_GetProductCountByAlphabet", storeFrontFk, productType);
            }
            catch (Exception ex)
            {
                ds = null;
                throw ex;
            }
            return ds;
        }

        // get count of search result for every letter to disable letters that don't have products
        public DataSet GetPetProductCountByAlphabet(object storeFrontFk)
        {
            try
            {

                ds = SqlHelper.ExecuteDataset(_sCon, "usp_be_GetPetProductCountByAlphabet", storeFrontFk);
            }
            catch (Exception ex)
            {
                ds = null;
                throw ex;
            }
            return ds;
        }

        public DataSet GetMatchingProducts(string searchTerm,
            int storeFrontFk,
            bool activeProduct,
            bool activeProductStorefront)
        {
            try
            {

                ds = SqlHelper.ExecuteDataset(_sCon, "usp_GetMatchingProducts", searchTerm,
                    storeFrontFk,
                    activeProduct,
                    activeProductStorefront);
            }
            catch (Exception ex)
            {
                ds = null;
                throw ex;
            }
            return ds;
        }

        // Save product enquiry details for the unavailable brand/generic for a product
        public int AddProductEnquiry(int productFk, string email, int brandGeneric, int customerFk, int storefrontFk, int dataCollect, DateTime dateCreated)
        {
            try
            {
                SqlParameter[] paramCollection = SqlHelperParameterCache.GetSpParameterSet(_sCon, "usp_AddProductEnquiry", true);

                paramCollection[1].Value = productFk;
                paramCollection[2].Value = email;
                paramCollection[3].Value = customerFk;
                paramCollection[4].Value = storefrontFk;
                paramCollection[5].Value = dateCreated;
                paramCollection[6].Value = brandGeneric;
                paramCollection[7].Value = dataCollect;
                paramCollection[8].Value = null; //Product Enquiry id
                paramCollection[9].Value = null; //message

                SqlHelper.ExecuteNonQuery(_sCon, CommandType.StoredProcedure, "usp_AddProductEnquiry", paramCollection);

                var value = paramCollection[8].Value;
                if (value != null)
                    return Convert.ToInt32(value.ToString());
                return 0;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        // gets Products for top medications
        public DataSet GetTopMedications(int storeFrontId, string productType)
        {
            try
            {

                ds = SqlHelper.ExecuteDataset(_sCon, "usp_getTopMedications", storeFrontId, productType);
            }
            catch (Exception ex)
            {
                ds = null;
                throw ex;
            }
            return ds;
        }
       // gets all Products for general products with display name
        public DataSet GetAllProducts(int storeFrontId)
        {
            try
            {

                ds = SqlHelper.ExecuteDataset(_sCon, "usp_getAllGeneralProducts", storeFrontId);
            }
            catch (Exception ex)
            {
                ds = null;
                throw ex;
            }
            return ds;
        }

        public DataSet GetVolumeDiscountProductSize(int productSizeFk)
        {
            try
            {

                ds = SqlHelper.ExecuteDataset(_sCon, "usp_getVolumeDiscount_ProductSize", productSizeFk);
            }
            catch (Exception ex)
            {
                ds = null;
                throw ex;
            }
            return ds;
        }
    }

 }

