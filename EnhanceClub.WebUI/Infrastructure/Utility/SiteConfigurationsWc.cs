using System;
using System.Configuration;

namespace EnhanceClub.WebUI.Infrastructure.Utility
{
    public class SiteConfigurationsWc
    {
        // V.Imp,  controllers have access to these variables using affiliate model binders
        // however in master layout pages I wanted to avoid any kind of model, so I used this file to make data avaialble to layout page

        public static string ExitCoupon
        {
            get
            {
                return ConfigurationManager.AppSettings["ExitCoupon"].ToString();
            }
        }

        public static bool UseCdn
        {
            get
            {
                return Convert.ToBoolean(ConfigurationManager.AppSettings["UseCdn"]);
            }
        }

        public static string CdnUrl
        {
            get
            {
                return ConfigurationManager.AppSettings["CdnUrl"];
            }
        }

        public static string EmailTemplatePath
        {
            get
            {
                return ConfigurationManager.AppSettings["EmailTemplatePath"];
            }
        }
        // indicates dev or live mode (all emails are sent to developer in dev mode) 

        public static string Environment
        {
            get
            {
                return ConfigurationManager.AppSettings["Environment"];
            }
        }

        public static string StorefrontId
        {
            get
            {
                return ConfigurationManager.AppSettings["StoreFrontID"].ToString();
            }
        }

        public static int StoreFrontCountryId
        {
            get
            {
                return Convert.ToInt32(ConfigurationManager.AppSettings["StoreFrontCountryId"]);
            }
        }


        public static string StorefrontFax
        {
            get
            {
                return ConfigurationManager.AppSettings["StoreFrontFax"].ToString();
            }
        }

        public static string StorefrontContact
        {
            get
            {
                return ConfigurationManager.AppSettings["StoreFrontContact"].ToString();
            }
        }

        public static string StorefrontEnquiry
        {
            get
            {
                return ConfigurationManager.AppSettings["storeFrontEnquiry"].ToString();
            }
        }

        public static string StorefrontCustomerService
        {
            get
            {
                return ConfigurationManager.AppSettings["StoreFrontCustomerService"].ToString();
            }
        }

        public static string StorefrontSales
        {
            get
            {
                return ConfigurationManager.AppSettings["StoreFrontSales"].ToString();
            }
        }

        public static string StorefrontLogo
        {
            get
            {
                return ConfigurationManager.AppSettings["storeFrontLogo"].ToString();
            }
        }

        public static string StoreFrontSpecify
        {
            get
            {
                return ConfigurationManager.AppSettings["StoreFrontSpecify"].ToString();
            }
        }

        public static string StorefrontName
        {
            get
            {
                return ConfigurationManager.AppSettings["storeFrontName"].ToString();
            }
        }

        public static string StorefrontAddress1
        {
            get
            {
                return ConfigurationManager.AppSettings["storeFrontAddress1"].ToString();
            }
        }


        public static string StorefrontAddress2
        {
            get
            {
                return ConfigurationManager.AppSettings["storeFrontAddress2"].ToString();
            }
        }

        public static string StorefrontAddress3
        {
            get
            {
                return ConfigurationManager.AppSettings["storeFrontAddress3"].ToString();
            }
        }

        public static string StorefrontUrl
        {
            get
            {
                return ConfigurationManager.AppSettings["storeFrontUrl"].ToString();
            }
        }

        public static string StorefrontEmailHeader
        {
            get
            {
                return ConfigurationManager.AppSettings["storeFrontEmailHeader"].ToString();
            }
        }

        public static string FreeShippingThreshHold
        {
            get
            {
                return ConfigurationManager.AppSettings["FreeShippingThreshHold"].ToString();
            }
        }

        public static string FlatRateShippingPrice
        {
            get
            {
                return ConfigurationManager.AppSettings["FlatRateShippingPrice"].ToString();
            }
        }
        public static string FreeShippingPrice
        {
            get
            {
                return ConfigurationManager.AppSettings["FreeShippingPrice"].ToString();
            }
        }

        public static string EmailFrom
        {
            get
            {
                return ConfigurationManager.AppSettings["SignupEmailFrom"].ToString();
            }
        }
        public static string GetInTouchMailFromAddress
        {
            get
            {
                return ConfigurationManager.AppSettings["GetInTouchEmailFrom"].ToString();
            }
        }
        public static string IPStackAPIKey
        {
            get
            {
                return ConfigurationManager.AppSettings["IPStackAPIKey"].ToString();
            }
        }

        public static string IPStackAPIUrl
        {
            get
            {
                return ConfigurationManager.AppSettings["IPStackAPIUrl"].ToString();
            }
        }

        public static string IpifyAPIUrl
        {
            get
            {
                return ConfigurationManager.AppSettings["IpifyAPIUrl"].ToString();
            }
        }

        public static string ShippingtimeFrame
        {
            get
            {
                return ConfigurationManager.AppSettings["ShippingtimeFrame"].ToString();
            }
        }
        public static string ProductSizeImagePath
        {
            get
            {
                try
                {
                    return ConfigurationManager.AppSettings["ProductSizeImagePath"].ToString();
                }
                catch
                {

                    throw new Exception();
                }
            }
        }
        public static bool EnableTax
        {
            get
            {
                return Convert.ToBoolean(ConfigurationManager.AppSettings["EnableTax"]);
            }
        }

        public static bool EmailSend
        {
            get
            {
                return Convert.ToBoolean(ConfigurationManager.AppSettings["EmailSend"]);
            }
        }

        public static string AesEncryptionKey
        {
            get
            {
                return ConfigurationManager.AppSettings["AesEncryptionKey"].ToString();

            }
        }

        public static int PasswordRecoveryLinkExpiryTime
        {
            get
            {
                return Convert.ToInt32(ConfigurationManager.AppSettings["PasswordRecoveryLinkExpiryTime"]);
            }
        }
        public static string PrescriptionPrefix
        {
            get
            {
                return ConfigurationManager.AppSettings["PrescriptionPrefix"].ToString();

            }
        }

        public static string PrescriptionUploadDir
        {
            get
            {
                return ConfigurationManager.AppSettings["PrescriptionUploadDir"].ToString();

            }
        }

        public static string CustomerIdUploadDir
        {
            get
            {
                return ConfigurationManager.AppSettings["CustomerIdUploadDir"].ToString();
            }
        }

        public static string CustomerIdDocumentPrefix
        {
            get
            {
                return ConfigurationManager.AppSettings["CustomerIdDocumentPrefix"].ToString();
            }
        }

        public static string ProductImagePath
        {
            get
            {
                return ConfigurationManager.AppSettings["ProductImagePath"].ToString();
            }
        }

        public static string ProductGenericImagePrefix
        {
            get
            {
                return ConfigurationManager.AppSettings["ProductGenericImagePrefix"].ToString();
            }
        }

        public static string ProductBrandImagePrefix
        {
            get
            {
                return ConfigurationManager.AppSettings["ProductBrandImagePrefix"].ToString();
            }
        }
        public static bool EnableProductGeneric
        {
            get
            {
                return Convert.ToBoolean(ConfigurationManager.AppSettings["EnableProductGeneric"]);
            }
        }

        public static int QuestionnaireTimeSpan
        {
            get
            {
                return Convert.ToInt32(ConfigurationManager.AppSettings["QuestionnaireTimeSpan"]);
            }
        }

        public static int QuestionnaireTimeSpanDays
        {
            get
            {
                return Convert.ToInt32(ConfigurationManager.AppSettings["QuestionnaireTimeSpanDays"]);
            }
        }
        public static string SecurityInfoEnquiry
        {
            get
            {
                return ConfigurationManager.AppSettings["SecurityInfoEnquiry"].ToString();
            }
        }
        public static string TermForPractitioner
        {
            get
            {
                return ConfigurationManager.AppSettings["TermForPractitioner"].ToString();
            }
        }

        public static string UploadS3BucketKeyId
        {
            get
            {
                return ConfigurationManager.AppSettings["UploadS3BucketKeyId"].ToString();
            }
        }

        public static string UploadS3BucketName
        {
            get
            {
                return ConfigurationManager.AppSettings["UploadS3BucketName"].ToString();
            }
        }

        public static string UploadS3BucketSecretKey
        {
            get
            {
                return ConfigurationManager.AppSettings["UploadS3BucketSecretKey"].ToString();
            }
        }
        public static string UploadS3CustomerIdDir
        {
            get
            {
                return ConfigurationManager.AppSettings["UploadS3CustomerIdDir"].ToString();
            }
        }
        public static string UploadS3CustomerIdDocumentPrefix
        {
            get
            {
                return ConfigurationManager.AppSettings["UploadS3CustomerIdDocumentPrefix"].ToString();
            }
        }

        public static string UploadS3PrescriptionDir
        {
            get
            {
                return ConfigurationManager.AppSettings["UploadS3PrescriptionDir"].ToString();
            }
        }
        public static string UploadS3PrescriptionPrefix
        {
            get
            {
                return ConfigurationManager.AppSettings["UploadS3PrescriptionPrefix"].ToString();
            }
        }
        public static string PayFlowUrlTest
        {
            get
            {

                return ConfigurationManager.AppSettings["PayFlowUrlTest"].ToString();

            }
        }

        public static string PayFlowUrlLive
        {
            get
            {

                return ConfigurationManager.AppSettings["PayFlowUrlLive"].ToString();

            }
        }
        public static int DefaultPaymentOptionFk
        {
            get
            {

                return Convert.ToInt32(ConfigurationManager.AppSettings["DefaultPaymentOptionFk"].ToString());

            }
        }
        public static bool EnablePayment
        {
            get
            {
                return Convert.ToBoolean(ConfigurationManager.AppSettings["EnablePayment"]);
            }
        }

        public static bool EnableSubscription
        {
            get
            {
                return Convert.ToBoolean(ConfigurationManager.AppSettings["EnableSubscription"]);
            }
        }

        public static int SubscriptionInterval
        {
            get
            {
                return Convert.ToInt32(ConfigurationManager.AppSettings["SubscriptionInterval"]);
            }
        }

        public static int ShowProductStrengthUnitTogether
        {
            get
            {
                return Convert.ToInt32(ConfigurationManager.AppSettings["ShowProductStrengthUnitTogether"]);
            }
        }


        public static bool EnablePrescriptionNotification
        {
            get
            {
                return Convert.ToBoolean(ConfigurationManager.AppSettings["EnablePrescriptionNotification"]);
            }
        }
        public static string PrivacyPolicyAddress1
        {
            get
            {
                return ConfigurationManager.AppSettings["PrivacyPolicyAddress1"].ToString();
            }
        }
        public static string PrivacyPolicyAddress2
        {
            get
            {
                return ConfigurationManager.AppSettings["PrivacyPolicyAddress2"].ToString();
            }
        }
        public static string PrivacyPolicyAddress3
        {
            get
            {
                return ConfigurationManager.AppSettings["PrivacyPolicyAddress3"].ToString();
            }
        }
        public static string PrivacyPolicyEmail
        {
            get
            {
                return ConfigurationManager.AppSettings["PrivacyPolicyEmail"].ToString();
            }
        }

        public static int UseCognitoSignup
        {
            get
            {
                return Convert.ToInt32(ConfigurationManager.AppSettings["UseCognitoSignup"]);
            }
        }
        public static string ShowProductBrandNameWithGeneric
        {
            get
            {
                return ConfigurationManager.AppSettings["ShowProductBrandNameWithGeneric"].ToString();
            }
        }

        public static string DirecPostAPIKeyDev
        {
            get
            {
                return ConfigurationManager.AppSettings["DirecPostAPIKeyDev"].ToString();
            }
        }

        public static string DirecPostAPIKeyLive
        {
            get
            {
                return ConfigurationManager.AppSettings["DirecPostAPIKeyLive"].ToString();
            }
        }

        public static string DirectPostAPIUrl
        {
            get
            {
                return ConfigurationManager.AppSettings["DirectPostAPIUrl"].ToString();
            }
        }

        public static int EnableChat
        {
            get
            {
                return Convert.ToInt32(ConfigurationManager.AppSettings["EnableChat"]);

            }
        }

        public static bool SaveCreditCardDetails
        {
            get
            {
                return Convert.ToBoolean(ConfigurationManager.AppSettings["SaveCreditCardDetails"]);
            }
        }

        public static string PaymenyTransactionType
        {
            get
            {
                return ConfigurationManager.AppSettings["PaymenyTransactionType"].ToString();
            }
        }

       public static string DebugMessageRecipient
        {
            get
            {
                return ConfigurationManager.AppSettings["DebugMessageRecipient"].ToString();
            }
        }

        public static bool UpdateConsultationConsentForAllOrders
        {
            get
            {
                return Convert.ToBoolean(ConfigurationManager.AppSettings["UpdateConsultationConsentForAllOrders"]);
            }

        }
        public static bool InactiveProductLinkHide
        {
            get
            {
                return Convert.ToBoolean(ConfigurationManager.AppSettings["InactiveProductLinkHide"]);
            }

        }

        public static bool ProductSizeVisibleFrontEndOnly
        {
            get
            {
                return Convert.ToBoolean(ConfigurationManager.AppSettings["ProductSizeVisibleFrontEndOnly"]);
            }
        }

        public static string ApplicableLocation
        {
            get
            {
                return ConfigurationManager.AppSettings["ApplicableLocation"];
            }
        }

        public static string PriceCurrency
        {
            get
            {
                return ConfigurationManager.AppSettings["PriceCurrency"];
            }
        }
        public static string EnableBloodPressurePopup
        {
            get
            {
                return ConfigurationManager.AppSettings["EnableBloodPressurePopup"];
            }
        }
        public static int EnableCaptcha
        {
            get
            {
                return Convert.ToInt32(ConfigurationManager.AppSettings["EnableCaptcha"]);
            }
        }

        public static string RecaptchaSiteKey
        {
            get
            {
                return Convert.ToString(ConfigurationManager.AppSettings["reCAPTCHASiteKey"]);
            }
        }

        public static string RecaptchaSecretKey
        {
            get
            {
                return Convert.ToString(ConfigurationManager.AppSettings["reCAPTCHASecretKey"]);
            }
        }

        public static string RecaptchaTokenVerifyUri
        {
            get
            {
                return Convert.ToString(ConfigurationManager.AppSettings["reCAPTCHATokenVerifyUri"]);
            }
        }

        public static double RecaptchaScore
        {
            get
            {
                return Convert.ToDouble(ConfigurationManager.AppSettings["reCAPTCHAScore"]);
            }
        }

        public static string StorefrontCountryName
        {
            get
            {
                return ConfigurationManager.AppSettings["StorefrontCountryName"];
            }
        }

        public static int ShowEditCredentials
        {
            get
            {
                return Convert.ToInt32(ConfigurationManager.AppSettings["ShowEditCredentials"]);
            }
        }

        public static int ShowMultipleShippingAddress
        {
            get
            {
                return Convert.ToInt32(ConfigurationManager.AppSettings["ShowMultipleShippingAddress"]);
            }
        }

        public static string FooterProductsFk
        {
            get
            {
                return ConfigurationManager.AppSettings["FooterProductsFk"];
            }
        }

        public static int HomePageUpdateJune2021
        {
            get
            {
                return Convert.ToInt32(ConfigurationManager.AppSettings["HomePageUpdate-June2021"]);
            }
        }

        public static int ProductPageUpdateJune2021
        {
            get
            {
                return Convert.ToInt32(ConfigurationManager.AppSettings["ProductPageUpdate-June2021"]);
            }
        }

        public static int IsBPKnownLogic
        {
            get
            {
                return Convert.ToInt32(ConfigurationManager.AppSettings["IsBPKnownLogic"]);
            }
        }

        public static int BPNotKnownProblemId
        {
            get
            {
                return Convert.ToInt32(ConfigurationManager.AppSettings["BPNotKnownProblemId"]);
            }
        }

        public static int Interval
        {
            get
            {
                return Convert.ToInt32(ConfigurationManager.AppSettings["Interval"]);
            }
        }

        public static int MaxEmailSentCount
        {
            get
            {
                return Convert.ToInt32(ConfigurationManager.AppSettings["MaxEmailSentCount"]);
            }
        }

        public static int IsZendeskActive
        {
            get
            {
                return Convert.ToInt32(ConfigurationManager.AppSettings["IsZendeskActive"]);
            }
        }

        public static string ZendeskDomain
        {
            get
            {
                return Convert.ToString(ConfigurationManager.AppSettings["ZendeskDomain"]);
            }
        }

        public static string ZendeskEmail
        {
            get
            {
                return Convert.ToString(ConfigurationManager.AppSettings["ZendeskEmail"]);
            }
        }

        public static string ZendeskToken
        {
            get
            {
                return Convert.ToString(ConfigurationManager.AppSettings["ZendeskToken"]);
            }
        }

        public static string ZendeskCategoryFieldId
        {
            get
            {
                return Convert.ToString(ConfigurationManager.AppSettings["ZendeskCategoryFieldId"]);
            }
        }

        public static string ZendeskPhoneFieldId
        {
            get
            {
                return Convert.ToString(ConfigurationManager.AppSettings["ZendeskPhoneFieldId"]);
            }
        }

        public static int ShowPromotionBanner
        {
            get
            {
                return Convert.ToInt32(ConfigurationManager.AppSettings["ShowPromotionBanner"]);
            }
        }
        public static int FileUploadSizeLimit
        {
            get
            {
                return Convert.ToInt32(ConfigurationManager.AppSettings["FileUploadSizeLimit"]);
            }
        }

        public static bool CustomerQuestionnaireIssuesFlag
        {
            get
            {
                return Convert.ToBoolean(ConfigurationManager.AppSettings["CustomerQuestionnaireIssuesFlag"]);
            }
        }

        public static int TrackCustomerQuestionnaireResponse
        {
            get
            {
                return Convert.ToInt32(ConfigurationManager.AppSettings["TrackCustomerQuestionnaireResponse"]);
            }
        }

        public static int ShowHomePageDoctorSection
        {
            get
            {
                return Convert.ToInt32(ConfigurationManager.AppSettings["ShowHomePageDoctorSection"]);
            }
        }

        public static int CreditAvaliableLogic
        {
            get
            {
                return Convert.ToInt32(ConfigurationManager.AppSettings["CreditAvaliableLogic"]);
            }
        }

        public static int EnableRefill
        {
            get
            {
                return Convert.ToInt32(ConfigurationManager.AppSettings["EnableRefill"]);
            }
        }
        public static int EnableImpactScript
        {
            get
            {
                return Convert.ToInt32(ConfigurationManager.AppSettings["EnableImpactScript"]);
            }
        }

        public static int CheckoutDiscountOffer
        {
            get
            {
                return Convert.ToInt32(ConfigurationManager.AppSettings["CheckoutDiscountOffer"]);
            }
        }

        public static int CheckoutDiscountOfferExpiryDays
        {
            get
            {
                return Convert.ToInt32(ConfigurationManager.AppSettings["CheckoutDiscountOfferExpiryDays"]);
            }
        }

    }
}