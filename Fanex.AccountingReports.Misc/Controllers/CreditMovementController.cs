namespace Fanex.AccountingReports.Misc.Controllers
{
    using System.Web.Mvc;
    using Core.Web;
    using Fanex.AccountingReports.Core.Exceptions;
    using Fanex.AccountingReports.Core.Extensions;
    using Fanex.AccountingReports.Core.Web.Controllers;
    using Fanex.AccountingReports.Misc.Criteria.CreditMovement;
    using Fanex.AccountingReports.Misc.Resources;
    using Fanex.AccountingReports.Misc.Services.Implement.CreditMovement;
    using Fanex.Excel;
    using Fanex.Excel.Extensions;
    using Fanex.NAP.Core.Business;
    using Fanex.NAP.Core.Web.Attribute;

    public class CreditMovementController : BaseController
    {
        private CreditMovementService _service;

        private string _amount = "300,000";

        public CreditMovementController()
        {
            _service = new CreditMovementService();
        }

        [UserActivityFilter]
        [NapAuth(FunctionKey = AccReportFunctionKeys.CreditMovement, Policy = Constant.Policy.Read)]
        public ActionResult Index()
        {
            ViewBag.Amount = _amount;
            return View("~/Areas/Misc/Views/CreditMovement/Index.cshtml");
        }

        public JsonResult GetSites()
        {
            var site = _service.GetSites();
            return Json(site);
        }

        [HttpPost]
        [HandleAjaxError]
        public JsonNetResult GetReport(GetCreditMovementCriteria criteria)
        {
            var report = _service.GetReport(criteria);
            return new JsonNetResult(report);
        }

        public EmptyResult ExportReport(GetCreditMovementCriteria criteria)
        {
            FanexExcel result = new FanexExcel(_service.GetFullReportFromCache(criteria));
            NpoiExcelExtensions.ResponseStreamExcel(
                result.ToExcel2003(MiscResources.BetCountReport_SheetName),
                _service.GetReportName(criteria));
            return new EmptyResult();
        }
    }
}
