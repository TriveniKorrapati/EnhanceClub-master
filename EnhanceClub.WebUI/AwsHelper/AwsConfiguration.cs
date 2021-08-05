using EnhanceClub.Domain.AwsHelper;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace EnhanceClub.WebUI.AwsHelper
{
    public class AwsConfiguration
    {
        public static string CognitoAccessKeyId => ConfigurationManager.AppSettings["AccessKeyId"];
        public static string CognitoSecretAccessKey => ConfigurationManager.AppSettings["SecretAccessKey"];

        public static string SecretManagerCognitoUserPoolKey =>
            ConfigurationManager.AppSettings["SecretManagerCognitoUserPoolKey"];

        static CognitoCredentials cognitoCredentials = JsonConvert.DeserializeObject<CognitoCredentials>
        (AwssmHelper.GetSecret(AwsConfiguration.SecretManagerCognitoUserPoolKey,
            AwsConfiguration.CognitoAccessKeyId,
            AwsConfiguration.CognitoSecretAccessKey));

        public static string PoolId = cognitoCredentials.PoolId;
        public static string ClientId = cognitoCredentials.ClientId;
    }
}