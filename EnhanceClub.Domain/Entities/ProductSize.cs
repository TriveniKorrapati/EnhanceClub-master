using System.Collections.Generic;
using System.Text;

namespace EnhanceClub.Domain.Entities
{
    // used to deal with product size that belong to product in a storefront

    public class ProductSize
    {
        public int ProductFk { get; set; }

        public int ProductSizeId { get; set; }
        public int ProductSizeProductUnitFk { get; set; }
        public string ProductSizeStrength { get; set; }
        public decimal ProductSizeQuantity { get; set; }
        public int ProductSizeMaxOrder { get; set; }
        public decimal ProductSizePrice { get; set; }
        public bool ProductSizeBlockCndIp { get; set; }
        public bool ProductSizeFrontEndNa { get; set; }
        public bool ProductSizeLimited { get; set; }
        public decimal ProductSizeLimitedQty { get; set; }

        public int ProductSizeOrder { get; set; }
        public bool ProductSizeGeneric { get; set; }

        public string ProductSizeRecommendedDosage { get; set; }

        public string ProductUnitNamePk { get; set; }
    
        public bool ProductSizeStoreFrontFrontEndNa { get; set; }
        public bool ProductSizeStoreFrontNotAvailable { get; set; }
        public bool ProductSizeStoreFrontExcludeWholeSale { get; set; }
        public string ProductSizeStoreFrontPreferText { get; set; }
        public bool ProductSizeStoreFrontPriceDefaultWholeSaleActive { get; set; }
        public decimal ProductSizeStoreFrontPriceDefaultWholeSale { get; set; }
        public string ProductSizeStoreFrontRecommendedDosage { get; set; }

        // used by product size drop down on search product pages
        public decimal StoreFrontExchangeRate { get; set; }

        // this value is used to display in drop down for product size text
        public string DisplayText
        {
            get
            {
                // for select a size top option
                if (ProductSizeId == 0)
                {
                    return "Select A Size...";
                }

                StringBuilder dispText = new StringBuilder();

                if (ProductSizeGeneric)
                {

                    dispText.Append("Generic");
                    dispText.Append(" ");
                    dispText.Append(ProductSizeStrength);
                    dispText.Append(" ");
                    dispText.Append(ProductSizeQuantity.ToString("0.##"));                    

                }

                else
                {
                    dispText.Append("Brand");
                    dispText.Append(" ");
                    dispText.Append(ProductSizeStrength);
                    dispText.Append(" ");
                    dispText.Append(ProductSizeQuantity.ToString("0.##"));                   

                }
                
                if (!string.IsNullOrEmpty(ProductUnitNamePk))
                {
                    dispText.Append(" ");
                    dispText.Append(ProductUnitNamePk);
                }

                if (!string.IsNullOrEmpty(ProductSizeStoreFrontPreferText))
                {
                    dispText.Append(" ");
                    dispText.Append(ProductSizeStoreFrontPreferText);
                    
                }

                dispText.Append(" - ");
                dispText.Append(ProductSizePrice.ToString("c"));

                if (ProductSizeMaxOrder > 0)
                {
                    //dispText.Append("  <b> Max( ");
                    //dispText.Append("<span style = \"color=\"116BCF\" \" ");
                    //dispText.Append(ProductSizeMaxOrder);
                    //dispText.Append("  per order )</b> </span>");

                    dispText.Append("  Max( ");
                    dispText.Append(ProductSizeMaxOrder);
                    dispText.Append("  per order )");
                }

                //if (ProductSizePrice != 0 && StoreFrontExchangeRate != 0)
                //{
                //    dispText.Append(" - ");
                //    dispText.Append((ProductSizePrice*StoreFrontExchangeRate).ToString("c"));
                //}
                if (ProductSizeLimited)
                {

                    if (ProductSizeLimitedQty <= 0)
                    {
                        dispText.Append(" - SOLD OUT");
                        
                    }
                    else
                    {
                        dispText.Append(" - ");
                        dispText.Append(ProductSizeLimitedQty);
                        dispText.Append(" left");
                    }
                }

                return dispText.ToString(); 
             
   
            }

            //set { _displayText = value; }
        }

        // this value is used for value field of drop down
        public int DisplayValue
        {
            get

            {
                // for select a size top option
                if (ProductSizeId == 0)
                {
                    return 0;
                }

                if (ProductSizeStoreFrontNotAvailable || (ProductSizeLimited && (ProductSizeLimitedQty <= 0)))
                {
                    return 0;
                }
                else
                {
                    return ProductSizeId;
                }
                
            }

            //--
        }

        public string DisplayTextHome
        {
            get
            {
                return ProductSizeStrength;
            }
        }

        public string DisplayTextNoBrand
        {
            get
            {
                return DisplayText.Replace("Brand", "").Replace("Generic", "");
            }
        }

        public string DisplayQuantity
        {
            get
            {
                StringBuilder dispText = new StringBuilder();
                dispText.Append(ProductSizeQuantity.ToString("0.##"));
                if (!string.IsNullOrEmpty(ProductUnitNamePk))
                {
                    dispText.Append(" ");
                    dispText.Append(ProductUnitNamePk);
                }
                return dispText.ToString();

            }
        }

        public List<string> ListQuantity
        {
            get
            {

                return null;
            }
        }
        public string ProductCountryOfOrigin { get; set; }
        public string ProductCountryOriginCode { get; set; }

        public bool ProductSizeVisibleFrontEnd { get; set; }
    }
}
