namespace Fanex.AccountingReports.Main.Controllers
{
    using System.Web.Mvc;
    using Core.Web;
    using Fanex.AccountingReports.Core.Web.Controllers;
    using Fanex.AccountingReports.Main.Criteria.WinLossSummary;
    using Fanex.AccountingReports.Main.Services.Implement.WinLossSummary;
    using Fanex.AccountingReports.Main.Services.Interface;
    using Fanex.NAP.Core.Business;
    using Fanex.NAP.Core.Web.Attribute;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class WinLossSummaryController : BaseController
    {
        private readonly IWinLossSummaryService _reportService;

        public WinLossSummaryController()
        {
            this._reportService = new WinLossSummaryService();
        }

        [HttpGet]
        [UserActivityFilter]
        [NapAuth(FunctionKey = AccReportFunctionKeys.WinLossSummary, Policy = Constant.Policy.Read)]
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public JsonResult GetReport(GetReportCriteria criteria)
        {
            var result = this._reportService.GetReport(criteria);

            var jsonResult = Json(result);
            jsonResult.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return jsonResult;
        }
    }
}
