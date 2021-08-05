using System;
using System.IO;
using Amazon;
using Amazon.SecretsManager;
using Amazon.SecretsManager.Model;
using Newtonsoft.Json;

namespace EnhanceClub.Domain.AwsHelper
{
    public class CognitoCredentials
    {
        public string app { get; set; }
        public string Service { get; set; }
        public string POOL_id { get; set; }
        public string CLIENT_id { get; set; }
        public string AWSREGION { get; set; }
        //CongnitoCredentials congnitoCredentials = JsonConvert.DeserializeObject<CongnitoCredentials>(response.SecretString);
    }
    public class AwssmHelper
    {
        public AwssmHelper()
        {
        }

        public static string GetSecret(string secretName, string accessKeyId, string secretAccessKey)
        {
            string secret = "";


            MemoryStream memoryStream = new MemoryStream();
            AmazonSecretsManagerConfig amazonSecretsManagerConfig = new AmazonSecretsManagerConfig();
            amazonSecretsManagerConfig.RegionEndpoint = RegionEndpoint.CACentral1;

            GetSecretValueRequest request = new GetSecretValueRequest();
            IAmazonSecretsManager client = new AmazonSecretsManagerClient(accessKeyId, secretAccessKey, amazonSecretsManagerConfig);
            request.SecretId = secretName;
            request.VersionStage = "AWSCURRENT"; // VersionStage defaults to AWSCURRENT if unspecified.
            GetSecretValueResponse response = null;
            // In this sample we only handle the specific exceptions for the 'GetSecretValue' API.
            // See https://docs.aws.amazon.com/secretsmanager/latest/apireference/API_GetSecretValue.html
            // We rethrow the exception by default.

            try
            {
                response = client.GetSecretValue(request);

                //  response = client.GetSecretValueAsync(request).Result;
            }
            catch (DecryptionFailureException e)
            {
                // Secrets Manager can't decrypt the protected secret text using the provided KMS key.
                // Deal with the exception here, and/or rethrow at your discretion.
                throw;
            }
            catch (InternalServiceErrorException e)
            {
                // An error occurred on the server side.
                // Deal with the exception here, and/or rethrow at your discretion.
                throw;
            }

            catch (InvalidRequestException e)
            {
                // You provided a parameter value that is not valid for the current state of the resource.
                // Deal with the exception here, and/or rethrow at your discretion.
                throw;
            }

            catch (System.AggregateException ae)
            {
                // More than one of the above exceptions were triggered.
                // Deal with the exception here, and/or rethrow at your discretion.
                throw;
            }

            // Decrypts secret using the associated KMS CMK.
            // Depending on whether the secret is a string or binary, one of these fields will be populated.
            if (response.SecretString != null)
            {
                secret = response.SecretString;
            }
            else
            {
                memoryStream = response.SecretBinary;
                StreamReader reader = new StreamReader(memoryStream);
                string decodedBinarySecret = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(reader.ReadToEnd()));
            }
            return response.SecretString;


            // Your code goes here.
        }
    }
}
