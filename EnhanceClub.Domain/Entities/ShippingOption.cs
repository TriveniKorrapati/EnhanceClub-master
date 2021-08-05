using System;

namespace EnhanceClub.Domain.Entities
{
    // used for shipping options
    public class ShippingOption
    {
        public int ShippingOptionId { get; set; }
        public int ShippingOptionAffiliateId { get; set; }
       
        public string ShippingOptionName { get; set; }
        public int ShippingOptionOrder { get; set; }
        public decimal ShippingOptionCost { get; set; }
        public decimal ShippingOptionPrice { get; set; }
        
        public bool ShippingOptionActive { get; set; }
        public bool ShippingOptionDefault { get; set; }
        public DateTime ShippingOptionDateCreated { get; set; }
    }
}
