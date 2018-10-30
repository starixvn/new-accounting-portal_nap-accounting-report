namespace Fanex.AccountingReports.Gaming.Controllers
{
    using System.Web.Mvc;
    using AccountingReports.Core.Web;
    using Fanex.AccountingReports.Core.Exceptions;
    using Fanex.AccountingReports.Core.Web.Controllers;
    using Fanex.AccountingReports.Gaming.Criteria.NumberGameWinLossByCurrency;
    using Fanex.AccountingReports.Gaming.Services.NumberGameWinLossByCurrency;
    using Fanex.NAP.Core.Business;
    using Fanex.NAP.Core.Web.Attribute;

    public class NumberGameWinLossByCurrencyController : BaseController
    {
        private readonly NumberGameWinLossByCurrencyService _reportService;

        public NumberGameWinLossByCurrencyController()
        {
            _reportService = new NumberGameWinLossByCurrencyService();
        }

        [HttpGet]
        [UserActivityFilter]
        [NapAuth(FunctionKey = AccReportFunctionKeys.NumberGameWinLossByCurrency, Policy = Constant.Policy.Read)]
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [HandleAjaxError]
        public JsonResult GetNumberGameWinlossByCur(NumberGameWinLossByCurrencyMainCriteria getNumberGameWinlossByCurCriteria)
        {
            var result = this._reportService.GetNumberGameWinLossByCurrencyMain(getNumberGameWinlossByCurCriteria);
            return Json(result);
        }

        [HttpPost]
        [HandleAjaxError]
        public JsonResult GetNumberGameWinlossDetailByCur(NumberGameWinLossByCurrencyDetailCriteria getNumberGameWinlossByCurDetailCriteria)
        {
            var result = this._reportService.GetNumberGameWinLossByCurrencyDetail(getNumberGameWinlossByCurDetailCriteria);
            return Json(result);
        }

        [HttpPost]
        public JsonResult GetCurrencies()
        {
            var result = this._reportService.GetCurrencies();
            return Json(result);
        }

        public ActionResult ExportReportMain(NumberGameWinLossByCurrencyMainCriteria getNumberGameWinlossByCurCriteria)
        {
            this._reportService.GetExportExcelNumberGameWinLossByCurMain(getNumberGameWinlossByCurCriteria);
            return new EmptyResult();
        }

        public ActionResult ExportReportDetail(NumberGameWinLossByCurrencyDetailCriteria getNumberGameWinlossByCurDetailCriteria)
        {
            this._reportService.GetExportExcelNumberGameWinLossByCurDetail(getNumberGameWinlossByCurDetailCriteria);
            return new EmptyResult();
        }
    }
}
