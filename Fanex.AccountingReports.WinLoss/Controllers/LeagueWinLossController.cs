namespace Fanex.AccountingReports.WinLoss.Controllers
{
    using System.Web.Mvc;
    using Fanex.AccountingReports.Core.Web.Controllers;
    using Fanex.AccountingReports.WinLoss.Criteria.LeagueWinLoss;
    using Fanex.AccountingReports.WinLoss.Services.Implement;
    using Fanex.AccountingReports.WinLoss.Services.Interface;
    using Fanex.NAP.Core.Business;
    using Fanex.NAP.Core.Web.Attribute;
    using Services.Implement.LeagueWinLoss;
    using Services.Interface.LeagueWinLoss;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class LeagueWinLossController : BaseController
    {
        private readonly ILeagueWinLossService _reportService;
        private readonly ILeagueWinLossExportExcel _excelService;

        public LeagueWinLossController()
        {
            _reportService = new LeagueWinLossService();
            _excelService = new LeagueWinLossExportExcel(_reportService);
        }

        [HttpGet]
        [NapAuth(FunctionKey = AccReportFunctionKeys.LeagueWinLoss, Policy = Constant.Policy.Read)]
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public JsonResult GetLeagueWinLoss(GetLeagueWinLossCriteria criteria)
        {
            var report = this._reportService.GetLeagueWinLoss(criteria);
            return Json(report, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetLeagueWinLossBySport(GetLeagueWinLossBySportCriteria criteria)
        {
            var report = this._reportService.GetLeagueWinLossBySport(criteria);
            return Json(report, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetLeagueWinLossByBetType(GetLeagueWinLossByBetTypeCriteria criteria)
        {
            var report = this._reportService.GetLeagueWinLossByBetType(criteria);
            return Json(report, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ExportLeagueWinLoss(GetLeagueWinLossCriteria criteria)
        {
            this._excelService.ExportLeagueWinLoss(criteria);
            return new EmptyResult();
        }

        public ActionResult ExportLeagueWinLossBySport(GetLeagueWinLossBySportCriteria criteria)
        {
            this._excelService.ExportLeagueWinLossBySport(criteria);
            return new EmptyResult();
        }

        public ActionResult ExportLeagueWinLossByBetType(GetLeagueWinLossByBetTypeCriteria criteria)
        {
            this._excelService.ExportLeagueWinLossByBetType(criteria);
            return new EmptyResult();
        }
    }
}
