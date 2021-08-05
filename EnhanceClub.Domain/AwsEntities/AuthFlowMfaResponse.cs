using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnhanceClub.Domain.AwsEntities
{
    public class AuthFlowMfaResponse
    {
        public int UserId { get; set; }

        public string AccessToken { get; set; }

        public string IdToken { get; set; }

        public string Message { get; set; }

        public string Exception { get; set; }

        public int ExpiresIn { get; set; }
    }
}
