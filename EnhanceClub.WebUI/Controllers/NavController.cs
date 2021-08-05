using System.Web.Mvc;
using EnhanceClub.Domain.Abstract;

namespace EnhanceClub.WebUI.Controllers
{
    public class NavController : Controller
    {
        private readonly IMenuItemRepository _repoMenu;
        private readonly IMenuTabRepository _repoMenuTab;

        public NavController(IMenuItemRepository repo, IMenuTabRepository repoMenuTab)
        {
            _repoMenu = repo;
            this._repoMenuTab = repoMenuTab;
        }
        
        // this generates simple menu where menu is single level 
        public PartialViewResult Menu()
        {
            return PartialView(_repoMenu.MenuItems);
        }

        // this generates multi level menu 
        public PartialViewResult MultiMenu()
        {
            return PartialView(_repoMenuTab.MenuTabs);
        }


	}
}