using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnhanceClub.Domain.Entities
{
    public class VolumeDiscountProductSize
    {
        public int ProductSizeFk { get; set; }

        public int RelatedProductSizeFk { get; set; }

        public string DiscountMessage { get; set; }

        public string DiscountImageName { get; set; }

        public decimal ProductSizePrice { get; set; }

        public decimal ProductSizeQuantity { get; set; }

        public string ProductSizeUnit { get; set; }

        public decimal ProductSizeNonDiscountedPrice { get; set; }

        public decimal FrontendVisibleItemPrice { get; set; }

        public decimal FrontendVisibleItemQuantity { get; set; }

        public string DisplayUnit
        {
            get
            {
                return ProductSizeQuantity.ToString("##") + " " + ProductSizeUnit;
            }
        }

        public int ProductSizeOrder { get; set; }
    }
}
