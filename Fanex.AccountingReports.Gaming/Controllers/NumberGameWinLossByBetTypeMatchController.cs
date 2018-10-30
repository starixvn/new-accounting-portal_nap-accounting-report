namespace Fanex.AccountingReports.Gaming.Controllers
{
    using System.Web.Mvc;
    using AccountingReports.Core.Web;
    using Fanex.AccountingReports.Core.Exceptions;
    using Fanex.AccountingReports.Core.Web.Controllers;
    using Fanex.AccountingReports.Gaming.Criteria.NumberGameWinLossByBetTypeMatch;
    using Fanex.AccountingReports.Gaming.Services.Implement.NumberGameWinLossByBetTypeMatch;
    using Fanex.NAP.Core.Business;
    using Fanex.NAP.Core.Web.Attribute;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class NumberGameWinLossByBetTypeMatchController : BaseController
    {
        private readonly NumberGameWinLossByBetTypeMatchService _numberGameWinLossService;

        private readonly WinLossByBetTypeService _winLossByBetTypeService;

        private readonly WinLossByMatchService _winLossByMatchService;

        public NumberGameWinLossByBetTypeMatchController()
        {
            _numberGameWinLossService = new NumberGameWinLossByBetTypeMatchService();
            _winLossByBetTypeService = new WinLossByBetTypeService();
            _winLossByMatchService = new WinLossByMatchService();
        }

        [HttpGet]
        [UserActivityFilter]
        [NapAuth(FunctionKey = AccReportFunctionKeys.NumberGameWinLoss, Policy = Constant.Policy.Read)]
        public ActionResult Index()
        {
            return View("~/Areas/Gaming/Views/NumberGameWinLossByBetTypeMatch/Index.cshtml");
        }

        [HttpPost]
        [HandleAjaxError]
        public JsonResult GetCurrencyList()
        {
            var result = _numberGameWinLossService.GetCurrencyList();
            return Json(result);
        }

        [HttpPost]
        [HandleAjaxError]
        public JsonResult GetWinLossByBetTypeMainReport(GetWinLossByBetTypeMainReportCriteria criteria)
        {
            var result = _winLossByBetTypeService.GetMainReport(criteria);
            return Json(result);
        }

        [HttpPost]
        [HandleAjaxError]
        public JsonResult GetWinLossByBetTypeDetailReport(GetWinLossByBetTypeDetailReportCriteria criteria)
        {
            var result = _winLossByBetTypeService.GetDetailReport(criteria);
            return Json(result);
        }

        [HttpPost]
        [HandleAjaxError]
        public JsonResult GetWinLossByMatchMainReport(GetWinLossByMatchMainReportCriteria criteria)
        {
            var result = _winLossByMatchService.GetMainReport(criteria);
            return Json(result);
        }

        [HttpPost]
        [HandleAjaxError]
        public JsonResult GetWinLossByMatchDetailReport(GetWinLossByMatchDetailReportCriteria criteria)
        {
            var result = _winLossByMatchService.GetDetailReport(criteria);
            return Json(result);
        }

        public ActionResult ExportWinLossByBetTypeMainReport(GetWinLossByBetTypeMainReportCriteria criteria)
        {
            _winLossByBetTypeService.ExportMainReport(criteria);
            return new EmptyResult();
        }

        public ActionResult ExportWinLossByBetTypeDetailReport(GetWinLossByBetTypeDetailReportCriteria criteria)
        {
            _winLossByBetTypeService.ExportDetailReport(criteria);
            return new EmptyResult();
        }

        public ActionResult ExportWinLossByMatchMainReport(GetWinLossByMatchMainReportCriteria criteria)
        {
            _winLossByMatchService.ExportMainReport(criteria);
            return new EmptyResult();
        }

        public ActionResult ExportWinLossByMatchDetailReport(GetWinLossByMatchDetailReportCriteria criteria)
        {
            _winLossByMatchService.ExportDetailReport(criteria);
            return new EmptyResult();
        }
    }
}
