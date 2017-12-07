namespace Fanex.AccountingReports.Misc.Controllers
{
    using System.Web.Mvc;
    using Fanex.AccountingReports.Core.Web.Controllers;
    using Fanex.AccountingReports.Misc.Criteria.BetCount;
    using Fanex.AccountingReports.Misc.Services.Implement;
    using Fanex.AccountingReports.Misc.Services.Interface;
    using Fanex.NAP.Core.Business;
    using Fanex.NAP.Core.Web.Attribute;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class BetCountController : BaseController
    {
        private readonly IBetCountService _reportService;

        public BetCountController()
        {
            this._reportService = new BetCountService();
        }

        [HttpGet]
        [NapAuth(FunctionKey = AccReportFunctionKeys.BetCount, Policy = Constant.Policy.Read)]
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public JsonResult GetProducts(GetProductsCriteria criteria)
        {
            var listProducts = this._reportService.GetProducts(criteria);
            return Json(listProducts, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetSports(GetProductsCriteria criteria)
        {
            var listSports = this._reportService.GetSports(criteria);
            return Json(listSports, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetReport(GetBetCountReportCriteria criteria)
        {
            var report = this._reportService.GetReport(criteria);
            return Json(report, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult SortReport(GetBetCountReportCriteria criteria)
        {
            var report = this._reportService.SortReport(criteria);
            return Json(report, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ExportReport(GetBetCountReportCriteria criteria)
        {
            this._reportService.ExportReport(criteria);
            return new EmptyResult();
        }
    }
}
