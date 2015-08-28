// -----------------------------------------------------------------------
// <copyright file="KenoWinLossController.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
// <history>
//   <change who="Phillip.Truong" date="2014.04.14">Created</change>
// </history>

namespace Fanex.AccountingReports.Licensee.Controllers
{
    using System.Web.Mvc;
    using Fanex.AccountingReports.Core.Exceptions;
    using Fanex.AccountingReports.Core.Web.Controllers;
    using Fanex.AccountingReports.Licensee.Criteria.KenoWinLoss;
    using Fanex.AccountingReports.Licensee.Services.Implement.PlaytechCasinoWinLoss;
    using Fanex.AccountingReports.Licensee.Services.Interface;
    using Fanex.Excel.Extensions;
    using Fanex.NAP.Core.Business;
    using Fanex.NAP.Core.Web.Attribute;

    /// <summary>
    /// Play tech casino win loss controller.
    /// </summary>
    [NapSuperAuth]
    public class KenoWinLossController : BaseController
    {
        private IKenoWinLossService _kenoWinLossService;

        public KenoWinLossController()
            : this(null)
        {
        }

        public KenoWinLossController(IKenoWinLossService kenoWinLossService)
        {
            _kenoWinLossService = kenoWinLossService ?? new KenoWinLossService();
        }

        [HttpGet]
        [NapAuth(FunctionKey = AccReportFunctionKeys.KenoWinLoss, Policy = Constant.Policy.Read)]
        public ActionResult Index()
        {
            ViewBag.ListCountryMarket = this._kenoWinLossService.GetListCountryMarket();
            return View("~/Areas/Licensee/Views/KenoWinLoss/Index.cshtml");
        }

        [HttpPost]
        public JsonResult GetLicensee()
        {
            var result = this._kenoWinLossService.GetListLicensees();
            return Json(result);
        }

        [HttpPost]
        public JsonResult GetCurrencyByLicensee(GetKenoCurrencyByLicenseeCriteria criteria)
        {
            var result = this._kenoWinLossService.GetListCurrency(criteria);
            return Json(result);
        }

        [HandleAjaxError]
        public JsonNetResult GetKenoWinLossReport2(GetKenoWinLossCriteria criteria)
        {
            var result = this._kenoWinLossService.GetKenoWinLossReport2(criteria);
            return new JsonNetResult(result);
        }

        [HandleAjaxError]
        public JsonResult GetKenoWinLossReport(GetKenoWinLossCriteria criteria)
        {
            var result = this._kenoWinLossService.GetKenoWinLossReport(criteria);

            return Json(result);
        }

        [HandleAjaxError]
        public ActionResult ExportKenoWinLossReport(GetKenoWinLossCriteria criteria)
        {
            this._kenoWinLossService.ExportKenoWinLossReport(criteria);
            return new EmptyResult();
        }
    }
}