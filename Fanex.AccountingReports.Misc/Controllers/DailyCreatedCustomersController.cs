namespace Fanex.AccountingReports.Misc.Controllers
{
    using System.Web.Mvc;
    using Core.Web;
    using Fanex.AccountingReports.Core.Exceptions;
    using Fanex.AccountingReports.Core.Web.Controllers;
    using Fanex.AccountingReports.Misc.Criteria.DailyCreatedCustomers;
    using Fanex.AccountingReports.Misc.Services.Implement;
    using Fanex.AccountingReports.Misc.Services.Interface;
    using Fanex.NAP.Core.Business;
    using Fanex.NAP.Core.Web.Attribute;

    public class DailyCreatedCustomersController : BaseController
    {
        private readonly IDailyCreatedCustomersService _reportService;

        public DailyCreatedCustomersController()
        {
            this._reportService = new DailyCreatedCustomersService();
        }

        [HttpGet]
        [UserActivityFilter]
        [NapAuth(FunctionKey = AccReportFunctionKeys.DailyCreatedCustomers, Policy = Constant.Policy.Read)]
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [HandleAjaxError]
        public JsonResult GetReport(GetDailyCreatedCustomersCriteria criteria)
        {
            var report = this._reportService.GetReport(criteria);
            return Json(report, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [HandleAjaxError]
        public JsonResult GetReportDetails(GetDailyCreatedCustomersDetailsCriteria criteria)
        {
            var report = this._reportService.GetReportDetails(criteria);
            var jsonResult = Json(report, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        public ActionResult ExportReport(GetDailyCreatedCustomersCriteria criteria)
        {
            this._reportService.ExportReport(criteria);
            return new EmptyResult();
        }

        public ActionResult ExportReportDetails(GetDailyCreatedCustomersDetailsCriteria criteria)
        {
            this._reportService.ExportReportDetails(criteria);
            return new EmptyResult();
        }
    }
}
