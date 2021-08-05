using System;

namespace EnhanceClub.Domain.Entities
{
    public class CreditTransaction
    {
        public DateTime CreditTransactionDate { get; set; }
        public int CreditTransactionOrder { get; set; }
        public string CreditTransactionComments { get; set; }
        public decimal CreditTransactionAmount { get; set; }
        public int CreditTransactionUserAdmin { get; set; }
    
    }
}