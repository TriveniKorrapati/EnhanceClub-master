using System;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EnhanceClub.Domain.Concrete;

namespace EnhanceClub.WebUI.Filters
{
    public class AuthorizeIpAddressAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            string ipAddress = HttpContext.Current.Request.UserHostAddress;

            if (ipAddress != null && !IsIpAddressAllowed(ipAddress.Trim()))
            {
                context.HttpContext.Response.StatusCode = 404;
                context.Result = new RedirectResult("/error/showip",true);
                
               // context.Result = new HttpStatusCodeResult(400, ipAddress);

            }

            base.OnActionExecuting(context);
        }

        private bool IsIpAddressAllowed(string ipAddress)
        {
            //return false; // only for testing
            if (!string.IsNullOrWhiteSpace(ipAddress))
            {
                if (!IsCdnIp(ipAddress))
                {
                   return true;
                    
                }
            }
            return false;
        }

        // check if ip is canadian ip
        private bool IsCdnIp(string ipAddress)
        {
            if (!String.IsNullOrEmpty(ipAddress))
            {

                // first check if it is allowed IP
                var allowedIp = ConfigurationManager.AppSettings["AllowedIP"].ToString();
                if (allowedIp.Split(',').Contains(ipAddress))
                {
                    return false;
                }
                string[] ipOctet= ipAddress.Split('.');
                long ipnum = 16777216 * Convert.ToInt64(ipOctet[0]) + 65536 * Convert.ToInt64(ipOctet[1]) + 256 * Convert.ToInt64(ipOctet[2]) + Convert.ToInt32(ipOctet[3]);
                
                GlobalFunctions gbf = new GlobalFunctions();
                var isCan = gbf.IsCanadianIp(ipnum);

               // gbf.SendDebugEmail(ipnum.ToString(), "Authorize module " + DateTime.Now);

                return isCan;
            }
            return false;
        }
    }
}