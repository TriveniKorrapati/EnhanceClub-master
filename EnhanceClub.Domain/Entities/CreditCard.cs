using System;

// Created by Rajiv S : 26 Mar 2020

namespace EnhanceClub.Domain.Entities
{
    public class CreditCard
    {
        public string CardNumber { get; set; }
        public string CardHolderName { get; set; }
        public DateTime ExpiryDate { get; set; }
        public string SecureCode { get; set; }
    }
}
