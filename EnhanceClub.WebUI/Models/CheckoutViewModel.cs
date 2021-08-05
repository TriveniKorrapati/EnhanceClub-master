using System.Collections.Generic;
using EnhanceClub.Domain.Entities;
using System.Web.Mvc;
using System;

namespace EnhanceClub.WebUI.Models
{
    public class CheckoutViewModel
    {
        public Cart Cart { get; set; }
        public CustomerMinimal CustomerInfo { get; set; }
        public ShippingDetails ShipDetails { get; set; }
        public CreditCard CreditCard { get; set; }
        public IEnumerable<PatientProfile> ProfileList { get; set; }
        public Country CustomerCountry { get; set; }
        public IEnumerable<ShippingOption> OrderShipOptions { get; set; }
        public List<Province> ProvinceList { get; set; }
      
        public string StorefrontContact { get; set; }
        public decimal CreditAvailable { get; set; }
        public decimal StoreFrontExchangeRate { get; set; }
        public int NoChargePaymentOption { get; set; }

        public List<SelectListItem> MonthList
        {
            get
            {
                List<SelectListItem> monthList = new List<SelectListItem>
                {
                    new SelectListItem() {Text = @"January", Value = "1", Selected = false},
                    new SelectListItem() {Text = @"February", Value = "2", Selected = false},
                    new SelectListItem() {Text = @"March", Value = "3", Selected = false},
                    
                    new SelectListItem() {Text = @"April", Value = "4", Selected = false},
                    new SelectListItem() {Text = @"May", Value = "5", Selected = false},
                    new SelectListItem() {Text = @"June", Value = "6", Selected = false},

                    new SelectListItem() {Text = @"July", Value = "7", Selected = false},
                    new SelectListItem() {Text = @"August", Value = "8", Selected = false},
                    new SelectListItem() {Text = @"September", Value = "9", Selected = false},

                    new SelectListItem() {Text = @"October", Value = "10", Selected = false},
                    new SelectListItem() {Text = @"November", Value = "11", Selected = false},
                    new SelectListItem() {Text = @"December", Value = "12", Selected = false},
                };

                return monthList;
            }
        }

        public List<SelectListItem> YearList
        {
            get
            {
                List<SelectListItem> yearList = new List<SelectListItem>();

                for (int i = DateTime.Now.Year; i < DateTime.Now.Year + 10; i++)
                {

                    yearList.Add(new SelectListItem() { Text = @i.ToString(), Value = @i.ToString(), Selected = false });

                };

                return yearList;
            }
        }
        public List<SelectListItem> CardTypeList
        {
            get
            {
                List<SelectListItem> cardTypeList = new List<SelectListItem>
                {
                   // new SelectListItem() {Text = @"Select Credit Card Type", Selected = true},
                    new SelectListItem() {Text = @"Visa", Value = "visa", Selected = true},
                    new SelectListItem() {Text = @"MasterCard", Value = "mc", Selected = false},
                    new SelectListItem() {Text = @"Discover", Value = "discover", Selected = false},

                };

                return cardTypeList;
            }
        }
        public string CardExpireYear
        {
            get
            {
                if (DateTime.Now.Month.ToString() == "12")
                {
                    return DateTime.Now.AddYears(1).Year.ToString();
                }

                return DateTime.Now.Year.ToString();
            }
        }
        public string CardExpireMonth
        {
            get
            {               
               return DateTime.Now.AddMonths(1).Month.ToString();
            }
        }

        public List<CustomerShippingAddress> CustomerShippingAddresses { get; set; }

        public int SelectedShipAddressFk { get; set; }

        public bool IsRefill { get; set; }
        public int PreviousOrderCount { get; set; }
    }
}