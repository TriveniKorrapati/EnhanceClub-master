using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Transfer;
using EnhanceClub.Domain.Abstract;
using EnhanceClub.Domain.Entities;
using EnhanceClub.Domain.Entities.Enum;
using EnhanceClub.WebUI.Infrastructure.Utility;
using EnhanceClub.WebUI.Models;
using RazorEngine.Templating;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace EnhanceClub.WebUI.Controllers
{
    public class OrderController : Controller
    {
        // customer repository
        private readonly ICustomerRepository _customerRepository;
        private readonly IStorefrontRepository _storefrontRepository;
        private readonly IOrderProcessor _orderRepository;
        private readonly IProductRepository _productRepository;
        private readonly IEmailSender _emailSender;
        private readonly IAdminRepository _adminRepository;

        public int PageSize = 25;

        public OrderController(ICustomerRepository customerRepository,
            IStorefrontRepository storefrontRepository,
            IOrderProcessor orderRepository, IProductRepository productRepository, IEmailSender mailSender,
            IAdminRepository adminRepository)
        {
            _customerRepository = customerRepository;
            _storefrontRepository = storefrontRepository;
            _orderRepository = orderRepository;
            _productRepository = productRepository;
            _emailSender = mailSender;
            _adminRepository = adminRepository;
        }

        // this action method is created to avoid duplication on orderconfirm if user refreshes the page.
        [Authorize]
        public ActionResult OrderConfirm(int orderId, int orderTotal, LoggedCustomer loggedCustomer, AffiliateInfo affiliateInfo)
        {
            if (loggedCustomer.CustomerId == 0)
            {
                return RedirectToAction("Login", "Customer");
            }

            @ViewBag.Title = " Order Received | " + SiteConfigurationsWc.StorefrontName;
            // get customer info
            CustomerMinimal customerInfo = _customerRepository.GetCustomerInfo(loggedCustomer.CustomerId);

            customerInfo.Sha1CustomerEmail = Utility.GetSha1(customerInfo.CustomerEmail);
            OrderDetailMinimal orderInfo = _orderRepository.GetOrderDetailMinimal(orderId, affiliateInfo.AffiliateStoreFrontFk);
            if (orderInfo.OrderInvoiceCustomerFk == loggedCustomer.CustomerId)
            {
                string questionnaireCatId = null;
                bool multipleCat = false;
                foreach (var product in orderInfo.OrderCart)
                {
                    if (multipleCat)
                    {
                        if (!questionnaireCatId.Contains(product.ProductQuestionnaireCatId.ToString()))
                        {
                            questionnaireCatId = questionnaireCatId + "," + product.ProductQuestionnaireCatId.ToString();
                        }
                    }
                    else
                    {
                        questionnaireCatId = product.ProductQuestionnaireCatId.ToString();
                    }

                    multipleCat = true;
                }
                var orderTimeElapsed = DateTime.Now.Subtract(orderInfo.OrderInvoiceDateCreated).TotalMinutes;

                decimal OrderTax = orderInfo.OrderInvoiceProvinceTaxAmount +
                    orderInfo.OrderInvoiceGlobalTaxAmount +
                    orderInfo.OrderInvoiceHarmonizedTaxAmount;

                OrderTax = OrderTax * 100;

                return View(new OrderConfirmViewModel
                {
                    OrderId = orderId,
                    OrderTotal = orderInfo.OrderTotal,
                    CartTotal = orderInfo.CartTotal,
                    ShippingPrice = orderInfo.OrderInvoiceShippingPrice,
                    StoreFrontFax = affiliateInfo.StoreFrontFax,
                    ProductList = orderInfo.OrderCart,
                    HasRxProducts = orderInfo.CartHasRxProducts(),
                    CustomerInfo = customerInfo,
                    OrderTimeElapsed = orderTimeElapsed,
                    QuestionnaireCatId = questionnaireCatId,
                    OrderInfo = orderInfo,
                    OrderTax = OrderTax
                });
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        [SessionExpire]
        [HttpPost]
        public ActionResult OrderCreate(Cart cart, AffiliateInfo affiliateInfo, LoggedCustomer loggedCustomer, ShippingDetails shipDetails,
                                    Customer customer,
                                    string cardNumber, int cardExpireMonth, int cardExpireYear,
                                    string cardHolderName, string cardType, string cvvCode,
                                    IEnumerable<ProductCart> autoFilledCart = null, int selectedShipAddressFk = 0)

        {


            if (loggedCustomer.CustomerId == 0)
            {
                return Redirect("/get-started");
            }
            // if cart is empty redirect to cart
            if (!cart.CartItems.Any())
            {
                // if cart is empty because user refreshed order confirm page, send to home page so that db operation are not duplicated
                return RedirectToAction("Index", "Home");
            }
            @ViewBag.Title = "Order Confirmation Success | " + SiteConfigurationsWc.StorefrontUrl;
            if (Request.Url != null) { @ViewBag.canonicalRef = Request.Url.AbsoluteUri; }

            //get selected shipping address for multiple address
            if (SiteConfigurationsWc.ShowMultipleShippingAddress == 1)
            {
                var customerShipAddress = _customerRepository.GetCustomerShippingAddress(loggedCustomer.CustomerId, selectedShipAddressFk).FirstOrDefault();
                if (customerShipAddress.CustomerAddressId > 0)
                {
                    shipDetails.ShippingAddress = customerShipAddress.CustomerAddress;
                    shipDetails.ShippingCity = customerShipAddress.CustomerCity;
                    shipDetails.ShippingFirstName = customerShipAddress.CustomerFirstName;
                    shipDetails.ShippingLastName = customerShipAddress.CustomerLastName;
                    shipDetails.ShippingZipCode = customerShipAddress.CustomerZipCode;
                    shipDetails.ShippingCountryId = customerShipAddress.ShippingCountryId;
                    shipDetails.ShippingProvinceId = customerShipAddress.ShippingProvinceId;
                }

            }

            int paymentOptionId = affiliateInfo.DefaultPaymentOptionFk;
            var paymentOptionName = affiliateInfo.DefaultPaymentOptionName;

            // if payment is done fully with customer credit then set paymentOptionId = 2
            bool fullCreditApplied = false;
            if (cart.CreditAppliedFlag && cart.CartNetTotal == 0)
            {
                paymentOptionId = 2;
                fullCreditApplied = true;
            }

            /* logic to change payment option in case of partial payment by credit and coupon */
            if (cart.CreditApplied > 0 && cart.CouponAmount > 0 && cart.CartNetTotal == 0)
            {
                paymentOptionId = 4;
                paymentOptionName = "Pay via Coupon";
            }
            else if (cart.CouponAmount > 0 && cart.CartNetTotal == 0)
            {
                paymentOptionId = 4;
                paymentOptionName = "Pay via Coupon";
            }
          
            if (paymentOptionId != 0)
            {
                cart.PaymentOptionId = paymentOptionId;
            }
            // get customer info
            CustomerMinimal customerInfo = _customerRepository.GetCustomerInfo(loggedCustomer.CustomerId);

            // update the billing address details in case user changes at front end for the current order for both existing and new user
            customerInfo.CustomerCountryId = customer.CustomerCountryId;
            customerInfo.CustomerFirstName = customer.CustomerFirstName;
            customerInfo.CustomerLastName = customer.CustomerLastName;

            customerInfo.CustomerProvinceId = customer.CustomerProvinceId;
            customerInfo.CustomerAddress = customer.CustomerAddress;
            customerInfo.CustomerCity = customer.CustomerCity;
            customerInfo.CustomerZipCode = customer.CustomerZipCode;
            PatientProfile patientProfile = new PatientProfile();

            IEnumerable<PatientProfile> patientProfileList = _customerRepository.GetPatientProfilesOfCustomer(loggedCustomer.CustomerId,
                                                                  0,
                                                                  affiliateInfo.
                                                                  AffiliateStoreFrontFk);
            if (patientProfileList != null)
            {
                if (patientProfileList.Count() > 1)
                {
                    var mailBody = loggedCustomer.CustomerId + "has multiple patienprofiles linked.";
                    // send debug email to dev team
                    _emailSender.SendEmail(SiteConfigurationsWc.DebugMessageRecipient, "Duplicate Patient profile", mailBody, string.Empty, 0);
                }
                patientProfile = patientProfileList.FirstOrDefault();

            }


            // get storefront currency exchange rate
            StoreFrontInfo storeFrontInfo = _storefrontRepository.GetStoreFrontInfo(affiliateInfo.AffiliateStoreFrontFk);

            int tmpOrderTotal = Convert.ToInt32(cart.CartNetTotal);

            var orderTotalAmount = cart.CartNetTotal;
            cardNumber = cardNumber.Replace("-", string.Empty);
            OrderTransaction orderTransaction = new OrderTransaction();

            /*Start: if flag is set to true, only then save credit card details*/
            if (SiteConfigurationsWc.SaveCreditCardDetails)
            {
                orderTransaction.OrdertransactionCardexpiremonth = cardExpireMonth;
                orderTransaction.OrdertransactionCardexpireyear = cardExpireYear;
                orderTransaction.OrdertransactionCardname = cardHolderName;
                orderTransaction.OrdertransactionCardtype = cardType;
                orderTransaction.OrdertransactionCardcvv2Code = cvvCode;

                string aesEncryptionKey = SiteConfigurationsWc.AesEncryptionKey;
                orderTransaction.OrdertransactionCardnum = Utility.EncryptCfm(aesEncryptionKey, cardNumber);
            }
            orderTransaction.OrdertransactionAmount = fullCreditApplied == true ? cart.CreditApplied : orderTotalAmount;

            // set payment process variables

            int orderTransactionError = 1;
            string orderTransactionMsg = "Transaction Disrupted- EC";
            string clientIp = Request.UserHostAddress;


            // add order transaction
            var ordertransactionId =
                        _orderRepository.AddOrderTransaction(customerInfo.CustomerId,
                        0, // orderinvoice id is passed as 0
                        0, // ordertempinvoiceid is passed as 0
                        paymentOptionId,
                        orderTransaction.OrdertransactionCardcvv2Code,
                        orderTransaction.OrdertransactionCardname,
                        orderTransaction.OrdertransactionCardnum,
                        orderTransaction.OrdertransactionCardtype,
                        clientIp,
                        orderTransactionMsg,
                        orderTransaction.OrdertransactionTransactionnum,
                        orderTransaction.OrdertransactionAmount,
                        0, //cvv2 type is set to 0
                        orderTransaction.OrdertransactionCardexpiremonth != null ? orderTransaction.OrdertransactionCardexpiremonth.Value : 0,
                        orderTransaction.OrdertransactionCardexpireyear != null ? orderTransaction.OrdertransactionCardexpireyear.Value : 0,
                        orderTransactionError,
                        DateTime.Now,
                        affiliateInfo.AffiliateStoreFrontFk,
                        1); // pass 1 for transaction disrupt

            var paymentTransactionTypeFk = 2; // 2 for auth
            if (SiteConfigurationsWc.EnablePayment)
            {
                if (!cart.CouponPromotionalFreeOrder && cart.CartNetTotal > 0)
                {
                    orderTransaction = ProcessDirectPostPayment(cardNumber, cardExpireMonth, cardExpireYear, cardHolderName, cardType, cvvCode,
                        orderTotalAmount, customerInfo, affiliateInfo.AffiliateStoreFrontFk, paymentOptionId, paymentOptionName, affiliateInfo.PaymentTransactionType);
                }
                else
                {
                    if (cart.CouponPromotionalFreeOrder)
                    {
                        paymentTransactionTypeFk = 8;
                        orderTransaction.OrdertransactionStatusmessage = "Fully Paid Promotional coupon applied";
                    }
                    else if (fullCreditApplied)
                    {
                        paymentTransactionTypeFk = 11;
                        orderTransaction.OrdertransactionStatusmessage = "Paid By Customer Credit";
                    }
                    else if(cart.CartNetTotal == 0 && cart.CouponAmount > 0) {

                        paymentTransactionTypeFk = 12;
                        orderTransaction.OrdertransactionStatusmessage = "Fully Paid By Coupon";
                    }
                    orderTransaction.OrdertransactionStatusId = 1;
                }
            }
            else
            {
                orderTransaction.OrdertransactionStatusId = 1;

            }
            orderTransaction.OrdertransactionId = ordertransactionId;

            // if payment is authorized, only then create order
            if (orderTransaction.OrdertransactionStatusId == 1)
            {
                if (SiteConfigurationsWc.EnableRefill == 1)
                {
                    bool isRefill = false;
                    int refillShipInvoiceFk = 0;
                    // check for refill orders
                    foreach (var cartItem in cart.CartItems)
                    {
                        // get if there are refills order exist for this product size
                        var refillOrder = _orderRepository.GetCustomerActiveRefillOrders(loggedCustomer.CustomerId,
                                                cartItem.ProductCart.ProductSizeId, DateTime.UtcNow);

                        if (refillOrder != null)
                        {
                            if (!refillOrder.SubstitutionPermitted)
                            {
                                // check if refill cart product size is same as current cartitem product size
                                if (refillOrder.ProductSizeFk == cartItem.ProductCart.ProductSizeId)
                                    isRefill = true;
                            }
                            else
                            {
                                isRefill = true;
                            }
                            if (cart.IsAutoRefillOrder)
                            {
                                refillShipInvoiceFk = cartItem.CartItemShipInvoiceFk == 0 ? refillOrder.RefillShipInvoiceFk : cartItem.CartItemShipInvoiceFk;
                                cartItem.AutoRefillCompleted = true;
                                cartItem.AutoRefillEndDate = DateTime.UtcNow;
                                cartItem.RefillMessage = "Auto Refill from Email Of Shipping Id: " + refillShipInvoiceFk;
                               
                            }                                
                            else
                            {
                                refillShipInvoiceFk = refillOrder.RefillShipInvoiceFk;
                                cartItem.RefillMessage = "Refill Of Shipping Id: " + refillShipInvoiceFk;
                            }

                            /// update cartItem refill properties
                            cart.UpdateCartItemRefill(cartItem.ProductCart.ProductSizeId, isRefill, refillShipInvoiceFk,
                                        refillOrder.NumberOfRefillsAllowed, refillOrder.NumberOfRefillsAvailable);

                        }

                    }
                }

                // Create Order Invoice
                int newOrderId = _orderRepository.ProcessOrderNoPayment(cart,
                                                                        affiliateInfo,
                                                                        customerInfo,
                                                                        shipDetails,
                                                                        storeFrontInfo,
                                                                        _customerRepository,
                                                                        _productRepository,
                                                                        _emailSender,
                                                                        patientProfile,
                                                                        paymentTransactionTypeFk
                                                                        );
                // send order created email 
                if (newOrderId > 0)
                {
                    if (SiteConfigurationsWc.EnableRefill == 1)
                    {
                        if (cart.IsRefill)
                        {
                            int updateRefillStatus = _orderRepository.UpdateOrderInvoiceRefillStatus(cart.IsRefill, newOrderId);
                        }
                    }

                    // if the flag is set then update consultation consent for all orders
                    if (SiteConfigurationsWc.UpdateConsultationConsentForAllOrders)
                    {
                        int updateStatus = _customerRepository.UpdateOrderConsultationFlag(newOrderId, true);
                    }


                    if (orderTransaction.OrdertransactionId != 0)
                    {
                        int updateStatus = _orderRepository.LinkOrderTransactionToOrder(orderTransaction.OrdertransactionId, newOrderId,
                                 orderTransaction.OrdertransactionStatusmessage, 0, DateTime.Now,
                                 orderTransaction.OrdertransactionTransactionnum, orderTransaction.OrderTransactionPaymentAuthCode, paymentTransactionTypeFk);
                    }

                    var logOrderTransactionId = _orderRepository.AddLogOrderTransaction(orderTransaction.OrdertransactionId, newOrderId, paymentOptionId,
                                                    paymentTransactionTypeFk, DateTime.Now);


                    // to be checked fro autofill, commented  : 22 March 2021
                    //  UpdateCartForAutoRefill(newOrderId, autoFilledCart, customerInfo);

                    OrderDetail orderDetail = _customerRepository.GetCustomerOrderDetail(customerInfo.CustomerId, affiliateInfo.AffiliateStoreFrontFk, newOrderId).FirstOrDefault();

                    // Order confirmation email to Customer 
                    if (SiteConfigurationsWc.EmailSend)
                    {
                        try
                        {
                            var templateService = new TemplateService();
                            var emailHtmlBody = "";

                            string templateFilePath =
                                System.Web.HttpContext.Current.Server.MapPath(SiteConfigurationsWc.EmailTemplatePath + "OrderCreatedEmail.cshtml");

                            if (!string.IsNullOrEmpty(templateFilePath))
                            {
                                emailHtmlBody = templateService.Parse(System.IO.File.ReadAllText(templateFilePath), orderDetail, null, null);
                            }

                            var mailSubject = "Order Confirmation";
                            _emailSender.SendEmail(customerInfo.CustomerEmail, mailSubject, emailHtmlBody, SiteConfigurationsWc.StorefrontSales, newOrderId);

                            // Start: add send email log, shown in customer summary popup Nitro
                            var logId = _adminRepository.AddLogSendEmailReminder(0, orderDetail.StoreFrontId,
                                                        customerInfo.CustomerId, newOrderId, orderDetail.OrderInvoiceBillingEmail, orderDetail.OrderInvoiceBillingPhone, DateTime.Now,
                                                        (int)EmailTypeEnum.OrderCreated, mailSubject, emailHtmlBody, (int)SourceApplicationEnum.EnhanceClub);

                            // End: add send email log, shown in customer summary popup Nitro

                        }
                        catch (Exception ex)
                        {
                            //// to do : log that email was not fired, but let code pass through
                            int errorLogId = _adminRepository.AddLogUnexpected("error creating order create email html",
                                "CP-OrderRepositorySql.cs",
                                ex.InnerException == null ? ex.Message : ex.InnerException.ToString().Substring(1, 100),
                               customerInfo.CustomerId,
                                loggedCustomer.CustomerStorefrontId,
                                0,
                                0);
                        }

                    }

                    // Start: Revisit when refill functionality is finalized Code for Refill check
                    // var returnToRefill = UpdatedRefillCart(orderDetail, loggedCustomer.CustomerId);
                    ////returning to Refill Confirm if Refill order
                    //if (returnToRefill)
                    //{
                    //    return View("OrderConfirmRefill");
                    //}
                    // end: Revisit when refill functionality is finalized Code for Refill check

                }
                else
                {
                    // if order is not created in database, then cancel the auth transaction
                    var securityKey = SiteConfigurationsWc.DirecPostAPIKeyDev;
                    if (SiteConfigurationsWc.Environment == "live")
                    {
                        securityKey = SiteConfigurationsWc.DirecPostAPIKeyLive;
                    }

                    var voidApiRequest = "security_key=" + SiteConfigurationsWc.DirecPostAPIKeyDev + "&transactionid=" +
                        orderTransaction.OrdertransactionTransactionnum + "&type=void";

                    // call private method to call direct post api
                    var responseKeyValue = PayByDirectPostApi(voidApiRequest);

                    if (responseKeyValue.Count > 0)
                    {
                        orderTransaction.OrdertransactionTransactionnum = responseKeyValue["transactionid"];
                        orderTransaction.OrdertransactionStatusmessage = responseKeyValue["responsetext"];
                        orderTransaction.OrderTransactionPaymentAuthCode = responseKeyValue["authcode"];

                        int updateStatus = _orderRepository.LinkOrderTransactionToOrder(orderTransaction.OrdertransactionId, newOrderId,
                                   orderTransaction.OrdertransactionStatusmessage, 0, DateTime.Now,
                                   orderTransaction.OrdertransactionTransactionnum, orderTransaction.OrderTransactionPaymentAuthCode, paymentTransactionTypeFk);

                        var logOrderTransactionId = _orderRepository.AddLogOrderTransaction(orderTransaction.OrdertransactionId, newOrderId, paymentOptionId,
                                                        5, DateTime.Now);


                    }

                    return View("OrderNotCreated");


                }

                return RedirectToAction("OrderConfirm", "Order", new { @OrderId = newOrderId, OrderTotal = tmpOrderTotal });
            }
            else
            {
                // payment did not go through
                TempData["creditCardError"] = "Invalid Payment Info";

                return RedirectToAction("Checkout", "Order");
            }

        }

        [Authorize]
        [SessionExpire]
        public ActionResult Checkout(Cart cart,
                                    AffiliateInfo affiliateInfo,
                                    LoggedCustomer loggedCustomer,
                                    ShippingDetails shipDetails,
                                    int shippingOptionId = 0,
                                    decimal creditApplied = 0.0m,
                                    string couponCode = "",
                                    int selectedShipAddressFk = 0,
                                    int getCouponCodeCrossValue = 0
                                    )
        {
            // added check for logged in customer to prevent null refernce error if session expires
            if (loggedCustomer.CustomerId == 0)
            {
                return Redirect("/get-started");
            }
            else
            {
                // if cart is empty redirect to cart
                if (!cart.CartItems.Any())
                {
                    return RedirectToAction("Index", "Home");
                }

                // get customer partial account info 
                var customerPartial = _customerRepository.GetCustomerInfoPartial(loggedCustomer.CustomerId);

                List<OrderDetail> OrderList = _customerRepository.GetCustomerAllOrders(loggedCustomer.CustomerId, affiliateInfo.AffiliateStoreFrontFk);

                int customerOrderCount = 0;
                if (OrderList != null && OrderList.Count > 0)
                {
                    customerOrderCount = OrderList.Where(o=>o.OrderInvoiceActive == true).Count();
                }

                // user is registered partially, then navigate to account-info section
                if (customerPartial != null && customerPartial.CustomerIsPartial)
                {
                    return RedirectToAction("Account-Info", "Customer", new { partialProfile = true });
                }

                // check if cart item is already opted for auto refill
                if (SiteConfigurationsWc.EnableSubscription)
                {
                    var productSizeList = cart.CartItems.Select(x => x.ProductCart.ProductSizeId).ToList();
                    string productSizeCombined = string.Join(",", productSizeList);
                    var SubscriptionProducts = _orderRepository.GetCustomerSubscription(loggedCustomer.CustomerId, productSizeCombined);

                    foreach (var cartItem in cart.CartItems)
                    {
                        if (SubscriptionProducts != null)
                        {
                            var autoFilledItem = SubscriptionProducts.Where(x => x.SubscriptionProductSizeFk == cartItem.ProductCart.ProductSizeId).FirstOrDefault();
                            if (autoFilledItem != null)
                            {
                                if ((autoFilledItem.SubscriptionSignUpDate - DateTime.Now).Days < SiteConfigurationsWc.SubscriptionInterval)
                                {
                                    cart.UpdateAutoReFill(cartItem.ProductCart.ProductSizeId);
                                }
                            }
                        }
                    }
                }
               
                @ViewBag.Title = "Shipping and Billing Details | Certified Canadian Pharmacy";

                if (Request.Url != null) { @ViewBag.canonicalRef = Request.Url.AbsoluteUri; }

                if (getCouponCodeCrossValue == 1)
                {
                    cart.CouponCode = "";
                    //    cart.CalculateCoupon(loggedCustomer, affiliateInfo, _orderRepository);
                }
                else
                {
                    if (!string.IsNullOrEmpty(couponCode))
                    {
                        cart.CouponCode = couponCode;
                        cart.CalculateCoupon(loggedCustomer, affiliateInfo, _orderRepository, cart);
                    }
                    // check if coupon was applied previously and recalculate coupon total 
                    else if (!string.IsNullOrEmpty(cart.CouponCode))
                    {
                        cart.CalculateCoupon(loggedCustomer, affiliateInfo, _orderRepository, cart);
                    }
                }

                // get credit available to customer
                CustomerCredit customerCredit = _customerRepository.GetCustomerCreditWithOrderCount(loggedCustomer.CustomerId);

                // if form is resubmitted to apply credit get shipping option Id from cart
                if (creditApplied != 0)
                {
                    shippingOptionId = cart.ShippingOptionId;
                    TempData["creditError"] = "";
                    // do server side validation for credit
                    if (creditApplied > customerCredit.CreditAmount)
                    {
                        cart.CreditApplied = 0;
                        TempData["creditError"] = "Invalid Credit Amount";
                        cart.CreditAppliedFlag = false;
                    }
                    else
                    {
                        if (creditApplied > (cart.ComputeCartTotalValue() - cart.CouponAmount))
                        {
                            creditApplied = cart.ComputeCartTotalValue() - cart.CouponAmount;
                        }

                        cart.CreditApplied = creditApplied;
                        cart.CreditAppliedFlag = true;

                        if (cart.CartNetTotal < 0)
                        {
                            TempData["creditError"] = "Discount Amount can not exceed Cart Total Amount";
                            cart.CreditApplied = 0;
                            cart.CreditAppliedFlag = false;
                        }
                    }
                    // cart.CreditApplied = creditApplied > customerCredit.CreditAmount ? 0 : creditApplied;

                }
                else
                {
                    cart.CreditApplied = 0;
                    cart.CreditAppliedFlag = false;
                }

                // get list of shipping options for the affiliate 
                List<ShippingOption> shipOptions = null;
                shipOptions = _storefrontRepository.GetShippingOptionsAffiliate(affiliateInfo.AffiliateId, shippingOptionId);

                // get list of default shipping options 
                if (shipOptions == null || shipOptions.Count == 0)
                {
                    shipOptions = _storefrontRepository.GetShippingOptionsDefault(shippingOptionId);
                }

                // assign shipping price to cart
                cart.ShippingPrice = shipOptions.First().ShippingOptionPrice;

                cart.ShippingOptionId = shipOptions.First().ShippingOptionId;


                // get customer info
                CustomerMinimal customerInfo = _customerRepository.GetCustomerInfo(loggedCustomer.CustomerId);

                // get Country List
                Country customerCountry = null;
                customerCountry = _customerRepository.GetCountry(customerInfo.CustomerCountryId).First();

                // province list for country
                // get province list
                List<Province> provinceList = _customerRepository.GetProvinceListByCountry(customerCountry.CountryId);

                // get storefront currency exchange rate
                StoreFrontInfo storeFrontInfo = _storefrontRepository.GetStoreFrontInfo(affiliateInfo.AffiliateStoreFrontFk);

                //Comment: Tax - Get the provincial and global tax percentage to calculate tax amount on otc products
                if (SiteConfigurationsWc.EnableTax)
                {
                    TaxDetails taxDetails = _orderRepository.GetProvinceTaxDetails(customerInfo.CustomerProvinceId);
                    cart.ProvincialTaxPercentage = taxDetails.ProvincialTaxPercentage;
                    cart.GlobalTaxPercentage = taxDetails.GlobalTaxPercentage;
                    cart.HSTPercentage = taxDetails.HSTPercentage;
                }

                // get customer shipping address
                var customerAddress = _customerRepository.GetCustomerShippingAddress(loggedCustomer.CustomerId, 0);

                return View("Checkout", new CheckoutViewModel
                {
                    Cart = cart,
                    CustomerInfo = customerInfo,
                    CustomerCountry = customerCountry,
                    ProvinceList = provinceList,
                    OrderShipOptions = shipOptions,
                    //ProfileList = custProfiles,
                    CreditAvailable = customerCredit == null ? 0 : customerCredit.CreditAmount,
                    StorefrontContact = affiliateInfo.StorefrontContact,
                    StoreFrontExchangeRate = storeFrontInfo.StoreFrontCurrencyExchangeRate,
                    NoChargePaymentOption = affiliateInfo.NoChargePaymentOption,
                    CustomerShippingAddresses = customerAddress,
                    SelectedShipAddressFk = selectedShipAddressFk,
                    PreviousOrderCount = customerOrderCount
                });
            }
        }

        // update tax in order summary if shipping province is changed from address
        public ActionResult UpdateCartProvincialTax(Cart cart, LoggedCustomer loggedCustomer, int customerProvinceFk)
        {
            if (loggedCustomer.CustomerId == 0)
            {
                return RedirectToAction("Login", "Customer");
            }

            if (SiteConfigurationsWc.EnableTax)
            {
                TaxDetails taxDetails = _orderRepository.GetProvinceTaxDetails(customerProvinceFk);
                cart.ProvincialTaxPercentage = taxDetails.ProvincialTaxPercentage;
                cart.GlobalTaxPercentage = taxDetails.GlobalTaxPercentage;
                cart.HSTPercentage = taxDetails.HSTPercentage;
            }
            // get customer info
            CustomerMinimal customerInfo = _customerRepository.GetCustomerInfo(loggedCustomer.CustomerId);
            customerInfo.CustomerProvinceId = customerProvinceFk;

            return PartialView("_orderSummary",
                new CheckoutViewModel
                {
                    Cart = cart,
                    CustomerInfo = customerInfo
                });
        }


        // process payment with nmi direct post api
        private OrderTransaction ProcessDirectPostPayment(string cardNumber, int cardExpiryMonth, int cardExpiryYear,
                                                        string cardHolderName, string cardType, string cvvCode,
                                                        decimal orderTotal,
                                                        CustomerMinimal customer,
                                                        int storefrontFk, int paymentOptionId,
                                                        string paymentOptionName, string paymentTransactionType)
        {
            OrderTransaction orderTransaction = new OrderTransaction();

            var cardExpiry = cardExpiryMonth.ToString("00") + cardExpiryYear.ToString().Substring(2);

            // initialize response variables
            string resPnRef = "";
            int resResult = 0;
            int responseCode = 0;
            string resRespMsg = "";
            string resAuthCode = "";
            string respAvsAddr = "";
            string respAvsZip = "";
            string respIAvs = "";
            string respCheckAccount = "";
            string respCvv = "";
            string respCheckaba = "";
            string respVaultId = "";
            string orderid = "";
            string type = "";
            string ccNumber = "";

            // build a string and pass transaction type as auth
            var securityKey = SiteConfigurationsWc.DirecPostAPIKeyDev;
            if (SiteConfigurationsWc.Environment == "live")
            {
                securityKey = SiteConfigurationsWc.DirecPostAPIKeyLive;
            }
            String strPost = "security_key=" + securityKey
               + "&firstname=" + customer.CustomerFirstName + "&lastname=" + customer.CustomerLastName
               + "&address1=" + customer.CustomerAddress + "&city=" + customer.CustomerCity + "&state=" + customer.BillingProvinceName
               + "&zip=" + customer.CustomerZipCode + "&payment=" + paymentOptionName + "&type=" + paymentTransactionType
               + "&amount=" + orderTotal + "&ccnumber=" + cardNumber + "&ccexp=" + cardExpiry + "&cvv=" + cvvCode;

            // call private method to call direct post api
            var responseKeyValue = PayByDirectPostApi(strPost);

            // read all variable from response
            if (responseKeyValue.Count > 0)
            {
                resResult = Convert.ToInt32(responseKeyValue["response"]);
                resPnRef = responseKeyValue["transactionid"];
                resRespMsg = responseKeyValue["responsetext"];
                resAuthCode = responseKeyValue["authcode"];
                respAvsAddr = responseKeyValue["avsresponse"];
                respCvv = responseKeyValue["cvvresponse"];
                orderid = responseKeyValue["orderid"];
                type = responseKeyValue["type"];
                responseCode = Convert.ToInt32(responseKeyValue["response_code"]);
                //ccNumber = responseKeyValue["cc_number"]; 
                //respVaultId = responseKeyValue["customer_vault_id"];
                //respCheckaba = responseKeyValue["checkaba"];
                //respCheckAccount = responseKeyValue["checkaccount"];
            }

            orderTransaction.OrdertransactionTransactionnum = resPnRef;

            string otrMessage = "";
            if (SiteConfigurationsWc.Environment.ToLower() == "dev")
            {
                otrMessage = "Sandbox-";
            }

            if (resResult == 1)
            {

                orderTransaction.OrdertransactionStatusmessage = "Success: " + otrMessage + " -Transaction ID  :" + resPnRef.Trim() + " -Msg: " + resRespMsg + " -AUTHCODE:" + resAuthCode.Trim() + " -CVV2MATCH:" + respCvv.Trim() + " -ResponseCode:" + responseCode;
                orderTransaction.OrdertransactionStatusId = 1;
                orderTransaction.OrdertransactionError = false;
                orderTransaction.OrderTransactionPaymentAuthCode = resAuthCode;
                orderTransaction.OrderTransactionPaymentTransactionType = 2; // 2 is used for auth transaction

                bool setAvsCheck = respAvsAddr.Trim().ToLower() == "n" || respAvsZip.Trim().ToLower() == "n" || respCvv.Trim().ToLower() == "n";

                orderTransaction.SetAvsCheck = setAvsCheck;
            }
            else
            {
                orderTransaction.OrdertransactionStatusmessage = "Error: " + otrMessage + " Transaction ID  :" + resPnRef.Trim() + " -Msg: " + resRespMsg + " -AUTHCODE:" + resAuthCode.Trim() + " -CVV2MATCH:" + respCvv.Trim() + " -ResponseCode:" + responseCode; ;
                orderTransaction.OrdertransactionStatusId = 0;
                orderTransaction.OrdertransactionError = true;
                orderTransaction.OrderTransactionPaymentAuthCode = resAuthCode;
                orderTransaction.OrderTransactionPaymentTransactionType = 2; // 2 is used for auth transaction
            }

            return orderTransaction;

        }

        private Dictionary<string, string> PayByDirectPostApi(string strPost)
        {
            StreamWriter myWriter = null;
            HttpWebRequest objRequest = (HttpWebRequest)WebRequest.Create(SiteConfigurationsWc.DirectPostAPIUrl);
            objRequest.Method = "POST";
            objRequest.ContentLength = strPost.Length;
            objRequest.ContentType = "application/x-www-form-urlencoded";

            try
            {
                myWriter = new StreamWriter(objRequest.GetRequestStream());
                myWriter.Write(strPost);
            }
            catch (Exception e)
            {
                var message = e.Message;
            }
            finally
            {
                myWriter.Close();
            }

            HttpWebResponse objResponse = (HttpWebResponse)objRequest.GetResponse();

            var result = string.Empty;

            using (StreamReader sr =
               new StreamReader(objResponse.GetResponseStream()))
            {

                result = sr.ReadToEnd();

                // Close and clean up the StreamReader
                sr.Close();
            }

            // convert response string to key value pairs
            var responseKeyValue = new Dictionary<string, string>();

            if (!string.IsNullOrEmpty(result))
                responseKeyValue = Utility.ConvertStringToDictionary(result, '&');
            return responseKeyValue;
        }


        //Added for prescription Upload

        [Authorize]
        [SessionExpire]
        public ActionResult UploadDocument(AffiliateInfo affiliateInfo, int orderId, LoggedCustomer loggedCustomer)
        {
            if (loggedCustomer.CustomerId == 0)
            {
                return RedirectToAction("Login", "Customer");
            }

            @ViewBag.Title = "Send Prescription Online - Select Your Send Method";
            @ViewBag.MetaRobot = "noindex,follow";

            bool idImageFound = false;
            bool backIdImageFound = false;
            string idImageFileName = "";
            string idBackImageName = "";

            List<CustomerIdDocument> customerDocumentList = _customerRepository.GetCustomerIdDocumentByCustomerId(loggedCustomer.CustomerId);

            if (customerDocumentList != null)
            {
                idImageFound = true;
                idImageFileName = customerDocumentList[0].CustomerIdDocumentFileName;
                idBackImageName = customerDocumentList[0].CustomerIdDocumentBackSideFileName;
                if (idBackImageName != null) { backIdImageFound = true; } else { backIdImageFound = false; }
                @ViewBag.CustomerDocumentValidated = customerDocumentList[0].CustomerIdDocumentIsValid == true ? true : false;
            }
            else { @ViewBag.idImageFound = false; @ViewBag.backIdImageFound = false; @ViewBag.idImageFileName = ""; @ViewBag.CustomerDocumentValidated = false; }

            OrderDetailMinimal orderInfo = _orderRepository.GetOrderDetailMinimal(orderId, affiliateInfo.AffiliateStoreFrontFk);
            string questionnaireCatId = null;

            bool multipleCat = false;
            foreach (var product in orderInfo.OrderCart)
            {
                if (multipleCat)
                {
                    questionnaireCatId = questionnaireCatId + "," + product.ProductQuestionnaireCatId.ToString();
                }
                else
                {
                    questionnaireCatId = product.ProductQuestionnaireCatId.ToString();
                }

                multipleCat = true;
            }
            return View("CustomerUploadDoc", new PrescriptionUploadViewModel
            {
                OrderId = orderId,
                PrescriptionImageFound = idImageFound,
                IdImageName = idImageFileName,
                IdBackImageName = idBackImageName,
                CustomerDocumentList = customerDocumentList,
                QuestionnaireCatId = questionnaireCatId,
                backIdImageFound = backIdImageFound

            });
        }

        // load prescription to server folder
        [Authorize]
        [SessionExpire]
        public ActionResult LoadPrescription(LoggedCustomer loggedCustomer, AffiliateInfo affiliateInfo)
        {
            if (loggedCustomer.CustomerId == 0)
            {
                return RedirectToAction("Login", "Customer");
            }

            HttpPostedFileBase prescriptionImage = Request.Files[0];
            //string returnUrl = Request.Form["returnUrl"];
            int orderInvoiceId = int.Parse(Request.Form["orderInvoiceId"]);
            bool uploadFailed = false;

            var imagePrefix = @SiteConfigurationsWc.PrescriptionPrefix;

            // get extension of uploaded file and remove leading "."
            if (prescriptionImage != null && prescriptionImage.ContentLength > 0)
            {
                var extension = Path.GetExtension(prescriptionImage.FileName);
                if (extension != null)
                {
                    var fileExtension = extension.Replace(".", "");

                    var allowExtensionList = "jpeg,jpg,png,bmp".Split(',').ToList();
                    var isAllowedFound = allowExtensionList.FirstOrDefault(x => x.Contains(fileExtension.ToLower()));

                    if (isAllowedFound == null)
                    {
                        TempData["UploadPresMsgErr"] = "Invalid file type, please upload jpeg/jpg/png/bmp  only";
                        uploadFailed = true;
                        return Json(new { status = uploadFailed, message = TempData["UploadPresMsg"] == null ? TempData["UploadPresMsgErr"] : TempData["UploadPresMsg"], success = TempData["UploadPresMsgErr"] == null ? true : false });
                    }

                    try
                    {
                        //string newFileName = imagePrefix + "-" + orderInvoiceId + Path.GetExtension(checkImage.FileName);

                        string newFileName = imagePrefix + "-" + orderInvoiceId + "." + fileExtension;

                        // save with new name
                        string pathToLoad = Server.MapPath(SiteConfigurationsWc.PrescriptionUploadDir) + Path.GetFileName(newFileName);
                        prescriptionImage.SaveAs(pathToLoad);

                        TempData["UploadPresMsg"] = prescriptionImage.FileName + " Uploaded Successfully";

                        // save image details in database
                        var updateStatus = _customerRepository.AddCustomerPrescription(loggedCustomer.CustomerId, orderInvoiceId, affiliateInfo.AffiliateStoreFrontFk, newFileName, true, DateTime.Now);
                        bool idInactivated = _customerRepository.UpdatePrescriptionInActive(updateStatus, loggedCustomer.CustomerId, orderInvoiceId, affiliateInfo.AffiliateStoreFrontFk);

                    }
                    catch (Exception ex)
                    {
                        TempData["UploadPresMsgErr"] = "Error occured during File upload ";
                        uploadFailed = true;
                        // send email with error message

                    }
                }

                else
                {
                    TempData["UploadPresMsgErr"] = "Invalid file type, please upload jpeg/jpg/png/bmp only";
                    uploadFailed = true;
                }
            }

            else
            {
                TempData["UploadPresMsgErr"] = "You have not specified a file to upload";
                uploadFailed = true;
            }
            return Json(new { status = uploadFailed, message = TempData["UploadPresMsg"] == null ? TempData["UploadPresMsgErr"] : TempData["UploadPresMsg"], success = TempData["UploadPresMsgErr"] == null ? true : false });
        }

        // load prescription to server folder
        [Authorize]
        [SessionExpire]
        public ActionResult LoadPrescriptionS3(LoggedCustomer loggedCustomer, AffiliateInfo affiliateInfo)
        {
            if (loggedCustomer.CustomerId == 0)
            {
                return RedirectToAction("Login", "Customer");
            }

            HttpPostedFileBase prescriptionImage = Request.Files[0];

            //string returnUrl = Request.Form["returnUrl"];
            int orderInvoiceId = int.Parse(Request.Form["orderInvoiceId"]);
            bool uploadFailed = false;

            var imagePrefix = @SiteConfigurationsWc.UploadS3PrescriptionPrefix;

            // get extension of uploaded file and remove leading "."
            if (prescriptionImage != null && prescriptionImage.ContentLength > 0)
            {
                var extension = Path.GetExtension(prescriptionImage.FileName);
                if (extension != null)
                {
                    var fileExtension = extension.Replace(".", "");

                    var allowExtensionList = "jpeg,jpg,png,bmp".Split(',').ToList();
                    var isAllowedFound = allowExtensionList.FirstOrDefault(x => x.Contains(fileExtension.ToLower()));

                    if (isAllowedFound == null)
                    {
                        TempData["UploadPresMsgErr"] = "Invalid file type, please upload jpeg/jpg/png/bmp  only";
                        uploadFailed = true;
                        return Json(new { status = uploadFailed, message = TempData["UploadPresMsg"] == null ? TempData["UploadPresMsgErr"] : TempData["UploadPresMsg"], success = TempData["UploadPresMsgErr"] == null ? true : false });
                    }

                    AmazonS3Config s3Config = new AmazonS3Config { RegionEndpoint = Amazon.RegionEndpoint.GetBySystemName("ca-central-1"), SignatureMethod = SigningAlgorithm.HmacSHA256 };

                    AmazonS3Client s3Client = new AmazonS3Client(@SiteConfigurationsWc.UploadS3BucketKeyId.ToString(), @SiteConfigurationsWc.UploadS3BucketSecretKey.ToString(), s3Config);

                    var fileTransferUtility = new TransferUtility(s3Client);

                    try
                    {
                        //string newFileName = imagePrefix + "-" + orderInvoiceId + Path.GetExtension(checkImage.FileName);

                        string newFileName = imagePrefix + "-" + orderInvoiceId + "." + fileExtension;

                        var fileTransferUtilityRequest = new TransferUtilityUploadRequest
                        {
                            BucketName = @SiteConfigurationsWc.UploadS3BucketName + "/" + @SiteConfigurationsWc.UploadS3PrescriptionDir,

                            //FilePath = filePath,
                            InputStream = prescriptionImage.InputStream,
                            StorageClass = S3StorageClass.Standard,
                            //PartSize = 6291456, // 6 MB.  
                            Key = newFileName,
                            CannedACL = S3CannedACL.PublicRead
                        };

                        fileTransferUtility.Upload(fileTransferUtilityRequest);
                        fileTransferUtility.Dispose();


                        TempData["UploadPresMsg"] = prescriptionImage.FileName + " Uploaded Successfully";

                        // save image details in database
                        var updateStatus = _customerRepository.AddCustomerPrescription(loggedCustomer.CustomerId, orderInvoiceId, affiliateInfo.AffiliateStoreFrontFk, newFileName, true, DateTime.Now);
                        bool idInactivated = _customerRepository.UpdatePrescriptionInActive(updateStatus, loggedCustomer.CustomerId, orderInvoiceId, affiliateInfo.AffiliateStoreFrontFk);

                    }
                    catch (Exception ex)
                    {
                        TempData["UploadPresMsgErr"] = "Error occured during File upload ";
                        uploadFailed = true;
                        // send email with error message

                    }
                }

                else
                {
                    TempData["UploadPresMsgErr"] = "Invalid file type, please upload jpeg/jpg/png/bmp only";
                    uploadFailed = true;
                }
            }

            else
            {
                TempData["UploadPresMsgErr"] = "You have not specified a file to upload";
                uploadFailed = true;
            }
            return Json(new { status = uploadFailed, message = TempData["UploadPresMsg"] == null ? TempData["UploadPresMsgErr"] : TempData["UploadPresMsg"], success = TempData["UploadPresMsgErr"] == null ? true : false });
        }

        [Authorize]
        [SessionExpire]
        public ActionResult LoadId(LoggedCustomer loggedCustomer, AffiliateInfo affiliateInfo)
        {
            if (loggedCustomer.CustomerId == 0)
            {
                return RedirectToAction("Login", "Customer");
            }

            bool uploadFailed = false;
            HttpPostedFileBase files = Request.Files[0];

            var imagePrefix = @SiteConfigurationsWc.CustomerIdDocumentPrefix;

            // get extension of uploaded file and remove leading "."
            if (files != null && files.ContentLength > 0)
            {
                var extension = Path.GetExtension(files.FileName);
                if (extension != null)
                {
                    var fileExtension = extension.Replace(".", "");

                    //make list of extensions that should not be uploaded

                    var allowExtensionList = "jpeg,jpg,png,bmp".Split(',').ToList();
                    var isAllowedFound = allowExtensionList.FirstOrDefault(x => x.Contains(fileExtension.ToLower()));

                    if (isAllowedFound == null)
                    {
                        TempData["UploadMsgErr"] = "Invalid file type, please upload jpeg/jpg/png/bmp only";
                        uploadFailed = true;

                        return Json(new { status = uploadFailed, message = TempData["UploadMsg"] == null ? TempData["UploadMsgErr"] : TempData["UploadMsg"], success = TempData["UploadMsgErr"] == null ? true : false });
                    }


                    string newFileName = imagePrefix + "-" + loggedCustomer.CustomerId + "." + fileExtension;

                    try
                    {
                        string pathToLoad = Server.MapPath(SiteConfigurationsWc.CustomerIdUploadDir) + Path.GetFileName(newFileName);
                        files.SaveAs(pathToLoad);

                        TempData["UploadMsg"] = files.FileName + " Uploaded Successfully";

                        // save image details in database
                        var updateStatus = _customerRepository.AddCustomerIdDocumentDetail(loggedCustomer.CustomerId, affiliateInfo.AffiliateStoreFrontFk, newFileName, true, DateTime.Now, string.Empty);
                        bool idInactivated = _customerRepository.UpdateCustomerIdInActive(updateStatus, loggedCustomer.CustomerId, affiliateInfo.AffiliateStoreFrontFk);
                    }


                    catch (Exception ex)
                    {
                        TempData["UploadMsgErr"] = "Error occured during File upload. ";
                        uploadFailed = true;
                        // send email with error message

                    }
                }

                else
                {
                    TempData["UploadMsgErr"] = "Invalid file type, please upload jpeg/jpg/png/bmp only";
                    uploadFailed = true;
                }
            }

            else
            {
                TempData["UploadMsgErr"] = "You have not specified a file to upload";
                uploadFailed = true;
            }

            return Json(new { status = uploadFailed, message = TempData["UploadMsg"] == null ? TempData["UploadMsgErr"] : TempData["UploadMsg"], success = TempData["UploadMsgErr"] == null ? true : false });
        }


        public ActionResult LoadIdS3(LoggedCustomer loggedCustomer, AffiliateInfo affiliateInfo)
        {
            if (loggedCustomer.CustomerId == 0)
            {
                return RedirectToAction("logout", "Customer");
            }

            bool uploadFailed = false;
            var mobileDeviceParam = Request.Params["mobileDevice"];
            bool mobileDevice = mobileDeviceParam == "0" ? false : true;
            string module = Request.Params["module"];

            if (Request.Files.Count > 0)
            {

                HttpPostedFileBase files = Request.Files[0];

                var imagePrefix = @SiteConfigurationsWc.UploadS3CustomerIdDocumentPrefix;


                // get extension of uploaded file and remove leading "."
                if (files != null && files.ContentLength > 0)
                {
                    var extension = Path.GetExtension(files.FileName);
                    if (extension != null)
                    {
                        var fileExtension = extension.Replace(".", "");

                        //make list of extensions that should not be uploaded

                        var allowExtensionList = "jpeg,jpg,png,bmp".Split(',').ToList();
                        var isAllowedFound = allowExtensionList.FirstOrDefault(x => x.Contains(fileExtension.ToLower()));

                        if (isAllowedFound == null)
                        {
                            TempData["UploadMsgErr"] = "Invalid file type, please upload jpeg/jpg/png/bmp only";
                            uploadFailed = true;

                            //Add log 
                            var logUpdateStatus = _customerRepository.AddLogCustomerIdDocument(0,
                                                            loggedCustomer.CustomerId, affiliateInfo.AffiliateStoreFrontFk,
                                                            "Front image Upload", DateTime.Now, mobileDevice, module, "Error - (Invalid File format). ");

                            return Json(new { status = uploadFailed, message = TempData["UploadMsg"] == null ? TempData["UploadMsgErr"] : TempData["UploadMsg"], success = TempData["UploadMsgErr"] == null ? true : false });
                        }

                        string newFileName = imagePrefix + "-" + loggedCustomer.CustomerId + "." + fileExtension;

                        AmazonS3Config s3Config = new AmazonS3Config { RegionEndpoint = Amazon.RegionEndpoint.GetBySystemName("ca-central-1"), SignatureMethod = SigningAlgorithm.HmacSHA256 };

                        AmazonS3Client s3Client = new AmazonS3Client(@SiteConfigurationsWc.UploadS3BucketKeyId.ToString(), @SiteConfigurationsWc.UploadS3BucketSecretKey.ToString(), s3Config);

                        var fileTransferUtility = new TransferUtility(s3Client);
                        try
                        {

                            if (files.ContentLength > 0)
                            {
                                var fileTransferUtilityRequest = new TransferUtilityUploadRequest
                                {
                                    BucketName = @SiteConfigurationsWc.UploadS3BucketName + "/" + @SiteConfigurationsWc.UploadS3CustomerIdDir,
                                    //ServerSideEncryptionKeyManagementServiceKeyId = "arn:aws:kms:ca-central-1:196676168700:key/3baf6179-9183-4181-9b84-d288045cf0e3",
                                    ServerSideEncryptionMethod = ServerSideEncryptionMethod.AWSKMS,

                                    //FilePath = filePath,
                                    InputStream = files.InputStream,
                                    StorageClass = S3StorageClass.Standard,
                                    //PartSize = 6291456, // 6 MB.  
                                    Key = newFileName,
                                    CannedACL = S3CannedACL.BucketOwnerFullControl,

                                };

                                //fileTransferUtilityRequest.Metadata.Add("param1", "Value1");  
                                //fileTransferUtilityRequest.Metadata.Add("param2", "Value2");  
                                try
                                {
                                    fileTransferUtility.Upload(fileTransferUtilityRequest);
                                }
                                catch (Exception e)
                                {

                                    throw e;
                                }

                                fileTransferUtility.Dispose();
                            }

                            // get existing document, so if user replace only front image, back image is retained
                            List<CustomerIdDocument> customerDocumentList = _customerRepository.GetCustomerIdDocumentByCustomerId(loggedCustomer.CustomerId);
                            var previousBackImage = string.Empty;
                            if (customerDocumentList != null && customerDocumentList.Count > 0)
                            {
                                previousBackImage = customerDocumentList[0].CustomerIdDocumentBackSideFileName;
                            }


                            // save image details in database
                            var docId = _customerRepository.AddCustomerIdDocumentDetail(loggedCustomer.CustomerId,
                                affiliateInfo.AffiliateStoreFrontFk,
                                newFileName,
                                true,
                                DateTime.Now, previousBackImage);
                            //Add log 
                            var logUpdateStatus = _customerRepository.AddLogCustomerIdDocument(docId,
                                                            loggedCustomer.CustomerId, affiliateInfo.AffiliateStoreFrontFk,
                                                            "Front Image Upload", DateTime.Now, mobileDevice, module, "success");

                            if (docId > 0)
                            {
                                bool idInactivated = _customerRepository.UpdateCustomerIdInActive(docId,
                                    loggedCustomer.CustomerId,
                                    affiliateInfo.AffiliateStoreFrontFk);
                            }

                            TempData["UploadMsg"] = "Id Uploaded Successfully";


                        }


                        catch (Exception ex)
                        {
                            TempData["UploadMsg"] = "Error occured during File upload ";

                            //Add log 
                            var logUpdateStatus = _customerRepository.AddLogCustomerIdDocument(0,
                                                            loggedCustomer.CustomerId, affiliateInfo.AffiliateStoreFrontFk,
                                                            "Front Image Upload ", DateTime.Now, mobileDevice, module, "Error -" + ex.Message);
                            // send email with error message

                        }

                    }

                    else
                    {
                        TempData["UploadMsgErr"] = "Invalid file type, please upload jpeg/jpg/png/bmp only";
                        uploadFailed = true;

                        //Add log 
                        var logUpdateStatus = _customerRepository.AddLogCustomerIdDocument(0,
                                                        loggedCustomer.CustomerId, affiliateInfo.AffiliateStoreFrontFk,
                                                        "Front Image Upload", DateTime.Now, mobileDevice, module, "Error - (Invalid File format)");
                    }
                }

                else
                {
                    TempData["UploadMsgErr"] = "You have not specified a file to upload";
                    uploadFailed = true;

                    //Add log 
                    var logUpdateStatus = _customerRepository.AddLogCustomerIdDocument(0,
                                                    loggedCustomer.CustomerId, affiliateInfo.AffiliateStoreFrontFk,
                                                    "Front Image Upload ", DateTime.Now, mobileDevice, module, "Error - (No File Selected)");
                }
            }
            else
            {
                TempData["UploadMsgErr"] = "You have not specified a file to upload";
                uploadFailed = true;

                //Add log 
                var logUpdateStatus = _customerRepository.AddLogCustomerIdDocument(0,
                                                loggedCustomer.CustomerId, affiliateInfo.AffiliateStoreFrontFk,
                                                "Front Image Upload ", DateTime.Now, mobileDevice, module, "Error - (No File Selected)");
            }

            return Json(new { status = uploadFailed, message = TempData["UploadMsg"] == null ? TempData["UploadMsgErr"] : TempData["UploadMsg"], success = TempData["UploadMsgErr"] == null ? true : false });
        }


        public ActionResult LoadIdS3BackImage(LoggedCustomer loggedCustomer, AffiliateInfo affiliateInfo)
        {
            if (loggedCustomer.CustomerId == 0)
            {
                return RedirectToAction("Logout", "Customer");
            }

            bool uploadFailed = false;

            // get mobiledevice(true/false) and module from request parameters
            var mobileDeviceParam = Request.Params["mobileDevice"];
            bool mobileDevice = mobileDeviceParam == "0" ? false : true;
            string module = Request.Params["module"];

            if (Request.Files.Count > 0)
            {
                HttpPostedFileBase files = Request.Files[0];

                var imagePrefix = @SiteConfigurationsWc.UploadS3CustomerIdDocumentPrefix + "-back";

                // get extension of uploaded file and remove leading "."
                if (files != null && files.ContentLength > 0)
                {
                    //convert bytes to KB - limit file size to 4 Mb
                    if (files.ContentLength / 1024 < SiteConfigurationsWc.FileUploadSizeLimit)
                    {
                        var extension = Path.GetExtension(files.FileName);
                        if (extension != null)
                        {
                            var fileExtension = extension.Replace(".", "");

                            //make list of extensions that should not be uploaded

                            var allowExtensionList = "jpeg,jpg,png,bmp".Split(',').ToList();
                            var isAllowedFound = allowExtensionList.FirstOrDefault(x => x.Contains(fileExtension.ToLower()));

                            if (isAllowedFound == null)
                            {
                                TempData["UploadMsgErr"] = "Invalid file type, please upload jpeg or jpg or png or bmp only";
                                uploadFailed = true;

                                return Json(new { status = uploadFailed, message = TempData["UploadMsg"] == null ? TempData["UploadMsgErr"] : TempData["UploadMsg"], success = TempData["UploadMsgErr"] == null ? true : false });
                            }


                            string newFileName = imagePrefix + "-" + loggedCustomer.CustomerId + "." + fileExtension;

                            AmazonS3Config s3Config = new AmazonS3Config { RegionEndpoint = Amazon.RegionEndpoint.GetBySystemName("ca-central-1"), SignatureMethod = SigningAlgorithm.HmacSHA256 };

                            AmazonS3Client s3Client = new AmazonS3Client(@SiteConfigurationsWc.UploadS3BucketKeyId.ToString(), @SiteConfigurationsWc.UploadS3BucketSecretKey.ToString(), s3Config);

                            var fileTransferUtility = new TransferUtility(s3Client);
                            try
                            {

                                if (files.ContentLength > 0)
                                {
                                    var fileTransferUtilityRequest = new TransferUtilityUploadRequest
                                    {
                                        BucketName = @SiteConfigurationsWc.UploadS3BucketName + "/" + @SiteConfigurationsWc.UploadS3CustomerIdDir,

                                        //FilePath = filePath,
                                        InputStream = files.InputStream,
                                        StorageClass = S3StorageClass.Standard,
                                        //PartSize = 6291456, // 6 MB.  
                                        Key = newFileName,
                                        CannedACL = S3CannedACL.PublicRead
                                    };

                                    //fileTransferUtilityRequest.Metadata.Add("param1", "Value1");  
                                    //fileTransferUtilityRequest.Metadata.Add("param2", "Value2");  

                                    fileTransferUtility.Upload(fileTransferUtilityRequest);
                                    fileTransferUtility.Dispose();
                                }
                                // to do check if front image is uploaded for customer
                                // if upload then update, else allow customer to upload back image (this scenario will come in mobile)

                                // get existing document, so if user replace only front image, back image is retained
                                List<CustomerIdDocument> customerDocumentList = _customerRepository.GetCustomerIdDocumentByCustomerId(loggedCustomer.CustomerId);

                                var customerIdDocumentFk = 0;
                                if (customerDocumentList != null && customerDocumentList.Count > 0)
                                {
                                    var activeCustomerDocument = customerDocumentList.Where(x => x.CustomerIdDocumentActive).FirstOrDefault();
                                    // update back image details in database
                                    if (activeCustomerDocument != null)
                                    {
                                        customerIdDocumentFk = _customerRepository.UpdateCustomerIdBackFileName(activeCustomerDocument.CustomerIdDocumentId,
                                            newFileName, loggedCustomer.CustomerId,
                                        affiliateInfo.AffiliateStoreFrontFk, DateTime.Now);
                                    }
                                    else
                                    {
                                        customerIdDocumentFk = _customerRepository.AddCustomerIdDocumentDetail(loggedCustomer.CustomerId,
                                       affiliateInfo.AffiliateStoreFrontFk,
                                       string.Empty,
                                       true,
                                       DateTime.Now, newFileName);
                                    }

                                }

                                else
                                {
                                    // save image details in database
                                    customerIdDocumentFk = _customerRepository.AddCustomerIdDocumentDetail(loggedCustomer.CustomerId,
                                        affiliateInfo.AffiliateStoreFrontFk,
                                        string.Empty,
                                        true,
                                        DateTime.Now, newFileName);

                                    //if (customerIdDocumentFk > 0)
                                    //{
                                    //    bool idInactivated = _customerRepository.UpdateCustomerIdInActive(customerIdDocumentFk,
                                    //        loggedCustomer.CustomerId,
                                    //        affiliateInfo.AffiliateStoreFrontFk);
                                    //}
                                }



                                //Add log 
                                var logUpdateStatus = _customerRepository.AddLogCustomerIdDocument(customerIdDocumentFk,
                                                                loggedCustomer.CustomerId, affiliateInfo.AffiliateStoreFrontFk,
                                                                "Back Image Uploaded", DateTime.Now, mobileDevice, module, "success");

                                //bool idInactivated = _customerRepository.UpdateCustomerIdInActive(updateStatus,
                                //    loggedCustomer.CustomerId,
                                //    affiliateInfo.AffiliateStoreFrontFk);

                                TempData["UploadMsg"] = "Id Uploaded Successfully";


                            }


                            catch (Exception ex)
                            {
                                //Add log 
                                var logUpdateStatus = _customerRepository.AddLogCustomerIdDocument(0,
                                                                loggedCustomer.CustomerId, affiliateInfo.AffiliateStoreFrontFk,
                                                                "Back Image Upload", DateTime.Now, mobileDevice, module, "Failed - " + ex.Message);

                                TempData["UploadMsg"] = "Error occured during File upload ";
                                // send email with error message

                            }

                        }

                        else
                        {
                            TempData["UploadMsgErr"] = "Invalid file type, please upload jpeg or jpg or png or bmp only";
                            uploadFailed = true;
                            //Add log 
                            var logUpdateStatus = _customerRepository.AddLogCustomerIdDocument(0,
                                                            loggedCustomer.CustomerId, affiliateInfo.AffiliateStoreFrontFk,
                                                            "Back Image Upload", DateTime.Now, mobileDevice, module, "Failed - (Invalid file format)");

                        }
                    }
                    else
                    {
                        TempData["UploadMsgErr"] = "Please upload file size less than " + SiteConfigurationsWc.FileUploadSizeLimit + " KB";
                        uploadFailed = true;
                        var logUpdateStatus = _customerRepository.AddLogCustomerIdDocument(0,
                                                           loggedCustomer.CustomerId, affiliateInfo.AffiliateStoreFrontFk,
                                                           "Back Image Upload", DateTime.Now, mobileDevice, module, "Failed - (file Size exceeded limit" + SiteConfigurationsWc.FileUploadSizeLimit + " KB" + ")");
                    }
                }
                else
                {
                    TempData["UploadMsgErr"] = "You have not specified a file to upload";
                    uploadFailed = true;
                    var logUpdateStatus = _customerRepository.AddLogCustomerIdDocument(0,
                                                       loggedCustomer.CustomerId, affiliateInfo.AffiliateStoreFrontFk,
                                                       "Back Image Upload", DateTime.Now, mobileDevice, module, "Failed - (No file selected)");

                }

            }
            else
            {
                TempData["UploadMsgErr"] = "You have not specified a file to upload";
                uploadFailed = true;
                var logUpdateStatus = _customerRepository.AddLogCustomerIdDocument(0,
                                                   loggedCustomer.CustomerId, affiliateInfo.AffiliateStoreFrontFk,
                                                   "Back Image Uploaded", DateTime.Now, mobileDevice, module, "Failed - (No file selected)");
            }
            return Json(new { status = uploadFailed, message = TempData["UploadMsg"] == null ? TempData["UploadMsgErr"] : TempData["UploadMsg"], success = TempData["UploadMsgErr"] == null ? true : false });
        }


        [ActionName("upload-document-success")]
        public ActionResult UploadDocumentSuccess()
        {
            @ViewBag.Title = "Document Upload Complete " + SiteConfigurationsWc.StorefrontUrl;
            @ViewBag.MetaRobot = "noindex,follow";
            return View("upload-document-success");
        }

        //Added for prescription Upload

        [Authorize]
        [SessionExpire]
        public ActionResult RequirePrescription(int orderId, LoggedCustomer loggedCustomer)
        {
            if (loggedCustomer.CustomerId == 0)
            {
                return RedirectToAction("Logout", "Customer");
            }

            @ViewBag.Title = "Health Questionnaire | " + SiteConfigurationsWc.StorefrontName;

            // to do code optimization for list<int> and string[] conversions
            // get Questionnaire category based on order id
            List<int> questionnaireCategoryList = _orderRepository.GetProductQuestionnaireCategoryByOrderId(orderId);
            string questionnaireCatId = string.Join(",", questionnaireCategoryList);
            List<QuestionOptionSelected> alreadyAnsweredQuestionnaireList = new List<QuestionOptionSelected>();
            if ((questionnaireCatId != "0") && !String.IsNullOrEmpty(questionnaireCatId))
            {
                string[] questionnaireCatIdArr = questionnaireCatId.Split(',');
                foreach (string questionnaireCatIdArrItem in questionnaireCatIdArr)
                {
                    int catId = Convert.ToInt32(questionnaireCatIdArrItem);

                    bool checkIfUserAnsweredQuestion = _orderRepository.IfUserAlreadyAnsweredQuestionnaire(catId, loggedCustomer.CustomerId, DateTime.Now, SiteConfigurationsWc.QuestionnaireTimeSpan, 0);
                    if (checkIfUserAnsweredQuestion)
                    {
                        alreadyAnsweredQuestionnaireList.AddRange(_orderRepository.GetAnsweredQuestionnaireForCustomer(catId, loggedCustomer.CustomerId, DateTime.Now, SiteConfigurationsWc.QuestionnaireTimeSpan, 0));
                        List<string> productNameList = alreadyAnsweredQuestionnaireList.Select(o => o.ProductName).Distinct().ToList();
                        TempData["ProductNamesList"] = productNameList;
                        TempData["list"] = alreadyAnsweredQuestionnaireList;
                        questionnaireCatIdArr = questionnaireCatIdArr.Where(s => s != questionnaireCatIdArrItem).ToArray();
                    }
                }
                if (questionnaireCatIdArr.Count() > 0)
                {
                    questionnaireCatId = string.Join(",", questionnaireCatIdArr);
                    List<Questionnaire> questionnaireList = new List<Questionnaire>();
                    questionnaireList = _orderRepository.GetQuestionnaireByCategoryId(questionnaireCatId, "Questionnaire_AskOrder Asc");
                    List<QuestionnaireOption> questionnaireOptionList = _orderRepository.GetQuestionOptionByQuestionnaireId(questionnaireCatId, "QuestionnaireOption_Order Asc");
                    List<QuestionnaireGroup> questionnaireGroupList = _orderRepository.GetQuestionnaireGroupByCatId(questionnaireCatIdArr[0], "QuestionnaireGroup_Order Asc");

                    List<CustomerIdDocument> customerDocumentList = _customerRepository.GetCustomerIdDocumentByCustomerId(loggedCustomer.CustomerId);
                    if (customerDocumentList != null)
                    {
                        if (!string.IsNullOrEmpty(customerDocumentList[0].CustomerIdDocumentBackSideFileName))
                            @ViewBag.backIdImageFound = true;
                        else
                            @ViewBag.backIdImageFound = false;

                        if (!string.IsNullOrEmpty(customerDocumentList[0].CustomerIdDocumentFileName))
                            @ViewBag.idImageFound = true;
                        else
                            ViewBag.idImageFound = false;

                        @ViewBag.idImageFileName = customerDocumentList[0].CustomerIdDocumentFileName;
                        @ViewBag.IdBackImageName = customerDocumentList[0].CustomerIdDocumentBackSideFileName;

                        @ViewBag.CustomerDocumentValidated = customerDocumentList[0].CustomerIdDocumentIsValid == true ? true : false;
                    }
                    else { @ViewBag.backIdImageFound = false; @ViewBag.idImageFound = false; @ViewBag.idImageFileName = ""; @ViewBag.IdBackImageName = ""; @ViewBag.CustomerDocumentValidated = false; }
                    // return View("RequirePrescription");
                    if (questionnaireList != null)
                    {
                        // add customerSessionTracker record 

                        var customerSessionTrackerId = _orderRepository.AddCustomerSessionTracker(loggedCustomer.CustomerId, orderId, "RequirePrescription",
                            "Start-Answer Questionnaire", 0, DateTime.Now);

                        return View("RequirePrescription", new RequirePrescriptionViewModel
                        {
                            QuestionnaireList = questionnaireList,
                            QuestionnaireOptionList = questionnaireOptionList,
                            QuestionnaireGroupList = questionnaireGroupList,
                            QuestionnaireCatId = questionnaireCatId,
                            OrderId = orderId,
                            CustomerSessionTrackerFk = customerSessionTrackerId

                        });
                    }
                    else
                    {
                        return View("NoQuestionnaire");
                    }
                }
                else
                {
                    return RedirectToAction("ReviewAnswerGet", new { orderId = orderId });
                    //return View("AlreadyAnswered");
                }

            }
            else
            {
                return View("NoQuestionnaire");
            }
        }

        [HttpPost]
        public ActionResult ReviewAnswer(LoggedCustomer loggedCustomer, List<QuestionOptionSelected> answerList, int orderId, int sessionTrackerId = 0, int deviceType = 0)
        {
            /* deviceType 0-desktop 1-mobile */

            if (loggedCustomer.CustomerId == 0)
            {
                var updateSessionStatus = _orderRepository.UpdateCustomerSessionTracker(sessionTrackerId, "Error- Session Expired",
                                                2, DateTime.Now);

                if (SiteConfigurationsWc.CustomerQuestionnaireIssuesFlag)
                {
                    _orderRepository.AddCustomerQuestionnaireIssues(loggedCustomer.CustomerId, orderId, 0, deviceType, "Error- Session Expired", false, DateTime.Now);
                }

                return RedirectToAction("Login", "Customer");
            }

            @ViewBag.Title = "Questionnaire Review | " + SiteConfigurationsWc.StorefrontUrl;

            try
            {
                // restoring info in tempdata list
                if (TempData["list"] != null)
                {
                    var previouslyAnsweredList = TempData["list"] as List<QuestionOptionSelected>;
                    answerList.AddRange(previouslyAnsweredList);
                    //answeredCategoriesList = previouslyAnsweredList.Select(o => o.QuestionnaireCategoryId).Distinct().ToList();
                    // productNamesListForAnswredQuestionnaire = TempData["ProductNamesList"] as List<string>;
                }

                // update tempdate again to handle refreshes/reloads
                TempData["list"] = answerList;
            }
            catch (Exception ex)
            {
                if (SiteConfigurationsWc.CustomerQuestionnaireIssuesFlag)
                {
                    string errorMessage = string.Empty;

                    if (ex.Message.Contains('\''))
                    {
                        errorMessage = errorMessage.Replace("'", "''");
                    }
                    _orderRepository.AddCustomerQuestionnaireIssues(loggedCustomer.CustomerId, orderId, 0, deviceType, errorMessage, false, DateTime.Now);
                }
            }

            // redirect to get action, so there is no issue when page reloads
            return RedirectToAction("RequirePrescriptionReviewAnswers", new { orderId = orderId, sessionTrackerId = sessionTrackerId, deviceType = deviceType });


        }

        public ActionResult RequirePrescriptionReviewAnswers(LoggedCustomer loggedCustomer, int orderId, int sessionTrackerId, int deviceType)
        {
            try
            {
                List<int> catgoryArray = _orderRepository.GetProductQuestionnaireCategoryByOrderId(orderId);

                string questionnaireCatId = null;
                bool multpleCat = false;
                foreach (int catId in catgoryArray)
                {
                    if (multpleCat)
                    {
                        questionnaireCatId = questionnaireCatId + "," + catId.ToString();
                    }
                    else
                    {
                        questionnaireCatId = catId.ToString();
                    }

                    multpleCat = true;
                }

                List<Questionnaire> questionnaireList = _orderRepository.GetQuestionnaireByCategoryId(questionnaireCatId, "Questionnaire_AskOrder Asc");
                List<QuestionnaireOption> questionnaireOptionList = _orderRepository.GetQuestionOptionByQuestionnaireId(questionnaireCatId, "QuestionnaireOption_Order Asc");
                List<QuestionnaireGroup> questionnaireGroupList = _orderRepository.GetQuestionnaireGroupByCatId(questionnaireCatId, "QuestionnaireGroup_Order Asc");
                List<int> answeredCategoriesList = new List<int>();
                List<string> productNamesListForAnswredQuestionnaire = new List<string>();

                // 
                List<QuestionOptionSelected> answerList = new List<QuestionOptionSelected>();
                if (TempData["list"] != null)
                {
                    answerList = TempData["list"] as List<QuestionOptionSelected>;

                    answeredCategoriesList = answerList.Select(o => o.QuestionnaireCategoryId).Distinct().ToList();
                    productNamesListForAnswredQuestionnaire = answerList.Select(o => o.ProductName).Distinct().ToList();
                }
                TempData["list"] = answerList;

                return View("RequirePrescriptionReviewAnswers", new RequirePrescriptionReviewViewModel
                {
                    QuestionnaireList = questionnaireList,
                    QuestionnaireOptionList = questionnaireOptionList,
                    AnswerList = answerList,
                    OrderId = orderId,
                    QuestionnaireGroupList = questionnaireGroupList,
                    QuestionnaireCatId = questionnaireCatId,
                    AnsweredCategoriesList = answeredCategoriesList,
                    ProductNameList = productNamesListForAnswredQuestionnaire,
                    CustomerSessionTrackerFk = sessionTrackerId

                });
            }
            catch (Exception ex)
            {
                string errorMessage = string.Empty;

                if (ex.Message.Contains('\''))
                {
                    errorMessage = errorMessage.Replace("'", "''");
                }

                if (SiteConfigurationsWc.CustomerQuestionnaireIssuesFlag)
                {
                    _orderRepository.AddCustomerQuestionnaireIssues(loggedCustomer.CustomerId, orderId, 0, deviceType, errorMessage, false, DateTime.Now);
                }
            }

            return View("RequirePrescriptionReviewAnswers", new RequirePrescriptionReviewViewModel
            {
                QuestionnaireList = null,
                QuestionnaireOptionList = null,
                AnswerList = null,
                OrderId = orderId,
                QuestionnaireGroupList = null,
                QuestionnaireCatId = null,
                AnsweredCategoriesList = null,
                ProductNameList = null,
                CustomerSessionTrackerFk = sessionTrackerId

            });
        }

        [HttpGet]
        public ActionResult ReviewAnswerGet(LoggedCustomer loggedCustomer, int orderId)
        {
            var customerSessionTrackerId = 0;
            if (loggedCustomer.CustomerId == 0)
            {
                customerSessionTrackerId = _orderRepository.AddCustomerSessionTracker(loggedCustomer.CustomerId, orderId, "ReviewAnswerGet",
                          "Error- Session Expired", 2, DateTime.Now);

                return RedirectToAction("Login", "Customer");
            }

            customerSessionTrackerId = _orderRepository.AddCustomerSessionTracker(loggedCustomer.CustomerId, orderId, "ReviewAnswerGet",
                            "Start- Answer Questionnaire", 0, DateTime.Now);

            @ViewBag.Title = "Questionnaire Review | " + SiteConfigurationsWc.StorefrontUrl;

            List<int> questionnaireCategoryList = _orderRepository.GetProductQuestionnaireCategoryByOrderId(orderId);
            string questionnaireCatId = string.Join(",", questionnaireCategoryList);
            List<QuestionOptionSelected> alreadyAnsweredQuestionnaireList = new List<QuestionOptionSelected>();
            List<string> productNameList = new List<string>();

            if ((questionnaireCatId != "0") && !String.IsNullOrEmpty(questionnaireCatId))
            {
                string[] questionnaireCatIdArr = questionnaireCatId.Split(',');
                foreach (string questionnaireCatIdArrItem in questionnaireCatIdArr)
                {
                    int catId = Convert.ToInt32(questionnaireCatIdArrItem);
                    bool checkIfUserAnsweredQuestion = _orderRepository.IfUserAlreadyAnsweredQuestionnaire(catId, loggedCustomer.CustomerId, DateTime.Now, SiteConfigurationsWc.QuestionnaireTimeSpan, 0);
                    if (checkIfUserAnsweredQuestion)
                    {
                        alreadyAnsweredQuestionnaireList.AddRange(_orderRepository.GetAnsweredQuestionnaireForCustomer(catId, loggedCustomer.CustomerId, DateTime.Now, SiteConfigurationsWc.QuestionnaireTimeSpan, 0));
                        productNameList = alreadyAnsweredQuestionnaireList.Select(o => o.ProductName).Distinct().ToList();
                        //TempData["ProductNamesList"] = productNameList;
                        //TempData["list"] = alreadyAnsweredQuestionnaireList;
                        questionnaireCatIdArr = questionnaireCatIdArr.Where(s => s != questionnaireCatIdArrItem).ToArray();
                    }
                }

            }

            var answerList = alreadyAnsweredQuestionnaireList; //TempData["list"] as List<QuestionOptionSelected>;
            var productNamesListForAnswredQuestionnaire = productNameList;// TempData["ProductNamesList"] as List<string>;

            List<int> catgoryArray = answerList.Select(o => o.QuestionnaireCategoryId).Distinct().ToList();
            // string questionnaireCatId = null;
            bool multpleCat = false;
            foreach (int catId in catgoryArray)
            {
                if (multpleCat)
                {
                    questionnaireCatId = questionnaireCatId + "," + catId.ToString();
                }
                else
                {
                    questionnaireCatId = catId.ToString();
                }

                multpleCat = true;
            }
            List<Questionnaire> questionnaireList = _orderRepository.GetQuestionnaireByCategoryId(questionnaireCatId, "Questionnaire_AskOrder Asc");
            List<QuestionnaireOption> questionnaireOptionList = _orderRepository.GetQuestionOptionByQuestionnaireId(questionnaireCatId, "QuestionnaireOption_Order Asc");
            List<QuestionnaireGroup> questionnaireGroupList = _orderRepository.GetQuestionnaireGroupByCatId(questionnaireCatId, "QuestionnaireGroup_Order Asc");

            return View("RequirePrescriptionReviewAnswers", new RequirePrescriptionReviewViewModel
            {
                QuestionnaireList = questionnaireList,
                QuestionnaireOptionList = questionnaireOptionList,
                AnswerList = answerList,
                OrderId = orderId,
                QuestionnaireGroupList = questionnaireGroupList,
                QuestionnaireCatId = questionnaireCatId,
                AnsweredCategoriesList = catgoryArray,
                ProductNameList = productNamesListForAnswredQuestionnaire,
                CustomerSessionTrackerFk = customerSessionTrackerId

            });
        }


        [HttpPost]
        public ActionResult AddQuestionnaireResponses(LoggedCustomer loggedCustomer, List<QuestionOptionSelected> answerList, int orderId, int sessionTrackerId = 0, int deviceType = 0)
        {

            if (loggedCustomer.CustomerId == 0)
            {
                // update session expired details in database

                var updateSessionStatus = _orderRepository.UpdateCustomerSessionTracker(sessionTrackerId, "Error- Session Expired",
                                                2, DateTime.Now);


                if (SiteConfigurationsWc.CustomerQuestionnaireIssuesFlag)
                {
                    _orderRepository.AddCustomerQuestionnaireIssues(loggedCustomer.CustomerId, orderId, 0, deviceType, "Error- Session Expired", false, DateTime.Now);
                }

                return RedirectToAction("Login", "Customer");
            }

            try
            {
                bool bpNotKnownStatus = false;
                int questionnaireProblemId = 0;
                int questionnaireCategoryId = 0;

                ViewBag.BPNotKnowStatus = "false";

                DateTime utcTime = DateTime.UtcNow;

                foreach (var li in answerList)
                {
                    if (li.QuestionnaireOptionId == 0)
                    {
                        int questionResponse = _orderRepository.AddQuestionnaireResponse(loggedCustomer.CustomerId, orderId, li.QuestionnaireId
                            , li.QuestionnaireCategoryId, li.AnswerText, " ", li.QuestionnaireOptionId, utcTime);
                    }
                    else if (li.isSelected)
                    {
                        var explanationText = "";
                        //if (li.AskMoreInfoMulOption == 1)
                        //{
                        //    if (answerList.Where(x => (x.QuestionnaireId == li.QuestionnaireId && x.ExplanationText != null)).ToList().Count()>0)
                        //    {
                        //        explanationText = answerList.FirstOrDefault(x => (x.QuestionnaireId == li.QuestionnaireId && x.ExplanationText != null)).ExplanationText;
                        //    }

                        //}
                        //else
                        //{
                        explanationText = li.ExplanationText;
                        //}

                        /* BP questionnaire flag in orderinvoice table*/
                        if (li.QuestionnaireId == 35 && li.QuestionnaireOptionId == 58)
                        {
                            questionnaireProblemId = 6;
                            bpNotKnownStatus = true;
                            questionnaireCategoryId = li.QuestionnaireCategoryId;
                        }

                        int questionResponse = _orderRepository.AddQuestionnaireResponse(loggedCustomer.CustomerId, orderId, li.QuestionnaireId
                           , li.QuestionnaireCategoryId, "NULL", explanationText, li.QuestionnaireOptionId, utcTime);
                    }
                }
                bool hardStop = answerList.Any(p => p.HardStop == true && p.isSelected == true);
                var catIdList = answerList.Select(x => x.QuestionnaireCategoryId).Distinct();
                foreach (var catId in catIdList)
                {

                    int addResponseTable = _orderRepository.AddCustomerQuestionnaireResponse(loggedCustomer.CustomerId, orderId, catId, 0, utcTime, utcTime, hardStop);

                    if (bpNotKnownStatus && SiteConfigurationsWc.IsBPKnownLogic == 1)
                    {
                        _orderRepository.UpdateCustomerQuestionnaireCategoryResponse(addResponseTable, loggedCustomer.CustomerId, questionnaireProblemId, null, utcTime, null);

                        int problemFk = SiteConfigurationsWc.BPNotKnownProblemId;  /* BP Not Known */
                        string comment = "BP response not provided";
                        string ipAddress = Utility.GetVisitorIpAddress();
                        _orderRepository.AddLogPreScreenOrderBPUpdate(loggedCustomer.CustomerId, orderId, addResponseTable, comment, ipAddress, utcTime);

                        ViewBag.BPNotKnowStatus = "true";
                    }
                }

                if (SiteConfigurationsWc.IsBPKnownLogic == 1)
                {
                    /* BP questionnaire logic*/
                    _orderRepository.UpdateOrderInvoiceBPStatus(orderId, bpNotKnownStatus);

                    if (bpNotKnownStatus)
                    {
                        /* add email queue request*/
                        string subject = "Pending BP questionnaire response";
                        int sourceApplication = 1;
                        int emailType = 12;

                        var templateService = new TemplateService();
                        var emailHtmlBody = "";

                        string templateFilePath = System.Web.HttpContext.Current.Server.MapPath(SiteConfigurationsWc.EmailTemplatePath + "BPQuestionnaireEmail.cshtml");

                        if (!string.IsNullOrEmpty(templateFilePath))
                        {
                            var viewBag = new DynamicViewBag();
                            viewBag.AddValue("UserName", loggedCustomer.CustomerFirstName + " " + loggedCustomer.CustomerLastName);
                            viewBag.AddValue("orderId", orderId);
                            viewBag.AddValue("custId", loggedCustomer.CustomerId);

                            emailHtmlBody = templateService.Parse(System.IO.File.ReadAllText(templateFilePath), null, viewBag, null);
                        }

                        _orderRepository.AddEmailRecordForProcessing(Convert.ToInt32(SiteConfigurationsWc.StorefrontId), sourceApplication, loggedCustomer.CustomerEmail,
                            subject, emailHtmlBody, orderId, loggedCustomer.CustomerId, 0, emailType, SiteConfigurationsWc.Interval, SiteConfigurationsWc.MaxEmailSentCount);
                    }
                }


                if (SiteConfigurationsWc.TrackCustomerQuestionnaireResponse == 1)
                {
                    _orderRepository.UpdateCustomerQuestionnaireResponse(loggedCustomer.CustomerId, orderId, 0, 0, true, "status", DateTime.Now);
                }

                // update CustomerSessionTrackerStatus
                var updateSessionStatusSuccess = _orderRepository.UpdateCustomerSessionTracker(sessionTrackerId, "Complete- Answer Questionnaire",
                                                    1, DateTime.Now);

            }
            catch (Exception ex)
            {
                if (SiteConfigurationsWc.CustomerQuestionnaireIssuesFlag)
                {
                    string errorMessage = string.Empty;

                    if (ex.Message.Contains('\''))
                    {
                        errorMessage = errorMessage.Replace("'", "''");
                    }

                    _orderRepository.AddCustomerQuestionnaireIssues(loggedCustomer.CustomerId, orderId, 0, deviceType, errorMessage, false, DateTime.Now);
                }
            }

            return View("thankyou-questionnaire");
        }

        [HttpPost]
        public PartialViewResult LoadSideBar(string catId, int itemCount)
        {
            List<QuestionnaireGroup> questionnaireGroupList = _orderRepository.GetQuestionnaireGroupByCatId(catId, "QuestionnaireGroup_Order Asc");

            return PartialView("UploadDocLeftMenuDynamic", new QuestionGroupSideBarModel { QuestionnaireGroupList = questionnaireGroupList, ActiveMenu = "upload-doc", ItemCount = itemCount });
        }

        [HttpPost]
        public ActionResult TrackCustomerQuestionnaireResponse(LoggedCustomer loggedCustomer, int questionnaireFk, int orderId, int deviceType)
        {
            bool flag = false;

            _orderRepository.UpdateCustomerQuestionnaireResponse(loggedCustomer.CustomerId, orderId, questionnaireFk, deviceType, false, "questionnaire", DateTime.Now);

            var questionnaireResponse = new Dictionary<string, bool>
            {
                { "status", flag }
            };

            return Json(questionnaireResponse, JsonRequestBehavior.AllowGet);
        }

        public ActionResult PrescriptionOptions(int orderId, LoggedCustomer loggedCustomer, AffiliateInfo affiliateInfo)
        {
            if (loggedCustomer.CustomerId == 0)
            {
                return RedirectToAction("Login", "Customer", new { returnurl = "/prescription-options/" + orderId });
            }

            @ViewBag.Title = " Order Received | " + SiteConfigurationsWc.StorefrontName;
            // get customer info
            CustomerMinimal customerInfo = _customerRepository.GetCustomerInfo(loggedCustomer.CustomerId);

            OrderDetailMinimal orderInfo = _orderRepository.GetOrderDetailMinimal(orderId, affiliateInfo.AffiliateStoreFrontFk);

            // check if signed in user and OrderInvoiceCustomerFk is different
            // added check for order security
            if (orderInfo.OrderInvoiceCustomerFk != loggedCustomer.CustomerId)
            {
                return RedirectToAction("dashboard", "customer");
            }

            string questionnaireCatId = null;
            bool multipleCat = false;
            foreach (var product in orderInfo.OrderCart)
            {
                if (multipleCat)
                {
                    questionnaireCatId = questionnaireCatId + "," + product.ProductQuestionnaireCatId.ToString();
                }
                else
                {
                    questionnaireCatId = product.ProductQuestionnaireCatId.ToString();
                }

                multipleCat = true;
            }
            var orderTimeElapsed = DateTime.Now.Subtract(orderInfo.OrderInvoiceDateCreated).TotalMinutes;

            return View(new OrderConfirmViewModel
            {
                OrderId = orderId,
                QuestionnaireCatId = questionnaireCatId
            });
        }

        public ActionResult ViewAnsweredQuestionnaire(LoggedCustomer loggedCustomer, int orderId)
        {
            if (loggedCustomer.CustomerId == 0)
            {
                return RedirectToAction("Login", "Customer");
            }
            List<int> questionnaireCategoryList = _orderRepository.GetProductQuestionnaireCategoryByOrderId(orderId);
            string questionnaireCatId = string.Join(",", questionnaireCategoryList);
            var answerList = new List<QuestionOptionSelected>();

            string[] questionnaireCatIdArr = questionnaireCatId.Split(',');
            foreach (string questionnaireCatIdArrItem in questionnaireCatIdArr)
            {
                int catId = Convert.ToInt32(questionnaireCatIdArrItem);
                bool checkIfUserAnsweredQuestion = _orderRepository.IfUserAlreadyAnsweredQuestionnaire(catId, loggedCustomer.CustomerId, DateTime.Now, SiteConfigurationsWc.QuestionnaireTimeSpan, orderId);
                if (checkIfUserAnsweredQuestion)
                {
                    answerList.AddRange(_orderRepository.GetAnsweredQuestionnaireForCustomer(catId, loggedCustomer.CustomerId, DateTime.Now, SiteConfigurationsWc.QuestionnaireTimeSpan, orderId));

                    questionnaireCatIdArr = questionnaireCatIdArr.Where(s => s != questionnaireCatIdArrItem).ToArray();
                }
            }
            var productNamesListForAnswredQuestionnaire = answerList.Select(o => o.ProductName).Distinct().ToList();
            @ViewBag.Title = "Questionnaire View | " + SiteConfigurationsWc.StorefrontUrl;
            List<int> catgoryArray = answerList.Select(o => o.QuestionnaireCategoryId).Distinct().ToList();
            //string questionnaireCatId = null;
            //bool multpleCat = false;
            //foreach (int catId in catgoryArray)
            //{
            //    if (multpleCat)
            //    {
            //        questionnaireCatId = questionnaireCatId + "," + catId.ToString();
            //    }
            //    else
            //    {
            //        questionnaireCatId = catId.ToString();
            //    }

            //    multpleCat = true;
            //}
            List<Questionnaire> questionnaireList = _orderRepository.GetQuestionnaireByCategoryId(questionnaireCatId, "Questionnaire_AskOrder Asc");
            List<QuestionnaireOption> questionnaireOptionList = _orderRepository.GetQuestionOptionByQuestionnaireId(questionnaireCatId, "QuestionnaireOption_Order Asc");
            List<QuestionnaireGroup> questionnaireGroupList = _orderRepository.GetQuestionnaireGroupByCatId(questionnaireCatId, "QuestionnaireGroup_Order Asc");

            return View("ViewAnsweredQuestionnaire", new RequirePrescriptionReviewViewModel
            {
                QuestionnaireList = questionnaireList,
                QuestionnaireOptionList = questionnaireOptionList,
                AnswerList = answerList,
                OrderId = orderId,
                QuestionnaireGroupList = questionnaireGroupList,
                QuestionnaireCatId = questionnaireCatId,
                AnsweredCategoriesList = catgoryArray,
                ProductNameList = productNamesListForAnswredQuestionnaire

            });
        }

        private void UpdateCartForAutoRefill(int newOrderId, IEnumerable<ProductCart> autoFilledCart, CustomerMinimal customerInfo)
        {
            if (SiteConfigurationsWc.EnableSubscription)
            {
                // check if autofill is checked for any cart item and update product cart
                if (autoFilledCart.Any(x => x.Subscription))
                {
                    foreach (var autoFilledItem in autoFilledCart)
                    {
                        if (autoFilledItem.Subscription)
                        {
                            var dateCreated = DateTime.Now;
                            var SubscriptionId = _orderRepository.AddCustomerSubscription(customerInfo.CustomerId, newOrderId, autoFilledItem.ProductId,
                                        autoFilledItem.ProductSizeId, autoFilledItem.CartItemQuantity, autoFilledItem.Subscription, dateCreated);
                            if (SubscriptionId > 0)
                            {
                                _customerRepository.AddLogCustomerSubscription(customerInfo.CustomerId, SubscriptionId, autoFilledItem.Subscription, "Add", dateCreated);
                            }
                        }
                    }
                }
            }
        }

        private bool UpdatedRefillCart(OrderDetail orderDetail, int customerId)
        {
            bool returnToRefill = true;
            foreach (CartDetail cartItem in orderDetail.OrderCart)
            {
                RefillOrder refillOrderStatusdDetail = _orderRepository.GetOrderRefillDetails(customerId, cartItem.ProductSizeStrength, cartItem.ProductSizeId);
                if (refillOrderStatusdDetail.RefilQuantityAuthorised > refillOrderStatusdDetail.RefilQuantityUsed)
                {
                    // returnToRefill = true;
                    int updateCartRefill = _orderRepository.UpdateOrderCartRefillStatus(cartItem.ProductSizeId, orderDetail.OrderInvoiceId, true);
                }
                else
                {
                    returnToRefill = false;
                }

            }
            return returnToRefill;
        }

        public ActionResult GetBPQuestionnaires()
        {
            var BPQuestionnaire = _orderRepository.GetBPQuestionnaires();

            List<string> topBloodPressureQuestionnaire = new List<string>();
            List<string> bottomBloodPressureQuestionnaire = new List<string>();
            var questionnaireCount = 0;

            if (BPQuestionnaire != null)
            {
                questionnaireCount = BPQuestionnaire.Count;
                topBloodPressureQuestionnaire = BPQuestionnaire.Where(x => x.QuestionnaireFk == 36).Select(x => x.QuestionnaireOption).ToList();
                bottomBloodPressureQuestionnaire = BPQuestionnaire.Where(x => x.QuestionnaireFk == 37).Select(x => x.QuestionnaireOption).ToList();

            }
            return Json(new
            {
                questionnaireCount = questionnaireCount,
                topBloodPressureQuestionnaire = topBloodPressureQuestionnaire,
                bottomBloodPressureQuestionnaire = bottomBloodPressureQuestionnaire
            }, JsonRequestBehavior.AllowGet);
        }


        public ActionResult UpdateBPQuestionnaireResponse(int OrderInvoiceId, int CustomerFk, int topBloodPressureSelected, int bottomBloodPressureSelected)
        {

            if (OrderInvoiceId > 0 && CustomerFk > 0 && topBloodPressureSelected > 0 && bottomBloodPressureSelected > 0)
            {
                _orderRepository.UpdateBPQuestionnaireResponse(CustomerFk, OrderInvoiceId, 36, topBloodPressureSelected); /* update top blood pressure */
                _orderRepository.UpdateBPQuestionnaireResponse(CustomerFk, OrderInvoiceId, 37, bottomBloodPressureSelected); /* update bottom blood pressure */

                _orderRepository.UpdateOrderInvoiceBPStatus(OrderInvoiceId, false); /* update order invoice BPNotKnown status */

                string ipAddress = Utility.GetVisitorIpAddress();
                string comment = "BP response provided";
                _orderRepository.AddLogPreScreenOrderBPUpdate(CustomerFk, OrderInvoiceId, 0, comment, ipAddress, DateTime.UtcNow);

                /* removed schedule email notification*/
                _orderRepository.UpdateNitroEmailQueueStatus(CustomerFk, OrderInvoiceId);
            }


            return Json(new
            {
                orderStatus = 1
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult RefillOrder(LoggedCustomer loggedCustomer, AffiliateInfo affiliateInfo, Cart cart, int shippingInvoiceId)
        {
            if (loggedCustomer.CustomerId == 0)
            {
                return RedirectToAction("Login", "Customer", new { returnurl = "/refill-order/" + shippingInvoiceId });
            }

            // get cart details for the shipping invoice
            var cartDetail = _customerRepository.GetCustomerOrderCartByShipInvoice(loggedCustomer.CustomerId, affiliateInfo.AffiliateStoreFrontFk, shippingInvoiceId);
            var refillsLeft =  cartDetail.NumberOfRefillsAvailable - cartDetail.RefillOrdersPlaced;

            if (cartDetail.RefillOrdersPlaced > cartDetail.NumberOfRefillsAvailable ||
                cartDetail.NumberOfRefillsAllowed < refillsLeft)
            {
                TempData["IsAutoRefill"] = 1;
                return RedirectToAction("ProductDetail", "Product", new { searchTerm = cartDetail.ProductName.ToLower()});
            }

            // Add same productsize in cart
            ProductCart productCart = new ProductCart { ProductSizeId = cartDetail.ProductSizeId };

            cart.AddCartItem(productCart, 1, 1);

            cart.IsAutoRefillOrder = true;
            var shippingInvoiceFk = shippingInvoiceId;
            if (cartDetail.IsRefillCartItem)
            {
                shippingInvoiceFk = cartDetail.OriginalRefillOrderShippIngInvoiceFk;
                cart.UpdateCartItemRefill(cartDetail.ProductSizeId, true, cartDetail.OriginalRefillOrderShippIngInvoiceFk,
                                    cartDetail.NumberOfRefillsAllowed, refillsLeft);
            }


            var refillMessage = "Auto Refill from Email Of Shipping Id: " + shippingInvoiceFk;

            var logRefillId = _orderRepository.AddLogRefillOrders(0, shippingInvoiceFk,
                                   cartDetail.NumberOfRefillsAllowed, refillsLeft, refillMessage,
                                   "Create Order", DateTime.UtcNow,
                                    DateTime.UtcNow, true);
            // update logRefillId for cartitem
            cart.CartItems.Where(x => x.ProductCart.ProductSizeId == cartDetail.ProductSizeId).Select(c => { c.LogRefillId = logRefillId; return c; });

            for (var x = 0; x < cart.CartItems.Count(); x++)
            {
                cart.CartItems.ElementAt(x).LogRefillId = logRefillId;
                // FrontendVisibleProductSizeFk is used to fetch the discount volume products
                var frontendVisible = cart.CartItems.ElementAt(x).ProductCart.ProductSizeId;
                if (cart.CartItems.ElementAt(x).ProductCart.FrontendVisibleProductSizeFk != 0)
                    frontendVisible = cart.CartItems.ElementAt(x).ProductCart.FrontendVisibleProductSizeFk;

                ProductCart productInCart =
                    _productRepository.GetProductSizeInfoForCart(cart.CartItems.ElementAt(x).ProductCart.ProductSizeId, affiliateInfo.AffiliateStoreFrontFk, frontendVisible);
                if (productInCart != null)
                {
                    cart.CartItems.ElementAt(x).ProductCart = productInCart;
                }

            }
            return RedirectToAction("checkout", "order", new { isAutoRefill = true});
        }

        public ActionResult AddRefillQuestionnaire(LoggedCustomer loggedCustomer, 
                        AffiliateInfo affiliateInfo, Cart cart, List<QuestionOptionSelected> questionOptionSelected, int orderInvoiceFk)
        {
            var customerSessionTrackerId = 0;
            if (loggedCustomer.CustomerId == 0)
            {
                customerSessionTrackerId = _orderRepository.AddCustomerSessionTracker(loggedCustomer.CustomerId, orderInvoiceFk, "AddRefillQuestionnaire",
                          "Error- Session Expired", 2, DateTime.Now);

                return RedirectToAction("Login", "Customer");
            }

            customerSessionTrackerId = _orderRepository.AddCustomerSessionTracker(loggedCustomer.CustomerId, orderInvoiceFk, "AddRefillQuestionnaire",
                            "Start- Answer Refill Questionnaire", 0, DateTime.Now);

            @ViewBag.Title = "Refill Questionnaire | " + SiteConfigurationsWc.StorefrontUrl;

            var refillQuestions = _orderRepository.GetRefillQuestionnaire();
            // to do build viewModel and show refillQuestionnaire

            return View();
        }


    }


}
