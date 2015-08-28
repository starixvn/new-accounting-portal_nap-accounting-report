// --------------------------------------------------------------------------
// <copyright file="MAMonitorToolController.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Louis.Nguyen" date="2012.12.07">Created</change>
// </history>

namespace Fanex.AccountingReports.Misc.Controllers
{
    using System.Web.Mvc;
    using Fanex.AccountingReports.Core.Exceptions;
    using Fanex.AccountingReports.Core.Web.Controllers;
    using Fanex.AccountingReports.Misc.Criteria.MAMonitorTool;
    using Fanex.AccountingReports.Misc.Services.Implement.MAMonitorTool;
    using Fanex.AccountingReports.Misc.Services.Interface;
    using Fanex.NAP.Core.Business;
    using Fanex.NAP.Core.Web.Attribute;

    /// <summary>
    /// MA Monitor Tool Controller.
    /// </summary>
    public class MAMonitorToolController : BaseController
    {
        private readonly IMAMonitorToolService _reportService;

        private readonly IMAMonitorToolService _exportReportService;

        public MAMonitorToolController()
            : this(null, null)
        {
        }

        public MAMonitorToolController(
            IMAMonitorToolService reportService,
            IMAMonitorToolService exportReportService)
        {
            _reportService = reportService ?? new MAMonitorToolService();
            _exportReportService = exportReportService ?? new MAMonitorToolExportExcelService();
        }

        [HttpGet]
        [NapAuth(FunctionKey = AccReportFunctionKeys.MAMonitorTool, Policy = Constant.Policy.Read)]
        public ActionResult Index()
        {
            return View("~/Areas/Misc/Views/MAMonitorTool/Index.cshtml");
        }

        [HttpPost]
        [HandleAjaxError]
        public JsonResult GetReportTypeList()
        {
            var result = _reportService.GetReportTypeList();
            return Json(result);
        }

        [HttpPost]
        [HandleAjaxError]
        public JsonResult GetProductList()
        {
            var result = _reportService.GetProductList();
            return Json(result);
        }

        [HttpPost]
        [HandleAjaxError]
        public JsonResult GetSportList()
        {
            var result = _reportService.GetSportList();
            return Json(result);
        }

        [HttpPost]
        [HandleAjaxError]
        public JsonResult GetBetTypeList(GetBetTypeListCriteria criteria)
        {
            var result = _reportService.GetProductBettype(criteria);
            return Json(result);
        }

        [HttpPost]
        [HandleAjaxError]
        public JsonResult GetCurrencyList(GetCurrencyListCriteria criteria)
        {
            var result = _reportService.GetCurrencyList();
            return Json(result);
        }

        [HttpPost]
        [HandleAjaxError]
        public JsonResult GetLeagueTypeList()
        {
            var result = _reportService.GetLeagueTypeList();
            return Json(result);
        }

        [HttpPost]
        [HandleAjaxError]
        public JsonResult GetLeagueList(GetLeagueListCriteria criteria)
        {
            var result = _reportService.GetLeagueList(criteria);
            return Json(result);
        }

        [HttpPost]
        [HandleAjaxError]
        public JsonResult GetReport(GetReportCriteria criteria)
        {
            var result = _reportService.GetReport(criteria);
            var jsonResult = Json(result, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        public ActionResult ExportReport(GetReportCriteria criteria)
        {
            criteria.IsExportExcel = true;
            _exportReportService.ExportReport(criteria);
            return new EmptyResult();
        }
    }
}
