using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;

namespace EnhanceClub.Domain.AwsEntities
{
    public class SrpAuthResponse
    {
        private IEnumerable<string> ChallengeName { get; set; }

        public string SessionID { get; set; }

        public string Message { get; set; }

        public string Exception { get; set; }

        public string CustomerEmail { get; set; }

        public string AccessToken { get; set; }

        public bool UserNotConfirmed { get; set; }
    }
}
