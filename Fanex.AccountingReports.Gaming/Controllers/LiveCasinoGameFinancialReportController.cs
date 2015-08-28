// --------------------------------------------------------------------------
// <copyright file="LiveCasinoGameFinancialReportController.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Phillip.Truong" date="2012.12.07">Created</change>
// </history>

namespace Fanex.AccountingReports.Gaming.Controllers
{
    using System.Web.Mvc;
    using Fanex.AccountingReports.Core.Web.Controllers;
    using Fanex.AccountingReports.Gaming.Criteria.LiveCasinoGameFinancialReport;
    using Fanex.AccountingReports.Gaming.Services.LiveCasinoGameFinancialReport;
    using Fanex.NAP.Core.Business;
    using Fanex.NAP.Core.Web.Attribute;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class LiveCasinoGameFinancialReportController : BaseController
    {
        private readonly ILiveCasinoGameFinancialReportService _reportService;

        public LiveCasinoGameFinancialReportController()
            : this(null)
        {
        }

        public LiveCasinoGameFinancialReportController(ILiveCasinoGameFinancialReportService reportService)
        {
            this._reportService = reportService ?? new LiveCasinoGameFinancialReportService();
        }

        [NapAuth(FunctionKey = AccReportFunctionKeys.LiveCasinoGameFinancialReport, Policy = Constant.Policy.Read)]
        public ActionResult Index()
        {
            return View("~/Areas/Gaming/Views/LiveCasinoGameFinancialReport/Index.cshtml");
        }

        [HttpPost]
        public JsonResult GetLicensees()
        {
            var reseller = this._reportService.GetLicensees();
            return Json(reseller, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetCurrencies()
        {
            var currencies = this._reportService.GetCurrencies();
            return Json(currencies, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetGameTypes()
        {
            var gameTypes = this._reportService.GetGameTypes();
            return Json(gameTypes, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetGameTables(GetGameTableCriteria criteria)
        {
            var tables = this._reportService.GetGameTables(criteria);
            return Json(tables, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetFinancialReport(GetFinancialReportCriteria criteria)
        {
            var tables = this._reportService.GetFinancialReport(criteria);
            return Json(tables, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ExportFinancialReport(GetFinancialReportCriteria criteria)
        {
            this._reportService.ExportFinancialReport(criteria);
            return new EmptyResult();
        }
    }
}
