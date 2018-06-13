namespace Fanex.AccountingReports.WinLoss.Controllers
{
    using System.Web.Mvc;
    using Core.Web;
    using Fanex.AccountingReports.Core.Exceptions;
    using Fanex.AccountingReports.Core.Web.Controllers;
    using Fanex.AccountingReports.WinLoss.Criteria.DailyMixParlayWinLoss;
    using Fanex.AccountingReports.WinLoss.Services.Implement.DailyMixParlayWinLoss;
    using Fanex.AccountingReports.WinLoss.Services.Interface.DailyMixParlayWinLoss;
    using Fanex.NAP.Core.Business;
    using Fanex.NAP.Core.Web.Attribute;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class DailyMixParlayWinLoss_2hLinkController : BaseController
    {
        private readonly IDailyMixParlayWinLossService _reportService;

        public DailyMixParlayWinLoss_2hLinkController()
        {
            this._reportService = new DailyMixParlayWinLossService();
        }

        [HttpGet]
        [UserActivityFilter]
        [NapAuth(FunctionKey = AccReportFunctionKeys.HouseWinLossByProducts, Policy = Constant.Policy.Read)]
        public ActionResult Index()
        {
            return View();
        }

        [UserActivityFilter]
        [NapAuth(FunctionKey = AccReportFunctionKeys.DailyMixParlayWinLoss, Policy = Constant.Policy.Read)]
        public ActionResult MixParlayPopup()
        {
            return View("~/Areas/WinLoss/Views/DailyMixParlayWinLoss_2hLink/MixParlayReportPopup.cshtml");
        }

        [HttpPost]
        [HandleAjaxError]
        public JsonResult GetCombinationWinLossReport(GetCombinationWinLossReportCriteria criteria)
        {
            var report = this._reportService.GetCombinationWinLossReport(criteria);
            return Json(report, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [HandleAjaxError]
        public JsonResult GetAdvanceMixParlayReport(GetAdvanceMixParlayReportCriteria criteria)
        {
            var report = this._reportService.GetAdvanceMixParlayReport(criteria);
            return Json(report, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [HandleAjaxError]
        public JsonResult GetMixParlayReport(GetMixParlayReportCriteria criteria)
        {
            var report = this._reportService.GetMixParlayReport(criteria);
            return Json(report, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ExportCombinationWinLossReport(GetCombinationWinLossReportCriteria criteria)
        {
            this._reportService.ExportCombinationWinLossReport(criteria);
            return new EmptyResult();
        }

        public ActionResult ExportAdvanceMixParlayReport(GetAdvanceMixParlayReportCriteria criteria)
        {
            this._reportService.ExportAdvanceMixParlayReport(criteria);
            return new EmptyResult();
        }

        public ActionResult ExportMixParlayReport(GetMixParlayReportCriteria criteria)
        {
            this._reportService.ExportMixParlayReport(criteria);
            return new EmptyResult();
        }
    }
}
