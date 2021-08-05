using System.Collections.Generic;
using System.Web.Mvc;
using EnhanceClub.Domain.Entities;

namespace EnhanceClub.WebUI.Models
{
    // this view model is used to send customer info and province info as model to views (e.g. update account-info
    public class CustomerInfoViewModel
    {
        public CustomerMinimal CustomerMinimal { get; set; }

        public Customer Customer { get; set; }

        public List<Province> ProvinceList { get; set; }

        public SelectList ProvinceSelectList { get; set; }

        public LoggedCustomer LoggedCustomer { get; set; }

        public List<CustomerShippingAddress> CustomerShippingAddresses { get; set; }
    }
    
}