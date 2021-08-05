using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using EnhanceClub.Domain.Helpers;

namespace EnhanceClub.Domain.Concrete
{
    public class StorefrontDbLayer
    {
        //readonly string _sCon = ConfigurationManager.ConnectionStrings["Connection"].ToString();

        private readonly string _sCon = @SiteConfigurations.SCon;

        DataSet _ds;

        // get storefront and currency info

        public DataSet GetStoreFrontInfo(int storeFrontId)
        {
            try
            {
                _ds = SqlHelper.ExecuteDataset(_sCon, "usp_getStoreFrontInfo", storeFrontId);
            }
            catch (Exception ex)
            {
                _ds = null;
                throw ex;
            }
            return _ds;

        }

        // get shipping options for Affiliate

        public DataSet GetShippingOptionsAffiliate(int affiliateId,int shippingOptionId)
        {
            try
            {
                _ds = SqlHelper.ExecuteDataset(_sCon, "usp_getShippingOptionsAffiliate", affiliateId, shippingOptionId);
            }
            catch (Exception ex)
            {
                _ds = null;
                throw ex;
            }
            return _ds;
        }

        // get default shipping options
        public DataSet GetShippingOptionsDefault(int shippingOptionId)
        {
            try
            {
                _ds = SqlHelper.ExecuteDataset(_sCon, "usp_getShippingOptionsDefault", shippingOptionId);
            }
            catch (Exception ex)
            {
                _ds = null;
                throw ex;
            }
            return _ds;
        }

        // get list of blogs in Blog table
        public DataSet GetBlogList(int blogId, int blogLanguageFk, int storeFrontFk, string sortBy)
        {
            try
            {
                _ds = SqlHelper.ExecuteDataset(_sCon, "usp_GetBlogTable", blogId, null, blogLanguageFk,storeFrontFk, sortBy);
            }
            catch (Exception ex)
            {
                _ds = null;
                throw ex;
            }
            return _ds;
        }

        // get blog details
        public DataSet GetBlog(string blogUrl, int blogLanguageFk, int storeFrontFk)
        {
            try
            {
                _ds = SqlHelper.ExecuteDataset(_sCon, "usp_GetBlogTable", 0, blogUrl,blogLanguageFk, storeFrontFk, "");
            }
            catch (Exception ex)
            {
                _ds = null;
                throw ex;
            }
            return _ds;
        }

        // add subscribed user for news letter
        public int AddSubscribedUser(int storefrontFk, string name, string email, string ipAddress, DateTime dateCreated)
        {
            try
            {
                SqlParameter[] paramCollection = SqlHelperParameterCache.GetSpParameterSet(_sCon, "usp_AddSubscribedUser", true);

                paramCollection[1].Value = storefrontFk;
                paramCollection[2].Value = name;
                paramCollection[3].Value = email;
                paramCollection[4].Value = ipAddress;
                paramCollection[5].Value = dateCreated;
                paramCollection[6].Value = null; // new Id generated
                paramCollection[7].Value = null;  // message

                SqlHelper.ExecuteNonQuery(_sCon, CommandType.StoredProcedure, "usp_AddSubscribedUser", paramCollection);

                var msg = paramCollection[7].Value;

                var value = paramCollection[6].Value;
                if (value != null) return Convert.ToInt32(value.ToString());

                return 0;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        // get all blog ids for a storefront
        public DataSet GetAllBlogNumbers(int blogLanguageFk, int storeFrontFk)
        {
            try
            {
                _ds = SqlHelper.ExecuteDataset(_sCon, "usp_GetAllBlogNumbers", blogLanguageFk, storeFrontFk);
            }
            catch (Exception ex)
            {
                _ds = null;
                throw ex;
            }
            return _ds;
        }
        // get Payment options for store front
        public DataSet GetPaymentOptionsStoreFront(int customerStoreFrontId, int paymentOptionFk, bool active)
        {
            try
            {
                _ds = SqlHelper.ExecuteDataset(_sCon, "usp_be_getPaymentOptionsStoreFront", customerStoreFrontId, paymentOptionFk, active);
            }
            catch (Exception ex)
            {
                _ds = null;
                throw ex;
            }
            return _ds;
        }
    }
}
