using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using EnhanceClub.Domain.Abstract;
using EnhanceClub.Domain.Entities;

namespace EnhanceClub.Domain.Concrete
{
    public class StorefrontRepositorySql : IStorefrontRepository
    {
        private readonly StorefrontDbLayer _storefrontDbl = new StorefrontDbLayer();

        // Get storefront and currency info

        public StoreFrontInfo GetStoreFrontInfo(int storeFrontId)
        {

            DataSet dsStoreFrontInfo = _storefrontDbl.GetStoreFrontInfo(storeFrontId);

            StoreFrontInfo storeFrontInfo = null;

            if (dsStoreFrontInfo.Tables[0].Rows.Count > 0)
            {
                storeFrontInfo =
                    dsStoreFrontInfo.Tables[0].AsEnumerable()
                        .Select(row => new StoreFrontInfo
                        {
                            StoreFrontId = row.Field<int>("StoreFront_Id"),
                            StoreFrontNamePk = Convert.IsDBNull(row.Field<string>("StoreFront_Name_Pk")) ? string.Empty : row.Field<string>("StoreFront_Name_Pk"),
                            StoreFrontPhone = Convert.IsDBNull(row.Field<string>("StoreFront_Phone")) ? string.Empty : row.Field<string>("StoreFront_Phone"),
                            StoreFrontFax = Convert.IsDBNull(row.Field<string>("StoreFront_Fax")) ? string.Empty : row.Field<string>("StoreFront_Fax"),
                            StoreFrontCurrencyAbbreviation = Convert.IsDBNull(row.Field<string>("Currency_Abbreviation_Pk")) ? string.Empty : row.Field<string>("Currency_Abbreviation_Pk"),
                            StoreFrontCurrencyName = Convert.IsDBNull(row.Field<string>("Currency_Name_Pk")) ? string.Empty : row.Field<string>("Currency_Name_Pk"),
                            StoreFrontCurrencyExchangeRate = Convert.IsDBNull(row.Field<decimal>("Currency_ExchangeRate")) ? 0.00m : row.Field<decimal>("Currency_ExchangeRate"),
                            StoreFrontCurrencyId = Convert.IsDBNull(row.Field<int>("Currency_Id")) ? 0 : row.Field<int>("Currency_Id"),
                        }).ToList().FirstOrDefault();

            }
            return storeFrontInfo;
        }

        // get List of ShippingOptions
        public List<ShippingOption> GetShippingOptionsAffiliate(int affiliateId, int shippingOptionId)
        {
            DataSet dsShippingOptions = _storefrontDbl.GetShippingOptionsAffiliate(affiliateId,shippingOptionId);

            List<ShippingOption> shippingOptions = null;

            if (dsShippingOptions.Tables[0].Rows.Count > 0)
            {
                shippingOptions =
                    dsShippingOptions.Tables[0].AsEnumerable()
                        .Select(row => new ShippingOption
                        {
                            ShippingOptionId          = row.Field<int>("ShippingOption_Id"),
                            ShippingOptionAffiliateId = row.Field<int>("ShippingOption_Affiliate_Id"),
                            ShippingOptionName        = Convert.IsDBNull(row.Field<string>("ShippingOption_Name")) ? string.Empty : row.Field<string>("ShippingOption_Name"),
                            ShippingOptionOrder       = Convert.IsDBNull(row.Field<int>("ShippingOption_Order")) ? 0 : row.Field<int>("ShippingOption_Order"),
                            ShippingOptionCost        = Convert.IsDBNull(row.Field<decimal>("ShippingOption_Cost")) ? 0.00m : row.Field<decimal>("ShippingOption_Cost"),
                            ShippingOptionPrice       = Convert.IsDBNull(row.Field<decimal>("ShippingOption_Price")) ? 0.00m : row.Field<decimal>("ShippingOption_Price"),
                            ShippingOptionActive      = row.Field<object>("ShippingOption_Active") != DBNull.Value && Convert.ToBoolean(row.Field<object>("ShippingOption_Active")),
                            ShippingOptionDefault     = row.Field<object>("ShippingOption_Default") != DBNull.Value && Convert.ToBoolean(row.Field<object>("ShippingOption_Default")),
                            ShippingOptionDateCreated = row.Field<object>("ShippingOption_DateCreated") != DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(row.Field<object>("ShippingOption_DateCreated"))

                        }).ToList();

            }
            return shippingOptions;
        }

        // get list of default shipping options

        public List<ShippingOption> GetShippingOptionsDefault(int shippingOptionId)
        {
            DataSet dsShippingOptions = _storefrontDbl.GetShippingOptionsDefault(shippingOptionId);

            List<ShippingOption> shippingOptions = null;

            if (dsShippingOptions.Tables[0].Rows.Count > 0)
            {
                shippingOptions =
                    dsShippingOptions.Tables[0].AsEnumerable()
                        .Select(row => new ShippingOption
                        {
                            ShippingOptionId = row.Field<int>("ShippingOption_Id"),
                            ShippingOptionName = Convert.IsDBNull(row.Field<string>("ShippingOption_Name")) ? string.Empty : row.Field<string>("ShippingOption_Name"),
                            ShippingOptionOrder = Convert.IsDBNull(row.Field<int>("ShippingOption_Order")) ? 0 : row.Field<int>("ShippingOption_Order"),
                            ShippingOptionCost = Convert.IsDBNull(row.Field<decimal>("ShippingOption_Cost")) ? 0.00m : row.Field<decimal>("ShippingOption_Cost"),
                            ShippingOptionPrice = Convert.IsDBNull(row.Field<decimal>("ShippingOption_Price")) ? 0.00m : row.Field<decimal>("ShippingOption_Price"),
                            ShippingOptionActive = row.Field<object>("ShippingOption_Active") != DBNull.Value && Convert.ToBoolean(row.Field<object>("ShippingOption_Active")),
                            ShippingOptionDefault = row.Field<object>("ShippingOption_Default") != DBNull.Value && Convert.ToBoolean(row.Field<object>("ShippingOption_Default")),
                            ShippingOptionDateCreated = row.Field<object>("ShippingOption_DateCreated") != DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(row.Field<object>("ShippingOption_DateCreated"))

                        }).ToList();

            }
            return shippingOptions;
        }

        // get list of blogs in Blog table
        public List<BlogTable> GetBlogList(int blogId, int blogLanguageFk,int storeFrontFk, string sortBy)
        {
            DataSet dsBlogList = _storefrontDbl.GetBlogList(blogId, blogLanguageFk,storeFrontFk, sortBy);

            List<BlogTable> blogList = new List<BlogTable>();

            if (dsBlogList.Tables[0].Rows.Count > 0)
            {
                blogList = GetBlogDataFromDataSet(dsBlogList);
            }
            return blogList;
        }

        // get blog details 
        public BlogTable GetBlog(string blogUrl, int blogLanguageFk, int storeFrontFk)
        {
            DataSet dsBlog = _storefrontDbl.GetBlog(blogUrl, blogLanguageFk,storeFrontFk);

            BlogTable blog = new BlogTable();

            if (dsBlog.Tables[0].Rows.Count > 0)
            {
                blog = GetBlogDataFromDataSet(dsBlog).FirstOrDefault();
            }
            return blog;
        }

        public int AddSubscribedUser(int storefrontFk, string name, string email, string ipAddress, DateTime dateCreated)
        {
            int subscribedUserId = _storefrontDbl.AddSubscribedUser(storefrontFk, name, email, ipAddress, dateCreated);

            return subscribedUserId;
        }

        private List<BlogTable> GetBlogDataFromDataSet(DataSet dsBlogList)
        {
            return
                dsBlogList.Tables[0].AsEnumerable()
                    .Select(row => new BlogTable
                    {
                        BlogId = Convert.IsDBNull(row.Field<object>("Blog_id")) ? 0 : Convert.ToInt32(row.Field<object>("Blog_id")),
                        BlogHeadline = Convert.IsDBNull(row.Field<string>("Blog_headline")) ? string.Empty : row.Field<string>("Blog_headline"),
                        BlogTnImage = Convert.IsDBNull(row.Field<string>("Blog_tn_image")) ? string.Empty : row.Field<string>("Blog_tn_image"),
                        BlogPreview = Convert.IsDBNull(row.Field<string>("Blog_preview")) ? string.Empty : row.Field<string>("Blog_preview"),
                        BlogLink = Convert.IsDBNull(row.Field<string>("Blog_link")) ? string.Empty : row.Field<string>("Blog_link"),
                        BlogActive = row.Field<object>("Blog_active") != DBNull.Value && Convert.ToBoolean(row.Field<object>("Blog_active")),
                        BlogFeatured = row.Field<object>("Blog_featured") != DBNull.Value && Convert.ToBoolean(row.Field<object>("Blog_featured")),
                        BlogDateCreated = row.Field<object>("Blog_datecreated") as DateTime?,
                        BlogLastModified = row.Field<object>("Blog_lastmodified") as DateTime?,
                        BlogStorefrontFk = Convert.IsDBNull(row.Field<object>("Blog_storefront_fk")) ? 0 : Convert.ToInt32(row.Field<object>("Blog_storefront_fk")),
                        BlogStorefrontName = Convert.IsDBNull(row.Field<string>("StorefrontName")) ? string.Empty : row.Field<string>("StorefrontName"),
                        BlogMetaTitle = Convert.IsDBNull(row.Field<string>("Blog_MetaTitle")) ? string.Empty : row.Field<string>("Blog_MetaTitle"),
                        BlogMetaDescription = Convert.IsDBNull(row.Field<string>("Blog_MetaDescription")) ? string.Empty : row.Field<string>("Blog_MetaDescription"),
                        BlogMainImage = Convert.IsDBNull(row.Field<string>("Blog_Mainimage")) ? string.Empty : row.Field<string>("Blog_MainImage")
                    }).ToList();
        }

        // get all blog ids for a storefront
        public List<int> GetAllBlogNumbers(int blogLanguageFk, int storeFrontFk)
        {
            DataSet dsAllBlogList = _storefrontDbl.GetAllBlogNumbers(blogLanguageFk, storeFrontFk);

            List<int> blogIds = null;
            if (dsAllBlogList.Tables[0].Rows.Count > 0)
            {
                blogIds = dsAllBlogList.Tables[0].AsEnumerable()
                    .Select(r => r.Field<int>("Blog_Id"))
                    .ToList();
            }
            return blogIds;

        }
        // Get Payment options for storefront
        public List<PaymentOption> GetPaymentOptionsStoreFront(int storeFrontId,
            int paymentOptionFk,
            bool active)
        {
            DataSet dsPaymentOption = _storefrontDbl.GetPaymentOptionsStoreFront(storeFrontId, paymentOptionFk, active);

            List<PaymentOption> paymentOptionList = null;

            if (dsPaymentOption.Tables[0].Rows.Count > 0)
            {
                paymentOptionList = dsPaymentOption.Tables[0].AsEnumerable()
                    .Select(row => new PaymentOption
                    {
                        PaymentOptionId =
                            Convert.IsDBNull(row.Field<object>("Paymentoption_Id"))
                                ? 0
                                : Convert.ToInt32(row.Field<object>("Paymentoption_Id")),
                        PaymentOptionCurrencyFk =
                            Convert.IsDBNull(row.Field<object>("Paymentoption_Currency_Fk"))
                                ? 0
                                : Convert.ToInt32(row.Field<object>("Paymentoption_Currency_Fk")),
                        PaymentOptionPaymentTypeFk =
                            Convert.IsDBNull(row.Field<object>("Paymentoption_Paymenttype_Fk"))
                                ? 0
                                : Convert.ToInt32(row.Field<object>("Paymentoption_Paymenttype_Fk")),

                        PaymentOptionName =
                            Convert.IsDBNull(row.Field<string>("Paymentoption_Name"))
                                ? string.Empty
                                : row.Field<string>("Paymentoption_Name"),

                        PaymentOptionUsername =
                            Convert.IsDBNull(row.Field<string>("Paymentoption_Username"))
                                ? string.Empty
                                : row.Field<string>("Paymentoption_Username"),

                        PaymentOptionPassword = Convert.IsDBNull(row.Field<string>("Paymentoption_Password"))
                            ? string.Empty
                            : row.Field<string>("Paymentoption_Password"),

                        PaymentOptionActive =
                            row.Field<object>("Paymentoption_Active") != DBNull.Value &&
                            Convert.ToBoolean(row.Field<object>("Paymentoption_Active")),
                        PaymentOptionCc =
                            row.Field<object>("Paymentoption_cc") != DBNull.Value &&
                            Convert.ToBoolean(row.Field<object>("Paymentoption_cc")),

                        PaymentOptionOrder =
                            Convert.IsDBNull(row.Field<object>("Paymentoption_Order"))
                                ? 0
                                : Convert.ToInt32(row.Field<object>("Paymentoption_Order"))

                    }).ToList();
            }

            return paymentOptionList;
        }

    }
    
}
