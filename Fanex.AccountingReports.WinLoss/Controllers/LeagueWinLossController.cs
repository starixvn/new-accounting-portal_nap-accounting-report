namespace Fanex.AccountingReports.WinLoss.Controllers
{
    using Constants;
    using Core.Web;
    using Fanex.AccountingReports.Core.Web.Controllers;
    using Fanex.AccountingReports.WinLoss.Criteria.LeagueWinLoss;
    using Fanex.AccountingReports.WinLoss.Services.Implement;
    using Fanex.NAP.Core.Business;
    using Fanex.NAP.Core.Web.Attribute;
    using NAP.Core.Configuration;
    using NAP.Core.Service.Impl;
    using NAP.Core.Service.Interface;
    using NAP.Core.Web.Session;
    using Services.Implement.LeagueWinLoss;
    using System.Linq;
    using System.Web.Mvc;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class LeagueWinLossController : BaseController
    {
        private readonly LeagueWinLossService _reportService;
        private readonly LeagueWinLossExportExcel _excelService;
        private readonly ICurrentSession _currentSession;
        private readonly IUserService _userService;

        public LeagueWinLossController()
        {
            _reportService = new LeagueWinLossService();
            _excelService = new LeagueWinLossExportExcel(_reportService);
            _currentSession = new CurrentSession();
            _userService = new UserService();
        }

        [HttpGet]
        [UserActivityFilter]
        [NapAuth(FunctionKey = AccReportFunctionKeys.LeagueWinLoss, Policy = Constant.Policy.Read)]
        public ActionResult Index()
        {
            var modePermission = this._userService.GetPermissions(
                _currentSession.UserId,
                NapSettings.Settings.SiteId,
                AccReportFunctionKeys.LeagueWinLossModePermission)
                .Where(p => p.Policy >= (int)Constant.Policy.Read)
                .Select(p => p.Key);

            var productPermission = this._userService.GetPermissions(
             _currentSession.UserId,
             NapSettings.Settings.SiteId,
             AccReportFunctionKeys.LeagueWinLossProductPermission)
             .Where(p => p.Policy >= (int)Constant.Policy.Read)
             .OrderBy(x => x.Name)
             .Select(p => p.Key);

            var permission = productPermission.Any() ? productPermission.FirstOrDefault() : string.Empty;
            ViewBag.PermissionProfileID = _reportService.GetPermissionProfileId(permission);

            ViewBag.IsFullModePermission = modePermission.Count() > 1;
            ViewBag.IsNormalMode = modePermission.FirstOrDefault() == LeagueWinLossPermissionConstants.LeagueWinLoss_NormalMode;
            ViewBag.Status = _reportService.GetStatus();
            return View();
        }

        [HttpPost]
        public JsonResult GetSport()
        {
            var sports = this._reportService.GetSports();
            return Json(sports, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetGlobalShowTime()
        {
            var results = this._reportService.GetGlobalShowTime();
            return Json(results, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetSiteFilter()
        {
            var sports = this._reportService.GetSiteFilter();
            return Json(sports, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetLeagueWinLoss(GetLeagueWinLossCriteria criteria)
        {
            var report = this._reportService.GetLeagueWinLoss(criteria);
            return Json(report, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetBetTypes(GetBetTypesCriteria criteria)
        {
            var report = this._reportService.GetBetTypes(criteria);
            return Json(report, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetBetTypesByStatus(GetBetTypesCriteria criteria)
        {
            var report = this._reportService.GetBetTypesByLiveNonLiveStatus(criteria);
            return Json(report, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetMatches(GetMatchesCriteria criteria)
        {
            var report = this._reportService.GetMatches(criteria);
            return Json(report, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetMatchesFromBetTypeStatus(GetMatchesCriteria criteria)
        {
            var report = this._reportService.GetMatchesFromBetTypeStatus(criteria);
            return Json(report, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetSites(GetSitesCriteria criteria)
        {
            var report = this._reportService.GetSites(criteria);
            return Json(report, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetVirtualRate(GetVirtualRateCriteria criteria)
        {
            var report = this._reportService.GetVirtualRate(criteria);
            return Json(report, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetSupers(GetSuperCriteria criteria)
        {
            var report = this._reportService.GetSupers(criteria);
            return Json(report, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult ExportLeagueWinLoss(GetLeagueWinLossCriteria criteria)
        {
            this._excelService.ExportLeagueWinLoss(criteria);
            return new EmptyResult();
        }

        [HttpPost]
        public ActionResult ExportLeagueWinLossViewMatches(GetMatchesCriteria criteria)
        {
            this._excelService.ExportLeagueWinLossViewMatches(criteria);
            return new EmptyResult();
        }

        [HttpPost]
        public ActionResult ExportLeagueWinLossViewMatchesFromBetTypeStatus(GetMatchesCriteria criteria)
        {
            this._excelService.ExportLeagueWinLossViewMatchesFromBetTypeStatus(criteria);
            return new EmptyResult();
        }

        [HttpPost]
        public ActionResult ExportLeagueWinLossViewBetTypes(GetBetTypesCriteria criteria)
        {
            this._excelService.ExportLeagueWinLossViewBetTypes(criteria);
            return new EmptyResult();
        }

        [HttpPost]
        public ActionResult ExportLeagueWinLossViewBetTypesByStatus(GetBetTypesCriteria criteria)
        {
            this._excelService.ExportLeagueWinLossViewBetTypesByStatus(criteria);
            return new EmptyResult();
        }

        [HttpPost]
        public ActionResult ExportLeagueWinLossViewSites(GetSitesCriteria criteria)
        {
            this._excelService.ExportLeagueWinLossViewSites(criteria);
            return new EmptyResult();
        }

        [HttpPost]
        public ActionResult ExportLeagueWinLossViewSupers(GetSuperCriteria criteria)
        {
            this._excelService.ExportLeagueWinLossViewSupers(criteria);
            return new EmptyResult();
        }
    }
}