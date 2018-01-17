namespace Fanex.AccountingReports.Gaming.Controllers
{
    using System.Web.Mvc;
    using Fanex.AccountingReports.Core.Criteria;
    using Fanex.AccountingReports.Core.Exceptions;
    using Fanex.AccountingReports.Core.Web.Controllers;
    using Fanex.AccountingReports.Gaming.Criteria.LiveCasinoWinLossByAgency;
    using Fanex.AccountingReports.Gaming.Services.Implement;
    using Fanex.AccountingReports.Gaming.Services.Implement.LiveCasinoWinLossByAgency;
    using Fanex.AccountingReports.Gaming.Services.Interface;
    using Fanex.NAP.Core.Business;
    using Fanex.NAP.Core.Web.Attribute;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class LiveCasinoAgencyWinLossController : BaseController
    {
        private readonly ILiveCasinoAgencyWinLossService _reportService;
        private readonly ILiveCasinoProductService _liveCasinoProductService;

        public LiveCasinoAgencyWinLossController()
        {
            this._reportService = new LiveCasinoAgencyWinLossService();
            this._liveCasinoProductService = new LiveCasinoProductService();
        }

        [HttpGet]
        [UserActivityFilter]
        [NapAuth(FunctionKey = AccReportFunctionKeys.LiveCasinoAgencyWinLoss, Policy = Constant.Policy.Read)]
        public ActionResult Index()
        {
            return View("~/Areas/Gaming/Views/LiveCasinoAgencyWinLoss/Index.cshtml");
        }

        [HttpPost]
        [HandleAjaxError]
        public JsonResult GetProductList()
        {
            var result = this._reportService.GetProductList();
            return Json(result);
        }

        [HttpPost]
        [HandleAjaxError]
        public JsonResult GetProductGameType(GetGameTypeListCriteria criteria)
        {
            var result = this._reportService.GetProductGameTypes(criteria);
            return Json(result);
        }

        [HttpPost]
        [HandleAjaxError]
        public JsonResult GetCurrencyList(GetExchangeCurrenciesCriteria criteria)
        {
            var result = this._reportService.GetCurrencyList(criteria);
            return Json(result);
        }

        [HttpPost]
        [HandleAjaxError]
        public JsonResult GetGameTableList(GetGameTableListCriteria criteria)
        {
            var result = this._reportService.GetGameTableList(criteria);
            return Json(result);
        }

        [HttpPost]
        [HandleAjaxError]
        public JsonResult GetReport(GetReportCriteria criteria)
        {
            var result = this._reportService.GetReport(criteria);
            var jsonResult = Json(result);
            jsonResult.MaxJsonLength = int.MaxValue;
            jsonResult.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return jsonResult;
        }

        [HttpPost]
        [HandleAjaxError]
        public JsonResult SortReport(GetReportCriteria getReportCriteria)
        {
            var result = this._reportService.SortReport(getReportCriteria);
            var jsonResult = Json(result);
            jsonResult.MaxJsonLength = int.MaxValue;
            jsonResult.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return jsonResult;
        }

        public ActionResult ExportReport(GetReportCriteria criteria)
        {
            this._reportService.ExportReport(criteria);
            return new EmptyResult();
        }
    }
}
