namespace Fanex.AccountingReports.Gaming.Controllers
{
    using System.Web.Mvc;
    using Fanex.AccountingReports.Core.Exceptions;
    using Fanex.AccountingReports.Core.Web.Controllers;
    using Fanex.AccountingReports.Gaming.Criteria.CasinoWinLoss;
    using Fanex.AccountingReports.Gaming.Models.CasinoWinLoss;
    using Fanex.AccountingReports.Gaming.Services.Implement.CasinoWinLoss;
    using Fanex.AccountingReports.Gaming.Services.Interface.CasinoWinLoss;
    using Fanex.NAP.Core.Business;
    using Fanex.NAP.Core.Web.Attribute;

    /// <summary>
    /// The controller of CasinoWinLost report.
    /// </summary>
    public class CasinoWinLossController : BaseController
    {
        private ICasinoWinLossGetReportService _getReportService;

        private ICasinoWinLossExportReportService _exportReportService;

        public CasinoWinLossController()
        {
            _getReportService = new CasinoWinLossGetReportService();
            _exportReportService = new CasinoWinLossExportReportService();
        }

        [HttpGet]
        [NapAuth(FunctionKey = AccReportFunctionKeys.CasinoWinLoss, Policy = Constant.Policy.Read)]
        public ActionResult Index()
        {
            return View("~/Areas/Gaming/Views/CasinoWinLoss/Index.cshtml");
        }

        [HttpPost]
        [HandleAjaxError]
        public ActionResult GetReport(GetCasinoWinLossCriteria criteria)
        {
            CasinoWinLoss report = _getReportService.GetReport(criteria);
            var jsonResult = Json(report);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        public EmptyResult ExportReport(GetCasinoWinLossCriteria criteria)
        {
            _exportReportService.ExportReport(criteria);
            return new EmptyResult();
        }
    }
}
