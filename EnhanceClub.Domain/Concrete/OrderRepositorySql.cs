using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using EnhanceClub.Domain.Abstract;
using EnhanceClub.Domain.Entities;
using EnhanceClub.Domain.Helpers;

namespace EnhanceClub.Domain.Concrete
{
    public class OrderRepositorySql : IOrderProcessor
    {
        private readonly OrderDbLayer _orderDbl = new OrderDbLayer();

        // process no payment orders
        public int ProcessOrderNoPayment(Cart cart,
            AffiliateInfo affiliateInfo,
            CustomerMinimal customerInfo,
            ShippingDetails shipDetails,
            StoreFrontInfo storefrontInfo,
            ICustomerRepository repositoryCustomer,
            IProductRepository repositoryProduct,
            IEmailSender emailSender,
            PatientProfile patientInfo,
            int paymentTransactionTypeFk
            )
        {

            int orderInvoiceId = 0;

            if (cart.CartItems.Any())
            {
                // Step 1: add to order transaction table first
                var customerId = customerInfo.CustomerId;
                var orderTempFk = 0;
                var paymentOptionFk = cart.PaymentOptionId;
                var cardCvv2 = string.Empty;
                var cardName = string.Empty;
                var cardNum = string.Empty;
                var cardType = string.Empty;
                var transactionIp = HttpContext.Current.Request.UserHostAddress;
                var statusMessage = "Transaction Disrupted";
                var transactionNum = string.Empty;
                var transactionAmount = cart.CartNetTotal;
                var cardCvv2Type = 0;
                var cardExpireMonth = 0;
                var cardExpireYear = 0;
                var transactionError = 1;
                // changed later to 0 when payment succeeds -- this is no payment option but process is kept same so that code can be reused and flow is same
                var transactionDate = DateTime.Now;
                var storeFrontFk = affiliateInfo.AffiliateStoreFrontFk;
                var FrontOfTheLinePrice = 0.00m;
                var FrontOfTheLineFk = 0;
                int orderTransactionId = 0;
                //if (!SiteConfigurations.EnablePayment)
                //{

                //    orderTransactionId = _orderDbl.AddOrderTransaction(customerId,
                //    orderInvoiceId,
                //    orderTempFk,
                //    paymentOptionFk,
                //    cardCvv2,
                //   cardName,
                //    cardNum,
                //    cardType,
                //    transactionIp,
                //    statusMessage,
                //    transactionNum,
                //    transactionAmount,
                //    cardCvv2Type,
                //    cardExpireMonth,
                //    cardExpireYear,
                //    transactionError,
                //    transactionDate,
                //    storeFrontFk,
                //    1);
                //}



                // since this is no payment option no payment charge or failed payment log is coded

                var userAdminCreateFK = 0;
                var userAdminProcessing = 0;
                var transferOrder = 0;
                var orderInvoiceActive = 1;
                var cpPaymentPending = 0;
                var dateCreated = DateTime.Now;
                var lastModified = DateTime.Now;

                //-- Step2 : Create Order Invoice
                orderInvoiceId = _orderDbl.AddOrderInvoice(customerInfo.CustomerCountryId,
                    customerInfo.CustomerProvinceId,
                    cart.CouponId,
                    storefrontInfo.StoreFrontCurrencyId,
                    customerInfo.CustomerId,
                    shipDetails.ShippingCountryId,
                    cart.ShippingOptionId,
                    shipDetails.ShippingProvinceId,
                    userAdminCreateFK,
                    userAdminProcessing,
                    customerInfo.CustomerAddress,
                    customerInfo.CustomerCity,
                    customerInfo.CustomerEmail,
                    customerInfo.CustomerFirstName,
                    customerInfo.CustomerLastName,
                    customerInfo.CustomerPhone,
                    customerInfo.CustomerZipCode,
                    shipDetails.ShippingAddress,
                    shipDetails.ShippingCity,
                    shipDetails.ShippingEmail,
                    shipDetails.ShippingFirstName,
                    shipDetails.ShippingLastName,
                    shipDetails.ShippingPhone,
                    shipDetails.ShippingZipCode,
                    cart.CouponAmount,
                    cart.CreditApplied,
                    storefrontInfo.StoreFrontCurrencyExchangeRate,
                    cart.ShippingPrice,
                    transferOrder,
                    orderInvoiceActive,
                    cpPaymentPending,
                    dateCreated,
                    lastModified,
                    FrontOfTheLinePrice,
                    FrontOfTheLineFk,
                    cart.ProvincialTaxPercentage,
                    cart.CartProvincialTaxAmount,
                    cart.GlobalTaxPercentage,
                    cart.CartGlobalTaxAmount,
                    cart.HSTPercentage,
                    cart.CartHSTTaxAmount,
                    patientInfo.PatientProfileConsultationConsent,
                    paymentTransactionTypeFk
                    );


                // Step 3:  get exchange rates for cart from GlobalSetting table
                DataSet dsExchangeRates = _orderDbl.GetExchangeRate();
                var usdExchange = 1.00;
                var audExhange = 1.07657;
                var nzdExchange = 1.16;
                var gbpExchange = 0.59;
                var eudExchange = 0.72;

                if (dsExchangeRates != null)
                {

                    usdExchange = Convert.ToDouble
                        ((from m in dsExchangeRates.Tables[0].AsEnumerable()
                          where m.Field<string>("CurrencyName") == "USD"
                          select m.Field<string>("CurrencyExchangeRate")
                            ).FirstOrDefault());

                    audExhange = Convert.ToDouble((from m in dsExchangeRates.Tables[0].AsEnumerable()
                                                   where m.Field<string>("CurrencyName") == "AUD"
                                                   select m.Field<string>("CurrencyExchangeRate")
                        ).FirstOrDefault());


                    nzdExchange = Convert.ToDouble((from m in dsExchangeRates.Tables[0].AsEnumerable()
                                                    where m.Field<string>("CurrencyName") == "NZD"
                                                    select m.Field<string>("CurrencyExchangeRate")
                        ).FirstOrDefault());


                    gbpExchange = Convert.ToDouble((from m in dsExchangeRates.Tables[0].AsEnumerable()
                                                    where m.Field<string>("CurrencyName") == "GBP"
                                                    select m.Field<string>("CurrencyExchangeRate")
                        ).FirstOrDefault());


                    eudExchange = Convert.ToDouble((from m in dsExchangeRates.Tables[0].AsEnumerable()
                                                    where m.Field<string>("CurrencyName") == "EUD"
                                                    select m.Field<string>("CurrencyExchangeRate")
                        ).FirstOrDefault());

                }


                // Step 4: create cart with new orderinvoiceId created.
                var shippingInvoiceFk = 0;
                var cartRefill = 0;
                var cartItemCost = 0.0m;
                var cartItemPharmacyCost = 0.0m;
                var cartRefillDay = 0;
                var cartReship = 0;
                var cartPmReturn = 0;
                var dateCartCreated = DateTime.UtcNow;
                var dateCartModified = DateTime.UtcNow;

                var patientProfileFk = patientInfo.PatientProfileId;

                foreach (var cartItem in cart.CartItems)
                {
                    int cartId = _orderDbl.AddCart(orderInvoiceId,
                        shippingInvoiceFk,
                        patientProfileFk, // assign default patient profile to cart item
                        cartItem.ProductCart.ProductSizeId,
                        cartRefill,
                        cartItemCost,
                        cartItemPharmacyCost,
                        cartItem.Quantity,
                        cartItem.ProductCart.ProductSizeStoreFrontPrice,
                        cartRefillDay,
                        cartItem.CartRefill,
                        cartReship,
                        cartPmReturn,
                        usdExchange,
                        audExhange,
                        dateCartCreated,
                        dateCartModified,
                        nzdExchange,
                        gbpExchange,
                        eudExchange,
                        cartItem.IsRefillCartItem,
                        cartItem.CartItemShipInvoiceFk);

                    if (cartItem.IsRefillCartItem)
                    {
                        if (cartItem.AutoRefillCompleted)
                        {
                            // update end end and auto refill completed flag
                            var updateStatusLogRefill = _orderDbl.UpdateLogRefillOrder(cartItem.LogRefillId, true, dateCartModified, orderInvoiceId);
                        }
                        else
                        {
                            var logRefillId = _orderDbl.AddLogRefillOrders(orderInvoiceId, cartItem.CartItemShipInvoiceFk,
                                   cartItem.NumberOfRefillsAllowed, cartItem.NumberOfRefillsAvailable, cartItem.RefillMessage,
                                   "Create Order", cartItem.AutoRefillStartDate,
                                    dateCartModified, false);
                        }

                    }
                }

                    /*Start moved the code to add transaction in order controller when direct post 
               * api payment is processed
               */
                    var otStatusMessage = "Payment Authorized";
                    var otTransactionError = 0;

                    // Step 5: update order transaction table
                    if (!SiteConfigurations.EnablePayment)
                    {
                        int updateStatus = _orderDbl.LinkOrderTransactionToOrder(orderTransactionId, orderInvoiceId,
                            otStatusMessage, otTransactionError, DateTime.UtcNow, transactionNum, string.Empty, 0);
                    }

                    // Step 6: Perform Credit deduction if credit was used by customer

                    int transactionType = 3;
                    string transactionComment = "Credit Applied to a Purchase";
                    int customerReferred = 0;
                    // 
                    if (cart.CreditApplied != 0)
                    {

                        int customerTransactionId = _orderDbl.AddCredit(customerInfo.CustomerId, orderInvoiceId,
                            cart.CreditApplied * -1, transactionType, transactionComment, customerReferred, DateTime.UtcNow);

                        // add to Order Transaction
                        paymentOptionFk = SiteConfigurations.PaymentOptionCustomerCredit;

                        statusMessage = "Paid by Customer Credit";
                        transactionError = 0;
                        transactionAmount = cart.CreditApplied;

                        // add orderTransaction only if partial payment is done bu customer credit
                        if (cart.CartNetTotal != 0)
                        {
                            int creditOrderTransactionId = _orderDbl.AddOrderTransaction(customerId,
                            orderInvoiceId,
                            orderTempFk,
                            paymentOptionFk,
                            cardCvv2,
                            cardName,
                            cardNum,
                            cardType,
                            transactionIp,
                            statusMessage,
                            transactionNum,
                            transactionAmount,
                            cardCvv2Type,
                            cardExpireMonth,
                            cardExpireYear,
                            transactionError,
                            transactionDate,
                            storeFrontFk,
                            SiteConfigurations.PaymentTransactionTypeCustomerCreditFk
                            );
                        }


                    }
                    // Step 7: Perform Referral Credit
                    if (cart.CartNetTotal >= 99)
                    {
                        ApplyReferralCredit(customerId, repositoryCustomer, affiliateInfo, orderInvoiceId);
                    }

                    // Step 8: Perform Limited quantity deduction

                    var cartItemsWithLimitedQty = cart.CartItems.Count(x => x.ProductCart.ProductSizeLimited);

                    if (cartItemsWithLimitedQty != 0)
                    {
                        foreach (var thisCartItem in cart.CartItems)
                        {
                            if (thisCartItem.ProductCart.ProductSizeLimited && thisCartItem.ProductCart.ProductSizeLimitedQty > 0)
                            {
                                repositoryProduct.UpdateLimitedQty(thisCartItem.ProductCart.ProductSizeId, thisCartItem.Quantity, orderInvoiceId, DateTime.Now);
                            }
                        }
                    }

                    cart.Clear();
                    HttpContext.Current.Session["Cart"] = new Cart();

                }

                return orderInvoiceId;
            }

            // get coupon data
            public Coupon ValidateCoupon(string couponCode, int storeFrontId, DateTime thisDate)
            {
                DataSet dsCouponInfo = _orderDbl.ValidateCoupon(couponCode, storeFrontId, thisDate);

                Coupon couponInfo = null;

                if (dsCouponInfo != null && dsCouponInfo.Tables[0].Rows.Count > 0)
                {
                    couponInfo =
                        dsCouponInfo.Tables[0].AsEnumerable()
                            .Select(row => new Coupon
                            {
                                CouponId = row.Field<int>("Coupon_Id"),
                                CouponCode = row.Field<string>("Coupon_Code_Pk"),
                                CouponDiscountAmount = Convert.IsDBNull(row.Field<decimal>("Coupon_DiscountPrice")) ? 0m : row.Field<decimal>("Coupon_DiscountPrice"),
                                CouponDiscountRate = Convert.IsDBNull(row.Field<decimal>("Coupon_DiscountRate")) ? 0m : row.Field<decimal>("Coupon_DiscountRate"),
                                CouponMaxUse = Convert.IsDBNull(row.Field<int>("Coupon_Maxuse")) ? 0 : row.Field<int>("Coupon_Maxuse"),
                                CouponMinPurchase = Convert.IsDBNull(row.Field<decimal>("Coupon_MinPurchase")) ? 0m : row.Field<decimal>("Coupon_MinPurchase"),
                                CouponMaxDiscount = Convert.IsDBNull(row.Field<object>("Coupon_MaxDiscount")) ? 0m : Convert.ToDecimal(row.Field<object>("Coupon_MaxDiscount")),
                                CouponActive = row.Field<object>("Coupon_Active") != DBNull.Value && Convert.ToBoolean(row.Field<object>("Coupon_Active")),
                                CouponFreeShip = row.Field<object>("Coupon_FreeShip") != DBNull.Value && Convert.ToBoolean(row.Field<object>("Coupon_FreeShip")),
                                CouponExpiryDate = row.Field<object>("Coupon_ExpiryDate") as DateTime?,
                                CouponValid = Convert.IsDBNull(row.Field<int>("Coupon_Valid")) ? 0 : row.Field<int>("Coupon_Valid"),
                                CouponMessage = Convert.IsDBNull(row.Field<string>("Coupon_Message")) ? String.Empty : row.Field<string>("Coupon_Message"),
                                CouponNewCustomerOnly = row.Field<object>("Coupon_NewCustomer") != DBNull.Value && Convert.ToBoolean(row.Field<object>("Coupon_NewCustomer")),
                                CouponPromotionalFreeOrder = row.Field<object>("Coupon_PromotionalFreeOrder") != DBNull.Value && Convert.ToBoolean(row.Field<object>("Coupon_PromotionalFreeOrder"))

                            }).ToList().FirstOrDefault();
                }

                return couponInfo;
            }

            // get count of how many times a coupon was used by customer
            public int GetCouponUsageCountByCustomer(int customerId, int couponId, int storeFrontId)
            {
                return _orderDbl.GetCouponUsageCountByCustomer(customerId, couponId, storeFrontId);
            }

            // check if coupon is allowed to a customer
            public bool IsCustomerAllowedCoupon(int customerId, int couponId, int storeFrontId)
            {
                return _orderDbl.IsCustomerAllowedCoupon(customerId, couponId, storeFrontId);
            }

            // get order detail used by order confirmation page
            public OrderDetailMinimal GetOrderDetailMinimal(int orderId, int storefrontId)
            {
                DataSet dsOrderDetail = _orderDbl.GetOrderDetailMinimal(orderId);

                OrderDetailMinimal orderDetail = null;

                if (dsOrderDetail.Tables[0].Rows.Count > 0)
                {
                    orderDetail =
                       dsOrderDetail.Tables[0].AsEnumerable()
                       .Select(row => new OrderDetailMinimal()
                       {
                           OrderInvoiceId = Convert.IsDBNull(row.Field<object>("OrderInvoice_Id")) ? 0 : Convert.ToInt32(row.Field<object>("OrderInvoice_Id")),
                           OrderInvoiceDateCreated = (DateTime)row.Field<object>("OrderInvoice_DateCreated"),
                           OrderInvoiceCreditAmount = Convert.IsDBNull(row.Field<object>("OrderInvoice_CreditAmount")) ? 0.0m : Convert.ToDecimal(row.Field<object>("OrderInvoice_CreditAmount")),
                           OrderInvoiceCouponAmount = Convert.IsDBNull(row.Field<object>("Orderinvoice_CouponAmount")) ? 0.0m : Convert.ToDecimal(row.Field<object>("Orderinvoice_CouponAmount")),
                           OrderInvoiceShippingPrice = Convert.IsDBNull(row.Field<object>("OrderInvoice_ShippingPrice")) ? 0.0m : Convert.ToDecimal(row.Field<object>("OrderInvoice_ShippingPrice")),
                           OrderCart = GetOrderCart(row.Field<int>("OrderInvoice_Id"), storefrontId),
                           OrderInvoiceCustomerFk = Convert.IsDBNull(row.Field<object>("Orderinvoice_Customer_Fk")) ? 0 : Convert.ToInt32(row.Field<object>("Orderinvoice_Customer_Fk")),
                           OrderInvoiceProvinceTaxAmount = Convert.IsDBNull(row.Field<object>("Orderinvoice_ProvincialTaxAmount")) ? 0.0m : Convert.ToDecimal(row.Field<object>("Orderinvoice_ProvincialTaxAmount")),
                           OrderInvoiceGlobalTaxAmount = Convert.IsDBNull(row.Field<object>("Orderinvoice_GlobalTaxAmount")) ? 0.0m : Convert.ToDecimal(row.Field<object>("Orderinvoice_GlobalTaxAmount")),
                           OrderInvoiceHarmonizedTaxAmount = Convert.IsDBNull(row.Field<object>("Orderinvoice_HarmonizedTaxAmount")) ? 0.0m : Convert.ToDecimal(row.Field<object>("Orderinvoice_HarmonizedTaxAmount")),

                           OrderInvoiceProvincialTaxPercentage = Convert.IsDBNull(row.Field<object>("Orderinvoice_ProvincialTaxPercentage")) ? 0.0m : Convert.ToDecimal(row.Field<object>("Orderinvoice_ProvincialTaxPercentage")),
                           OrderInvoiceGlobalTaxPercentage = Convert.IsDBNull(row.Field<object>("Orderinvoice_GlobalTaxPercentage")) ? 0.0m : Convert.ToDecimal(row.Field<object>("Orderinvoice_GlobalTaxPercentage")),
                           OrderInvoiceHarmonizedTaxPercentage = Convert.IsDBNull(row.Field<object>("Orderinvoice_HarmonizedTaxPercentage")) ? 0.0m : Convert.ToDecimal(row.Field<object>("Orderinvoice_HarmonizedTaxPercentage")),
                           OrderInvoiceCouponCodePk = row.Field<string>("OrderInvoice_CouponCode"),

                       }).ToList().FirstOrDefault();

                }
                return orderDetail;
            }

            // get cart details for order id
            public List<CartDetail> GetOrderCart(int orderId, int storefrontId)
            {
                DataSet dsCustomerOrderCart = _orderDbl.GetOrderCart(orderId, storefrontId);

                List<CartDetail> customerOrderCart = null;

                if (dsCustomerOrderCart.Tables[0].Rows.Count > 0)
                {
                    customerOrderCart =
                        dsCustomerOrderCart.Tables[0].AsEnumerable()
                            .Select(row => new CartDetail
                            {
                                CartShippingInvoiceFk = Convert.IsDBNull(row.Field<object>("Cart_ShippingInvoice_Fk")) ? 0 : Convert.ToInt32(row.Field<object>("Cart_ShippingInvoice_Fk")),
                                CartItemPrice = Convert.IsDBNull(row.Field<object>("Cart_ItemPrice")) ? 0.0m : Convert.ToDecimal(row.Field<object>("Cart_ItemPrice")),
                                CartItemQuantity = Convert.IsDBNull(row.Field<object>("Cart_ItemQuantity")) ? 0 : Convert.ToInt32(row.Field<object>("Cart_ItemQuantity")),
                                ProductSizeId = Convert.IsDBNull(row.Field<object>("Productsize_Id")) ? 0 : Convert.ToInt32(row.Field<object>("Productsize_Id")),
                                ProductName = Convert.IsDBNull(row.Field<string>("Product_Name")) ? string.Empty : row.Field<string>("Product_Name"),
                                ProductTypeFk = Convert.IsDBNull(row.Field<object>("Product_Producttype_Fk")) ? 0 : Convert.ToInt32(row.Field<object>("Product_Producttype_Fk")),
                                ProductSafeUrlName = Convert.IsDBNull(row.Field<string>("Product_SafeUrlName")) ? string.Empty : row.Field<string>("Product_SafeUrlName"),
                                ProductSizeStoreFrontPrice = Convert.IsDBNull(row.Field<object>("Productsize_Storefront_Price")) ? 0.0m : Convert.ToDecimal(row.Field<object>("Productsize_Storefront_Price")),
                                ProductSizeHeader = Convert.IsDBNull(row.Field<string>("ProductSize_Header")) ? string.Empty : row.Field<string>("ProductSize_Header"),
                                ProductSizeStrength = Convert.IsDBNull(row.Field<string>("ProductSize_Strength")) ? string.Empty : row.Field<string>("ProductSize_Strength"),
                                ProductSizeQuantity = Convert.IsDBNull(row.Field<object>("ProductSize_Quantity")) ? 0 : Convert.ToInt32(row.Field<object>("ProductSize_Quantity")),
                                ProductQuestionnaireCatId = Convert.IsDBNull(row.Field<object>("Product_QuestionnaireCategory_Fk")) ? 0 : Convert.ToInt32(row.Field<object>("Product_QuestionnaireCategory_Fk")),

                            }).ToList();

                }
                return customerOrderCart;
            }

            // check if customer has placed any order
            public bool IfCustomerHasOrders(int customerId)
            {
                return _orderDbl.IfCustomerHasOrders(customerId);
            }

            //Comment: Tax- get provincial tax details
            public TaxDetails GetProvinceTaxDetails(int provinceStateId)
            {
                DataSet dsTaxDeails = _orderDbl.GetProvinceTaxDetails(provinceStateId);
                TaxDetails proinceTaxDetails = new TaxDetails();
                if (dsTaxDeails.Tables[0].Rows.Count > 0)
                {
                    proinceTaxDetails =
                       dsTaxDeails.Tables[0].AsEnumerable()
                       .Select(row => new TaxDetails()
                       {

                           GlobalTaxPercentage = Convert.IsDBNull(row.Field<object>("provincialtax_globaltaxPercent")) ? 0.0m : Convert.ToDecimal(row.Field<object>("provincialtax_globaltaxPercent")),
                           ProvincialTaxPercentage = Convert.IsDBNull(row.Field<object>("provincialtax_provincetaxPercent")) ? 0.0m : Convert.ToDecimal(row.Field<object>("provincialtax_provincetaxPercent")),
                           HSTPercentage = Convert.IsDBNull(row.Field<object>("ProvincialTax_HarmonizedTaxPercent")) ? 0.0m : Convert.ToDecimal(row.Field<object>("ProvincialTax_HarmonizedTaxPercent"))

                       }).ToList().FirstOrDefault();

                }
                return proinceTaxDetails;


            }
            // this method takes care of referral credit if applicable
            private int ApplyReferralCredit(int customerId, ICustomerRepository repositoryCustomer, AffiliateInfo affiliateInfo, int orderInvoiceId)
            {
                int creditAddStatus = 0;
                int referringCustomerId = repositoryCustomer.GetReferredBy(customerId);
                if (referringCustomerId != 0)
                {
                    int thisCustomerOrderCount = repositoryCustomer.GetCustomerOrderCount(customerId);
                    bool isCreditAlreadyIssued = repositoryCustomer.IsCreditAlreadyIssued(customerId);

                    if (thisCustomerOrderCount == 1 && !isCreditAlreadyIssued)
                    {
                        // check if referring customer exists and belongs to same affiliate
                        CustomerMinimal referredCustomerInfo = repositoryCustomer.GetReferredCustomerInfo(referringCustomerId, customerId);
                        if (referredCustomerInfo != null)
                        {
                            int referCreditTransactionType = 3;
                            string referringComment = "Customer gained Credit for referring a customer to this site. " + customerId;

                            string referredComment = "Customer referred to site. " + referringCustomerId;

                            if (referredCustomerInfo.CustomerAffiliateFk == affiliateInfo.AffiliateId)
                            {
                                creditAddStatus = repositoryCustomer.AddReferCreditToBoth(referringCustomerId,
                                                                                             affiliateInfo.ReferrerCredit,
                                                                                             referringComment,
                                                                                             customerId,
                                                                                             affiliateInfo.ReferredCredit,
                                                                                             referredComment,
                                                                                             referCreditTransactionType,
                                                                                             DateTime.Now,
                                                                                             orderInvoiceId
                                                                                             );
                            }
                        }

                    }
                }
                return creditAddStatus;
            }

            // update order invoice to clear payment pending
            public int UpdateOrderInvoicePaymentPending(string orderInvoiceToUpdate, bool paymentPending, DateTime dateUpdated)
            {
                var updateStatus = _orderDbl.UpdateOrderInvoicePaymentPending(orderInvoiceToUpdate, paymentPending, dateUpdated);
                return updateStatus;
            }

            // get cart details for order id
            public List<Questionnaire> GetQuestionnaireByCategoryId(string categoryId, string sortBy)
            {
                DataSet dsQuestionnaire = _orderDbl.GetQuestionnaireByCategoryId(categoryId, sortBy);

                List<Questionnaire> questionnaireList = null;

                if (dsQuestionnaire.Tables[0].Rows.Count > 0)
                {
                    questionnaireList =
                        dsQuestionnaire.Tables[0].AsEnumerable()
                            .Select(row => new Questionnaire
                            {
                                QuestionnaireId = Convert.IsDBNull(row.Field<object>("Questionnaire_Id")) ? 0 : Convert.ToInt32(row.Field<object>("Questionnaire_Id")),
                                QuestionnaireCategoryFk = Convert.IsDBNull(row.Field<object>("QuestionnaireCategory_id")) ? 0 : Convert.ToInt32(row.Field<object>("QuestionnaireCategory_id")),
                                QuestionnaireAskOrder = Convert.IsDBNull(row.Field<object>("Questionnaire_AskOrder")) ? 0 : Convert.ToInt32(row.Field<object>("Questionnaire_AskOrder")),
                                QuestionnaireText = Convert.IsDBNull(row.Field<string>("Questionnaire_Question")) ? string.Empty : row.Field<string>("Questionnaire_Question"),
                                QuestionnaireCategory = Convert.IsDBNull(row.Field<string>("QuestionnaireCategory_Text")) ? string.Empty : row.Field<string>("QuestionnaireCategory_Text")
                               ,
                                QuestionnaireMulAns = row.Field<object>("Questionnaire_MultipleAns") != DBNull.Value && Convert.ToBoolean(row.Field<object>("Questionnaire_MultipleAns")),
                                QuestionnaireAskMoreInfo = row.Field<object>("Questionnaire_AskMoreInfo") != DBNull.Value && Convert.ToBoolean(row.Field<object>("Questionnaire_AskMoreInfo")),
                                QuestionnaireGroupId = Convert.IsDBNull(row.Field<object>("QuestionnaireCategoryRelation_Group_FK")) ? 0 : Convert.ToInt32(row.Field<object>("QuestionnaireCategoryRelation_Group_FK")),
                                QuestionnaireGroupText = Convert.IsDBNull(row.Field<string>("QuestionnaireGroup_Text")) ? string.Empty : row.Field<string>("QuestionnaireGroup_Text"),
                                dateCreated = Convert.IsDBNull(row.Field<object>("Questionnaire_DateCreated")) ?
                               DateTime.MinValue : Convert.ToDateTime(row.Field<object>("Questionnaire_DateCreated"))

                            }).ToList();

                }
                return questionnaireList;
            }

            // get cart details for order id
            public List<QuestionnaireOption> GetQuestionOptionByQuestionnaireId(string QuestionnaireId, string sortBy)
            {
                DataSet dsQuestionnaireOption = _orderDbl.GetQuestionOptionByQuestionnaireId(QuestionnaireId, sortBy);

                List<QuestionnaireOption> questionnaireOptionList = null;

                if (dsQuestionnaireOption.Tables[0].Rows.Count > 0)
                {
                    questionnaireOptionList =
                        dsQuestionnaireOption.Tables[0].AsEnumerable()
                            .Select(row => new QuestionnaireOption
                            {
                                QuestionOptionId = Convert.IsDBNull(row.Field<object>("QuestionnaireOption_Id")) ? 0 : Convert.ToInt32(row.Field<object>("QuestionnaireOption_Id")),
                                QuestionOptionText = Convert.IsDBNull(row.Field<string>("QuestionnaireOption_OptionText")) ? string.Empty : row.Field<string>("QuestionnaireOption_OptionText"),
                                QuestionnaireFk = Convert.IsDBNull(row.Field<object>("QuestionnaireOption_Questionnaire_Fk")) ? 0 : Convert.ToInt32(row.Field<object>("QuestionnaireOption_Questionnaire_Fk")),
                                QuestionOptionAskOrder = Convert.IsDBNull(row.Field<object>("QuestionnaireOption_Order")) ? 0 : Convert.ToInt32(row.Field<object>("QuestionnaireOption_Order")),
                                QuestionOptionAskMoreInfo = row.Field<object>("QuestionnaireOption_AskMoreInfo") != DBNull.Value && Convert.ToBoolean(row.Field<object>("QuestionnaireOption_AskMoreInfo")),
                                QuestionOptionNone = row.Field<object>("QuestionnaireOption_NoneOption") != DBNull.Value && Convert.ToBoolean(row.Field<object>("QuestionnaireOption_NoneOption")),
                                dateCreated = Convert.IsDBNull(row.Field<object>("QuestionnaireOption_DateCreated")) ?
                               DateTime.MinValue : Convert.ToDateTime(row.Field<object>("QuestionnaireOption_DateCreated")),
                                HardStop = row.Field<object>("QuestionnaireOption_HardStop") != DBNull.Value && Convert.ToBoolean(row.Field<object>("QuestionnaireOption_HardStop")),
                                QuestionOptionPlaceHolder = Convert.IsDBNull(row.Field<object>("QuestionnaireOption_PlaceHolderText")) ? string.Empty : Convert.ToString(row.Field<object>("QuestionnaireOption_PlaceHolderText"))

                            }).ToList();

                }
                return questionnaireOptionList;
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
                int questionnaireResponseId = _orderDbl.AddQuestionnaireResponse(customerId,
                                                    orderInvoiceFk,
                                                    QuestionnaireFk,
                                                    QuestionnaireCategoryFk,
                                                    answerText,
                                                    moreInfo,
                                                    QuestionnaireOptionFk,
                                                    QuestionnaireDateCreated
                                                  );
                return questionnaireResponseId;

            }

            // get cart details for order id  
            public List<QuestionnaireGroup> GetQuestionnaireGroupByCatId(string QuestionnaireCatId, string sortBy)
            {
                DataSet dsQuestionnaireGroup = _orderDbl.GetQuestionnaireGroupByCatId(QuestionnaireCatId, sortBy);

                List<QuestionnaireGroup> questionnaireGroupList = null;

                if (dsQuestionnaireGroup.Tables[0].Rows.Count > 0)
                {
                    questionnaireGroupList =
                        dsQuestionnaireGroup.Tables[0].AsEnumerable()
                            .Select(row => new QuestionnaireGroup
                            {
                                QuestionnaireGroupId = Convert.IsDBNull(row.Field<object>("QuestionnaireGroup_id")) ? 0 : Convert.ToInt32(row.Field<object>("QuestionnaireGroup_id")),
                                QuestionnaireGroupText = Convert.IsDBNull(row.Field<string>("QuestionnaireGroup_Text")) ? string.Empty : row.Field<string>("QuestionnaireGroup_Text"),
                                QuestionnaireCatId = Convert.IsDBNull(row.Field<object>("QuestionnaireCategory_id")) ? 0 : Convert.ToInt32(row.Field<object>("QuestionnaireCategory_id")),
                                GroupCount = Convert.IsDBNull(row.Field<object>("GroupCount")) ? 0 : Convert.ToInt32(row.Field<object>("GroupCount"))


                            }).ToList();

                }
                return questionnaireGroupList;
            }
            // check if user has order of same amount in past 30 minutes
            public bool IfUserAlreadyAnsweredQuestionnaire(int catId, int customerId, DateTime currentTimeStamp, int orderTimeSpan, int orderId)
            {
                DataSet dsDuplicateOrders = _orderDbl.IfUserAlreadyAnsweredQuestionnaire(catId, customerId, currentTimeStamp, orderTimeSpan, orderId);

                bool answeredQuestionnaire = false;

                if (dsDuplicateOrders.Tables[0].Rows.Count > 0)
                {
                    answeredQuestionnaire = true;

                }
                return answeredQuestionnaire;
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
                int questionnaireResponseId = _orderDbl.AddCustomerQuestionnaireResponse(customerId,
                                                    orderInvoiceFk,
                                                    categoryFk,
                                                    approve,
                                                    dateCreated,
                                                    dateModified,
                                                    hardStop

                                                  );
                return questionnaireResponseId;

            }

            public List<int> GetProductQuestionnaireCategoryByOrderId(int orderInvoicefk)
            {
                DataSet dsProdCategory = _orderDbl.GetProductQuestionnaireCategoryByOrderId(orderInvoicefk);

                List<int> prodCategoryList = null;
                if (dsProdCategory.Tables[0].Rows.Count > 0)
                {
                    prodCategoryList = dsProdCategory.Tables[0].AsEnumerable()
                        .Select(r => r.Field<int>("product_questionnairecategory_Fk"))
                        .ToList();
                }
                return prodCategoryList;
            }
            // add to order transaction
            public int AddOrderTransaction(int customerId,
                                           int orderInvoiceId,
                                           int orderTempId,
                                           int paymentOptionFk,
                                           string cardCvv2Code,
                                           string cardName,
                                           string cardNum,
                                           string cardType,
                                           string clientIp,
                                           string orderTransactionMsg,
                                           string transactionNum,
                                           decimal orderTransactionAmount,
                                           int cardCvv2Type,
                                           int cardExpireMonth,
                                           int cardExpireYear,
                                           int orderTransactionError,
                                           DateTime dateCreated,
                                           int storeFrontFk,
                                           int paymentTransactionTypeFk
                                           )
            {
                int orderTransactionId = _orderDbl.AddOrderTransaction(customerId,
                                                                                  orderInvoiceId,
                                                                                  orderTempId,
                                                                                  paymentOptionFk,
                                                                                  cardCvv2Code,
                                                                                  cardName,
                                                                                  cardNum,
                                                                                  cardType,
                                                                                  clientIp,
                                                                                  orderTransactionMsg,
                                                                                  transactionNum,
                                                                                  orderTransactionAmount,
                                                                                  cardCvv2Type,
                                                                                  cardExpireMonth,
                                                                                  cardExpireYear,
                                                                                  orderTransactionError,
                                                                                  dateCreated,
                                                                                  storeFrontFk,
                                                                                  paymentTransactionTypeFk
                                                             );
                return orderTransactionId;
            }

            public int AddCustomerSubscription(int customerFk, int orderInvoiceFk, int productFk, int productSizeFk, decimal cartItemQuantity, bool autoFillActive, DateTime dateCreated)
            {
                return _orderDbl.AddCustomerSubscription(customerFk, orderInvoiceFk, productFk, productSizeFk, cartItemQuantity, autoFillActive, dateCreated);
            }

            public List<CustomerSubscription> GetCustomerSubscription(int customerFk, string productSizeFkList)
            {
                DataSet dsSubscription = _orderDbl.GetCustomerSubscription(customerFk, productSizeFkList);

                List<CustomerSubscription> SubscriptionList = null;
                if (dsSubscription.Tables[0].Rows.Count > 0)
                {
                    SubscriptionList =
                        dsSubscription.Tables[0].AsEnumerable()
                            .Select(row => new CustomerSubscription
                            {
                                CustomerSubscriptionId = Convert.IsDBNull(row.Field<object>("CustomerSubscription_id")) ? 0 : Convert.ToInt32(row.Field<object>("CustomerSubscription_id")),
                                SubscriptionSignUpDate = (DateTime)row.Field<object>("CustomerSubscription_DateSubscriptionSignUp"),
                                SubscriptionProductSizeFk = Convert.IsDBNull(row.Field<object>("CustomerSubscription_ProductSize_fk")) ? 0 : Convert.ToInt32(row.Field<object>("CustomerSubscription_ProductSize_fk"))
                            }).ToList();
                }
                return SubscriptionList;
            }
            public RefillOrder GetOrderRefillDetails(int customerFk, string productSizeStrength, int productSizeId)
            {
                DataSet dsTaxDeails = _orderDbl.GetOrderRefillDetails(customerFk, productSizeStrength, productSizeId);
                RefillOrder proinceTaxDetails = new RefillOrder();
                if (dsTaxDeails.Tables[0].Rows.Count > 0)
                {
                    proinceTaxDetails =
                       dsTaxDeails.Tables[0].AsEnumerable()
                       .Select(row => new RefillOrder()
                       {
                           CustomerFk = Convert.IsDBNull(row.Field<object>("CustomerPrescription_Customer_Fk")) ? 0 : Convert.ToInt32(row.Field<object>("CustomerPrescription_Customer_Fk")),
                           PrescriptionId = Convert.IsDBNull(row.Field<object>("CustomerPrescription_Id")) ? 0 : Convert.ToInt32(row.Field<object>("CustomerPrescription_Id")),
                           ProductStrength = Convert.IsDBNull(row.Field<string>("QuestionnairePrescriptionDetails_ProductStrength")) ? string.Empty : row.Field<string>("QuestionnairePrescriptionDetails_ProductStrength"),
                           RefilQuantityUsed = Convert.IsDBNull(row.Field<object>("refillUsed")) ? 0 : Convert.ToInt32(row.Field<object>("refillUsed")),
                           RefilQuantityAuthorised = Convert.IsDBNull(row.Field<object>("QuestionnairePrescriptionDetails_RefillNumbers")) ? 0 : Convert.ToInt32(row.Field<object>("QuestionnairePrescriptionDetails_RefillNumbers")),
                           ProductSizeFk = Convert.IsDBNull(row.Field<object>("QuestionnairePrescriptionDetails_ProductSize_Fk")) ? 0 : Convert.ToInt32(row.Field<object>("QuestionnairePrescriptionDetails_ProductSize_Fk")),
                       }).ToList().FirstOrDefault();

                }
                return proinceTaxDetails;
            }
            // update Cart Refill
            public int UpdateOrderCartRefillStatus(int productSizeId,
                int orderInvoiceId,
                bool refillStatus)
            {
                var updateStatus = _orderDbl.UpdateOrderCartRefillStatus(productSizeId, orderInvoiceId, refillStatus);
                return updateStatus;
            }
            public List<QuestionOptionSelected> GetAnsweredQuestionnaireForCustomer(int QuestionnaireCatId, int customerFk, DateTime currentTimeStamp, int orderTimeSpan, int orderId)
            {
                DataSet dsSubscription = _orderDbl.GetAnsweredQuestionnaireForCustomer(QuestionnaireCatId, customerFk, currentTimeStamp, orderTimeSpan, orderId);

                List<QuestionOptionSelected> SubscriptionList = null;
                if (dsSubscription.Tables[0].Rows.Count > 0)
                {
                    SubscriptionList =
                        dsSubscription.Tables[0].AsEnumerable()
                            .Select(row => new QuestionOptionSelected
                            {
                                QuestionnaireId = Convert.IsDBNull(row.Field<object>("QuestionnaireResponse_Question_Fk")) ? 0 : Convert.ToInt32(row.Field<object>("QuestionnaireResponse_Question_Fk")),
                                QuestionnaireCategoryId = Convert.IsDBNull(row.Field<object>("QuestionnaireResponse_Category_Fk")) ? 0 : Convert.ToInt32(row.Field<object>("QuestionnaireResponse_Category_Fk")),
                                QuestionnaireOptionId = Convert.IsDBNull(row.Field<object>("QuestionnaireResponse_Option_Fk")) ? 0 : Convert.ToInt32(row.Field<object>("QuestionnaireResponse_Option_Fk")),
                                AnswerText = Convert.IsDBNull(row.Field<string>("QuestionnaireResponse_AnswerText")) ? string.Empty : row.Field<string>("QuestionnaireResponse_AnswerText"),
                                isSelected = true,
                                ExplanationText = Convert.IsDBNull(row.Field<object>("QuestionnaireResponse_AskMoreInfo")) ? string.Empty : row.Field<string>("QuestionnaireResponse_AskMoreInfo").Trim(),
                                ProductName = Convert.IsDBNull(row.Field<string>("ProductName")) ? string.Empty : row.Field<string>("ProductName"),
                            }).ToList();
                }
                return SubscriptionList;
            }

            public int LinkOrderTransactionToOrder(int orderTransactionId, int orderInvoiceId, string statusMessage,
                int transactionError, DateTime lastModified, string transactionNum, string transactionAuthCode, int transactionType)
            {
                int updateStatus = _orderDbl.LinkOrderTransactionToOrder(orderTransactionId, orderInvoiceId,
                       statusMessage, transactionError, DateTime.Now, transactionNum, transactionAuthCode, transactionType);
                return updateStatus;
            }

            public int AddLogOrderTransaction(int orderTransactionFk, int orderInvoiceFk, int paymentOptionFk,
                                int paymentTransactionTypeFk, DateTime dateCreated)
            {
                return _orderDbl.AddLogOrderTransaction(orderTransactionFk, orderInvoiceFk, paymentOptionFk, paymentTransactionTypeFk, dateCreated);
            }

            public int AddCustomerSessionTracker(int customerFk, int orderInvoiceFk, string actionName, string message, int trackinStatus, DateTime dateCreated)
            {
                return _orderDbl.AddCustomerSessionTracker(customerFk, orderInvoiceFk, actionName, message, trackinStatus, dateCreated);
            }

            public int UpdateCustomerSessionTracker(int trackingId, string trackingMessage, int trackingStatus, DateTime dateUpdated)
            {
                int updateStatus = _orderDbl.UpdateCustomerSessionTracker(trackingId, trackingMessage,
                                        trackingStatus, dateUpdated);
                return updateStatus;
            }

            public int UpdateOrderInvoiceBPStatus(int orderInvoiceId, bool BPNotKnown)
            {
                int updateStatus = _orderDbl.UpdateOrderInvoiceBPStatus(orderInvoiceId, BPNotKnown);
                return updateStatus;
            }

            public List<BPQuestionnaires> GetBPQuestionnaires()
            {
                DataSet dsQuestionnaire = _orderDbl.GetBPQuestionnaires();

                List<BPQuestionnaires> QuestionnaireList = null;
                if (dsQuestionnaire.Tables[0].Rows.Count > 0)
                {
                    QuestionnaireList =
                        dsQuestionnaire.Tables[0].AsEnumerable()
                            .Select(row => new BPQuestionnaires
                            {
                                QuestionnaireOptionId = Convert.IsDBNull(row.Field<object>("QuestionnaireOption_Id")) ? 0 : Convert.ToInt32(row.Field<object>("QuestionnaireOption_Id")),
                                QuestionnaireOptionText = Convert.IsDBNull(row.Field<object>("QuestionnaireOption_OptionText")) ? string.Empty : Convert.ToString(row.Field<object>("QuestionnaireOption_OptionText")),
                                QuestionnaireFk = Convert.IsDBNull(row.Field<object>("QuestionnaireOption_Questionnaire_Fk")) ? 0 : Convert.ToInt32(row.Field<object>("QuestionnaireOption_Questionnaire_Fk")),
                                QuestionnaireOptionOrder = Convert.IsDBNull(row.Field<object>("QuestionnaireOption_Order")) ? 0 : Convert.ToInt32(row.Field<object>("QuestionnaireOption_Order")),
                                QuestionnaireOption = Convert.IsDBNull(row.Field<object>("QuestionnaireOption")) ? string.Empty : Convert.ToString(row.Field<object>("QuestionnaireOption")),
                            }).ToList();
                }

                return QuestionnaireList;
            }

            public int UpdateCustomerQuestionnaireCategoryResponse(int questionnaireResponseId, int customerFk, int questionnaireProblemFk, string problemAssignUserAdmin,
                 DateTime problemAssignDate, string assignDoctorComment)
            {
                int updateStatus = _orderDbl.UpdateCustomerQuestionnaireCategoryResponse(questionnaireResponseId, customerFk, questionnaireProblemFk, problemAssignUserAdmin,
                    problemAssignDate, assignDoctorComment);
                return updateStatus;
            }

            public int UpdateBPQuestionnaireResponse(int customerFk, int orderInvoiceFk, int questionFk, int responseOptionFk)
            {
                int updateStatus = _orderDbl.UpdateBPQuestionnaireResponse(customerFk, orderInvoiceFk,
                                        questionFk, responseOptionFk);
                return updateStatus;
            }

            public int AddLogPreScreenOrderBPUpdate(int customerFk, int orderInvoiceFk, int categoryResponseFk, string comment, string ipAddress, DateTime dateCreated)
            {
                int updateStatus = _orderDbl.AddLogPreScreenOrderBPUpdate(customerFk, orderInvoiceFk, categoryResponseFk,
                                        comment, ipAddress, dateCreated);
                return updateStatus;
            }

            public int AddEmailRecordForProcessing(int storeFrontFk, int sourceApplicationFk, string toEmail, string emailSubject, string emailBody, int orderInvoiceFk,
                int customerFk, int nitroUserFk, int emailTypeFk, int interval, int maxEmailSentCount)
            {
                int updateStatus = _orderDbl.AddEmailRecordForProcessing(storeFrontFk, sourceApplicationFk, toEmail, emailSubject, emailBody, orderInvoiceFk,
                                        customerFk, nitroUserFk, emailTypeFk, interval, maxEmailSentCount);
                return updateStatus;
            }

            public int UpdateNitroEmailQueueStatus(int customerFk, int orderInvoiceFk)
            {
                int updateStatus = _orderDbl.UpdateNitroEmailQueueStatus(customerFk, orderInvoiceFk);
                return updateStatus;
            }

            public int AddCustomerQuestionnaireIssues(int customerFk, int orderInvoiceFk, int questionnaireFk, int deviceType, string exception, bool completeStatus, DateTime dateCreated)
            {
                int updateStatus = _orderDbl.AddCustomerQuestionnaireIssues(customerFk, orderInvoiceFk, questionnaireFk,
                                        deviceType, exception, completeStatus, dateCreated);
                return updateStatus;
            }

            public int UpdateCustomerQuestionnaireResponse(int customerFk, int orderInvoiceFk, int questionnaireFk, int deviceType, bool completeStatus, string operationType, DateTime dateCreated)
            {
                int updateStatus = _orderDbl.UpdateCustomerQuestionnaireResponse(customerFk, orderInvoiceFk, questionnaireFk, deviceType,
                                        completeStatus, operationType, dateCreated);
                return updateStatus;
            }

            public RefillOrder GetCustomerActiveRefillOrders(int customerId, int productSizeFk, DateTime currentDate)
            {
                DataSet dsOrderDetail = _orderDbl.GetCustomerActiveRefillOrders(customerId, productSizeFk, currentDate);

                RefillOrder orderDetail = null;

                if (dsOrderDetail.Tables[0].Rows.Count > 0)
                {
                    orderDetail =
                       dsOrderDetail.Tables[0].AsEnumerable()
                       .Select(row => new RefillOrder()
                       {
                           OrderInvoiceId = Convert.IsDBNull(row.Field<object>("orderinvoice_id")) ? 0 : Convert.ToInt32(row.Field<object>("OrderInvoice_Id")),
                           NumberOfRefillsAllowed = Convert.IsDBNull(row.Field<object>("QuestionnairePrescriptionDetails_RefillNumbers")) ? 0 :
                                                Convert.ToInt32(row.Field<object>("QuestionnairePrescriptionDetails_RefillNumbers")),
                           NumberOfRefillsAvailable = Convert.IsDBNull(row.Field<object>("QuestionnairePrescriptionDetails_RefillsAvailable")) ? 0 :
                                                Convert.ToInt32(row.Field<object>("QuestionnairePrescriptionDetails_RefillsAvailable")),
                           ProductSizeFk = Convert.IsDBNull(row.Field<object>("Cart_Productsize_Fk")) ? 0 : Convert.ToInt32(row.Field<object>("Cart_Productsize_Fk")),
                           SubstituteProductSizeFk = Convert.IsDBNull(row.Field<object>("SubstituteProductSizeId")) ? 0 : Convert.ToInt32(row.Field<object>("SubstituteProductSizeId")),
                           PrescriptionExpiryDate = (DateTime)(row.Field<object>("CustomerPrescription_ExpiryDate")),
                           SubstitutionPermitted = row.Field<object>("QuestionnairePrescriptionDetails_SubstitutionPermitted") != DBNull.Value && Convert.ToBoolean(row.Field<object>("QuestionnairePrescriptionDetails_SubstitutionPermitted")),
                           RefillShipInvoiceFk = Convert.IsDBNull(row.Field<object>("Cart_Shippinginvoice_Fk")) ? 0 : Convert.ToInt32(row.Field<object>("Cart_Shippinginvoice_Fk"))
                       }).ToList().FirstOrDefault();

                }
                return orderDetail;
            }

            public int UpdateOrderInvoiceRefillStatus(bool refillStatus, int orderInvoiceFk)
            {
                int updateStatus = _orderDbl.UpdateOrderInvoiceRefillStatus(refillStatus, orderInvoiceFk);
                return updateStatus;
            }

            public int AddLogRefillOrders(int orderInvoiceFk, int shipInvoiceFk, int refillsAllowed, int refillsAvailable,
                                            string refillMessage, string refillAction, DateTime? autoRefillStartDate, 
                                            DateTime dateCreated, bool isAutoRefill)
            {
                int logId = _orderDbl.AddLogRefillOrders(orderInvoiceFk, shipInvoiceFk, refillsAllowed, refillsAvailable,
                                            refillMessage, refillAction, autoRefillStartDate,  dateCreated, isAutoRefill);
                return logId;
            }

        // update log Refill order
        public int UpdateLogRefillOrder(int logRefillOrderId,   bool refillComplete, DateTime refillCompleteDate, int orderInvoiceFk)
        {
            var updateStatus = _orderDbl.UpdateLogRefillOrder(logRefillOrderId, refillComplete, refillCompleteDate, orderInvoiceFk);
            return updateStatus;
        }

        // get cart details for order id
        public List<Questionnaire> GetRefillQuestionnaire()
        {
            DataSet dsQuestionnaire = _orderDbl.GetRefillQuestionnaire();

            List<Questionnaire> questionnaireList = null;

            if (dsQuestionnaire.Tables[0].Rows.Count > 0)
            {
                questionnaireList =
                    dsQuestionnaire.Tables[0].AsEnumerable()
                        .Select(row => new Questionnaire
                        {
                            QuestionnaireId = Convert.IsDBNull(row.Field<object>("RefillQuestionnaire_id")) ? 0 : Convert.ToInt32(row.Field<object>("RefillQuestionnaire_id")),
                            QuestionnaireAskOrder = Convert.IsDBNull(row.Field<object>("RefillQuestionnaire_AskOrder")) ? 0 : Convert.ToInt32(row.Field<object>("RefillQuestionnaire_AskOrder")),
                            QuestionnaireText = Convert.IsDBNull(row.Field<string>("RefillQuestionnaire_Question")) ? string.Empty : row.Field<string>("RefillQuestionnaire_Question"),
                           dateCreated = Convert.IsDBNull(row.Field<object>("RefillQuestionnaire_dateCreated")) ?
                                            DateTime.MinValue : Convert.ToDateTime(row.Field<object>("RefillQuestionnaire_dateCreated"))

                        }).ToList();

            }
            return questionnaireList;
        }

        public int AddRefillQuestionnaireResponse(int customerFk, int orderInvoiceFk, List<QuestionOptionSelected> answerList, DateTime dateCreated)
        {
            var updateStatus = _orderDbl.AddRefillQuestionnaireResponse(customerFk, orderInvoiceFk, answerList, dateCreated);
            return updateStatus;
        }
    }
    }
