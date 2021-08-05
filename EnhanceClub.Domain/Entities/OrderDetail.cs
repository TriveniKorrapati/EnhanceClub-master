using System;
using System.Collections.Generic;
using System.Linq;

namespace EnhanceClub.Domain.Entities
{
    // This class is used for working with list of all orders placed by Customer
    // --users
    //  CustomerRepositorySql.GetCustomerAllOrders
    //  CustomerRepositorySql.GetCustomerOrderDetail

    public class OrderDetail
    {
        public int RefillCount                        { get; set; }
        public bool CartPharmacyReturn               { get; set; }
        public string PharmacyName                   { get; set; }

        public int StoreFrontId                      { get; set; }
        public string StoreFrontNamePk               { get; set; }
        public int StoreFrontCurrencyFk              { get; set; }
        public bool StoreFrontActive                 { get; set; }

        public int AffiliateId                       { get; set; }
        public string AffiliateName                  { get; set; }

        public string CustomerFirstName              { get; set; }
        public string CustomerLastName               { get; set; }
        public bool CustomerActive                   { get; set; }
        public bool CustomerFreeShipping             { get; set; }
        public bool CustomerFreeShippingOneYear      { get; set; }
        public DateTime CustomerFreeShippingStartDate { get; set; }

        public string UserAdminProcessingName        { get; set; }
        public string UserAdminProcessingFirstName   { get; set; }
        public string UserAdminProcessingLastName    { get; set; }

        public int OrderInvoiceId                    { get; set; }
        public bool OrderInvoiceActive               { get; set; }
        public DateTime OrderInvoiceDateCreated      { get; set; }

        public int OrderInvoiceCustomerFk            { get; set; }
        public string OrderInvoiceBillingFirstName   { get; set; }
        public string OrderInvoiceBillingLastName    { get; set; }
        public string OrderInvoiceBillingAddress     { get; set; }
        public string OrderInvoiceBillingCity        { get; set; }
        public string OrderInvoiceBillingProvince    { get; set; }
        public string OrderInvoiceBillingCountry     { get; set; }
        public string OrderInvoiceBillingPhone       { get; set; }
        public string OrderInvoiceBillingEmail       { get; set; }
        public string OrderInvoiceBillingZipCode     { get; set; }

        public string OrderInvoiceShippingFirstName  { get; set; }
        public string OrderInvoiceShippingLastName   { get; set; }
        public string OrderInvoiceShippingAddress    { get; set; }
        public string OrderInvoiceShippingCity       { get; set; }
        public string OrderInvoiceShippingProvince   { get; set; }
        public string OrderInvoiceShippingCountry    { get; set; }
        public string OrderInvoiceShippingPhone      { get; set; }
        public string OrderInvoiceShippingEmail      { get; set; }
        public string OrderInvoiceShippingZipCode    { get; set; }

        public decimal OrderInvoiceShippingPrice     { get; set; }
        public decimal OrderInvoiceCouponAmount      { get; set; }
        public decimal OrderInvoiceCreditAmount      { get; set; }

        public int ShippingInvoiceId                 { get; set; }
        public bool ShippingInvoiceShipped           { get; set; }
        public DateTime ShippingInvoiceShippingDate  { get; set; }
        public int ShippingInvoiceProblemFk          { get; set; }
        public bool ShippingInvoicePharmacyExported  { get; set; }
        public DateTime ShippingInvoiceDeletedDate   { get; set; }

        public List<CartDetail> OrderCart { get; set; }

        public int CartCountForOrder { get; set; }  // used to filter orders where cart has no record for an order
 
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

        //Comment: Tax- start: properties to show provincial and global tax
        public decimal OrderInvoiceProvincialTaxPercentage { get; set; }

        public decimal OrderInvoiceGlobalTaxPercentage { get; set; }

        public decimal OrderInvoiceHarmonizedTaxPercentage { get; set; }

        public decimal OrderInvoiceProvinceTaxAmount { get; set; }

        public decimal OrderInvoiceGlobalTaxAmount { get; set; }

        public decimal OrderInvoiceHarmonizedTaxAmount { get; set; }


        
    }
}
