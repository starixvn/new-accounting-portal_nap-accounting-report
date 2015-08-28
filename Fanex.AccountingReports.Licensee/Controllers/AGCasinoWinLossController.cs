// --------------------------------------------------------------------------
// <copyright file="AGCasinoWinLossController.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------

namespace Fanex.AccountingReports.Licensee.Controllers
{
    using System.Diagnostics.CodeAnalysis;
    using System.Web.Mvc;
    using Fanex.AccountingReports.Core.Exceptions;
    using Fanex.AccountingReports.Core.Web.Controllers;
    using Fanex.AccountingReports.Licensee.Criteria.AGCasinoWinloss;
    using Fanex.AccountingReports.Licensee.Services.Implement;
    using Fanex.AccountingReports.Licensee.Services.Interface;
    using Fanex.NAP.Core.Business;
    using Fanex.NAP.Core.Web.Attribute;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    [NapSuperAuth]
    public class AGCasinoWinLossController : BaseController
    {
        private readonly IAGCasinoWinLossService _reportService;

        public AGCasinoWinLossController()
            : this(null)
        {
        }

        public AGCasinoWinLossController(IAGCasinoWinLossService reportService)
        {
            this._reportService = reportService ?? new AGCasinoWinLossService();
        }

        [HandleAjaxError]
        public JsonResult Licensee()
        {
            return Json(this._reportService.Licensee());
        }

        [HandleAjaxError]
        public JsonResult CurrencyByLicensee(GetCurrencyByLicensee criteria)
        {
            return Json(this._reportService.CurrencyByLicensee(criteria));
        }

        [HttpGet]
        [NapAuth(FunctionKey = AccReportFunctionKeys.AGCasinoWinloss, Policy = Constant.Policy.Read)]
        public ActionResult Index()
        {
            return View("~/Areas/Licensee/Views/AGCasinoWinloss/Index.cshtml");
        }

        [HandleAjaxError]
        public JsonResult GenerateMainReport(GetMainReportCriteria criteria)
        {
            return Json(this._reportService.GetAGCasinoWinlossMainReport(criteria));
        }

        [HandleAjaxError]
        public ActionResult ExportExcelMainReport(GetMainReportCriteria criteria)
        {
            this._reportService.ExportAGCasinoWinlossMainReport(criteria);
            return new EmptyResult();
        }

        [HandleAjaxError]
        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = "Strongly Type is required.")]
        public JsonResult GenerateSuperReport(GetSuperReportCriteria criteria)
        {
            return Json(this._reportService.GetAGCasinoWinlossSMAMReport(criteria));
        }

        [HandleAjaxError]
        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = "Strongly Type is required.")]
        public ActionResult ExportExcelSuperReport(GetSuperReportCriteria criteria)
        {
            this._reportService.ExportAGCasinoWinlossSMAMReport(criteria);
            return new EmptyResult();
        }

        [HandleAjaxError]
        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = "Strongly Type is required.")]
        public JsonResult GenerateMasterReport(GetMasterReportCriteria criteria)
        {
            return Json(this._reportService.GetAGCasinoWinlossSMAMReport(criteria));
        }

        [HandleAjaxError]
        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = "Strongly Type is required.")]
        public ActionResult ExportExcelMasterReport(GetMasterReportCriteria criteria)
        {
            this._reportService.ExportAGCasinoWinlossSMAMReport(criteria);
            return new EmptyResult();
        }

        [HandleAjaxError]
        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = "Strongly Type is required.")]
        public JsonResult GenerateAgentReport(GetAgentReportCriteria criteria)
        {
            return Json(this._reportService.GetAGCasinoWinlossSMAMReport(criteria));
        }

        [HandleAjaxError]
        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = "Strongly Type is required.")]
        public ActionResult ExportExcelAgentReport(GetAgentReportCriteria criteria)
        {
            this._reportService.ExportAGCasinoWinlossSMAMReport(criteria);
            return new EmptyResult();
        }

        [HandleAjaxError]
        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = "Strongly Type is required.")]
        public JsonResult GenerateMemberReport(GetMemberReportCriteria criteria)
        {
            return Json(this._reportService.GetAGCasinoWinlossSMAMReport(criteria));
        }

        [HandleAjaxError]
        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = "Strongly Type is required.")]
        public ActionResult ExportExcelMemberReport(GetMemberReportCriteria criteria)
        {
            this._reportService.ExportAGCasinoWinlossSMAMReport(criteria);
            return new EmptyResult();
        }
    }
}