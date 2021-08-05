using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


// Created by Rajiv S : 26 Mar 2020

namespace EnhanceClub.Domain.Entities
{
     public class Customer : CustomerMinimal
    {
        public int       CustomerAffiliateId         { get; set; }
        public string    CustomerStoreFrontName      { get; set; }
        
        public string    CustomerSex                 { get; set; }
        public DateTime? CustomerDob                 { get; set; }
        public string    CustomerType                { get; set; }
        public decimal   CustomerCreditLimit         { get; set; }
        public bool      CustomerPaymentPending      { get; set; }
        public string    CustomerReferralId           { get; set; }

        public string    CustomerPhoneExt           { get; set; }

        public string    CustomerSecondaryPhone      { get; set; }
        public string    CustomerSecondaryPhoneExt  { get; set; }

        public string    CustomerFax                 { get; set; }

        [Required(ErrorMessage = "Please Re enter password")]
        public string CustomerPasswordVerify { get; set; }

        public string    CustomerSecondaryEmail      { get; set; }
        public string    CustomerInvoiceEmail        { get; set; }

        //// moved to customer minimal class to use in order checkout step 2
        //public bool      CustomerFreeShipping        { get; set; }
        //public bool      CustomerFreeShippingOneYear { get; set; }
        //public DateTime  CustomerFreeShippingStartDate { get; set; }

        public bool      CustomerActive                { get; set; }
        public bool      CustomerTestAccount           { get; set; }
        public DateTime  CustomerDateCreated           { get; set; }
        public DateTime  CustomerLastModified          { get; set; }
        public DateTime CustomerLastLoginDate          { get; set; }

        // Moved to CustomerMinimal class to use in update customer profile and patient profile
        // public string CustomerLastIp { get; set; } 
 
        public List<PatientProfile> CustomerPatientProfiles   { get; set; }
        public List<CreditCard> CustomerCreditCards           { get; set; }
        public List<CustomerTransaction> CustomerTransactions { get; set; }
        public List<CustomerNote> CustomerNotes               { get; set; }

        public string CustomerPhoneArea { get; set; }
        public string CustomerPhone3Digit { get; set; }
        public string CustomerPhone4Digit { get; set; }

        public string CustomerEveningPhoneArea { get; set; }
        public string CustomerEveningPhone3Digit { get; set; }
        public string CustomerEveningPhone4Digit { get; set; }

        public int CustomerUserAdminFk { get; set; }
        public string CustomerCellPhoneArea { get; set; }
        public string CustomerCellPhone3Digit { get; set; }
        public string CustomerCellPhone4Digit { get; set; }

        // is partial indicates that sign up step 2 is not completed yet
        public bool IsPartial { get; set; }

        // Remove Special Characters from selected fields
        public void StripSpecialChar()
         {
            // Remove special characters from first name
            CustomerFirstName = CustomerFirstName.Replace("'", "")
                 .Replace(">", "")
                 .Replace("<", "")
                 .Replace(":", "")
                 .Replace(";", "")
                 .Replace("=", "")
                 .Replace("\\", "")
                 .Replace("//", "")
                 .Replace("(", "")
                 .Replace(")", "")
                 .Replace("''", "")
                 .Replace(@"""", "" );

             // Remove special characters from Last name
           CustomerLastName =  CustomerLastName.Replace("'", "")
                 .Replace(">", "")
                 .Replace("<", "")
                 .Replace(":", "")
                  .Replace(";", "")
                 .Replace("=", "")
                 .Replace("\\", "")
                 .Replace("//", "")
                 .Replace("(", "")
                 .Replace(")", "")
                 .Replace("''", "")
                 .Replace(@"""", "" );

             // Remove special characters from Last name
             CustomerEmail = CustomerEmail.Replace("'", "")
                 .Replace(">", "")
                 .Replace("<", "")
                 .Replace(":", "")
                  .Replace(";","")
                 .Replace("=", "")
                 .Replace("\\","")
                 .Replace("//","")
                 .Replace("(", "")
                 .Replace(")", "")
                 .Replace("''", "")
                 .Replace(@"""",""); 
          
         }

         // this is used to initialize fields like date created, active and last login date
         public void SetOtherFields(bool isActive, DateTime dateCreated, DateTime lastModified, DateTime lastLogin, int affiliateFk, bool isTestAccount)
         {
             CustomerActive = isActive;
             CustomerDateCreated = dateCreated;
             CustomerLastModified = lastModified;
             CustomerLastLoginDate = lastLogin;
             CustomerAffiliateId = affiliateFk;
             CustomerTestAccount = isTestAccount;
         }
    }

}
