using System;
using Amazon;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using EnhanceClub.WebUI.Infrastructure.Utility;

namespace EnhanceClub.WebUI.AwsHelper
{
    public class GetS3PresignedUrl
    {

       private static IAmazonS3 s3Client;

        public static string GeneratePreSignedUrl(double duration,string objectKey)
        {
            string urlString = "";
            
            AmazonS3Config s3Config = new AmazonS3Config { RegionEndpoint = Amazon.RegionEndpoint.GetBySystemName("ca-central-1"), SignatureMethod = SigningAlgorithm.HmacSHA256 };

          s3Client = new AmazonS3Client(@SiteConfigurationsWc.UploadS3BucketKeyId.ToString(), @SiteConfigurationsWc.UploadS3BucketSecretKey.ToString(), s3Config);

            try
            {
                GetPreSignedUrlRequest request1 = new GetPreSignedUrlRequest
                {
                    BucketName = SiteConfigurationsWc.UploadS3BucketName,
                    Key = objectKey,
                    Expires = DateTime.UtcNow.AddHours(duration)
                };
                urlString = s3Client.GetPreSignedURL(request1);
            }
            catch (AmazonS3Exception e)
            {
                Console.WriteLine("Error encountered on server. Message:'{0}' when writing an object", e.Message);
            }
            catch (Exception e)
            {
                Console.WriteLine("Unknown encountered on server. Message:'{0}' when writing an object", e.Message);
            }
            return urlString;
        }
    }
}