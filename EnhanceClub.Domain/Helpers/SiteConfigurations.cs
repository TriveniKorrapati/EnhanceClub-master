using System;
using System.Configuration;
using EnhanceClub.Domain.AwsHelper;
using Newtonsoft.Json;

namespace EnhanceClub.Domain.Helpers
{
    public static class SiteConfigurations
    {
        public static int StoreFrontId
        {
            get
            {
                return Convert.ToInt32(ConfigurationManager.AppSettings["StoreFrontId"]);
            }
        }

        public static string StoreFrontName
        {
            get
            {
                return ConfigurationManager.AppSettings["StoreFrontName"].ToString();

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

        public static string AccessKeyId
        {
            get
            {
                return ConfigurationManager.AppSettings["AccessKeyId"].ToString();
            }
        }
        public static string SecretAccessKey
        {
            get
            {
                return ConfigurationManager.AppSettings["SecretAccessKey"].ToString();
            }
        }
        public static string SecretManagerConnectionKey
        {
            get
            {
                return ConfigurationManager.AppSettings["SecretManagerConnectionKey"].ToString();
            }
        }


        public static string SCon
        {
            get
            {
                
               RdsConnection rdsConnection =  JsonConvert.DeserializeObject<RdsConnection> (AwssmHelper.GetSecret(SecretManagerConnectionKey, AccessKeyId, SecretAccessKey));
               string sCon = rdsConnection.ConString;
                return sCon;
            }
        }

        public static bool EnablePayment
        {
            get
            {
                return  Convert.ToBoolean(ConfigurationManager.AppSettings["EnablePayment"]);
            }
        }

        public static bool ProductSizeVisibleFrontEndOnly
        {
            get
            {
                return Convert.ToBoolean(ConfigurationManager.AppSettings["ProductSizeVisibleFrontEndOnly"]);
            }
        }

        public static int PaymentOptionCustomerCredit
        {
            get
            {
                return Convert.ToInt32(ConfigurationManager.AppSettings["PaymentOptionCustomerCredit"]);
            }
        }

        public static int PaymentTransactionTypeCustomerCreditFk
        {
            get
            {
                return Convert.ToInt32(ConfigurationManager.AppSettings["PaymentTransactionTypeCustomerCreditFk"]);
            }
        }
    }
}
