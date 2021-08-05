using EnhanceClub.Domain.Helpers;
using System.Collections.Generic;

namespace EnhanceClub.Domain.Entities
{
    // this class is used to represent products added in cart
    public class ProductCart : Product
    {
        public int ProductId { get; set; }
        public string ProductSafeUrlName { get; set; }
        public string ProductUnitNamePk { get; set; }
        public int ProductTypeFk { get; set; }
        public string ProductStoreFrontName { get; set; }
        public bool ProductSizeGeneric { get; set; }
        public string ProductSizeStrength { get; set; }
        public decimal ProductSizeQuantity { get; set; }
        public decimal ProductSizeStoreFrontPrice { get; set; }
        public bool ProductSizeLimited { get; set; }
        public decimal ProductSizeLimitedQty { get; set; }

        public string ProductImagePath
        {
            get
            {
                var imagePath = SiteConfigurations.ProductImagePath;
                imagePath += SiteConfigurations.ProductGenericImagePrefix + ProductId + ".png";
                //if(ProductSizeGeneric)
                //{
                //    imagePath += SiteConfigurations.ProductGenericImagePrefix + ProductId + ".png";
                //}
                //else
                //{
                //    imagePath += SiteConfigurations.ProductBrandImagePrefix + ProductId + ".png";
                //}
                return imagePath; 
            }
        }

        // the properties will be used for auto fill products
        public bool Subscription { get; set; }
        public decimal CartItemQuantity { get; set; }

        public IEnumerable<VolumeDiscountProductSize> VolumeDiscountProductSize { get; set; }

        public int FrontendVisibleProductSizeFk { get; set; }

        public bool ProductSizeVisibleFrontEnd { get; set; }

        public string ProductDisplayName { get; set; }

        public decimal ProductSizeNonDiscountedPrice { get; set; }

        public decimal ProductSizeSavedPrice
        {
            get
            {
                return ProductSizeNonDiscountedPrice - ProductSizeStoreFrontPrice;
            }
        }

        public string CartDisplayColor {
            get
            {
                var displayColor = "yellow";
                switch(ProductDisplayName.ToLower())
                {                   
                    case "play":
                        displayColor = "yellow";
                        break;
                    case "rise":
                        displayColor = "orange";
                        break;
                    case "extra":
                        displayColor = "red";
                        break;
                    case "thick":
                        displayColor = "green";
                        break;
                    case "inject":
                        displayColor = "purple";
                        break;

                }
                return displayColor;
            }
        }

        public int ProductSizeOrder { get; set; }
    }
}
