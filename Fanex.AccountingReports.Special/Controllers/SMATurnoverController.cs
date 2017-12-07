namespace Fanex.AccountingReports.Special.Controllers
{
    using System.Web.Mvc;
    using Fanex.AccountingReports.Core.Exceptions;
    using Fanex.AccountingReports.Core.Web.Controllers;
    using Fanex.AccountingReports.Special.Criteria.SMATurnover;
    using Fanex.AccountingReports.Special.Services.Implement.SMATurnover;
    using Fanex.AccountingReports.Special.Services.Interface;
    using Fanex.NAP.Core.Business;
    using Fanex.NAP.Core.Web.Attribute;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class SMATurnoverController : BaseController
    {
        private ISMATurnoverService _smasTurnOverService;

        public SMATurnoverController()
        {
            _smasTurnOverService = new SMATurnoverService();
        }

        [HttpGet]
        [NapAuth(FunctionKey = AccReportFunctionKeys.SmaTurnOver, Policy = Constant.Policy.Read)]
        public ActionResult Index()
        {
            return View("~/Areas/Special/Views/SMATurnover/Index.cshtml");
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
        public ActionResult GetSMAsTurnover(SMATurnoverCriteria criteria)
        {
            var result = _smasTurnOverService.GetSMAsTurnover(criteria);
            var jsonResult = Json(result);
            jsonResult.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        [HttpPost]
        [HandleAjaxError]
        public ActionResult SortReport(SMATurnoverCriteria criteria)
        {
            var result = _smasTurnOverService.SortReport(criteria);
            var jsonResult = Json(result);
            jsonResult.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        public ActionResult ExportData(SMATurnoverCriteria criteria)
        {
            _smasTurnOverService.ExportReportData(criteria);
            return new EmptyResult();
        }
    }
}
