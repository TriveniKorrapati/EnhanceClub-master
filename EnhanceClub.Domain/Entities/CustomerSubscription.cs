using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnhanceClub.Domain.Entities
{
    public class CustomerSubscription 
    {
        public int CustomerSubscriptionId { get; set; }

        public DateTime SubscriptionSignUpDate { get; set; }

        public int SubscriptionProductSizeFk { get; set; }

        public decimal SubscriptionQuantity { get; set; }

        public string ProductName { get; set; }

        public bool ProductSizeGeneric { get; set; }

        public string ProductSizeStrength { get; set; }

        public string ProductSizeUnit { get; set; }

        public decimal ProducSizeQuantity { get; set; }

        public int SubscriptionDaysToExpire
        {
            get
            {
              return  (SubscriptionSignUpDate - DateTime.Now).Days;
            }
        }
    }
}
