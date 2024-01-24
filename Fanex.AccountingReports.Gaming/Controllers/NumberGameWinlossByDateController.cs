namespace Fanex.AccountingReports.Gaming.Controllers
{
    using System.Web.Mvc;
    using AccountingReports.Core.Web;
    using Fanex.AccountingReports.Core.Exceptions;
    using Fanex.AccountingReports.Core.Web.Controllers;
    using Fanex.AccountingReports.Gaming.Criteria;
    using Fanex.AccountingReports.Gaming.Criteria.NumberGameWinLossByDate;
    using Fanex.AccountingReports.Gaming.Services.Implement;
    using Fanex.NAP.Core.Business;
    using Fanex.NAP.Core.Web.Attribute;

    public class NumberGameWinlossByDateController : BaseController
    {
        private readonly NumberGameWinlossByDateService _numberGameWinlossByDateService;
        private readonly GamingService _reportService;

        public NumberGameWinlossByDateController()
        {
            this._numberGameWinlossByDateService = new NumberGameWinlossByDateService();
            _reportService = new GamingService();
        }

        [HttpGet]
        [UserActivityFilter]
        [NapAuth(FunctionKey = AccReportFunctionKeys.NumberGameWinLossByDate, Policy = Constant.Policy.Read)]
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public JsonResult GetLicensee(GetNumberGameLicenseeCriteria getNumberGameLicenseeCriteria)
        {
            var result = this._numberGameWinlossByDateService.GetLicensee(getNumberGameLicenseeCriteria);
            return Json(result);
        }

        [HttpPost]
        public JsonResult GetBettypes(GetNumberGameBetTypeListCriteria getNumberGameBettypeCriteria)
        {
            var result = _reportService.GetBetTypes(getNumberGameBettypeCriteria);
            return Json(result);
        }

        [HttpPost]
        public JsonResult GetCurrencies()
        {
            var result = this._numberGameWinlossByDateService.GetCurrencies();
            return Json(result);
        }

        [HttpPost]
        [HandleAjaxError]
        public JsonResult GetNumberGameWinlossByDate(GetNumberGameWinLossByDateCriteria getNumberGameWinlossByDateCriteria)
        {
            var result = this._numberGameWinlossByDateService.GetNumberGameWinLossByDate(getNumberGameWinlossByDateCriteria);
            return Json(result);
        }

        public ActionResult ExportReport(GetNumberGameWinLossByDateCriteria getNumberGameWinlossByDateCriteria)
        {
            this._numberGameWinlossByDateService.ExportNumberGameByDate(getNumberGameWinlossByDateCriteria);
            return new EmptyResult();
        }
    }
}
