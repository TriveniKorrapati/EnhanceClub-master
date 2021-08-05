namespace EnhanceClub.Domain.Entities
{
    public class CustomerCredit

    // used to pass customer credit and order count in checkout step 3
    {
        public decimal CreditAmount { get; set; }
        public int OrderCount { get; set; }
    }
}
