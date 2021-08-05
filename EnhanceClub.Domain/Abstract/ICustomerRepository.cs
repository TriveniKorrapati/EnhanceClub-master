
// Created by Rajiv S : 26 Mar 2020

using System;
using System.Collections.Generic;
using System.Data;
using EnhanceClub.Domain.AwsEntities;
using EnhanceClub.Domain.Entities;

namespace EnhanceClub.Domain.Abstract
{
    public interface ICustomerRepository
    {
        IEnumerable<Customer> Customers{ get;}
        DataSet ValidateLogin(string email, string password, int storeFrontId);

        DataSet ValidateEmailLogin(string email, int storeFrontId);

        decimal GetCustomerCredit(int customerId);
        CustomerCredit GetCustomerCreditWithOrderCount(int customerId);
        // this method has different where clause to get data for referring customer
        CustomerMinimal GetReferredCustomerInfo(int referingCustomerId,int customerId);

        CustomerMinimal GetCustomerInfo(int customerId);
        List<Province> GetProvinceListByCountry(int countryId);
        int UpdateCustomerInfo(CustomerMinimal customerInfo);
        List<CreditTransaction> GetCustomerCreditTransactions(int customerId);
        List<ProductsOrdered> GetCustomerProductsOrdered(int customerId, int productTypeFk, int customerStorefrontId);
        List<OrderDetail> GetCustomerAllOrders(int customerId, int customerStorefrontId);
        List<CartDetail> GetCustomerOrderCart(int customerId, int customerStorefrontId, int orderId);
        List<OrderDetail> GetCustomerOrderDetail(int customerId, int customerStorefrontId, int orderId);

        bool CustomerEmailRegistered(string customerEmail, int affiliateId);
        int GetCustomerFromEmailOrId(string customerEmail, int customerId, int affiliateId);
        int AddCustomer(Customer customerInfo);
        CustomerLoginInfo GetCustomerLoginInfo(string customerEmail, int affiliateId);
        IEnumerable<PatientProfile> GetPatientProfilesOfCustomer(int customerId, int patientProfileId, int storeFrontId);
        List<PatientMedication> GetPatientMedicationByProfile( int patientProfileId, int medicationId, int customerId, int storefrontId);
        bool EmailFoundForOtherCustomer(string customerEmail, int affiliateStoreFrontFk, int customerId);
        int UpdateProfile(PatientProfile patient, int storeFrontId, int patientProfileId, int customerId, string customerName, string customerEmail, string customerLastIp, DateTime lastModified);
        int UpdateMedication(int patientProfileId, int patientMedicationId, string patientMedicationDrugName, string patientMedicationIllness, DateTime patientProfileLastModified);
        int AddMedication(int patientProfileId, string patientMedicationDrugName, string patientMedicationIllness, DateTime createdOn);
        
        int AddPatientProfile(PatientProfile patientProfile, int customerId, string customerName, string customerEmail, string customerLastIp, DateTime dateCreated);
        
        IEnumerable<Country> GetCountry(int countryId);
        
        int GetReferredBy(int customerId);
        int GetCustomerOrderCount(int customerId);
        bool IsCreditAlreadyIssued(int customerId);

        int AddReferCreditToBoth(int referringCustomerId, decimal refererCredit,string referringComment,  int customerId, decimal referredCredit,string referedComment, int referCreditTransactionType, DateTime dateCreated, int orderInvoiceId);

        Province GetProvinceById(int shippingProvinceId);
        Country GetCountryById(int shippingCountryId);
        bool EmailAlreadyExits(string customerEmail, int affiliateId);
        void DelProfile(int patientProfileId, int customerId);

        // add customer email to PromoEmails table
        int AddPromoEmail(int emailTypeFk,string customerEmail,string clientIp,string storefrontId,bool isActive,DateTime dateAdded);

        // unsubscribe customer from news letter 
        int UnsubscribeNewLetter(int promoIdDecoded);

        // this method adds customer sign up first step
        int AddCustomerPartial(CustomerSignUp customerInfo);

        // this method updates step 2 info
        int UpdateCustomerPartial(CustomerSignUp customerInfo);

        // get user password recovery details
        UserPasswordRecovery GetUserPasswordRecoveryDetails(string token);

        // add user password recovery details
        int AddUserPasswordRecoveryDetails(int customerFk, string encryptedToken, string decryptedToken,
                                            DateTime linkExpiry, int resetStatus, DateTime dateCreated);

        // update customer password
        int UpdateCustomerPassword(int customerFk, string password, DateTime dateModified);

        // update password recovery reset status
        int UserPasswordRecoveryResetStatus(int userPasswordRecoveryResetStatus, int customerFk, int resetStatus, DateTime dateModified);

        List<OrderStatus> GetCustomerOpenOrders(int customerId, int storefrontId);

        List<OrderStatus> GetCustomerClosedOrders(int customerId, int storefrontId);

        CustomerMinimal GetCustomerInfoPartial(int customerId);

        int UpdateCustomerIsPartialFlag(int customerId, bool isPartial);

        // update general enquiry submitted from get in touch page
        int AddGeneralEnquiry(int storefrontFk, string customerName, string customerEmail, string customerPhone, string queryPosted, DateTime dateCreated, string category, string zendeskTicketId);

         //Upload Id document details  for customer
        int AddCustomerIdDocumentDetail(int customerFk, int storeFrontFk, string fileName, bool active, DateTime dateCreated, string existingBackImage);

        // upload prescription
        int AddCustomerPrescription(int customerFk, int orderInvoiceFk, int storeFrontFk, string fileName, bool active, DateTime dateCreated);

        // get Customer Document Id List based on Customer id
        List<CustomerIdDocument> GetCustomerIdDocumentByCustomerId(int customerId);

        //To make all other  uploaded id inactive
        bool UpdateCustomerIdInActive(int documentId, int customerFk, int storeFrontFk);
        //To make all other  uploaded id inactive
        int UpdateCustomerIdBackFileName(int customerIdDocumentFk, string backSideFileName, int customerFk, int storeFrontFk, DateTime dateModified);

        QuestionnaireCategoryResponse GetQuestionnairCategoryResponseByCategory(int customerFk, int productCategoryFk, int orderInvoiceFk);

        //To make all other  customer prescription inactive
        bool UpdatePrescriptionInActive(int documentId, int customerFk,int orderInvoiceId, int storeFrontFk);

        int AddLogCosultationConsent( bool? updatedvalue, int patientProfileFk, int storefronFk, string actionType, DateTime dateCreated);

        List<CustomerSubscription> GetCustomerAllSubscription(int customerFk);

        int UnSubscribeAutoRefill(int subscriptionId);

        int AddLogCustomerSubscription(int customerFk, int SubscriptionFk, bool updatedValue, string actionType, DateTime dateCreated);
        List<CustomerLastOrder> GetCustomerLastOrder(int storeFrontFk, int customerId, DateTime currentTimeStamp, int orderTimeSpan);

        int UpdateOrderConsultationFlag(int orderFk, bool? orderConsultationFlag);

        // method to add log custmer id document
        int AddLogCustomerIdDocument(int customerDocumentId, int customerFk, int storefronFk, 
                                        string actionType, DateTime dateCreated,
                                        bool mobileDevice, string module, string logMessage);

        int UpdateCartItemPatientProfile(int orderFk, int patientProfileFk);

        int AddLogCongnitoSignUpResponse(CognitoSignupResponse cognitoSigupResponse, int storefrontFk, int customerFk, string ipAddress, DateTime dateCreated, string response);

        int AddLogCognitoSignUpVerification(CognitoConfirmSignupResponse cognitoConfirmSignupResponse, int storefrontFk, int customerFk,
                                            DateTime verifiedDate, string ipAddress, string response, int actionType, bool userRequested);

        int AddLogCognitoSignupAuthSession(SrpAuthResponse srpAuthResponse, int storefrontFk, int customerFk,
                                           DateTime timeStamp, string ipAddress, string response);

        int UpdateCustomerCognitoUserId(int customerId, int cognitoUserId);

        int AddLogCognitoAuthFlowMfaResponse(AuthFlowMfaResponse authFlowMfaResponse, int storefrontFk, int customerFk,
                                           string ipAddress, string response, DateTime dateCreated, int actionType);

        int AddLogCognitoUserAttributesResponse(UserAttributeResponse userAttributeResponse, CustomerSignUp customerInfo, int storefrontFk, int customerFk,
                                          string ipAddress, string response, DateTime dateCreated, int actionType);

        int AddLogCognitoResetPassword(CognitoSignupResponse cognitoResponse, int storefrontFk, string customerEmail, string resetCode,
                                         string ipAddress, string response, DateTime dateCreated, int actionType);

        int AddLogCognitoSendEmailVerification(CognitoSignupResponse cognitoConfirmSignupResponse, int storefrontFk, string customerEmail,
                                          string ipAddress, string response, DateTime dateCreated, int actionType, bool userRequested);
        //get all consultationhours availabe
        List<ConsultationHours> GetConsultationHours(int customerfk);

        //add patient prefered time of consultation
        int AddPatientConsultationHours(int consultationHoursFk, int customerFk, bool active, DateTime startDate, DateTime? endDate);

        List<PatientConsultationHours> GetPatientPreferedConsultationHour(int patientConsultationDetailsId, int consultationHoursFk, int customerFk);

        int InActivatePatientConsultationHours(int customerId, string patientConsultationHoursIds, DateTime endDate);

        IEnumerable<SocialHistory> GetSocialHistoryList();

        int UpdateCustomerSignUpEmailVerifiedFlag(int customerId, bool emailVerified);
        DataSet GetDeveloperLogin(string email, int storeFrontId);

        List<CustomerShippingAddress> GetCustomerShippingAddress(int customerFk, int shippingAddressFk);

        int AddCustomerShippingAddress(int customerFk, string customerFirstName, string customerLastName,
                                            string customerPhone,
                                            string shippingAddress, string shippingCity,
                                           int countryFk, int provinceFk,
                                           string zipCode, bool addressActive, bool defaultAddress, DateTime dateCreated);
        int UpdateShippingAddress(int shippingAdressFk, string firstName, string lastName,
                string address, string city, string zipcode, int provinceFk, int countryFk);

        int UpdateCustomerShippingAddressDefaultFlag(int customerId, int customerShippingAddressId, bool isDefault);

        int UpdateCustomerShippingAddressActiveFlag(int customerId, int customerShippingAddressId, bool isActive);

        int UpdateCustomerCredentials(int customerId, string email, string phone);

        int AddLogCustomerShippingAddress(List<LogCustomerShippingAddress> logCustomerShippingAddress);

        CartDetail GetCustomerOrderCartByShipInvoice(int customerId, int customerStorefrontId, int orderId);
    }
}
