using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnhanceClub.Domain.Entities
{
    public class TopFeaturedProduct : FeaturedProduct
    {
        public ProductSearch ProductSearch { get; set; }

        public int ProductStoreFrontDisplayOrder { get; set; }
    }
}
