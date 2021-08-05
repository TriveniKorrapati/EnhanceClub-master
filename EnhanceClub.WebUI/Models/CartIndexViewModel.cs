using System.Collections.Generic;
using EnhanceClub.Domain.Entities;

namespace EnhanceClub.WebUI.Models
{
    public class CartIndexViewModel
    {
        public Cart Cart { get; set; }
        public string ReturnUrl { get; set; }
        public CustomerMinimal CustomerInfo { get; set; }
        public IEnumerable<PatientProfile> ProfileList { get; set; }
        public int FreeShipShippingOption { get; set; }
        public int FlatRateShippingOption { get; set; }
        public decimal FlatRateShippingPrice { get; set; }
        public decimal FreeShippingThreshHold { get; set; } // determines when code in checkout step 2 (checkout.cshtml) switches to free shipping

        public ShippingDetails ShipDetails { get; set; }

        public FeaturedProduct RelatedProducts { get; set; }
    }
}