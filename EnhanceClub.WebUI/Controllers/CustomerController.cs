using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Web.Mvc;
using System.Web.Security;
using Castle.Core.Internal;
using EnhanceClub.Domain.Abstract;
using EnhanceClub.Domain.Entities;
using EnhanceClub.WebUI.Helpers;
using EnhanceClub.WebUI.Infrastructure.Utility;
using EnhanceClub.WebUI.Models;
using RazorEngine.Templating;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;
using EnhanceClub.WebUI.AwsHelper;
using EnhanceClub.Domain.Entities.Enum;
using EnhanceClub.Domain.AwsEntities;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

// Created by Rajiv S : 23 July 2020

namespace EnhanceClub.WebUI.Controllers
{

    public class CustomerController : Controller
    {
        private readonly ICustomerRepository _repositoryCustomer;
        private readonly IAuthProvider _authProvider;
        private readonly IEmailSender _emailSender;
        private readonly IAdminRepository _adminRepository;
        private readonly IOrderProcessor _orderRepository;
        public int PageSize = 5;

        public CustomerController(ICustomerRepository customerRepository,
                                    IAuthProvider auth,
                                    IEmailSender mailSender,
                                    IAdminRepository adminRepository,
                                    IOrderProcessor orderRepository)
        {
            this._repositoryCustomer = customerRepository;
            _authProvider = auth;
            _emailSender = mailSender;
            _adminRepository = adminRepository;
            _orderRepository = orderRepository;
        }

        // present login form
        public ViewResult Login(string returnUrl)
        {
            @ViewBag.Title = "Customer Login | " + @SiteConfigurationsWc.StorefrontName;

            @ViewBag.Description =
                "Welcome to " + @SiteConfigurationsWc.StorefrontName + ". Login to your account now and start your order.";
            @ViewBag.MetaRobot = "noindex,follow";

            if (@Request.Url != null)
            {
                @ViewBag.canonicalRef = Request.Url.AbsoluteUri;
            }

            return View("login", new LoginViewModel
            {
                Terms = false,
                TeleHealthTerms = false
            });
        }

        [HttpPost]
        public ActionResult Login(LoginViewModel model, string returnUrl, AffiliateInfo affiliateInfo)
        {
            ViewBag.ReturnUrl = returnUrl;
            if (ModelState.IsValid)
            {
                /* setup user local time zone*/
                if (HttpContext.Request.Cookies.AllKeys.Contains("UserTimeZoneOffsetMinutes"))
                {
                    if (HttpContext.Request.Cookies["UserTimeZoneOffsetMinutes"] != null && HttpContext.Request.Cookies["TimeZoneName"] != null)
                    {
                        Session["UserTimeZoneOffsetMinutes"] = HttpContext.Request.Cookies["UserTimeZoneOffsetMinutes"].Value;
                        Session["TimeZoneName"] = HttpUtility.UrlDecode(HttpContext.Request.Cookies["TimeZoneName"].Value);
                    }
                }
                var cleanEmailAddress = Utility.StripGmailAddress(model.Email);

                // before checking with aws check with our db first
                var customerId = _repositoryCustomer.GetCustomerFromEmailOrId(cleanEmailAddress, 0, affiliateInfo.AffiliateId);

                if (customerId == 0)
                {
                    ModelState.AddModelError("", "You have entered an invalid username or password.");
                    return View("login", new LoginViewModel
                    {
                        Email = model.Email,
                        Password = null,
                        Terms = false,
                        TeleHealthTerms = false

                    });
                }
                var userDetails = CognitoHelper.GetUser(cleanEmailAddress);

                // to do check if email is not verified
                // then send email verification code to user and verify code
                
                var srpAuthResponse = CognitoHelper.GetCredentialsAsync(cleanEmailAddress, model.Password).Result;
                
                if (srpAuthResponse.UserNotConfirmed)
                {
                    var sendEmail = CognitoHelper.ResendConfirmationEmail(cleanEmailAddress);

                    var ipAddress = Utility.GetVisitorIpAddress();
                    if (sendEmail.HttpStatusCode == HttpStatusCode.OK)
                    {
                        var logid = _repositoryCustomer.AddLogCognitoSendEmailVerification(sendEmail, affiliateInfo.AffiliateStoreFrontFk, model.Email, ipAddress, "success",
                            DateTime.Now, (int)CognitoActionTypeEnum.SignIn, false);
                    }
                    else
                    {
                        var logid = _repositoryCustomer.AddLogCognitoSendEmailVerification(sendEmail, affiliateInfo.AffiliateStoreFrontFk, model.Email, ipAddress, "error",
                           DateTime.Now, (int)CognitoActionTypeEnum.SignIn, false);
                    }

                    return View("signup-verification",
                               new CustomerSignUp
                               {
                                   CustomerId = model.CustomerId,
                                   CustomerTypedEmail = model.Email,
                                   CustomerEmail = cleanEmailAddress,
                                   CustomerPassword = model.Password,
                                   CognitoActionType = (int)CognitoActionTypeEnum.SignIn
                               });
                }

                else if (string.IsNullOrEmpty(srpAuthResponse.Message))
                {
                    // get customer information

                    if (!string.IsNullOrEmpty(srpAuthResponse.SessionID))
                    {

                        TempData["sessionId"] = srpAuthResponse.SessionID;

                        return View("signin-mfa", new CustomerSignUp
                        {
                            CustomerId = model.CustomerId,
                            CustomerEmail = cleanEmailAddress,
                            SessionId = srpAuthResponse.SessionID
                        });
                    }
                }

                ModelState.AddModelError("", "You have entered an invalid username or password.");
                return View("login", new LoginViewModel
                {
                    Email = model.Email,
                    Password = null,
                    Terms = false,
                    TeleHealthTerms = false

                });


                //  var validCredentials = CognitoHelper.CheckPasswordAsync(model.Email, model.Password).Result;               

                //encrypt password before comparing 
                // model.Password = Utility.EncryptCfm(SiteConfigurationsWc.AesEncryptionKey, model.Password);

                //if (_authProvider.Authenticate(model.Email, model.Password, _repositoryCustomer))

                //if (validCredentials)
                //{
                //    if (String.IsNullOrEmpty(returnUrl))
                //    {
                //        return RedirectToAction("dashboard", "Customer", new { searchTerm = "", fromLogin = true }); // replace it with my account section when ready
                //    }
                //    else
                //    {
                //        return Redirect(returnUrl);
                //    }

                //    // return Redirect(returnUrl ?? Url.Action("Index", "Home"));
                //    //return RedirectToAction(returnUrl  ?? Url.Action("Index", "Home"));
                //}
                //else
                //{
                //    ModelState.AddModelError("", "You have entered an invalid username or password.");
                //    return View("login", new LoginViewModel
                //    {
                //        Email = model.Email,
                //        Password = null,
                //        Terms = false,
                //        TeleHealthTerms = false

                //    });
                //}
            }
            else
            {
                return View();
            }
        }

        // login user after mfa code is authenticated
        public ActionResult LoginUser(string customerEmail, string accessToken)
        {
            // password is empty for authentication
            if (_authProvider.Authenticate(customerEmail, accessToken, _repositoryCustomer))
            {
                return RedirectToAction("dashboard", "Customer", new { searchTerm = "", fromLogin = true });
            }
            return View("login", new LoginViewModel
            {
                Email = customerEmail,
                Password = null,
                Terms = false,
                TeleHealthTerms = false

            });
        }

        // new customer sign up step 1
        [ActionName("get-started")]
        public ViewResult SignUp()
        {
            @ViewBag.Title = "Create New Account - Signup Account | " + SiteConfigurationsWc.StorefrontName;
            @ViewBag.Description = "Get started with " + SiteConfigurationsWc.StorefrontName + ". Create your account and be the best version of yourself.";
            return View(new CustomerInfoViewModel { Customer = new Customer() });

        }

        [HttpPost]
        [ActionName("get-started")]
        public ActionResult SignUpCognito(CustomerSignUp customerInfo, AffiliateInfo affiliateInfo, string returnUrl)
        {
            // Comment: #IpAddress - get customer ip address 
            var ipAddress = Utility.GetVisitorIpAddress();
            customerInfo.CustomerLastIp = ipAddress;


            if (ModelState.IsValid)
            {

                // Setup other fields like Date created, affiliate , last modified 
                customerInfo.SetOtherFields(true,
                    DateTime.Now,
                    DateTime.Now,
                    DateTime.Now,
                    affiliateInfo.AffiliateId,
                    false);

                // clean email Address in case of gmail account
                var cleanEmailAddress = Utility.StripGmailAddress(customerInfo.CustomerEmail);

                // Set as Partial account until step 2 is completed
                customerInfo.CustomerIsPartial = true;

                customerInfo.CustomerCountryFk = @SiteConfigurationsWc.StoreFrontCountryId;
                customerInfo.CustomerProvinceFk = 0;
                customerInfo.CustomerAddress = string.Empty;
                customerInfo.CustomerCity = string.Empty;
                customerInfo.CustomerFirstName = string.Empty;
                customerInfo.CustomerLastName = string.Empty;
                customerInfo.CustomerFreeShipping = false;
                customerInfo.CustomerFreeShippingOneYear = false;
                customerInfo.CustomerPassword = customerInfo.CustomerPassword;

                // in case of gmail account if it has(+ or .), the typed email will save it with +/.
                // in case email address is already clean the both fields will still store customeremail
                customerInfo.CustomerTypedEmail = customerInfo.CustomerEmail;
                customerInfo.CustomerEmail = cleanEmailAddress;

                // save to aws cognito user pools

                var signupResult = AwsHelper.CognitoHelper.AwsSignUpUser(customerInfo.CustomerEmail, customerInfo.CustomerPassword, customerInfo.CustomerPhoneWithCountryCode);


                if (signupResult.HttpStatusCode == HttpStatusCode.OK)
                {
                    customerInfo.CognitoUserSub = signupResult.UserSub;
                    // Add Account 
                    int newCustomerId = _repositoryCustomer.AddCustomerPartial(customerInfo);

                    if (newCustomerId != 0)
                    {
                        // add to Log
                        _repositoryCustomer.AddLogCongnitoSignUpResponse(signupResult, affiliateInfo.AffiliateStoreFrontFk, newCustomerId, ipAddress, DateTime.Now, "Success -User Created in AWS and Database");

                        customerInfo.CustomerId = newCustomerId;

                        //// Login Customer
                        _authProvider.PreAuthenticate(customerInfo.CustomerEmail, _repositoryCustomer);

                        //if (SiteConfigurationsWc.EmailSend)
                        //{
                        //    // send welcome email to user
                        //    var templateService = new TemplateService();
                        //    var emailHtmlBody = "";

                        //    string templateFilePath =
                        //        System.Web.HttpContext.Current.Server.MapPath(
                        //            SiteConfigurationsWc.EmailTemplatePath + "SignupEmail.cshtml"); // to do use updated template


                        //    if (!templateFilePath.IsNullOrEmpty())
                        //    {
                        //        emailHtmlBody = templateService.Parse(System.IO.File.ReadAllText(templateFilePath),
                        //            customerInfo,
                        //            null,
                        //            null);
                        //    }

                        //    var mailSubject = "Welcome to EnhanceClub";


                        //    _emailSender.SendEmail(customerInfo.CustomerEmail,
                        //        mailSubject,
                        //        emailHtmlBody,
                        //        SiteConfigurationsWc.EmailFrom,
                        //        0);

                        //    // add send email log
                        //    var logId = _adminRepository.AddLogSendEmailReminder(0, affiliateInfo.AffiliateStoreFrontFk,
                        //                                customerInfo.CustomerId, 0, customerInfo.CustomerEmail, customerInfo.CustomerPhone, DateTime.Now,
                        //                                (int)EmailTypeEnum.SignUp, mailSubject, emailHtmlBody, (int)SourceApplicationEnum.EnhanceClub);

                        //}

                        //  return RedirectToAction(returnUrl ?? Url.RouteUrl("customer-route-4"));
                        //return Redirect("/get-started-step-2");
                        // return RedirectToAction("signup-verification");
                        return View("signup-verification",
                               new CustomerSignUp
                               {
                                   CustomerId = customerInfo.CustomerId,
                                   CustomerEmail = customerInfo.CustomerEmail,
                                   CustomerPassword = customerInfo.CustomerPassword,
                                   CognitoActionType = (int)CognitoActionTypeEnum.SignUp
                               });

                    }

                    else
                    {

                        // add to Log
                        _repositoryCustomer.AddLogCongnitoSignUpResponse(signupResult, affiliateInfo.AffiliateStoreFrontFk, newCustomerId, ipAddress, DateTime.Now, "Error - Error creating user in database");


                        ModelState.AddModelError("email", "Error! could not create Account, please contact Support");

                        return
                            View(new CustomerSignUpViewModel
                            {
                                CustomerSignUpInfo = customerInfo
                            });
                    }
                }
                else
                {
                    // add to log
                    _repositoryCustomer.AddLogCongnitoSignUpResponse(signupResult, affiliateInfo.AffiliateStoreFrontFk, 0, ipAddress, DateTime.Now, "Error - Error creating user in AWS");

                    if (signupResult.ErrorCode == "InvalidPasswordException")
                        ModelState.AddModelError("password", "Please choose a strong password and signup again");
                    else
                        ModelState.AddModelError("email", "Error! could not create Account, please contact Support");

                    return
                        View(new CustomerSignUpViewModel
                        {
                            CustomerSignUpInfo = customerInfo
                        });

                }

            }
            else
            {
                ModelState.AddModelError("", "");
                return View(new CustomerSignUpViewModel
                {
                    CustomerSignUpInfo = customerInfo

                });
            }
        }


        //[ActionName("signup-verification")]
        //[Authorize]
        //public ActionResult SignUpVerification(LoggedCustomer loggedCustomer)
        //{
        //    if (loggedCustomer.CustomerId == 0)
        //    {
        //        return RedirectToAction("get-started");
        //    }
        //    @ViewBag.Title = "Create New Account - Signup Account | " + SiteConfigurationsWc.StorefrontName;
        //    @ViewBag.Description = "Get started with " + SiteConfigurationsWc.StorefrontName + ". Create your account and be the best version of yourself.";

        //    if (Request.Url != null)
        //    {
        //        @ViewBag.canonicalRef = Request.Url.AbsoluteUri;
        //    }

        //    // get province list for canada (country code - 36)
        //    List<Province> provinceList = _repositoryCustomer.GetProvinceListByCountry(SiteConfigurationsWc.StoreFrontCountryId);

        //    return View(new CustomerSignUp
        //    {
        //        CustomerId = loggedCustomer.CustomerId,
        //        CustomerEmail = loggedCustomer.CustomerEmail,
        //        ProvinceList = provinceList,

        //    });

        //}

        public ActionResult VerifyAuthenticationCode(LoggedCustomer loggedCustomer, Cart cart, CustomerSignUp customerInfo, AffiliateInfo affiliateInfo, string returnUrl, int actionType = 1)
        {
            var ipAddress = Utility.GetVisitorIpAddress();
            var cleanEmailAddress = Utility.StripGmailAddress(customerInfo.CustomerEmail);
            var signupVerifyResponse = AwsHelper.CognitoHelper.VerifyAccessCode(cleanEmailAddress, customerInfo.CognitoSignupEmailVerificationCode);

            var userRequested = actionType == (int)CognitoActionTypeEnum.UserRequestedEmailResend ? true : false;
            var sessionId = string.Empty;
            if (signupVerifyResponse.HttpStatusCode == HttpStatusCode.OK)
            {
                // update singup email verified flag
                // get customer id by email
                var customerId = loggedCustomer.CustomerId;
                if (customerId == 0)
                    customerId = _repositoryCustomer.GetCustomerFromEmailOrId(customerInfo.CustomerEmail, 0, affiliateInfo.AffiliateId);

                var emailVerifiedStatus = _repositoryCustomer.UpdateCustomerSignUpEmailVerifiedFlag(customerId, true);


                // add log for verification success
                var updateStatus = _repositoryCustomer.AddLogCognitoSignUpVerification(signupVerifyResponse, affiliateInfo.AffiliateStoreFrontFk, customerId,
                    DateTime.Now, ipAddress, "success", actionType, userRequested);

                var srpAuthResponse = AwsHelper.CognitoHelper.GetCredentialsAsync(cleanEmailAddress, customerInfo.CustomerPassword).Result;

                // to do add logs for session id
                //

                if (srpAuthResponse != null)
                {
                    if (!string.IsNullOrEmpty(srpAuthResponse.SessionID))
                    {
                        var sessionLogId = _repositoryCustomer.AddLogCognitoSignupAuthSession(srpAuthResponse, affiliateInfo.AffiliateStoreFrontFk, customerId,
                            DateTime.Now, ipAddress, "Success");

                        // get province list for canada (country code - 36)
                        List<Province> provinceList = _repositoryCustomer.GetProvinceListByCountry(SiteConfigurationsWc.StoreFrontCountryId);

                        return
                            View("get-started-step-2", new CustomerSignUp
                            {
                                CustomerId = customerInfo.CustomerId,
                                SessionId = srpAuthResponse.SessionID,
                                CustomerEmail = cleanEmailAddress,
                                ProvinceList = provinceList,

                            });
                    }
                }

                var logId = _repositoryCustomer.AddLogCognitoSignupAuthSession(srpAuthResponse, affiliateInfo.AffiliateStoreFrontFk, loggedCustomer.CustomerId,
                           DateTime.Now, ipAddress, "Error");

                // to do need to update this message, as in this case  code verification is successful but session id is null
                @ViewBag.ErrorMessage = "Please enter a valid authentication code";

            }
            else
            {
                // add log for verification error
                var updateStatus = _repositoryCustomer.AddLogCognitoSignUpVerification(signupVerifyResponse, affiliateInfo.AffiliateStoreFrontFk, loggedCustomer.CustomerId,
                  DateTime.Now, ipAddress, "error", actionType, userRequested);

                @ViewBag.ErrorMessage = "Please enter a valid authentication code";
            }

            return View("signup-verification",
                new CustomerSignUp
                {
                    CustomerId = loggedCustomer.CustomerId,
                    CustomerEmail = loggedCustomer.CustomerEmail,
                    CustomerPassword = customerInfo.CustomerPassword
                });

            //  return Json(new { authenticationApproved }, JsonRequestBehavior.AllowGet);

        }

        // this method verifies MFA phone code
        public ActionResult VerifyMfaPhoneCode(int customerId, string customerEmail, string phoneVerificationCode,
            string sessionId, int actionType, AffiliateInfo affiliateInfo)
        {
            bool authenticationApproved = false;
            CognitoHelper cHelper = new CognitoHelper();

            var ipAddress = Utility.GetVisitorIpAddress();

            // get user attributes (user sub) from aws cognito
            var cleanEmailAddress = Utility.StripGmailAddress(customerEmail);
            var userAttributes = CognitoHelper.GetUser(cleanEmailAddress);
            var userSub = string.Empty;
            if (userAttributes != null)
            {
                userSub = userAttributes.Where(x => x.Name == "sub").Select(x => x.Value).FirstOrDefault();
            }

            // Start: commented code to fetch stub from database // to do remove after testing
            // get user sub from database
            //var customerInfo = _repositoryCustomer.GetCustomerInfoPartial(customerId);
            //var userSub = customerInfo!= null? customerInfo.CognitoUserSub : string.Empty;     
            // End: commented code to fetch stub from database // to do remove after testing

            var result = cHelper.RespondToTwoFactorChallengeAsync(userSub, phoneVerificationCode, sessionId, new CancellationToken()).Result;

            var accessToken = string.Empty;
            if (!string.IsNullOrEmpty(result.AccessToken))
            {
                accessToken = result.AccessToken;
                authenticationApproved = true;

                // update user id in customer table
                var updateCustomerStatus = _repositoryCustomer.UpdateCustomerCognitoUserId(customerId, result.UserId);

                var authenticateUser = _authProvider.Authenticate(cleanEmailAddress, accessToken, _repositoryCustomer);

                var logid = _repositoryCustomer.AddLogCognitoAuthFlowMfaResponse(result, affiliateInfo.AffiliateStoreFrontFk, customerId, ipAddress, "success", DateTime.Now, actionType);
            }
            else
            {
                var logid = _repositoryCustomer.AddLogCognitoAuthFlowMfaResponse(result, affiliateInfo.AffiliateStoreFrontFk, customerId, ipAddress, "error", DateTime.Now, actionType);
            }

            // add log and 

            return Json(new { verified = authenticationApproved, userAccessToken = accessToken }, JsonRequestBehavior.AllowGet);
        }

        // New customer sign up step -2 
        [ActionName("get-started-step-2")]
        [Authorize]
        public ActionResult SignUpStep2(LoggedCustomer loggedCustomer, bool partialProfile = false)
        {
            if (loggedCustomer.CustomerId == 0)
            {
                return RedirectToAction("get-started");
            }
            @ViewBag.Title = "Create New Account - Signup Account | " + SiteConfigurationsWc.StorefrontName;
            @ViewBag.Description = "Get started with " + SiteConfigurationsWc.StorefrontName + ". Create your account and be the best version of yourself.";

            if (Request.Url != null)
            {
                @ViewBag.canonicalRef = Request.Url.AbsoluteUri;
            }
            if (partialProfile)
            {
                ViewBag.ReturnMessage = "Let's complete the signup first to continue with your order.";
            }
            // get province list for canada (country code - 36)
            List<Province> provinceList = _repositoryCustomer.GetProvinceListByCountry(SiteConfigurationsWc.StoreFrontCountryId);

            return View(new CustomerSignUp
            {
                CustomerId = loggedCustomer.CustomerId,
                CustomerEmail = loggedCustomer.CustomerEmail,
                ProvinceList = provinceList

            });

        }

        [HttpPost]
        [ActionName("get-started-step-2")]
        public ActionResult SignUpStep2(LoggedCustomer loggedCustomer, Cart cart, CustomerSignUp customerInfo, AffiliateInfo affiliateInfo, string returnUrl, string accessToken)
        {

            if (loggedCustomer.CustomerId == 0)
            {
                return RedirectToAction("get-started");
            }

            // Comment: #IpAddress - get customer ip address 
            var ipAddress = Utility.GetVisitorIpAddress();
            customerInfo.CustomerLastIp = ipAddress;

            //CognitoHelper cHelper = new CognitoHelper();

            //var result = cHelper.RespondToTwoFactorChallengeAsync(customerInfo.CustomerEmail, customerInfo.CognitoSignupPhoneVerificationCode, customerInfo.SessionId, new CancellationToken());
            var result = CognitoHelper.UpdateUserAttributes(loggedCustomer.CustomerEmail, accessToken, customerInfo);

            // get province list for canada (country code - 36)
            List<Province> provinceList = _repositoryCustomer.GetProvinceListByCountry(SiteConfigurationsWc.StoreFrontCountryId);

            if (result.HttpStatusCode == HttpStatusCode.OK)
            {
                // add log
                var logId = _repositoryCustomer.AddLogCognitoUserAttributesResponse(result, customerInfo, affiliateInfo.AffiliateStoreFrontFk,
                                            loggedCustomer.CustomerId, ipAddress, "success", DateTime.Now, (int)CognitoActionTypeEnum.SignUp);

                // update address details in database
                if (ModelState.IsValid)
                {

                    // reset to partial flag to full account
                    customerInfo.CustomerIsPartial = false;
                    customerInfo.CustomerId = loggedCustomer.CustomerId;

                    // Update Customer Account with Step 2 Information
                    int updateCustomer = _repositoryCustomer.UpdateCustomerPartial(customerInfo);

                    if (updateCustomer != 0)
                    {
                        // add default shipping address
                        if(SiteConfigurationsWc.ShowMultipleShippingAddress == 1)
                        {
                            var shipAddress = _repositoryCustomer.AddCustomerShippingAddress(loggedCustomer.CustomerId, customerInfo.CustomerFirstName,
                                   customerInfo.CustomerLastName, customerInfo.CustomerPhone, customerInfo.CustomerAddress, customerInfo.CustomerCity,
                                   SiteConfigurationsWc.StoreFrontCountryId,
                                   customerInfo.CustomerProvinceFk, customerInfo.CustomerZipCode, true, true, DateTime.Now);
                        }
                        
                        //if (SiteConfigurationsWc.EmailSend)
                        //{
                        //    // send fully registered emails to user
                        //    var templateService = new TemplateService();
                        //    var emailHtmlBody = "";

                        //    string templateFilePath =
                        //        System.Web.HttpContext.Current.Server.MapPath(
                        //            SiteConfigurationsWc.EmailTemplatePath + "SignupEmail.cshtml");


                        //    if (!templateFilePath.IsNullOrEmpty())
                        //    {
                        //        emailHtmlBody = templateService.Parse(System.IO.File.ReadAllText(templateFilePath),
                        //            customerInfo,
                        //            null,
                        //            null);
                        //    }

                        //    var mailSubject = "Welcome to EnhanceClub";


                        //    _emailSender.SendEmail(customerInfo.CustomerEmail,
                        //        mailSubject,
                        //        emailHtmlBody,
                        //        SiteConfigurationsWc.EmailFrom,
                        //        0);

                        //}

                        //_authProvider.Authenticate(loggedCustomer.CustomerEmail, accessToken, _repositoryCustomer);

                        //  return RedirectToAction(returnUrl ?? Url.RouteUrl("customer-route-4"));
                        if (cart != null && cart.CartItems.Any())
                        {
                            return Redirect("/order/checkout");
                        }
                        else
                        {
                            return Redirect("/get-started-step-3");
                        }


                    }

                    else
                    {

                        ModelState.AddModelError("email", "Error! could not update Account, please contact Support");

                        return
                            View(new CustomerSignUp
                            {
                                CustomerId = customerInfo.CustomerId,
                                ProvinceList = provinceList,

                            });
                    }

                }
                else
                {
                    ModelState.AddModelError("", "Please complete Account Info");
                    return View(customerInfo);
                }
            }
            var logerrorId = _repositoryCustomer.AddLogCognitoUserAttributesResponse(result, customerInfo, affiliateInfo.AffiliateStoreFrontFk,
                                           loggedCustomer.CustomerId, ipAddress, "error", DateTime.Now, (int)CognitoActionTypeEnum.SignUp);
            ModelState.AddModelError("email", "Error! could not update Account, please contact Support");

            return
                View(new CustomerSignUp
                {
                    CustomerId = customerInfo.CustomerId,
                    ProvinceList = provinceList,

                });
            //return null;
        }


        // New customer sign up step - 3 
        [ActionName("get-started-step-3")]
        public ViewResult SignUpStep3(LoggedCustomer loggedCustomer)
        {

            @ViewBag.Title = "Create New Account - Signup Account | " + SiteConfigurationsWc.StorefrontName;
            @ViewBag.Description = "Get started with " + SiteConfigurationsWc.StorefrontName + ". Create your account and be the best version of yourself.";


            if (Request.Url != null)
            {
                @ViewBag.canonicalRef = Request.Url.AbsoluteUri;
            }


            return View(new CustomerSignUp
            {
                CustomerId = loggedCustomer.CustomerId,
                CustomerEmail = loggedCustomer.CustomerEmail,

            });

        }


        // Password Recovery

        [ActionName("forgot-password")]
        public ViewResult ForgotPassword()
        {
            @ViewBag.Title = "Password Recovery - Forgot Password | " + SiteConfigurationsWc.StorefrontName;

            @ViewBag.Description =
                "Lost password? Reset your " + SiteConfigurationsWc.StorefrontName + "password by entering your username or email address.";

            if (Request.Url != null)
            {
                @ViewBag.canonicalRef = Request.Url.AbsoluteUri;
            }

            return View();
        }

        // send password recovery email

        [HttpPost]
        [ActionName("forgot-password")]
        public ActionResult ForgotPassword(string customerEmail, AffiliateInfo affiliateInfo, string returnUrl)
        {
            var ipAddress = Utility.GetVisitorIpAddress();
            if (ModelState.IsValid)
            {
                // get Customer Login Info    
                var cleanEmailAddress = Utility.StripGmailAddress(customerEmail);
                CustomerLoginInfo loginInfo = _repositoryCustomer.GetCustomerLoginInfo(cleanEmailAddress, affiliateInfo.AffiliateId);
                if (loginInfo != null)
                {
                    if (!loginInfo.SignupEmailVerified)
                    {
                        //  add log
                        var logIdIncomplete = _repositoryCustomer.AddLogCognitoResetPassword(new CognitoSignupResponse(), affiliateInfo.AffiliateStoreFrontFk, customerEmail, string.Empty,
                                        ipAddress, "error - Incomplete signup", DateTime.Now, (int)CognitoActionTypeEnum.InCompleteSignup);
                        return View("incomplete-signup",
                       new PasswordRecoveryViewModel { EmailProvided = customerEmail });
                    }
                    var resetPassword = CognitoHelper.ResetPassword(cleanEmailAddress);
                    if (resetPassword.HttpStatusCode == HttpStatusCode.OK)
                    {
                        //  add log
                        var logId = _repositoryCustomer.AddLogCognitoResetPassword(resetPassword, affiliateInfo.AffiliateStoreFrontFk, customerEmail, string.Empty,
                                        ipAddress, "success- send verification code", DateTime.Now, (int)CognitoActionTypeEnum.ResetPassword);
                        return View("RecoverLogin",
                          new PasswordRecoveryViewModel { CustomerLoginInfo = loginInfo, EmailProvided = cleanEmailAddress });
                    }
                    else
                    {
                        // add log
                        var logId = _repositoryCustomer.AddLogCognitoResetPassword(resetPassword, affiliateInfo.AffiliateStoreFrontFk, customerEmail, string.Empty,
                                      ipAddress, "error- send verification code", DateTime.Now, (int)CognitoActionTypeEnum.ResetPassword);

                        ModelState.AddModelError("", "Please provide a valid email");
                        return View();
                    }
                }
                else
                {
                    return View("RecoverLogin", new PasswordRecoveryViewModel { CustomerLoginInfo = loginInfo, EmailProvided = cleanEmailAddress });
                }

            }
            else
            {
                ModelState.AddModelError("", "Please provide a valid email");
                return View();
            }
        }

        [ActionName("reset-password")]
        public ActionResult ResetPassword(string code, string email, string password, AffiliateInfo affiliateInfo)
        {
            var ipAddress = Utility.GetVisitorIpAddress();

            // check if user signup email is verified


            var response = CognitoHelper.VerifyResetCode(email, code, password);
            if (response.HttpStatusCode == HttpStatusCode.OK)
            {
                //  add log 
                var logId = _repositoryCustomer.AddLogCognitoResetPassword(response, affiliateInfo.AffiliateStoreFrontFk, email, code,
                                   ipAddress, "success- verify reset code", DateTime.Now, (int)CognitoActionTypeEnum.VerifyResetPassword);
                @ViewBag.ResetPassword = "success";

            }
            else
            {

                // add log
                var logId = _repositoryCustomer.AddLogCognitoResetPassword(response, affiliateInfo.AffiliateStoreFrontFk, email, code,
                              ipAddress, "error- verify reset code", DateTime.Now, (int)CognitoActionTypeEnum.VerifyResetPassword);
                // return view to show invalid url message
                @ViewBag.ResetPassword = "invalid-password";
                ModelState.AddModelError("", response.Message);
                return View("RecoverLogin",
                      new PasswordRecoveryViewModel { EmailProvided = email });
            }

            return View();


        }

        // update customer password
        public ActionResult UpdatePassword(LoginViewModel loginViewModel)
        {
            int userPasswordRecoveryId = 0;
            if (TempData["userPasswordRecoveryId"] != null)
            {
                userPasswordRecoveryId = Convert.ToInt32(TempData["userPasswordRecoveryId"]);
            }

            //encrypt passwd before updating
            loginViewModel.Password = Utility.EncryptCfm(SiteConfigurationsWc.AesEncryptionKey, loginViewModel.Password);

            // update customer password
            var upadteStatus = _repositoryCustomer.UpdateCustomerPassword(loginViewModel.CustomerId, loginViewModel.Password, DateTime.Now);
            var resetStatus = 0;

            // update reset status (0 - default 1- success, 2- link expired)

            if (upadteStatus != 0)
            {
                resetStatus = 1;
                @ViewBag.ResetPassword = "success";
            }

            var passwordRecoveryStatus = _repositoryCustomer.UserPasswordRecoveryResetStatus(userPasswordRecoveryId, loginViewModel.CustomerId, resetStatus, DateTime.Now);

            return View("reset-password");
        }


        // process logout
        public ActionResult Logout()
        {
            Session.Clear();
            Session.Abandon();
            Request.GetOwinContext().Authentication.SignOut("JWTToken");
            //if(Request.Cookies["JWTToken"] != null)
            Response.Cookies.Clear();
            //if (Request.Cookies["JWTToken"] != null)
            //{
            Response.Cookies["JWTToken"].Expires = DateTime.Now.AddDays(-1);
            //}

            FormsAuthentication.SignOut();
            return RedirectToAction("Login", "Customer");
        }

        // my account link from layout page

        public ActionResult Account()
        {
            return Redirect(Url.Action("myaccount"));
        }

        // my account 
        [SessionExpire]
        [Authorize]
        public ViewResult MyAccount(LoggedCustomer loggedCustomer, AffiliateInfo affiliateInfo, int sortOrder = 1, int page = 1)
        {
            @ViewBag.Title = "My Account | " + SiteConfigurationsWc.StorefrontUrl;

            @ViewBag.Description =
                 SiteConfigurationsWc.StorefrontUrl + " is your destination for safe, certified, high quality medications at prices well below those of traditional pharmacies.";

            if (Request.Url != null)
            {
                @ViewBag.canonicalRef = Request.Url.AbsoluteUri;
            }

            /*Customer Credit */
            var customerCredit = _repositoryCustomer.GetCustomerCredit(loggedCustomer.CustomerId);
            ViewBag.CustomerCredit = customerCredit;

            var customerIdDocument = _repositoryCustomer.GetCustomerIdDocumentByCustomerId(loggedCustomer.CustomerId);
            var allOrders = GetCustomerAllOrders(loggedCustomer.CustomerId, loggedCustomer.CustomerStorefrontId);

            if (sortOrder == 1)
                allOrders = allOrders.OrderByDescending(x => x.OrderInvoiceDateCreated).ToList();
            else if (sortOrder == 2)
                allOrders = allOrders.OrderBy(x => x.OrderInvoiceDateCreated).ToList();
            else if (sortOrder == 3)
                allOrders = allOrders.OrderByDescending(x => x.OrderTotal).ToList();
            else if (sortOrder == 4)
                allOrders = allOrders.OrderBy(x => x.OrderTotal).ToList();


            PagingInfo pageInfo = GetPagingInfo(page, allOrders.Count());

            // for refill combine all records for shipping invoice
            if (SiteConfigurationsWc.EnableRefill == 1)
                allOrders = allOrders.GroupBy(x => x.OrderInvoiceId).Select(x => x.FirstOrDefault()).ToList();

            allOrders = allOrders.Skip((page - 1) * PageSize).Take(PageSize).ToList();

            IEnumerable<PatientProfile> patientProfile = _repositoryCustomer.GetPatientProfilesOfCustomer(loggedCustomer.CustomerId,
                                                                   0,
                                                                   affiliateInfo.
                                                                   AffiliateStoreFrontFk);
            if (patientProfile != null) { @ViewBag.IsProfileShow = true; }

            return View(new AccountSectionViewModel
            {
                AllOrders = allOrders,
                LoggedCustomer = loggedCustomer,
                SortOrder = sortOrder,
                PagingInfo = pageInfo,
                UploadDocument = customerIdDocument != null && customerIdDocument.Count > 0 ? true : false

            });
        }


        [AllowAnonymous]
        public JsonResult DoesEmailExist(string customerEmail, AffiliateInfo affiliateInfo)
        {
            // string email = "abc";

            if (customerEmail == null)
            {
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var emailFound = _repositoryCustomer.EmailAlreadyExits(customerEmail, affiliateInfo.AffiliateId);
                return Json(emailFound, JsonRequestBehavior.AllowGet);
            }
        }


        public ActionResult SendSignupEmail(LoggedCustomer loggedCustomer, AffiliateInfo affiliateInfo)
        {

            if (SiteConfigurationsWc.EmailSend)
            {
                // send welcome email to user
                var templateService = new TemplateService();
                var emailHtmlBody = "";

                string templateFilePath =
                    System.Web.HttpContext.Current.Server.MapPath(
                        SiteConfigurationsWc.EmailTemplatePath + "SignupEmail.cshtml");


                if (!templateFilePath.IsNullOrEmpty())
                {
                    emailHtmlBody = templateService.Parse(System.IO.File.ReadAllText(templateFilePath),
                        loggedCustomer,
                        null,
                        null);
                }

                var mailSubject = "Welcome to EnhanceClub";


                _emailSender.SendEmail(loggedCustomer.CustomerEmail,
                    mailSubject,
                    emailHtmlBody,
                    SiteConfigurationsWc.EmailFrom,
                    0);

                // get customer info 
                var customerInfo = _repositoryCustomer.GetCustomerInfoPartial(loggedCustomer.CustomerId);

                var logId = _adminRepository.AddLogSendEmailReminder(0, affiliateInfo.AffiliateStoreFrontFk,
                                                       loggedCustomer.CustomerId, 0, loggedCustomer.CustomerEmail, customerInfo.CustomerPhone, DateTime.Now,
                                                       (int)EmailTypeEnum.SignUp, mailSubject, emailHtmlBody, (int)SourceApplicationEnum.EnhanceClub);
            }

            return Redirect("/get-started-step-3");
        }


        // to get invoice html for styling purpose use this method
        public ActionResult OrderInvoicePdf(int orderId, LoggedCustomer loggedCustomer)
        {
            // get Order Details  
            List<OrderDetail> orderDetail = _repositoryCustomer.GetCustomerOrderDetail(loggedCustomer.CustomerId, loggedCustomer.CustomerStorefrontId, orderId);
            return View("orderinvoiceprint", new OrderInvoiceViewModel { OrderDetailList = orderDetail });
        }

        [SessionExpire]
        [Authorize]
        [ActionName("Account-Info")]
        public ViewResult AccountInfo(LoggedCustomer loggedCustomer, AffiliateInfo affiliateInfo, bool partialProfile = false)
        {
            @ViewBag.Title = "Customer Update Profile Form | Enhance Club  ";


            @ViewBag.Description = "Customer update profile form. Update your existing profile information to keep your profile up to date.";
            CustomerMinimal customerInfo = new CustomerMinimal();

            if (loggedCustomer.CustomerIsPartial)
            {
                // get customer info 
                customerInfo = _repositoryCustomer.GetCustomerInfoPartial(loggedCustomer.CustomerId);
                if (!customerInfo.CustomerIsPartial)
                {
                    // get customer info 
                    customerInfo = _repositoryCustomer.GetCustomerInfo(loggedCustomer.CustomerId);
                }
            }
            else
            {
                // get customer info 
                customerInfo = _repositoryCustomer.GetCustomerInfo(loggedCustomer.CustomerId);
            }

            if (partialProfile)
            {
                ViewBag.ReturnMessage = "Let's complete your profile first to continue with your order.";
            }

            /*Customer Credit */
            var customerCredit = _repositoryCustomer.GetCustomerCredit(loggedCustomer.CustomerId);
            ViewBag.CustomerCredit = customerCredit;

            //customerInfo.CustomerPassword = Utility.DecryptCfm(SiteConfigurationsWc.AesEncryptionKey, customerInfo.CustomerPassword);

            // get province list
            List<Province> provinceList = _repositoryCustomer.GetProvinceListByCountry(SiteConfigurationsWc.StoreFrontCountryId);

            // I am passing ProvinceSelectList and ProvinceList both just to test, we actually need only one
            IEnumerable<PatientProfile> patientProfile = _repositoryCustomer.GetPatientProfilesOfCustomer(loggedCustomer.CustomerId,
                                                                 0,
                                                                 affiliateInfo.
                                                                 AffiliateStoreFrontFk);
            // get customer shipping address
            var customerAddress = _repositoryCustomer.GetCustomerShippingAddress(loggedCustomer.CustomerId, 0);

            if (patientProfile != null) { @ViewBag.IsProfileShow = true; }
            return
                View(new CustomerInfoViewModel
                {
                    CustomerMinimal = customerInfo,
                    ProvinceList = provinceList,
                    ProvinceSelectList = new SelectList(provinceList, "ProvinceId", "ProvinceName"),
                    LoggedCustomer = loggedCustomer,
                    CustomerShippingAddresses = customerAddress
                });
        }

        // process update to account info
        [SessionExpire]
        [HttpPost]
        [ActionName("account-Info")]
        public ActionResult AccountInfo(CustomerMinimal customerInfo, string returnUrl, AffiliateInfo affiliateInfo,
            LoggedCustomer loggedCustomer)
        {
            if (loggedCustomer.CustomerId == 0)
            {
                return RedirectToAction("Login", "Customer");
            }

            if (ModelState.IsValid)
            {
                // get the clean url to check the existing record in database
                customerInfo.CustomerEmail = Utility.StripGmailAddress(customerInfo.CustomerEmail);

                // Check email for duplicates
                var emailFound = _repositoryCustomer.EmailFoundForOtherCustomer(customerInfo.CustomerEmail,
                    affiliateInfo.AffiliateStoreFrontFk, loggedCustomer.CustomerId);

                IEnumerable<PatientProfile> patientProfile = _repositoryCustomer.GetPatientProfilesOfCustomer(loggedCustomer.CustomerId,
                                                                   0,
                                                                   affiliateInfo.
                                                                   AffiliateStoreFrontFk);
                if (patientProfile != null) { @ViewBag.IsProfileShow = true; }

                if (emailFound)
                {
                    ModelState.AddModelError("email", "The Email Provided is already Registered with another account");

                    // get province list
                    List<Province> provinceList = _repositoryCustomer.GetProvinceListByCountry(SiteConfigurationsWc.StoreFrontCountryId);

                    // I am passing ProvinceSelectList and ProvinceList both just to test, we actually need only one

                    StringBuilder mailBody = new StringBuilder();
                    mailBody.Append("<br/> Customer Id: ");
                    mailBody.Append(customerInfo.CustomerId);
                    mailBody.Append(" Encountered duplicate email editing account");

                    mailBody.Append("<br/> Storefront : ");
                    mailBody.Append(SiteConfigurationsWc.StorefrontUrl);
                    mailBody.Append("<br/> Email : ");
                    mailBody.Append(customerInfo.CustomerEmail);


                    // Send Email
                    try
                    {
                        //_emailSender.SendEmail(@SiteConfigurationsWc.DebugMessageRecipient,
                        //            "duplicate email editing account in " + SiteConfigurationsWc.StorefrontUrl, mailBody.ToString(), "",
                        //            0);
                    }
                    catch (Exception e)
                    {
                        throw e;
                    }

                    PopulateProvinceName(customerInfo);
                    PopulateCountryName(customerInfo);

                    return
                        View(new CustomerInfoViewModel
                        {
                            LoggedCustomer = loggedCustomer,
                            CustomerMinimal = customerInfo,
                            ProvinceList = provinceList,
                            ProvinceSelectList = new SelectList(provinceList, "ProvinceId", "ProvinceName"),
                            CustomerShippingAddresses = _repositoryCustomer.GetCustomerShippingAddress(loggedCustomer.CustomerId, 0)
                        });
                }
                else
                {

                    // Date: Jan 13, 2021. commented code to update Customer Password for cognito implementation
                    // customerInfo.CustomerPassword = Utility.EncryptCfm(SiteConfigurationsWc.AesEncryptionKey, customerInfo.CustomerPassword);

                    var ipAddress = Utility.GetVisitorIpAddress();
                    var accessToken = string.Empty;
                    if (Request.Cookies["JWTToken"] != null)
                    {
                        accessToken = Request.Cookies["JWTToken"].Value;
                    }

                    CustomerMinimal customerExistingInfo = null;
                    if (loggedCustomer.CustomerIsPartial)
                    {
                        customerExistingInfo = _repositoryCustomer.GetCustomerInfoPartial(loggedCustomer.CustomerId);
                    }
                    else
                    {
                        // get customer info 
                        customerExistingInfo = _repositoryCustomer.GetCustomerInfo(loggedCustomer.CustomerId);
                    }




                    var customerSignup = new CustomerSignUp
                    {
                        CustomerFirstName = customerInfo.CustomerFirstName,
                        CustomerLastName = customerInfo.CustomerLastName,
                        CustomerAddress = customerInfo.CustomerAddress,
                        CustomerProvinceFk = customerInfo.CustomerProvinceId,
                        CustomerCity = customerInfo.CustomerCity,
                        CustomerZipCode = customerInfo.CustomerZipCode
                    };


                    var result = CognitoHelper.UpdateUserAttributes(loggedCustomer.CustomerEmail, accessToken, customerSignup);

                    if (result.HttpStatusCode == HttpStatusCode.OK)
                    {
                        // add log

                        var logId = _repositoryCustomer.AddLogCognitoUserAttributesResponse(result, customerSignup, affiliateInfo.AffiliateStoreFrontFk,
                                                    loggedCustomer.CustomerId, ipAddress, "success", DateTime.Now, (int)CognitoActionTypeEnum.EditAccount);
                    }
                    else
                    {
                        var logId = _repositoryCustomer.AddLogCognitoUserAttributesResponse(result, customerSignup, affiliateInfo.AffiliateStoreFrontFk,
                                          loggedCustomer.CustomerId, ipAddress, "error", DateTime.Now, (int)CognitoActionTypeEnum.EditAccount);
                    }

                    customerInfo.CustomerEmail = Utility.StripGmailAddress(customerInfo.CustomerEmail);
                    // save the changes to the account info 
                    var updateStatus = _repositoryCustomer.UpdateCustomerInfo(customerInfo);

                    // update customer partial flag                  
                    if (updateStatus == 1 && loggedCustomer.CustomerIsPartial == true)
                        _repositoryCustomer.UpdateCustomerIsPartialFlag(loggedCustomer.CustomerId, false); // set customer is partial flag to false

                    // add address in customer shipping address
                    if (SiteConfigurationsWc.ShowMultipleShippingAddress == 1)
                    {
                        if (customerExistingInfo != null)
                        {
                            if (customerExistingInfo.CustomerProvinceId != customerInfo.CustomerProvinceId)
                            {
                                // deactivate shipping address for previous province
                                var updateActiveStatus = _repositoryCustomer.UpdateCustomerShippingAddressActiveFlag(loggedCustomer.CustomerId, 0, false);
                            }
                        }

                        // check if there is no shipping address added for the customer, then add billing address as default shipping address
                        var customerShippingAddress = _repositoryCustomer.GetCustomerShippingAddress(loggedCustomer.CustomerId, 0);
                        if(customerShippingAddress.Count ==0)
                        {
                            var shipAddressId = _repositoryCustomer.AddCustomerShippingAddress(loggedCustomer.CustomerId, customerInfo.CustomerFirstName,
                                   customerInfo.CustomerLastName, customerInfo.CustomerPhone, customerInfo.CustomerAddress, customerInfo.CustomerCity,
                                   customerInfo.CustomerCountryId,
                                   customerInfo.CustomerProvinceId, customerInfo.CustomerZipCode, true, true, DateTime.Now);
                        }                       

                    }

                    // Start: update customer name is session so that change reflects same time  : 2 Mar 2017
                    loggedCustomer.CustomerFirstName = customerInfo.CustomerFirstName;
                    loggedCustomer.CustomerLastName = customerInfo.CustomerLastName;
                    loggedCustomer.CustomerIsPartial = false;
                    if (System.Web.HttpContext.Current.Session != null)
                    {
                        System.Web.HttpContext.Current.Session["LoggedCustomer"] = loggedCustomer;
                    }
                    // End : update customer name is session so that change reflects same time  : 2 Mar 2017

                    TempData["successMessage"] = "Account successfully updated ";
                    return RedirectToAction(returnUrl ?? Url.Action("account-info", "Customer"));
                }

            }
            else
            {
                ModelState.AddModelError("", "Please complete Account Info");
                // get province list
                List<Province> provinceList = _repositoryCustomer.GetProvinceListByCountry(SiteConfigurationsWc.StoreFrontCountryId);

                PopulateProvinceName(customerInfo);
                PopulateCountryName(customerInfo);

                return
                    View(new CustomerInfoViewModel
                    {
                        LoggedCustomer = loggedCustomer,
                        CustomerMinimal = customerInfo,
                        ProvinceList = provinceList,
                        ProvinceSelectList = new SelectList(provinceList, "ProvinceId", "ProvinceName"),
                        CustomerShippingAddresses = _repositoryCustomer.GetCustomerShippingAddress(loggedCustomer.CustomerId, 0)
                    });
            }
        }

        public ActionResult ResendConfirmationEmail(string customerEmail, AffiliateInfo affiliateInfo)
        {
            var resendCode = CognitoHelper.ResendConfirmationEmail(customerEmail);
            var resendStatus = false;
            var ipAddress = Utility.GetVisitorIpAddress();

            if (resendCode.HttpStatusCode == HttpStatusCode.OK)
            {
                resendStatus = true;

                var logid = _repositoryCustomer.AddLogCognitoSendEmailVerification(resendCode, affiliateInfo.AffiliateStoreFrontFk, customerEmail, ipAddress, "success",
                       DateTime.Now, (int)CognitoActionTypeEnum.UserRequestedEmailResend, true);

            }
            else
            {
                var logid = _repositoryCustomer.AddLogCognitoSendEmailVerification(resendCode, affiliateInfo.AffiliateStoreFrontFk, customerEmail, ipAddress, "error",
                      DateTime.Now, (int)CognitoActionTypeEnum.UserRequestedEmailResend, true);
            }
            return Json(new { status = resendStatus }, JsonRequestBehavior.AllowGet);

        }

        // action method for dashboard. For now my account and dashboard has same code, dashboard may have additional features
        [ActionName("dashboard")]
        [Authorize]
        public ActionResult Dashboard(LoggedCustomer loggedCustomer, AffiliateInfo affiliateInfo, int sortOrder = 1, int page = 1, bool fromLogin = false)
        {
            if (loggedCustomer.CustomerId == 0)
            {
                return RedirectToAction("Login", "Customer");
            }

            @ViewBag.Title = "Dashboard | " + SiteConfigurationsWc.StorefrontUrl;


            @ViewBag.Description =
                 SiteConfigurationsWc.StorefrontUrl + " is your destination for safe, certified, high quality medications at prices well below those of traditional pharmacies.";

            if (Request.Url != null)
            {
                @ViewBag.canonicalRef = Request.Url.AbsoluteUri;
            }

            /*Customer Credit */
            var customerCredit = _repositoryCustomer.GetCustomerCredit(loggedCustomer.CustomerId);
            ViewBag.CustomerCredit = customerCredit;

            var customerIdDocument = _repositoryCustomer.GetCustomerIdDocumentByCustomerId(loggedCustomer.CustomerId);
            var allOrders = GetCustomerAllOrders(loggedCustomer.CustomerId, loggedCustomer.CustomerStorefrontId);

            if (sortOrder == 1)
                allOrders = allOrders.OrderByDescending(x => x.OrderInvoiceDateCreated).ToList();
            else if (sortOrder == 2)
                allOrders = allOrders.OrderBy(x => x.OrderInvoiceDateCreated).ToList();
            else if (sortOrder == 3)
                allOrders = allOrders.OrderByDescending(x => x.OrderTotal).ToList();
            else if (sortOrder == 4)
                allOrders = allOrders.OrderBy(x => x.OrderTotal).ToList();

            PagingInfo pageInfo = GetPagingInfo(page, allOrders.Count());

            // for refill combine all records for shipping invoice
            if(SiteConfigurationsWc.EnableRefill == 1)
                allOrders = allOrders.GroupBy(x => x.OrderInvoiceId).Select(x=>x.FirstOrDefault()).ToList();

            allOrders = allOrders.Skip((page - 1) * PageSize).Take(PageSize).ToList();

            IEnumerable<PatientProfile> patientProfile = _repositoryCustomer.GetPatientProfilesOfCustomer(loggedCustomer.CustomerId,
                                                                   0,
                                                                   affiliateInfo.
                                                                   AffiliateStoreFrontFk);
            if (patientProfile != null) { ViewBag.IsProfileShow = true; }

            return View(new AccountSectionViewModel
            {
                AllOrders = allOrders,
                LoggedCustomer = loggedCustomer,
                SortOrder = sortOrder,
                PagingInfo = pageInfo,
                UploadDocument = customerIdDocument != null && customerIdDocument.Count > 0 ? true : false,
                fromLogin = fromLogin,
                CustomerIsPartial = loggedCustomer.CustomerIsPartial

            });
        }


        //get all orders of logged in customer
        private List<OrderStatus> GetCustomerAllOrders(int customerId, int storefrontFk)
        {
            // get open orders for the customer
            List<OrderStatus> openOrders = _repositoryCustomer.GetCustomerOpenOrders(customerId, storefrontFk);

            // get closed orders for the customer
            List<OrderStatus> closedOrders = _repositoryCustomer.GetCustomerClosedOrders(customerId, storefrontFk);

            var allOrders = openOrders.Concat(closedOrders).ToList();

            return allOrders;
        }

        // method submits general enquiry
        
        public async Task<ActionResult> AddGeneralEnquiry(string customerName, string customerEmail, string customerPhone,
                                    string queryPosted, string ddlCategory, string captchaToken, AffiliateInfo affiliateInfo)
        {

            if (SiteConfigurationsWc.EnableCaptcha == 1)
            {
                if (captchaToken != null)
                {
                    var isCaptchaValid = await IsCaptchaValid(captchaToken);

                    if (!isCaptchaValid)
                    {
                        // send email 
                        ViewBag.GoogleCaptchaMessage = "Error submitting your query, Please contact the support team at";
                        ViewBag.GoogleCaptchaSupportNumber = " 1-844-836-2582.";

                        if (SiteConfigurationsWc.IsZendeskActive == 1)
                        {
                            var zendeskCategory = await ZendeskService.GetZendeskCategoryFieldValue();

                            if (string.IsNullOrEmpty(zendeskCategory.Value))
                            {
                                ZendeskCategoryModel zendeskCategoryModel = zendeskCategory.Key;

                                return View("get-in-touch", new GetInTouchViewModel
                                {
                                    CaptchaSiteKey = @SiteConfigurationsWc.RecaptchaSiteKey,
                                    custom_field_options = zendeskCategoryModel.ticket_field.custom_field_options
                                });
                            }
                        }
                        else
                        {
                            return View("get-in-touch", new GetInTouchViewModel { CaptchaSiteKey = @SiteConfigurationsWc.RecaptchaSiteKey });
                        } 
                    }
                }
            }


            queryPosted = Utility.StripSpecialChar(queryPosted);
            customerName = Utility.StripSpecialChar(customerName);
            customerPhone = Utility.StripSpecialChar(customerPhone);
            @ViewBag.Title = "Enhance Club Enquiry Submit";

            var mailSubject = "Query from Customer: " + customerName;
            string zendeskTicketId = null;

            if (SiteConfigurationsWc.IsZendeskActive == 1)
            {
                ZendeskTicketModel zenDeskModel = new ZendeskTicketModel();

                ZendeskComment comment = new ZendeskComment();
                comment.body = queryPosted + " \n  " + " Customer Phone Number : " + customerPhone;

                ZendeskRequester zendeskRequester = new ZendeskRequester();
                zendeskRequester.name = customerName;
                zendeskRequester.email = customerEmail;

                List<ZendeskCustomField> zendeskCustomFieldList = new List<ZendeskCustomField>();
                ZendeskCustomField zendeskCustomField = new ZendeskCustomField();
                zendeskCustomField.id = Convert.ToInt64(SiteConfigurationsWc.ZendeskCategoryFieldId);
                zendeskCustomField.value = ddlCategory;
                zendeskCustomFieldList.Add(zendeskCustomField);

                if (!string.IsNullOrEmpty(customerPhone))
                {
                    customerPhone = new string(customerPhone.Where(c => char.IsDigit(c)).ToArray());
                }

                zendeskCustomField = new ZendeskCustomField();
                zendeskCustomField.id = Convert.ToInt64(SiteConfigurationsWc.ZendeskPhoneFieldId);
                zendeskCustomField.value = customerPhone;
                zendeskCustomFieldList.Add(zendeskCustomField);

                ZendeskRequest zendeskRequest = new ZendeskRequest();
                zendeskRequest.comment = comment;
                zendeskRequest.subject = mailSubject;
                zendeskRequest.requester = zendeskRequester;
                zendeskRequest.custom_fields = zendeskCustomFieldList;

                zenDeskModel.request = zendeskRequest;

                var result = await ZendeskService.CreateUserZendeskTicket(zenDeskModel, customerEmail);

                if (!result.Value)
                {
                    ViewBag.ZendeskMessage = result.Key;
                }
                else
                {
                    ZendeskTicketResponse zendeskTicketResponse = JsonConvert.DeserializeObject<ZendeskTicketResponse>(result.Key);
                    zendeskTicketId = zendeskTicketResponse.request.id.ToString();
                }

                if (!result.Value)
                {
                    var zendeskCategory = await ZendeskService.GetZendeskCategoryFieldValue();
                    ZendeskCategoryModel zendeskCategoryModel = zendeskCategory.Key;                     

                    return View("get-in-touch", new GetInTouchViewModel { CaptchaSiteKey = @SiteConfigurationsWc.RecaptchaSiteKey,
                        custom_field_options = zendeskCategoryModel.ticket_field.custom_field_options
                    });
                }

                /* Updated user contact details */

                ZendeskUser zendeskUser = new ZendeskUser();
                zendeskUser.phone = customerPhone;
                zendeskUser.id = null;

                ZendeskUserModel zendeskUserModel = new ZendeskUserModel();
                zendeskUserModel.user = zendeskUser;

                await ZendeskService.AddUserContactOnZendesk(zendeskUserModel, customerEmail);
            }

            var generalEnquiry = _repositoryCustomer.AddGeneralEnquiry(affiliateInfo.AffiliateStoreFrontFk, customerName,
                                                                    customerEmail, customerPhone, queryPosted, DateTime.Now, ddlCategory, zendeskTicketId);
            GetInTouchEmail getintoucemail = new GetInTouchEmail
            {
                Name = customerName,
                Email = customerEmail,
                Question = queryPosted,
                Phone = customerPhone
            };

            if (SiteConfigurationsWc.IsZendeskActive == 0)
            {
                if (SiteConfigurationsWc.EmailSend)
                {
                    // send welcome email to user
                    var templateService = new TemplateService();
                    var emailHtmlBody = "";

                    string templateFilePath =
                        System.Web.HttpContext.Current.Server.MapPath(
                            SiteConfigurationsWc.EmailTemplatePath + "GetInTouchEmail.cshtml"); // to do use updated template


                    if (!templateFilePath.IsNullOrEmpty())
                    {
                        emailHtmlBody = templateService.Parse(System.IO.File.ReadAllText(templateFilePath),
                            getintoucemail,
                            null,
                            null);
                    }


                    _emailSender.GetInTouchSendEmail(SiteConfigurationsWc.StorefrontEnquiry,
                        mailSubject,
                        emailHtmlBody,
                        SiteConfigurationsWc.GetInTouchMailFromAddress,
                        0);

                    //// add send email log
                    //var logId = _adminRepository.AddLogSendEmailReminder(0, affiliateInfo.AffiliateStoreFrontFk,
                    //                            customerInfo.CustomerId, 0, customerInfo.CustomerEmail, customerInfo.CustomerPhone, DateTime.Now,
                    //                            (int)EmailTypeEnum.SignUp, mailSubject, emailHtmlBody, (int)SourceApplicationEnum.EnhanceClub);

                }
            }

            if (generalEnquiry > 0)
            {
                return RedirectToAction("thank-you");
            }

            return View("get-in-touch", new GetInTouchViewModel { CaptchaSiteKey = @SiteConfigurationsWc.RecaptchaSiteKey });
        }

        [ActionName("thank-you")]
        public ActionResult ThankYou()
        {
            return View();
        }
        private async Task<bool> IsCaptchaValid(string response)
        {
            try
            {
                var secretKey = SiteConfigurationsWc.RecaptchaSecretKey;

                using (var client = new HttpClient())
                {
                    var values = new Dictionary<string, string>
                    {
                        {"secret", secretKey},
                        {"response", response},
                        {"remoteip", Request.UserHostAddress}
                    };

                    var content = new FormUrlEncodedContent(values);
                    var verify = await client.PostAsync(SiteConfigurationsWc.RecaptchaTokenVerifyUri, content);
                    var captchaResponseJson = await verify.Content.ReadAsStringAsync();
                    var captchaResult = JsonConvert.DeserializeObject<CaptchaResponseViewModel>(captchaResponseJson);

                    return captchaResult.Success
                           && captchaResult.Action == "getintouch"
                           && captchaResult.Score > SiteConfigurationsWc.RecaptchaScore;
                }
            }
            catch (Exception ex)
            {
                return false;
            }

        }

        // add Profile
        public ActionResult AddProfile(string returnUrl, LoggedCustomer loggedCustomer, AffiliateInfo affiliateInfo, bool havePrescription)
        {
            if (loggedCustomer.CustomerId == 0)
            {
                return RedirectToAction("Login", "Customer");
            }

            if (loggedCustomer.CustomerId == 0)
                return RedirectToAction("get-started");
            @ViewBag.Title = "Add a Patient Profile | " + SiteConfigurationsWc.StorefrontName;
            @ViewBag.ReturnUrl = returnUrl;

            PatientProfile patientProfile = new PatientProfile();
            IEnumerable<PatientProfile> patientProfileList = _repositoryCustomer.GetPatientProfilesOfCustomer(loggedCustomer.CustomerId,
                                                                   0,
                                                                   affiliateInfo.
                                                                   AffiliateStoreFrontFk);

            //comment: patient consultation; March 24,2021
            //List<ConsultationHours> consultationHourList = _repositoryCustomer.GetConsultationHours(loggedCustomer.CustomerId);

            CustomerMinimal customerInfo = _repositoryCustomer.GetCustomerInfo(loggedCustomer.CustomerId);
            var socialHistoryList = _repositoryCustomer.GetSocialHistoryList().ToList();
            if (patientProfileList != null)
            {
                patientProfile = patientProfileList.FirstOrDefault();
                if (!string.IsNullOrEmpty(patientProfile.PatientProfileSocialHistory))
                {
                    IEnumerable<int> selectedSocialHistoryList = patientProfile.PatientProfileSocialHistory.Split(',').Select(Int32.Parse).ToList();
                    foreach (var selectedVal in selectedSocialHistoryList)
                    {
                        socialHistoryList.Where(d => d.SocialHistoryId == selectedVal).ToList().ForEach(x => x.IsSelected = true);
                    }
                }


                return View("ReviewPatientProfile", new CustomerProfileViewModel
                {
                    PatientProfile = patientProfile,
                    MedicationList = new List<PatientMedication>(),
                    havePrescription = havePrescription == true ? true : false,
                    SocialHistoryList = socialHistoryList
                    // ConsultationHours= consultationHourList //comment: patient consultation; March 24,2021
                });
            }

            patientProfile.PatientProfileFirstName = customerInfo.CustomerFirstName;
            patientProfile.PatientProfileLastName = customerInfo.CustomerLastName;
            patientProfile.CustomerProvinceCode = customerInfo.CustomerProvinceCode;
            return View("PatientProfile", new CustomerProfileViewModel
            {
                PatientProfile = patientProfile,
                MedicationList = new List<PatientMedication>(),
                havePrescription = havePrescription == true ? true : false,
                SocialHistoryList = socialHistoryList
                //ConsultationHours = consultationHourList //comment: patient consultation; March 24,2021
            });
        }

        [HttpPost]
        public ActionResult AddProfile(PatientProfile patientProfile,
                                       IEnumerable<PatientMedication> medicationList,
                                       AffiliateInfo affiliateInfo,
                                       LoggedCustomer loggedCustomer,
                                       string returnUrl, bool consentChanged, bool checkedValue,
                                       IEnumerable<ConsultationHours> consultationHours,
                                       IEnumerable<SocialHistory> socialHistoryList)
        {
            if (loggedCustomer.CustomerId == 0)
            {
                return RedirectToAction("Login", "Customer");
            }

            @ViewBag.ReturnUrl = returnUrl;
            if (ModelState.IsValid)
            {
                // Comment: #IpAddress - get customer ip address 
                var customerLastIp = CommonFunctions.GetVisitorIpAddress();
                patientProfile.PatientProfileConsultationConsent = checkedValue;

                // changes to save social history for add new profile
                if (socialHistoryList != null && socialHistoryList.Count() > 0)
                {
                    IEnumerable<int> selectedSocialHistoryList = socialHistoryList.Where(x => x.IsSelected).Select(x => x.SocialHistoryId);
                    patientProfile.PatientProfileSocialHistory = string.Join(",", selectedSocialHistoryList);
                }

                // add patient profile
                int newProfileId = 0;
                newProfileId = _repositoryCustomer.AddPatientProfile(patientProfile, loggedCustomer.CustomerId, loggedCustomer.CustomerName, loggedCustomer.CustomerEmail, customerLastIp, DateTime.Now);

                //comment: patient consultation; March 24,2021
                //add consulation details
                /* if (consultationHours != null)
                 {
                     List<ConsultationHours> preferdHours = consultationHours.Where(x => x.IsSelected).ToList();
                     foreach (ConsultationHours hoursSelected in preferdHours)
                     {
                         //check if active record does not exist , then add new 
                         int consentDetailId = _repositoryCustomer.AddPatientConsultationHours(hoursSelected.ConsultationHoursId, loggedCustomer.CustomerId, true, DateTime.Now, null);

                     }
                 }*/

                //Check if  customer is coming from Order Confirm page
                var checkLastOrderStamp = DateTime.Now;
                List<CustomerLastOrder> customerLastOrderList = _repositoryCustomer.GetCustomerLastOrder(affiliateInfo.AffiliateStoreFrontFk, loggedCustomer.CustomerId, checkLastOrderStamp, 15);
                CustomerLastOrder customerLastOrder = null;

                if (customerLastOrderList.Count > 0)
                {
                    foreach (var order in customerLastOrderList)
                    {
                        var cartUpdateStatus = _repositoryCustomer.UpdateCartItemPatientProfile(order.OrderInvoiceId, newProfileId);
                    }


                    // customerLastOrder = customerLastOrderList.FirstOrDefault();
                }
                //comment: patient consultation; March 24,2021
                /*
                if (customerLastOrder != null)
                {
                    var cartUpdateStatus = _repositoryCustomer.UpdateCartItemPatientProfile(customerLastOrder.OrderInvoiceId, newProfileId);


                    //if (consentChanged)
                    {
                        //update order consent                       
                       // int updateStatus = _repositoryCustomer.UpdateOrderConsultationFlag(customerLastOrder.OrderInvoiceId, patientProfile.PatientProfileConsultationConsent);

                        // save consent changed log
                       // var logConsentId = _repositoryCustomer.AddLogCosultationConsent(patientProfile.PatientProfileConsultationConsent, newProfileId, affiliateInfo.AffiliateStoreFrontFk, "Add", DateTime.Now);
                    }
                }   */
            }
            else
            {
                ModelState.AddModelError("", "Please complete profile Info");
                var socialHistory = _repositoryCustomer.GetSocialHistoryList().ToList();
                return View("ReviewPatientProfile", new CustomerProfileViewModel
                {
                    PatientProfile = patientProfile,
                    SocialHistoryList = socialHistory
                });
            }

            if (String.IsNullOrEmpty(returnUrl))
            {
                return Redirect(Url.Action("myaccount"));
            }
            else
            {
                return Redirect(returnUrl);
            }

        }

        private PagingInfo GetPagingInfo(int page, int total)
        {
            PagingInfo pageInfo = new PagingInfo
            {
                CurrentPage = page,
                ItemsPerPage = PageSize,
                TotalItems = total
            };
            if (Request.Url != null)
            {
                string sPage = "page" + page;
                string pPage = "page" + (page - 1);
                string nPage = "page" + (page + 1);

                if (page == 1 && pageInfo.TotalPages > 1)
                {
                    if (Request.Url.AbsoluteUri.EndsWith("1"))
                    {
                        @ViewBag.canonicalNext = Request.Url.AbsoluteUri.Replace("1", "2");
                    }
                    else
                    {
                        @ViewBag.canonicalNext = Request.Url.AbsoluteUri + "/page2";
                    }

                }

                if (page > 1 && pageInfo.TotalPages > page)
                {
                    @ViewBag.canonicalPrev = Regex.Replace(Request.Url.AbsoluteUri, sPage, pPage, RegexOptions.IgnoreCase);
                    @ViewBag.canonicalNext = Regex.Replace(Request.Url.AbsoluteUri, sPage, nPage, RegexOptions.IgnoreCase);
                }

                if (page > 1 && pageInfo.TotalPages == page)
                {
                    @ViewBag.canonicalPrev = Regex.Replace(Request.Url.AbsoluteUri, sPage, pPage, RegexOptions.IgnoreCase);

                }

            }
            return pageInfo;

        }

        public ActionResult EditPatientProfile(LoggedCustomer loggedCustomer, AffiliateInfo affiliateInfo)
        {
            if (loggedCustomer.CustomerId == 0)
            {
                return RedirectToAction("Login", "Customer");
            }

            @ViewBag.Title = "Update Patient Profile | " + SiteConfigurationsWc.StorefrontUrl;
            @ViewBag.ReturnUrl = "EditPatientProfile";

            PatientProfile profileToUpdate = null;
            List<PatientMedication> medicationList = null;

            //comment: patient consultation; March 24,2021
            //List<ConsultationHours> consultationHourList = _repositoryCustomer.GetConsultationHours(loggedCustomer.CustomerId);

            // get profile data for profile id
            IEnumerable<PatientProfile> patientProfile = _repositoryCustomer.GetPatientProfilesOfCustomer(loggedCustomer.CustomerId,
                                                                    0,
                                                                    affiliateInfo.
                                                                    AffiliateStoreFrontFk);
            var socialHistoryList = _repositoryCustomer.GetSocialHistoryList().ToList();

            /*Customer Credit */
            var customerCredit = _repositoryCustomer.GetCustomerCredit(loggedCustomer.CustomerId);
            ViewBag.CustomerCredit = customerCredit;

            if (patientProfile != null)
            {
                @ViewBag.IsProfileShow = true;
                profileToUpdate = patientProfile.FirstOrDefault();
                if (!string.IsNullOrEmpty(profileToUpdate.PatientProfileSocialHistory))
                {
                    IEnumerable<int> selectedSocialHistoryList = profileToUpdate.PatientProfileSocialHistory.Split(',').Select(Int32.Parse).ToList();
                    foreach (var selectedVal in selectedSocialHistoryList)
                    {
                        socialHistoryList.Where(d => d.SocialHistoryId == selectedVal).ToList().ForEach(x => x.IsSelected = true);
                    }
                }


                //comment: patient consultation; March 24,2021
                //profileToUpdate.ConsultationHours = consultationHourList;

                return View("EditPatientProfile", new CustomerProfileViewModel
                {
                    PatientProfile = profileToUpdate,
                    MedicationList = medicationList,
                    SocialHistoryList = socialHistoryList,
                    //ConsultationHours= consultationHourList //comment: patient consultation; March 24,2021
                });
            }
            return Redirect("account-info");
        }
        // update customer patient profile

        [SessionExpire]
        [HttpPost]
        public ActionResult UpdateProfile(PatientProfile patient, IEnumerable<PatientMedication> medicationList,
            AffiliateInfo affiliateInfo, LoggedCustomer loggedCustomer, string returnUrl, int patientProfileId,
            bool consentChanged, bool checkedValue, IEnumerable<ConsultationHours> consultationHours, IEnumerable<SocialHistory> socialHistoryList)
        {
            if (loggedCustomer.CustomerId == 0)
            {
                return RedirectToAction("Login", "Customer");
            }

            // Comment: #IpAddress - get customer ip address 
            var customerLastIp = "";
            patient.PatientProfileConsultationConsent = checkedValue;

            var socialHistorySelection = string.Empty;
            if (socialHistoryList != null && socialHistoryList.Count() > 0)
            {
                IEnumerable<int> selectedSocialHistoryList = socialHistoryList.Where(x => x.IsSelected).Select(x => x.SocialHistoryId);
                patient.PatientProfileSocialHistory = string.Join(",", selectedSocialHistoryList);
            }
            // update Patient profile
            int profileUdated = _repositoryCustomer.UpdateProfile(patient, affiliateInfo.AffiliateStoreFrontFk, patientProfileId, loggedCustomer.CustomerId,
                loggedCustomer.CustomerName, loggedCustomer.CustomerEmail, customerLastIp,
                DateTime.Now);


            //Check if  customet is coming from Order Confirm page
            var checkLastOrderStamp = DateTime.Now;
            List<CustomerLastOrder> customerLastOrderList = _repositoryCustomer.GetCustomerLastOrder(affiliateInfo.AffiliateStoreFrontFk, loggedCustomer.CustomerId, checkLastOrderStamp, 15);

            // commented code to update consultation hours
            //get all active consultation hours
            /*List<PatientConsultationHours> patientPreferedConsultationHour = _repositoryCustomer.GetPatientPreferedConsultationHour(0, 0, loggedCustomer.CustomerId);
            
           
            if (consentChanged)
            {              
                if (checkedValue)
                {
                    //check if any consultation hours selected
                    if (consultationHours.Any(m => m.IsSelected))
                    {                        
                        List<ConsultationHours> preferdHours = consultationHours.Where(x => x.IsSelected).ToList();
                        foreach (ConsultationHours hoursSelected in preferdHours)
                        {
                            //then add preferd consultation hours to db
                            int consentDetailId = _repositoryCustomer.AddPatientConsultationHours(hoursSelected.ConsultationHoursId, loggedCustomer.CustomerId, true, DateTime.Now, null);

                        }
                    }
                }
                else
                {
                    // if patient selected No to consultation then deactivate existing hours and update end date
                    if (patientPreferedConsultationHour != null)
                    {
                        IEnumerable<int> patientPreferedConsultationHourList = patientPreferedConsultationHour.Select(x => x.PatientConsultationDetailsId);

                        string deactivateConsulationIds = string.Join(",", patientPreferedConsultationHourList);
                       
                        // inactive all prefered hours to the customer
                        int updateStatus = _repositoryCustomer.InActivatePatientConsultationHours(loggedCustomer.CustomerId, deactivateConsulationIds, DateTime.Now);
                    }
                }
            if (customerLastOrderList.Count > 0)
                {
                    //update order consent
                    CustomerLastOrder customerLastOrder = customerLastOrderList.FirstOrDefault();
                    int updateStatus = _repositoryCustomer.UpdateOrderConsultationFlag(customerLastOrder.OrderInvoiceId, checkedValue);
                }
                // save consent changed log
                var logConsentId = _repositoryCustomer.AddLogCosultationConsent(patient.PatientProfileConsultationConsent, patient.PatientProfileId, affiliateInfo.AffiliateStoreFrontFk, "Update", DateTime.Now);
*/
            //}
            // case when consent option not changed, may update new timings 
            /*if (patientPreferedConsultationHour != null)
            {
                if (checkedValue)
                {

                    IEnumerable<int> patientPreferedConsultationHourList = patientPreferedConsultationHour.Select(x => x.PatientConsultationHoursFk);
                IEnumerable<int> newConsultationHoursSelected = consultationHours.Where(x => x.IsSelected).Select(x => x.ConsultationHoursId);

                    // checks if patient selected any other timings
                    if ((patientPreferedConsultationHourList.Count() != newConsultationHoursSelected.Count() ||
                   patientPreferedConsultationHourList.Except(newConsultationHoursSelected).Any()))
                    {

                        // if so deactivate existing hours and add newly updated hours
                        IEnumerable<int> deactivateIds = patientPreferedConsultationHour.Select(x => x.PatientConsultationDetailsId);

                        string deactivateConsulationIds = string.Join(",", deactivateIds);
                        
                        // deactivate all prefered hours to the customer
                        int updateStatus = _repositoryCustomer.InActivatePatientConsultationHours(loggedCustomer.CustomerId, deactivateConsulationIds, DateTime.Now);

                        //adding prefered consultation hours
                        List<ConsultationHours> preferdHours = consultationHours.Where(x => x.IsSelected).ToList();
                        foreach (ConsultationHours hoursSelected in preferdHours)
                        {
                            int consentDetailId = _repositoryCustomer.AddPatientConsultationHours(hoursSelected.ConsultationHoursId, loggedCustomer.CustomerId, true, DateTime.Now, null);

                        }
                    }
                }
            }*/
            TempData["message"] = "Profile successfully updated ";

            return Redirect(returnUrl);

        }

        public ActionResult RefillProducts(LoggedCustomer loggedCustomer)
        {
            if (loggedCustomer.CustomerId == 0)
            {
                return RedirectToAction("Login", "Customer");
            }

            var refillProductsList = _repositoryCustomer.GetCustomerAllSubscription(loggedCustomer.CustomerId);
            return View("refill-products", new RefillProductsViewModel
            {
                AutoFillProductsList = refillProductsList
            });
        }

        public ActionResult UnsubscribeSubscription(LoggedCustomer loggedCustomer, int subscriptionId)
        {
            var updateStatus = _repositoryCustomer.UnSubscribeAutoRefill(subscriptionId);
            _repositoryCustomer.AddLogCustomerSubscription(loggedCustomer.CustomerId, subscriptionId, false, "Update", DateTime.Now);
            return RedirectToAction("RefillProducts");
        }

        [ActionName("id-documents")]
        public ActionResult GetCustomerDocuments(LoggedCustomer loggedCustomer, AffiliateInfo affiliateInfo)
        {
            if (loggedCustomer.CustomerId == 0)
            {
                return RedirectToAction("Login", "Customer");
            }

            @ViewBag.Title = "Upload/Replace Customer ID Documents";
            @ViewBag.MetaRobot = "noindex,follow";

            bool idImageFound = false;
            bool backIdImageFound = false;
            string idImageFileName = "";
            string idBackImageName = "";

            /*Customer Credit */
            var customerCredit = _repositoryCustomer.GetCustomerCredit(loggedCustomer.CustomerId);
            ViewBag.CustomerCredit = customerCredit;


            List<CustomerIdDocument> customerDocumentList = _repositoryCustomer.GetCustomerIdDocumentByCustomerId(loggedCustomer.CustomerId);

            if (customerDocumentList != null)
            {
                idImageFound = true;
                idImageFileName = customerDocumentList[0].CustomerIdDocumentFileName;
                idBackImageName = customerDocumentList[0].CustomerIdDocumentBackSideFileName;
                if (idBackImageName != null) { backIdImageFound = true; } else { backIdImageFound = false; }
                @ViewBag.CustomerDocumentValidated = customerDocumentList[0].CustomerIdDocumentIsValid == true ? true : false;
            }
            else { @ViewBag.idImageFound = false; @ViewBag.backIdImageFound = false; @ViewBag.idImageFileName = ""; @ViewBag.CustomerDocumentValidated = false; }

            IEnumerable<PatientProfile> patientProfile = _repositoryCustomer.GetPatientProfilesOfCustomer(loggedCustomer.CustomerId,
                                                                  0,
                                                                  affiliateInfo.
                                                                  AffiliateStoreFrontFk);
            if (patientProfile != null) { @ViewBag.IsProfileShow = true; }

            return View("CustomerIdDocuments", new PrescriptionUploadViewModel
            {

                PrescriptionImageFound = idImageFound,
                IdImageName = idImageFileName,
                IdBackImageName = idBackImageName,
                CustomerDocumentList = customerDocumentList,
                backIdImageFound = backIdImageFound

            });
        }

        public ActionResult UpdateShippingAddress(LoggedCustomer loggedCustomer, int shippingAdressFk, string customerFirstName, string customerLastName,
                string shippingAddress, string shippingCity, string shippingZipcode, int shippingProvinceId, int shippingCountryId)
        {
            // check for duplicate shipping address
            var customerOriginalShippingAddresses = _repositoryCustomer.GetCustomerShippingAddress(loggedCustomer.CustomerId, shippingAdressFk).FirstOrDefault();

            var updateStatus = _repositoryCustomer.UpdateShippingAddress(shippingAdressFk, customerFirstName, customerLastName,
                                        shippingAddress, shippingCity, shippingZipcode, shippingProvinceId, shippingCountryId);
            var updatedShipAddress = new CustomerShippingAddress
            {
                CustomerFirstName = customerFirstName,
                CustomerLastName = customerLastName,
                CustomerAddress = shippingAddress,
                CustomerCity = shippingCity,
                ShippingZipCode = shippingZipcode,
                ShippingProvinceId = shippingProvinceId,
                CustomerCountryId = shippingCountryId
            };

            List<LogCustomerShippingAddress> customerAddressLog = GetCustomerShippingAddressLog(loggedCustomer.CustomerId, shippingAdressFk,
                                                                "Update", customerOriginalShippingAddresses, updatedShipAddress);
            var logId = _repositoryCustomer.AddLogCustomerShippingAddress(customerAddressLog);

            return RedirectToAction("account-info");
        }


        [HttpPost]
        public ActionResult AddShippingAddress(LoggedCustomer loggedCustomer, CustomerShippingAddress shippingAddress)
        {
            // check for duplicate shipping address
            var customerShippingAddresses = _repositoryCustomer.GetCustomerShippingAddress(loggedCustomer.CustomerId, 0);

            var existingAddress = customerShippingAddresses.Where(x => x.CustomerAddress.Equals(shippingAddress.CustomerAddress, StringComparison.OrdinalIgnoreCase) &&
                                    x.CustomerCity.Equals(shippingAddress.CustomerCity, StringComparison.OrdinalIgnoreCase)
                                    && x.CustomerZipCode.Equals(shippingAddress.CustomerZipCode, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();

            if (existingAddress == null)
            {
                var shipAddressId = _repositoryCustomer.AddCustomerShippingAddress(loggedCustomer.CustomerId, shippingAddress.CustomerFirstName,
                shippingAddress.CustomerLastName, shippingAddress.CustomerPhone, shippingAddress.CustomerAddress, shippingAddress.CustomerCity, shippingAddress.CustomerCountryId,
                shippingAddress.ShippingProvinceId, shippingAddress.CustomerZipCode, true, shippingAddress.CustomerDefaultAddress, DateTime.Now);
                if (shipAddressId > 0)
                {
                    List<LogCustomerShippingAddress> customerAddressLog = GetCustomerShippingAddressLog(loggedCustomer.CustomerId, shipAddressId, 
                                                                "Add", new CustomerShippingAddress(), shippingAddress);
                    var logId = _repositoryCustomer.AddLogCustomerShippingAddress(customerAddressLog);
                    return RedirectToAction("account-info");
                }
                // add error info        
                TempData["message"] = "Failed to add shipping address";
            }
            else
            {
                // add error info        
                TempData["message"] = "Shipping Address already exist.";
            }
            return RedirectToAction("account-info");
        }

        public ActionResult UpdateCustomerShippingAddressDefaultFlag(LoggedCustomer loggedCustomer, int shippingAdressFk)
        {
            // check for duplicate shipping address
            var customerOriginalShippingAddresses = _repositoryCustomer.GetCustomerShippingAddress(loggedCustomer.CustomerId, shippingAdressFk).FirstOrDefault();

            var updateStatus = _repositoryCustomer.UpdateCustomerShippingAddressDefaultFlag(loggedCustomer.CustomerId, shippingAdressFk, true);

            // get all shipping address
            var customerShippingAddresses = _repositoryCustomer.GetCustomerShippingAddress(loggedCustomer.CustomerId, 0);

            var updatedShipAddress = customerShippingAddresses.Where(x => x.CustomerAddressId == shippingAdressFk).FirstOrDefault();

            List<LogCustomerShippingAddress> customerAddressLog = GetCustomerShippingAddressLog(loggedCustomer.CustomerId, shippingAdressFk,
                                                                "Update", customerOriginalShippingAddresses, updatedShipAddress);
            var logId = _repositoryCustomer.AddLogCustomerShippingAddress(customerAddressLog);

            return PartialView("_shippingAddress",
                new ShippingAddressViewModel
                {
                    CustomerShippingAddress = customerShippingAddresses,
                    ProvinceList = _repositoryCustomer.GetProvinceListByCountry(SiteConfigurationsWc.StoreFrontCountryId),
                    AllowEdit = true,
                    ShippingAddressProvinceFk = customerShippingAddresses.Where(x=>x.CustomerDefaultAddress).Select(x=>x.ShippingProvinceId).FirstOrDefault()
                });

           // return Json(true, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetCustomerOpenOrder(LoggedCustomer loggedCustomer, AffiliateInfo affiliateInfo)
        {
            // get customer Open Orders and exiting province
            var openOrders = _repositoryCustomer.GetCustomerOpenOrders(loggedCustomer.CustomerId, affiliateInfo.AffiliateStoreFrontFk);
            var orderCount = 0;
            var orderInvoiceId = string.Empty;
          // List<int> orderInvoiceList = new List<int>();
            if (openOrders !=null)
            {
                orderCount = openOrders.Count;
               var orderInvoiceList = openOrders.Select(x => new { orderId = x.OrderInvoiceId, shippingprovince = x.OrderInvoiceShippingProvinceName }).ToList();

                foreach (var orderInvoice in orderInvoiceList)
                {
                    orderInvoiceId = orderInvoiceId + orderInvoice.orderId + "(" + orderInvoice.shippingprovince + "), ";
                }
                //orderInvoiceList = openOrders.Select(x => x.OrderInvoiceId).ToList();
               
               
            }
            return Json(new { orderCount = orderCount, orderInvoiceId = orderInvoiceId }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult DeleteShippingAddress(LoggedCustomer loggedCustomer, int shippingAddressFk)
        {
            var deleteStatus = _repositoryCustomer.UpdateCustomerShippingAddressActiveFlag(loggedCustomer.CustomerId, shippingAddressFk, false);

            return RedirectToAction("account-info");
        }

        [HttpGet]
        public ActionResult UpdateCustomerCredentials()
        {
            return View("update-customer-credential");
        }

        [ActionName("update-customer-credential")]
        public ActionResult UpdateCustomerCredentials(LoggedCustomer loggedCustomer, string newEmail)
        {
            var accessToken = string.Empty;
            if (Request.Cookies["JWTToken"] != null)
            {
                accessToken = Request.Cookies["JWTToken"].Value;
            }

            var updateEmailResponse = CognitoHelper.UpdateUserEmail(newEmail, accessToken);

            // to do add log

            if (updateEmailResponse.HttpStatusCode == HttpStatusCode.OK)
            {
                // mark email verified flag as false

                var emailVerifiedStatus = _repositoryCustomer.UpdateCustomerSignUpEmailVerifiedFlag(loggedCustomer.CustomerId, false);
                @ViewBag.UpdatedValue = newEmail;
                return View("verify-customer-credential");
            }
            else
            {
                // to do show custom message
                @ViewBag.ErrorMessage = "Failed to updated email address";
            }

            // pass some parameter to show error
            return View("update-customer-credential");

        }

        public ActionResult VerifyCustomerCredentials(LoggedCustomer loggedCustomer, string updatedValue,
            string verificationCode, string attribute = "email")
        {
            var accessToken = string.Empty;
            if (Request.Cookies["JWTToken"] != null)
            {
                accessToken = Request.Cookies["JWTToken"].Value;
            }
            var response = CognitoHelper.VerifyUserEmailAttribute(verificationCode, accessToken, attribute);
            if (response.HttpStatusCode == HttpStatusCode.OK)
            {
                ViewBag.EmailUpdateMessage = "Email updated successfully";
                // update customer email in database and update customer email in customer table
                var emailVerifiedStatus = _repositoryCustomer.UpdateCustomerSignUpEmailVerifiedFlag(loggedCustomer.CustomerId, true);
                var updatedPhone = string.Empty;
                var updatedEmail = string.Empty;
                if (attribute == "email")
                {
                    updatedEmail = updatedValue;
                }
                var updateEmail = _repositoryCustomer.UpdateCustomerCredentials(loggedCustomer.CustomerId, updatedEmail, updatedPhone);

                // to do add log

                ViewBag.EmailUpdateMessage = "Email updated successfully";

                return RedirectToAction("logout");

            }
            ViewBag.EmailUpdateMessage = "Failed to verify authentication code.";
            return View("verify-customer-credential");


        }

        private List<LogCustomerShippingAddress> GetCustomerShippingAddressLog(int customerId, int customerAddressFk, string actionType,
            CustomerShippingAddress originalShipAddress,
            CustomerShippingAddress updatedShipAddress)
        {
            // get list of changed properties name and log product size changes
            var changedPropertyName = CommonFunctions.GetChangedPropertiesName(originalShipAddress, updatedShipAddress);
            List<LogCustomerShippingAddress> listLogCustomerAddress = new List<LogCustomerShippingAddress>();
            DateTime logCreatedDate = DateTime.Now;

            foreach (var name in changedPropertyName)
            {                
                if (!listLogCustomerAddress.Any(x => x.LogUpdatedFieldName == name))
                {
                    if (Enum.IsDefined(typeof(LogCustomerShippingAddressEnum), name))
                    {
                        listLogCustomerAddress.Add(new LogCustomerShippingAddress
                        {
                            CustomerFk = customerId,
                            CustomerShippingAddressFk = customerAddressFk,
                            LogUpdatedFieldName = name,
                            LogUpdatedFieldFk = (int)Enum.Parse(typeof(LogCustomerShippingAddressEnum), name, true),
                            LogUpdatedFieldOriginalValue = originalShipAddress.GetType().GetProperty(name).GetValue(originalShipAddress) != null ?
                                                        originalShipAddress.GetType().GetProperty(name).GetValue(originalShipAddress).ToString() : string.Empty,
                            LogUpdatedFieldModifiedValue = updatedShipAddress.GetType().GetProperty(name).GetValue(updatedShipAddress) != null ?
                                                        updatedShipAddress.GetType().GetProperty(name).GetValue(updatedShipAddress).ToString() : string.Empty,
                            LogDateCreated = logCreatedDate,
                            LogActionType = actionType

                        });
                    }
                }
            }
            return listLogCustomerAddress;
        }

        private void PopulateProvinceName(CustomerMinimal customerInfo)
        {
            var province = _repositoryCustomer.GetProvinceById(customerInfo.CustomerProvinceId);

            customerInfo.BillingProvinceName = province?.ProvinceName;
        }

        private void PopulateCountryName(CustomerMinimal customerInfo)
        {
            var country = _repositoryCustomer.GetCountryById(customerInfo.CustomerCountryId);

            customerInfo.BillingCountryName = country?.CountryName;
            customerInfo.CustomerCountryName = country?.CountryName;
        }
    }


}