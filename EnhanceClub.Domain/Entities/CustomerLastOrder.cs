using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnhanceClub.Domain.Entities
{
    public class CustomerLastOrder
    {
        public int OrderInvoiceId { get; set; }
        public DateTime OrderInvoiceDateCreated { get; set; }
        public decimal OrderCartTotal { get; set; }
        public int OrderPlacedMinutesAgo { get; set; }
    }
}
