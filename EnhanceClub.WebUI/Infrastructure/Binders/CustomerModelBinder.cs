using System.Web.Mvc;
using EnhanceClub.Domain.Entities;

namespace EnhanceClub.WebUI.Infrastructure.Binders
{
    public class CustomerModelBinder : IModelBinder
    {
        private const string sessionKey = "LoggedCustomer";

        public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            // get the loggedCustomer from the session
            LoggedCustomer loggedCustomer = null;

            if (controllerContext.HttpContext.Session != null)
            {
                loggedCustomer = (LoggedCustomer)controllerContext.HttpContext.Session[sessionKey];
            }

            if (loggedCustomer == null)
            {
                loggedCustomer = new LoggedCustomer();
                if (controllerContext.HttpContext.Session != null)
                {
                    controllerContext.HttpContext.Session[sessionKey] = loggedCustomer;
                }
            }

            return loggedCustomer;
        }
    }
}