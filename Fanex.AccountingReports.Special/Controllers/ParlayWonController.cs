namespace Fanex.AccountingReports.Special.Controllers
{
    using System.Web.Mvc;
    using Core.Web;
    using Fanex.AccountingReports.Core.Web.Controllers;
    using Fanex.AccountingReports.Special.Criteria.ParlayWon;
    using Fanex.AccountingReports.Special.Services.Implement;
    using Fanex.NAP.Core.Business;
    using Fanex.NAP.Core.Web.Attribute;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class ParlayWonController : BaseController
    {
        private readonly ParlayWonService _reportService;

        public ParlayWonController()
        {
            this._reportService = new ParlayWonService();
        }

        [HttpGet]
        [UserActivityFilter]
        [NapAuth(FunctionKey = AccReportFunctionKeys.ParlayWon, Policy = Constant.Policy.Read)]
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public JsonResult GetReport(GetParlayWonReportCriteria criteria)
        {
            var currencyReport = this._reportService.GetReport(criteria);
            return Json(currencyReport, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult SortReport(GetParlayWonReportCriteria criteria)
        {
            var report = this._reportService.SortReport(criteria);
            return Json(report, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ExportReport(GetParlayWonReportCriteria criteria)
        {
            this._reportService.ExportReport(criteria);
            return new EmptyResult();
        }
    }
}