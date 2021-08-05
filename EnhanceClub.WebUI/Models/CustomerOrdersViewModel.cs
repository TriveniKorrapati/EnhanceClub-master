using System.Collections.Generic;
using EnhanceClub.Domain.Entities;

namespace EnhanceClub.WebUI.Models
{
    // this view model is used to pass Customer Rx products Ordered and all Order List
    public class CustomerOrdersViewModel
    {
        public IEnumerable<ProductsOrdered> ProductsOrderedList { get; set; }
        public IEnumerable<OrderDetail> CustomerOrdersList { get; set; }
        public LoggedCustomer LoggedCustomer { get; set; }
    }
}