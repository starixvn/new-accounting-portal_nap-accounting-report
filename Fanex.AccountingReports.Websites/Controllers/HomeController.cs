namespace Fanex.AccountingReports.Websites.Controllers
{
    using System.Web.Mvc;
    using Fanex.AccountingReports.Core.Criteria;
    using Fanex.AccountingReports.Core.Models;
    using Fanex.AccountingReports.Core.Services.Impl;
    using Fanex.AccountingReports.Core.Services.Interface;
    using Fanex.AccountingReports.Core.Web.Controllers;
    using Fanex.NAP.Core.Web.Attribute;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    [NapSuperAuth]
    public class HomeController : BaseController
    {
        private readonly IMostVisitedPagesService _mostVisitedPagesService;

        private readonly MenuService _menuService;

        public HomeController()
        {
            this._mostVisitedPagesService = new MostVisitedPagesService();
            _menuService = new MenuService();
        }

        /// <summary>
        /// Gets Index page.
        /// </summary>
        /// <returns>Index page.</returns>
        public ActionResult Index()
        {
            return View();
        }

        [ChildActionOnly]
        public PartialViewResult _Menu()
        {
            ViewBag.MenuItemList = CurrentSession.MenuItemList;
            return PartialView("~/Views/Shared/_Menu.cshtml");
        }

        [HttpPost]
        public JsonResult GetMostvisitedPages()
        {
            var mostVisitedPages = this._mostVisitedPagesService.GetMostVisitedPages();
            return Json(mostVisitedPages, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult UpdateMostVisitedPageCount(MostVisitedPage mostVisitedPage)
        {
            this._mostVisitedPagesService.UpdateCount(mostVisitedPage);
            return new EmptyResult();
        }

        [HttpGet]
        public ActionResult UpdateMostVisitedPageCountByUrl(string url)
        {
            var menuPath = MenuService.GetMenuPathFromRequestFilePath(url, Request.ApplicationPath);
            menuPath = menuPath.TrimEnd(new[] { '/', '#' });
            var menu = _menuService.GetMenuByURL(menuPath);
            if (menu != null)
            {
                var mostVisitedPage = new MostVisitedPage { MenuId = menu.Id };
                this._mostVisitedPagesService.UpdateCount(mostVisitedPage);
                return Content(menu.Id.ToString());

            }
            return new EmptyResult();
        }



        [HttpPost]
        public ActionResult UpdateMostVisitedPagePosition(UpdateMostVisitedPagePositionCriteria mostVisitedPage)
        {
            this._mostVisitedPagesService.UpdatePosition(mostVisitedPage);
            return new EmptyResult();
        }

        [HttpPost]
        public JsonResult DeleteMostVisitedPage(DeleteMostVisitedPagesCriteria mostVisitedPage)
        {
            this._mostVisitedPagesService.Delete(mostVisitedPage);
            var mostVisitedPages = this._mostVisitedPagesService.GetMostVisitedPages();
            return Json(mostVisitedPages, JsonRequestBehavior.AllowGet);
        }
    }
}
