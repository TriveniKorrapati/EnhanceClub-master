using EnhanceClub.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EnhanceClub.WebUI.Models
{
    public class RefillProductsViewModel
    {
        public IEnumerable<CustomerSubscription> AutoFillProductsList { get; set; }
       
    }
}