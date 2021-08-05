using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using EnhanceClub.Domain.Concrete;

namespace EnhanceClub.WebUI
{
    public class SessionExpire : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {

          // save to Log Table 
           var actionLog = new GlobalFunctions
            {
                LogController = filterContext.ActionDescriptor.ControllerDescriptor.ControllerName,
                LogAction = filterContext.ActionDescriptor.ActionName + " (Logged By: Custom Action Filter)",
                LogIp = filterContext.HttpContext.Request.UserHostAddress,
                LogDateTime = filterContext.HttpContext.Timestamp,
                LogSessionTimeout = HttpContext.Current.Session["LoggedCustomer"] == null
            };

            // -- moved inside if condition to log only when session expire caused a login redirect
           // actionLog.AddActionLog();


            if (HttpContext.Current.Session["LoggedCustomer"] == null)
            {
                actionLog.AddActionLog();
                FormsAuthentication.SignOut();
                filterContext.Result =
                new RedirectToRouteResult(new RouteValueDictionary   
                    {  
                     { "action", "Login" },  
                     { "controller", "Customer" },  
                     { "returnUrl", filterContext.HttpContext.Request.RawUrl}  
                    }
                 );

                return;
            }
        }

    }  
}