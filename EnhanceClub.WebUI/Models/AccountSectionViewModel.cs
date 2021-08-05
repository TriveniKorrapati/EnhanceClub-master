using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using EnhanceClub.Domain.Entities;

namespace EnhanceClub.WebUI.Models
{
    public class AccountSectionViewModel
    {
        public LoggedCustomer LoggedCustomer { get; set; }

        public List<OrderStatus> AllOrders { get; set; }
       
        public List<ProductsOrdered> ProductsOrdered { get; set; }

        public int SortOrder { get; set; }

        public PagingInfo PagingInfo { get; set; }

        public bool UploadDocument { get; set; }

        public bool fromLogin { get; set; }

        public bool CustomerIsPartial { get; set; }
        
    }
}