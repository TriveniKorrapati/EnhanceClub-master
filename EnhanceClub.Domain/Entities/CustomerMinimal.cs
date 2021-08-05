using System;
using System.ComponentModel.DataAnnotations;

namespace EnhanceClub.Domain.Entities
{
    public class CustomerMinimal
    {        
        public int CustomerId { get; set; }

        [Required(ErrorMessage = "Please Enter the First Name")]
        public string CustomerFirstName { get; set; }

        [Required(ErrorMessage = "Please Enter the Last Name")]
        public string CustomerLastName { get; set; }

        [Required(ErrorMessage = "Please Enter Valid Email")]
     
        public string CustomerEmail { get; set; }

        //[Required(ErrorMessage = "Please Enter password")]
        public string CustomerPassword { get; set; }

        [Required(ErrorMessage = "Please Enter Phone")]
        public string CustomerPhone { get; set; }

        public string CustomerEveningPhone { get; set; }
        
        [Required(ErrorMessage = "Please Enter a valid Address")]
        public string CustomerAddress { get; set; }


        public int CustomerCountryId { get; set; }
        public int CustomerProvinceId { get; set; }
        public string CustomerCity { get; set; }

        [Required(ErrorMessage = "Please Enter a Postal Code")]
        public string CustomerZipCode { get; set; }

        // Province name and country name are used for billing address display in checkout step 3
        public string BillingCountryName { get; set; }
        public string BillingProvinceName { get; set; }

        // moved here from customer class to use in order checkout step 2
        public bool CustomerFreeShipping { get; set; }
        public bool CustomerFreeShippingOneYear { get; set; }
        public DateTime CustomerFreeShippingStartDate { get; set; }

        // this field is used for referral payment on submit order OrderRepository.Sql
        public int CustomerAffiliateFk { get; set; }
        public string CustomerCellPhone { get; set; }

        //Moved from Customer class to use in update customer profile and patient profile
        public string CustomerLastIp { get; set; }

        public string CustomerCountryName { get; set; }

        // property to check if customer is registered partially
        public bool CustomerIsPartial { get; set; }

        public string CustomerName
        {
            get { return CustomerFirstName + " " + CustomerLastName; }
        }

        public string CustomerTypedEmail { get; set; }
        public string CustomerProvinceCode { get; set; }
        public string Sha1CustomerEmail { get; set; }

    }
}