using System.Web.Mvc;

namespace EnhanceClub.WebUI
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
           // filters.Add(new AuthorizeIpAddressAttribute());
        }
    }
}
