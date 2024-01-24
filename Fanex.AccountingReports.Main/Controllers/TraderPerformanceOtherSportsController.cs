namespace Fanex.AccountingReports.Main.Controllers
{
    using System.Web.Mvc;
    using Core.Web;
    using Fanex.AccountingReports.Core.Exceptions;
    using Fanex.AccountingReports.Core.Web.Controllers;
    using Fanex.AccountingReports.Main.Criteria.TraderPerformance;
    using Fanex.AccountingReports.Main.Services.Implement.TraderPerformanceOtherSports;
    using Fanex.NAP.Core.Business;
    using Fanex.NAP.Core.Web.Attribute;

    public class TraderPerformanceOtherSportsController : BaseController
    {
        private readonly TraderPerformanceOtherSportsService _reportService;

        public TraderPerformanceOtherSportsController()
        {
            this._reportService = new TraderPerformanceOtherSportsService();
        }

        [HttpGet]
        [UserActivityFilter]
        [NapAuth(FunctionKey = AccReportFunctionKeys.TraderPerformanceOtherSports, Policy = Constant.Policy.Read)]
        public ActionResult Index()
        {
            return View("~/Areas/Main/Views/TraderPerformanceOtherSports/Index.cshtml");
        }

        [HttpPost]
        [HandleAjaxError]
        public JsonResult GetTraderPerformanceForOtherSports(GetReportCriteria criteria)
        {
            var result = _reportService.GetReport(criteria);
            var jsonResult = Json(result);
            jsonResult.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        [HttpPost]
        [HandleAjaxError]
        public JsonResult GetTraderPerformanceForOtherSportsDetail(GetDetailReportCriteria criteria)
        {
            var result = _reportService.GetDetailReport(criteria);
            var jsonResult = Json(result);
            jsonResult.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        [HttpPost]
        [HandleAjaxError]
        public JsonResult SortTraderPerformanceForOtherSports(GetReportCriteria criteria)
        {
            var result = _reportService.SortReportData(criteria);
            var jsonResult = Json(result);
            jsonResult.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        [HttpPost]
        [HandleAjaxError]
        public JsonResult SortTraderPerformanceForOtherSportsDetail(GetDetailReportCriteria criteria)
        {
            var result = _reportService.SortDetailReportData(criteria);
            var jsonResult = Json(result);
            jsonResult.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        public ActionResult ExportReport(GetReportCriteria criteria)
        {
            this._reportService.ExportReport(criteria);
            return new EmptyResult();
        }

        public ActionResult ExportDetailReport(GetDetailReportCriteria criteria)
        {
            this._reportService.ExportDetailReport(criteria);
            return new EmptyResult();
        }
    }
}
