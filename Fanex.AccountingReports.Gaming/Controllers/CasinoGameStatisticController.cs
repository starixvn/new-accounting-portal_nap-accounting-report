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

        public CasinoGameStatisticController()
            : this(null, null, null)
        {
        }

        public CasinoGameStatisticController(ICasinoGameStatisticService reportService, IFilterSectionService filterService, IExportExcelService excelService)
        {
            _reportService = reportService ?? new CasinoGameStatisticService();
            _filterService = filterService ?? new FilterSectionService();
            _excelService = excelService ?? new ExportExcelService();
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
        public JsonResult GetReport(GetReportByDailyCriteria criteria)
        {
            var result = _reportService.GetReport(criteria);
            var jsonResult = Json(result);
            jsonResult.MaxJsonLength = int.MaxValue;
            jsonResult.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return jsonResult;
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
        public JsonResult GetReportByGrandTotal(GetReportByGrandRMCriteria criteria)
        {
            var result = _reportService.GetReportByGrandTotal(criteria);
            var jsonResult = Json(result);
            jsonResult.MaxJsonLength = int.MaxValue;
            jsonResult.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return jsonResult;
        }

        public ActionResult ExportGrandTotal(GetReportByDailyCriteria criteria)
        {
            GetReportByGrandRMCriteria grandRMCriteria = new GetReportByGrandRMCriteria()
            {
                FromDate = criteria.FromDate,
                ToDate = criteria.ToDate,
                UserName = criteria.UserName,
                ExcludeTestData = criteria.ExcludeTestData,
                LicenseeId = criteria.LicenseeId,
                GameTypeId = criteria.GameTypeId,
                GameTableId = criteria.GameTableId,
                Currency = criteria.Currency,
                ShowRefundColumn = criteria.ShowRefundColumn,
                ProductId = criteria.ProductId,
                Product = criteria.Product,
                IsExportExcel = criteria.IsExportExcel
            };

            var total = _reportService.GetReportByGrandTotal(grandRMCriteria);

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
    }
}