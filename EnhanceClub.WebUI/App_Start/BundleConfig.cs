using System.Web.Optimization;
namespace EnhanceClub.WebUI
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {

            bundles.Add(new ScriptBundle("~/bundles/customJs").Include(
                "~/Scripts/homePage.js"));

            bundles.Add(new ScriptBundle("~/bundles/customer-profile-validation").Include(
                "~/Scripts/Validation/customerProfileValidation.js"));


            BundleTable.EnableOptimizations = true;
        }
    }
}
