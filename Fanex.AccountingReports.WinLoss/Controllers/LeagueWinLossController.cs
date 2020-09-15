namespace Fanex.AccountingReports.WinLoss.Controllers
{
    using Core.Web;
    using Fanex.AccountingReports.Core.Web.Controllers;
    using Fanex.AccountingReports.WinLoss.Criteria.LeagueWinLoss;
    using Fanex.AccountingReports.WinLoss.Services.Implement;
    using Fanex.NAP.Core.Business;
    using Fanex.NAP.Core.Web.Attribute;
    using Services.Implement.LeagueWinLoss;
    using System.Web.Mvc;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class LeagueWinLossController : BaseController
    {
        private readonly LeagueWinLossService _reportService;
        private readonly LeagueWinLossExportExcel _excelService;

        public LeagueWinLossController()
        {
            _reportService = new LeagueWinLossService();
            _excelService = new LeagueWinLossExportExcel(_reportService);
        }

        [HttpGet]
        [UserActivityFilter]
        [NapAuth(FunctionKey = AccReportFunctionKeys.LeagueWinLoss, Policy = Constant.Policy.Read)]
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public JsonResult GetSport()
        {
            var sports = this._reportService.GetSports();
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
        public JsonResult GetMatches(GetMatchesCriteria criteria)
        {
            var report = this._reportService.GetMatches(criteria);
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

        public ActionResult ExportLeagueWinLoss(GetLeagueWinLossCriteria criteria)
        {
            this._excelService.ExportLeagueWinLoss(criteria);
            return new EmptyResult();
        }

        //public ActionResult ExportLeagueWinLossBySport(GetLeagueWinLossBySportCriteria criteria)
        //{
        //    this._excelService.ExportLeagueWinLossBySport(criteria);
        //    return new EmptyResult();
        //}

        //public ActionResult ExportLeagueWinLossByBetType(GetMatchesCriteria criteria)
        //{
        //    this._excelService.ExportLeagueWinLossByBetType(criteria);
        //    return new EmptyResult();
        //}
    }
}