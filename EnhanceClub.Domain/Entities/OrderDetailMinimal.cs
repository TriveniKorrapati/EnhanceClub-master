using System;
using System.Collections.Generic;
using System.Linq;

namespace EnhanceClub.Domain.Entities
{
    // this class is used to display order confirm page

    public class OrderDetailMinimal
    {

        public int OrderInvoiceId { get; set; }
        public int OrderInvoiceCustomerFk{ get; set; }

        public DateTime OrderInvoiceDateCreated { get; set; }

        public decimal OrderInvoiceCreditAmount { get; set; }

        public decimal OrderInvoiceCouponAmount { get; set; }
        public string OrderInvoiceCouponCodePk { get; set; }

        public decimal OrderInvoiceShippingPrice { get; set; }

        public bool OrderinvoiceBPNotKnown { get; set; }

        public List<CartDetail> OrderCart { get; set; }

        //public decimal OrderTotal
        //{
        //    get
        //    {
        //        decimal total = OrderCart.Sum(x => x.CartLineTotal) + OrderInvoiceShippingPrice -
        //                        OrderInvoiceCouponAmount -
        //                        OrderInvoiceCreditAmount;
        //        return total;
        //    }
        //}
        //Order Total with Tax
        public decimal OrderTotal
        {
            get
            {
                decimal total = OrderCart.Sum(x => x.CartLineTotal) + OrderInvoiceShippingPrice -
                                OrderInvoiceCouponAmount -
                                OrderInvoiceCreditAmount
                                + OrderInvoiceGlobalTaxAmount // add global and provincial tax amount to total
                                + OrderInvoiceProvinceTaxAmount
                                + OrderInvoiceHarmonizedTaxAmount;

                return total;
            }
        }
        public decimal CartTotal
        {
            get
            {
                decimal total = OrderCart.Sum(x => x.CartLineTotal);
                return total;
            }
        }

        // check if cart has prescription products
        public bool CartHasRxProducts()
        {
            var cartRxCount = OrderCart.Count(p => p.ProductTypeFk == 2);

            if (cartRxCount == 0)
            {
                return false;

            }
            return true;
        }
        //Comment: Tax- start: properties to show provincial and global tax
        public decimal OrderInvoiceProvincialTaxPercentage { get; set; }

        public decimal OrderInvoiceGlobalTaxPercentage { get; set; }

        public decimal OrderInvoiceHarmonizedTaxPercentage { get; set; }

        public decimal OrderInvoiceProvinceTaxAmount { get; set; }

        public decimal OrderInvoiceGlobalTaxAmount { get; set; }

        public decimal OrderInvoiceHarmonizedTaxAmount { get; set; }

        public decimal OrderInvoiceCouponAppliedAmount { get; set; }

        public decimal OrderInvoiceCreditAppliedAmount { get; set; }

    }
}
