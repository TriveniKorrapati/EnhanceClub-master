namespace EnhanceClub.Domain.Entities
{
    // used for storefront and currency details
    public class StoreFrontInfo
    {
        public int StoreFrontId { get; set; }
        public string StoreFrontNamePk { get; set; }
        public string StoreFrontPhone { get; set; }
        public string StoreFrontFax { get; set; }
        public string StoreFrontCurrencyAbbreviation { get; set; }
        public string StoreFrontCurrencyName { get; set; }
        public decimal StoreFrontCurrencyExchangeRate { get; set; }
        public int StoreFrontCurrencyId { get; set; }
        public int NoChargePaymentOption { get; set; }
    }
}
