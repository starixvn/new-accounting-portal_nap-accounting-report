namespace Fanex.AccountingReports.Misc.Controllers
{
    using System.Web.Mvc;
    using Core.Web;
    using Fanex.AccountingReports.Core.Exceptions;
    using Fanex.AccountingReports.Core.Web.Controllers;
    using Fanex.AccountingReports.Misc.Criteria.MAMonitorTool;
    using Fanex.AccountingReports.Misc.Services.Implement.MAMonitorTool;
    using Fanex.NAP.Core.Business;
    using Fanex.NAP.Core.Web.Attribute;

    /// <summary>
    /// MA Monitor Tool Controller.
    /// </summary>
    public class MAMonitorToolController : BaseController
    {
        private readonly MAMonitorToolService _reportService;

        private readonly MAMonitorToolService _exportReportService;

        public MAMonitorToolController()
        {
            _reportService = new MAMonitorToolService();
            _exportReportService = new MAMonitorToolExportExcelService();
        }

        [HttpGet]
        [UserActivityFilter]
        [NapAuth(FunctionKey = AccReportFunctionKeys.MAMonitorTool, Policy = Constant.Policy.Read)]
        public ActionResult Index()
        {
            return View("~/Areas/Misc/Views/MAMonitorTool/Index.cshtml");
        }

        [HttpPost]
        [HandleAjaxError]
        public JsonResult GetReportTypeList()
        {
            var result = _reportService.GetReportTypeList();
            return Json(result);
        }

        [HttpPost]
        [HandleAjaxError]
        public JsonResult GetProductList()
        {
            var result = _reportService.GetProductList();
            return Json(result);
        }

        [HttpPost]
        [HandleAjaxError]
        public JsonResult GetSportList()
        {
            var result = _reportService.GetSportList();
            return Json(result);
        }

        [HttpPost]
        [HandleAjaxError]
        public JsonResult GetBetTypeList(GetBetTypeListCriteria criteria)
        {
            var result = _reportService.GetProductBettype(criteria);
            return Json(result);
        }

        [HttpPost]
        [HandleAjaxError]
        public JsonResult GetCurrencyList(GetCurrencyListCriteria criteria)
        {
            var result = _reportService.GetCurrencyList();
            return Json(result);
        }

        [HttpPost]
        [HandleAjaxError]
        public JsonResult GetLeagueTypeList()
        {
            var result = _reportService.GetLeagueTypeList();
            return Json(result);
        }

        [HttpPost]
        [HandleAjaxError]
        public JsonResult GetLeagueList(GetLeagueListCriteria criteria)
        {
            var result = _reportService.GetLeagueList(criteria);
            return Json(result);
        }

        [HttpPost]
        [HandleAjaxError]
        public JsonResult GetReport(GetReportCriteria criteria)
        {
            var result = _reportService.GetReport(criteria);
            var jsonResult = Json(result, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        public ActionResult ExportReport(GetReportCriteria criteria)
        {
            criteria.IsExportExcel = true;
            _exportReportService.ExportReport(criteria);
            return new EmptyResult();
        }
    }
}