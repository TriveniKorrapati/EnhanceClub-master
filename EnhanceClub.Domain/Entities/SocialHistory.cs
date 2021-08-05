using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnhanceClub.Domain.Entities
{
    public class SocialHistory
    {
        public int SocialHistoryId { get; set; }

        public string SocialHistoryName { get; set; }

        public bool IsSelected { get; set; }
    }
}
