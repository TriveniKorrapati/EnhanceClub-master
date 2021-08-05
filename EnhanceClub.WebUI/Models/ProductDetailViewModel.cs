using EnhanceClub.Domain.Entities;

namespace EnhanceClub.WebUI.Models
{
    public class ProductDetailViewModel
    {
        public ProductSearch ProductSearch { get; set; }

        public string CustomerEmail { get; set; }

        public string ProductDisplayClass { get; set; }

        public string ProductGenericImage { get; set; }

        public string ProductBrandImage { get; set; }

        public string ProductBgImage { get; set; }

        public string ProductBgVideo { get; set; }

        public string RecommendedDosage { get; set; }
    }
}