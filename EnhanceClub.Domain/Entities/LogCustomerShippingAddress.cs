using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnhanceClub.Domain.Entities
{
    public class LogCustomerShippingAddress
    {
        public int CustomerFk { get; set; }

        public int CustomerShippingAddressFk { get; set; }

        public int LogUpdatedFieldFk { get; set; }

        public string LogUpdatedFieldName { get; set; }

        public string LogUpdatedFieldOriginalValue { get; set; }

        public string LogUpdatedFieldModifiedValue { get; set; }

        public string LogActionType { get; set; }

        public DateTime LogDateCreated { get; set; }
    }
}
