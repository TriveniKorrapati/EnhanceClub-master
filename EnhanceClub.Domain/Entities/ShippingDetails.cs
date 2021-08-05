namespace EnhanceClub.Domain.Entities
{
    public class ShippingDetails
    {
        
        public string ShippingFirstName { get; set; }

        public string ShippingLastName { get; set; }

        public string ShippingEmail { get; set; }

        public string ShippingPhone { get; set; }

        public string ShippingAddress { get; set; }

        public string ShippingCity { get; set; }

        public int ShippingCountryId { get; set; }

        public int ShippingProvinceId { get; set; }

        public string ShippingZipCode { get; set; }
    }
}

