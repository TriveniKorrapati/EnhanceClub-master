using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnhanceClub.Domain.Entities
{
    public class UserPasswordRecovery
    {
        public int UserPasswordRecoveryId { get; set; }

        public int CustomerFk { get; set; }

        public string EncryptedToken { get; set; }

        public string DecryptedToken { get; set; }

        public DateTime LinkExpiryTime { get; set; }

        public int ResetStatus { get; set; }
    }
}
