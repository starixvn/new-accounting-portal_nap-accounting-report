namespace Fanex.AccountingReports.Misc.Controllers
{
    using System.Web.Mvc;
    using Fanex.AccountingReports.Core.Exceptions;
    using Fanex.AccountingReports.Core.Web.Controllers;
    using Fanex.AccountingReports.Misc.Criteria.TotalSMAMemberActive;
    using Fanex.AccountingReports.Misc.Services.Implement;
    using Fanex.AccountingReports.Misc.Services.Interface;
    using Fanex.NAP.Core.Business;
    using Fanex.NAP.Core.Web.Attribute;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class TotalSMAMemberActiveController : BaseController
    {
        private readonly ITotalSMAMemberActiveService _reportService;

        public TotalSMAMemberActiveController()
        {
            this._reportService = new TotalSMAMemberActiveService();
        }

        [HttpGet]
        [HandleAjaxError]
        [NapAuth(FunctionKey = AccReportFunctionKeys.MiscTotalSMAandMemberActive, Policy = Constant.Policy.Read)]
        public ActionResult ExportReport(GetTotalSMAMemberActiveCriteria criteria)
        {
            this._reportService.ExportReport(criteria);
            return new EmptyResult();
        }

        [HttpGet]
        [HandleAjaxError]
        [NapAuth(FunctionKey = AccReportFunctionKeys.MiscTotalSMAandMemberActive, Policy = Constant.Policy.Read)]
        public ActionResult ExportStatusDetails(GetStatusDetailsCriteria criteria)
        {
            this._reportService.ExportStatusDetails(criteria);
            return new EmptyResult();
        }

        [HttpPost]
        [HandleAjaxError]
        [NapAuth(FunctionKey = AccReportFunctionKeys.MiscTotalSMAandMemberActive, Policy = Constant.Policy.Read)]
        public JsonResult GetReport(GetTotalSMAMemberActiveCriteria criteria)
        {
            var report = this._reportService.GetReport(criteria);
            return Json(report, JsonRequestBehavior.DenyGet);
        }

        [HttpPost]
        [HandleAjaxError]
        [NapAuth(FunctionKey = AccReportFunctionKeys.MiscTotalSMAandMemberActive, Policy = Constant.Policy.Read)]
        public JsonResult GetSites(GetSitesCriteria criteria)
        {
            var sites = this._reportService.GetSites(criteria);
            return Json(sites, JsonRequestBehavior.DenyGet);
        }

        [HttpPost]
        [HandleAjaxError]
        [NapAuth(FunctionKey = AccReportFunctionKeys.MiscTotalSMAandMemberActive, Policy = Constant.Policy.Read)]
        public JsonResult GetStatusDetails(GetStatusDetailsCriteria criteria)
        {
            var details = this._reportService.GetStatusDetails(criteria);
            return Json(
                new
                {
                    TotalRow = criteria.TotalRow,
                    Report = details
                },
                JsonRequestBehavior.DenyGet);
        }

        [HttpGet]
        [NapAuth(FunctionKey = AccReportFunctionKeys.MiscTotalSMAandMemberActive, Policy = Constant.Policy.Read)]
        public ActionResult Index()
        {
            return View(@"~\Areas\Misc\Views\TotalSMAMemberActive\Index.cshtml");
        }
    }
}
