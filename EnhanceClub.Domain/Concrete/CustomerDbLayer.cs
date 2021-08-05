using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using EnhanceClub.Domain.AwsEntities;
using EnhanceClub.Domain.AwsHelper;
using EnhanceClub.Domain.Entities;
using EnhanceClub.Domain.Helpers;
using EnhanceClub.Domain.LogEntities;


// Created by Rajiv S : 26 Mar 2020

//-- This implementation provides database persistence for CustomerRepositorySql--//

namespace EnhanceClub.Domain.Concrete
{
    public class CustomerDbLayer
    {
        // readonly string _sCon = ConfigurationManager.ConnectionStrings["Connection"].ToString();

       private readonly string _sCon = @SiteConfigurations.SCon;

        DataSet _ds;

        

        // get customer login information
        public DataSet GetCustomerLogin(string email, string password, int storeFrontId)
        {
            try
            {
                _ds = SqlHelper.ExecuteDataset(_sCon, "usp_GetCustomerLoginPartial", email, password, storeFrontId);

               // _ds = SqlHelper.ExecuteDataset(_sCon, "usp_GetCustomerLoginPartial", email, "newpass", storeFrontId);
            }
            catch (Exception ex)
            {
                _ds = null;
                throw ex;
            }
            return _ds;
        }

        public DataSet GetCustomerLogin(string email, int storeFrontId)
        {
            try
            {
                _ds = SqlHelper.ExecuteDataset(_sCon, "usp_GetCustomerLoginPartial", email, string.Empty, storeFrontId);


            }
            catch (Exception ex)
            {
                _ds = null;
                throw ex;
            }
            return _ds;
        }



        // Add customer sign up step 1
        public int AddCustomerPartial(string customerLastIp,
            int customerAffiliateId,
            bool customerIsPartial,
            bool agreeTeleHealth,
             bool agreeTerms,
            int customerCountryFk,
            int customerProvinceFk,
            string customerAddress,
            string customerCity,
            string customerFirstName,
            string customerLastName,
            bool customerFreeShipping,
            bool customerFreeShippingOneYear,
            string customerEmail,
            string customerPassword,
            bool customerActive,
            bool customerTestAccount,
            DateTime customerDateCreated,
            DateTime customerLastModified,
            DateTime customerLastLoginDate,
            string logUserActionParameters,
            string cognitoUserSub,
            string customerPhone,
            string customerTypedEmail)
        {
            try
            {
                logUserActionParameters = CommonFunctions.StripApostropheSymbol(logUserActionParameters);
                SqlParameter[] paramCollection = SqlHelperParameterCache.GetSpParameterSet(_sCon, "usp_addCustomerPartial_cognito", true);

                paramCollection[1].Value = null; // new customer Id
                paramCollection[2].Value = null;  // message

                paramCollection[3].Value = customerLastIp;
                paramCollection[4].Value = customerAffiliateId;
                paramCollection[5].Value = customerIsPartial;

                paramCollection[6].Value = agreeTeleHealth;
                paramCollection[7].Value = agreeTerms;

                paramCollection[8].Value = customerCountryFk;
                paramCollection[9].Value = customerProvinceFk;
                paramCollection[10].Value = CommonFunctions.StripSpecialChar(customerAddress);
                paramCollection[11].Value = CommonFunctions.StripSpecialChar(customerCity);
                paramCollection[12].Value = CommonFunctions.StripSpecialChar(customerFirstName);
                paramCollection[13].Value = CommonFunctions.StripSpecialChar(customerLastName);
                paramCollection[14].Value = customerFreeShipping;
                paramCollection[15].Value = customerFreeShippingOneYear;


                paramCollection[16].Value = CommonFunctions.StripSpecialChar(customerEmail);
                paramCollection[17].Value = string.Empty;
                paramCollection[18].Value = customerActive;
                paramCollection[19].Value = customerTestAccount;
                paramCollection[20].Value = customerDateCreated;
                paramCollection[21].Value = customerLastModified;
                paramCollection[22].Value = customerLastLoginDate;
                paramCollection[23].Value = cognitoUserSub;
                paramCollection[24].Value = customerPhone;
                paramCollection[25].Value = customerTypedEmail;




                SqlHelper.ExecuteNonQuery(_sCon, CommandType.StoredProcedure, "usp_addCustomerPartial_cognito", paramCollection);

                var msg = paramCollection[2].Value;

                var actionQuery = msg.ToString();

                var value = paramCollection[1].Value;

                if (value != null)
                {
                    int customerId = Convert.ToInt32(value.ToString());
                    LogUserAction logUserAction = new LogUserAction
                    {
                        LogUserActionTypeFk = 1,
                        LogUserActionName = "AddCustomerPartial",
                        LogUserActionParams = logUserActionParameters,
                        LogUserActionStoreFrontFk = SiteConfigurations.StoreFrontId,
                        LogActionStoreFrontName = SiteConfigurations.StoreFrontName,
                        LogActionUserIp = customerLastIp,
                        LogUserActionQuery = actionQuery.Trim(),
                        LogActionCustomerFk = customerId,
                        LogActionCustomerName = CommonFunctions.StripSpecialChar(customerFirstName) + " " + CommonFunctions.StripSpecialChar(customerLastName),
                        LogUserActionErrorFk = 0,
                        LogActionCustomerEmail = customerEmail
                    };
                    // insert Add Customer log
                    int logUserActionId = AddLogUserAction(logUserAction);

                    return customerId;
                }

                return 0;

            }
            catch (Exception ex)
            {
                // log exception details
                LogUserAction logUserAction = new LogUserAction
                {
                    LogUserActionTypeFk = 1,
                    LogUserActionName = "AddCustomerPartial",
                    LogUserActionParams = logUserActionParameters,
                    LogUserActionStoreFrontFk = SiteConfigurations.StoreFrontId,
                    LogActionStoreFrontName = SiteConfigurations.StoreFrontName,
                    LogActionUserIp = customerLastIp,
                    LogUserActionQuery = string.Empty, // query will be empty in case of exception
                    LogActionCustomerFk = 0,
                    LogActionCustomerName = CommonFunctions.StripSpecialChar(customerFirstName) + " " + CommonFunctions.StripSpecialChar(customerLastName),
                    LogActionCustomerEmail = CommonFunctions.StripSpecialChar(customerEmail),
                    LogUserActionDatabaseException = ex.Message,
                    LogUserActionErrorFk = 1 // set 1 for database exception
                };
                AddLogUserAction(logUserAction);
                return 0;
            }
        }

        // update customer partial handles updating step 2 info
        public int UpdateCustomerPartial(int customerId,
            bool customerIsPartial,
            string customerEmail,
            string customerFirstName,
            string customerLastName,
            string customerAddress,
            string customerCity,
            int customerProvinceFk,
            string customerZipCode,
            string customerPhone,
            string customerLastIp,
            string actionParameters)
        {
            try
            {
                SqlParameter[] paramCollection = SqlHelperParameterCache.GetSpParameterSet(_sCon, "usp_UpdateCustomerPartial", true);

                paramCollection[1].Value = customerId;
                paramCollection[2].Value = customerIsPartial;
                paramCollection[3].Value = CommonFunctions.StripSpecialChar(customerFirstName);
                paramCollection[4].Value = CommonFunctions.StripSpecialChar(customerLastName);
                paramCollection[5].Value = CommonFunctions.StripSpecialChar(customerAddress);
                paramCollection[6].Value = CommonFunctions.StripSpecialChar(customerCity);
                paramCollection[7].Value = customerProvinceFk;
                paramCollection[8].Value = CommonFunctions.StripSpecialChar(customerZipCode);
                paramCollection[9].Value = customerPhone;
                paramCollection[10].Value = null; // UpdateStatus
                paramCollection[11].Value = null; // Message

                SqlHelper.ExecuteNonQuery(_sCon, CommandType.StoredProcedure, "usp_UpdateCustomerPartial", paramCollection);

                var updateStatus = Convert.ToInt32(paramCollection[10].Value.ToString());
                var message = paramCollection[11].Value.ToString();

                // get the sql query
                var actionQuery = message;

                LogUserAction logUserAction = new LogUserAction
                {
                    LogUserActionTypeFk = 2,
                    LogUserActionName = "UpdateCustomerPartial",
                    LogUserActionParams = actionParameters,
                    LogUserActionStoreFrontFk = SiteConfigurations.StoreFrontId,
                    LogActionStoreFrontName = SiteConfigurations.StoreFrontName,
                    LogUserActionQuery = actionQuery.Trim(),
                    LogActionUserIp = customerLastIp,
                    LogActionCustomerFk = customerId,
                    LogActionCustomerName = CommonFunctions.StripSpecialChar(customerFirstName) + " " + CommonFunctions.StripSpecialChar(customerLastName),
                    LogActionCustomerEmail = CommonFunctions.StripSpecialChar(customerEmail)
                };
                // insert Add Customer log
                int logUserActionId = AddLogUserAction(logUserAction);

                return updateStatus;
            }
            catch (Exception ex)
            {
                // log exception details
                LogUserAction logUserAction = new LogUserAction
                {
                    LogUserActionTypeFk = 2,
                    LogUserActionName = "UpdateCustomerPartial",
                    LogUserActionParams = actionParameters,
                    LogUserActionStoreFrontFk = SiteConfigurations.StoreFrontId,
                    LogActionStoreFrontName = SiteConfigurations.StoreFrontName,
                    LogActionUserIp = customerLastIp,
                    LogUserActionQuery = string.Empty, // query is empty in case of exception
                    LogActionCustomerFk = customerId,
                    LogActionCustomerName = CommonFunctions.StripSpecialChar(customerFirstName) + " " + CommonFunctions.StripSpecialChar(customerLastName),
                    LogActionCustomerEmail = CommonFunctions.StripSpecialChar(customerEmail),
                    LogUserActionDatabaseException = ex.Message,
                    LogUserActionErrorFk = 1 // set 1 for database exception
                };
                AddLogUserAction(logUserAction);
                throw ex;
            }
        }


        // get customer Credit
        public decimal GetCustomerCredit(int customerId)
        {
            try
            {
                SqlParameter[] paramCollection = SqlHelperParameterCache.GetSpParameterSet(_sCon, "usp_getCustomerCredit", true);

                paramCollection[1].Value = customerId;
                paramCollection[2].Value = null; // CreditBalance

                SqlHelper.ExecuteNonQuery(_sCon, CommandType.StoredProcedure, "usp_getCustomerCredit", paramCollection);

                var value = paramCollection[2].Value;
                if (value != null)
                {
                    return Convert.ToDecimal(value.ToString());
                }
                else
                {
                    return 0.0m;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        // get customer credit and order count for checkout step 3
        public DataSet GetCustomerCreditWithOrderCount(int customerId)
        {
            try
            {
                _ds = SqlHelper.ExecuteDataset(_sCon, "usp_getCustomerCreditWithOrderCount", customerId);
            }
            catch (Exception ex)
            {
                _ds = null;
                throw ex;
            }
            return _ds;
        }

        // get customer info 
        public DataSet GetCustomerInfo_ById(int customerId)
        {
            try
            {
                _ds = SqlHelper.ExecuteDataset(_sCon, "usp_GetCustomer_ById", customerId);
            }
            catch (Exception ex)
            {
                _ds = null;
                throw ex;
            }
            return _ds;
        }

        // get customer Info for referred customer
        public DataSet GetReferredCustomerInfo(int referringCustomerId, int customerId)
        {
            try
            {
                _ds = SqlHelper.ExecuteDataset(_sCon, "usp_GetReferedCustomerInfo", referringCustomerId, customerId);
            }
            catch (Exception ex)
            {
                _ds = null;
                throw ex;
            }
            return _ds;
        }

        // get customer info by Id
        public DataSet GetCustomerById(int customerId)
        {
            try
            {
                _ds = SqlHelper.ExecuteDataset(_sCon, "usp_GetCustomer_ById", customerId);
            }
            catch (Exception ex)
            {
                _ds = null;
                throw ex;
            }
            return _ds;
        }

        // Get List of province based on country

        public DataSet GetProvinceListByCountry(int countryId)
        {
            try
            {
                _ds = SqlHelper.ExecuteDataset(_sCon, "usp_GetProvinceList_ByCountry", countryId);
            }
            catch (Exception ex)
            {
                _ds = null;
                throw ex;
            }
            return _ds;
        }

        // get Country List or single country record

        public DataSet GetCountry(int countryId)
        {
            try
            {
                _ds = SqlHelper.ExecuteDataset(_sCon, "usp_GetCountry", countryId);
            }
            catch (Exception ex)
            {
                _ds = null;
                throw ex;
            }
            return _ds;
        }

        // update Customer Info
        public int UpdateCustomerInfo(int customerId,
                                      string firstName,
                                      string lastName,
                                      string email,
                                      string password,
                                      string phone,
                                      string eveningPhone,
                                      string cellPhone,
                                      string address,
                                      string city,
                                      int countryId,
                                      int provinceId,
                                      string zipCode,
                                      string actionParameters,
                                      string customerLastIp
                                     )
        {
            try
            {
                actionParameters = CommonFunctions.StripApostropheSymbol(actionParameters);

                SqlParameter[] paramCollection = SqlHelperParameterCache.GetSpParameterSet(_sCon, "usp_UpdateCustomerInfo", true);

                paramCollection[1].Value = customerId;
                paramCollection[2].Value = CommonFunctions.StripSpecialChar(firstName);
                paramCollection[3].Value = CommonFunctions.StripSpecialChar(lastName);
                paramCollection[4].Value = CommonFunctions.StripSpecialChar(email);
                paramCollection[5].Value = password;
                paramCollection[6].Value = CommonFunctions.StripSpecialChar(phone);
                paramCollection[7].Value = CommonFunctions.StripSpecialChar(eveningPhone);
                paramCollection[8].Value = CommonFunctions.StripSpecialChar(cellPhone);
                paramCollection[9].Value = CommonFunctions.StripSpecialChar(address);
                paramCollection[10].Value = CommonFunctions.StripSpecialChar(city);
                paramCollection[11].Value = countryId;
                paramCollection[12].Value = provinceId;
                paramCollection[13].Value = CommonFunctions.StripSpecialChar(zipCode);
                paramCollection[14].Value = null; // UpdateStatus
                paramCollection[15].Value = null; // Message

                SqlHelper.ExecuteNonQuery(_sCon, CommandType.StoredProcedure, "usp_UpdateCustomerInfo", paramCollection);

                var updateStatus = Convert.ToInt32(paramCollection[14].Value.ToString());
                var message = paramCollection[15].Value.ToString();
                // get the sql query
                var actionQuery = message;

                LogUserAction logUserAction = new LogUserAction
                {
                    LogUserActionTypeFk = 2,
                    LogUserActionName = "UpdateCustomer",
                    LogUserActionParams = actionParameters,
                    LogUserActionStoreFrontFk = SiteConfigurations.StoreFrontId,
                    LogActionStoreFrontName = SiteConfigurations.StoreFrontName,
                    LogUserActionQuery = actionQuery.Trim(),
                    LogActionUserIp = customerLastIp,
                    LogActionCustomerFk = customerId,
                    LogActionCustomerName = CommonFunctions.StripSpecialChar(firstName) + " " + CommonFunctions.StripSpecialChar(lastName),
                    LogActionCustomerEmail = CommonFunctions.StripSpecialChar(email)
                };
                // insert Add Customer log
                int logUserActionId = AddLogUserAction(logUserAction);

                return updateStatus;
            }
            catch (Exception ex)
            {
                // log exception details
                LogUserAction logUserAction = new LogUserAction
                {
                    LogUserActionTypeFk = 2,
                    LogUserActionName = "UpdateCustomer",
                    LogUserActionParams = actionParameters,
                    LogUserActionStoreFrontFk = SiteConfigurations.StoreFrontId,
                    LogActionStoreFrontName = SiteConfigurations.StoreFrontName,
                    LogActionUserIp = customerLastIp,
                    LogUserActionQuery = string.Empty, // query is empty in case of exception
                    LogActionCustomerFk = customerId,
                    LogActionCustomerName = CommonFunctions.StripSpecialChar(firstName) + " " + CommonFunctions.StripSpecialChar(lastName),
                    LogActionCustomerEmail = CommonFunctions.StripSpecialChar(email),
                    LogUserActionDatabaseException = ex.Message,
                    LogUserActionErrorFk = 1 // set 1 for database exception
                };
                AddLogUserAction(logUserAction);
                throw ex;
            }
        }

        // get customer's credit transactions

        public DataSet GetCustomerCreditTransactions(int customerId)
        {
            try
            {
                _ds = SqlHelper.ExecuteDataset(_sCon, "usp_getCustomerCreditTransactions", customerId);
            }
            catch (Exception ex)
            {
                _ds = null;
                throw ex;
            }
            return _ds;
        }

        // get customer's Rx Products Ordered
        public DataSet GetCustomerProductSizeOrdered(int customerId, int productTypeFk, int customerStorefrontId)
        {
            try
            {
                _ds = SqlHelper.ExecuteDataset(_sCon, "usp_getCustomerProductSizeOrdered", customerId, productTypeFk, customerStorefrontId);
            }
            catch (Exception ex)
            {
                _ds = null;
                throw ex;
            }
            return _ds;
        }

        // get list of all orders of customer
        public DataSet GetCustomerAllOrders(int customerId, int customerStorefrontId)
        {
            try
            {
                _ds = SqlHelper.ExecuteDataset(_sCon, "usp_getCustomerAllOrders", customerId, customerStorefrontId);
            }
            catch (Exception ex)
            {
                _ds = null;
                throw ex;
            }
            return _ds;
        }

        // get Cart Details for OrderId
        public DataSet GetCustomerOrderCart(int customerId, int customerStorefrontId, int orderId)
        {
            try
            {
                _ds = SqlHelper.ExecuteDataset(_sCon, "usp_getCustomerOrderCart_ByOrderInvoiceId", customerId, customerStorefrontId, orderId);
            }
            catch (Exception ex)
            {
                _ds = null;
                throw ex;
            }
            return _ds;
        }

        // get customer order details
        public DataSet GetCustomerOrderDetail(int customerId, int customerStorefrontId, int orderId)
        {
            try
            {
                _ds = SqlHelper.ExecuteDataset(_sCon, "usp_getCustomerOrderDetail", customerId, customerStorefrontId, orderId);
            }
            catch (Exception ex)
            {
                _ds = null;
                throw ex;
            }
            return _ds;
        }

        // check if customer email already exists
        public bool CustomerEmailRegistered(string customerEmail, int affiliateId)
        {
            try
            {
                SqlParameter[] paramCollection = SqlHelperParameterCache.GetSpParameterSet(_sCon, "usp_IsCustomerEmailRegistered", true);

                paramCollection[1].Value = customerEmail;
                paramCollection[2].Value = affiliateId;
                paramCollection[3].Value = null; // true/false

                SqlHelper.ExecuteNonQuery(_sCon, CommandType.StoredProcedure, "usp_IsCustomerEmailRegistered", paramCollection);

                var value = paramCollection[3].Value;
                return value != null && Convert.ToBoolean(value.ToString());

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        // check if account exists based on email or id
        public int GetCustomerFromEmailOrId(string customerEmail, int customerId, int affiliateId)
        {
            try
            {
                SqlParameter[] paramCollection = SqlHelperParameterCache.GetSpParameterSet(_sCon, "usp_getCustomerFromEmailOrId", true);

                paramCollection[1].Value = customerEmail;
                paramCollection[2].Value = customerId;
                paramCollection[3].Value = affiliateId;
                paramCollection[4].Value = null; // Id of customer record found

                SqlHelper.ExecuteNonQuery(_sCon, CommandType.StoredProcedure, "usp_getCustomerFromEmailOrId", paramCollection);

                var value = paramCollection[4].Value;
                if (String.IsNullOrEmpty(value.ToString()))
                {
                    return 0;
                }
                else
                {
                    return Convert.ToInt32(value.ToString());

                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        // Add Customer to DB
        public int AddCustomer(
                               string customerLastIp,
                               int customerAffiliateId,
                               string customerFirstName,
                               string customerLastName,
                               string customerSex,
                               string customerEmail,
                               string customerPassword,
                               string customerPhone,
                               string customerEveningPhone,
                               string customerCellPhone,
                               string customerAddress,
                               string customerCity,
                               int customerCountryId,
                               int customerProvinceId,
                               string customerZipCode,
                               string customerReferralId,
                               bool customerActive,
                               bool customerTestAccount,
                               DateTime customerDateCreated,
                               DateTime customerLastModified,
                               DateTime customerLastLoginDate,
                               int customerUserAdminFk,
                               DateTime? customerDob,
                               string logUserActionParameters
                            )
        {
            try
            {
                logUserActionParameters = CommonFunctions.StripApostropheSymbol(logUserActionParameters);
                SqlParameter[] paramCollection = SqlHelperParameterCache.GetSpParameterSet(_sCon, "usp_AddCustomer", true);

                paramCollection[1].Value = customerLastIp;
                paramCollection[2].Value = customerAffiliateId;
                paramCollection[3].Value = CommonFunctions.StripSpecialChar(customerFirstName);
                paramCollection[4].Value = CommonFunctions.StripSpecialChar(customerLastName);
                paramCollection[5].Value = CommonFunctions.StripSpecialChar(customerSex);
                paramCollection[6].Value = CommonFunctions.StripSpecialChar(customerEmail);
                paramCollection[7].Value = customerPassword;
                paramCollection[8].Value = CommonFunctions.StripSpecialChar(customerPhone);
                paramCollection[9].Value = CommonFunctions.StripSpecialChar(customerEveningPhone);
                paramCollection[10].Value = CommonFunctions.StripSpecialChar(customerCellPhone);
                paramCollection[11].Value = CommonFunctions.StripSpecialChar(customerAddress);
                paramCollection[12].Value = CommonFunctions.StripSpecialChar(customerCity);
                paramCollection[13].Value = customerCountryId;
                paramCollection[14].Value = customerProvinceId;
                paramCollection[15].Value = CommonFunctions.StripSpecialChar(customerZipCode);
                paramCollection[16].Value = CommonFunctions.StripSpecialChar(customerReferralId);
                paramCollection[17].Value = customerActive;
                paramCollection[18].Value = customerTestAccount;
                paramCollection[19].Value = customerDateCreated;
                paramCollection[20].Value = customerLastModified;
                paramCollection[21].Value = customerLastLoginDate;
                paramCollection[22].Value = customerUserAdminFk;
                paramCollection[23].Value = customerDob;
                paramCollection[24].Value = null; // new customer Id
                paramCollection[25].Value = null;  // message

                SqlHelper.ExecuteNonQuery(_sCon, CommandType.StoredProcedure, "usp_AddCustomer", paramCollection);

                var msg = paramCollection[25].Value;
                var actionQuery = msg.ToString();
                var value = paramCollection[24].Value;
                if (value != null)
                {
                    int customerId = Convert.ToInt32(value.ToString());
                    LogUserAction logUserAction = new LogUserAction
                    {
                        LogUserActionTypeFk = 1,
                        LogUserActionName = "AddCustomer",
                        LogUserActionParams = logUserActionParameters,
                        LogUserActionStoreFrontFk = SiteConfigurations.StoreFrontId,
                        LogActionStoreFrontName = SiteConfigurations.StoreFrontName,
                        LogActionUserIp = customerLastIp,
                        LogUserActionQuery = actionQuery.Trim(),
                        LogActionCustomerFk = customerId,
                        LogActionCustomerName = CommonFunctions.StripSpecialChar(customerFirstName) + " " + CommonFunctions.StripSpecialChar(customerLastName),
                        LogUserActionErrorFk = 0,
                        LogActionCustomerEmail = CommonFunctions.StripSpecialChar(customerEmail)
                    };
                    // insert Add Customer log
                    int logUserActionId = AddLogUserAction(logUserAction);

                    return customerId;
                }

                return 0;

            }
            catch (Exception ex)
            {
                // log exception details
                LogUserAction logUserAction = new LogUserAction
                {
                    LogUserActionTypeFk = 1,
                    LogUserActionName = "AddCustomer",
                    LogUserActionParams = logUserActionParameters,
                    LogUserActionStoreFrontFk = SiteConfigurations.StoreFrontId,
                    LogActionStoreFrontName = SiteConfigurations.StoreFrontName,
                    LogActionUserIp = customerLastIp,
                    LogUserActionQuery = string.Empty, // query will be empty in case of exception
                    LogActionCustomerFk = 0,
                    LogActionCustomerName = CommonFunctions.StripSpecialChar(customerFirstName),
                    LogActionCustomerEmail = CommonFunctions.StripSpecialChar(customerEmail),
                    LogUserActionDatabaseException = ex.Message,
                    LogUserActionErrorFk = 1 // set 1 for database exception
                };
                AddLogUserAction(logUserAction);
                throw ex;
            }
        }

        // get customer Logininfo based on email
        public DataSet GetCustomerLoginInfo(string customerEmail, int affiliateId)
        {
            try
            {
                _ds = SqlHelper.ExecuteDataset(_sCon, "usp_getCustomerLoginInfo", customerEmail, affiliateId);
            }
            catch (Exception ex)
            {
                _ds = null;
                throw ex;
            }
            return _ds;
        }

        // get patient profile of customer
        public DataSet GetPatientProfilesOfCustomer(int customerId, int patientProfileId, int storefontId)
        {
            try
            {
                _ds = SqlHelper.ExecuteDataset(_sCon, "usp_getPatientProfilesOfCustomer", customerId, patientProfileId, storefontId);
            }
            catch (Exception ex)
            {
                _ds = null;
                throw ex;
            }
            return _ds;
        }

        // get medication list for a patient profile
        public DataSet GetPatientMedicationByProfile(int patientProfileId, int medicationId, int customerId, int storefrontId)
        {
            try
            {
                _ds = SqlHelper.ExecuteDataset(_sCon, "usp_getPatientMedicationByProfile", patientProfileId, medicationId, customerId, storefrontId);
            }
            catch (Exception ex)
            {
                _ds = null;
                throw ex;
            }
            return _ds;
        }

        // check if email being saved  exists for another customer
        public bool EmailFoundForOtherCustomer(string customerEmail, int storeFrontId, int customerId)
        {
            try
            {
                SqlParameter[] paramCollection = SqlHelperParameterCache.GetSpParameterSet(_sCon, "usp_IsEmailAssignedtoOtherCustomer", true);

                paramCollection[1].Value = customerEmail;
                paramCollection[2].Value = storeFrontId;
                paramCollection[3].Value = customerId;
                paramCollection[4].Value = null; // true/false

                SqlHelper.ExecuteNonQuery(_sCon, CommandType.StoredProcedure, "usp_IsEmailAssignedtoOtherCustomer", paramCollection);

                var value = paramCollection[4].Value;
                return value != null && Convert.ToBoolean(value.ToString());

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        // update patient profile
        public int UpdateProfile(int patientProfileId,
                                 string patientProfileAllergy,
                                 string patientProfileFirstName,
                                 string patientProfileLastName,
                                 string patientProfileSex,
                                 string patientProfileMedicalHistory,
                                 int patientProfileWeightLb,
                                 DateTime patientProfileBirthDate,
                                 string patientProfilePhysicianFirstName,
                                 string patientProfilePhysicianLastName,
                                 string patientProfilePhysicianPhone,
                                 string patientProfilePhysicianFax,
                                 bool patientProfileChildproofCap,
                                 string Patientprofile_Medication,
                                 string Patientprofile_PhoneNumber,
                                 DateTime patientProfileLastModified,
                                 string actionParameters,
                                 string customerLastIp,
                                 string customerEmail,
                                 int customerId,
                                 string customerName,
                                 bool? patientProfileConsent,
                                 string phn,
                                 string socialHistory,
                                 string pastSurgery,
                                 string familyIllness,
                                 string herbalMedication,
                                 string patientGenderOther
                                )
        {
            try
            {
                SqlParameter[] paramCollection = SqlHelperParameterCache.GetSpParameterSet(_sCon, "usp_UpdateProfile", true);

                paramCollection[1].Value = patientProfileId;
                paramCollection[2].Value = CommonFunctions.StripSpecialChar(patientProfileAllergy);
                paramCollection[3].Value = CommonFunctions.StripSpecialChar(patientProfileFirstName);
                paramCollection[4].Value = CommonFunctions.StripSpecialChar(patientProfileLastName);
                paramCollection[5].Value = patientProfileSex;
                paramCollection[6].Value = CommonFunctions.StripSpecialChar(patientProfileMedicalHistory);
                paramCollection[7].Value = patientProfileWeightLb;
                paramCollection[8].Value = patientProfileBirthDate;
                paramCollection[9].Value = CommonFunctions.StripSpecialChar(patientProfilePhysicianFirstName);
                paramCollection[10].Value = CommonFunctions.StripSpecialChar(patientProfilePhysicianLastName);
                paramCollection[11].Value = CommonFunctions.StripSpecialChar(patientProfilePhysicianPhone);
                paramCollection[12].Value = CommonFunctions.StripSpecialChar(patientProfilePhysicianFax);
                paramCollection[13].Value = patientProfileChildproofCap;
                paramCollection[14].Value = patientProfileLastModified;
                paramCollection[15].Value = null; // UpdateStatus
                paramCollection[16].Value = null; // Message
                paramCollection[17].Value = CommonFunctions.StripSpecialChar(Patientprofile_Medication);
                paramCollection[18].Value = CommonFunctions.StripSpecialChar(Patientprofile_PhoneNumber);
                paramCollection[19].Value = patientProfileConsent;
                paramCollection[20].Value = phn;
                paramCollection[21].Value = socialHistory;
                paramCollection[22].Value = pastSurgery;
                paramCollection[23].Value = familyIllness;
                paramCollection[24].Value = herbalMedication;
                paramCollection[25].Value = patientGenderOther;

                SqlHelper.ExecuteNonQuery(_sCon, CommandType.StoredProcedure, "usp_UpdateProfile", paramCollection);

                var updateStatus = Convert.ToInt32(paramCollection[15].Value.ToString());
                var message = paramCollection[16].Value.ToString();
                var actionQuery = message;

                LogUserAction logUserAction = new LogUserAction
                {
                    LogUserActionTypeFk = 4,
                    LogUserActionName = "UpdatePatientProfile",
                    LogUserActionParams = actionParameters,
                    LogUserActionStoreFrontFk = SiteConfigurations.StoreFrontId,
                    LogActionStoreFrontName = SiteConfigurations.StoreFrontName,
                    LogUserActionQuery = actionQuery.Trim(),
                    LogActionUserIp = customerLastIp,
                    LogActionCustomerFk = customerId,
                    LogActionCustomerName = customerName,
                    LogActionCustomerEmail = customerEmail
                };
                // insert Add Customer log
                int logUserActionId = AddLogUserAction(logUserAction);

                return updateStatus;
            }
            catch (Exception ex)
            {
                // log exception details
                LogUserAction logUserAction = new LogUserAction
                {
                    LogUserActionTypeFk = 4,
                    LogUserActionName = "UpdatePatientProfile",
                    LogUserActionParams = actionParameters,
                    LogUserActionStoreFrontFk = SiteConfigurations.StoreFrontId,
                    LogActionStoreFrontName = SiteConfigurations.StoreFrontName,
                    LogActionUserIp = customerLastIp,
                    LogUserActionQuery = string.Empty, // query will be empty in case of exception
                    LogActionCustomerFk = customerId,
                    LogActionCustomerName = customerName,
                    LogActionCustomerEmail = customerEmail,
                    LogUserActionDatabaseException = ex.Message,
                    LogUserActionErrorFk = 1 // set 1 for database exception
                };
                AddLogUserAction(logUserAction);
                throw ex;
            }
        }

        // update patient profile

        public int UpdatePatientMedication(int patientProfileId,
                                           int patientMedicationId,
                                           string patientMedicationDrugname,
                                           string patientMedicationIllness,
                                           DateTime patientMedicationLastModifed)
        {
            try
            {
                SqlParameter[] paramCollection = SqlHelperParameterCache.GetSpParameterSet(_sCon, "usp_UpdatePatientMedication", true);

                paramCollection[1].Value = patientProfileId;
                paramCollection[2].Value = patientMedicationId;
                paramCollection[3].Value = patientMedicationDrugname;
                paramCollection[4].Value = patientMedicationIllness;
                paramCollection[5].Value = patientMedicationLastModifed;
                paramCollection[6].Value = null; // UpdateStatus
                paramCollection[7].Value = null; // Message

                SqlHelper.ExecuteNonQuery(_sCon, CommandType.StoredProcedure, "usp_UpdatePatientMedication", paramCollection);

                var updateStatus = Convert.ToInt32(paramCollection[6].Value.ToString());
                var message = paramCollection[7].Value.ToString();

                return updateStatus;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        // Add Patient Medication
        public int AddPatientMedication(int patientProfileId, string patientmedicationDrugname, string patientmedicationIllness, DateTime createdOn)
        {
            try
            {
                SqlParameter[] paramCollection = SqlHelperParameterCache.GetSpParameterSet(_sCon, "usp_AddPatientMedication", true);

                paramCollection[1].Value = patientProfileId;
                paramCollection[2].Value = CommonFunctions.StripSpecialChar(patientmedicationDrugname);
                paramCollection[3].Value = CommonFunctions.StripSpecialChar(patientmedicationIllness);
                paramCollection[4].Value = createdOn;
                paramCollection[5].Value = null; // new medication Id
                paramCollection[6].Value = null;  // message

                SqlHelper.ExecuteNonQuery(_sCon, CommandType.StoredProcedure, "usp_AddPatientMedication", paramCollection);

                var msg = paramCollection[6].Value;

                var value = paramCollection[5].Value;
                if (value != null) return Convert.ToInt32(value.ToString());

                return 0;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public int AddPatientProfile(int customerId,
                                     string patientProfileAllergy,
                                     string patientProfileFirstName,
                                     string patientProfileLastName,
                                     string patientProfileSex,
                                     string patientProfileMedicalHistory,
                                     int patientProfileWeightLb,
                                     DateTime patientProfileBirthDate,
                                     string patientProfilePhysicianFirstName,
                                     string patientProfilePhysicianLastName,
                                     string patientProfilePhysicianPhone,
                                     string patientProfilePhysicianFax,
                                     bool patientProfileChildproofCap,
                                     DateTime createdOn,
                                     string actionParameters,
                                     string customerLastIp,
                                     string customerEmail,
                                     string customerName,
                                    string patientPhoneNumber,
                                    string patientMedication,
                                    bool? consultationConsent,
                                    string healthNumber,
                                    string socialHistory,
                                    string pastSurgery,
                                    string familyIllness,
                                    string herbalSupplement,
                                    string patientSexOther
                                    )
        {
            try
            {
                SqlParameter[] paramCollection = SqlHelperParameterCache.GetSpParameterSet(_sCon, "usp_AddPatientProfile", true);

                paramCollection[1].Value = customerId;
                paramCollection[2].Value = CommonFunctions.StripSpecialChar(patientProfileAllergy);
                paramCollection[3].Value = CommonFunctions.StripSpecialChar(patientProfileFirstName);
                paramCollection[4].Value = CommonFunctions.StripSpecialChar(patientProfileLastName);
                paramCollection[5].Value = patientProfileSex;
                paramCollection[6].Value = CommonFunctions.StripSpecialChar(patientProfileMedicalHistory);
                paramCollection[7].Value = patientProfileWeightLb;
                paramCollection[8].Value = patientProfileBirthDate;
                paramCollection[9].Value = CommonFunctions.StripSpecialChar(patientProfilePhysicianFirstName);
                paramCollection[10].Value = CommonFunctions.StripSpecialChar(patientProfilePhysicianLastName);
                paramCollection[11].Value = CommonFunctions.StripSpecialChar(patientProfilePhysicianPhone);
                paramCollection[12].Value = CommonFunctions.StripSpecialChar(patientProfilePhysicianFax);
                paramCollection[13].Value = patientProfileChildproofCap;
                paramCollection[14].Value = createdOn;
                paramCollection[15].Value = patientPhoneNumber;
                paramCollection[16].Value = patientMedication;
                paramCollection[17].Value = consultationConsent;
                paramCollection[18].Value = healthNumber;
                paramCollection[19].Value = null; // new profile Id
                paramCollection[20].Value = null;  // message
                paramCollection[21].Value = socialHistory;
                paramCollection[22].Value = pastSurgery;
                paramCollection[23].Value = familyIllness;
                paramCollection[24].Value = herbalSupplement;
                paramCollection[25].Value = patientSexOther;


                SqlHelper.ExecuteNonQuery(_sCon, CommandType.StoredProcedure, "usp_AddPatientProfile", paramCollection);

                var msg = paramCollection[20].Value;
                var actionQuery = msg.ToString();
                var value = paramCollection[19].Value;
                if (value != null)
                {
                    LogUserAction logUserAction = new LogUserAction
                    {
                        LogUserActionTypeFk = 3,
                        LogUserActionName = "AddPatientProfile",
                        LogUserActionParams = actionParameters,
                        LogUserActionStoreFrontFk = SiteConfigurations.StoreFrontId,
                        LogActionStoreFrontName = SiteConfigurations.StoreFrontName,
                        LogUserActionQuery = actionQuery.Trim(),
                        LogActionUserIp = customerLastIp,
                        LogActionCustomerFk = customerId,
                        LogActionCustomerName = customerName,
                        LogActionCustomerEmail = customerEmail
                    };
                    // insert Add Customer log
                    int logUserActionId = AddLogUserAction(logUserAction);

                    return Convert.ToInt32(value.ToString());
                }

                return 0;

            }
            catch (Exception ex)
            {
                // log exception details
                LogUserAction logUserAction = new LogUserAction
                {
                    LogUserActionTypeFk = 3,
                    LogUserActionName = "AddPatientProfile",
                    LogUserActionParams = actionParameters,
                    LogUserActionStoreFrontFk = SiteConfigurations.StoreFrontId,
                    LogActionStoreFrontName = SiteConfigurations.StoreFrontName,
                    LogActionUserIp = customerLastIp,
                    LogUserActionQuery = string.Empty, // action query is empty in case of exception
                    LogActionCustomerFk = customerId,
                    LogActionCustomerName = customerName,
                    LogActionCustomerEmail = customerEmail,
                    LogUserActionDatabaseException = ex.Message,
                    LogUserActionErrorFk = 1 // set 1 for database exception
                };

                AddLogUserAction(logUserAction);
                throw ex;
            }
        }


        // return Id of customer referring passed customer
        public int GetReferredBy(int customerId)
        {
            try
            {
                SqlParameter[] paramCollection = SqlHelperParameterCache.GetSpParameterSet(_sCon, "usp_getReferedBy", true);

                paramCollection[1].Value = customerId;
                paramCollection[2].Value = null; // id of Refering Customer

                SqlHelper.ExecuteNonQuery(_sCon, CommandType.StoredProcedure, "usp_getReferedBy", paramCollection);

                var value = paramCollection[2].Value;
                if (value != null)
                {
                    return Convert.ToInt32(value.ToString());
                }
                else
                {
                    return 0;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        // return order count of the customer
        public int GetCustomerOrderCount(int customerId)
        {
            try
            {
                SqlParameter[] paramCollection = SqlHelperParameterCache.GetSpParameterSet(_sCon, "[usp_getCustomerOrderCount]", true);

                paramCollection[1].Value = customerId;
                paramCollection[2].Value = null; // order count

                SqlHelper.ExecuteNonQuery(_sCon, CommandType.StoredProcedure, "[usp_getCustomerOrderCount]", paramCollection);

                var value = paramCollection[2].Value;
                if (value != null)
                {
                    return Convert.ToInt32(value.ToString());
                }
                else
                {
                    return 0;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        // check if credit is already issued 

        public bool IsCreditAlreadyIssued(int customerId)
        {
            try
            {
                SqlParameter[] paramCollection = SqlHelperParameterCache.GetSpParameterSet(_sCon, "usp_IsCreditAlreadyIssued", true);

                paramCollection[1].Value = customerId;
                paramCollection[2].Value = null; // true/false

                SqlHelper.ExecuteNonQuery(_sCon, CommandType.StoredProcedure, "usp_IsCreditAlreadyIssued", paramCollection);

                var value = paramCollection[2].Value;
                return value != null && Convert.ToBoolean(value.ToString());

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        // Add Refer Credit to both accounts
        public int AddReferCreditToBoth(int referringCustomerId,
                                        decimal referringCredit,
                                        string referringComment,
                                        int customerId,
                                        decimal referredCredit,
                                        string referredComment,
                                        int referCreditTransactionType,
                                        DateTime dateCreated,
                                        int orderInvoiceId)
        {
            try
            {
                SqlParameter[] paramCollection = SqlHelperParameterCache.GetSpParameterSet(_sCon, "usp_AddReferCreditToBoth", true);

                paramCollection[1].Value = referringCustomerId;
                paramCollection[2].Value = referringCredit;
                paramCollection[3].Value = referringComment;
                paramCollection[4].Value = customerId;
                paramCollection[5].Value = referredCredit;
                paramCollection[6].Value = referredComment;
                paramCollection[7].Value = referCreditTransactionType;
                paramCollection[8].Value = dateCreated;
                paramCollection[9].Value = orderInvoiceId;

                paramCollection[10].Value = null; // new Id of first record added
                paramCollection[11].Value = null;  // message

                SqlHelper.ExecuteNonQuery(_sCon, CommandType.StoredProcedure, "usp_AddReferCreditToBoth", paramCollection);

                var msg = paramCollection[11].Value;
                var value = paramCollection[10].Value;

                if (value != null) return Convert.ToInt32(value.ToString());

                return 0;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        // get province by Id
        public DataSet GetProvinceById(int shippingProvinceId)
        {
            try
            {
                _ds = SqlHelper.ExecuteDataset(_sCon, "usp_getProvince_ById", shippingProvinceId);
            }
            catch (Exception ex)
            {
                _ds = null;
                throw ex;
            }
            return _ds;
        }

        // Get Country by Id
        public DataSet GetCountryById(int shippingCountryId)
        {
            try
            {
                _ds = SqlHelper.ExecuteDataset(_sCon, "usp_getCountry", shippingCountryId);
            }
            catch (Exception ex)
            {
                _ds = null;
                throw ex;
            }
            return _ds;
        }

        public bool IsEmailAlreadyRegistered(string customerEmail, int affiliateId)
        {
            try
            {
                SqlParameter[] paramCollection = SqlHelperParameterCache.GetSpParameterSet(_sCon, "usp_IsCustomerEmailRegistered", true);

                paramCollection[1].Value = customerEmail;
                paramCollection[2].Value = affiliateId;
                paramCollection[3].Value = null; // true/false

                SqlHelper.ExecuteNonQuery(_sCon, CommandType.StoredProcedure, "usp_IsCustomerEmailRegistered", paramCollection);

                var value = paramCollection[3].Value;
                return value != null && Convert.ToBoolean(value.ToString());

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void DelProfile(int patientProfileId, int customerId)
        {
            try
            {
                SqlParameter[] paramCollection = SqlHelperParameterCache.GetSpParameterSet(_sCon, "usp_delPatientProfile", true);

                paramCollection[1].Value = patientProfileId;
                paramCollection[2].Value = customerId;
                SqlHelper.ExecuteNonQuery(_sCon, CommandType.StoredProcedure, "usp_delPatientProfile", paramCollection);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        // add customer email to PromoEmails table
        public int AddPromoEmail(int emailTypeFk, string customerEmail, string clientIp, string storefrontId, bool isActive, DateTime dateAdded)
        {
            try
            {
                SqlParameter[] paramCollection = SqlHelperParameterCache.GetSpParameterSet(_sCon, "usp_AddPromoEmail", true);

                paramCollection[1].Value = emailTypeFk;
                paramCollection[2].Value = customerEmail;
                paramCollection[3].Value = clientIp;
                paramCollection[4].Value = storefrontId;
                paramCollection[5].Value = isActive;
                paramCollection[6].Value = dateAdded;
                paramCollection[7].Value = null; // new Id of record added
                paramCollection[8].Value = null;  // message

                SqlHelper.ExecuteNonQuery(_sCon, CommandType.StoredProcedure, "usp_AddPromoEmail", paramCollection);

                var msg = paramCollection[8].Value;
                var value = paramCollection[7].Value;

                if (value != null) return Convert.ToInt32(value.ToString());

                return 0;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public int UnsubscribeNewLetter(int promoIdDecoded)
        {
            try
            {
                SqlParameter[] paramCollection = SqlHelperParameterCache.GetSpParameterSet(_sCon, "usp_UnsubscribeNewLetter", true);

                paramCollection[1].Value = promoIdDecoded;
                paramCollection[2].Value = null; // UpdateStatus
                paramCollection[3].Value = null; // Message

                SqlHelper.ExecuteNonQuery(_sCon, CommandType.StoredProcedure, "usp_UnsubscribeNewLetter", paramCollection);

                var updateStatus = Convert.ToInt32(paramCollection[2].Value.ToString());
                var message = paramCollection[3].Value.ToString();

                return updateStatus;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        // Add user action log to keep a track of malicious attacks
        public int AddLogUserAction(LogUserAction logUserAction)
        {
            try
            {
                SqlParameter[] paramCollection = SqlHelperParameterCache.GetSpParameterSet(_sCon, "usp_AddLogUserAction", true);

                paramCollection[1].Value = logUserAction.LogUserActionTypeFk;
                paramCollection[2].Value = logUserAction.LogUserActionName;
                paramCollection[3].Value = logUserAction.LogUserActionParams;
                paramCollection[4].Value = string.IsNullOrEmpty(logUserAction.LogUserActionQuery) ? string.Empty : logUserAction.LogUserActionQuery.Replace("'", "''"); //To convert ' to '' in returned action query
                paramCollection[5].Value = logUserAction.LogUserActionStoreFrontFk;
                paramCollection[6].Value = logUserAction.LogActionStoreFrontName;
                paramCollection[7].Value = logUserAction.LogActionUserIp;
                paramCollection[8].Value = logUserAction.LogActionCustomerFk;
                paramCollection[9].Value = logUserAction.LogActionCustomerName;
                paramCollection[10].Value = logUserAction.LogActionOrderInvoiceFk;
                paramCollection[11].Value = logUserAction.LogActionShippingInvoiceFk;
                paramCollection[12].Value = logUserAction.LogActionCustomerEmail;
                paramCollection[13].Value = string.IsNullOrEmpty(logUserAction.LogUserActionDatabaseException) ? string.Empty : logUserAction.LogUserActionDatabaseException.Replace("@", "").Replace("'", "''");
                paramCollection[14].Value = logUserAction.LogUserActionErrorFk;
                paramCollection[15].Value = null; // loguseractionid
                paramCollection[16].Value = null; // message


                SqlHelper.ExecuteNonQuery(_sCon, CommandType.StoredProcedure, "usp_AddLogUserAction", paramCollection);

                var value = paramCollection[15].Value;

                if (value != null)
                {
                    return (int)value;
                }
                else
                {
                    return 0;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public int AddUserPasswordRecoveryDetails(int customerFk, string encryptedToken, string decryptedToken, DateTime linkExpiry, int resetStatus, DateTime dateCreated)
        {
            try
            {
                SqlParameter[] paramCollection = SqlHelperParameterCache.GetSpParameterSet(_sCon, "usp_AddUserPasswordRecoveryDetails", true);

                paramCollection[1].Value = customerFk;
                paramCollection[2].Value = encryptedToken;
                paramCollection[3].Value = decryptedToken;
                paramCollection[4].Value = linkExpiry;
                paramCollection[5].Value = resetStatus;
                paramCollection[6].Value = dateCreated;
                paramCollection[7].Value = null; // userPasswordRecoveryId
                paramCollection[8].Value = null; // message


                SqlHelper.ExecuteNonQuery(_sCon, CommandType.StoredProcedure, "usp_AddUserPasswordRecoveryDetails", paramCollection);

                var value = paramCollection[7].Value;
                var message = paramCollection[8].Value;
                if (value != null)
                {
                    return (int)value;
                }
                else
                {
                    return 0;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        // get Customer Password recovery Token
        public DataSet GetUserPasswordRecoveryDetails(string token)
        {
            try
            {
                _ds = SqlHelper.ExecuteDataset(_sCon, "usp_getUserPasswordRecoveryDetails", token);
            }
            catch (Exception ex)
            {
                _ds = null;
                throw ex;
            }
            return _ds;
        }

        // update customer password
        public int UpdateCustomerPassword(int customerFk, string password, DateTime dateModified)
        {
            try
            {
                SqlParameter[] paramCollection = SqlHelperParameterCache.GetSpParameterSet(_sCon, "usp_updateCustomerPassword", true);

                paramCollection[1].Value = customerFk;
                paramCollection[2].Value = password;
                paramCollection[3].Value = dateModified;
                paramCollection[4].Value = null; //updateStatus
                paramCollection[5].Value = null; // message


                SqlHelper.ExecuteNonQuery(_sCon, CommandType.StoredProcedure, "usp_updateCustomerPassword", paramCollection);

                var value = paramCollection[4].Value;

                if (value != null)
                {
                    return (int)value;
                }
                return 0;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        // update password recovery reset status
        public int UserPasswordRecoveryResetStatus(int userPasswordRecoveryId, int customerFk, int resetStatus, DateTime dateModified)
        {
            try
            {
                SqlParameter[] paramCollection = SqlHelperParameterCache.GetSpParameterSet(_sCon, "usp_updatePasswordResetStatus", true);

                paramCollection[1].Value = customerFk;
                paramCollection[2].Value = resetStatus;
                paramCollection[3].Value = userPasswordRecoveryId;
                paramCollection[4].Value = dateModified;
                paramCollection[5].Value = null; //updateStatus
                paramCollection[6].Value = null; // message


                SqlHelper.ExecuteNonQuery(_sCon, CommandType.StoredProcedure, "usp_updatePasswordResetStatus", paramCollection);

                var value = paramCollection[5].Value;

                if (value != null)
                {
                    return (int)value;
                }
                return 0;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DataSet GetCustomerOpenOrders(int customerId, int storefrontId)
        {
            try
            {
                _ds = SqlHelper.ExecuteDataset(_sCon, "usp_getCustomerOrdersByStatus", customerId, storefrontId, "open");
            }
            catch (Exception ex)
            {
                _ds = null;
                throw ex;
            }
            return _ds;
        }

        // get closed orders for the customer
        public DataSet GetCustomerClosedOrders(int customerId, int storefrontId /*, DateTime startDate, DateTime endDate*/) //uncomment parameters to get the closed orders based on year Selection
        {
            try
            {
                _ds = SqlHelper.ExecuteDataset(_sCon, "usp_getCustomerOrdersByStatus", customerId, storefrontId, "closed");
                /*ds = SqlHelper.ExecuteDataset(sCon, "usp_getCustomerOrdersByStatusTem", customerId, storefrontId, "closed" , startDate, endDate);*/
            }
            catch (Exception ex)
            {
                _ds = null;
                throw ex;
            }
            return _ds;
        }

        // get customer info 
        public DataSet GetCustomerInfoPartial_ById(int customerId)
        {
            try
            {
                _ds = SqlHelper.ExecuteDataset(_sCon, "usp_GetCustomerPartial_ById", customerId);
            }
            catch (Exception ex)
            {
                _ds = null;
                throw ex;
            }
            return _ds;
        }

        public int UpdateCustomerIsPartialFlag(int customerId, bool isPartial)
        {
            try
            {
                SqlParameter[] paramCollection = SqlHelperParameterCache.GetSpParameterSet(_sCon, "usp_UpdateCustomerIsPartialFlag", true);

                paramCollection[1].Value = customerId;
                paramCollection[2].Value = isPartial;
                paramCollection[3].Value = null; // update status
                paramCollection[4].Value = null; // message

                SqlHelper.ExecuteNonQuery(_sCon, CommandType.StoredProcedure, "usp_UpdateCustomerIsPartialFlag", paramCollection);

                var value = paramCollection[3].Value;
                if (value != null)
                {
                    return Convert.ToInt32(value.ToString());
                }
                else
                {
                    return 0;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        // update general enquiry submitted from get in touch page
        public int AddGeneralEnquiry(int storefrontFk, string customerName, string customerEmail, string customerPhone, string queryPosted, DateTime dateCreated, string category, string zendeskTicketId)
        {
            try
            {
                SqlParameter[] paramCollection = SqlHelperParameterCache.GetSpParameterSet(_sCon, "usp_AddGeneralEnquiry", true);

                paramCollection[1].Value = storefrontFk;
                paramCollection[2].Value = customerName;
                paramCollection[3].Value = customerEmail;
                paramCollection[4].Value = customerPhone;
                paramCollection[5].Value = queryPosted;
                paramCollection[6].Value = dateCreated;
                paramCollection[7].Value = category;
                paramCollection[8].Value = zendeskTicketId;
                paramCollection[9].Value = null; // generalEnquiryid
                paramCollection[10].Value = null; // message


                SqlHelper.ExecuteNonQuery(_sCon, CommandType.StoredProcedure, "usp_AddGeneralEnquiry", paramCollection);

                var value = paramCollection[9].Value;

                if (value != null)
                {
                    return (int)value;
                }
                else
                {
                    return 0;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //methods to upload Id document details  for customer
        public int AddCustomerIdDocumentDetail(int customerFK, int storeFrontFK, string fileName, bool active, DateTime dateCreated, string existingBackImage)
        {
            try
            {
                SqlParameter[] paramCollection = SqlHelperParameterCache.GetSpParameterSet(_sCon, "usp_addCustomerIdDocument", true);

                paramCollection[1].Value = customerFK;
                paramCollection[2].Value = storeFrontFK;
                paramCollection[3].Value = fileName;
                paramCollection[4].Value = active;
                paramCollection[5].Value = dateCreated;
                paramCollection[6].Value = existingBackImage;

                paramCollection[7].Value = null; // customerDocumentId
                paramCollection[8].Value = null; // message


                SqlHelper.ExecuteNonQuery(_sCon, CommandType.StoredProcedure, "usp_addCustomerIdDocument", paramCollection);

                var value = paramCollection[7].Value;
                var message = paramCollection[8].Value;
                if (value != null)
                {
                    return (int)value;
                }
                else
                {
                    return 0;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //methods to upload Id document details  for customer
        public int AddCustomerPrescription(int customerFK, int orderInvoiceFk, int storeFrontFK, string fileName, bool active, DateTime dateCreated)
        {
            try
            {
                SqlParameter[] paramCollection = SqlHelperParameterCache.GetSpParameterSet(_sCon, "usp_addCustomerPrescription", true);

                paramCollection[1].Value = customerFK;
                paramCollection[2].Value = orderInvoiceFk;
                paramCollection[3].Value = storeFrontFK;
                paramCollection[4].Value = fileName;
                paramCollection[5].Value = active;
                paramCollection[6].Value = dateCreated;
                paramCollection[7].Value = null; // customerDocumentId
                paramCollection[8].Value = null; // message


                SqlHelper.ExecuteNonQuery(_sCon, CommandType.StoredProcedure, "usp_addCustomerPrescription", paramCollection);

                var value = paramCollection[7].Value;
                var message = paramCollection[8].Value;
                if (value != null)
                {
                    return (int)value;
                }
                else
                {
                    return 0;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public DataSet GetCustomerIdDocumentByCustomerId(int customerId)
        {
            try
            {
                _ds = SqlHelper.ExecuteDataset(_sCon, "usp_getCustomerIdDocument", customerId);
            }
            catch (Exception ex)
            {
                _ds = null;
                throw ex;
            }
            return _ds;
        }
        public bool UpdateCustomerIdInActive(int documentId, int customerFK, int storeFrontFK)
        {
            try
            {
                SqlParameter[] paramCollection = SqlHelperParameterCache.GetSpParameterSet(_sCon, "usp_DeactivatePrevCustomerIdDoc", true);

                paramCollection[1].Value = documentId;
                paramCollection[2].Value = customerFK;
                paramCollection[3].Value = storeFrontFK; // true/false
                paramCollection[4].Value = null;
                SqlHelper.ExecuteNonQuery(_sCon, CommandType.StoredProcedure, "usp_DeactivatePrevCustomerIdDoc", paramCollection);

                var value = paramCollection[4].Value;
                return value != null && Convert.ToBoolean(value.ToString());

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //Get List for Questionnaire Responses
        public DataSet IfUserAlreadyAnsweredQuestionnaire(int catId, int customerId, DateTime currentTimeStamp, int orderTimeSpan, int orderInvoiceFk)
        {
            try
            {
                _ds = SqlHelper.ExecuteDataset(_sCon, "usp_GetQuestionnaireResponseByCustomerFK", catId, customerId, currentTimeStamp, orderTimeSpan, orderInvoiceFk);
            }
            catch (Exception ex)
            {
                _ds = null;
                throw ex;
            }
            return _ds;
        }

        // get customer QuestionnarireRespose by category
        public DataSet GetQuestionnairCategoryResponseByCategory(int customerFk, int productCategoryFk, int orderInvoiceFk)
        {
            try
            {
                _ds = SqlHelper.ExecuteDataset(_sCon, "usp_getQuestionnaireCategoryResponse_ByCategory", customerFk, productCategoryFk, orderInvoiceFk);
            }
            catch (Exception ex)
            {
                _ds = null;
                throw ex;
            }
            return _ds;
        }
        //added for uset all presciption inactive except the uploaded 
        public bool UpdatePrescriptionInActive(int documentId, int customerFK, int orderInvoiceId, int storeFrontFK)
        {
            try
            {
                SqlParameter[] paramCollection = SqlHelperParameterCache.GetSpParameterSet(_sCon, "usp_DeactivatePrevPrescription", true);

                paramCollection[1].Value = documentId;
                paramCollection[2].Value = customerFK;
                paramCollection[3].Value = orderInvoiceId;
                paramCollection[4].Value = storeFrontFK; // true/false              
                paramCollection[5].Value = null;
                SqlHelper.ExecuteNonQuery(_sCon, CommandType.StoredProcedure, "usp_DeactivatePrevPrescription", paramCollection);

                var value = paramCollection[5].Value;
                return value != null && Convert.ToBoolean(value.ToString());

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public int AddLogCosultationConsent(bool? updatedvalue, int patientProfileFk, int storefronFk, string actionType, DateTime dateCreated)
        {
            try
            {
                SqlParameter[] paramCollection = SqlHelperParameterCache.GetSpParameterSet(_sCon, "usp_AddLogCosultationConsent", true);

                paramCollection[1].Value = actionType == "Add"? updatedvalue : !updatedvalue;
                paramCollection[2].Value = updatedvalue;
                paramCollection[3].Value = patientProfileFk;
                paramCollection[4].Value = storefronFk;
                paramCollection[5].Value = actionType;
                paramCollection[6].Value = dateCreated;
               
                paramCollection[7].Value = null; // logConsentId
                paramCollection[8].Value = null; // message


                SqlHelper.ExecuteNonQuery(_sCon, CommandType.StoredProcedure, "usp_AddLogCosultationConsent", paramCollection);

                var value = paramCollection[7].Value;
                var message = paramCollection[8].Value;
                if (value != null)
                {
                    return (int)value;
                }
                else
                {
                    return 0;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DataSet GetCustomerAllSubscription(int customerFk)
        {
            try
            {
                _ds = SqlHelper.ExecuteDataset(_sCon, "usp_getCustomerAllSubscription", customerFk);
            }
            catch (Exception ex)
            {
                _ds = null;
                throw ex;
            }
            return _ds;
        }

        public int UnSubscribeAutoRefill(int subscriptionId)
        {
            try
            {
                SqlParameter[] paramCollection = SqlHelperParameterCache.GetSpParameterSet(_sCon, "usp_UnsubscribeAutoRefill", true);

                paramCollection[1].Value = subscriptionId;
                paramCollection[2].Value = null; // UpdateStatus
                paramCollection[3].Value = null; // Message

                SqlHelper.ExecuteNonQuery(_sCon, CommandType.StoredProcedure, "usp_UnsubscribeAutoRefill", paramCollection);

                var updateStatus = Convert.ToInt32(paramCollection[2].Value.ToString());
                var message = paramCollection[3].Value.ToString();

                return updateStatus;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public int AddLogCustomerSubscription(int customerFk, int subscriptionFk, bool updatedValue, string actionType, DateTime dateCreated)
        {
            try
            {
                SqlParameter[] paramCollection = SqlHelperParameterCache.GetSpParameterSet(_sCon, "usp_AddLogCustomerSubscription", true);

                paramCollection[1].Value = customerFk;
                paramCollection[2].Value = subscriptionFk;
                paramCollection[3].Value = !updatedValue;
                paramCollection[4].Value = updatedValue;
                paramCollection[5].Value = actionType;
                paramCollection[6].Value = dateCreated;

                paramCollection[7].Value = null; // logConsentId
                paramCollection[8].Value = null; // message


                SqlHelper.ExecuteNonQuery(_sCon, CommandType.StoredProcedure, "usp_AddLogCustomerSubscription", paramCollection);

                var value = paramCollection[7].Value;
                var message = paramCollection[8].Value;
                if (value != null)
                {
                    return (int)value;
                }
                else
                {
                    return 0;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        public DataSet GetCustomerLastOrder(int storeFrontFk,
           int customerId,
           
           DateTime currentTimeStamp,
           int orderTimeSpan)
        {
            try
            {
                _ds = SqlHelper.ExecuteDataset(_sCon, "usp_GetCustomerLastOrder", storeFrontFk, customerId, currentTimeStamp, orderTimeSpan);
            }
            catch (Exception ex)
            {
                _ds = null;
                throw ex;
            }
            return _ds;
        }
        public int UpdateOrderConsultationFlag(int orderFk, bool? orderConsultationFlag)
        {
            try
            {
                SqlParameter[] paramCollection = SqlHelperParameterCache.GetSpParameterSet(_sCon, "usp_updateOrderConsentFlag", true);

                paramCollection[1].Value = orderFk;
                paramCollection[2].Value = orderConsultationFlag;


                paramCollection[3].Value = null; // UpdateStatus
                paramCollection[4].Value = null; // Message

                SqlHelper.ExecuteNonQuery(_sCon, CommandType.StoredProcedure, "usp_updateOrderConsentFlag", paramCollection);

                var updateStatus = Convert.ToInt32(paramCollection[3].Value.ToString());
                var message = paramCollection[4].Value.ToString();

                return updateStatus;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public int UpdateCustomerIdBackFileName(int customerIdDocumentFk, string backSideFileName, int customerFK, int storeFrontFK, DateTime modifiedDate)
        {
            try
            {
                SqlParameter[] paramCollection = SqlHelperParameterCache.GetSpParameterSet(_sCon, "usp_updateCustomerIdDocBackFileName", true);
                paramCollection[1].Value = null;
                paramCollection[2].Value = null;

                paramCollection[3].Value = customerIdDocumentFk;
                paramCollection[4].Value = customerFK;
                paramCollection[5].Value = storeFrontFK;
                paramCollection[6].Value = backSideFileName; 
                paramCollection[7].Value = modifiedDate;

                SqlHelper.ExecuteNonQuery(_sCon, CommandType.StoredProcedure, "usp_updateCustomerIdDocBackFileName", paramCollection);

                var value = paramCollection[1].Value;
                var returnVal = paramCollection[2].Value;

                return Convert.ToInt32(returnVal);
               // return value != null && Convert.ToBoolean(value.ToString());

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public int AddLogCustomerIdDocument(int customerDocumentId, int customerFk, int storefronFk, 
            string actionType, DateTime dateCreated, bool mobileDevice, string module, string logMessage)
        {
            try
            {
                SqlParameter[] paramCollection = SqlHelperParameterCache.GetSpParameterSet(_sCon, "usp_AddLogCustomerIdDocumentUpload", true);
                paramCollection[1].Value = null; // logCustomerIdDocumentId
                paramCollection[2].Value = null; // message

                paramCollection[3].Value = customerDocumentId;
                paramCollection[4].Value = customerFk;
                paramCollection[5].Value = storefronFk;
                paramCollection[6].Value = actionType;
                paramCollection[7].Value = dateCreated;
                paramCollection[8].Value = mobileDevice;
                paramCollection[9].Value = module;
                paramCollection[10].Value = logMessage;

               


                SqlHelper.ExecuteNonQuery(_sCon, CommandType.StoredProcedure, "usp_AddLogCustomerIdDocumentUpload", paramCollection);

                var value = paramCollection[1].Value;
                var message = paramCollection[2].Value;
                if (value != null)
                {
                    return (int)value;
                }
                else
                {
                    return 0;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public int UpdateCartItemPatientProfile(int orderFk, int patientProfileFk)
        {
            try
            {
                SqlParameter[] paramCollection = SqlHelperParameterCache.GetSpParameterSet(_sCon, "usp_updateCartItemPatientProfile", true);

                paramCollection[1].Value = orderFk;
                paramCollection[2].Value = patientProfileFk;


                paramCollection[3].Value = null; // UpdateStatus
                paramCollection[4].Value = null; // Message

                SqlHelper.ExecuteNonQuery(_sCon, CommandType.StoredProcedure, "usp_updateCartItemPatientProfile", paramCollection);

                var updateStatus = Convert.ToInt32(paramCollection[3].Value.ToString());
                var message = paramCollection[4].Value.ToString();

                return updateStatus;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public int AddLogCongnitoSignUpResponse(CognitoSignupResponse cognitoSigupResponse, int storefrontFk, int customerFk, string ipAddress, DateTime dateCreated, string response)
        {
            try
            {
                SqlParameter[] paramCollection = SqlHelperParameterCache.GetSpParameterSet(_sCon, "usp_AddLogCognitoSignupResponse", true);

                paramCollection[1].Value = null; // logConsentId
                paramCollection[2].Value = null; // message

                paramCollection[3].Value = storefrontFk;
                paramCollection[4].Value = cognitoSigupResponse.UserConfirmed;

                paramCollection[5].Value = cognitoSigupResponse.HttpStatusCode;
                paramCollection[6].Value = cognitoSigupResponse.UserSub;
                paramCollection[7].Value = cognitoSigupResponse.CodeDeliveryMethod;
                paramCollection[8].Value = ipAddress;
                paramCollection[9].Value = response;
                paramCollection[10].Value = cognitoSigupResponse.UserCreated;
                paramCollection[11].Value = cognitoSigupResponse.Message == null ? string.Empty : cognitoSigupResponse.Message.Replace("'", ""); ;
                paramCollection[12].Value = cognitoSigupResponse.Exception == null ? string.Empty : cognitoSigupResponse.Exception.Replace("'", ""); ;
                paramCollection[13].Value = dateCreated;
                paramCollection[14].Value = customerFk;

                SqlHelper.ExecuteNonQuery(_sCon, CommandType.StoredProcedure, "usp_AddLogCognitoSignupResponse", paramCollection);

                var value = paramCollection[1].Value;
                var message = paramCollection[2].Value;
                if (value != null)
                {
                    return (int)value;
                }
                else
                {
                    return 0;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public int AddLogCognitoSignUpVerification(CognitoConfirmSignupResponse cognitoConfirmSignupResponse, int storefrontFk, int customerFk,
                                            DateTime verifiedDate, string ipAddress, string response, int actionType, bool userRequested)
        {
            try
            {
                SqlParameter[] paramCollection = SqlHelperParameterCache.GetSpParameterSet(_sCon, "usp_AddLogCognitoSignUpVerification", true);

                paramCollection[1].Value = null; // logConsentId
                paramCollection[2].Value = null; // message

                paramCollection[3].Value = storefrontFk;
                paramCollection[4].Value = customerFk;

                paramCollection[5].Value = cognitoConfirmSignupResponse.AccessCode;
                paramCollection[6].Value = cognitoConfirmSignupResponse.AccessCodeVerified;

                paramCollection[7].Value = cognitoConfirmSignupResponse.HttpStatusCode;
                paramCollection[8].Value = response;
                paramCollection[9].Value = cognitoConfirmSignupResponse.Message == null ? String.Empty : cognitoConfirmSignupResponse.Message.Replace("'", "");
                paramCollection[10].Value = cognitoConfirmSignupResponse.Exception == null ? string.Empty : cognitoConfirmSignupResponse.Exception.Replace("'", ""); ;
                paramCollection[11].Value = ipAddress;
                paramCollection[12].Value = verifiedDate;
                paramCollection[13].Value = actionType;
                paramCollection[14].Value = userRequested;

                SqlHelper.ExecuteNonQuery(_sCon, CommandType.StoredProcedure, "usp_AddLogCognitoSignUpVerification", paramCollection);

                var value = paramCollection[1].Value;
                var message = paramCollection[2].Value;
                if (value != null)
                {
                    return (int)value;
                }
                else
                {
                    return 0;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public int AddLogCognitoSignupAuthSession(SrpAuthResponse srpAuthResponse, int storefrontFk, int customerFk,
                                           DateTime timeStamp, string ipAddress, string response)
        {
            try
            {
                SqlParameter[] paramCollection = SqlHelperParameterCache.GetSpParameterSet(_sCon, "usp_AddLogCognitoSignupAuthSession", true);

                paramCollection[1].Value = null; // logConsentId
                paramCollection[2].Value = null; // message

                paramCollection[3].Value = storefrontFk;
                paramCollection[4].Value = customerFk;

                paramCollection[5].Value = srpAuthResponse.CustomerEmail;
                paramCollection[6].Value = srpAuthResponse.SessionID;

                paramCollection[7].Value = timeStamp;
                paramCollection[8].Value = response;
                paramCollection[9].Value = srpAuthResponse.Message == null ? string.Empty : srpAuthResponse.Message.Replace("'", "");
                paramCollection[10].Value = srpAuthResponse.Exception == null ? string.Empty : srpAuthResponse.Exception.Replace("'", ""); ;
                paramCollection[11].Value = ipAddress;
                paramCollection[12].Value = timeStamp;

                SqlHelper.ExecuteNonQuery(_sCon, CommandType.StoredProcedure, "usp_AddLogCognitoSignupAuthSession", paramCollection);

                var value = paramCollection[1].Value;
                var message = paramCollection[2].Value;
                if (value != null)
                {
                    return (int)value;
                }
                else
                {
                    return 0;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public int AddLogCognitoAuthFlowMfaResponse(AuthFlowMfaResponse authFlowMfaResponse, int storefrontFk, int customerFk,
                                           string ipAddress, string response, DateTime dateCreated, int actionType)
        {
            try
            {
                SqlParameter[] paramCollection = SqlHelperParameterCache.GetSpParameterSet(_sCon, "usp_AddLogAuthFlowMfaResponse", true);

                paramCollection[1].Value = null; // logConsentId
                paramCollection[2].Value = null; // message

                paramCollection[3].Value = storefrontFk;
                paramCollection[4].Value = customerFk;

                paramCollection[5].Value = authFlowMfaResponse.AccessToken;
                paramCollection[6].Value = authFlowMfaResponse.UserId;
                paramCollection[7].Value = authFlowMfaResponse.IdToken;
                paramCollection[8].Value = authFlowMfaResponse.ExpiresIn;

                paramCollection[9].Value = response;
                paramCollection[10].Value = authFlowMfaResponse.Message == null ? string.Empty : authFlowMfaResponse.Message.Replace("'", "");
                paramCollection[11].Value = authFlowMfaResponse.Exception == null ? string.Empty : authFlowMfaResponse.Exception.Replace("'", ""); ;
                paramCollection[12].Value = ipAddress;
                paramCollection[13].Value = dateCreated;
                paramCollection[14].Value = actionType;

                SqlHelper.ExecuteNonQuery(_sCon, CommandType.StoredProcedure, "usp_AddLogAuthFlowMfaResponse", paramCollection);

                var value = paramCollection[1].Value;
                var message = paramCollection[2].Value;
                if (value != null)
                {
                    return (int)value;
                }
                else
                {
                    return 0;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public int UpdateCustomerCognitoUserId(int customerId, int cognitoUserId)
        {
            try
            {
                SqlParameter[] paramCollection = SqlHelperParameterCache.GetSpParameterSet(_sCon, "usp_updateCustomerCognitoUserId", true);

                paramCollection[1].Value = null; // UpdateStatus
                paramCollection[2].Value = null; // Message


                paramCollection[3].Value = customerId;
                paramCollection[4].Value = cognitoUserId;

                SqlHelper.ExecuteNonQuery(_sCon, CommandType.StoredProcedure, "usp_updateCustomerCognitoUserId", paramCollection);

                var updateStatus = Convert.ToInt32(paramCollection[1].Value.ToString());
                var message = paramCollection[2].Value.ToString();

                return updateStatus;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public int AddLogCognitoUserAttributesResponse(UserAttributeResponse userAttributeResponse, CustomerSignUp customerInfo, int storefrontFk, int customerFk,
                                          string ipAddress, string response, DateTime dateCreated, int actionType)
        {
            try
            {
                SqlParameter[] paramCollection = SqlHelperParameterCache.GetSpParameterSet(_sCon, "usp_AddLogCognitoUserAttributesResponse", true);

                paramCollection[1].Value = null; // logConsentId
                paramCollection[2].Value = null; // message

                paramCollection[3].Value = storefrontFk;
                paramCollection[4].Value = customerFk;

                paramCollection[5].Value = userAttributeResponse.HttpStatusCode;
                paramCollection[6].Value = userAttributeResponse.AccessToken;
                paramCollection[7].Value = customerInfo.CustomerAddress;
                paramCollection[8].Value = customerInfo.CustomerCity;
                paramCollection[9].Value = customerInfo.CustomerZipCode;

                paramCollection[10].Value = customerInfo.CustomerProvinceFk;

                paramCollection[11].Value = response;
                paramCollection[12].Value = userAttributeResponse.Message == null ? string.Empty : userAttributeResponse.Message.Replace("'", "");
                paramCollection[13].Value = userAttributeResponse.Exception == null ? string.Empty : userAttributeResponse.Exception.Replace("'", ""); ;
                paramCollection[14].Value = ipAddress;
                paramCollection[15].Value = dateCreated;
                paramCollection[16].Value = actionType;

                SqlHelper.ExecuteNonQuery(_sCon, CommandType.StoredProcedure, "usp_AddLogCognitoUserAttributesResponse", paramCollection);

                var value = paramCollection[1].Value;
                var message = paramCollection[2].Value;
                if (value != null)
                {
                    return (int)value;
                }
                else
                {
                    return 0;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public int AddLogCognitoResetPassword(CognitoSignupResponse cognitoResponse, int storefrontFk, string customerEmail, string resetCode,
                                          string ipAddress, string response, DateTime dateCreated, int actionType)
        {
            try
            {
                SqlParameter[] paramCollection = SqlHelperParameterCache.GetSpParameterSet(_sCon, "usp_AddLogCognitoResetPassword", true);

                paramCollection[1].Value = null; // logConsentId
                paramCollection[2].Value = null; // message

                paramCollection[3].Value = storefrontFk;
                paramCollection[4].Value = customerEmail;

                paramCollection[5].Value = cognitoResponse.HttpStatusCode;
                paramCollection[6].Value = resetCode;

                paramCollection[7].Value = response;
                paramCollection[8].Value = cognitoResponse.Message == null ? string.Empty : cognitoResponse.Message.Replace("'", "");
                paramCollection[9].Value = cognitoResponse.Exception == null ? string.Empty : cognitoResponse.Exception.Replace("'", "");
                paramCollection[10].Value = ipAddress;
                paramCollection[11].Value = dateCreated;
                paramCollection[12].Value = actionType;

                SqlHelper.ExecuteNonQuery(_sCon, CommandType.StoredProcedure, "usp_AddLogCognitoResetPassword", paramCollection);

                var value = paramCollection[1].Value;
                var message = paramCollection[2].Value;
                if (value != null)
                {
                    return (int)value;
                }
                else
                {
                    return 0;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public int AddLogCognitoSendEmailVerification(CognitoSignupResponse cognitoResponse, int storefrontFk, string customerEmail,
                                         string ipAddress, string response, DateTime dateCreated, int actionType, bool userRequested)
        {
            try
            {
                SqlParameter[] paramCollection = SqlHelperParameterCache.GetSpParameterSet(_sCon, "usp_AddLogCognitoSendEmailVerification", true);

                paramCollection[1].Value = null; // logConsentId
                paramCollection[2].Value = null; // message

                paramCollection[3].Value = storefrontFk;
                paramCollection[4].Value = customerEmail;

                paramCollection[5].Value = cognitoResponse.HttpStatusCode;

                paramCollection[6].Value = response;
                paramCollection[7].Value = cognitoResponse.Message == null ? string.Empty : cognitoResponse.Message.Replace("'", "");
                paramCollection[8].Value = cognitoResponse.Exception == null ? string.Empty : cognitoResponse.Exception.Replace("'", "");
                paramCollection[9].Value = ipAddress;
                paramCollection[10].Value = dateCreated;
                paramCollection[11].Value = actionType;
                paramCollection[12].Value = userRequested;

                SqlHelper.ExecuteNonQuery(_sCon, CommandType.StoredProcedure, "usp_AddLogCognitoSendEmailVerification", paramCollection);

                var value = paramCollection[1].Value;
                var message = paramCollection[2].Value;
                if (value != null)
                {
                    return (int)value;
                }
                else
                {
                    return 0;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public DataSet GetConsultationHours(int customerfk)
        {
            try
            {
                _ds = SqlHelper.ExecuteDataset(_sCon, "usp_GetConsultationHours", customerfk);
            }
            catch (Exception ex)
            {
                _ds = null;
                throw ex;
            }
            return _ds;
        }
        public int AddPatientConsultationHours(int consultationHoursFk, int customerFk, bool active, DateTime startDate, DateTime? endDate)
        {
            try
            {
                SqlParameter[] paramCollection = SqlHelperParameterCache.GetSpParameterSet(_sCon, "usp_AddPatientConsultationDetails", true);

                paramCollection[1].Value = null; // Id
                paramCollection[2].Value = null; // message

                paramCollection[3].Value = consultationHoursFk;
                paramCollection[4].Value = customerFk;

                paramCollection[5].Value = active;
                paramCollection[6].Value = startDate;
                paramCollection[7].Value = endDate;

                SqlHelper.ExecuteNonQuery(_sCon, CommandType.StoredProcedure, "usp_AddPatientConsultationDetails", paramCollection);

                var value = paramCollection[1].Value;
                var message = paramCollection[2].Value;
                if (value != null)
                {
                    return (int)value;
                }
                else
                {
                    return 0;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public DataSet GetPatientPreferedConsultationHour(int patientConsultationDetailsId, int consultationHoursFk, int customerFk)
        {
            try
            {
                _ds = SqlHelper.ExecuteDataset(_sCon, "usp_GetPatientPreferedConsultationHour", patientConsultationDetailsId, consultationHoursFk, customerFk);
            }
            catch (Exception ex)
            {
                _ds = null;
                throw ex;
            }
            return _ds;
        }

        public int InActivatePatientConsultationHours(int customerId, string patientConsultationHoursIds, DateTime endDate)
        {
            try
            {
                SqlParameter[] paramCollection = SqlHelperParameterCache.GetSpParameterSet(_sCon, "usp_InActivatePatientConsultationHours", true);

                paramCollection[1].Value = customerId;
                paramCollection[2].Value = patientConsultationHoursIds;
                paramCollection[3].Value = endDate;
                paramCollection[4].Value = null; // UpdateStatus
                paramCollection[5].Value = null; // Message

                SqlHelper.ExecuteNonQuery(_sCon, CommandType.StoredProcedure, "usp_InActivatePatientConsultationHours", paramCollection);

                var updateStatus = Convert.ToInt32(paramCollection[4].Value.ToString());
                var message = paramCollection[5].Value.ToString();

                return updateStatus;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DataSet GetSocialHistoryList()
        {
            try
            {
                _ds = SqlHelper.ExecuteDataset(_sCon, "usp_getSocialHistory");
            }
            catch (Exception ex)
            {
                _ds = null;
                throw ex;
            }
            return _ds;
        }

        public int UpdateCustomerSignUpEmailVerifiedFlag(int customerId, bool emailVerified)
        {
            try
            {
                SqlParameter[] paramCollection = SqlHelperParameterCache.GetSpParameterSet(_sCon, "usp_UpdateCustomerSignupEmailVerifiedFlag", true);

                paramCollection[1].Value = customerId;
                paramCollection[2].Value = emailVerified;
                paramCollection[3].Value = null; // update status
                paramCollection[4].Value = null; // message

                SqlHelper.ExecuteNonQuery(_sCon, CommandType.StoredProcedure, "usp_UpdateCustomerSignupEmailVerifiedFlag", paramCollection);

                var value = paramCollection[3].Value;
                if (value != null)
                {
                    return Convert.ToInt32(value.ToString());
                }
                else
                {
                    return 0;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public DataSet GetDeveloperLogin(string email, int storeFrontId)
        {
            try
            {
                _ds = SqlHelper.ExecuteDataset(_sCon, "usp_IsDeveloper", email, storeFrontId);


            }
            catch (Exception ex)
            {
                _ds = null;
                throw ex;
            }
            return _ds;
        }

        public DataSet GetCustomerShippingAddress(int custometFk, int shippingAddressFk)
        {
            try
            {
                _ds = SqlHelper.ExecuteDataset(_sCon, "usp_getCustomerShippingAddresses", custometFk, shippingAddressFk);
            }
            catch (Exception ex)
            {
                _ds = null;
                throw ex;
            }
            return _ds;
        }

        public int AddCustomerShippingAddress(int customerFk, string customerFirstName, string customerLastName, 
                                             string customerPhone,
                                             string shippingAddress, string shippingCity,
                                            int countryFk, int provinceFk, 
                                            string zipCode, bool addressActive, bool defaultAddress, DateTime dateCreated)
        {
            try
            {
                SqlParameter[] paramCollection = SqlHelperParameterCache.GetSpParameterSet(_sCon, "usp_addCustomerShippingAddress", true);

                paramCollection[1].Value = null; // customerAddressId
                paramCollection[2].Value = null; // message


                paramCollection[3].Value = customerFk;
                paramCollection[4].Value = customerFirstName;
                paramCollection[5].Value = customerLastName;
                paramCollection[6].Value = string.IsNullOrEmpty(customerPhone)?string.Empty : customerPhone;
                paramCollection[7].Value = shippingAddress;
                paramCollection[8].Value = shippingCity;
                paramCollection[9].Value = countryFk;
                paramCollection[10].Value = provinceFk;
                paramCollection[11].Value = zipCode;
                paramCollection[12].Value = addressActive;
                paramCollection[13].Value = defaultAddress;
                paramCollection[14].Value = dateCreated;
               

                SqlHelper.ExecuteNonQuery(_sCon, CommandType.StoredProcedure, "usp_addCustomerShippingAddress", paramCollection);

                var value = paramCollection[1].Value;

                if (value != null)
                {
                    return (int)value;
                }
                else
                {
                    return 0;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public int UpdateShippingAddress(int shippingAdressFk, string firstName, string lastName,
                string address, string city, string zipcode, int provinceFk, int countryFk)
        {
            try
            {
                SqlParameter[] paramCollection = SqlHelperParameterCache.GetSpParameterSet(_sCon, "usp_updateCustomerShippingAddress", true);

                paramCollection[1].Value = null; // update status
                paramCollection[2].Value = null; // message

                paramCollection[3].Value = shippingAdressFk;
                paramCollection[4].Value = firstName;
                paramCollection[5].Value = lastName;
                paramCollection[6].Value = address;
                paramCollection[7].Value = city;
                paramCollection[8].Value = countryFk;
                paramCollection[9].Value = provinceFk;
                paramCollection[10].Value = zipcode;

                SqlHelper.ExecuteNonQuery(_sCon, CommandType.StoredProcedure, "usp_updateCustomerShippingAddress", paramCollection);

                var value = paramCollection[1].Value;
                if (value != null)
                {
                    return Convert.ToInt32(value.ToString());
                }
                else
                {
                    return 0;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public int UpdateCustomerShippingAddressDefaultFlag(int customerId, int customerShippingAddressId, bool isDefault)
        {
            try
            {
                SqlParameter[] paramCollection = SqlHelperParameterCache.GetSpParameterSet(_sCon, "usp_updateCustomerShippingAddressDefaultFlag", true);

                paramCollection[1].Value = null; // update status
                paramCollection[2].Value = null; // message

                paramCollection[3].Value = customerId;
                paramCollection[4].Value = customerShippingAddressId;
                paramCollection[5].Value = isDefault;
               

                SqlHelper.ExecuteNonQuery(_sCon, CommandType.StoredProcedure, "usp_updateCustomerShippingAddressDefaultFlag", paramCollection);

                var value = paramCollection[1].Value;
                if (value != null)
                {
                    return Convert.ToInt32(value.ToString());
                }
                else
                {
                    return 0;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public int UpdateCustomerShippingAddressActiveFlag(int customerId, int customerShippingAddressId, bool isActive)
        {
            try
            {
                SqlParameter[] paramCollection = SqlHelperParameterCache.GetSpParameterSet(_sCon, "usp_updateCustomerShippingAddressActiveFlag", true);

                paramCollection[1].Value = null; // update status
                paramCollection[2].Value = null; // message

                paramCollection[3].Value = customerId;
                paramCollection[4].Value = customerShippingAddressId;
                paramCollection[5].Value = isActive;


                SqlHelper.ExecuteNonQuery(_sCon, CommandType.StoredProcedure, "usp_updateCustomerShippingAddressActiveFlag", paramCollection);

                var value = paramCollection[1].Value;
                if (value != null)
                {
                    return Convert.ToInt32(value.ToString());
                }
                else
                {
                    return 0;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public int UpdateCustomerCredentials(int customerId, string email, string phone)
        {
            try
            {
                SqlParameter[] paramCollection = SqlHelperParameterCache.GetSpParameterSet(_sCon, "usp_updateCustomerCredentials", true);

                paramCollection[1].Value = null; // update status
                paramCollection[2].Value = null; // message

                paramCollection[3].Value = customerId;
                paramCollection[4].Value = email;
                paramCollection[5].Value = phone;
               

                SqlHelper.ExecuteNonQuery(_sCon, CommandType.StoredProcedure, "usp_updateCustomerCredentials", paramCollection);

                var value = paramCollection[1].Value;
                if (value != null)
                {
                    return Convert.ToInt32(value.ToString());
                }
                else
                {
                    return 0;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        // Add Log of customer shipping address for changed fields by passing a datatable
        public int AddLogCustomerShippingAddress(List<LogCustomerShippingAddress> logCustomerShippingAddress)
        {
            // create a datatable to be passed as parameter to stored procedure
            DataTable tblLogCustomerShippingAddress= new DataTable();

            tblLogCustomerShippingAddress.Columns.Add("LogCustomerShippingAddress_Customer_Fk", typeof(int));
            tblLogCustomerShippingAddress.Columns.Add("LogCustomerShippingAddress_CustomerShippingAddress_Fk", typeof(int));
            tblLogCustomerShippingAddress.Columns.Add("LogCustomerShippingAddress_ShippingAddressField_Fk", typeof(int));
            tblLogCustomerShippingAddress.Columns.Add("LogCustomerShippingAddress_UpdatedFieldName", typeof(string));
            tblLogCustomerShippingAddress.Columns.Add("LogCustomerShippingAddress_UpdatedField_OriginalValue", typeof(string));
            tblLogCustomerShippingAddress.Columns.Add("LogCustomerShippingAddress_UpdatedField_ModifiedValue", typeof(string));
            tblLogCustomerShippingAddress.Columns.Add("LogCustomerShippingAddress_ActionType", typeof(string));
            tblLogCustomerShippingAddress.Columns.Add("LogCustomerShippingAddress_DateCreated", typeof(DateTime));            


            foreach (var customerShippAddress in logCustomerShippingAddress)
            {
                DataRow row = tblLogCustomerShippingAddress.NewRow();
                row["LogCustomerShippingAddress_Customer_Fk"] = customerShippAddress.CustomerFk;
                row["LogCustomerShippingAddress_CustomerShippingAddress_Fk"] = customerShippAddress.CustomerShippingAddressFk;
                row["LogCustomerShippingAddress_ShippingAddressField_Fk"] = customerShippAddress.LogUpdatedFieldFk;
                row["LogCustomerShippingAddress_UpdatedFieldName"] = customerShippAddress.LogUpdatedFieldName;
                row["LogCustomerShippingAddress_UpdatedField_OriginalValue"] = customerShippAddress.LogUpdatedFieldOriginalValue;
                row["LogCustomerShippingAddress_UpdatedField_ModifiedValue"] = customerShippAddress.LogUpdatedFieldModifiedValue;
                row["LogCustomerShippingAddress_ActionType"] = customerShippAddress.LogActionType;
                row["LogCustomerShippingAddress_DateCreated"] = customerShippAddress.LogDateCreated;
                

                tblLogCustomerShippingAddress.Rows.Add(row);
            }

            SqlParameter[] paramCollection = SqlHelperParameterCache.GetSpParameterSet(_sCon, "usp_AddLogCustomerShippingAddress", true);

            paramCollection[1].TypeName = "LogCustomerShippingAddressType";
            paramCollection[1].Value = tblLogCustomerShippingAddress;
            paramCollection[2].Value = null; // UpdateStatus
            paramCollection[3].Value = null; // Message

            SqlHelper.ExecuteNonQuery(_sCon, CommandType.StoredProcedure, "usp_AddLogCustomerShippingAddress", paramCollection);

            int uStatus = (int)paramCollection[2].Value;

            var uMsg = paramCollection[3].Value;

            if (uMsg != null)
            {
                var message = uMsg.ToString();
            }

            return uStatus;
        }

        // get Cart Details for OrderId
        public DataSet GetCustomerOrderCartByShipInvoice(int customerId, int customerStorefrontId, int shipInvoiceId)
        {
            try
            {
                _ds = SqlHelper.ExecuteDataset(_sCon, "usp_getCustomerOrderCart_ByShippingInvoiceId", customerId, customerStorefrontId, shipInvoiceId);
            }
            catch (Exception ex)
            {
                _ds = null;
                throw ex;
            }
            return _ds;
        }
    }
}
