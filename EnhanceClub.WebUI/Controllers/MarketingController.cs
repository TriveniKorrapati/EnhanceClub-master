using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using EnhanceClub.Domain.Abstract;
using EnhanceClub.Domain.Entities;
using EnhanceClub.WebUI.Helpers;
using EnhanceClub.WebUI.Infrastructure.Utility;

namespace EnhanceClub.WebUI.Controllers
{
     //marketing controller
    public class MarketingController : Controller
    {
        private readonly IStorefrontRepository _storeFrontRepository;
      
        private readonly IProductRepository _productRepository;
        public MarketingController(IStorefrontRepository storefrontRepository,
            IProductRepository productRepository)
        {
            _storeFrontRepository = storefrontRepository;
            _productRepository = productRepository;
        }
        // Online pharmacy city page


        public ActionResult ComingSoon(AffiliateInfo affiliateInfo)
        {
            @ViewBag.Title = "Coming Soon";
            return View(affiliateInfo);
        }

        // to add subscribe user
        public ActionResult SubscribeUser(string name, string email, AffiliateInfo affiliateInfo)
        {
            var ipAddress = CommonFunctions.GetVisitorIpAddress();
            var userId = _storeFrontRepository.AddSubscribedUser(affiliateInfo.AffiliateStoreFrontFk, name, email, ipAddress, DateTime.Now);
           if (userId > 0)
            {
                return Json("success", JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json("error", JsonRequestBehavior.AllowGet);
            };
        }

    }
}