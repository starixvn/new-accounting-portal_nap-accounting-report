// --------------------------------------------------------------------------
// <copyright file="ParlayWonController.cs" company="Nexcel Solutions Vietnam">
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
    using Fanex.AccountingReports.Special.Criteria.ParlayWon;
    using Fanex.AccountingReports.Special.Services.Implement;
    using Fanex.AccountingReports.Special.Services.Interface;
    using Fanex.NAP.Core.Business;
    using Fanex.NAP.Core.Web.Attribute;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class ParlayWonController : BaseController
    {
        private readonly IParlayWonService _reportService;

        public ParlayWonController()
        {
            this._reportService = new ParlayWonService();
        }

        [HttpGet]
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
