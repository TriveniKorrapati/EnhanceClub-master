using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnhanceClub.Domain.Entities
{
   public class PaymentOption
    {
        public int PaymentOptionId { get; set; }

        public int PaymentOptionCurrencyFk { get; set; }

        public int PaymentOptionPaymentTypeFk { get; set; }

        public string PaymentOptionName { get; set; }

        public string PaymentOptionUsername { get; set; }

        public string PaymentOptionPassword { get; set; }

        public bool PaymentOptionActive { get; set; }

        public bool PaymentOptionCc { get; set; }

        public int? PaymentOptionOrder { get; set; }
    }
}
