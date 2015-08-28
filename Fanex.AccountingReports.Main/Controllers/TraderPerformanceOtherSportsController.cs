// -----------------------------------------------------------------------
// <copyright file="TraderPerformanceOtherSportsController.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Louis.Nguyen" date="2012.12.07">Created</change>
//   <change who="Franklin.Nguyen" date="2014.01.06">Created</change>
// </history>

namespace Fanex.AccountingReports.Main.Controllers
{
    using System.Web.Mvc;
    using Fanex.AccountingReports.Core.Exceptions;
    using Fanex.AccountingReports.Core.Web.Controllers;
    using Fanex.AccountingReports.Main.Criteria.TraderPerformance;
    using Fanex.AccountingReports.Main.Services.Implement.TraderPerformanceOtherSports;
    using Fanex.AccountingReports.Main.Services.Interface;
    using Fanex.NAP.Core.Business;
    using Fanex.NAP.Core.Web.Attribute;

    /// <summary>
    /// Trader performance for other sports  controller.
    /// </summary>
    public class TraderPerformanceOtherSportsController : BaseController
    {
        private readonly ITraderPerformanceOtherSportsService _reportService;

        public TraderPerformanceOtherSportsController()
            : this(null)
        {
        }

        public TraderPerformanceOtherSportsController(ITraderPerformanceOtherSportsService reportService)
        {
            this._reportService = reportService ?? new TraderPerformanceOtherSportsService();
        }

        [HttpGet]
        [NapAuth(FunctionKey = AccReportFunctionKeys.TraderPerformanceOtherSports, Policy = Constant.Policy.Read)]
        public ActionResult Index()
        {
            return View("~/Areas/Main/Views/TraderPerformanceOtherSports/Index.cshtml");
        }

        [HttpPost]
        [HandleAjaxError]
        public JsonResult GetTraderPerformanceForOtherSports(GetReportCriteria criteria)
        {
            var result = _reportService.GetReport(criteria);
            var jsonResult = Json(result);
            jsonResult.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        [HttpPost]
        [HandleAjaxError]
        public JsonResult GetTraderPerformanceForOtherSportsDetail(GetDetailReportCriteria criteria)
        {
            var result = _reportService.GetDetailReport(criteria);
            var jsonResult = Json(result);
            jsonResult.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        [HttpPost]
        [HandleAjaxError]
        public JsonResult SortTraderPerformanceForOtherSports(GetReportCriteria criteria)
        {
            var result = _reportService.SortReportData(criteria);
            var jsonResult = Json(result);
            jsonResult.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        [HttpPost]
        [HandleAjaxError]
        public JsonResult SortTraderPerformanceForOtherSportsDetail(GetDetailReportCriteria criteria)
        {
            var result = _reportService.SortDetailReportData(criteria);
            var jsonResult = Json(result);
            jsonResult.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        public ActionResult ExportReport(GetReportCriteria criteria)
        {
            this._reportService.ExportReport(criteria);
            return new EmptyResult();
        }

        public ActionResult ExportDetailReport(GetDetailReportCriteria criteria)
        {
            this._reportService.ExportDetailReport(criteria);
            return new EmptyResult();
        }
    }
}