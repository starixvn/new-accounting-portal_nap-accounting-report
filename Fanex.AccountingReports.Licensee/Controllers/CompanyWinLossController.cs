// --------------------------------------------------------------------------
// <copyright file="AGCasinoWinLossController.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------

namespace Fanex.AccountingReports.Licensee.Controllers
{
    using System.Web.Mvc;
    using Fanex.AccountingReports.Core.Exceptions;
    using Fanex.AccountingReports.Core.Extensions;
    using Fanex.AccountingReports.Core.Web.Controllers;
    using Fanex.AccountingReports.Licensee.Criteria.CompanyWinLoss;
    using Fanex.AccountingReports.Licensee.Resources;
    using Fanex.AccountingReports.Licensee.Services.Implement;
    using Fanex.AccountingReports.Licensee.Services.Interface;
    using Fanex.Excel;
    using Fanex.Excel.Extensions;
    using Fanex.NAP.Core.Business;
    using Fanex.NAP.Core.Web.Attribute;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class CompanyWinLossController : BaseController
    {
        private readonly ICompanyWinLossService _reportService;

        public CompanyWinLossController()
        {
            this._reportService = new CompanyWinLossService();
        }

        [HttpGet]
        [NapAuth(FunctionKey = AccReportFunctionKeys.CompanyWinLoss, Policy = Constant.Policy.Read)]
        public ActionResult Index()
        {
            return View("~/Areas/Licensee/Views/CompanyWinLoss/Index.cshtml");
        }

        [HttpPost]
        [HandleAjaxError]
        public JsonResult GetCurrenciesByLicensee(GetCurrenciesByLicenseeCriteria criteria)
        {
            var currencies = this._reportService.GetCurrenciesByLicensee(criteria);
            return Json(currencies, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [HandleAjaxError]
        public JsonResult GetProductListByLicense(ProductListByLicenseCriteria criteria)
        {
            return Json(this._reportService.GetProductListByLicense(criteria));
        }

        [HttpPost]
        [HandleAjaxError]
        public JsonNetResult GetCompanyWinLoss(CompanyWinLossCriteria criteria)
        {
            return new JsonNetResult(this._reportService.GetCompanyWinLoss(criteria));
        }

        public EmptyResult ExportReport(CompanyWinLossCriteria criteria)
        {
            string sheetName = Labels.Company_Win_Loss;
            string reportName = this._reportService.GetReportName(criteria);
            FanexExcel result = new FanexExcel(this._reportService.GetFullReportFromCache(criteria));
            NpoiExcelExtensions.ResponseStreamExcel(result.ToExcel2003(sheetName), reportName);

            return new EmptyResult();
        }
    }
}