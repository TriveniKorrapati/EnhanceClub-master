using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using EnhanceClub.Domain.Entities;

namespace EnhanceClub.WebUI.Models
{
    public class ImpactLoggedCustomerViewModel
    {

        public string CustomerId { get; set; }
        public string CustomerEmail { get; set; }
        public string SHA1CustomerEmail { get; set; }

    }
}