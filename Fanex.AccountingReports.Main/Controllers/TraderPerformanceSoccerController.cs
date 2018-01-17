namespace Fanex.AccountingReports.Main.Controllers
{
    using System.IO;
    using System.Web.Mvc;
    using Fanex.AccountingReports.Core.Exceptions;
    using Fanex.AccountingReports.Core.Web.Controllers;
    using Fanex.AccountingReports.Main.Criteria.TraderPerformance;
    using Fanex.AccountingReports.Main.Services.Implement.TraderPerformanceSoccer;
    using Fanex.AccountingReports.Main.Services.Interface;
    using Fanex.NAP.Core.Business;
    using Fanex.NAP.Core.Web.Attribute;
    using OfficeOpenXml;
    using Repositories.TraderPerformanceSoccerPH;
    using Services.TraderPerformanceSoccerPH;

    /// <summary>
    /// Trader performance for soccer controller.
    /// </summary>
    public class TraderPerformanceSoccerController : BaseController
    {
        private readonly ITraderPerformanceSoccerService reportService;        

        public TraderPerformanceSoccerController()
            {
            this.reportService = new TraderPerformanceSoccerService();
        }

        [HttpGet]
        [UserActivityFilter]
        [NapAuth(FunctionKey = AccReportFunctionKeys.TraderPerformanceSoccer, Policy = Constant.Policy.Read)]
        public ActionResult Index()
        {
            return View("~/Areas/Main/Views/TraderPerformanceSoccer/Index.cshtml");
        }

        [HttpPost]
        [HandleAjaxError]
        public JsonResult GetTraderPerformanceSoccer(GetReportCriteria criteria)
        {
            var result = reportService.GetReport(criteria);
            var jsonResult = Json(result);
            jsonResult.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        [HttpPost]
        [HandleAjaxError]
        public JsonResult GetTraderPerformanceSoccerDetail(GetDetailReportCriteria criteria)
        {
            var result = reportService.GetDetailReport(criteria);
            var jsonResult = Json(result);
            jsonResult.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        [HttpPost]
        [HandleAjaxError]
        public JsonResult SortTraderPerformanceSoccer(GetReportCriteria criteria)
        {
            var result = reportService.SortReportData(criteria);
            var jsonResult = Json(result);
            jsonResult.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        [HttpPost]
        [HandleAjaxError]
        public JsonResult SortTraderPerformanceSoccerDetail(GetDetailReportCriteria criteria)
        {
            var result = reportService.SortDetailReportData(criteria);
            var jsonResult = Json(result);
            jsonResult.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        public ActionResult ExportReport(GetReportCriteria criteria)
        {
            this.reportService.ExportReport(criteria);
            return new EmptyResult();
        }

        public ActionResult ExportDetailReport(GetDetailReportCriteria criteria)
        {
            this.reportService.ExportDetailReport(criteria);
            return new EmptyResult();
        }

        public ActionResult ExportSummaryPH(GetSummaryPHCriteria criteria)
        {
            var path = @"App_Data\Templates\TraderPerformance_Soccer_Sum.xlsx";
            var traderDataService = new TraderDataServiceImp();
            var excelPackage = new ExcelPackage(new FileInfo(path));
            var traderSumPivotServiceImp = new TraderSumPivotServiceImp();
            var excelTraderSumPHService = new TraderSumExcelServiceImp(traderDataService, traderSumPivotServiceImp, excelPackage);

            excelTraderSumPHService.Execute(criteria);
            return new EmptyResult();
        }

        public ActionResult ExportDetailPH(GetDetailPHCriteria criteria)
        {
            var path = @"App_Data\Templates\TraderPerformance_Soccer_Detail.xlsx";
            var traderDataService = new TraderDataServiceImp();
            var excelPackage = new ExcelPackage(new FileInfo(path));
            var traderDetailPivotService = new TraderDetailPivotServiceImp();
            var excelTraderDetailPHService = new TraderDetailExcelServiceImp(traderDataService, traderDetailPivotService, excelPackage);
            excelTraderDetailPHService.Execute(criteria);
            return new EmptyResult();
        }
    }
}
