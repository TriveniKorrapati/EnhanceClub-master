using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnhanceClub.Domain.Entities
{
    public class CustomerShippingAddress: CustomerMinimal
    {
        public int CustomerAddressId { get; set; }

        public bool CustomerDefaultAddress { get; set; }

        public string ShippingAddressProvinceName { get; set; }

        public string ShippingAddress { get; set; }

        public string ShippingCity { get; set; }

        public string ShippingZipCode { get; set; }

        public int ShippingProvinceId { get; set; }

        public int ShippingCountryId { get; set; }
    }
}
