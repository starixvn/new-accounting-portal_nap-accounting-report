// --------------------------------------------------------------------------
// <copyright file="LiveCasinoGameStatisticController.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Louis.Nguyen" date="2012.12.07">Created</change>
// </history>

namespace Fanex.AccountingReports.Gaming.Controllers
{
    using System.Diagnostics.CodeAnalysis;
    using System.Web.Mvc;
    using Fanex.AccountingReports.Core.Criteria;
    using Fanex.AccountingReports.Core.Exceptions;
    using Fanex.AccountingReports.Core.Web.Controllers;
    using Fanex.AccountingReports.Gaming.Criteria.CasinoGameStatistic;
    using Fanex.AccountingReports.Gaming.Services.Implement.CasinoGameStatistic;
    using Fanex.AccountingReports.Gaming.Services.Interface.CasinoGameStatistic;
    using Fanex.NAP.Core.Business;
    using Fanex.NAP.Core.Web.Attribute;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class CasinoGameStatisticController : BaseController
    {
        private readonly ICasinoGameStatisticService _reportService;
        private readonly IFilterSectionService _filterService;
        private readonly IExportExcelService _excelService;
        private readonly IExportExcelHourlyService _excelHourlyService;

        public CasinoGameStatisticController()
            : this(null, null, null, null)
        {
        }

        [SuppressMessage("StyleCop.CSharp.Nexcel.NexcelCustomRules",
           "SP2100:CodeLineMustNotBeLongerThan",
           Justification = "Reviewed. Suppression is OK here.")]
        public CasinoGameStatisticController(ICasinoGameStatisticService reportService, IFilterSectionService filterService, IExportExcelService excelService, IExportExcelHourlyService excelHourlyService)
        {
            _reportService = reportService ?? new CasinoGameStatisticService();
            _filterService = filterService ?? new FilterSectionService();
            _excelService = excelService ?? new ExportExcelService();
            _excelHourlyService = excelHourlyService ?? new ExportExcelHourlyService();
        }

        [HttpGet]
        [NapAuth(FunctionKey = AccReportFunctionKeys.CasinoGameStatistics, Policy = Constant.Policy.Read)]
        public ActionResult Index()
        {
            return View("~/Areas/Gaming/Views/CasinoGameStatistic/Index.cshtml");
        }

        [HttpPost]
        [HandleAjaxError]
        public JsonResult GetCurrencyList(GetExchangeCurrenciesCriteria criteria)
        {
            var result = _filterService.GetCurrencyList(criteria);
            return Json(result);
        }

        [HttpPost]
        [HandleAjaxError]
        public JsonResult GetProductList()
        {
            var result = _filterService.GetProductList();
            return Json(result);
        }

        [HttpPost]
        [HandleAjaxError]
        public JsonResult Get3rdPartyByProduct(string product)
        {
            var result = _filterService.Get3rdPartyByProduct(product);
            return Json(result);
        }

        [HttpPost]
        [HandleAjaxError]
        public JsonResult GetProductGameType(GetGameTypeListCriteria criteria)
        {
            var result = _filterService.GetProductGameTypes(criteria);
            return Json(result);
        }

        [HttpPost]
        [HandleAjaxError]
        public JsonResult GetGameTableList(GetGameTableListCriteria criteria)
        {
            var result = _filterService.GetGameTableList(criteria);
            return Json(result);
        }

        [HttpPost]
        [HandleAjaxError]
        public JsonResult GetReportByDaily(GetReportByDailyCriteria criteria)
        {
            var result = _reportService.GetReport(criteria);
            var jsonResult = Json(result);
            jsonResult.MaxJsonLength = int.MaxValue;
            jsonResult.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return jsonResult;
        }

        [HttpPost]
        [HandleAjaxError]
        public JsonResult GetReportDailyByCurrency(GetReportByDailyCriteria criteria)
        {
            var result = _reportService.GetReport(criteria);
            var jsonResult = Json(result);
            jsonResult.MaxJsonLength = int.MaxValue;
            jsonResult.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return jsonResult;
        }

        [HttpPost]
        [HandleAjaxError]
        public JsonResult GetReportByMonthly(GetReportByDailyCriteria criteria)
        {
            var result = _reportService.GetReport(criteria);
            var jsonResult = Json(result);
            jsonResult.MaxJsonLength = int.MaxValue;
            jsonResult.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return jsonResult;
        }

        [HttpPost]
        [HandleAjaxError]
        public JsonResult GetReportMonthlyByCurrency(GetReportByDailyCriteria criteria)
        {
            var result = _reportService.GetReport(criteria);
            var jsonResult = Json(result);
            jsonResult.MaxJsonLength = int.MaxValue;
            jsonResult.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return jsonResult;
        }

        [HttpPost]
        [HandleAjaxError]
        public JsonResult GetReportByUserName(GetReportByDailyCriteria criteria)
        {
            var result = _reportService.GetReport(criteria);
            var jsonResult = Json(result);
            jsonResult.MaxJsonLength = int.MaxValue;
            jsonResult.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return jsonResult;
        }

        [HttpPost]
        [HandleAjaxError]
        public JsonResult GetReportByGameType(GetReportByDailyCriteria criteria)
        {
            var result = _reportService.GetReport(criteria);
            var jsonResult = Json(result);
            jsonResult.MaxJsonLength = int.MaxValue;
            jsonResult.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return jsonResult;
        }

        [HttpPost]
        [HandleAjaxError]
        public JsonResult GetReportGameTypeCurrency(GetReportByDailyCriteria criteria)
        {
            var result = _reportService.GetReport(criteria);
            var jsonResult = Json(result);
            jsonResult.MaxJsonLength = int.MaxValue;
            jsonResult.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return jsonResult;
        }

        [HttpPost]
        [HandleAjaxError]
        public JsonResult GetReportByGameTable(GetReportByDailyCriteria criteria)
        {
            var result = _reportService.GetReport(criteria);
            var jsonResult = Json(result);
            jsonResult.MaxJsonLength = int.MaxValue;
            jsonResult.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return jsonResult;
        }

        [HttpPost]
        [HandleAjaxError]
        public JsonResult GetReportGameTableByCurrency(GetReportByDailyCriteria criteria)
        {
            var result = _reportService.GetReport(criteria);
            var jsonResult = Json(result);
            jsonResult.MaxJsonLength = int.MaxValue;
            jsonResult.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return jsonResult;
        }

        [HttpPost]
        [HandleAjaxError]
        public JsonResult GetReportByCompany(GetReportByDailyCriteria criteria)
        {
            var result = _reportService.GetReport(criteria);
            var jsonResult = Json(result);
            jsonResult.MaxJsonLength = int.MaxValue;
            jsonResult.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return jsonResult;
        }

        [HttpPost]
        [HandleAjaxError]
        public JsonResult GetReportCompanyByCurrency(GetReportByDailyCriteria criteria)
        {
            var result = _reportService.GetReport(criteria);
            var jsonResult = Json(result);
            jsonResult.MaxJsonLength = int.MaxValue;
            jsonResult.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return jsonResult;
        }

        [HttpPost]
        [HandleAjaxError]
        public JsonResult GetReportByCurrency(GetReportByDailyCriteria criteria)
        {
            var result = _reportService.GetReport(criteria);
            var jsonResult = Json(result);
            jsonResult.MaxJsonLength = int.MaxValue;
            jsonResult.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return jsonResult;
        }

        [HttpPost]
        [HandleAjaxError]
        public JsonResult SortReport(GetReportByDailyCriteria getReportCriteria)
        {
            var result = _reportService.SortReport(getReportCriteria);
            var jsonResult = Json(result);
            jsonResult.MaxJsonLength = int.MaxValue;
            jsonResult.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return jsonResult;
        }

        public ActionResult ExportReport(GetReportByDailyCriteria criteria)
        {
            _reportService.ExportReport(criteria);
            return new EmptyResult();
        }

        [HttpPost]
        [HandleAjaxError]
        public JsonResult GetReportByGrandTotal(GetReportByDailyCriteria criteria)
        {
            var result = _reportService.GetReport(criteria);
            var jsonResult = Json(result);
            jsonResult.MaxJsonLength = int.MaxValue;
            jsonResult.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return jsonResult;
        }

        public ActionResult ExportGrandTotal(GetReportByDailyCriteria criteria)
        {
            var total = _reportService.GetReportByGrandTotal(criteria);

            _excelService.ExportReport(total, criteria);
            return new EmptyResult();
        }

        [HttpPost]
        [HandleAjaxError]
        public JsonResult GetReportByHourly(GetReportByHourlyCriteria criteria)
        {
            var result = _reportService.GetReportByHourly(criteria);
            var jsonResult = Json(result);
            jsonResult.MaxJsonLength = int.MaxValue;
            jsonResult.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return jsonResult;
        }

        [HttpPost]
        [HandleAjaxError]
        public JsonResult GetLicenseeList(GetLicenseeListCriteria criteria)
        {
            var result = _reportService.GetLicenseeList(criteria);
            var jsonResult = Json(result);
            jsonResult.MaxJsonLength = int.MaxValue;
            jsonResult.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return jsonResult;
        }

        public ActionResult ExportHourly(GetReportByHourlyCriteria criteria)
        {
            _excelHourlyService.ExportReport(criteria);
            return new EmptyResult();
        }
    }
}