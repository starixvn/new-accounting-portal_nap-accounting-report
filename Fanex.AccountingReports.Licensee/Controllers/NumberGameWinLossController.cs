// --------------------------------------------------------------------------
// <copyright file="NumberGameWinLossController.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Ricky.Nguyen" date="2012.11.9">Created</change>
// </history>

namespace Fanex.AccountingReports.Licensee.Controllers
{
    using System.Web.Mvc;
    using Fanex.AccountingReports.Core.Web.Controllers;
    using Fanex.AccountingReports.Licensee.Criteria;
    using Fanex.AccountingReports.Licensee.Criteria.NumberGameWinLoss;
    using Fanex.AccountingReports.Licensee.Services.Implement;
    using Fanex.AccountingReports.Licensee.Services.Interface;
    using Fanex.NAP.Core.Business;
    using Fanex.NAP.Core.Web.Attribute;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class NumberGameWinLossController : BaseController
    {
        private readonly INumberGameWinLossService _reportService;

        public NumberGameWinLossController()
            : this(null)
        {
        }

        public NumberGameWinLossController(INumberGameWinLossService reportService)
        {
            _reportService = reportService ?? new NumberGameWinLossService();
        }

        [HttpGet]
        [NapAuth(FunctionKey = AccReportFunctionKeys.NumberGameWinLossByLicensee, Policy = Constant.Policy.Read)]
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public JsonResult SelAllLicensee()
        {
            var result = _reportService.GetAllLicensees(new GetLicenseeByResellerCriteria());
            return Json(result);
        }

        [HttpPost]
        public JsonResult SelNumberGameWinLossReport(GetNumberGameWinLossCriteria selNumberGameWinLossCriteria)
        {
            var result = _reportService.GetNumberGameWinLoss(selNumberGameWinLossCriteria);
            return Json(result);
        }

        [HttpGet]
        public ActionResult ExportGameWinLossReport(GetNumberGameWinLossCriteria selNumberGameWinLossCriteria)
        {
            _reportService.ExportExcelNumberGameWinLossReport(selNumberGameWinLossCriteria);
            return new EmptyResult();
        }
    }
}