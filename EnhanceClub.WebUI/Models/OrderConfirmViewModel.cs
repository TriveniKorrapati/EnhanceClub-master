using System.Collections.Generic;
using EnhanceClub.Domain.Entities;

namespace EnhanceClub.WebUI.Models
{
    public class OrderConfirmViewModel
    {
        public int OrderId  {get; set;}

        public decimal OrderTotal { get; set; }

        public decimal ShippingPrice { get; set; }

        public string StoreFrontFax { get; set; }

        public CustomerMinimal CustomerInfo { get; set; }

        public IEnumerable<CartDetail> ProductList { get; set; }

        public bool HasRxProducts { get; set; }

        public decimal CartTotal { get; set; }

        public double OrderTimeElapsed { get; set; }
        public string QuestionnaireCatId { get; set; }
        public OrderDetailMinimal OrderInfo { get; set; }
        public decimal OrderTax { get; set; }

    }
}