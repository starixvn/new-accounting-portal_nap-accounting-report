// -----------------------------------------------------------------------
// <copyright file="TotalCreditAccountByProductController.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Franklin.Nguyen" date="2014.06.10">Created</change>
// </history>

namespace Fanex.AccountingReports.Misc.Controllers
{
    using System.Web.Mvc;
    using Fanex.AccountingReports.Core.Exceptions;
    using Fanex.AccountingReports.Core.Web.Controllers;
    using Fanex.AccountingReports.Misc.Criteria.TotalCreditAccountByProduct;
    using Fanex.AccountingReports.Misc.Services.Implement.TotalCreditAccountByProduct;
    using Fanex.AccountingReports.Misc.Services.Interface;
    using Fanex.NAP.Core.Business;
    using Fanex.NAP.Core.Web.Attribute;

    /// <summary>
    /// Total credit account by product controller.
    /// </summary>
    public class TotalCreditAccountByProductController : BaseController
    {
        private ITotalCreditAccountByProductService _totalCreditAccountByProductService;

        public TotalCreditAccountByProductController()
        {
            _totalCreditAccountByProductService = new TotalCreditAccountByProductService();
        }

        [HttpGet]
        [NapAuth(FunctionKey = AccReportFunctionKeys.TotalCreditBetCountByProduct, Policy = Constant.Policy.Read)]
        public ActionResult Index()
        {
            return View("~/Areas/Misc/Views/TotalCreditAccountByProduct/Index.cshtml");
        }

        [HttpPost]
        [HandleAjaxError]
        public ActionResult GetCurrency()
        {
            var currencies = _totalCreditAccountByProductService.GetCurrencies();
            return Json(currencies, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [HandleAjaxError]
        public ActionResult GetProducts()
        {
            var products = _totalCreditAccountByProductService.GetProducts();
            return Json(products, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [HandleAjaxError]
        public ActionResult GetTotalCreditAccountByProduct(GetTotalCreditAccountByProductCriteria criteria)
        {
            var result = _totalCreditAccountByProductService.GetTotalCreditAccountByProduct(criteria);
            var jsonResult = Json(result);
            jsonResult.MaxJsonLength = int.MaxValue;
            jsonResult.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return jsonResult;
        }

        [HttpPost]
        [HandleAjaxError]
        public ActionResult GetBetCountDetail(GetBetCountDetailCriteria criteria)
        {
            var result = _totalCreditAccountByProductService.GetBetCountDetail(criteria);
            var jsonResult = Json(result);
            jsonResult.MaxJsonLength = int.MaxValue;
            jsonResult.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return jsonResult;
        }

        public ActionResult ExportTotalCreditAccountByProduct(GetTotalCreditAccountByProductCriteria criteria)
        {
            _totalCreditAccountByProductService.ExportTotalCreditAccountByProductData(criteria);
            return new EmptyResult();
        }

        public ActionResult ExportBetCountDetail(GetBetCountDetailCriteria criteria)
        {
            _totalCreditAccountByProductService.ExportBetCountDetailData(criteria);
            return new EmptyResult();
        }
    }
}