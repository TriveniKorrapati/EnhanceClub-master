using System;
using System.Configuration;
using System.Web.Mvc;
using EnhanceClub.Domain.Entities;

namespace EnhanceClub.WebUI.Infrastructure.Binders
{
    public class AffiliateInfoModelBinder : IModelBinder
    {
        private const string sessionKey = "AffiliateInfo";

        public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            // Affiliate Id is saved in Web.config, if there is Affiliate ID in Session (future use where affiliate is decided based on other logic) use it, otherwise extract from  Web.config 
            AffiliateInfo affiliateInfo = null;

            if (controllerContext.HttpContext.Session != null)
            {
                affiliateInfo = (AffiliateInfo)controllerContext.HttpContext.Session[sessionKey];
            }

            var storeFrontId = Convert.ToInt32(ConfigurationManager.AppSettings["StoreFrontId"].ToString());
            var storeFrontFax = ConfigurationManager.AppSettings["StoreFrontFax"].ToString();
            var storeFrontContact = ConfigurationManager.AppSettings["StoreFrontContact"].ToString();

            var storeFrontEnquiry = ConfigurationManager.AppSettings["storeFrontEnquiry"].ToString();
            var storeFrontWebsiteProblems = ConfigurationManager.AppSettings["StoreFrontWebsiteProblems"].ToString();
            var storeFrontMarketing = ConfigurationManager.AppSettings["storeFrontMarketing"].ToString();
            var storeFrontSales = ConfigurationManager.AppSettings["StoreFrontSales"].ToString();
            var freeShipShippingOption = Convert.ToInt32(ConfigurationManager.AppSettings["FreeShipShippingOption"].ToString());
            var flatRateShippingOption = Convert.ToInt32(ConfigurationManager.AppSettings["FlatRateShippingOption"].ToString());
            var flatRateShippingPrice = Convert.ToDecimal(ConfigurationManager.AppSettings["FlatRateShippingPrice"].ToString());
            var freeShippingThreshHold = Convert.ToDecimal(ConfigurationManager.AppSettings["FreeShippingThreshHold"].ToString()); // determines when code in checkout step three switches to free shipping

            var noChargePaymentOption = Convert.ToInt32(ConfigurationManager.AppSettings["NoChargePaymentOption"].ToString());
            var refererCredit = Convert.ToDecimal(ConfigurationManager.AppSettings["RefererCredit"].ToString());
            var referredCredit = Convert.ToDecimal(ConfigurationManager.AppSettings["ReferredCredit"].ToString());

            var storefrontName = ConfigurationManager.AppSettings["storeFrontName"].ToString();
            var storefrontUrl = ConfigurationManager.AppSettings["storeFrontUrl"].ToString();
            var storefrontLogo = ConfigurationManager.AppSettings["storeFrontLogo"].ToString();
            var storefrontEmailHeader = ConfigurationManager.AppSettings["storeFrontEmailHeader"].ToString();
            var defaultPaymentOptionFk = Convert.ToInt32(ConfigurationManager.AppSettings["DefaultPaymentOptionFk"].ToString());
            var defaultPaymentOptionName = ConfigurationManager.AppSettings["DefaultPaymentOptionName"].ToString();
            var paymentTransactionType = ConfigurationManager.AppSettings["PaymenyTransactionType"].ToString();

            if (affiliateInfo == null)
            {
                // if it is not defined in session, extract from web.config
                var affiliateId = Convert.ToInt32(ConfigurationManager.AppSettings["AffiliateID"].ToString());

                // extract store front id from web.config
            
                affiliateInfo = new AffiliateInfo {AffiliateId = affiliateId, 
                                                   AffiliateStoreFrontFk = storeFrontId,
                                                   StoreFrontFax = storeFrontFax,
                                                   StorefrontContact= storeFrontContact,
                                                   StoreFrontEnquiry = storeFrontEnquiry,
                                                   StoreFrontMarketing = storeFrontMarketing,
                                                   StoreFrontSales = storeFrontSales,
                                                   StoreFrontWebsiteProblems = storeFrontWebsiteProblems,
                                                   FreeShipShippingOption = freeShipShippingOption,
                                                   FlatRateShippingOption = flatRateShippingOption,
                                                   FlatRateShippingPrice = flatRateShippingPrice,
                                                   NoChargePaymentOption = noChargePaymentOption,
                                                   ReferrerCredit = refererCredit,
                                                   ReferredCredit = referredCredit,
                                                   StorefrontUrl = storefrontUrl,
                                                   StorefrontLogo = storefrontLogo,
                                                   StorefrontEmailHeader = storefrontEmailHeader,
                                                   StorefrontName = storefrontName,
                                                   FreeShippingThreshHold = freeShippingThreshHold,
                                                   DefaultPaymentOptionFk = defaultPaymentOptionFk,
                                                   DefaultPaymentOptionName = defaultPaymentOptionName,
                                                   PaymentTransactionType = paymentTransactionType
                                                   };

            }
            else
            {
                affiliateInfo.AffiliateStoreFrontFk = storeFrontId;
            }

            return affiliateInfo;
        }
    }
}