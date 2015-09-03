// --------------------------------------------------------------------------
// <copyright file="PeakConcurrentUsersController.cs" company="Nexcel Solutions Vietnam">
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
    using Fanex.AccountingReports.Licensee.Criteria.PeakConcurrentUser;
    using Fanex.AccountingReports.Licensee.Models.PeakConcurrentUser;
    using Fanex.AccountingReports.Licensee.Services.Implement;
    using Fanex.AccountingReports.Licensee.Services.Interface;
    using Fanex.NAP.Core.Business;
    using Fanex.NAP.Core.Web.Attribute;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class PeakConcurrentUsersController : BaseController
    {
        private readonly IPeakConcurrentUserService _reportService;
        private readonly ILicenseeSiteService _licenseeSite;

        public PeakConcurrentUsersController()
            : this(null, null)
        {
        }

        public PeakConcurrentUsersController(IPeakConcurrentUserService reportService, ILicenseeSiteService licenseeSite)
        {
            _reportService = reportService ?? new PeakConcurrentUserService();
            _licenseeSite = licenseeSite ?? new LicenseeSiteService();
        }

        [HttpGet]
        [NapAuth(FunctionKey = AccReportFunctionKeys.PeakConcurrentUsers, Policy = Constant.Policy.Read)]
        public ActionResult Index()
        {
            ViewBag.LicensseSiteList = new SelectList(_licenseeSite.GetLicenseeSiteList(), "LicenseeName", "LicenseeName");
            return View();
        }

        [HttpPost]
        public JsonResult Partial(PeakConcurrentUserCriteria peakConcurrentUserCriteria)
        {
            var result = new PeakConcurrentUser();
            result = _reportService.GetConcurrentUserList(peakConcurrentUserCriteria);
            return Json(result);
        }

        [HttpGet]
        public ActionResult ExportGamePeakReport(PeakConcurrentUserCriteria peakConcurrentUserCriteria)
        {
            _reportService.PeakExport2Excel(peakConcurrentUserCriteria);
            return new EmptyResult();
        }
    }
}