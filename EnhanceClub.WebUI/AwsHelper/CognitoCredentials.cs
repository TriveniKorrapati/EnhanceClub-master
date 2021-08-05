using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EnhanceClub.WebUI.AwsHelper
{
    public class CognitoCredentials
    {
        public string App { get; set; }

        public string Service { get; set; }

        [JsonProperty("POOL_id")]
        public string PoolId { get; set; }

        [JsonProperty("CLIENT_id")]
        public string ClientId { get; set; }

        public string AwsRegion { get; set; }
       
    }
}
