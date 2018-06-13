namespace Fanex.AccountingReports.Racing.Controllers
{
    using System.Web.Mvc;
    using AccountingReports.Core.Web;
    using Fanex.AccountingReports.Core.Exceptions;
    using Fanex.AccountingReports.Core.Web.Controllers;
    using Fanex.AccountingReports.Racing.Criteria.RacingGroupDaily;
    using Fanex.AccountingReports.Racing.Services.Implement.RacingGroupDaily;
    using Fanex.AccountingReports.Racing.Services.Interface.RacingGroupDaily;
    using Fanex.NAP.Core.Business;
    using Fanex.NAP.Core.Web.Attribute;

    /// <summary>
    /// The controller of Racing Group Daily report.
    /// </summary>
    public class RacingGroupDailyController : BaseController
    {
        private readonly IRacingGroupDailyGetReportServices _getReportService;
        private readonly IRacingGroupDailyExportReportServices _exportReportService;

        public RacingGroupDailyController()
            {
            this._getReportService = new RacingGroupDailyGetReportServices();
            this._exportReportService = new RacingGroupDailyExportReportServices();
        }

        [HttpGet]
        [UserActivityFilter]
        [NapAuth(FunctionKey = AccReportFunctionKeys.RacingGroupDaily, Policy = Constant.Policy.Read)]
        public ActionResult Index()
        {
            return View("~/Areas/Racing/Views/RacingGroupDaily/Index.cshtml");
        }

        [HttpPost]
        [HandleAjaxError]
        public JsonResult GetReport(GetWinLostRacingGroupCriteria criteria)
        {
            var report = this._getReportService.GetReport(criteria);
            var jsonResult = Json(report);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        public EmptyResult ExportReport(GetWinLostRacingGroupCriteria criteria)
        {
            this._exportReportService.ExportReport(criteria);
            return new EmptyResult();
        }
    }
}
