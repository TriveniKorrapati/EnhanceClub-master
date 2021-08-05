using System.Collections.Generic;
using EnhanceClub.Domain.Entities;

namespace EnhanceClub.WebUI.Models
{
    // this class is used to pass order invoice data to View
    public class OrderInvoiceViewModel
    {
        public IEnumerable<CartDetail> CartDetailList { get; set; }
        public IEnumerable<OrderDetail> OrderDetailList { get; set; } 
    }
}