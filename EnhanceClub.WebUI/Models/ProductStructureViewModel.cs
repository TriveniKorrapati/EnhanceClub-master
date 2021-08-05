using System.Collections.Generic;
using EnhanceClub.Domain.Entities;
using EnhanceClub.WebUI.Infrastructure.Utility;

namespace EnhanceClub.WebUI.Models
{
    public class ProductStructureViewModel
    {
        public string PageUrl { get; set; }
        public string ProductName { get; set; }
        public string ProductDescription { get; set; }
        public string ProductId { get; set; }
        public string MainImageUrl { get; set; }
        public decimal LowPrice { get; set; }
        public decimal HighPrice { get; set; }
        public int ProductType { get; set; }
        public bool PetProduct { get; set; }

        public int BrandProductCount { get; set; }
        public int GenericProductCount { get; set; }

        public IEnumerable<FeaturedProduct> RelatedProducts { get; set; }

        public string BrandGeneric
        {
            get
            {
                string brandGeneric = "";
                if (BrandProductCount > 0 && GenericProductCount > 0)
                {
                    brandGeneric = "Brand and Generic";
                }
                else
                {
                    if (BrandProductCount > 0 && GenericProductCount == 0)
                    {
                        brandGeneric = "Brand";
                    }
                    if (BrandProductCount == 0 && GenericProductCount > 0)
                    {
                        brandGeneric = "Generic";
                    }
                }
                return brandGeneric;
            }
        }

        public string ProductCategory
        {
            get
            {
                string category = "";

                if (ProductType == 1)
                {
                    category = "OTC";
                }
                if (ProductType == 2)
                {
                    category = "RX";
                }

                return category;
            }
        }

        public string MetaDescription { get; set; }

        public string ProductImagePath {
            get
            {
                var imagePath = SiteConfigurationsWc.ProductImagePath;
                imagePath += SiteConfigurationsWc.ProductGenericImagePrefix + ProductId + ".png";
                return imagePath;
            }
        }
    }
}