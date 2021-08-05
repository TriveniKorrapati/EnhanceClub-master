namespace EnhanceClub.Domain.Entities
{
    // this class is used by Model Binder to provide Affiliate Info to Controller methods
    public class AffiliateInfo

    {
        public int AffiliateId { get; set; }
        public int AffiliateStoreFrontFk { get; set; }
        public string StoreFrontFax { get; set; }
        public string StorefrontContact { get; set; }
        
        public string StoreFrontEnquiry { get; set; }
        public string StoreFrontWebsiteProblems { get; set; }
        public string StoreFrontMarketing { get; set; }
        public string StoreFrontSales { get; set; }

        public int FreeShipShippingOption { get; set; }
        public int DefaultPaymentOptionFk { get; set; }
        public string DefaultPaymentOptionName { get; set; }

        public int FlatRateShippingOption { get; set; }
        public decimal FlatRateShippingPrice { get; set; }
        public decimal FreeShippingThreshHold { get; set; }  // determines when code in checkout step three switches to free shipping
        public int NoChargePaymentOption { get; set; }

        public decimal ReferrerCredit { get; set; }
        public decimal ReferredCredit { get; set; }

        public string StorefrontName { get; set; }
        public string StorefrontUrl { get; set; }
        public string StorefrontLogo { get; set; }
        public string StorefrontEmailHeader { get; set; }
        public string PaymentTransactionType { get; set; }
    }
}
