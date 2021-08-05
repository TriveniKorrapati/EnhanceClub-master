namespace EnhanceClub.Domain.Entities
{
    // used to deal with customer session using model binder
    public class LoggedCustomer
    {
        public int CustomerId { get; set; }
        public int CustomerStorefrontId { get; set; }
        public string CustomerFirstName { get; set; }
        public string CustomerLastName { get; set; }

        public string CustomerName
        {
            get { return CustomerFirstName + " " + CustomerLastName; }
        }
        public string CustomerEmail { get; set; }
        public decimal CustomerCredit { get; set; }
        public int ProvinceStateId { get; set; }

        public bool CustomerIsPartial { get; set; }
    
    }
}
