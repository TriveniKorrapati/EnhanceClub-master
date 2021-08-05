using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using EnhanceClub.Domain.Abstract;
using EnhanceClub.Domain.AwsEntities;
using EnhanceClub.Domain.Entities;
using EnhanceClub.Domain.Helpers;

// Created by Rajiv S : 26 Mar 2020

//-- This implementation provides data based on sql stored procedures--//
//--  This is to be used during development and to be replaced with Rest based repository 

namespace EnhanceClub.Domain.Concrete
{
    public class CustomerRepositorySql : ICustomerRepository
    {
        private readonly CustomerDbLayer _customerDbl = new CustomerDbLayer();

        // get List of customers
        public IEnumerable<Customer> Customers
        {
            get
            {
                return ConvertToCustomerList(_customerDbl.GetCustomerById(0));
            }
        }

        // validate login 
        public DataSet ValidateLogin(string email, string password, int storeFrontId)
        {

            DataSet ds = _customerDbl.GetCustomerLogin(email, password, storeFrontId);
            return ds;
        }

        public DataSet ValidateEmailLogin(string email, int storeFrontId)
        {
            DataSet ds = _customerDbl.GetCustomerLogin(email, storeFrontId);
            return ds;
        }

        // this method adds customer sign up first step
        public int AddCustomerPartial(CustomerSignUp customerSignUpInfo)
        {
            // Create a string of sql parameters passed to stored procedure
            StringBuilder logUserActionParameters = new StringBuilder();
            logUserActionParameters.Append("{ ");
            logUserActionParameters.Append(" CustomerLastIp:");
            logUserActionParameters.Append(customerSignUpInfo.CustomerLastIp);

            logUserActionParameters.Append(", CustomerAffiliateId:");
            logUserActionParameters.Append(customerSignUpInfo.CustomerAffiliateId);

            logUserActionParameters.Append(", CustomerIsPartial:");
            logUserActionParameters.Append(customerSignUpInfo.CustomerIsPartial);

            logUserActionParameters.Append(", CustomerEmail:");
            logUserActionParameters.Append(customerSignUpInfo.CustomerEmail);

            logUserActionParameters.Append(", CustomerPassword:");
            logUserActionParameters.Append(customerSignUpInfo.CustomerPassword);

            logUserActionParameters.Append(", CustomerActive:");
            logUserActionParameters.Append(customerSignUpInfo.CustomerActive);

            logUserActionParameters.Append(", CustomerTestAccount:");
            logUserActionParameters.Append(customerSignUpInfo.CustomerTestAccount);

            logUserActionParameters.Append(", CustomerDateCreated:");
            logUserActionParameters.Append(customerSignUpInfo.CustomerDateCreated);

            logUserActionParameters.Append(", CustomerLastModified:");
            logUserActionParameters.Append(customerSignUpInfo.CustomerLastModified);

            logUserActionParameters.Append(", CustomerLastLoginDate:");
            logUserActionParameters.Append(customerSignUpInfo.CustomerLastLoginDate);

            logUserActionParameters.Append(", TeleHealthTerms:");
            logUserActionParameters.Append(customerSignUpInfo.TeleHealthTerms);



            logUserActionParameters.Append(" }");

            int newCustomerId = _customerDbl.AddCustomerPartial(customerSignUpInfo.CustomerLastIp,
                                                                       customerSignUpInfo.CustomerAffiliateId,
                                                                       customerSignUpInfo.CustomerIsPartial,
                                                                       customerSignUpInfo.TeleHealthTerms,
                                                                       customerSignUpInfo.Terms,
                                                                       customerSignUpInfo.CustomerCountryFk,
                                                                       customerSignUpInfo.CustomerProvinceFk,
                                                                       customerSignUpInfo.CustomerAddress,
                                                                       customerSignUpInfo.CustomerCity,
                                                                       customerSignUpInfo.CustomerFirstName,
                                                                       customerSignUpInfo.CustomerLastName,
                                                                       customerSignUpInfo.CustomerFreeShipping,
                                                                       customerSignUpInfo.CustomerFreeShippingOneYear,
                                                                       customerSignUpInfo.CustomerEmail,
                                                                       customerSignUpInfo.CustomerPassword,
                                                                       customerSignUpInfo.CustomerActive,
                                                                       customerSignUpInfo.CustomerTestAccount,
                                                                       customerSignUpInfo.CustomerDateCreated,
                                                                       customerSignUpInfo.CustomerLastModified,
                                                                       customerSignUpInfo.CustomerLastLoginDate,
                                                                       logUserActionParameters.ToString(),
                                                                       customerSignUpInfo.CognitoUserSub,
                                                                       customerSignUpInfo.CustomerPhoneWithCountryCode,
                                                                       customerSignUpInfo.CustomerTypedEmail
                                                                       );
            return newCustomerId;
        }

        // update step 2 info for customer
        public int UpdateCustomerPartial(CustomerSignUp customerInfo)
        {
            StringBuilder logUserActionParameters = new StringBuilder();
            logUserActionParameters.Append("{ ");

            logUserActionParameters.Append("CustomerLastIp:");
            logUserActionParameters.Append(customerInfo.CustomerLastIp);

            logUserActionParameters.Append(" CustomerFirstName:");
            logUserActionParameters.Append(customerInfo.CustomerFirstName);

            logUserActionParameters.Append(", CustomerLastName:");
            logUserActionParameters.Append(customerInfo.CustomerLastName);

            logUserActionParameters.Append(", CustomerAddress:");
            logUserActionParameters.Append(customerInfo.CustomerAddress);

            logUserActionParameters.Append(", CustomerCity:");
            logUserActionParameters.Append(customerInfo.CustomerCity);

            logUserActionParameters.Append(", CustomerProvinceId:");
            logUserActionParameters.Append(customerInfo.CustomerProvinceFk);

            logUserActionParameters.Append(", CustomerZipCode:");
            logUserActionParameters.Append(customerInfo.CustomerZipCode);

            logUserActionParameters.Append(", CustomerPhone:");
            logUserActionParameters.Append(customerInfo.CustomerPhone);

            logUserActionParameters.Append(" }");

            int updateCustomerStatus = _customerDbl.UpdateCustomerPartial(customerInfo.CustomerId,
                customerInfo.CustomerIsPartial,
                customerInfo.CustomerEmail,
                customerInfo.CustomerFirstName,
                customerInfo.CustomerLastName,
                customerInfo.CustomerAddress,
                customerInfo.CustomerCity,
                customerInfo.CustomerProvinceFk,
                customerInfo.CustomerZipCode,
                customerInfo.CustomerPhone,
                customerInfo.CustomerLastIp,
                logUserActionParameters.ToString()

                );

            return updateCustomerStatus;
        }


        // get customer credit
        public decimal GetCustomerCredit(int customerId)
        {
            return _customerDbl.GetCustomerCredit(customerId);
        }

        public CustomerCredit GetCustomerCreditWithOrderCount(int customerId)
        {
            DataSet dsCreditInfo = _customerDbl.GetCustomerCreditWithOrderCount(customerId);
            CustomerCredit creditInfo = null;

            if (dsCreditInfo.Tables[0].Rows.Count > 0)
            {
                creditInfo =
                    dsCreditInfo.Tables[0].AsEnumerable()
                        .Select(row => new CustomerCredit
                        {
                            OrderCount = Convert.IsDBNull(row.Field<int>("OrderCount")) ? 0 : row.Field<int>("OrderCount"),
                            CreditAmount = Convert.IsDBNull(row.Field<decimal>("CustomerCreditBalance")) ? 0 : row.Field<decimal>("CustomerCreditBalance"),

                        }).ToList().First();

            }

            return creditInfo;
        }


        // get customer information for update account
        public CustomerMinimal GetCustomerInfo(int customerId)
        {
            DataSet dsCustomer = _customerDbl.GetCustomerInfo_ById(customerId);

            if (dsCustomer.Tables[0].Rows.Count > 0)
            {
                return new CustomerMinimal
                {
                    CustomerId = Convert.ToInt32(dsCustomer.Tables[0].Rows[0]["Customer_Id"]),
                    CustomerFirstName = dsCustomer.Tables[0].Rows[0]["Customer_FirstName"].ToString(),
                    CustomerLastName = dsCustomer.Tables[0].Rows[0]["Customer_LastName"].ToString(),
                    CustomerEmail = dsCustomer.Tables[0].Rows[0]["Customer_Email"].ToString(),
                    CustomerPassword = dsCustomer.Tables[0].Rows[0]["Customer_Password"].ToString(),
                    CustomerPhone = dsCustomer.Tables[0].Rows[0]["Customer_Phone"].ToString(),
                    CustomerEveningPhone = dsCustomer.Tables[0].Rows[0]["Customer_EveningPhone"].ToString(),
                    CustomerAddress = dsCustomer.Tables[0].Rows[0]["Customer_Address"].ToString(),
                    CustomerCity = dsCustomer.Tables[0].Rows[0]["Customer_City"].ToString(),
                    CustomerCountryId = Convert.ToInt32(dsCustomer.Tables[0].Rows[0]["Customer_Country_Fk"]),
                    BillingCountryName = dsCustomer.Tables[0].Rows[0]["Customer_CountryName"].ToString(),
                    CustomerProvinceId = Convert.ToInt32(dsCustomer.Tables[0].Rows[0]["Customer_ProvinceState_Fk"]),
                    BillingProvinceName = dsCustomer.Tables[0].Rows[0]["Customer_ProvinceName"].ToString(),
                    CustomerZipCode = dsCustomer.Tables[0].Rows[0]["Customer_ZipCode"].ToString(),
                    CustomerFreeShipping = (bool)dsCustomer.Tables[0].Rows[0]["Customer_FreeShipping"],
                    CustomerFreeShippingOneYear = (bool)dsCustomer.Tables[0].Rows[0]["Customer_FreeShippingOneYear"],
                    CustomerFreeShippingStartDate = Convert.IsDBNull(dsCustomer.Tables[0].Rows[0]["Customer_FreeShippingStartDate"]) ? DateTime.MinValue : (DateTime)dsCustomer.Tables[0].Rows[0]["Customer_FreeShippingStartDate"],
                    CustomerCountryName = Convert.IsDBNull(dsCustomer.Tables[0].Rows[0]["Country_Name"]) ? string.Empty : dsCustomer.Tables[0].Rows[0]["Country_Name"].ToString(),
                    CustomerTypedEmail = Convert.IsDBNull(dsCustomer.Tables[0].Rows[0]["Customer_TypedEmail"]) ? string.Empty : dsCustomer.Tables[0].Rows[0]["Customer_TypedEmail"].ToString(),
                    CustomerProvinceCode = Convert.IsDBNull(dsCustomer.Tables[0].Rows[0]["Provincestate_Code"]) ? string.Empty : dsCustomer.Tables[0].Rows[0]["Provincestate_Code"].ToString()

                };
            }
            else
            {
                return null;
            }

        }

        // get information for referring customer 
        public CustomerMinimal GetReferredCustomerInfo(int referringCustomerId, int customerId)
        {
            DataSet dsCustomer = _customerDbl.GetReferredCustomerInfo(referringCustomerId, customerId);

            if (dsCustomer.Tables[0].Rows.Count > 0)
            {
                return new CustomerMinimal
                {
                    CustomerId = Convert.ToInt32(dsCustomer.Tables[0].Rows[0]["Customer_Id"]),
                    CustomerFirstName = dsCustomer.Tables[0].Rows[0]["Customer_FirstName"].ToString(),
                    CustomerLastName = dsCustomer.Tables[0].Rows[0]["Customer_LastName"].ToString(),
                    CustomerEmail = dsCustomer.Tables[0].Rows[0]["Customer_Email"].ToString(),
                    CustomerAffiliateFk = Convert.ToInt32(dsCustomer.Tables[0].Rows[0]["Customer_Affiliate_Fk"])
                };
            }
            else
            {
                return null;
            }
        }

        // get list of province
        public List<Province> GetProvinceListByCountry(int countryId)
        {
            DataSet dsProvinceList = _customerDbl.GetProvinceListByCountry(countryId);
            List<Province> provinceList = null;
            if (dsProvinceList.Tables[0].Rows.Count > 0)
            {
                provinceList =
                   dsProvinceList.Tables[0].AsEnumerable()
                   .Select(row => new Province
                   {
                       ProvinceId = row.Field<int>("ProvinceState_Id"),
                       ProvinceName = row.Field<string>("ProvinceState_Name"),
                       ProvinceStateCode = row.Field<string>("ProvinceState_Code"),
                       CountryId = row.Field<int>("ProvinceState_Country_Fk")
                   }).ToList();
            }
            return provinceList;
        }

        // get province by Id
        public Province GetProvinceById(int shippingProvinceId)
        {
            DataSet dsProvinceList = _customerDbl.GetProvinceById(shippingProvinceId);
            Province province = null;
            if (dsProvinceList.Tables[0].Rows.Count > 0)
            {
                province =
                   dsProvinceList.Tables[0].AsEnumerable()
                   .Select(row => new Province
                   {
                       ProvinceId = row.Field<int>("ProvinceState_Id"),
                       ProvinceName = row.Field<string>("ProvinceState_Name"),
                       ProvinceStateCode = row.Field<string>("ProvinceState_Code"),
                       CountryId = row.Field<int>("ProvinceState_Country_Fk")
                   }).ToList().FirstOrDefault();
            }
            return province;
        }

        // get country by Id
        public Country GetCountryById(int shippingCountryId)
        {
            DataSet dsCountryList = _customerDbl.GetCountryById(shippingCountryId);
            Country country = null;
            if (dsCountryList.Tables[0].Rows.Count > 0)
            {
                country =
                   dsCountryList.Tables[0].AsEnumerable()
                   .Select(row => new Country
                   {
                       CountryId = row.Field<int>("Country_Id"),
                       CountryName = row.Field<string>("Country_Name"),
                       CountryCode = row.Field<string>("Country_Code"),

                   }).ToList().FirstOrDefault();
            }
            return country;
        }

        // check if email exists at signup
        public bool EmailAlreadyExits(string customerEmail, int affiliateId)
        {
            return _customerDbl.IsEmailAlreadyRegistered(customerEmail, affiliateId);
        }

        // remove patient profile
        public void DelProfile(int patientProfileId, int customerId)
        {
            _customerDbl.DelProfile(patientProfileId, customerId);
        }

        // add customer email to PromoEmails table
        public int AddPromoEmail(int emailTypeFk, string customerEmail, string clientIp, string storefrontId, bool isActive, DateTime dateAdded)
        {
            return _customerDbl.AddPromoEmail(emailTypeFk, customerEmail, clientIp, storefrontId, isActive, dateAdded);

        }

        // unsubscribe customer from news letter 
        public int UnsubscribeNewLetter(int promoIdDecoded)
        {
            int updateStatus = _customerDbl.UnsubscribeNewLetter(promoIdDecoded);
            return updateStatus;
        }



        // get country list or single country
        public IEnumerable<Country> GetCountry(int countryId)
        {
            DataSet dsCountryList = _customerDbl.GetCountry(countryId);
            List<Country> countryList = null;

            if (dsCountryList.Tables[0].Rows.Count > 0)
            {
                countryList =
                   dsCountryList.Tables[0].AsEnumerable()
                   .Select(row => new Country
                   {

                       CountryId = row.Field<int>("Country_Id"),
                       CountryCode = row.Field<string>("Country_Code"),
                       CountryName = row.Field<string>("Country_Name"),
                       CountryActive = row.Field<object>("Country_Active") != DBNull.Value && Convert.ToBoolean(row.Field<object>("Country_Active"))

                   }).ToList();
            }
            return countryList;
        }

        // return customer Id of Referring Customer 
        public int GetReferredBy(int customerId)
        {
            return _customerDbl.GetReferredBy(customerId);
        }

        // return order count of the customer
        public int GetCustomerOrderCount(int customerId)
        {
            return _customerDbl.GetCustomerOrderCount(customerId);
        }

        // check if credit is already issued
        public bool IsCreditAlreadyIssued(int customerId)
        {
            return _customerDbl.IsCreditAlreadyIssued(customerId);
        }

        // Add Refer credit to both referer and referred customer
        public int AddReferCreditToBoth(int referringCustomerId,
                                       decimal refererCredit,
                                       string referringComment,
                                       int customerId,
                                       decimal referredCredit,
                                       string referredComment,
                                       int referCreditTransactionType,
                                       DateTime dateCreated,
                                       int orderInvoiceId)
        {
            return _customerDbl.AddReferCreditToBoth(
                                                     referringCustomerId,
                                                     referredCredit,
                                                     referringComment,
                                                     customerId,
                                                     referredCredit,
                                                     referredComment,
                                                     referCreditTransactionType,
                                                     dateCreated,
                                                     orderInvoiceId);

        }


        // update customer info
        public int UpdateCustomerInfo(CustomerMinimal customerInfo)
        {
            StringBuilder logUserActionParameters = new StringBuilder();
            logUserActionParameters.Append("{ ");

            logUserActionParameters.Append("CustomerLastIp:");
            logUserActionParameters.Append(customerInfo.CustomerLastIp);

            logUserActionParameters.Append(" CustomerFirstName:");
            logUserActionParameters.Append(customerInfo.CustomerFirstName);

            logUserActionParameters.Append(", CustomerLastName:");
            logUserActionParameters.Append(customerInfo.CustomerLastName);

            logUserActionParameters.Append(", CustomerEmail:");
            logUserActionParameters.Append(customerInfo.CustomerEmail);

            logUserActionParameters.Append(", CustomerPassword:");
            logUserActionParameters.Append(customerInfo.CustomerPassword);

            logUserActionParameters.Append(", CustomerPhone:");
            logUserActionParameters.Append(customerInfo.CustomerPhone);

            logUserActionParameters.Append(", CustomerEveningPhone:");
            logUserActionParameters.Append(customerInfo.CustomerEveningPhone);

            logUserActionParameters.Append(", CustomerCellPhone:");
            logUserActionParameters.Append(customerInfo.CustomerCellPhone);

            logUserActionParameters.Append(", CustomerAddress:");
            logUserActionParameters.Append(customerInfo.CustomerAddress);

            logUserActionParameters.Append(", CustomerCity:");
            logUserActionParameters.Append(customerInfo.CustomerCity);

            logUserActionParameters.Append(", CustomerCountryId:");
            logUserActionParameters.Append(customerInfo.CustomerCountryId);

            logUserActionParameters.Append(", CustomerProvinceId:");
            logUserActionParameters.Append(customerInfo.CustomerProvinceId);

            logUserActionParameters.Append(", CustomerZipCode:");
            logUserActionParameters.Append(customerInfo.CustomerZipCode);

            logUserActionParameters.Append(" }");


            int updateCustomerStatus = _customerDbl.UpdateCustomerInfo(customerInfo.CustomerId,
                customerInfo.CustomerFirstName,
                customerInfo.CustomerLastName,
                customerInfo.CustomerEmail,
                customerInfo.CustomerPassword,
                customerInfo.CustomerPhone,
                customerInfo.CustomerEveningPhone,
                customerInfo.CustomerCellPhone,
                customerInfo.CustomerAddress,
                customerInfo.CustomerCity,
                customerInfo.CustomerCountryId,
                customerInfo.CustomerProvinceId,
                customerInfo.CustomerZipCode,
                logUserActionParameters.ToString(),
                customerInfo.CustomerLastIp
                );

            return updateCustomerStatus;
        }
        // Get List of customers credit transactions
        public List<CreditTransaction> GetCustomerCreditTransactions(int customerId)
        {
            DataSet dsCreditTransaction = _customerDbl.GetCustomerCreditTransactions(customerId);

            List<CreditTransaction> creditTransactionList = null;

            if (dsCreditTransaction.Tables[0].Rows.Count > 0)
            {
                creditTransactionList =
                   dsCreditTransaction.Tables[0].AsEnumerable()
                   .Select(row => new CreditTransaction
                   {
                       CreditTransactionDate = row.Field<DateTime>("CustomerTransaction_DatePaid"),
                       CreditTransactionOrder = Convert.IsDBNull(row.Field<object>("CustomerTransaction_OrderInvoice_Fk")) ? 0 : Convert.ToInt32(row.Field<object>("CustomerTransaction_OrderInvoice_Fk")),
                       CreditTransactionComments = row.Field<string>("CustomerTransaction_Comment"),
                       CreditTransactionAmount = row.Field<decimal>("CustomerTransaction_Amount"),
                       CreditTransactionUserAdmin = Convert.IsDBNull(row.Field<object>("CustomerTransaction_Useradmin_fk")) ? 0 : Convert.ToInt32(row.Field<object>("CustomerTransaction_Useradmin_fk"))
                       //CreditTransactionUserAdmin = row.Field<int>("CustomerTransaction_Useradmin_fk") 

                   }).ToList();

            }
            return creditTransactionList;
        }

        // Get List of products ordered 
        public List<ProductsOrdered> GetCustomerProductsOrdered(int customerId, int productTypeFk, int customerStorefrontId)
        {
            DataSet dsProductsOrdered = _customerDbl.GetCustomerProductSizeOrdered(customerId, productTypeFk, customerStorefrontId);

            List<ProductsOrdered> productsOrderedList = null;

            if (dsProductsOrdered.Tables[0].Rows.Count > 0)
            {
                productsOrderedList =
                   dsProductsOrdered.Tables[0].AsEnumerable()
                   .Select(row => new ProductsOrdered
                   {
                       CartProductSizeFk = row.Field<int>("cart_productsize_fk"),
                       ProductStoreFrontRealName = row.Field<string>("product_storefront_realname"),
                       ProductId = row.Field<int>("product_id"),
                       ProductName = row.Field<string>("product_name"),
                       ProductActive = row.Field<bool>("product_active"),
                       ProductSizeId = row.Field<int>("productsize_id"),
                       ProductSizeQty = row.Field<decimal>("productsize_quantity"),
                       ProductSizeHeader = row.Field<string>("productsize_header"),
                       ProductSizeStrength = row.Field<string>("productsize_strength")

                   }).ToList();

            }
            return productsOrderedList;
        }

        // Get list of Orders for a Customer
        public List<OrderDetail> GetCustomerAllOrders(int customerId, int customerStorefrontId)
        {
            DataSet dsCustomerAllOrders = _customerDbl.GetCustomerAllOrders(customerId, customerStorefrontId);

            List<OrderDetail> customerAllOrderList = null;

            if (dsCustomerAllOrders.Tables[0].Rows.Count > 0)
            {
                customerAllOrderList =
                   dsCustomerAllOrders.Tables[0].AsEnumerable()
                   .Select(row => new OrderDetail
                   {
                       RefillCount = row.Field<int>("reFillCount"),
                       CartPharmacyReturn = row.Field<bool>("cartPharmacyReturn"),
                       PharmacyName = row.Field<string>("Pharmacy_Name"),

                       OrderInvoiceId = row.Field<int>("OrderInvoice_Id"),
                       OrderInvoiceActive = row.Field<bool>("OrderInvoice_Active"),
                       OrderInvoiceDateCreated = row.Field<DateTime>("OrderInvoice_DateCreated"),

                       AffiliateId = row.Field<int>("Affiliate_Id"),
                       AffiliateName = row.Field<string>("Affiliate_Name"),

                       CustomerFirstName = row.Field<string>("Customer_FirstName"),
                       CustomerLastName = row.Field<string>("Customer_LastName"),
                       CustomerActive = row.Field<bool>("Customer_Active"),

                       ShippingInvoiceId = Convert.IsDBNull(row.Field<object>("ShippingInvoice_Id")) ? 0 : Convert.ToInt32(row.Field<object>("ShippingInvoice_Id")),

                       ShippingInvoiceShipped = row.Field<object>("ShippingInvoice_Shipped") != DBNull.Value && Convert.ToBoolean(row.Field<object>("ShippingInvoice_Shipped")),

                       ShippingInvoiceShippingDate = row.Field<DateTime>("ShippingInvoice_ShippingDate"),
                       ShippingInvoicePharmacyExported = row.Field<object>("ShippingInvoice_PharmacyExported") != DBNull.Value && Convert.ToBoolean(row.Field<object>("ShippingInvoice_PharmacyExported")),

                       ShippingInvoiceProblemFk = Convert.IsDBNull(row.Field<object>("ShippingInvoice_ShippingInvoiceProblem_Fk")) ? 0 : Convert.ToInt32(row.Field<object>("ShippingInvoice_ShippingInvoiceProblem_Fk")),
                       ShippingInvoiceDeletedDate = row.Field<DateTime>("ShippingInvoice_DeletedDate"),

                       StoreFrontId = row.Field<int>("StoreFront_Id"),
                       StoreFrontNamePk = row.Field<string>("StoreFront_Name_Pk"),
                       StoreFrontCurrencyFk = row.Field<int>("StoreFront_Currency_Fk"),
                       StoreFrontActive = row.Field<bool>("StoreFront_Active"),
                       CartCountForOrder = row.Field<int>("CartCountForOrder")

                   }).Where(x => x.CartCountForOrder != 0).OrderByDescending(x => x.OrderInvoiceId).ToList();

            }
            return customerAllOrderList;
        }

        // Get Cart Details for an OrderId
        public List<CartDetail> GetCustomerOrderCart(int customerId, int customerStorefrontId, int orderId)
        {
            DataSet dsCustomerOrderCart = _customerDbl.GetCustomerOrderCart(customerId, customerStorefrontId, orderId);

            List<CartDetail> customerOrderCart = null;

            if (dsCustomerOrderCart.Tables[0].Rows.Count > 0)
            {
                customerOrderCart =
                   dsCustomerOrderCart.Tables[0].AsEnumerable()
                   .Select(row => new CartDetail
                   {
                       CartShippingInvoiceFk = row.Field<int>("Cart_ShippingInvoice_Fk"),
                       CartItemPrice = row.Field<decimal>("Cart_ItemPrice"),
                       CartItemQuantity = row.Field<int>("Cart_ItemQuantity"),
                       ProductSizeId= row.Field<int>("productsize_id"),
                       ProductName = row.Field<string>("Product_Name"),
                       // ProductSizeHeader = Convert.IsDBNull(row.Field<string>("ProductSize_Header")) ? string.Empty : row.Field<string>("ProductSize_Header"),  
                       ProductSizeHeader = row.Field<string>("ProductSize_Header"),
                       ProductSizeStrength = row.Field<string>("ProductSize_Strength"),
                       ProductSizeQuantity = row.Field<decimal>("ProductSize_Quantity"),
                       ProductId = Convert.IsDBNull(row.Field<object>("Product_Id")) ? 0 : Convert.ToInt32(row.Field<object>("Product_Id")),
                       ProductUnitNamePk = Convert.IsDBNull(row.Field<object>("ProductUnit_Name_Pk")) ? string.Empty : row.Field<string>("ProductUnit_Name_Pk"),
                       ProductSizeGeneric = row.Field<bool>("Productsize_Generic"),
                       ProductQuestionnaireCatId = Convert.IsDBNull(row.Field<object>("Product_QuestionnaireCategory_Fk")) ? 0 : Convert.ToInt32(row.Field<object>("Product_QuestionnaireCategory_Fk")),
                       IsQuestionnaireAnswered = IfUserAlreadyAnsweredQuestionnaire(Convert.IsDBNull(row.Field<object>("Product_QuestionnaireCategory_Fk")) ? 0 : Convert.ToInt32(row.Field<object>("Product_QuestionnaireCategory_Fk")),
                                         customerId, DateTime.Now, SiteConfigurations.QuestionnaireTimeSpan, orderId),
                       QuestionnaireCategoryResponse = GetQuestionnairCategoryResponseByCategory(customerId, 
                                Convert.IsDBNull(row.Field<object>("Product_QuestionnaireCategory_Fk")) ? 0 :
                                            Convert.ToInt32(row.Field<object>("Product_QuestionnaireCategory_Fk")), orderId),
                       CartItemShipped = row.Field<object>("shippingInvoice_shipped") !=Convert.DBNull && Convert.ToBoolean(row.Field<object>("shippingInvoice_shipped"))
                       


                   }).ToList();

            }
            return customerOrderCart;
        }

        // Get Order Details for an OrderId
        public List<OrderDetail> GetCustomerOrderDetail(int customerId, int customerStorefrontId, int orderId)
        {
            DataSet dsCustomerOrderDetail = _customerDbl.GetCustomerOrderDetail(customerId, customerStorefrontId, orderId);

            List<OrderDetail> customerOrderDetail = null;

            if (dsCustomerOrderDetail.Tables[0].Rows.Count > 0)
            {
                customerOrderDetail =
                   dsCustomerOrderDetail.Tables[0].AsEnumerable()
                   .Select(row => new OrderDetail
                   {
                       OrderInvoiceId = row.Field<int>("OrderInvoice_Id"),
                       OrderInvoiceCustomerFk = row.Field<int>("OrderInvoice_Customer_Fk"),
                       OrderInvoiceActive = row.Field<bool>("OrderInvoice_Active"),
                       OrderInvoiceDateCreated = row.Field<DateTime>("OrderInvoice_DateCreated"),
                       OrderInvoiceShippingPrice = row.Field<decimal>("OrderInvoice_ShippingPrice"),
                       OrderInvoiceCouponAmount = row.Field<decimal>("OrderInvoice_CouponAmount"),
                       OrderInvoiceCreditAmount = row.Field<decimal>("OrderInvoice_CreditAmount"),

                       OrderInvoiceBillingFirstName = row.Field<string>("OrderInvoice_BillingFirstName"),
                       OrderInvoiceBillingLastName = row.Field<string>("OrderInvoice_BillingLastName"),
                       OrderInvoiceBillingAddress = row.Field<string>("OrderInvoice_BillingAddress"),
                       OrderInvoiceBillingCity = row.Field<string>("OrderInvoice_BillingCity"),
                       OrderInvoiceBillingProvince = row.Field<string>("OrderInvoice_BillingProvinceState"),
                       OrderInvoiceBillingCountry = row.Field<string>("OrderInvoice_BillingCountry"),
                       OrderInvoiceBillingPhone = row.Field<string>("OrderInvoice_BillingPhone"),
                       OrderInvoiceBillingEmail = row.Field<string>("OrderInvoice_BillingEmail"),
                       OrderInvoiceBillingZipCode = row.Field<string>("OrderInvoice_Billingzipcode"),

                       OrderInvoiceShippingFirstName = row.Field<string>("OrderInvoice_ShippingFirstName"),
                       OrderInvoiceShippingLastName = row.Field<string>("OrderInvoice_ShippingLastName"),
                       OrderInvoiceShippingAddress = row.Field<string>("OrderInvoice_ShippingAddress"),
                       OrderInvoiceShippingCity = row.Field<string>("OrderInvoice_ShippingCity"),
                       OrderInvoiceShippingProvince = row.Field<string>("OrderInvoice_ShippingProvinceState"),
                       OrderInvoiceShippingCountry = row.Field<string>("OrderInvoice_ShippingCountry"),
                       OrderInvoiceShippingPhone = row.Field<string>("OrderInvoice_ShippingPhone"),
                       OrderInvoiceShippingEmail = row.Field<string>("OrderInvoice_ShippingEmail"),
                       OrderInvoiceShippingZipCode = row.Field<string>("OrderInvoice_ShippingZipCode"),

                       OrderInvoiceProvinceTaxAmount = Convert.IsDBNull(row.Field<object>("Orderinvoice_ProvincialTaxAmount")) ? 0.0m : Convert.ToDecimal(row.Field<object>("Orderinvoice_ProvincialTaxAmount")),
                       OrderInvoiceGlobalTaxAmount = Convert.IsDBNull(row.Field<object>("Orderinvoice_GlobalTaxAmount")) ? 0.0m : Convert.ToDecimal(row.Field<object>("Orderinvoice_GlobalTaxAmount")),
                       OrderInvoiceHarmonizedTaxAmount = Convert.IsDBNull(row.Field<object>("Orderinvoice_HarmonizedTaxAmount")) ? 0.0m : Convert.ToDecimal(row.Field<object>("Orderinvoice_HarmonizedTaxAmount")),

                       OrderInvoiceProvincialTaxPercentage = Convert.IsDBNull(row.Field<object>("Orderinvoice_ProvincialTaxPercentage")) ? 0.0m : Convert.ToDecimal(row.Field<object>("Orderinvoice_ProvincialTaxPercentage")),
                       OrderInvoiceGlobalTaxPercentage = Convert.IsDBNull(row.Field<object>("Orderinvoice_GlobalTaxPercentage")) ? 0.0m : Convert.ToDecimal(row.Field<object>("Orderinvoice_GlobalTaxPercentage")),
                       OrderInvoiceHarmonizedTaxPercentage = Convert.IsDBNull(row.Field<object>("Orderinvoice_HarmonizedTaxPercentage")) ? 0.0m : Convert.ToDecimal(row.Field<object>("Orderinvoice_HarmonizedTaxPercentage")),


                       AffiliateId = row.Field<int>("Affiliate_Id"),
                       AffiliateName = row.Field<string>("Affiliate_Name"),

                       CustomerFirstName = row.Field<string>("Customer_FirstName"),
                       CustomerLastName = row.Field<string>("Customer_LastName"),
                       CustomerActive = row.Field<bool>("Customer_Active"),
                       CustomerFreeShipping = row.Field<bool>("Customer_FreeShipping"),
                       CustomerFreeShippingOneYear = row.Field<bool>("Customer_FreeShippingOneYear"),

                       CustomerFreeShippingStartDate = row.Field<DateTime>("Customer_FreeShippingStartDate"),
                       UserAdminProcessingName = row.Field<string>("OrderInvoice_UserNameProcessing"),
                       UserAdminProcessingFirstName = row.Field<string>("OrderInvoice_UserAdminFirstNameProcessing"),
                       UserAdminProcessingLastName = row.Field<string>("OrderInvoice_UserAdminLastNameProcessing"),

                       StoreFrontId = row.Field<int>("StoreFront_Id"),
                       StoreFrontNamePk = row.Field<string>("StoreFront_Name_Pk"),
                       StoreFrontCurrencyFk = row.Field<int>("StoreFront_Currency_Fk"),
                       StoreFrontActive = row.Field<bool>("StoreFront_Active"),

                       OrderCart = GetCustomerOrderCart(row.Field<int>("OrderInvoice_Customer_Fk"),
                                                        row.Field<int>("StoreFront_Id"),
                                                        row.Field<int>("OrderInvoice_Id"))

                   }).ToList();

            }
            return customerOrderDetail;
        }

        // check if customer email is already registered.

        public bool CustomerEmailRegistered(string customerEmail, int affiliateId)
        {
            return _customerDbl.CustomerEmailRegistered(customerEmail, affiliateId);
        }

        // Verify if customer is found based on either email or Id and storefront

        public int GetCustomerFromEmailOrId(string customerEmail, int customerId, int affiliateId)
        {
            return _customerDbl.GetCustomerFromEmailOrId(customerEmail, customerId, affiliateId);
        }

        // Add new customer Signup to database
        public int AddCustomer(Customer customerInfo)
        {
            // Create a string of sql parameters passed to stored procedure
            StringBuilder logUserActionParameters = new StringBuilder();
            logUserActionParameters.Append("{ ");
            logUserActionParameters.Append(" CustomerLastIp:");
            logUserActionParameters.Append(customerInfo.CustomerLastIp);

            logUserActionParameters.Append(", CustomerAffiliateId:");
            logUserActionParameters.Append(customerInfo.CustomerAffiliateId);

            logUserActionParameters.Append(", CustomerFirstName:");
            logUserActionParameters.Append(customerInfo.CustomerFirstName);

            logUserActionParameters.Append(", CustomerLastName:");
            logUserActionParameters.Append(customerInfo.CustomerLastName);

            logUserActionParameters.Append(", CustomerSex:");
            logUserActionParameters.Append(customerInfo.CustomerSex);

            logUserActionParameters.Append(", CustomerEmail:");
            logUserActionParameters.Append(customerInfo.CustomerEmail);

            logUserActionParameters.Append(", CustomerPassword:");
            logUserActionParameters.Append(customerInfo.CustomerPassword);

            logUserActionParameters.Append(", CustomerPhone:");
            logUserActionParameters.Append(customerInfo.CustomerPhone);

            logUserActionParameters.Append(", CustomerEveningPhone:");
            logUserActionParameters.Append(customerInfo.CustomerEveningPhone);

            logUserActionParameters.Append(", CustomerCellPhone:");
            logUserActionParameters.Append(customerInfo.CustomerCellPhone);

            logUserActionParameters.Append(", CustomerAddress:");
            logUserActionParameters.Append(customerInfo.CustomerAddress);

            logUserActionParameters.Append(", CustomerCity:");
            logUserActionParameters.Append(customerInfo.CustomerCity);

            logUserActionParameters.Append(", CustomerCountryId:");
            logUserActionParameters.Append(customerInfo.CustomerCountryId);

            logUserActionParameters.Append(", CustomerProvinceId:");
            logUserActionParameters.Append(customerInfo.CustomerProvinceId);

            logUserActionParameters.Append(", CustomerZipCode:");
            logUserActionParameters.Append(customerInfo.CustomerZipCode);

            logUserActionParameters.Append(", CustomerReferralId:");
            logUserActionParameters.Append(customerInfo.CustomerReferralId);

            logUserActionParameters.Append(", CustomerActive:");
            logUserActionParameters.Append(customerInfo.CustomerActive);

            logUserActionParameters.Append(", CustomerTestAccount:");
            logUserActionParameters.Append(customerInfo.CustomerTestAccount);

            logUserActionParameters.Append(", CustomerDateCreated:");
            logUserActionParameters.Append(customerInfo.CustomerDateCreated);

            logUserActionParameters.Append(", CustomerLastModified:");
            logUserActionParameters.Append(customerInfo.CustomerLastModified);

            logUserActionParameters.Append(", CustomerLastLoginDate:");
            logUserActionParameters.Append(customerInfo.CustomerLastLoginDate);

            logUserActionParameters.Append(", CustomerUserAdminFk:");
            logUserActionParameters.Append(customerInfo.CustomerUserAdminFk);

            logUserActionParameters.Append(", CustomerDob):");
            logUserActionParameters.Append(customerInfo.CustomerDob);

            logUserActionParameters.Append(" }");

            int newCustomerId = _customerDbl.AddCustomer(customerInfo.CustomerLastIp,
                                                                       customerInfo.CustomerAffiliateId,
                                                                       customerInfo.CustomerFirstName,
                                                                       customerInfo.CustomerLastName,
                                                                       customerInfo.CustomerSex,
                                                                       customerInfo.CustomerEmail,
                                                                       customerInfo.CustomerPassword,
                                                                       customerInfo.CustomerPhone,
                                                                       customerInfo.CustomerEveningPhone,
                                                                       customerInfo.CustomerCellPhoneArea,
                                                                       customerInfo.CustomerAddress,
                                                                       customerInfo.CustomerCity,
                                                                       customerInfo.CustomerCountryId,
                                                                       customerInfo.CustomerProvinceId,
                                                                       customerInfo.CustomerZipCode,
                                                                       customerInfo.CustomerReferralId,
                                                                       customerInfo.CustomerActive,
                                                                       customerInfo.CustomerTestAccount,
                                                                       customerInfo.CustomerDateCreated,
                                                                       customerInfo.CustomerLastModified,
                                                                       customerInfo.CustomerLastLoginDate,
                                                                       customerInfo.CustomerUserAdminFk,
                                                                       customerInfo.CustomerDob,
                                                                       logUserActionParameters.ToString()
                                                                       );
            return newCustomerId;
        }

        public CustomerLoginInfo GetCustomerLoginInfo(string customerEmail, int affiliateId)
        {
            DataSet dsCustomerLoginInfo = _customerDbl.GetCustomerLoginInfo(customerEmail, affiliateId);

            CustomerLoginInfo customerLoginInfo = null;

            if (dsCustomerLoginInfo.Tables[0].Rows.Count > 0)
            {
                customerLoginInfo =
                    dsCustomerLoginInfo.Tables[0].AsEnumerable()
                        .Select(row => new CustomerLoginInfo
                        {
                            CustomerId = Convert.IsDBNull(row.Field<object>("Customer_Id")) ? 0 : Convert.ToInt32(row.Field<object>("Customer_Id")),
                            Email = Convert.IsDBNull(row.Field<object>("Customer_Email"))? string.Empty : Convert.ToString(row.Field<object>("Customer_Email")),
                            Password = Convert.IsDBNull(row.Field<object>("Customer_Password")) ? string.Empty : Convert.ToString(row.Field<object>("Customer_Password")) ,
                            FirstName = Convert.IsDBNull(row.Field<object>("Customer_Firstname")) ? string.Empty : Convert.ToString(row.Field<object>("Customer_Firstname")), 
                            SignupEmailVerified = row.Field<object>("Customer_SignupEmailVerified") != DBNull.Value && Convert.ToBoolean(row.Field<object>("Customer_SignupEmailVerified"))


                        }).ToList().FirstOrDefault();

            }
            return customerLoginInfo;
        }

        // get customer Patient profiles
        public IEnumerable<PatientProfile> GetPatientProfilesOfCustomer(int customerId, int patientProfileId, int storeFrontId)
        {
            DataSet dsPatientProfile = _customerDbl.GetPatientProfilesOfCustomer(customerId, patientProfileId, storeFrontId);

            List<PatientProfile> patientProfile = null;

            if (dsPatientProfile.Tables[0].Rows.Count > 0)
            {
                patientProfile =
                    dsPatientProfile.Tables[0].AsEnumerable()
                        .Select(row => new PatientProfile
                        {
                            PatientProfileId = row.Field<int>("PatientProfile_Id")
                            ,
                            PatientProfileAllergy = Convert.IsDBNull(row.Field<string>("PatientProfile_Allergy")) ? string.Empty : row.Field<string>("PatientProfile_Allergy")
                            ,
                            PatientProfileFirstName = Convert.IsDBNull(row.Field<string>("PatientProfile_FirstName")) ? string.Empty : row.Field<string>("PatientProfile_FirstName")
                            ,
                            PatientProfileLastName = Convert.IsDBNull(row.Field<string>("PatientProfile_LastName")) ? string.Empty : row.Field<string>("PatientProfile_LastName")
                            ,
                            PatientProfileSex = Convert.IsDBNull(row.Field<string>("PatientProfile_Sex")) ? string.Empty : row.Field<string>("PatientProfile_Sex")
                            ,
                            PatientProfileMedicalHistory = Convert.IsDBNull(row.Field<string>("PatientProfile_MedicalHistory")) ? string.Empty : row.Field<string>("PatientProfile_MedicalHistory")
                            ,
                            PatientProfileWeightLb = Convert.IsDBNull(row.Field<int>("PatientProfile_WeightLb")) ? 0 : row.Field<int>("PatientProfile_WeightLb")
                            ,
                            PatientProfileBirthDate = Convert.IsDBNull(row.Field<object>("PatientProfile_BirthDate")) ? DateTime.MinValue : Convert.ToDateTime(row.Field<object>("PatientProfile_BirthDate"))
                            ,
                            PatientProfileOwnerBirthDate = Convert.IsDBNull(row.Field<object>("PatientProfile_OwnerBirthDate")) ? DateTime.MinValue : Convert.ToDateTime(row.Field<object>("PatientProfile_OwnerBirthDate"))
                            ,
                            PatientProfilePet = row.Field<object>("PatientProfile_Pet") != DBNull.Value && Convert.ToBoolean(row.Field<object>("PatientProfile_Pet"))
                            ,
                            PatientProfilePhysicianFirstName = Convert.IsDBNull(row.Field<string>("PatientProfile_PhysicianFirstName")) ? string.Empty : row.Field<string>("PatientProfile_PhysicianFirstName")
                            ,
                            PatientProfilePhysicianLastName = Convert.IsDBNull(row.Field<string>("PatientProfile_PhysicianLastName")) ? string.Empty : row.Field<string>("PatientProfile_PhysicianLastName")
                            ,
                            PatientProfilePhysicianPhone = Convert.IsDBNull(row.Field<string>("PatientProfile_PhysicianPhone")) ? string.Empty : row.Field<string>("PatientProfile_PhysicianPhone")
                            ,
                            PatientProfilePhysicianFax = Convert.IsDBNull(row.Field<string>("PatientProfile_PhysicianFax")) ? string.Empty : row.Field<string>("PatientProfile_PhysicianFax")
                            ,
                            PatientProfileActive = row.Field<object>("PatientProfile_Active") != DBNull.Value && Convert.ToBoolean(row.Field<object>("PatientProfile_Active"))
                            ,
                            PatientProfileChildproofCap = row.Field<object>("PatientProfile_ChildproofCap") != DBNull.Value && Convert.ToBoolean(row.Field<object>("PatientProfile_ChildproofCap"))
                            ,
                            PatientProfileMedication = Convert.IsDBNull(row.Field<object>("Patientprofile_Medication")) ? string.Empty : row.Field<string>("Patientprofile_Medication")
                            ,
                            PatientProfilePhoneNumber = Convert.IsDBNull(row.Field<object>("Patientprofile_PhoneNumber")) ? string.Empty : row.Field<string>("Patientprofile_PhoneNumber"),

                            PatientProfileConsultationConsent = row.Field<object>("Patientprofile_ConsultationConsent") != DBNull.Value && Convert.ToBoolean(row.Field<object>("Patientprofile_ConsultationConsent")),

                            CustomerProvinceCode = Convert.IsDBNull(row.Field<object>("Provincestate_Code")) ? string.Empty : row.Field<string>("Provincestate_Code"),
                            
                            PatientPersonalHealthNumber = Convert.IsDBNull(row.Field<object>("PatientProfile_PersonalHealthNumber")) ? string.Empty : row.Field<string>("PatientProfile_PersonalHealthNumber"),

                            PatientProfileSocialHistory = Convert.IsDBNull(row.Field<object>("PatientProfile_SocialHistory")) ? string.Empty : row.Field<string>("PatientProfile_SocialHistory"),

                            PatientProfilePastSurgeries = Convert.IsDBNull(row.Field<object>("PatientProfile_PastSurgeries")) ? string.Empty : row.Field<string>("PatientProfile_PastSurgeries"),

                            PatientProfileFamilyHistoryIllness = Convert.IsDBNull(row.Field<object>("PatientProfile_FamilyHistoryIllness")) ? string.Empty : row.Field<string>("PatientProfile_FamilyHistoryIllness"),

                            PatientProfileHerbalSupplements = Convert.IsDBNull(row.Field<object>("PatientProfile_HerbalSupplements")) ? string.Empty : row.Field<string>("PatientProfile_HerbalSupplements"),

                            PatientProfileGenderOther = Convert.IsDBNull(row.Field<object>("Patientprofile_Sex_Other"))?string.Empty:row.Field<string>("Patientprofile_Sex_Other")
                        }).ToList();

            }
            return patientProfile;
        }

        // get patient medications based on profile id

        public List<PatientMedication> GetPatientMedicationByProfile(int patientProfileId, int medicationId, int customerId, int storefrontId)
        {
            DataSet dsPatientProfile = _customerDbl.GetPatientMedicationByProfile(patientProfileId, medicationId, customerId, storefrontId);

            List<PatientMedication> patientMedication = null;

            if (dsPatientProfile.Tables[0].Rows.Count > 0)
            {
                patientMedication =
                    dsPatientProfile.Tables[0].AsEnumerable()
                        .Select(row => new PatientMedication
                        {
                            PatientProfileId = row.Field<int>("Patientmedication_Patientprofile_Fk"),
                            PatientMedicationId = row.Field<int>("PatientMedication_Id"),
                            PatientMedicationDrugName = Convert.IsDBNull(row.Field<string>("Patientmedication_Drugname")) ? string.Empty : row.Field<string>("Patientmedication_Drugname"),
                            PatientMedicationEffectiveness = Convert.IsDBNull(row.Field<string>("Patientmedication_Effectiveness")) ? string.Empty : row.Field<string>("Patientmedication_Effectiveness"),
                            PatientMedicationFrequency = Convert.IsDBNull(row.Field<string>("Patientmedication_Frequency")) ? string.Empty : row.Field<string>("Patientmedication_Frequency"),
                            PatientMedicationHowLong = Convert.IsDBNull(row.Field<string>("Patientmedication_Howlong")) ? string.Empty : row.Field<string>("Patientmedication_Howlong"),
                            PatientMedicationIllness = Convert.IsDBNull(row.Field<string>("Patientmedication_Illness")) ? string.Empty : row.Field<string>("Patientmedication_Illness"),
                            PatientMedicationStrength = Convert.IsDBNull(row.Field<string>("Patientmedication_Strength")) ? string.Empty : row.Field<string>("Patientmedication_Strength"),
                            PatientMedicationDateCreated = Convert.IsDBNull(row.Field<object>("Patientmedication_DateCreated")) ? DateTime.MinValue : Convert.ToDateTime(row.Field<object>("Patientmedication_DateCreated")),
                            PatientMedicationLastModified = Convert.IsDBNull(row.Field<object>("Patientmedication_Lastmodified")) ? DateTime.MinValue : Convert.ToDateTime(row.Field<object>("Patientmedication_Lastmodified")),


                        }).ToList();

            }
            return patientMedication;
        }

        // check if email being saved into account is used by another customer
        public bool EmailFoundForOtherCustomer(string customerEmail, int storeFrontId, int customerId)
        {
            return _customerDbl.EmailFoundForOtherCustomer(customerEmail, storeFrontId, customerId);
        }

        // update patient profile
        public int UpdateProfile(PatientProfile patient, int storeFrontId, int patientProfileId, int customerId, string customerName, string customerEmail, string customerLastIp, DateTime lastModified)
        {
            // Create a string of sql parameters passed to stored procedure
            StringBuilder logUserActionParameters = new StringBuilder();
            logUserActionParameters.Append("{ ");

            logUserActionParameters.Append("PatientProfileChildproofCap:");
            logUserActionParameters.Append(patient.PatientProfileChildproofCap);

            logUserActionParameters.Append(", PatientProfilePhysicianFax:");
            logUserActionParameters.Append(patient.PatientProfilePhysicianFax);

            logUserActionParameters.Append(", PatientProfilePhysicianPhone:");
            logUserActionParameters.Append(patient.PatientProfilePhysicianPhone);

            logUserActionParameters.Append(", PatientProfilePhysicianLastName:");
            logUserActionParameters.Append(patient.PatientProfilePhysicianLastName);

            logUserActionParameters.Append(", PatientProfilePhysicianFirstName:");
            logUserActionParameters.Append(patient.PatientProfilePhysicianFirstName);

            logUserActionParameters.Append(", PatientProfileBirthDate:");
            logUserActionParameters.Append(patient.PatientProfileBirthDate);

            logUserActionParameters.Append(", PatientProfileWeightLb:");
            logUserActionParameters.Append(patient.PatientProfileWeightLb);

            logUserActionParameters.Append(", PatientProfileMedicalHistory:");
            logUserActionParameters.Append(patient.PatientProfileMedicalHistory);

            logUserActionParameters.Append(", PatientProfileSex:");
            logUserActionParameters.Append(patient.PatientProfileSex);

            logUserActionParameters.Append(", PatientProfileLastName:");
            logUserActionParameters.Append(patient.PatientProfileLastName);

            logUserActionParameters.Append(", PatientProfileFirstName:");
            logUserActionParameters.Append(patient.PatientProfileFirstName);

            logUserActionParameters.Append(", PatientAllergy:");
            logUserActionParameters.Append(patient.PatientProfileAllergy);

            logUserActionParameters.Append(", Patientprofile_PhoneNumber:");
            logUserActionParameters.Append(patient.PatientProfilePhoneNumber);

            logUserActionParameters.Append(", Patientprofile_Medication:");
            logUserActionParameters.Append(patient.PatientProfileMedication);


            logUserActionParameters.Append(", PatientProfileId:");
            logUserActionParameters.Append(patientProfileId);
            logUserActionParameters.Append(", LastModified:");
            logUserActionParameters.Append(lastModified);

            logUserActionParameters.Append(" }");


            int updateProfileStatus = _customerDbl.UpdateProfile(patientProfileId,
                                                                     patient.PatientProfileAllergy,
                                                                     patient.PatientProfileFirstName,
                                                                     patient.PatientProfileLastName,
                                                                     patient.PatientProfileSex,
                                                                     patient.PatientProfileMedicalHistory,
                                                                     patient.PatientProfileWeightLb,
                                                                     patient.PatientProfileBirthDate,
                                                                     patient.PatientProfilePhysicianFirstName,
                                                                     patient.PatientProfilePhysicianLastName,
                                                                     patient.PatientProfilePhysicianPhone,
                                                                     patient.PatientProfilePhysicianFax,
                                                                     patient.PatientProfileChildproofCap,
                                                                     patient.PatientProfileMedication,
                                                                     patient.PatientProfilePhoneNumber,
                                                                     lastModified,
                                                                     logUserActionParameters.ToString(),
                                                                     customerLastIp,
                                                                     customerEmail,
                                                                     customerId,
                                                                     customerName,
                                                                     patient.PatientProfileConsultationConsent,
                                                                     patient.PatientPersonalHealthNumber,
                                                                     patient.PatientProfileSocialHistory,
                                                                     patient.PatientProfilePastSurgeries,
                                                                     patient.PatientProfileFamilyHistoryIllness,
                                                                     patient.PatientProfileHerbalSupplements,
                                                                     patient.PatientProfileGenderOther
                                                                     );
            return updateProfileStatus;
        }

        // update patient medication

        public int UpdateMedication(int patientProfileId,
                                    int patientMedicationId,
                                    string patientMedicationDrugname,
                                    string patientMedicationIllness,
                                    DateTime patientMedicationLastModified)
        {
            int updateMedicationStatus = _customerDbl.UpdatePatientMedication(patientProfileId,
                                                                     patientMedicationId,
                                                                     patientMedicationDrugname,
                                                                     patientMedicationIllness,
                                                                     patientMedicationLastModified
                                                                     );
            return updateMedicationStatus;
        }

        // add New patient Medication

        public int AddMedication(int patientProfileId, string patientMedicationDrugName, string patientmedicationIllness,
            DateTime createdOn)
        {
            int newPatientMedicationId = _customerDbl.AddPatientMedication(patientProfileId, patientMedicationDrugName, patientmedicationIllness, createdOn);
            return newPatientMedicationId;
        }

        // add PatientProfile
        public int AddPatientProfile(PatientProfile patientProfile, int customerId, string customerName,
                                string customerEmail, string customerLastIp, DateTime createdOn)
        {
            // Create a string of sql parameters passed to stored procedure
            StringBuilder logUserActionParameters = new StringBuilder();
            logUserActionParameters.Append("{");

            logUserActionParameters.Append(" PatientProfileChildproofCap:");
            logUserActionParameters.Append(patientProfile.PatientProfileChildproofCap);

            logUserActionParameters.Append(", PatientProfilePhysicianFax:");
            logUserActionParameters.Append(patientProfile.PatientProfilePhysicianFax);

            logUserActionParameters.Append(", PatientProfilePhysicianPhone:");
            logUserActionParameters.Append(patientProfile.PatientProfilePhysicianPhone);

            logUserActionParameters.Append(", PatientProfilePhysicianLastName:");
            logUserActionParameters.Append(patientProfile.PatientProfilePhysicianLastName);

            logUserActionParameters.Append(", PatientProfilePhysicianFirstName:");
            logUserActionParameters.Append(patientProfile.PatientProfilePhysicianFirstName);

            logUserActionParameters.Append(", PatientProfileBirthDate:");
            logUserActionParameters.Append(patientProfile.PatientProfileBirthDate);

            logUserActionParameters.Append(", PatientProfileWeightLb:");
            logUserActionParameters.Append(patientProfile.PatientProfileWeightLb);

            logUserActionParameters.Append(", PatientProfileMedicalHistory:");
            logUserActionParameters.Append(patientProfile.PatientProfileMedicalHistory);

            logUserActionParameters.Append(", PatientProfileSex:");
            logUserActionParameters.Append(patientProfile.PatientProfileSex);

            logUserActionParameters.Append(", PatientProfileLastName:");
            logUserActionParameters.Append(patientProfile.PatientProfileLastName);

            logUserActionParameters.Append(", PatientProfileFirstName:");
            logUserActionParameters.Append(patientProfile.PatientProfileFirstName);

            logUserActionParameters.Append(", PatientAllergy:");
            logUserActionParameters.Append(patientProfile.PatientProfileAllergy);

            logUserActionParameters.Append(", CustomerFK):");
            logUserActionParameters.Append(customerId);
            logUserActionParameters.Append(", CreatedOn):");
            logUserActionParameters.Append(createdOn);

            logUserActionParameters.Append(" }");


            int newPatientProfileId = _customerDbl.AddPatientProfile(customerId,
                                                                      patientProfile.PatientProfileAllergy,
                                                                      patientProfile.PatientProfileFirstName,
                                                                      patientProfile.PatientProfileLastName,
                                                                      patientProfile.PatientProfileSex,
                                                                      patientProfile.PatientProfileMedicalHistory,
                                                                      patientProfile.PatientProfileWeightLb,
                                                                      patientProfile.PatientProfileBirthDate,
                                                                      patientProfile.PatientProfilePhysicianFirstName,
                                                                      patientProfile.PatientProfilePhysicianLastName,
                                                                      patientProfile.PatientProfilePhysicianPhone,
                                                                      patientProfile.PatientProfilePhysicianFax,
                                                                      patientProfile.PatientProfileChildproofCap,
                                                                      createdOn,
                                                                      logUserActionParameters.ToString(),
                                                                      customerLastIp,
                                                                      customerEmail,
                                                                      customerName,
                                                                      patientProfile.PatientProfilePhoneNumber,
                                                                      patientProfile.PatientProfileMedication,
                                                                      patientProfile.PatientProfileConsultationConsent,
                                                                      patientProfile.PatientPersonalHealthNumber,
                                                                      patientProfile.PatientProfileSocialHistory,
                                                                      patientProfile.PatientProfilePastSurgeries,
                                                                      patientProfile.PatientProfileFamilyHistoryIllness,
                                                                      patientProfile.PatientProfileHerbalSupplements, 
                                                                      patientProfile.PatientProfileGenderOther);



            return newPatientProfileId;

        }


        public int AddUserPasswordRecoveryDetails(int customerFk, string encryptedToken, string decryptedToken, DateTime linkExpiry, int resetStatus, DateTime dateCreated)
        {
            return _customerDbl.AddUserPasswordRecoveryDetails(customerFk, encryptedToken, decryptedToken, linkExpiry, resetStatus, dateCreated);
        }

        public UserPasswordRecovery GetUserPasswordRecoveryDetails(string token)
        {
            DataSet dsCustomer = _customerDbl.GetUserPasswordRecoveryDetails(token);

            if (dsCustomer.Tables[0].Rows.Count > 0)
            {
                return dsCustomer.Tables[0].AsEnumerable()
                        .Select(row => new UserPasswordRecovery
                        {
                            UserPasswordRecoveryId = Convert.IsDBNull(row.Field<object>("UserPasswordRecovery_Id")) ? 0 : Convert.ToInt32(row.Field<object>("UserPasswordRecovery_Id")),
                            CustomerFk = Convert.IsDBNull(row.Field<object>("UserPasswordRecovery_Customer_Fk")) ? 0 : Convert.ToInt32(row.Field<object>("UserPasswordRecovery_Customer_Fk")),
                            EncryptedToken = Convert.IsDBNull(row.Field<object>("UserPasswordRecovery_EncryptedToken")) ? string.Empty : Convert.ToString(row.Field<object>("UserPasswordRecovery_EncryptedToken")),
                            DecryptedToken = Convert.IsDBNull(row.Field<object>("UserPasswordRecovery_DecryptedToken")) ? string.Empty : Convert.ToString(row.Field<object>("UserPasswordRecovery_DecryptedToken")),
                            LinkExpiryTime = Convert.IsDBNull(row.Field<object>("UserPasswordRecovery_LinkExpiryTime")) ? DateTime.MinValue : Convert.ToDateTime(row.Field<object>("UserPasswordRecovery_LinkExpiryTime")),
                            ResetStatus = Convert.IsDBNull(row.Field<object>("UserPasswordRecovery_ResetStatus")) ? 0 : Convert.ToInt32(row.Field<object>("UserPasswordRecovery_ResetStatus"))


                        }).FirstOrDefault();
            }

            return null;

        }

        public int UpdateCustomerPassword(int customerFk, string password, DateTime dateModified)
        {
            return _customerDbl.UpdateCustomerPassword(customerFk, password, dateModified);
        }

        public int UserPasswordRecoveryResetStatus(int userPasswordRecoveryId, int customerFk, int resetStatus, DateTime dateModified)
        {
            return _customerDbl.UserPasswordRecoveryResetStatus(userPasswordRecoveryId, customerFk, resetStatus, dateModified);
        }

        // get open orders for the customer (used by my account section)
        public List<OrderStatus> GetCustomerOpenOrders(int customerId, int storefrontId)
        {
            DataSet dsCustomerAllOrders = _customerDbl.GetCustomerOpenOrders(customerId, storefrontId);

            List<OrderStatus> customerOpenOrderList = new List<OrderStatus>();

            if (dsCustomerAllOrders.Tables[0].Rows.Count > 0)
            {
                customerOpenOrderList =
                   dsCustomerAllOrders.Tables[0].AsEnumerable()
                   .Select(row => new OrderStatus
                   {
                       OrderInvoiceId = row.Field<int>("OrderInvoice_Id"),
                       OrderInvoiceActive = row.Field<bool>("OrderInvoice_Active"),
                       OrderInvoiceDateCreated = row.Field<DateTime>("OrderInvoice_DateCreated"),
                       OrderInvoiceCustomerFk = row.Field<int>("OrderInvoice_Customer_Fk"),
                       OrderInvoiceCpPaymentPending = row.Field<object>("OrderInvoice_CpPaymentPending") != DBNull.Value && Convert.ToBoolean(row.Field<object>("OrderInvoice_CpPaymentPending")),
                       OrderPmtStatus = Convert.IsDBNull(row.Field<string>("PaymentTransactionType_DisplayStatus")) ? string.Empty : row.Field<string>("PaymentTransactionType_DisplayStatus"),

                       ShippingInvoiceId = Convert.IsDBNull(row.Field<object>("ShippingInvoice_Id")) ? 0 : Convert.ToInt32(row.Field<object>("ShippingInvoice_Id")),
                       ShippingInvoiceShipped = row.Field<object>("ShippingInvoice_Shipped") != DBNull.Value && Convert.ToBoolean(row.Field<object>("ShippingInvoice_Shipped")),
                       ShippingInvoiceIsPrep = row.Field<object>("Shippinginvoice_IsPrep") != DBNull.Value && Convert.ToBoolean(row.Field<object>("Shippinginvoice_IsPrep")),
                       ShippingInvoiceShippingDate = (DateTime?)row.Field<object>("ShippingInvoice_ShippingDate"),
                       ShippingInvoicePharmacyExported = row.Field<object>("ShippingInvoice_PharmacyExported") != DBNull.Value && Convert.ToBoolean(row.Field<object>("ShippingInvoice_PharmacyExported")),
                       ShippingInvoiceProblemFk = Convert.IsDBNull(row.Field<object>("ShippingInvoice_ShippingInvoiceProblem_Fk")) ? 0 : Convert.ToInt32(row.Field<object>("ShippingInvoice_ShippingInvoiceProblem_Fk")),
                       ShippingInvoiceDeletedDate = (DateTime?)row.Field<object>("ShippingInvoice_DeleteDate"),
                       ShippingInvoiceIncorrectShip = row.Field<object>("ShippingInvoice_IncorrectShip") != DBNull.Value && Convert.ToBoolean(row.Field<object>("ShippingInvoice_IncorrectShip")),
                       ShippingInvoiceDocumentReceived = row.Field<object>("ShippingInvoice_DocumentReceived") != DBNull.Value && Convert.ToBoolean(row.Field<object>("ShippingInvoice_DocumentReceived")),
                       ShippingInvoiceDoctorApproveDate = (DateTime?)row.Field<object>("ShippingInvoice_DoctorApproveDate"),
                       ShippingInvoiceHasRxItem = row.Field<object>("Shippinginvoice_HasRxItem") != DBNull.Value &&
                                                  Convert.ToBoolean(
                                                      row.Field<object>("Shippinginvoice_HasRxItem")),
                       ShippingInvoiceTrackinCode = Convert.IsDBNull(row.Field<object>("ShippingInvoice_TrackingCode")) ? string.Empty : row.Field<string>("ShippingInvoice_TrackingCode"),
                       StoreFrontId = row.Field<int>("StoreFront_Id"),
                       StoreFrontNamePk = row.Field<string>("StoreFront_Name_Pk"),

                       CartCountForOrder = row.Field<int>("CartCountForOrder"),
                       PrescriptionId = Convert.IsDBNull(row.Field<object>("PrescriptionId")) ? 0 : Convert.ToInt32(row.Field<object>("PrescriptionId")),

                       PrescriptionApprovalStatus = Convert.IsDBNull(row.Field<object>("PrescriptionApprovalStatus")) ? 0 : Convert.ToInt32(row.Field<object>("PrescriptionApprovalStatus")),

                       QuestionnaireId = Convert.IsDBNull(row.Field<object>("QuestionnaireId")) ? 0 : Convert.ToInt32(row.Field<object>("QuestionnaireId")),
                       OrderinvoicePaymentTransactionTypeFk = Convert.IsDBNull(row.Field<object>("Orderinvoice_PaymentTransactionType_Fk")) ? 0 :
                                                    Convert.ToInt32(row.Field<object>("Orderinvoice_PaymentTransactionType_Fk")),
                       ShippingInvoiceDispensed = row.Field<object>("Shippinginvoice_Dispensed") != DBNull.Value &&
                                                        Convert.ToBoolean(row.Field<object>("Shippinginvoice_Dispensed")),
                       OrderInvoiceShippingProvinceName = Convert.IsDBNull(row.Field<object>("ShippingProvinceName")) ? 
                                                        string.Empty : Convert.ToString(row.Field<object>("ShippingProvinceName")),
                       OrderCart = GetCustomerOrderCart(row.Field<int>("OrderInvoice_Customer_Fk"),
                           row.Field<int>("StoreFront_Id"),
                           row.Field<int>("OrderInvoice_Id")),
                       OrderinvoiceBPNotKnown = row.Field<object>("Orderinvoice_BPNotKnown") != DBNull.Value && Convert.ToBoolean(row.Field<object>("Orderinvoice_BPNotKnown")),
                       OrderInvoiceCouponAppliedAmount = Convert.IsDBNull(row.Field<object>("Orderinvoice_Couponamount")) ? 0 :
                                                    Convert.ToDecimal(row.Field<object>("Orderinvoice_Couponamount")),
                       OrderInvoiceCreditAppliedAmount = Convert.IsDBNull(row.Field<object>("Orderinvoice_Creditamount")) ? 0 :
                                                    Convert.ToDecimal(row.Field<object>("Orderinvoice_Creditamount"))

                   }).Where(x => x.CartCountForOrder != 0).OrderByDescending(x => x.OrderInvoiceId).ToList();

            }
            return customerOpenOrderList;
        }

        // get closed orders for the customer
        public List<OrderStatus> GetCustomerClosedOrders(int customerId, int storefrontId)
        {
            DataSet dsCustomerClosedOrders = _customerDbl.GetCustomerClosedOrders(customerId, storefrontId);

            List<OrderStatus> customerClosedOrderList = new List<OrderStatus>();

            if (dsCustomerClosedOrders.Tables[0].Rows.Count > 0)
            {
                customerClosedOrderList =
                   dsCustomerClosedOrders.Tables[0].AsEnumerable()
                   .Select(row => new OrderStatus
                   {
                       OrderInvoiceId = row.Field<int>("OrderInvoice_Id"),
                       OrderInvoiceActive = row.Field<bool>("OrderInvoice_Active"),
                       OrderInvoiceDateCreated = row.Field<DateTime>("OrderInvoice_DateCreated"),
                       OrderInvoiceCustomerFk = row.Field<int>("OrderInvoice_Customer_Fk"),
                       OrderInvoiceCpPaymentPending = row.Field<object>("OrderInvoice_CpPaymentPending") != DBNull.Value && Convert.ToBoolean(row.Field<object>("OrderInvoice_CpPaymentPending")),

                       ShippingInvoiceId = Convert.IsDBNull(row.Field<object>("ShippingInvoice_Id")) ? 0 : Convert.ToInt32(row.Field<object>("ShippingInvoice_Id")),
                       ShippingInvoicePharmacyFk = Convert.IsDBNull(row.Field<object>("Shippinginvoice_Pharmacy_Fk")) ? 0 : Convert.ToInt32(row.Field<object>("Shippinginvoice_Pharmacy_Fk")),
                       ShippingInvoiceShipped = row.Field<object>("ShippingInvoice_Shipped") != DBNull.Value && Convert.ToBoolean(row.Field<object>("ShippingInvoice_Shipped")),
                       ShippingInvoiceShippingDate = (DateTime?)row.Field<object>("ShippingInvoice_ShippingDate"),
                       ShippingInvoiceAlpsShippingDate = (DateTime?)row.Field<object>("Shippinginvoice_alps_shippingdate"),
                       ShippingInvoicePharmacyExported = row.Field<object>("ShippingInvoice_PharmacyExported") != DBNull.Value && Convert.ToBoolean(row.Field<object>("ShippingInvoice_PharmacyExported")),
                       ShippingInvoiceProblemFk = Convert.IsDBNull(row.Field<object>("ShippingInvoice_ShippingInvoiceProblem_Fk")) ? 0 : Convert.ToInt32(row.Field<object>("ShippingInvoice_ShippingInvoiceProblem_Fk")),
                       ShippingInvoiceDeletedDate = (DateTime?)row.Field<object>("ShippingInvoice_DeleteDate"),
                       ShippingInvoiceIncorrectShip = row.Field<object>("ShippingInvoice_IncorrectShip") != DBNull.Value && Convert.ToBoolean(row.Field<object>("ShippingInvoice_IncorrectShip")),
                       ShippingInvoiceDocumentReceived = row.Field<object>("ShippingInvoice_DocumentReceived") != DBNull.Value && Convert.ToBoolean(row.Field<object>("ShippingInvoice_DocumentReceived")),
                       ShippingInvoiceDoctorApproveDate = (DateTime?)row.Field<object>("ShippingInvoice_DoctorApproveDate"),
                       ShippingInvoiceHasRxItem = row.Field<object>("Shippinginvoice_HasRxItem") != DBNull.Value &&
                                                  Convert.ToBoolean(
                                                      row.Field<object>("Shippinginvoice_HasRxItem")),
                       ShippingInvoiceTrackinCode = Convert.IsDBNull(row.Field<object>("ShippingInvoice_TrackingCode")) ? string.Empty : row.Field<string>("ShippingInvoice_TrackingCode"),
                       StoreFrontId = row.Field<int>("StoreFront_Id"),
                       StoreFrontNamePk = row.Field<string>("StoreFront_Name_Pk"),

                       CartCountForOrder = row.Field<int>("CartCountForOrder"),

                       OrderPmtStatus = Convert.IsDBNull(row.Field<string>("PaymentTransactionType_DisplayStatus")) ? string.Empty : row.Field<string>("PaymentTransactionType_DisplayStatus"),
                                        //row.Field<bool>("OrderInvoice_Active") == true ? "Closed" : "Deleted",

                       PrescriptionId = Convert.IsDBNull(row.Field<object>("PrescriptionId")) ? 0 : Convert.ToInt32(row.Field<object>("PrescriptionId")),
                       PrescriptionApprovalStatus = Convert.IsDBNull(row.Field<object>("PrescriptionApprovalStatus")) ? 0 : Convert.ToInt32(row.Field<object>("PrescriptionApprovalStatus")),
                       ShippingInvoiceDispensed = row.Field<object>("Shippinginvoice_Dispensed") != DBNull.Value &&
                                                        Convert.ToBoolean(row.Field<object>("Shippinginvoice_Dispensed")),
                       OrderInvoiceCouponAppliedAmount = Convert.IsDBNull(row.Field<object>("Orderinvoice_Couponamount")) ? 0 :
                                                    Convert.ToDecimal(row.Field<object>("Orderinvoice_Couponamount")),
                       OrderInvoiceCreditAppliedAmount = Convert.IsDBNull(row.Field<object>("Orderinvoice_Creditamount")) ? 0 :
                                                    Convert.ToDecimal(row.Field<object>("Orderinvoice_Creditamount")),

                       OrderCart = GetCustomerOrderCart(row.Field<int>("OrderInvoice_Customer_Fk"),
                           row.Field<int>("StoreFront_Id"),
                           row.Field<int>("OrderInvoice_Id")),
                       OrderinvoiceBPNotKnown = row.Field<object>("Orderinvoice_BPNotKnown") != DBNull.Value && Convert.ToBoolean(row.Field<object>("Orderinvoice_BPNotKnown"))

                   }).Where(x => x.CartCountForOrder != 0).OrderByDescending(x => x.OrderInvoiceId).ToList();

            }
            return customerClosedOrderList;
        }

        // get customer information for update account
        public CustomerMinimal GetCustomerInfoPartial(int customerId)
        {
            DataSet dsCustomer = _customerDbl.GetCustomerInfoPartial_ById(customerId);

            if (dsCustomer.Tables[0].Rows.Count > 0)
            {
                return new CustomerMinimal
                {
                    CustomerId = Convert.ToInt32(dsCustomer.Tables[0].Rows[0]["Customer_Id"]),
                    CustomerFirstName = dsCustomer.Tables[0].Rows[0]["Customer_FirstName"].ToString(),
                    CustomerLastName = dsCustomer.Tables[0].Rows[0]["Customer_LastName"].ToString(),
                    CustomerEmail = dsCustomer.Tables[0].Rows[0]["Customer_Email"].ToString(),
                    CustomerPassword = dsCustomer.Tables[0].Rows[0]["Customer_Password"].ToString(),
                    CustomerPhone = dsCustomer.Tables[0].Rows[0]["Customer_Phone"].ToString(),
                    CustomerEveningPhone = dsCustomer.Tables[0].Rows[0]["Customer_EveningPhone"].ToString(),
                    CustomerAddress = dsCustomer.Tables[0].Rows[0]["Customer_Address"].ToString(),
                    CustomerCity = dsCustomer.Tables[0].Rows[0]["Customer_City"].ToString(),
                    CustomerCountryId = Convert.ToInt32(dsCustomer.Tables[0].Rows[0]["Customer_Country_Fk"]),
                    CustomerProvinceId = Convert.ToInt32(dsCustomer.Tables[0].Rows[0]["Customer_ProvinceState_Fk"]),
                    CustomerZipCode = dsCustomer.Tables[0].Rows[0]["Customer_ZipCode"].ToString(),
                    CustomerFreeShipping = (bool)dsCustomer.Tables[0].Rows[0]["Customer_FreeShipping"],
                    CustomerFreeShippingOneYear = (bool)dsCustomer.Tables[0].Rows[0]["Customer_FreeShippingOneYear"],
                    CustomerFreeShippingStartDate = Convert.IsDBNull(dsCustomer.Tables[0].Rows[0]["Customer_FreeShippingStartDate"]) ? DateTime.MinValue : (DateTime)dsCustomer.Tables[0].Rows[0]["Customer_FreeShippingStartDate"],
                    CustomerIsPartial = dsCustomer.Tables[0].Rows[0]["Customer_IsPartial"] != DBNull.Value && Convert.ToBoolean(dsCustomer.Tables[0].Rows[0]["Customer_IsPartial"]),
                    CustomerTypedEmail = Convert.IsDBNull(dsCustomer.Tables[0].Rows[0]["Customer_TypedEmail"]) ? string.Empty : dsCustomer.Tables[0].Rows[0]["Customer_TypedEmail"].ToString(),
                    CustomerProvinceCode = Convert.IsDBNull(dsCustomer.Tables[0].Rows[0]["Provincestate_Code"]) ? string.Empty : dsCustomer.Tables[0].Rows[0]["Provincestate_Code"].ToString()

                };
            }
            else
            {
                return null;
            }

        }

        public int UpdateCustomerIsPartialFlag(int customerId, bool isPartial)
        {
            return _customerDbl.UpdateCustomerIsPartialFlag(customerId, isPartial);
        }

        // update general enquiry submitted from get in touch page
        public int AddGeneralEnquiry(int storefrontFk, string customerName, string customerEmail, string customerPhone, string queryPosted, DateTime dateCreated, string category, string zendeskTicketId)
        {
            int generalEnquiryId = _customerDbl.AddGeneralEnquiry(storefrontFk, customerName, customerEmail, customerPhone, queryPosted, dateCreated, category, zendeskTicketId);
            return generalEnquiryId;
        }

        //Upload Id document details  for customer
        public int AddCustomerIdDocumentDetail(int customerFK, int storeFrontFK, string fileName, bool active, DateTime dateCreated, string existingBackImage)
        {
            var customerDocumentID = _customerDbl.AddCustomerIdDocumentDetail(customerFK, storeFrontFK, fileName, active, dateCreated, existingBackImage);
            return customerDocumentID;
        }

        //Upload Prescription for customer
        public int AddCustomerPrescription(int customerFK, int orderInvoiceFk, int storeFrontFK, string fileName, bool active, DateTime dateCreated)
        {
            var customerDocumentID = _customerDbl.AddCustomerPrescription(customerFK, orderInvoiceFk, storeFrontFK, fileName, active, dateCreated);
            return customerDocumentID;
        }

        // converts customer data set to IEnumerable<customer>
        private IEnumerable<Customer> ConvertToCustomerList(DataSet customerDbSet)
        {
            // Linq version
            return (from DataRow drCustomer in customerDbSet.Tables[0].Rows
                    select new Customer
                    {
                        CustomerId = Convert.ToInt32(drCustomer["Customer_Id"]),
                        CustomerAffiliateId = drCustomer["Customer_Affiliate_Fk"] == DBNull.Value ? 0 : Convert.ToInt32(drCustomer["Customer_Affiliate_Fk"]),
                        CustomerStoreFrontName = drCustomer["StoreFront_Name_Pk"] == DBNull.Value ? "" : drCustomer["StoreFront_Name_Pk"].ToString(),
                        CustomerFirstName = drCustomer["Customer_FirstName"] == DBNull.Value ? "" : drCustomer["Customer_FirstName"].ToString(),
                        CustomerLastName = drCustomer["Customer_LastName"] == DBNull.Value ? "" : drCustomer["Customer_LastName"].ToString(),
                        CustomerSex = drCustomer["Customer_Sex"] == DBNull.Value ? "" : drCustomer["Customer_Sex"].ToString(),
                        CustomerDob = drCustomer["Customer_Dob"] == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(drCustomer["Customer_Dob"]),
                        CustomerType = drCustomer["Customer_Type"] == DBNull.Value ? "" : drCustomer["Customer_Type"].ToString(),
                        CustomerCreditLimit = drCustomer["Customer_CreditLimit"] == DBNull.Value ? 0m : Convert.ToDecimal(drCustomer["Customer_CreditLimit"]),
                        CustomerPaymentPending = drCustomer["Customer_PaymentPending"] != DBNull.Value && Convert.ToBoolean(drCustomer["Customer_PaymentPending"]),
                        CustomerReferralId = drCustomer["Customer_ReferralId"] == DBNull.Value ? "" : (drCustomer["Customer_ReferralId"]).ToString(),
                        CustomerPhone = drCustomer["Customer_Phone"] == DBNull.Value ? "" : drCustomer["Customer_Phone"].ToString(),
                        CustomerPhoneExt = drCustomer["Customer_Phone_Ext"] == DBNull.Value ? "" : drCustomer["Customer_Phone_Ext"].ToString(),
                        CustomerSecondaryPhone = drCustomer["Customer_SecondaryPhone"] == DBNull.Value ? "" : drCustomer["Customer_SecondaryPhone"].ToString(),
                        CustomerSecondaryPhoneExt = drCustomer["Customer_SecondaryPhone_ext"] == DBNull.Value ? "" : drCustomer["Customer_SecondaryPhone_ext"].ToString(),
                        CustomerFax = drCustomer["Customer_Fax"] == DBNull.Value ? "" : drCustomer["Customer_Fax"].ToString(),
                        CustomerEveningPhone = drCustomer["Customer_EveningPhone"] == DBNull.Value ? "" : drCustomer["Customer_EveningPhone"].ToString(),
                        CustomerEmail = drCustomer["Customer_Email"] == DBNull.Value ? "" : drCustomer["Customer_Email"].ToString(),
                        CustomerSecondaryEmail = drCustomer["Customer_SecondaryEmail"] == DBNull.Value ? "" : drCustomer["Customer_SecondaryEmail"].ToString(),
                        CustomerInvoiceEmail = drCustomer["Customer_InvoiceEmail"] == DBNull.Value ? "" : drCustomer["Customer_InvoiceEmail"].ToString(),
                        CustomerCountryId = drCustomer["Customer_Country_Fk"] == DBNull.Value ? 0 : Convert.ToInt32(drCustomer["Customer_Country_Fk"]),
                        CustomerProvinceId = drCustomer["Customer_ProvinceState_Fk"] == DBNull.Value ? 0 : Convert.ToInt32(drCustomer["Customer_ProvinceState_Fk"]),
                        CustomerCity = drCustomer["Customer_City"] == DBNull.Value ? "" : drCustomer["Customer_City"].ToString(),
                        CustomerZipCode = drCustomer["Customer_ZipCode"] == DBNull.Value ? "" : drCustomer["Customer_ZipCode"].ToString(),
                        CustomerFreeShipping = drCustomer["Customer_FreeShipping"] != DBNull.Value && Convert.ToBoolean(drCustomer["Customer_FreeShipping"]),
                        CustomerFreeShippingOneYear = drCustomer["Customer_FreeShippingOneYear"] != DBNull.Value && Convert.ToBoolean(drCustomer["Customer_FreeShippingOneYear"]),
                        CustomerFreeShippingStartDate = drCustomer["Customer_FreeShippingStartDate"] == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(drCustomer["Customer_FreeShippingStartDate"]),
                        CustomerActive = drCustomer["Customer_Active"] != DBNull.Value && Convert.ToBoolean(drCustomer["Customer_Active"]),
                        CustomerTestAccount = drCustomer["Customer_TestAccount"] != DBNull.Value && Convert.ToBoolean(drCustomer["Customer_TestAccount"]),
                        CustomerDateCreated = drCustomer["Customer_DateCreated"] == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(drCustomer["Customer_DateCreated"]),
                        CustomerLastModified = drCustomer["Customer_LastModified"] == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(drCustomer["Customer_LastModified"]),
                    }).ToList();

            // non Linq version
            //foreach (DataRow drCustomer in customerDbSet.Tables[0].Rows)
            //{
            //    var customer = new Customer
            //    {
            //        CustomerId = Convert.ToInt32(drCustomer["Customer_Id"]),
            //        CustomerAffiliateId = Convert.ToInt32(drCustomer["Customer_Affiliate_Fk"]),
            //        CustomerStoreFrontName = drCustomer["StoreFront_Name_Pk"].ToString(),
            //        CustomerFirstName = drCustomer["Customer_FirstName"].ToString(),
            //        CustomerLastName = drCustomer["Customer_LastName"].ToString(),
            //        CustomerSex = drCustomer["Customer_Sex"].ToString(),
            //        CustomerDob = Convert.ToDateTime(drCustomer["Customer_Dob"]),
            //        CustomerType = drCustomer["Customer_Type"].ToString(),
            //        CustomerCreditLimit = Convert.ToDecimal(drCustomer["Customer_CreditLimit"]),
            //        CustomerPaymentPending = Convert.ToBoolean(drCustomer["Customer_PaymentPending"]),
            //        CustomerReferalId = Convert.ToInt32(drCustomer["Customer_ReferralId"]),
            //        CustomerPhone = drCustomer["Customer_Phone"].ToString(),

            //        CustomerPhoneExt = drCustomer["Customer_Phone_Ext"].ToString(),
            //        CustomerSecondaryPhone = drCustomer["Customer_SecondaryPhone"].ToString(),
            //        CustomerSecondaryPhoneExt = drCustomer["Customer_SecondaryPhone_ext"].ToString(),
            //        CustomerFax = drCustomer["Customer_Fax"].ToString(),
            //        CustomerEveningPhone = drCustomer["Customer_EveningPhone"].ToString(),
            //        CustomerEmail = drCustomer["Customer_Email"].ToString(),
            //        CustomerSecondaryEmail = drCustomer["Customer_SecondaryEmail"].ToString(),
            //        CustomerInvoiceEmail = drCustomer["Customer_InvoiceEmail"].ToString(),
            //        CustomerCountryId = Convert.ToInt32(drCustomer["Customer_Country_Fk"]),
            //        CustomerProvinceId = Convert.ToInt32(drCustomer["Customer_Province_Fk"]),
            //        CustomerCity = drCustomer["Customer_City"].ToString(),
            //        CustomerZipCode = drCustomer["Customer_ZipCode"].ToString(),
            //        CustomerFreeShipping = Convert.ToBoolean(drCustomer["Customer_FreeShipping"]),
            //        CustomerFreeShippingOneYear = Convert.ToBoolean(drCustomer["Customer_FreeShippingOneYear"]),
            //        CustomerFreeShippingStartDate = Convert.ToDateTime(drCustomer["Customer_FreeShippingStartDate"]),
            //        CustomerActive = Convert.ToBoolean(drCustomer["Customer_Active"]),
            //        CustomerTestAccount = Convert.ToBoolean(drCustomer["Customer_TestAccount"]),
            //        CustomerDateCreated = Convert.ToDateTime(drCustomer["Customer_DateCreated"]),
            //        CustomerLastModified = Convert.ToDateTime(drCustomer["Customer_LastModified"]),

            //    };

            //    customerList.Add(customer);
            //}
        }
        public List<CustomerIdDocument> GetCustomerIdDocumentByCustomerId(int customerId)
        {
            DataSet dsCustomerIdDocument = _customerDbl.GetCustomerIdDocumentByCustomerId(customerId);

            List<CustomerIdDocument> customerIdDocumentList = null;

            if (dsCustomerIdDocument.Tables[0].Rows.Count > 0)
            {
                customerIdDocumentList =
                    dsCustomerIdDocument.Tables[0].AsEnumerable()
                        .Select(row => new CustomerIdDocument
                        {
                            CustomerIdDocumentId = row.Field<int>("CustomerIdDocument_Id"),
                            CustomerIdDocumentActive = row.Field<object>("CustomerIdDocument_Active") != DBNull.Value && Convert.ToBoolean(row.Field<object>("CustomerIdDocument_Active")),
                            CustomerIdDocumentFileName = Convert.IsDBNull(row.Field<string>("CustomerIdDocument_FileName")) ? string.Empty : row.Field<string>("CustomerIdDocument_FileName"),
                            CustomerIdDocumentBackSideFileName = Convert.IsDBNull(row.Field<string>("CustomerIdDocument_BackFileName")) ? string.Empty : row.Field<string>("CustomerIdDocument_BackFileName"),
                            CustomerIdDocumentIsValid = row.Field<object>("CustomerIdDocument_IsValidated") != DBNull.Value && Convert.ToBoolean(row.Field<object>("CustomerIdDocument_IsValidated")),
                            CustomerIdDocumentCustomerFk = row.Field<int>("CustomerIdDocument_Customer_FK"),
                            CustomerIdDocumentComment = Convert.IsDBNull(row.Field<string>("CustomerIdDocument_Comment")) ? string.Empty : row.Field<string>("CustomerIdDocument_Comment"),
                            CustomerIdDocumentStoreFrontFk = row.Field<int>("CustomerIdDocument_StoreFront_Fk"),
                            CustomerIdDocumentDateCreated = Convert.IsDBNull(row.Field<object>("CustomerIdDocument_DateCreated")) ? DateTime.MinValue : Convert.ToDateTime(row.Field<object>("CustomerIdDocument_DateCreated")),
                            CustomerIdDocumentExpiryDate = Convert.IsDBNull(row.Field<object>("CustomerIdDocument_ExpiryDate")) ? DateTime.MinValue : Convert.ToDateTime(row.Field<object>("CustomerIdDocument_ExpiryDate")),


                        }).ToList();

            }
            return customerIdDocumentList;
        }
        public bool UpdateCustomerIdInActive(int documentId, int customerFK, int storeFrontFK)
        {
            return _customerDbl.UpdateCustomerIdInActive(documentId, customerFK, storeFrontFK);
        }

        // check if user has order of same amount in past 30 minutes
        public bool IfUserAlreadyAnsweredQuestionnaire(int catId, int customerId, DateTime currentTimeStamp, int orderTimeSpan, int orderInvoiceFk =0)
        {
            DataSet dsQuestionnaireAnswered = _customerDbl.IfUserAlreadyAnsweredQuestionnaire(catId, customerId, currentTimeStamp, orderTimeSpan, orderInvoiceFk);

            bool answeredQuestionnaire = false;

            if (dsQuestionnaireAnswered.Tables[0].Rows.Count > 0)
            {
                answeredQuestionnaire = true;

            }
            return answeredQuestionnaire;
        }

        public QuestionnaireCategoryResponse GetQuestionnairCategoryResponseByCategory(int customerFk, int productCategoryFk, int orderInvoiceFk)
        {
            DataSet dsCustomer = _customerDbl.GetQuestionnairCategoryResponseByCategory(customerFk, productCategoryFk, orderInvoiceFk);
            if (dsCustomer.Tables.Count > 0)
            {
                if (dsCustomer.Tables[0].Rows.Count > 0)
                {
                    return dsCustomer.Tables[0].AsEnumerable()
                            .Select(row => new QuestionnaireCategoryResponse

                            {
                                CustomerQuestionnaireCategoryResponseApprove = Convert.IsDBNull(row.Field<object>("CustomerQuestionnaireCategoryResponse_Approve")) ? 0 : Convert.ToInt32(row.Field<object>("CustomerQuestionnaireCategoryResponse_Approve")),
                                CustomerQuestionnaireCategoryResponseId = Convert.IsDBNull(row.Field<object>("CustomerQuestionnaireCategoryResponse_Id")) ? 0 : Convert.ToInt32(row.Field<object>("CustomerQuestionnaireCategoryResponse_Id")),
                                CustomerQuestionnaireCategoryResponseDateCreated = (DateTime)row.Field<object>("CustomerQuestionnaireCategoryResponse_DateCreated"),
                                CustomerPrescriptionId = Convert.IsDBNull(row.Field<object>("CustomerPrescription_Id")) ? 0 : Convert.ToInt32(row.Field<object>("CustomerPrescription_Id")),
                                CustomerPrescriptionIsValidated = Convert.IsDBNull(row.Field<object>("CustomerPrescription_IsValidated")) ? 0 : Convert.ToInt32(row.Field<object>("CustomerPrescription_IsValidated")),
                                CustomerPrescriptionApproved = Convert.IsDBNull(row.Field<object>("CustomerPrescription_Approved")) ? 0 : Convert.ToInt32(row.Field<object>("CustomerPrescription_Approved")),
                                CustomerPrescriptionRefillAuthorization = row.Field<object>("QuestionnairePrescriptionDetails_RefillAuthorization") != DBNull.Value &&
                                                        Convert.ToBoolean(row.Field<object>("QuestionnairePrescriptionDetails_RefillAuthorization")),
                                CustomerPrescriptionRefillCount = Convert.IsDBNull(row.Field<object>("QuestionnairePrescriptionDetails_RefillsAvailable")) ? 0 : 
                                                                    Convert.ToInt32(row.Field<object>("QuestionnairePrescriptionDetails_RefillsAvailable")),

                            }).FirstOrDefault();
                }
            }
            
                return null;
        }
        public bool UpdatePrescriptionInActive(int documentId, int customerFK, int orderInvoiceId, int storeFrontFK)
        {
            return _customerDbl.UpdatePrescriptionInActive(documentId, customerFK,orderInvoiceId, storeFrontFK);
        }

        public int AddLogCosultationConsent( bool? updatedvalue, int patientProfileFk, int storefronFk, string actionType, DateTime dateCreated)
        {
            return _customerDbl.AddLogCosultationConsent(updatedvalue, patientProfileFk, storefronFk, actionType, dateCreated);
        }

        public List<CustomerSubscription> GetCustomerAllSubscription(int customerFk)
        {
            DataSet dsSubscription = _customerDbl.GetCustomerAllSubscription(customerFk);

            List<CustomerSubscription> SubscriptionList = null;
            if (dsSubscription.Tables[0].Rows.Count > 0)
            {
                SubscriptionList =
                    dsSubscription.Tables[0].AsEnumerable()
                        .Select(row => new CustomerSubscription
                        {
                            CustomerSubscriptionId = Convert.IsDBNull(row.Field<object>("CustomerSubscription_id")) ? 0 : Convert.ToInt32(row.Field<object>("CustomerSubscription_id")),
                            SubscriptionSignUpDate = (DateTime)row.Field<object>("CustomerSubscription_DateSubscriptionSignUp"),
                            SubscriptionProductSizeFk = Convert.IsDBNull(row.Field<object>("CustomerSubscription_ProductSize_fk")) ? 0 : Convert.ToInt32(row.Field<object>("CustomerSubscription_ProductSize_fk")),
                            SubscriptionQuantity = Convert.IsDBNull(row.Field<object>("CustomerSubscription_Quantity"))? 0 : Convert.ToDecimal(row.Field<object>("CustomerSubscription_Quantity")),
                            ProductName = Convert.IsDBNull(row.Field<object>("Product_Name"))? string.Empty : Convert.ToString(row.Field<object>("Product_Name")),
                            ProductSizeStrength = Convert.IsDBNull(row.Field<object>("Productsize_Strength")) ? string.Empty : Convert.ToString(row.Field<object>("Productsize_Strength")),
                            ProductSizeUnit = Convert.IsDBNull(row.Field<object>("ProductSize_Unit")) ? string.Empty : Convert.ToString(row.Field<object>("ProductSize_Unit")),
                            ProducSizeQuantity = Convert.IsDBNull(row.Field<object>("Productsize_Quantity")) ? 0 : Convert.ToDecimal(row.Field<object>("Productsize_Quantity")),
                            ProductSizeGeneric = row.Field<object>("Productsize_Generic") != DBNull.Value && Convert.ToBoolean(row.Field<object>("Productsize_Generic"))

                        }).ToList();
            }
            return SubscriptionList;
        }

        public int UnSubscribeAutoRefill(int subscriptionId)
        {
            return _customerDbl.UnSubscribeAutoRefill(subscriptionId);
        }

        public int AddLogCustomerSubscription(int customerFk, int SubscriptionFk, bool updatedValue, string actionType, DateTime dateCreated)
        {
            return _customerDbl.AddLogCustomerSubscription(customerFk, SubscriptionFk, updatedValue, actionType, dateCreated);
        }
        // check if user has order of same amount in past 30 minutes
        public List<CustomerLastOrder> GetCustomerLastOrder(int storeFrontFk,
            int customerId,
          
            DateTime currentTimeStamp,
            int orderTimeSpan)
        {
            DataSet dsDuplicateOrders = _customerDbl.GetCustomerLastOrder(storeFrontFk, customerId, currentTimeStamp, orderTimeSpan);

            List<CustomerLastOrder> duplicateOrders = null;

            if (dsDuplicateOrders.Tables[0].Rows.Count > 0)
            {
                duplicateOrders =
                    dsDuplicateOrders.Tables[0].AsEnumerable()
                        .Select(row => new CustomerLastOrder
                        {
                            OrderInvoiceId = Convert.IsDBNull(row.Field<object>("Orderinvoice_Id")) ? 0 : Convert.ToInt32(row.Field<object>("Orderinvoice_Id")),
                            OrderInvoiceDateCreated = (DateTime)row.Field<object>("OrderInvoice_DateCreated"),
                            OrderCartTotal = Convert.IsDBNull(row.Field<object>("OrderInvoice_CartTotal")) ? 0.0m : Convert.ToDecimal(row.Field<object>("OrderInvoice_CartTotal")),
                            OrderPlacedMinutesAgo = Convert.IsDBNull(row.Field<object>("Orderinvoice_PlacedMinutesAgo")) ? 0 : Convert.ToInt32(row.Field<object>("Orderinvoice_PlacedMinutesAgo")),

                        }).ToList();

            }
            return duplicateOrders;
        }
        public int UpdateOrderConsultationFlag(int orderFk, bool? orderConsultationFlag)
        {
            int updateStatus = _customerDbl.UpdateOrderConsultationFlag(orderFk, orderConsultationFlag);

            return updateStatus;

        }

        public int UpdateCustomerIdBackFileName(int customerIdDocumentFk, string backSideFileName, int customerFK, int storeFrontFK, DateTime modifiedDate)
      
        {
            return _customerDbl.UpdateCustomerIdBackFileName(customerIdDocumentFk,backSideFileName, customerFK, storeFrontFK, modifiedDate);
        }

        public int AddLogCustomerIdDocument(int customerDocumentId, int customerFk, int storefronFk, string actionType,
                                DateTime dateCreated, bool mobileDevice, string module, string message)
        {
            return _customerDbl.AddLogCustomerIdDocument(customerDocumentId, customerFk, storefronFk, actionType, 
                            dateCreated, mobileDevice, module, message);
        }

        public int UpdateCartItemPatientProfile(int orderFk, int patientProfileFk)
        {
            int updateStatus = _customerDbl.UpdateCartItemPatientProfile(orderFk, patientProfileFk);

            return updateStatus;
        }

        public int AddLogCongnitoSignUpResponse(CognitoSignupResponse cognitoSigupResponse, int storefrontFk, int customerFk, string ipAddress, DateTime dateCreated, string response)
        {
            int updateStatus = _customerDbl.AddLogCongnitoSignUpResponse(cognitoSigupResponse, storefrontFk, customerFk, ipAddress, dateCreated, response);
            return updateStatus;
        }

        public int AddLogCognitoSignUpVerification(CognitoConfirmSignupResponse cognitoConfirmSignupResponse, int storefrontFk, int customerFk,
                                            DateTime verifiedDate, string ipAddress, string response, int actionType, bool userRequested)
        {
            int updateStatus = _customerDbl.AddLogCognitoSignUpVerification(cognitoConfirmSignupResponse, storefrontFk,
                                customerFk, verifiedDate, ipAddress, response, actionType, userRequested);
            return updateStatus;
        }

        public int AddLogCognitoSignupAuthSession(SrpAuthResponse srpAuthResponse, int storefrontFk, int customerFk,
                                           DateTime timeStamp, string ipAddress, string response)
        {
            int updateStatus = _customerDbl.AddLogCognitoSignupAuthSession(srpAuthResponse, storefrontFk, customerFk, timeStamp, ipAddress, response);
            return updateStatus;
        }

        public int UpdateCustomerCognitoUserId(int customerId, int cognitoUserId)
        {
            int updateStatus = _customerDbl.UpdateCustomerCognitoUserId(customerId, cognitoUserId);
            return updateStatus;
        }

        public int AddLogCognitoAuthFlowMfaResponse(AuthFlowMfaResponse authFlowMfaResponse, int storefrontFk, int customerFk,
                                           string ipAddress, string response, DateTime dateCreated, int actionType)
        {
            int updateStatus = _customerDbl.AddLogCognitoAuthFlowMfaResponse(authFlowMfaResponse, storefrontFk, customerFk, ipAddress, response, dateCreated, actionType);
            return updateStatus;
        }

        public int AddLogCognitoUserAttributesResponse(UserAttributeResponse userAttributeResponse, CustomerSignUp customerInfo, int storefrontFk, int customerFk,
                                          string ipAddress, string response, DateTime dateCreated, int actionType)
        {
            int updateStatus = _customerDbl.AddLogCognitoUserAttributesResponse(userAttributeResponse, customerInfo, storefrontFk,
                                    customerFk, ipAddress, response, dateCreated, actionType);
            return updateStatus;
        }

        public int AddLogCognitoResetPassword(CognitoSignupResponse cognitoResponse, int storefrontFk, string customerEmail, string resetCode,
                                         string ipAddress, string response, DateTime dateCreated, int actionType)
        {
            int updateStatus = _customerDbl.AddLogCognitoResetPassword(cognitoResponse, storefrontFk,
                                    customerEmail, resetCode, ipAddress, response, dateCreated, actionType);
            return updateStatus;
        }


        public int AddLogCognitoSendEmailVerification(CognitoSignupResponse cognitoResponse, int storefrontFk, string customerEmail,
                                          string ipAddress, string response, DateTime dateCreated, int actionType, bool userRequested)
        {
            int updateStatus = _customerDbl.AddLogCognitoSendEmailVerification(cognitoResponse, storefrontFk,
                                    customerEmail, ipAddress, response, dateCreated, actionType, userRequested);
            return updateStatus;
        }

        public List<ConsultationHours> GetConsultationHours(int customerfk)
        {
            DataSet dsConsultationHours = _customerDbl.GetConsultationHours(customerfk);

            List<ConsultationHours> consultationHoursList = null;

            if (dsConsultationHours.Tables[0].Rows.Count > 0)
            {
                consultationHoursList =
                    dsConsultationHours.Tables[0].AsEnumerable()
                        .Select(row => new ConsultationHours
                        {
                            ConsultationHoursId = Convert.IsDBNull(row.Field<object>("ConsultationHours_Id")) ? 0 : Convert.ToInt32(row.Field<object>("ConsultationHours_Id")),
                            ConsultationHour = Convert.IsDBNull(row.Field<object>("ConsultationHours_Time"))? string.Empty : Convert.ToString(row.Field<object>("ConsultationHours_Time")),
                            IsSelected= row.Field<object>("PatientConsultationDetails_Active") != DBNull.Value && Convert.ToBoolean(row.Field<object>("PatientConsultationDetails_Active"))
                        }).ToList();

            }
            return consultationHoursList;
        }
        public int AddPatientConsultationHours(int consultationHoursFk, int customerFk, bool active, DateTime startDate, DateTime? endDate)
        {
            int updateStatus = _customerDbl.AddPatientConsultationHours(consultationHoursFk, customerFk,
                                    active, startDate, endDate);
            return updateStatus;
        }

        public List<PatientConsultationHours> GetPatientPreferedConsultationHour(int patientConsultationDetailsId, int consultationHoursFk, int customerFk)
        {
            DataSet dsConsultationHours = _customerDbl.GetPatientPreferedConsultationHour(patientConsultationDetailsId, consultationHoursFk, customerFk);

            List<PatientConsultationHours> consultationHoursList = null;

            if (dsConsultationHours.Tables[0].Rows.Count > 0)
            {
                consultationHoursList =
                    dsConsultationHours.Tables[0].AsEnumerable()
                        .Select(row => new PatientConsultationHours
                        {
                            PatientConsultationHoursFk = Convert.IsDBNull(row.Field<object>("PatientConsultationDetails_ConsultationHours_Fk")) ? 0 : Convert.ToInt32(row.Field<object>("PatientConsultationDetails_ConsultationHours_Fk")),
                            PatientConsultationDetailsId = Convert.IsDBNull(row.Field<object>("PatientConsultationDetails_Id")) ? 0 : Convert.ToInt32(row.Field<object>("PatientConsultationDetails_Id")),
                            PatientConsultationHoursCustomerFk = Convert.IsDBNull(row.Field<object>("PatientConsultationDetails_Customer_Fk")) ? 0 : Convert.ToInt32(row.Field<object>("PatientConsultationDetails_Customer_Fk")),
                        }).ToList();

            }
            return consultationHoursList;
        }

        public int InActivatePatientConsultationHours(int customerId, string patientConsultationHoursIds, DateTime endDate)
        {
            return _customerDbl.InActivatePatientConsultationHours(customerId, patientConsultationHoursIds, endDate);
        }

        // get social history list
        public IEnumerable<SocialHistory> GetSocialHistoryList()
        {
            DataSet dsSocialHistory = _customerDbl.GetSocialHistoryList();
            List<SocialHistory> socialHistoryList = null;

            if (dsSocialHistory.Tables[0].Rows.Count > 0)
            {
                socialHistoryList =
                   dsSocialHistory.Tables[0].AsEnumerable()
                   .Select(row => new SocialHistory
                   {

                       SocialHistoryId = row.Field<int>("SocialHistory_Id"),
                       SocialHistoryName = row.Field<string>("SocialHistory_Name"),
                     

                   }).ToList();
            }
            return socialHistoryList;
        }

        public int UpdateCustomerSignUpEmailVerifiedFlag(int customerId, bool emailVerified)
        {
            return _customerDbl.UpdateCustomerSignUpEmailVerifiedFlag(customerId, emailVerified);
        }
        public DataSet GetDeveloperLogin(string email, int storeFrontId)
        {
            DataSet ds = _customerDbl.GetDeveloperLogin(email, storeFrontId);
            return ds;
        }

        public List<CustomerShippingAddress> GetCustomerShippingAddress(int customerFk, int shippingAddressFk)
        {
            DataSet dsCustomer = _customerDbl.GetCustomerShippingAddress(customerFk, shippingAddressFk);
            List<CustomerShippingAddress> shippingAddress = new List<CustomerShippingAddress>();
            if (dsCustomer.Tables[0].Rows.Count > 0)
            {
                shippingAddress =
                   dsCustomer.Tables[0].AsEnumerable()
                   .Select(row => new CustomerShippingAddress
                   {
                       CustomerId = Convert.IsDBNull(row.Field<object>("CustomerShippingAddress_Customer_Fk")) ? 0 : Convert.ToInt32(row.Field<object>("CustomerShippingAddress_Customer_Fk")),
                       CustomerAddressId = Convert.IsDBNull(row.Field<object>("CustomerShippingAddress_Id"))?0 : Convert.ToInt32(row.Field<object>("CustomerShippingAddress_Id")),
                       CustomerFirstName = Convert.IsDBNull(row.Field<object>("CustomerShippingAddress_FirstName")) ? string.Empty : row.Field<string>("CustomerShippingAddress_FirstName"),
                       CustomerLastName = Convert.IsDBNull(row.Field<object>("CustomerShippingAddress_LastName")) ? string.Empty : row.Field<string>("CustomerShippingAddress_LastName"),
                     
                       CustomerAddress = Convert.IsDBNull(row.Field<object>("CustomerShippingAddress_Address")) ? string.Empty : row.Field<string>("CustomerShippingAddress_Address"),
                       CustomerCity = Convert.IsDBNull(row.Field<object>("CustomerShippingAddress_City")) ? string.Empty : row.Field<string>("CustomerShippingAddress_City"),
                      
                       
                       CustomerZipCode = Convert.IsDBNull(row.Field<object>("CustomerShippingAddress_ZipCode")) ? string.Empty : row.Field<string>("CustomerShippingAddress_ZipCode"),
                       
                       ShippingProvinceId = Convert.IsDBNull(row.Field<object>("CustomerShippingAddress_Provincestate_Fk")) ?0 : row.Field<int>("CustomerShippingAddress_Provincestate_Fk"),
                       BillingProvinceName =  Convert.IsDBNull(row.Field<object>("Provincestate_Name")) ?string.Empty :row.Field<string>("Provincestate_Name"),
                       ShippingCountryId = Convert.IsDBNull(row.Field<object>("CustomerShippingAddress_Country_Fk")) ? 0 : row.Field<int>("CustomerShippingAddress_Country_Fk"),
                       CustomerCountryName = Convert.IsDBNull(row.Field<object>("Country_Name")) ? string.Empty : row.Field<string>("Country_Name"),
                       CustomerDefaultAddress = row.Field<object>("CustomerShippingAddress_Default") != DBNull.Value && Convert.ToBoolean(row.Field<object>("CustomerShippingAddress_Default"))
                   }).ToList();
            }
            return shippingAddress;
        }

        public int AddCustomerShippingAddress(int customerFk, string customerFirstName, string customerLastName,
                                            string customerPhone,
                                            string shippingAddress, string shippingCity,
                                           int countryFk, int provinceFk,
                                           string zipCode, bool addressActive, bool defaultAddress, DateTime dateCreated)
        {
            var shippingAddressId = _customerDbl.AddCustomerShippingAddress(customerFk, customerFirstName, customerLastName, customerPhone,
                                                shippingAddress, shippingCity, countryFk, provinceFk, zipCode, addressActive, defaultAddress, dateCreated);
            return shippingAddressId;
        }

        public int UpdateShippingAddress(int shippingAdressFk, string firstName, string lastName,
                string address, string city, string zipcode, int provinceFk, int countryFk)
        {
            var updateStatus = _customerDbl.UpdateShippingAddress(shippingAdressFk, firstName, lastName,
                                        address, city, zipcode, provinceFk, countryFk);
            return updateStatus;
        }

        public int UpdateCustomerShippingAddressDefaultFlag(int customerId, int customerShippingAddressId, bool isDefault)                
        {
            var updateStatus = _customerDbl.UpdateCustomerShippingAddressDefaultFlag(customerId, customerShippingAddressId, isDefault);
            return updateStatus;
        }

        public int UpdateCustomerShippingAddressActiveFlag(int customerId, int customerShippingAddressId, bool isActive)
        {
            var updateStatus = _customerDbl.UpdateCustomerShippingAddressActiveFlag(customerId, customerShippingAddressId, isActive);
            return updateStatus;
        }

        public int UpdateCustomerCredentials(int customerId, string email, string phone)
        {
            var updateStatus = _customerDbl.UpdateCustomerCredentials(customerId, email, phone);
            return updateStatus;
        }

        public int AddLogCustomerShippingAddress(List<LogCustomerShippingAddress> logCustomerShippingAddress)
        {
            var updateStatus = _customerDbl.AddLogCustomerShippingAddress(logCustomerShippingAddress);
            return updateStatus;
        }

        // Get Cart Details for an Ship Invoice Id
        public CartDetail GetCustomerOrderCartByShipInvoice(int customerId, int customerStorefrontId, int orderId)
        {
            DataSet dsCustomerOrderCart = _customerDbl.GetCustomerOrderCartByShipInvoice(customerId, customerStorefrontId, orderId);

            CartDetail customerOrderCart = null;

            if (dsCustomerOrderCart.Tables[0].Rows.Count > 0)
            {
                customerOrderCart =
                   dsCustomerOrderCart.Tables[0].AsEnumerable()
                   .Select(row => new CartDetail
                   {
                       CartShippingInvoiceFk = row.Field<int>("Cart_ShippingInvoice_Fk"),
                       CartItemPrice = row.Field<decimal>("Cart_ItemPrice"),
                       CartItemQuantity = row.Field<int>("Cart_ItemQuantity"),
                       ProductSizeId = row.Field<int>("productsize_id"),
                       ProductName = row.Field<string>("Product_Name"),
                       // ProductSizeHeader = Convert.IsDBNull(row.Field<string>("ProductSize_Header")) ? string.Empty : row.Field<string>("ProductSize_Header"),  
                       ProductSizeHeader = row.Field<string>("ProductSize_Header"),
                       ProductSizeStrength = row.Field<string>("ProductSize_Strength"),
                       ProductSizeQuantity = row.Field<decimal>("ProductSize_Quantity"),
                       ProductId = Convert.IsDBNull(row.Field<object>("Product_Id")) ? 0 : Convert.ToInt32(row.Field<object>("Product_Id")),
                       ProductUnitNamePk = Convert.IsDBNull(row.Field<object>("ProductUnit_Name_Pk")) ? string.Empty : row.Field<string>("ProductUnit_Name_Pk"),
                       ProductSizeGeneric = row.Field<bool>("Productsize_Generic"),
                       IsRefillCartItem = row.Field<object>("Cart_IsRefillCartItem") != DBNull.Value && Convert.ToBoolean(row.Field<object>("Cart_IsRefillCartItem")),
                       OriginalRefillOrderShippIngInvoiceFk = Convert.IsDBNull(row.Field<object>("Cart_RefillShipInvoice_Fk")) ? 0 : Convert.ToInt32(row.Field<object>("Cart_RefillShipInvoice_Fk")),
                       OrderInvoiceFk = Convert.IsDBNull(row.Field<object>("cart_OrderInvoice_Fk")) ? 0 : Convert.ToInt32(row.Field<object>("cart_OrderInvoice_Fk")),
                       NumberOfRefillsAllowed = Convert.IsDBNull(row.Field<object>("QuestionnairePrescriptionDetails_RefillNumbers")) ? 0 :
                                                Convert.ToInt32(row.Field<object>("QuestionnairePrescriptionDetails_RefillNumbers")),
                       NumberOfRefillsAvailable = Convert.IsDBNull(row.Field<object>("QuestionnairePrescriptionDetails_RefillsAvailable")) ? 0 :
                                                Convert.ToInt32(row.Field<object>("QuestionnairePrescriptionDetails_RefillsAvailable")),
                       RefillOrdersPlaced = Convert.IsDBNull(row.Field<object>("RefillOrdersPlaced")) ? 0 :
                                                    Convert.ToInt32(row.Field<object>("RefillOrdersPlaced")),

                       //ProductQuestionnaireCatId = Convert.IsDBNull(row.Field<object>("Product_QuestionnaireCategory_Fk")) ? 0 : Convert.ToInt32(row.Field<object>("Product_QuestionnaireCategory_Fk")),

                       //IsQuestionnaireAnswered = IfUserAlreadyAnsweredQuestionnaire(Convert.IsDBNull(row.Field<object>("Product_QuestionnaireCategory_Fk")) ? 0 : Convert.ToInt32(row.Field<object>("Product_QuestionnaireCategory_Fk")),
                       //                  customerId, DateTime.Now, SiteConfigurations.QuestionnaireTimeSpan, orderId),
                       //QuestionnaireCategoryResponse = GetQuestionnairCategoryResponseByCategory(customerId,
                       //         Convert.IsDBNull(row.Field<object>("Product_QuestionnaireCategory_Fk")) ? 0 :
                       //                     Convert.ToInt32(row.Field<object>("Product_QuestionnaireCategory_Fk")), orderId)


                   }).FirstOrDefault();

            }
            return customerOrderCart;
        }
    }
}
