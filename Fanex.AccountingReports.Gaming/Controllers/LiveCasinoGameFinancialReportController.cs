namespace Fanex.AccountingReports.Gaming.Controllers
{
    using System.Web.Mvc;
    using AccountingReports.Core.Web;
    using Fanex.AccountingReports.Core.Web.Controllers;
    using Fanex.AccountingReports.Gaming.Criteria.LiveCasinoGameFinancialReport;
    using Fanex.AccountingReports.Gaming.Services.LiveCasinoGameFinancialReport;
    using Fanex.NAP.Core.Business;
    using Fanex.NAP.Core.Web.Attribute;

    public class LiveCasinoGameFinancialReportController : BaseController
    {
        private readonly ILiveCasinoGameFinancialReportService _reportService;

        public LiveCasinoGameFinancialReportController()
        {
            this._reportService = new LiveCasinoGameFinancialReportService();
        }

        [UserActivityFilter]
        [NapAuth(FunctionKey = AccReportFunctionKeys.LiveCasinoGameFinancialReport, Policy = Constant.Policy.Read)]
        public ActionResult Index()
        {
            return View("~/Areas/Gaming/Views/LiveCasinoGameFinancialReport/Index.cshtml");
        }

        [HttpPost]
        public JsonResult GetLicensees()
        {
            var reseller = this._reportService.GetLicensees();
            return Json(reseller, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetCurrencies()
        {
            var currencies = this._reportService.GetCurrencies();
            return Json(currencies, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetGameTypes(GetGameTypesCriteria criteria)
        {
            var gameTypes = this._reportService.GetGameTypes(criteria);
            return Json(gameTypes, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetGameTables(GetGameTableCriteria criteria)
        {
            var tables = this._reportService.GetGameTables(criteria);
            return Json(tables, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetFinancialReport(GetFinancialReportCriteria criteria)
        {
            var tables = this._reportService.GetFinancialReport(criteria);
            return Json(tables, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ExportFinancialReport(GetFinancialReportCriteria criteria)
        {
            this._reportService.ExportFinancialReport(criteria);
            return new EmptyResult();
        }
    }
}
