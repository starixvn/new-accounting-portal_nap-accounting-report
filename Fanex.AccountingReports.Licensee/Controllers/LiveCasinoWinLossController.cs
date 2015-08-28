// --------------------------------------------------------------------------
// <copyright file="LiveCasinoWinLossController.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Ricky.Nguyen" date="2012.11.9">Created</change>
// </history>

namespace Fanex.AccountingReports.Licensee.Controllers
{
    using System.Web.Mvc;
    using Fanex.AccountingReports.Core.Exceptions;
    using Fanex.AccountingReports.Core.Web.Controllers;
    using Fanex.AccountingReports.Licensee.Criteria;
    using Fanex.AccountingReports.Licensee.Criteria.LiveCasinoWinLoss;
    using Fanex.AccountingReports.Licensee.Services.Implement;
    using Fanex.AccountingReports.Licensee.Services.Interface;
    using Fanex.NAP.Core.Business;
    using Fanex.NAP.Core.Web.Attribute;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    [NapSuperAuth]
    public class LiveCasinoWinLossController : BaseController
    {
        private readonly ILiveCasinoWinLossService _reportService;

        public LiveCasinoWinLossController()
            : this(null)
        {
        }

        public LiveCasinoWinLossController(ILiveCasinoWinLossService reportService)
        {
            _reportService = reportService ?? new LiveCasinoWinLossService();
        }

        [HttpGet]
        [NapAuth(FunctionKey = AccReportFunctionKeys.LiveCasinoWinLossByLicensee, Policy = Constant.Policy.Read)]
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public JsonResult SelGameType()
        {
            var result = _reportService.GetGameType();
            return Json(result);
        }

        [HttpPost]
        public JsonResult SelAllLicensee()
        {
            var result = _reportService.GetAllLicensees(new GetLicenseeByResellerCriteria());
            return Json(result);
        }

        [HttpPost]
        [HandleAjaxError]
        public JsonResult SelLiveCasinoWinLossReport(GetLiveCasinoWinLossCriteria selLiveCasinoWinLossCriteria)
        {
            var result = _reportService.GetLiveCasinoWinLoss(selLiveCasinoWinLossCriteria);
            return Json(result);
        }

        [HttpGet]
        public ActionResult ExportLiveCasinoWinLossReport(GetLiveCasinoWinLossCriteria selLiveCasinoWinLossCriteria)
        {
            _reportService.ExportExcelLiveCasinoWinLossReport(selLiveCasinoWinLossCriteria);
            return new EmptyResult();
        }
    }
}