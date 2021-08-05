using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnhanceClub.Domain.Entities
{
    // used for customer sign up step 1
    public class CustomerSignUp
    {
        public int CustomerId { get; set; }
        public string CustomerEmail { get; set; }
        public string CustomerPassword { get; set; }
        public string CustomerLastIp { get; set; }
        public bool CustomerIsPartial { get; set; }      
      
        public string ConfirmPassword { get; set; }

        public int CustomerCountryFk { get; set; }
        public int CustomerProvinceFk { get; set; }
        public string CustomerAddress { get; set; }
        public string CustomerCity { get; set; }
        public string CustomerFirstName { get; set; }
        public string CustomerLastName { get; set; }

        public string CustomerZipCode { get; set; }
        public string CustomerPhone { get; set; }

        public bool CustomerFreeShipping { get; set; }
        public bool CustomerFreeShippingOneYear { get; set; }
        
        public bool CustomerTestAccount { get; set; }
        public int CustomerAffiliateId { get; set; }
        public DateTime CustomerLastLoginDate { get; set; }
        public DateTime CustomerLastModified { get; set; }
        public DateTime CustomerDateCreated { get; set; }
        public bool CustomerActive { get; set; }

        public bool Terms { get; set; }
        public bool TeleHealthTerms { get; set; }

        public List<Province> ProvinceList { get; set; }        

        // these fields do not accept null in db
        // this is used to initialize fields like date created, active and last login date
        public void SetOtherFields(bool isActive,
            DateTime dateCreated,
            DateTime lastModified,
            DateTime lastLogin,
            int affiliateFk,
            bool isTestAccount)
        {
            CustomerActive = isActive;
            CustomerDateCreated = dateCreated;
            CustomerLastModified = lastModified;
            CustomerLastLoginDate = lastLogin;
            CustomerAffiliateId = affiliateFk;
            CustomerTestAccount = isTestAccount;
        }

        // Start Cognito Sign In properties
        public int CognitoActionType { get; set; }

        public string SessionId { get; set; }

        public string CognitoUserSub { get; set; }

        public string CognitoSignupEmailVerificationCode { get; set; }

        public string CustomerPhoneCountryCode { get; set; }

        public string CustomerPhoneWithCountryCode
        {
            get
            {
                return CustomerPhoneCountryCode + CustomerPhone;
            }
        }

        public string CognitoSignupPhoneVerificationCode { get; set; }

        public string CustomerTypedEmail { get; set; }

        // End Cognito Sign In properties
    }
}
