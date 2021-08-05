using System.Collections.Generic;

namespace EnhanceClub.WebUI.Models
{
    public class BreadCrumbViewModel
    {
        public IEnumerable<BreadCrumbLink> BreadCrumbLinks { get; set; }
    }
}