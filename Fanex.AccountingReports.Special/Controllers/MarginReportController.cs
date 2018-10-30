namespace Fanex.AccountingReports.Special.Controllers
{
    using System.Web.Mvc;
    using Core.Web;
    using Fanex.AccountingReports.Core.Exceptions;
    using Fanex.AccountingReports.Core.Web.Controllers;
    using Fanex.AccountingReports.Special.Criteria.MarginReport;
    using Fanex.AccountingReports.Special.Services.Implement.MarginReport;
    using Fanex.NAP.Core.Business;
    using Fanex.NAP.Core.Web.Attribute;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class MarginReportController : BaseController
    {
        private MarginWinLossService _marginService;

        public MarginReportController()
        {
            this._marginService = new MarginWinLossService();
        }

        [HttpGet]
        [UserActivityFilter]
        [NapAuth(FunctionKey = AccReportFunctionKeys.MarginReport, Policy = Constant.Policy.Read)]
        public ActionResult Index()
        {
            return View("~/Areas/Special/Views/MarginReport/Index.cshtml");
        }

        [HttpPost]
        [HandleAjaxError]
        public JsonResult GetDataMarginReport(GetMarginWinLossCriteria criteria)
        {
            var report = this._marginService.GetDataMarginReport(criteria);
            var jsonResult = Json(report, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        public ActionResult ExportMarginReport(GetMarginWinLossCriteria criteria)
        {
            this._marginService.ExcelReport(criteria);
            return new EmptyResult();
        }
    }
}