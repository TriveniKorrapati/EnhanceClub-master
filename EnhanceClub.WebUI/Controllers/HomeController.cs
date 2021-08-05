using System;
using System.Linq;
using System.Web.Mvc;
using EnhanceClub.Domain.Entities;
using EnhanceClub.WebUI.Filters;
using EnhanceClub.WebUI.Helpers;
using System.Collections.Generic;
using EnhanceClub.WebUI.Infrastructure.Utility;
using EnhanceClub.WebUI.Models;
using EnhanceClub.Domain.Abstract;
using System.Threading.Tasks;

namespace EnhanceClub.WebUI.Controllers
{
  
    public class HomeController : Controller
    {
        private readonly IStorefrontRepository _storeFrontRepository;
        private readonly IProductRepository _productRepository;
        private readonly ICustomerRepository _customerRepository;

        public HomeController(IStorefrontRepository storeFrontRepository, IProductRepository productRepository, ICustomerRepository customerRepository)
        {
            _storeFrontRepository = storeFrontRepository;
            _productRepository = productRepository;
            _customerRepository = customerRepository;
        }
        //
        // GET: /Home/

       [OutputCache(CacheProfile = "cp-serverclient")]
        public ActionResult Index(AffiliateInfo affiliateInfo, LoggedCustomer loggedCustomer)
        {
            
            @ViewBag.Title = "Men's Health Online Delivery Pharmacy | " + SiteConfigurationsWc.StorefrontName;
            @ViewBag.Description = SiteConfigurationsWc.StorefrontName + " is an online pharmacy created for men looking to live their best lives with confidence. Order online today and explore the endless possibilities.";
            ViewBag.MetaKeywords = "Men's Health,Online Delivery Pharmacy,Online Pharmacy";

            @ViewBag.HeaderClass = "";
            if (Request.Url != null)
            {
                @ViewBag.canonicalRef = CommonFunctions.RemoveLastChar(Request.Url.AbsoluteUri);
            }
            List<BlogTable> Blogs = _storeFrontRepository.GetBlogList(0, 1, affiliateInfo.AffiliateStoreFrontFk,
                                " blog_featured desc, blog_datecreated desc ").Take(4).ToList(); 

           // get products for home page
            List<TopFeaturedProduct> featuredProducts = _productRepository.GetTopMedications(affiliateInfo.AffiliateStoreFrontFk, "rx").ToList();
            bool showPrescriptionNotification = false;
            bool uploadDocumentStatus = true;
            if(loggedCustomer.CustomerId > 0)
            {
                // get open orders for the customer
                List<OrderStatus> openOrders = _customerRepository.GetCustomerOpenOrders(loggedCustomer.CustomerId, affiliateInfo.AffiliateStoreFrontFk);
                if (openOrders.Count > 0)
                {
                    var customerIdDocument = _customerRepository.GetCustomerIdDocumentByCustomerId(loggedCustomer.CustomerId);
                    uploadDocumentStatus = customerIdDocument != null && customerIdDocument.Count > 0 ? true : false;
                    showPrescriptionNotification = openOrders.Where(x => x.OrderCart.Any(y => y.QuestionnaireCategoryResponse == null) && x.PrescriptionId == 0).Any();
                }
            }
            var returnView = "Index";
            if(SiteConfigurationsWc.HomePageUpdateJune2021 == 1)
            {
                returnView = "Index-v2";
            }
            
            return View(returnView, new HomePageViewModel { BlogList = Blogs,
                    FeaturedProducts = featuredProducts.OrderBy(x=>x.ProductStoreFrontDisplayOrder).ToList(),
                    ShowPrescriptionNotification = showPrescriptionNotification,
                    UploadDocumentStatus = uploadDocumentStatus
            });
        }

        // how It works
        [OutputCache(CacheProfile = "cp-serverclient")]
        [ActionName("how-it-works")]
        public ActionResult HowItWorks(AffiliateInfo affiliateInfo)
        {

            @ViewBag.Title = "How "+ SiteConfigurationsWc.StorefrontName + " Works | Step By Step Guide";
            @ViewBag.Description =
                "Find out how to place an order online from " + SiteConfigurationsWc.StorefrontName + ". Choose the enhancement you're looking for and follow these easy steps.";

            if (Request.Url != null) { @ViewBag.canonicalRef = Request.Url.AbsoluteUri; }
            return View(affiliateInfo);
        }

        [OutputCache(CacheProfile = "cp-serverclient")]       
        public ActionResult About(AffiliateInfo affiliateInfo)
        {

            @ViewBag.Title = "About Us - Our Story & Our Mission |  " + SiteConfigurationsWc.StorefrontName;
            @ViewBag.Description = SiteConfigurationsWc.StorefrontName +
                " offers high-quality, affordable products for men online. Learn more about Enhance Club and become the best version of yourself.";
            if (Request.Url != null) { @ViewBag.canonicalRef = Request.Url.AbsoluteUri; }
            return View(affiliateInfo);
        }


        // Get In Touch
        [OutputCache(CacheProfile = "cp-serverclient")]
        [ActionName("get-in-touch")]
        public async Task<ActionResult> GetInTouch(AffiliateInfo affiliateInfo)
        {

            @ViewBag.Title = "Get In Touch - Connect With Experts Today | " + SiteConfigurationsWc.StorefrontName;
            @ViewBag.Description =
                "You have got questions, we have got answers. Get in touch with us! The " + SiteConfigurationsWc.StorefrontName + " Team will be happy to help.";

            ViewBag.RecaptchaSiteKey = SiteConfigurationsWc.RecaptchaSiteKey;

            if (Request.Url != null) { @ViewBag.canonicalRef = Request.Url.AbsoluteUri; }

            if (SiteConfigurationsWc.IsZendeskActive == 1)
            {
                /* Zenddesk service*/
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
                else
                {

                    @ViewBag.ZendeskMessage = zendeskCategory.Value;
                }
            }
           

            return View("get-in-touch", new GetInTouchViewModel
            {
                CaptchaSiteKey = @SiteConfigurationsWc.RecaptchaSiteKey
            });
        }


        // Faq 
        [OutputCache(CacheProfile = "cp-serverclient")]
        public ActionResult Faq(AffiliateInfo affiliateInfo)
        {

            @ViewBag.Title = "Frequently Asked Questions (FAQ) | " + SiteConfigurationsWc.StorefrontName;
            @ViewBag.Description =
                "Find the answers to all of your questions right here. Learn more about " + SiteConfigurationsWc.StorefrontName  + ", our products, and our services. Have questions? We can help.";
            if (Request.Url != null) { @ViewBag.canonicalRef = Request.Url.AbsoluteUri; }
            return View(affiliateInfo);
        }

        [OutputCache(CacheProfile = "cp-serverclient")]
        [ActionName("shipping-and-returns")]
        public ActionResult ShippingAndReturns(AffiliateInfo affiliateInfo)
        {
            @ViewBag.Title = "Shipping & Return Information | " + SiteConfigurationsWc.StorefrontName;
            @ViewBag.Description = "Learn more about Enhance Club's shipping & return policy on prescription and over-the-counter products.";
            return View(affiliateInfo);
        }

        [OutputCache(CacheProfile = "cp-serverclient")]
        [ActionName("terms-and-conditions")]
        public ActionResult TermsAndConditions(AffiliateInfo affiliateInfo)
        {
            @ViewBag.Title = "Legal Statement & Terms of Service | " + SiteConfigurationsWc.StorefrontName;
            @ViewBag.Description = "Welcome to "+ SiteConfigurationsWc.StorefrontName  + ". Please review our Terms and Conditions carefully.";
            return View(affiliateInfo);
        }

        [OutputCache(CacheProfile = "cp-serverclient")]
        [ActionName("privacy-policy")]
        public ActionResult PrivacyPolicy(AffiliateInfo affiliateInfo)
        {
            @ViewBag.Title = "Privacy Policy  | " + SiteConfigurationsWc.StorefrontName;
            @ViewBag.Description =
             "Welcome to "+ SiteConfigurationsWc.StorefrontName + ". Please review our Privacy Policy carefully.";

            return View(affiliateInfo);
        }


        public ActionResult GetProductSizeQuantityList(AffiliateInfo affiliateInfo, string strength, int productFk=0, bool productGeneric= true)
        {
            var list = _productRepository.GetProductSizeForProduct(productFk, affiliateInfo.AffiliateStoreFrontFk, 1, strength, SiteConfigurationsWc.ProductSizeVisibleFrontEndOnly);
            list = list.Where(x => x.ProductSizeGeneric == productGeneric).ToList();
            return Json(list, JsonRequestBehavior.AllowGet);
           // return null;
        }

        [OutputCache(CacheProfile = "cp-serverclient")]
        [ActionName("telehealth-consent")]
        public ActionResult TelehealthConsent(AffiliateInfo affiliateInfo)
        {
            @ViewBag.Title = "Telehealth Consent | " + SiteConfigurationsWc.StorefrontName;
            @ViewBag.Description = "Welcome to " + SiteConfigurationsWc.StorefrontName + ". Please review our Telehealth Terms and Conditions carefully.";
            return View(affiliateInfo);
        }
    }
}