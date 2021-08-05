using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace EnhanceClub.Domain.AwsEntities
{
    // class is used to send verification response
    public class CognitoConfirmSignupResponse
    {
        public HttpStatusCode HttpStatusCode { get; set; }

        public string Message { get; set; }

        public string Exception { get; set; }

        public bool AccessCodeVerified { get; set; }

        public string AccessCode { get; set; }
    }
}
