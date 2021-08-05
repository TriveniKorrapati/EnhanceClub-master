using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnhanceClub.Domain.Entities
{
    public class CustomerIdDocument
    {
        public int CustomerIdDocumentId { get; set; }
        public string CustomerIdDocumentFileName { get; set; }
        public string CustomerIdDocumentBackSideFileName { get; set; }
        public DateTime? CustomerIdDocumentExpiryDate { get; set; }
        public bool CustomerIdDocumentIsValid { get; set; }
        public int CustomerIdDocumentCustomerFk { get; set; }
        public int CustomerIdDocumentStoreFrontFk { get; set; }
        public string CustomerIdDocumentComment { get; set; }
        public bool CustomerIdDocumentActive { get; set; }
        public DateTime CustomerIdDocumentDateCreated { get; set; }
    }
}
