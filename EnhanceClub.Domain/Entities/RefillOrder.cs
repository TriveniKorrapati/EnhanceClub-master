using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnhanceClub.Domain.Entities
{
   public class RefillOrder
    {
        public int CustomerFk { get; set; }

        public DateTime PrescriptionExpiryDate { get; set; }

        public int PrescriptionId { get; set; }

        //public int OrderId { get; set; }
        public int ProductSizeFk { get; set; }

        public int RefilQuantityAuthorised { get; set; }

        public int RefilQuantityUsed { get; set; }

        public int RefilQuantityLeft { get; set; }

        public string ProductStrength { get; set; }

        public int OrderInvoiceId { get; set; }

        public int NumberOfRefillsAllowed { get; set; }

        public int NumberOfRefillsAvailable { get; set; }

        public int SubstituteProductSizeFk { get; set; }

        public bool SubstitutionPermitted { get; set; }

        public int RefillShipInvoiceFk { get; set; }
    }
}
