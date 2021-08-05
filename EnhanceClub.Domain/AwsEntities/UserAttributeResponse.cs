using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace EnhanceClub.Domain.AwsEntities
{
    public class UserAttributeResponse
    {
        public string AccessToken { get; set; }

        public HttpStatusCode HttpStatusCode { get; set; }

        public string Message { get; set; }

        public string Exception { get; set; }
    }
}
