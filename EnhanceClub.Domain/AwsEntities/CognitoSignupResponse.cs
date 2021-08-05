using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace EnhanceClub.Domain.AwsEntities
{
    public class CognitoSignupResponse
    {
        public bool UserConfirmed { get; set; }
        public HttpStatusCode HttpStatusCode { get; set; }
        public string UserSub { get; set; }
        public string CodeDeliveryMethod { get; set; }
        public bool UserCreated { get; set; }
        public string Message { get; set; }
        public string Exception { get; set; }

        public string ErrorCode { get; set; }
    }
}
