// --------------------------------------------------------------------------
// <copyright file="WinLossByLicenseeController.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Louis.Bao" date="2012.11.26">Created</change>
// </history>

namespace Fanex.AccountingReports.Licensee.Controllers
{
    using System.Web.Mvc;
    using Fanex.AccountingReports.Core.Web.Controllers;
    using Fanex.AccountingReports.Licensee.Criteria.WinLossByLicensee;
    using Fanex.AccountingReports.Licensee.Services.Implement;
    using Fanex.AccountingReports.Licensee.Services.Interface;
    using Fanex.NAP.Core.Business;
    using Fanex.NAP.Core.Web.Attribute;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class WinLossByLicenseeController : BaseController
    {
        private readonly IWinLossByLicenseeService _reportService;

        public WinLossByLicenseeController()
            : this(null)
        {
        }

        public WinLossByLicenseeController(IWinLossByLicenseeService reportService)
        {
            this._reportService = reportService ?? new WinLossByLicenseeService();
        }

        [HttpGet]
        [NapAuth(FunctionKey = AccReportFunctionKeys.WinLossByLicensee, Policy = Constant.Policy.Read)]
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public JsonResult GetCurrencyReport(GetCurrencyReportCriteria criteria)
        {
            var currencyReport = this._reportService.GetCurrencyReport(criteria);
            return Json(currencyReport, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetLevelReport(GetLevelReportByCustIdCriteria criteria)
        {
            var agentReport = this._reportService.GetLevelReport(criteria);
            return Json(agentReport, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ExportCurrencyReport(GetCurrencyReportCriteria criteria)
        {
            this._reportService.ExportCurrencyReport(criteria);
            return new EmptyResult();
        }

        public ActionResult ExportLevelReport(GetLevelReportByCustIdCriteria criteria)
        {
            this._reportService.ExportLevelReport(criteria);
            return new EmptyResult();
        }
    }
}