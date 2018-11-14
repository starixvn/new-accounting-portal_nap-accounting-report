namespace Fanex.AccountingReports.Misc.Controllers
{
    using System.Web.Mvc;
    using Core.Web;
    using Fanex.AccountingReports.Core.Exceptions;
    using Fanex.AccountingReports.Core.Web.Controllers;
    using Fanex.AccountingReports.Misc.Criteria.SMATurnover;
    using Fanex.AccountingReports.Misc.Services.Implement.SMATurnover;
    using Fanex.NAP.Core.Business;
    using Fanex.NAP.Core.Web.Attribute;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class SMATurnoverController : BaseController
    {
        private SMATurnoverService _smasTurnOverService;

        public SMATurnoverController()
        {
            _smasTurnOverService = new SMATurnoverService();
        }

        [HttpGet]
        [UserActivityFilter]
        [NapAuth(FunctionKey = AccReportFunctionKeys.SmaTurnOver, Policy = Constant.Policy.Read)]
        public ActionResult Index()
        {
            return View("~/Areas/Misc/Views/SMATurnover/Index.cshtml");
        }

        [HttpPost]
        [HandleAjaxError]
        public ActionResult GetProducts()
        {
            var products = _smasTurnOverService.GetProducts();
            return Json(products, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [HandleAjaxError]
        public ActionResult GetAccountStatus()
        {
            var accountStatus = _smasTurnOverService.GetAccountStatus();
            return Json(accountStatus);
        }

        [HttpPost]
        [HandleAjaxError]
        public ActionResult GetSMAsTurnover(GetSMATurnoverReportCriteria criteria)
        {
            var result = _smasTurnOverService.GetSMAsTurnover(criteria);
            var jsonResult = Json(result);
            jsonResult.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        [HttpPost]
        [HandleAjaxError]
        public ActionResult SortReport(GetSMATurnoverReportCriteria criteria)
        {
            var result = _smasTurnOverService.SortReport(criteria);
            var jsonResult = Json(result);
            jsonResult.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        public ActionResult ExportData(GetSMATurnoverReportCriteria criteria)
        {
            _smasTurnOverService.ExportReportData(criteria);
            return new EmptyResult();
        }
    }
}