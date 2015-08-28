// --------------------------------------------------------------------------
// <copyright file="SpecialMatchAnalysisController.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Louis.Nguyen" date="2012.12.07">Created</change>
// </history>

namespace Fanex.AccountingReports.Special.Controllers
{
    using System.Web.Mvc;
    using Fanex.AccountingReports.Core.Web.Controllers;
    using Fanex.AccountingReports.Special.Criteria.SpecialMatchAnalysis;
    using Fanex.AccountingReports.Special.Services.Implement;
    using Fanex.AccountingReports.Special.Services.Interface;
    using Fanex.NAP.Core.Business;
    using Fanex.NAP.Core.Web.Attribute;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    [NapSuperAuth]
    public class SpecialMatchAnalysisController : BaseController
    {
        private readonly ISpecialMatchAnalysisService _reportService;

        public SpecialMatchAnalysisController()
            : this(null)
        {
        }

        public SpecialMatchAnalysisController(ISpecialMatchAnalysisService reportService)
        {
            this._reportService = reportService ?? new SpecialMatchAnalysisService();
        }

        [HttpGet]
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
