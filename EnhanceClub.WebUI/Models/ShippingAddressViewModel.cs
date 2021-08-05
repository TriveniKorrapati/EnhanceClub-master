using EnhanceClub.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EnhanceClub.WebUI.Models
{
    public class ShippingAddressViewModel
    {
      
        public List<Province> ProvinceList { get; set; }

        public List<CustomerShippingAddress> CustomerShippingAddress { get; set; }

        public bool AllowEdit { get; set; }

        public int SelectedShipAddressFk { get; set; }

        public int ShippingAddressProvinceFk { get; set; }
    }
}