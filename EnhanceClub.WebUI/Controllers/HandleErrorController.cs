using System.Web.Mvc;

namespace EnhanceClub.WebUI.Controllers
{
    public class HandleErrorController : Controller
    {
        public ActionResult NotFound(string aspxerrorpath)
        {
            return RedirectToAction("PageNotFound");
        }

        public ViewResult PageNotFound()
        {
            return View("NotFound");
        }

	}
}