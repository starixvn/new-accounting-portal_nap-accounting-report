namespace Fanex.AccountingReports.Special.Controllers
{
    using Core.Web;
    using Fanex.AccountingReports.Core.Configurations;
    using Fanex.AccountingReports.Core.Exceptions;
    using Fanex.AccountingReports.Core.Web.Controllers;
    using Fanex.AccountingReports.Special.Criteria.SaveBetReport;
    using Fanex.AccountingReports.Special.Services.Implement.SaveBetReport;
    using Fanex.Common;
    using Fanex.NAP.Core.Business;
    using Fanex.NAP.Core.Web.Attribute;
    using System.IO;
    using System.Net;
    using System.Web.Mvc;

    /// <summary>
    /// Save bet report controller.
    /// </summary>
    public class SaveBetReportController : BaseController
    {
        private readonly SaveBetReportService _saveBetReportService;
        
        public SaveBetReportController()
        {
            this._saveBetReportService = new SaveBetReportService();
        }

        [HttpGet]
        [UserActivityFilter]
        [NapAuth(FunctionKey = AccReportFunctionKeys.SaveBet, Policy = Constant.Policy.Read)]
        public ActionResult Index()
        {
            return View("~/Areas/Special/Views/SaveBetReport/Index.cshtml");
        }

        [HttpPost]
        [HandleAjaxError]
        public ActionResult GetSaveBetReport(GetMatchsCriteria criteria)
        {
            var result = this._saveBetReportService.GetMatchs(criteria);
            var jsonResult = Json(result);
            jsonResult.MaxJsonLength = int.MaxValue;
            jsonResult.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return jsonResult;
        }

        [HttpPost]
        [HandleAjaxError]
        public ActionResult GetSportsByDate(GetSportsCriteria criteria)
        {
            var result = this._saveBetReportService.GetSports(criteria);
            var jsonResult = Json(result);
            jsonResult.MaxJsonLength = int.MaxValue;
            jsonResult.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return jsonResult;
        }

        [HttpPost]
        [HandleAjaxError]
        public ActionResult GetLeaguesBySport(GetLeaguesCriteria criteria)
        {
            var result = this._saveBetReportService.GetLeagues(criteria);
            var jsonResult = Json(result);
            jsonResult.MaxJsonLength = int.MaxValue;
            jsonResult.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return jsonResult;
        }
    }
}