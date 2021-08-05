using System;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using EnhanceClub.Domain.Concrete;
using EnhanceClub.Domain.Entities;
using EnhanceClub.WebUI.Infrastructure.Binders;

namespace EnhanceClub.WebUI
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            // model binder for cart
            ModelBinders.Binders.Add(typeof(Cart), new CartModelBinder());

            // model binder for customer
            ModelBinders.Binders.Add(typeof(LoggedCustomer), new CustomerModelBinder());

            // model binder for Affiliate
            ModelBinders.Binders.Add(typeof(AffiliateInfo), new AffiliateInfoModelBinder());

            // Added to remove other view engines and just use razor
            ViewEngines.Engines.Clear();
            ViewEngines.Engines.Add(new RazorViewEngine()); 

        }

        void Application_Error(Object sender, EventArgs e)
        {
            var exception = Server.GetLastError();
            if (exception == null)
            {
                return;
            }
            // send error email
            StringBuilder errorMsg = new StringBuilder();
            errorMsg.Append("Error Description:");
            errorMsg.Append(exception.Message + "<BR/>");
            errorMsg.Append("Inner Exception:");
            errorMsg.Append(exception.InnerException + "<BR/>");
            errorMsg.Append("Source:");
            errorMsg.Append(exception.Source + "<BR/>");
            errorMsg.Append("StackTrace:");
            errorMsg.Append(exception.StackTrace + "<BR/>");
            errorMsg.Append("Exception Data:");
            errorMsg.Append(exception.Data + "<BR/>");
            errorMsg.Append("Target Site:");
            errorMsg.Append(exception.TargetSite + "<BR/>");
            errorMsg.Append("Client IP:");
            errorMsg.Append(HttpContext.Current.Request.UserHostAddress + "<br/>");

            GlobalFunctions gbf = new GlobalFunctions();
            gbf.SendDebugEmail(errorMsg.ToString(), "EnhanceClub Frontend Error Encountered at " + DateTime.Now);

            // Clear the error
            //Server.ClearError();
        }
    }
}
