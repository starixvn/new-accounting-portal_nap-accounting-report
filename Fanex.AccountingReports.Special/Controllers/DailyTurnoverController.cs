namespace Fanex.AccountingReports.Special.Controllers
{
    using System.Web.Mvc;
    using Core.Web;
    using Fanex.AccountingReports.Core.Exceptions;
    using Fanex.AccountingReports.Core.Web.Controllers;
    using Fanex.AccountingReports.Special.Criteria.DailyTurnover;
    using Fanex.AccountingReports.Special.Services.Implement.DailyTurnover;
    using Fanex.AccountingReports.Special.Services.Interface.DailyTurnover;
    using Fanex.AccountingReports.Special.Services.Interface.IDailyTurnover;
    using Fanex.NAP.Core.Business;
    using Fanex.NAP.Core.Web.Attribute;

    /// <summary>
    /// The controller of Daily Turnover.
    /// </summary>
    public class DailyTurnoverController : BaseController
    {
        private readonly IDailyTurnoverService _reportService;

        private readonly IDailyDetailChangedTurnoverService _reportDetailService;

        private readonly IDailyDetailChangedTurnoverExportReportService _exportReportDetailService;

        public DailyTurnoverController()
        {
            this._reportService = new DailyTurnoverService();
            this._reportDetailService = new DailyDetailChangedTurnoverService();
            this._exportReportDetailService = new DailyDetailChangedTurnoverExportReportService();
        }

        [HttpGet]
        [UserActivityFilter]
        [NapAuth(FunctionKey = AccReportFunctionKeys.DailyTurnover, Policy = Constant.Policy.Read)]
        public ActionResult Index()
        {
            return View("~/Areas/Special/Views/DailyTurnover/Index.cshtml");
        }

        [HttpPost]
        [HandleAjaxError]
        public JsonResult GetReport(GetDailyTurnoverCriteria criteria)
        {
            var result = this._reportService.GetReport(criteria);
            var jsonResult = Json(result);
            jsonResult.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        [HttpPost]
        [HandleAjaxError]
        public JsonResult GetDetailReport(GetDailyTurnoverChangedDetailCriteria criteria)
        {
            var result = this._reportDetailService.GetDetailReport(criteria);
            var jsonResult = Json(result);
            jsonResult.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        public EmptyResult ExportReport(GetDailyTurnoverCriteria criteria)
        {
            this._reportService.ExportReport(criteria);
            return new EmptyResult();
        }

        public EmptyResult ExportDetailReport(GetDailyTurnoverChangedDetailCriteria criteria)
        {
            this._exportReportDetailService.ExportDetailReport(criteria);
            return new EmptyResult();
        }

        public EmptyResult ExportDetailDifferenceReport(string index, string fromDate)
        {
            this._exportReportDetailService.ExportDetailDifferenceReport(index, fromDate);
            return new EmptyResult();
        }
    }
}
