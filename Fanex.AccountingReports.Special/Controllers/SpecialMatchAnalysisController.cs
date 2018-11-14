namespace Fanex.AccountingReports.Special.Controllers
{
    using System.Web.Mvc;
    using Core.Web;
    using Fanex.AccountingReports.Core.Web.Controllers;
    using Fanex.AccountingReports.Special.Criteria.SpecialMatchAnalysis;
    using Fanex.AccountingReports.Special.Services.Implement;
    using Fanex.NAP.Core.Business;
    using Fanex.NAP.Core.Web.Attribute;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class SpecialMatchAnalysisController : BaseController
    {
        private readonly SpecialMatchAnalysisService _reportService;

        public SpecialMatchAnalysisController()
        {
            this._reportService = new SpecialMatchAnalysisService();
        }

        [HttpGet]
        [UserActivityFilter]
        [NapAuth(FunctionKey = AccReportFunctionKeys.SpecialMatchAnalysis, Policy = Constant.Policy.Read)]
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public JsonResult GetSportLeague(GetSportLeagueCriteria criteria)
        {
            var report = this._reportService.GetSportLeague(criteria);
            return Json(report, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetLeagueMatch(GetLeageMatchCriteria criteria)
        {
            var report = this._reportService.GetLeagueMatch(criteria);
            return Json(report, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetBettype(GetBetTypeCriteria criteria)
        {
            var report = this._reportService.GetBetType(criteria);
            return Json(report, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetMatchAnalysis(GetMatchAnalysisCriteria criteria)
        {
            var report = this._reportService.GetMatchAnalysis(criteria);
            return Json(report, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult ExportReport(GetMatchAnalysisCriteria criteria)
        {
            this._reportService.ExportReport(criteria);
            return new EmptyResult();
        }
    }
}