using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using EnhanceClub.Domain.Entities;
using EnhanceClub.Domain.Entities.Enum;
using EnhanceClub.Domain.Helpers;

namespace EnhanceClub.Domain.Concrete
{
    public class OrderDbLayer
    {

        //readonly string _sCon = ConfigurationManager.ConnectionStrings["Connection"].ToString();

        private readonly string _sCon = @SiteConfigurations.SCon;

        DataSet _ds;

        // validate coupon 
        public DataSet ValidateCoupon(string couponCode, int storeFrontId, DateTime thisDate)
        {
            try
            {
                _ds = SqlHelper.ExecuteDataset(_sCon, "usp_ValidateCoupon", couponCode, storeFrontId, thisDate);
            }
            catch (Exception ex)
            {
                _ds = null;
                throw ex;
            }
            return _ds;
        }

        // check if coupon is max used by customer
        public int GetCouponUsageCountByCustomer(int customerId, int couponId, int storeFrontId)
        {
            try
            {
                SqlParameter[] paramCollection = SqlHelperParameterCache.GetSpParameterSet(_sCon, "usp_getCouponUsageCount", true);

                paramCollection[1].Value = customerId;
                paramCollection[2].Value = couponId;
                paramCollection[3].Value = storeFrontId;
                paramCollection[4].Value = null; // count of usage

                SqlHelper.ExecuteNonQuery(_sCon, CommandType.StoredProcedure, "usp_getCouponUsageCount", paramCollection);

                var value = paramCollection[4].Value;
                if (value != null)
                {
                    return Convert.ToInt32(value.ToString());
                }
                return 0;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        // check if customer is allowed to use a coupon
        public bool IsCustomerAllowedCoupon(int customerId, int couponId, int storeFrontId)
        {
            try
            {
                SqlParameter[] paramCollection = SqlHelperParameterCache.GetSpParameterSet(_sCon, "usp_IsCustomerAllowedCoupon", true);

                paramCollection[1].Value = customerId;
                paramCollection[2].Value = couponId;
                paramCollection[3].Value = storeFrontId;
                paramCollection[4].Value = null; // customer is allowed or not

                SqlHelper.ExecuteNonQuery(_sCon, CommandType.StoredProcedure, "usp_IsCustomerAllowedCoupon", paramCollection);

                var value = paramCollection[4].Value;
                if (value != null)
                {
                    return Convert.ToBoolean(value.ToString());
                }
                return false;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        // add record to order transaction

        public int AddOrderTransaction(int customerId,
                                       int orderInvoiceFk,
                                       int orderTempFk,
                                       int paymentOptionFk,
                                       string cardCvv2,
                                       string cardName,
                                       string cardNum,
                                       string cardType,
                                       string transactionIp,
                                       string statusMessage,
                                       string transactionNum,
                                       decimal transactionAmount,
                                       int cardCvv2Type,
                                       int cardExpireMonth,
                                       int cardExpireYear,
                                       int transactionError,
                                       DateTime transactionDate,
                                       int storeFrontFk,
                                       int paymentTransactionTypeFk = 0
                                       )
        {
            try
            {
                SqlParameter[] paramCollection = SqlHelperParameterCache.GetSpParameterSet(_sCon, "usp_addOrderTransaction", true);

                paramCollection[1].Value = customerId;
                paramCollection[2].Value = orderInvoiceFk;
                paramCollection[3].Value = orderTempFk;

                paramCollection[4].Value = paymentOptionFk;
                paramCollection[5].Value = cardCvv2;
                paramCollection[6].Value = cardName;

                paramCollection[7].Value = cardNum;

                paramCollection[8].Value = cardType;
                paramCollection[9].Value = transactionIp;
                paramCollection[10].Value = statusMessage;

                paramCollection[11].Value = transactionNum;
                paramCollection[12].Value = transactionAmount;
                paramCollection[13].Value = cardCvv2Type;

                paramCollection[14].Value = cardExpireMonth;
                paramCollection[15].Value = cardExpireYear;
                paramCollection[16].Value = transactionError;

                paramCollection[17].Value = transactionDate;
                paramCollection[18].Value = storeFrontFk;

                paramCollection[19].Value = paymentTransactionTypeFk;


                paramCollection[20].Value = null;  // new transaction Id generated
                paramCollection[21].Value = null;  // message
                paramCollection[22].Value = null;  // sqlUsed

                SqlHelper.ExecuteNonQuery(_sCon, CommandType.StoredProcedure, "usp_addOrderTransaction", paramCollection);

                var value = paramCollection[20].Value;
                var msg = paramCollection[21].Value;
                var sqlUsed = paramCollection[22].Value;


                if (value != null)
                {
                    return Convert.ToInt32(value.ToString());
                }
                return 0;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        // Add Order Invoice
        public int AddOrderInvoice(int customerCountryId,
                                   int customerProvinceId,
                                   int couponId,
                                   int storeFrontCurrencyId,
                                   int customerId,
                                   int shippingCountryId,
                                   int shippingOptionId,
                                   int shippingProvinceId,
                                   int userAdminCreateFk,
                                   int userAdminProcessing,
                                   string customerAddress,
                                   string customerCity,
                                   string customerEmail,
                                   string customerFirstName,
                                   string customerLastName,
                                   string customerPhone,
                                   string customerZipCode,
                                   string shippingAddress,
                                   string shippingCity,
                                   string shippingEmail,
                                   string shippingFirstName,
                                   string shippingLastName,
                                   string shippingPhone,
                                   string shippingZipCode,
                                   decimal couponAmount,
                                   decimal creditApplied,
                                   decimal storeFrontCurrencyExchangeRate,
                                   decimal shippingPrice,
                                   int transferOrder,
                                   int orderInvoiceActive,
                                   int cpPaymentPending,
                                   DateTime dateCreated,
                                   DateTime lastModified,
                                   decimal frontOfTheLinePrice,
                                   int frontOfTheLineFk,
                                    decimal provinceTaxPercent,
                                   decimal provinceTaxAmount,
                                    decimal globalTaxPercent,
                                    decimal globalTaxAmount,
                                    decimal harmonizedTaxPercent,
                                    decimal harmonizedTaxAmont,
                                    bool? consultationConsent,
                                    int paymentTransactionType)
        {
            try
            {
                SqlParameter[] paramCollection = SqlHelperParameterCache.GetSpParameterSet(_sCon, "usp_addOrderInvoiceWithTax", true);

                paramCollection[1].Value = customerCountryId;
                paramCollection[2].Value = customerProvinceId;
                paramCollection[3].Value = couponId;

                paramCollection[4].Value = storeFrontCurrencyId;
                paramCollection[5].Value = customerId;
                paramCollection[6].Value = shippingCountryId;

                paramCollection[7].Value = shippingOptionId;
                paramCollection[8].Value = shippingProvinceId;
                paramCollection[9].Value = userAdminCreateFk;

                paramCollection[10].Value = userAdminProcessing;
                paramCollection[11].Value = CommonFunctions.StripApostropheSymbol(customerAddress);
                paramCollection[12].Value = CommonFunctions.StripApostropheSymbol(customerCity);

                paramCollection[13].Value = customerEmail;
                paramCollection[14].Value = CommonFunctions.StripApostropheSymbol(customerFirstName);
                paramCollection[15].Value = CommonFunctions.StripApostropheSymbol(customerLastName);

                paramCollection[16].Value = customerPhone;
                paramCollection[17].Value = customerZipCode;
                paramCollection[18].Value = CommonFunctions.StripApostropheSymbol(shippingAddress);

                paramCollection[19].Value = CommonFunctions.StripApostropheSymbol(shippingCity);
                paramCollection[20].Value = string.IsNullOrEmpty(shippingEmail) ? customerEmail : shippingEmail;
                paramCollection[21].Value = string.IsNullOrEmpty(shippingFirstName) ? CommonFunctions.StripApostropheSymbol(customerFirstName) : CommonFunctions.StripApostropheSymbol(shippingFirstName);

                paramCollection[22].Value = string.IsNullOrEmpty(shippingLastName) ? CommonFunctions.StripApostropheSymbol(customerLastName) : CommonFunctions.StripApostropheSymbol(shippingLastName);
                paramCollection[23].Value = string.IsNullOrEmpty(shippingPhone) ? customerPhone : shippingPhone;
                paramCollection[24].Value = shippingZipCode;

                paramCollection[25].Value = couponAmount;
                paramCollection[26].Value = creditApplied;
                paramCollection[27].Value = storeFrontCurrencyExchangeRate;

                paramCollection[28].Value = shippingPrice;
                paramCollection[29].Value = transferOrder;
                paramCollection[30].Value = orderInvoiceActive;

                paramCollection[31].Value = cpPaymentPending;
                paramCollection[32].Value = dateCreated;
                paramCollection[33].Value = lastModified;

                paramCollection[34].Value = frontOfTheLinePrice;
                paramCollection[35].Value = frontOfTheLineFk;

                paramCollection[36].Value = provinceTaxPercent;
                paramCollection[37].Value = provinceTaxAmount;
                paramCollection[38].Value = globalTaxPercent;
                paramCollection[39].Value = globalTaxAmount;
                paramCollection[40].Value = harmonizedTaxPercent;
                paramCollection[41].Value = harmonizedTaxAmont;
                paramCollection[42].Value = consultationConsent == true ? true : false;
                paramCollection[43].Value = paymentTransactionType;

                paramCollection[44].Value = null;  // new order invoice Id generated
                paramCollection[45].Value = null;  // message
                paramCollection[46].Value = null;  // sql used

                SqlHelper.ExecuteNonQuery(_sCon, CommandType.StoredProcedure, "usp_addOrderInvoiceWithTax", paramCollection);

                var value = paramCollection[44].Value;
                var msg = paramCollection[45].Value;
                var sqlUsed = paramCollection[46].Value;

                if (value != null)
                {
                    return Convert.ToInt32(value.ToString());
                }
                return 0;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        // get currency exchange rates
        public DataSet GetExchangeRate()
        {
            try
            {
                _ds = SqlHelper.ExecuteDataset(_sCon, "usp_getExchangeRate");
            }
            catch (Exception ex)
            {
                _ds = null;
                throw ex;
            }
            return _ds;
        }

        // add to cart table
        public int AddCart(int orderInvoiceId,
                           int shippingInvoiceFk,
                           int cartProfileFk,
                           int productSizeId,
                           int cartRefill,
                           decimal cartItemCost,
                           decimal cartItemPharmacyCost,
                           int quantity,
                           decimal productSizeStoreFrontPrice,
                           int cartRefillDay,
                           bool refill,
                           int cartReship,
                           int cartPmReturn,
                           double usdExchange,
                           double audExchange,
                           DateTime dateCartCreated,
                           DateTime dateCartModified,
                           double nzdExchange,
                           double gbpExchange,
                           double eudExchange,
                           bool isRefillCartItem,
                           int refillShippingInvoiceFk)

        {
            try
            {
                SqlParameter[] paramCollection = SqlHelperParameterCache.GetSpParameterSet(_sCon, "usp_addCart", true);

                paramCollection[1].Value = orderInvoiceId;
                paramCollection[2].Value = shippingInvoiceFk;
                paramCollection[3].Value = cartProfileFk;

                paramCollection[4].Value = productSizeId;
                paramCollection[5].Value = cartRefill;
                paramCollection[6].Value = cartItemCost;

                paramCollection[7].Value = cartItemPharmacyCost;
                paramCollection[8].Value = quantity;
                paramCollection[9].Value = productSizeStoreFrontPrice;

                paramCollection[10].Value = cartRefillDay;
                paramCollection[11].Value = refill;
                paramCollection[12].Value = cartReship;

                paramCollection[13].Value = cartPmReturn;
                paramCollection[14].Value = usdExchange;
                paramCollection[15].Value = audExchange;

                paramCollection[16].Value = dateCartCreated;
                paramCollection[17].Value = dateCartModified;
                paramCollection[18].Value = nzdExchange;

                paramCollection[19].Value = gbpExchange;
                paramCollection[20].Value = eudExchange;

                paramCollection[21].Value = null;  // new cartid generated
                paramCollection[22].Value = null;  // message
                paramCollection[23].Value = null;  // sql used

                paramCollection[24].Value = isRefillCartItem;
                paramCollection[25].Value = refillShippingInvoiceFk;

                SqlHelper.ExecuteNonQuery(_sCon, CommandType.StoredProcedure, "usp_addCart", paramCollection);

                var msg = paramCollection[22].Value;
                var sqlUsed = paramCollection[23].Value;
                var value = paramCollection[21].Value;
                if (value != null)
                {
                    return Convert.ToInt32(value.ToString());
                }
                return 0;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        // update order transaction with new order invoice id and message
        public int LinkOrderTransactionToOrder(int orderTransactionId, int orderInvoiceId, string statusMessage,
            int transactionError, DateTime lastModified, string transactionNum, string transactionAuthCode, int transactionType)
        {
            try
            {
                SqlParameter[] paramCollection = SqlHelperParameterCache.GetSpParameterSet(_sCon, "usp_LinkOrderTransactionWithOrder", true);

                paramCollection[1].Value = orderTransactionId;
                paramCollection[2].Value = orderInvoiceId;
                paramCollection[3].Value = !string.IsNullOrEmpty(statusMessage) ? statusMessage : string.Empty;
                paramCollection[4].Value = transactionError;
                paramCollection[5].Value = lastModified;
                paramCollection[6].Value = !string.IsNullOrEmpty(transactionNum) ? transactionNum : string.Empty;
                paramCollection[7].Value = !string.IsNullOrEmpty(transactionAuthCode) ? transactionAuthCode : string.Empty;
                paramCollection[8].Value = transactionType;
                paramCollection[9].Value = null; // UpdateStatus
                paramCollection[10].Value = null; // Message

                SqlHelper.ExecuteNonQuery(_sCon, CommandType.StoredProcedure, "usp_LinkOrderTransactionWithOrder", paramCollection);

                var updateStatus = Convert.ToInt32(paramCollection[9].Value.ToString());
                var message = paramCollection[10].Value.ToString();

                return updateStatus;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        // Add credit used by customer

        public int AddCredit(int customerId,
                             int orderInvoiceId,
                             decimal creditApplied,
                             int transactionType,
                             string transactionComment,
                             int customerReferred,
                             DateTime dateCreated)
        {
            try
            {
                SqlParameter[] paramCollection = SqlHelperParameterCache.GetSpParameterSet(_sCon, "usp_addCustomerTransaction", true);

                paramCollection[1].Value = customerId;
                paramCollection[2].Value = orderInvoiceId;
                paramCollection[3].Value = creditApplied;

                paramCollection[4].Value = transactionType;
                paramCollection[5].Value = transactionComment;
                paramCollection[6].Value = customerReferred;

                paramCollection[7].Value = dateCreated;

                paramCollection[8].Value = null;  // new customerTransaction id generated
                paramCollection[9].Value = null;  // message
                paramCollection[10].Value = null;  // sql used

                SqlHelper.ExecuteNonQuery(_sCon, CommandType.StoredProcedure, "usp_addCustomerTransaction", paramCollection);

                var msg = paramCollection[9].Value;
                var sqlUsed = paramCollection[10].Value;
                var value = paramCollection[8].Value;
                if (value != null)
                {
                    return Convert.ToInt32(value.ToString());
                }
                return 0;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        // get order detail used by order confirmation page
        public DataSet GetOrderDetailMinimal(int orderId)
        {
            try
            {
                _ds = SqlHelper.ExecuteDataset(_sCon, "usp_getOrderDetailMinimal", orderId);
            }
            catch (Exception ex)
            {
                _ds = null;
                throw ex;
            }
            return _ds;
        }

        // get cart details for order id
        public DataSet GetOrderCart(int orderId, int storefrontId)
        {
            try
            {
                _ds = SqlHelper.ExecuteDataset(_sCon, "usp_getOrderCart_ByOrderInvoiceId", orderId, storefrontId);
            }
            catch (Exception ex)
            {
                _ds = null;
                throw ex;
            }
            return _ds;
        }

        // check if customer has placed any order
        public bool IfCustomerHasOrders(int customerId)
        {
            try
            {
                SqlParameter[] paramCollection = SqlHelperParameterCache.GetSpParameterSet(_sCon, "usp_be_IfCustomerPlacedAnyOrder", true);

                paramCollection[1].Value = customerId;
                paramCollection[2].Value = null; // true/false

                SqlHelper.ExecuteNonQuery(_sCon, CommandType.StoredProcedure, "usp_be_IfCustomerPlacedAnyOrder", paramCollection);

                var value = paramCollection[2].Value;
                return value != null && Convert.ToBoolean(value.ToString());

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //Comment: Tax- get provincial tax details
        public DataSet GetProvinceTaxDetails(int provinceStateId)
        {
            try
            {
                _ds = SqlHelper.ExecuteDataset(_sCon, "usp_getProvinceTaxDetails", provinceStateId);
            }
            catch (Exception ex)
            {
                _ds = null;
                throw ex;
            }
            return _ds;
        }

        // update order invoice to clear payment pending
        public int UpdateOrderInvoicePaymentPending(string orderInvoiceToUpdate,
            bool paymentPending,
            DateTime dateUpdated)
        {
            try
            {
                SqlParameter[] paramCollection = SqlHelperParameterCache.GetSpParameterSet(_sCon, "usp_be_UpdateOrderinvoicePaymentPending", true);

                paramCollection[1].Value = orderInvoiceToUpdate;
                paramCollection[2].Value = paymentPending;
                paramCollection[3].Value = dateUpdated;
                paramCollection[4].Value = null; // UpdateStatus
                paramCollection[5].Value = null; // Message

                SqlHelper.ExecuteNonQuery(_sCon, CommandType.StoredProcedure, "usp_be_UpdateOrderInvoicePaymentPending", paramCollection);

                var updateStatus = Convert.ToInt32(paramCollection[4].Value.ToString());
                var message = paramCollection[5].Value.ToString();

                return updateStatus;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //Get questionnaire based on category Id
        public DataSet GetQuestionnaireByCategoryId(string categoryId, string sortBy)
        {
            try
            {
                _ds = SqlHelper.ExecuteDataset(_sCon, "usp_GetQuestionnaireByCategory", categoryId, sortBy);
            }
            catch (Exception ex)
            {
                _ds = null;
                throw ex;
            }
            return _ds;
        }
        //Get questionnaire option based on questionnaire Id
        public DataSet GetQuestionOptionByQuestionnaireId(string QuestionnaireId, string sortBy)
        {
            try
            {
                _ds = SqlHelper.ExecuteDataset(_sCon, "usp_GetQuestionnaireOptionByQuestion", QuestionnaireId, sortBy);
            }
            catch (Exception ex)
            {
                _ds = null;
                throw ex;
            }
            return _ds;
        }
        // add Question Response
        public int AddQuestionnaireResponse(int customerId,
                                       int orderInvoiceFk,
                                       int QuestionnaireFk,
                                       int QuestionnaireCategoryFk,
            string answerText,
            string moreInfo,
                                       int QuestionnaireOptionFk,
            DateTime QuestionnaireDateCreated

                                      )
        {
            try
            {
                SqlParameter[] paramCollection = SqlHelperParameterCache.GetSpParameterSet(_sCon, "usp_AddQuestionnaireResponse", true);

                paramCollection[1].Value = customerId;
                paramCollection[2].Value = orderInvoiceFk;
                paramCollection[3].Value = QuestionnaireFk;

                paramCollection[4].Value = QuestionnaireCategoryFk;
                paramCollection[5].Value = answerText;
                paramCollection[6].Value = moreInfo;
                paramCollection[7].Value = QuestionnaireOptionFk;
                paramCollection[8].Value = QuestionnaireDateCreated;
                paramCollection[9].Value = null;  // message
                paramCollection[10].Value = null;
                // sqlUsed

                SqlHelper.ExecuteNonQuery(_sCon, CommandType.StoredProcedure, "usp_AddQuestionnaireResponse", paramCollection);


                var sqlUsed = paramCollection[10].Value;
                var value = paramCollection[9].Value;
                if (value != null)
                {
                    return Convert.ToInt32(value.ToString());
                }
                return 0;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //Get questionnaire option based on questionnaire Id
        public DataSet GetQuestionnaireGroupByCatId(string QuestionnaireCatId, string sortBy)
        {
            try
            {
                _ds = SqlHelper.ExecuteDataset(_sCon, "usp_GetQuestionnaireGroupByCategory", QuestionnaireCatId, sortBy);
            }
            catch (Exception ex)
            {
                _ds = null;
                throw ex;
            }
            return _ds;
        }
        //Get List for Questionnaire Responses
        public DataSet IfUserAlreadyAnsweredQuestionnaire(int catId, int customerId, DateTime currentTimeStamp, int orderTimeSpan, int orderInvoiceFk = 0)
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
        // add Question Response
        public int AddCustomerQuestionnaireResponse(int customerId,
                                       int orderInvoiceFk,
                                       int categoryFk,
                                       int approve,
                                       DateTime dateCreated,
                                       DateTime dateModified,
                                       bool? hardStop

            )
        {
            try
            {
                SqlParameter[] paramCollection = SqlHelperParameterCache.GetSpParameterSet(_sCon, "usp_AddCustomerQuestionnaireCategoryResponse", true);

                paramCollection[1].Value = customerId;
                paramCollection[2].Value = orderInvoiceFk;
                paramCollection[3].Value = categoryFk;

                paramCollection[4].Value = approve;
                paramCollection[5].Value = dateCreated;
                paramCollection[6].Value = dateModified;
                paramCollection[7].Value = hardStop;
                paramCollection[8].Value = null;  // message
                paramCollection[9].Value = null;
                // sqlUsed

                SqlHelper.ExecuteNonQuery(_sCon, CommandType.StoredProcedure, "usp_AddCustomerQuestionnaireCategoryResponse", paramCollection);


                var sqlUsed = paramCollection[9].Value;
                var value = paramCollection[8].Value;
                if (value != null)
                {
                    return Convert.ToInt32(value.ToString());
                }
                return 0;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //Get Questionnaire Product category
        public DataSet GetProductQuestionnaireCategoryByOrderId(int orderInvoicefk)
        {
            try
            {
                _ds = SqlHelper.ExecuteDataset(_sCon, "usp_getProductQuestionnaireCategoryByOrderId", orderInvoicefk);
            }
            catch (Exception ex)
            {
                _ds = null;
                throw ex;
            }
            return _ds;
        }


        public int AddCustomerSubscription(int customerFk, int orderInvoiceFk, int productFk, int productSizeFk, decimal cartItemQuantity, bool autoFillActive, DateTime dateCreated)
        {
            try
            {
                SqlParameter[] paramCollection = SqlHelperParameterCache.GetSpParameterSet(_sCon, "usp_AddCustomerSubscription", true);

                paramCollection[1].Value = customerFk;
                paramCollection[2].Value = orderInvoiceFk;
                paramCollection[3].Value = productFk;
                paramCollection[4].Value = productSizeFk;
                paramCollection[5].Value = cartItemQuantity;
                paramCollection[6].Value = autoFillActive;
                paramCollection[7].Value = dateCreated;

                paramCollection[8].Value = null; // SubscriptionId
                paramCollection[9].Value = null; // message


                SqlHelper.ExecuteNonQuery(_sCon, CommandType.StoredProcedure, "usp_AddCustomerSubscription", paramCollection);

                var value = paramCollection[8].Value;
                var message = paramCollection[9].Value;
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

        public DataSet GetCustomerSubscription(int customerFk, string productSizeFkList)
        {
            try
            {
                _ds = SqlHelper.ExecuteDataset(_sCon, "usp_getCustomerSubscription", customerFk, productSizeFkList);
            }
            catch (Exception ex)
            {
                _ds = null;
                throw ex;
            }
            return _ds;
        }
        // get Refill Order detail
        public DataSet GetOrderRefillDetails(int customerFk, string productSizeStrength, int productSizeId)
        {
            try
            {
                _ds = SqlHelper.ExecuteDataset(_sCon, "usp_GetOrderRefillDetail", customerFk, productSizeStrength, productSizeId);
            }
            catch (Exception ex)
            {
                _ds = null;
                throw ex;
            }
            return _ds;
        }
        // update Cart Refill
        public int UpdateOrderCartRefillStatus(int productSizeId,
            int orderInvoiceId,
            bool refillStatus)
        {
            try
            {
                SqlParameter[] paramCollection = SqlHelperParameterCache.GetSpParameterSet(_sCon, "usp_UpdateOrderCartRefillStatus", true);


                paramCollection[1].Value = productSizeId;
                paramCollection[2].Value = orderInvoiceId;
                paramCollection[3].Value = refillStatus; // UpdateStatus
                paramCollection[4].Value = null; // Message
                paramCollection[5].Value = null;

                SqlHelper.ExecuteNonQuery(_sCon, CommandType.StoredProcedure, "usp_UpdateOrderCartRefillStatus", paramCollection);

                var updateStatus = Convert.ToInt32(paramCollection[4].Value.ToString());
                var message = paramCollection[5].Value.ToString();

                return updateStatus;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        // get Refill Order detail
        public DataSet GetAnsweredQuestionnaireForCustomer(int QuestionnaireCatId, int customerFk, DateTime currentTimeStamp, int orderTimeSpan, int orderId)
        {
            try
            {
                _ds = SqlHelper.ExecuteDataset(_sCon, "usp_GetCustomerAnsweredQuestionnaireByCatId", QuestionnaireCatId, customerFk, currentTimeStamp, orderTimeSpan, orderId);
            }
            catch (Exception ex)
            {
                _ds = null;
                throw ex;
            }
            return _ds;
        }

        public int AddLogOrderTransaction(int orderTransactionFk, int orderInvoiceFk, int paymentOptionFk,
            int paymentTransactionTypeFk, DateTime dateCreated)
        {
            try
            {
                SqlParameter[] paramCollection = SqlHelperParameterCache.GetSpParameterSet(_sCon, "usp_AddLogOrderTransaction", true);

                paramCollection[1].Value = null; // logId
                paramCollection[2].Value = null; // message

                paramCollection[3].Value = orderTransactionFk;
                paramCollection[4].Value = orderInvoiceFk;
                paramCollection[5].Value = paymentOptionFk;
                paramCollection[6].Value = paymentTransactionTypeFk;
                paramCollection[7].Value = (int)SourceApplicationEnum.EnhanceClub;
                paramCollection[8].Value = dateCreated;


                SqlHelper.ExecuteNonQuery(_sCon, CommandType.StoredProcedure, "usp_AddLogOrderTransaction", paramCollection);

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

        public int AddCustomerSessionTracker(int customerFk, int orderInvoiceFk, string actionName, string trackingMessage, int trackinStatus, DateTime dateCreated)
        {
            try
            {
                SqlParameter[] paramCollection = SqlHelperParameterCache.GetSpParameterSet(_sCon, "usp_addCustomerSessionTracker", true);

                paramCollection[1].Value = null; // trackerId
                paramCollection[2].Value = null; // message

                paramCollection[3].Value = customerFk;
                paramCollection[4].Value = orderInvoiceFk;
                paramCollection[5].Value = actionName;
                paramCollection[6].Value = trackingMessage;
                paramCollection[7].Value = trackinStatus;
                paramCollection[8].Value = dateCreated;


                SqlHelper.ExecuteNonQuery(_sCon, CommandType.StoredProcedure, "usp_addCustomerSessionTracker", paramCollection);

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

        public int UpdateCustomerSessionTracker(int trackingId, string trackingMessage, int trackingStatus, DateTime dateUpdated)
        {
            try
            {
                SqlParameter[] paramCollection = SqlHelperParameterCache.GetSpParameterSet(_sCon, "usp_updateCustomerSessionTracker", true);

                paramCollection[1].Value = null; // trackerStatus
                paramCollection[2].Value = null; // message

                paramCollection[3].Value = trackingId;
                paramCollection[4].Value = trackingMessage;
                paramCollection[5].Value = trackingStatus;
                paramCollection[6].Value = dateUpdated;


                SqlHelper.ExecuteNonQuery(_sCon, CommandType.StoredProcedure, "usp_updateCustomerSessionTracker", paramCollection);

                var updateStatus = Convert.ToInt32(paramCollection[1].Value.ToString());
                var message = paramCollection[2].Value.ToString();

                return updateStatus;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public int UpdateOrderInvoiceBPStatus(int orderInvoiceId, bool BPNotKnown)
        {
            try
            {
                SqlParameter[] paramCollection = SqlHelperParameterCache.GetSpParameterSet(_sCon, "usp_UpdateOrderInvoiceBPStatus", true);

                paramCollection[1].Value = orderInvoiceId;
                paramCollection[2].Value = BPNotKnown;

                var updateStatus = SqlHelper.ExecuteNonQuery(_sCon, CommandType.StoredProcedure, "usp_UpdateOrderInvoiceBPStatus", paramCollection);

                return updateStatus;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DataSet GetBPQuestionnaires()
        {
            try
            {
                _ds = SqlHelper.ExecuteDataset(_sCon, "usp_GetBPQuestionnaires");
            }
            catch (Exception ex)
            {
                _ds = null;
                throw ex;
            }
            return _ds;
        }

        public int UpdateCustomerQuestionnaireCategoryResponse(int questionnaireResponseId, int customerFk, int questionnaireProblemFk, string problemAssignUserAdmin,
             DateTime problemAssignDate, string assignDoctorComment)
        {
            try
            {
                SqlParameter[] paramCollection = SqlHelperParameterCache.GetSpParameterSet(_sCon, "usp_UpdateCustomerQuestionnaireCategoryResponse", true);

                paramCollection[1].Value = questionnaireResponseId;
                paramCollection[2].Value = customerFk;
                paramCollection[3].Value = questionnaireProblemFk;
                paramCollection[4].Value = problemAssignUserAdmin;
                paramCollection[5].Value = problemAssignDate;
                paramCollection[6].Value = assignDoctorComment;

                var updateStatus = SqlHelper.ExecuteNonQuery(_sCon, CommandType.StoredProcedure, "usp_UpdateCustomerQuestionnaireCategoryResponse", paramCollection);

                return updateStatus;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public int UpdateBPQuestionnaireResponse(int customerFk, int orderInvoiceFk, int questionFk, int responseOptionFk)
        {
            try
            {
                SqlParameter[] paramCollection = SqlHelperParameterCache.GetSpParameterSet(_sCon, "usp_UpdateBPQuestionnaireResponse", true);

                paramCollection[1].Value = customerFk;
                paramCollection[2].Value = orderInvoiceFk;
                paramCollection[3].Value = questionFk;
                paramCollection[4].Value = responseOptionFk;

                var updateStatus = SqlHelper.ExecuteNonQuery(_sCon, CommandType.StoredProcedure, "usp_UpdateBPQuestionnaireResponse", paramCollection);

                return updateStatus;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //public int AddLogPreScreenAQuestionnaire(int questionnaireCategoryResponse, int problemFk, string comment, DateTime dateCreated)
        //{
        //    try
        //    {
        //        SqlParameter[] paramCollection = SqlHelperParameterCache.GetSpParameterSet(_sCon, "usp_be_AddLogPreScreenAQuestionnaire", true);

        //        paramCollection[1].Value = questionnaireCategoryResponse;
        //        paramCollection[2].Value = null;
        //        paramCollection[3].Value = null;
        //        paramCollection[4].Value = "2";
        //        paramCollection[5].Value = comment;
        //        paramCollection[6].Value = "update";
        //        paramCollection[7].Value = problemFk;
        //        paramCollection[8].Value = dateCreated;
        //        paramCollection[9].Value = null;
        //        paramCollection[10].Value = null;
        //        paramCollection[11].Value = null;

        //        SqlHelper.ExecuteNonQuery(_sCon, CommandType.StoredProcedure, "usp_be_AddLogPreScreenAQuestionnaire", paramCollection);

        //        var updateStatus = Convert.ToInt32(paramCollection[1].Value.ToString());

        //        return updateStatus; 
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        public int AddLogPreScreenOrderBPUpdate(int customerFk, int orderInvoiceFk, int categoryResponseFk, string comment, string ipAddress, DateTime dateCreated)
        {
            try
            {
                SqlParameter[] paramCollection = SqlHelperParameterCache.GetSpParameterSet(_sCon, "usp_AddLogPreScreenOrderBPUpdate", true);

                paramCollection[1].Value = customerFk;
                paramCollection[2].Value = orderInvoiceFk;
                paramCollection[3].Value = categoryResponseFk;
                paramCollection[4].Value = comment;
                paramCollection[5].Value = ipAddress;
                paramCollection[6].Value = dateCreated;

                var updateStatus = SqlHelper.ExecuteNonQuery(_sCon, CommandType.StoredProcedure, "usp_AddLogPreScreenOrderBPUpdate", paramCollection);

                return updateStatus;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public int AddEmailRecordForProcessing(int storeFrontFk, int sourceApplicationFk, string toEmail, string emailSubject, string emailBody, int orderInvoiceFk,
            int customerFk, int nitroUserFk, int emailTypeFk, int interval, int maxEmailSentCount)
        {
            try
            {
                SqlParameter[] paramCollection = SqlHelperParameterCache.GetSpParameterSet(_sCon, "usp_ST_NitroEmailQueueRegistration", true);

                paramCollection[1].Value = storeFrontFk;
                paramCollection[2].Value = sourceApplicationFk;
                paramCollection[3].Value = toEmail;
                paramCollection[4].Value = emailSubject;
                paramCollection[5].Value = emailBody;
                paramCollection[6].Value = null;
                paramCollection[7].Value = null;
                paramCollection[8].Value = orderInvoiceFk;
                paramCollection[9].Value = customerFk;
                paramCollection[10].Value = nitroUserFk;
                paramCollection[11].Value = DateTime.UtcNow;
                paramCollection[12].Value = interval;
                paramCollection[13].Value = maxEmailSentCount;
                paramCollection[14].Value = 0;
                paramCollection[15].Value = emailTypeFk;

                var updateStatus = SqlHelper.ExecuteNonQuery(_sCon, CommandType.StoredProcedure, "usp_ST_NitroEmailQueueRegistration", paramCollection);

                return updateStatus;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public int UpdateNitroEmailQueueStatus(int customerFk, int orderInvoiceFk)
        {
            try
            {
                SqlParameter[] paramCollection = SqlHelperParameterCache.GetSpParameterSet(_sCon, "usp_ST_NitroEmailQueueStatus", true);

                paramCollection[1].Value = customerFk;
                paramCollection[2].Value = orderInvoiceFk;

                var updateStatus = SqlHelper.ExecuteNonQuery(_sCon, CommandType.StoredProcedure, "usp_ST_NitroEmailQueueStatus", paramCollection);

                return updateStatus;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public int AddCustomerQuestionnaireIssues(int customerFk, int orderInvoiceFk, int questionnaireFk, int deviceType, string exception,bool completeStatus, DateTime dateCreated)
        {
            try
            {
                SqlParameter[] paramCollection = SqlHelperParameterCache.GetSpParameterSet(_sCon, "usp_AddCustomerQuestionnaireIssues", true);

                paramCollection[1].Value = customerFk;
                paramCollection[2].Value = orderInvoiceFk;
                paramCollection[3].Value = questionnaireFk;
                paramCollection[4].Value = deviceType == 0 ? "Desktop" : "Mobile";
                paramCollection[5].Value = exception;
                paramCollection[6].Value = completeStatus;
                paramCollection[7].Value = dateCreated;

                var updateStatus = SqlHelper.ExecuteNonQuery(_sCon, CommandType.StoredProcedure, "usp_AddCustomerQuestionnaireIssues", paramCollection);

                return updateStatus;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public int UpdateCustomerQuestionnaireResponse(int customerFk, int orderInvoiceFk, int questionnaireFk, int deviceType, bool completeStatus, string operationType, DateTime dateCreated)
        {
            try
            {
                SqlParameter[] paramCollection = SqlHelperParameterCache.GetSpParameterSet(_sCon, "usp_UpdateCustomerQuestionnaireResponse", true);

                paramCollection[1].Value = customerFk;
                paramCollection[2].Value = orderInvoiceFk;
                paramCollection[3].Value = questionnaireFk;  
                paramCollection[4].Value = deviceType == 0 ? "Desktop" : "Mobile";
                paramCollection[5].Value = completeStatus;
                paramCollection[6].Value = operationType;
                paramCollection[7].Value = dateCreated;

                var updateStatus = SqlHelper.ExecuteNonQuery(_sCon, CommandType.StoredProcedure, "usp_UpdateCustomerQuestionnaireResponse", paramCollection);

                return updateStatus;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DataSet GetCustomerActiveRefillOrders(int customerId, int productSizeFk, DateTime currentDate)
        {
            try
            {
                _ds = SqlHelper.ExecuteDataset(_sCon, "usp_getCustomerExistingRefillOrder", customerId, productSizeFk, currentDate);
            }
            catch (Exception ex)
            {
                _ds = null;
                throw ex;
            }
            return _ds;
        }


        public int UpdateOrderInvoiceRefillStatus(bool refillStatus, int orderInvoiceFk)
        {
            try
            {
                SqlParameter[] paramCollection = SqlHelperParameterCache.GetSpParameterSet(_sCon, "usp_UpdateOrderInvoiceRefillStatus", true);

                paramCollection[1].Value = null; // trackerStatus
                paramCollection[2].Value = null; // message

                paramCollection[3].Value = refillStatus;
                paramCollection[4].Value = orderInvoiceFk;
              
                SqlHelper.ExecuteNonQuery(_sCon, CommandType.StoredProcedure, "usp_UpdateOrderInvoiceRefillStatus", paramCollection);

                var updateStatus = Convert.ToInt32(paramCollection[1].Value.ToString());
                var message = paramCollection[2].Value.ToString();

                return updateStatus;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public int AddLogRefillOrders(int orderInvoiceFk, int shipInvoiceFk, int refillsAllowed, int refillsAvailable,
                                        string refillMessage, string refillAction, DateTime? autoRefillStartDate, 
                                        DateTime dateCreated, bool isAutoRefill)
        {
            try
            {
                SqlParameter[] paramCollection = SqlHelperParameterCache.GetSpParameterSet(_sCon, "usp_AddLogRefillOrders", true);

                paramCollection[1].Value = null;
                paramCollection[2].Value = null;

                paramCollection[3].Value = orderInvoiceFk;
                paramCollection[4].Value = shipInvoiceFk;
                paramCollection[5].Value = refillsAllowed;
                paramCollection[6].Value = refillsAvailable;
                paramCollection[7].Value = refillMessage;
                paramCollection[8].Value = SourceApplicationEnum.EnhanceClub;
                paramCollection[9].Value = refillAction;
                paramCollection[10].Value = autoRefillStartDate;
                paramCollection[11].Value = dateCreated;
                paramCollection[12].Value = isAutoRefill;

                SqlHelper.ExecuteNonQuery(_sCon, CommandType.StoredProcedure, "usp_AddLogRefillOrders", paramCollection);

                var updateStatus = Convert.ToInt32(paramCollection[1].Value.ToString());
                var message = paramCollection[2].Value.ToString();

                return updateStatus;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public int UpdateLogRefillOrder(int logRefillOrderId, bool refillComplete, DateTime refillCompleteDate, int orderInvoiceFk)
        {
            try
            {
                SqlParameter[] paramCollection = SqlHelperParameterCache.GetSpParameterSet(_sCon, "usp_updateLogRefillOrders", true);

                paramCollection[1].Value = null; // Message
                paramCollection[2].Value = null;

                paramCollection[3].Value = logRefillOrderId;
                paramCollection[4].Value = refillComplete;
                paramCollection[5].Value = refillCompleteDate;
                paramCollection[6].Value = orderInvoiceFk;
              

                SqlHelper.ExecuteNonQuery(_sCon, CommandType.StoredProcedure, "usp_updateLogRefillOrders", paramCollection);

                var updateStatus = Convert.ToInt32(paramCollection[1].Value.ToString());
                var message = paramCollection[2].Value.ToString();

                return updateStatus;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DataSet GetRefillQuestionnaire()
        {
            try
            {
                _ds = SqlHelper.ExecuteDataset(_sCon, "usp_GetRefillQuestionnaire");
            }
            catch (Exception ex)
            {
                _ds = null;
                throw ex;
            }
            return _ds;
        }

        public int AddRefillQuestionnaireResponse(int customerFk, int orderInvoiceFk, List<QuestionOptionSelected> answerList, DateTime dateCreated)
        {
            // create a datatable to be passed as parameter to stored procedure
            DataTable tblAnswerList = new DataTable();

            tblAnswerList.Columns.Add("RefillQuestionnaireResponse_Customer_Fk", typeof(int));
            tblAnswerList.Columns.Add("RefillQuestionnaireResponse_OrderInvoice_Fk", typeof(int));
            tblAnswerList.Columns.Add("RefillQuestionnaireResponse_RefillQuestionnaire_Fk", typeof(int));
            tblAnswerList.Columns.Add("RefillQuestionnaireResponse_OptionYesNo", typeof(bool));
            tblAnswerList.Columns.Add("RefillQuestionnaireResponse_ExplanationText", typeof(string));
            tblAnswerList.Columns.Add("RefillQuestionnaireResponse_DateCreated", typeof(DateTime));
            tblAnswerList.Columns.Add("RefillQuestionnaireResponse_DateModified", typeof(DateTime));


            foreach (var answer in answerList)
            {
                DataRow row = tblAnswerList.NewRow();
                row["RefillQuestionnaireResponse_Customer_Fk"] = customerFk;
                row["RefillQuestionnaireResponse_OrderInvoice_Fk"] = orderInvoiceFk;
                row["RefillQuestionnaireResponse_RefillQuestionnaire_Fk"] = answer.QuestionnaireId;
                row["RefillQuestionnaireResponse_OptionYesNo"] = false; // to do, change it to yes no resposnse dynamically
                row["RefillQuestionnaireResponse_ExplanationText"] = answer.ExplanationText;
                row["RefillQuestionnaireResponse_DateCreated"] = dateCreated;
                row["RefillQuestionnaireResponse_DateModified"] = dateCreated;

                tblAnswerList.Rows.Add(row);
            }

            SqlParameter[] paramCollection = SqlHelperParameterCache.GetSpParameterSet(_sCon, "usp_AddRefillQuestionnaireResponse", true);

            paramCollection[1].TypeName = "LogProductStorefrontType";
            paramCollection[1].Value = tblAnswerList;
            paramCollection[2].Value = null; // UpdateStatus
            paramCollection[3].Value = null; // Message

            SqlHelper.ExecuteNonQuery(_sCon, CommandType.StoredProcedure, "usp_AddRefillQuestionnaireResponse", paramCollection);

            int uStatus = (int)paramCollection[2].Value;

            var uMsg = paramCollection[3].Value;

            if (uMsg != null)
            {
                var message = uMsg.ToString();
            }

            return uStatus;
        }
    }
}
