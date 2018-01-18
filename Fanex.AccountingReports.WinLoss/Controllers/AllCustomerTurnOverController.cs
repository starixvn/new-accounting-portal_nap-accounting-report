namespace Fanex.AccountingReports.WinLoss.Controllers
{
    using System.Web.Mvc;
    using Fanex.AccountingReports.Core.Exceptions;
    using Fanex.AccountingReports.Core.Web.Controllers;
    using Fanex.AccountingReports.WinLoss.Criteria.GetAllCustomerTurnOver;
    using Fanex.AccountingReports.WinLoss.Services.Implement;
    using Fanex.AccountingReports.WinLoss.Services.Implement.AllCustomerTurnOver;
    using Fanex.AccountingReports.WinLoss.Services.Interface;
    using Fanex.AccountingReports.WinLoss.Services.Interface.AllCustomerTurnOver;
    using Fanex.NAP.Core.Business;
    using Fanex.NAP.Core.Web.Attribute;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class AllCustomerTurnOverController : BaseController
    {
        private readonly IAllCustomerTurnOverGetReportService _getReportService;
        private readonly IAllCustomerTurnOverExportReportService _exportReportService;

        public AllCustomerTurnOverController()
        {
            this._getReportService = new AllCustomerTurnOverGetReportService();
            this._exportReportService = new AllCustomerTurnOverExportReportService();
        }

        [HttpGet]
        [UserActivityFilter]
        [NapAuth(FunctionKey = AccReportFunctionKeys.AllCustomerTurnoverBySite, Policy = Constant.Policy.Read)]
        public ActionResult Index()
        {
            return View("~/Areas/WinLoss/Views/AllCustomerTurnOver/Index.cshtml");
        }

        [HttpPost]
        [HandleAjaxError]
        public JsonResult GetReport(GetAllCustomerTurnOverCriteria criteria)
        {
            var report = this._getReportService.GetReport(criteria);
            var jsonResult = Json(report);
            jsonResult.MaxJsonLength = int.MaxValue;
            jsonResult.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return jsonResult;
        }

        public ActionResult ExportReport(GetAllCustomerTurnOverCriteria criteria)
        {
            this._exportReportService.ExportReport(criteria);
            return new EmptyResult();
        }
    }
}
