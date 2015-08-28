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
    using Fanex.AccountingReports.Gaming.Criteria.LiveCasinoGameStatistic;
    using Fanex.AccountingReports.Gaming.Services.Implement.LiveCasinoGameStatistic;
    using Fanex.AccountingReports.Gaming.Services.Interface;
    using Fanex.NAP.Core.Business;
    using Fanex.NAP.Core.Web.Attribute;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class LiveCasinoGameStatisticController : BaseController
    {
        private readonly ILiveCasinoGameStatisticService _reportService;

        public LiveCasinoGameStatisticController()
            : this(null)
        {
        }

        public LiveCasinoGameStatisticController(ILiveCasinoGameStatisticService reportService)
        {
            _reportService = reportService ?? new LiveCasinoGameStatisticService();
        }

        [HttpGet]
        [NapAuth(FunctionKey = AccReportFunctionKeys.LiveCasinoGameStatistics, Policy = Constant.Policy.Read)]
        public ActionResult Index()
        {
            return View("~/Areas/Gaming/Views/LiveCasinoGameStatistic/Index.cshtml");
        }

        [HttpPost]
        [HandleAjaxError]
        public JsonResult GetCurrencyList(GetExchangeCurrenciesCriteria criteria)
        {
            var result = _reportService.GetCurrencyList(criteria);
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
        public JsonResult GetProductGameType(GetGameTypeListCriteria criteria)
        {
            var result = _reportService.GetProductGameTypes(criteria);
            return Json(result);
        }

        [HttpPost]
        [HandleAjaxError]
        public JsonResult GetGameTableList(GetGameTableListCriteria criteria)
        {
            var result = _reportService.GetGameTableList(criteria);
            return Json(result);
        }

        [HttpPost]
        [HandleAjaxError]
        public JsonResult GetReport(GetReportCriteria criteria)
        {
            var result = this._reportService.GetReport(criteria);
            var jsonResult = Json(result);
            jsonResult.MaxJsonLength = int.MaxValue;
            jsonResult.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return jsonResult;
        }

        [HttpPost]
        [HandleAjaxError]
        public JsonResult SortReport(GetReportCriteria getReportCriteria)
        {
            var result = this._reportService.SortReport(getReportCriteria);
            var jsonResult = Json(result);
            jsonResult.MaxJsonLength = int.MaxValue;
            jsonResult.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return jsonResult;
        }

        public ActionResult ExportReport(GetReportCriteria criteria)
        {
            this._reportService.ExportReport(criteria);
            return new EmptyResult();
        }
    }
}