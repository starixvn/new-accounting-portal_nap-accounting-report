// --------------------------------------------------------------------------
// <copyright file="MatchWinLoss2aController.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Louis.Nguyen" date="2012.12.07">Created</change>
// </history>

namespace Fanex.AccountingReports.Main.Controllers
{
    using System.Web.Mvc;
    using Fanex.AccountingReports.Core.Exceptions;
    using Fanex.AccountingReports.Core.Extensions;
    using Fanex.AccountingReports.Core.Web.Controllers;
    using Fanex.AccountingReports.Main.Criteria.MatchWinLoss2a;
    using Fanex.AccountingReports.Main.Services.Implement.MatchWinLoss2a;
    using Fanex.AccountingReports.Main.Services.Interface;
    using Fanex.AccountingReports.Main.Services.Interface.MatchWinLoss2a;
    using Fanex.Data.Repository;
    using Fanex.Excel.Extensions;
    using Fanex.NAP.Core.Business;
    using Fanex.NAP.Core.Web.Attribute;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class MatchWinLoss2aController : BaseController
    {
        private readonly IMatchWinLoss2aService _reportService;
        private readonly IMetricGamingService _metricGamingService;

        public MatchWinLoss2aController()
            : this(null, null)
        {
        }

        public MatchWinLoss2aController(IMatchWinLoss2aService reportService, IMetricGamingService metricGamingService)
        {
            this._reportService = reportService ?? new MatchWinLoss2aService();
            this._metricGamingService = metricGamingService ?? new MetricGamingService();
        }

        [HttpGet]
        [NapAuth(FunctionKey = AccReportFunctionKeys.MatchWinLoss2a, Policy = Constant.Policy.Read)]
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [HandleAjaxError]
        public JsonResult GetMainReport(GetMainReportCriteria criteria)
        {
            var result = this._reportService.GetMainReport(criteria);
            var jsonResult = Json(result);
            jsonResult.MaxJsonLength = int.MaxValue;
            jsonResult.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return jsonResult;
        }

        [HttpPost]
        [HandleAjaxError]
        public JsonResult GetSummaryReportByBetType(GetWinLossByBetTypeCriteria criteria)
        {
            var result = this._reportService.GetMainReportByBetType(criteria);
            var jsonResult = Json(result);
            jsonResult.MaxJsonLength = int.MaxValue;
            jsonResult.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return jsonResult;
        }

        [HttpPost]
        [HandleAjaxError]
        public JsonResult GetSummaryReportBySportType(GetMainReportCriteria criteria)
        {
            var result = this._reportService.GetMainReportBySportType(criteria);
            var jsonResult = Json(result);
            jsonResult.MaxJsonLength = int.MaxValue;
            jsonResult.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return jsonResult;
        }

        [HttpPost]
        [HandleAjaxError]
        public JsonResult GetDetailReportByBetType(GetDetailReportByBetTypeCriteria criteria)
        {
            var result = this._reportService.GetDetailReportByBetType(criteria);
            var jsonResult = Json(result);
            jsonResult.MaxJsonLength = int.MaxValue;
            jsonResult.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return jsonResult;
        }

        [HttpPost]
        [HandleAjaxError]
        public JsonResult GetDetailReportBeforeBalanceUpDayByBetType(GetDetailReportByBetTypeCriteria criteria)
        {
            var result = this._reportService.GetDetailReportBeforeBalanceUpDayByBetType(criteria);
            var jsonResult = Json(result);
            jsonResult.MaxJsonLength = int.MaxValue;
            jsonResult.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return jsonResult;
        }

        [HttpPost]
        [HandleAjaxError]
        public JsonResult GetDetailReportBySportsType(GetDetailReportByBetTypeCriteria criteria)
        {
            var result = this._reportService.GetBetTypeDetailReportBySports(criteria);
            var jsonResult = Json(result);
            jsonResult.MaxJsonLength = int.MaxValue;
            jsonResult.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return jsonResult;
        }

        [HttpPost]
        [HandleAjaxError]
        public JsonResult GetDetailReportBySportsTypeBeforeBalanceUpDay(GetDetailReportByBetTypeCriteria criteria)
        {
            var result = this._reportService.GetBetTypeDetailReportBySportsBeforeBalanceUpDay(criteria);
            var jsonResult = Json(result);
            jsonResult.MaxJsonLength = int.MaxValue;
            jsonResult.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return jsonResult;
        }

        [HttpPost]
        [HandleAjaxError]
        public JsonResult GetThirdPartyCasinoReport(GetThirdPartyCasinoReportCriteria criteria)
        {
            var result = this._reportService.GetThirdPartyCasinoReport(criteria);
            var jsonResult = Json(result);
            jsonResult.MaxJsonLength = int.MaxValue;
            jsonResult.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return jsonResult;
        }

        [HttpPost]
        [HandleAjaxError]
        public JsonNetResult GetMetricGamingByBettypeReport(GetMetricGamingCriteria criteria)
        {
            var report = this._metricGamingService.GetMetricGamingByBettypeReport(criteria);
            var jsonResult = new JsonNetResult(report);
            return jsonResult;
        }

        [HttpPost]
        [HandleAjaxError]
        public JsonResult GetMetricGamingDetailByBetType(GetMetricGamingDetailByBetTypeCriteria criteria)
        {
            var result = this._reportService.GetMetricGamingDetailByBetType(criteria);
            var jsonResult = Json(result);
            jsonResult.MaxJsonLength = int.MaxValue;
            jsonResult.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return jsonResult;
        }

        public ActionResult ExportSummaryReportByBetType(GetMainReportCriteria criteria)
        {
            this._reportService.ExportSummaryReportByBetType(criteria);
            return new EmptyResult();
        }

        public ActionResult ExportSummaryReportBySportType(GetMainReportCriteria criteria)
        {
            this._reportService.ExportSummaryReportBySportType(criteria);
            return new EmptyResult();
        }

        public ActionResult ExportDetailReportByBetType(GetDetailReportByBetTypeCriteria criteria)
        {
            this._reportService.ExportDetailReportByBetType(criteria);
            return new EmptyResult();
        }

        public ActionResult ExportDetailReportBySportsType(GetDetailReportByBetTypeCriteria criteria)
        {
            this._reportService.ExportDetailReportBySportsType(criteria);
            return new EmptyResult();
        }

        public ActionResult ExportAllDetailReport(GetMainReportCriteria criteria)
        {
            this._reportService.ExportAllDetailReport(criteria);
            return new EmptyResult();
        }

        public ActionResult ExportMetricGamingByBettypeReport(GetMetricGamingCriteria criteria)
        {
            var workbook = _metricGamingService.GetMetricGamingByBettypeReportWorkboook(criteria);
            string reportName = _metricGamingService.GetMetricGamingByBetypeReportName(criteria);
            NpoiExcelExtensions.ResponseStreamExcel(workbook, reportName);
            return new EmptyResult();
        }

        public ActionResult ExportMetricGamingDetailByBetTypeReport(GetMetricGamingDetailByBetTypeCriteria criteria)
        {
            this._reportService.ExportMetricGamingDetailReportByBetType(criteria);
            return new EmptyResult();
        }
    }
}