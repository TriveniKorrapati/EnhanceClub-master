using System;
using System.Web.Mvc;
using EnhanceClub.Domain.Entities;
using EnhanceClub.WebUI.Infrastructure.Utility;

namespace EnhanceClub.WebUI.Controllers
{
    public class ErrorController : Controller
    {

        // redirected by web.config for 404 
        public ActionResult NotFound(string aspxErrorPath)
        {
            @ViewBag.Title = "Page Not Found | " + SiteConfigurationsWc.StorefrontUrl;

                @ViewBag.Description =
                "Page Not Found. " + SiteConfigurationsWc.StorefrontName ;
                Response.StatusCode = 404;

               if (Request.Url != null) { @ViewBag.canonicalRef = Request.Url.AbsoluteUri; }

                return View("NotFound");
        }

        public ActionResult ProgramError(AffiliateInfo affiliateInfo)
        {
            return View((Object)affiliateInfo.StoreFrontWebsiteProblems);
        }

        public ActionResult ShowIp()
        {
            return View();
        }

        //{

        //    return RedirectToAction("PageNotFound", new { reqPath = aspxerrorpath });
        //}

        //public ViewResult PageNotFound(string reqPath)
        //{
           
        //    return this.View("NotFound");
        //}
        
	}
}