// --------------------------------------------------------------------------
// <copyright file="NegativeBalanceController.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="David.Tran" date="2012.11.16">Created</change>
// </history>

namespace Fanex.AccountingReports.Licensee.Controllers
{
    using System.Web.Mvc;
    using Fanex.AccountingReports.Core.Web.Controllers;
    using Fanex.AccountingReports.Licensee.Criteria.NegativeBalance;
    using Fanex.AccountingReports.Licensee.Models;
    using Fanex.AccountingReports.Licensee.Models.NegativeBalance;
    using Fanex.AccountingReports.Licensee.Services.Implement;
    using Fanex.AccountingReports.Licensee.Services.Interface;
    using Fanex.NAP.Core.Business;
    using Fanex.NAP.Core.Web.Attribute;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    [NapSuperAuth]
    public class NegativeBalanceController : BaseController
    {
        private readonly INegativeBanlance _reportService;
        private readonly ILicenseeSiteService _licenseeSite;

        public NegativeBalanceController()
            : this(null, null)
        {
        }

        public NegativeBalanceController(INegativeBanlance reportService, ILicenseeSiteService licenseeSite)
        {
            _reportService = reportService ?? new NegativeBalanceService();
            _licenseeSite = licenseeSite ?? new LicenseeSiteService();
        }

        [HttpGet]
        [NapAuth(FunctionKey = AccReportFunctionKeys.NegativeBalance, Policy = Constant.Policy.Read)]
        public ActionResult Index()
        {
            var licenseeSiteList = _licenseeSite.GetLicenseeSiteList();
            var all = new Licensee { LicenseeName = "All", LicenseeId = -1 };
            licenseeSiteList.Insert(0, all);
            ViewBag.LicensseSiteList = new SelectList(licenseeSiteList, "LicenseeName", "LicenseeName");
            return View();
        }

        [HttpPost]
        public JsonResult GetNegativeBalance(NegativeBalanceCriteria negativeBalanceCriteria)
        {
            var result = _reportService.GetNegativeBalance(negativeBalanceCriteria);
            return Json(result);
        }

        [HttpPost]
        public JsonResult SendMail(MailParam mail, NegativeBalanceCriteria negativeBalanceCriteria)
        {
            var result = _reportService.SendMail(mail, negativeBalanceCriteria);
            return Json(result);
        }

        [HttpGet]
        public ActionResult ExportNegativeBalanceReport(NegativeBalanceCriteria negativeBalanceCriteria)
        {
            _reportService.ExportExcelNegativeBalance(negativeBalanceCriteria);
            return new EmptyResult();
        }
    }
}