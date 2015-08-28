// -----------------------------------------------------------------------
// <copyright file="NumberOfActiveMembersForRacingController.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
// <history>
//   <change who="Franklin.Nguyen" date="2013.12.12">Created</change>
// </history>

namespace Fanex.AccountingReports.Racing.Controllers
{
    using System.Web.Mvc;
    using Fanex.AccountingReports.Core.Exceptions;
    using Fanex.AccountingReports.Core.Services.Impl;
    using Fanex.AccountingReports.Core.Web.Controllers;
    using Fanex.AccountingReports.Racing.Criteria.NumberOActiveMembersForRacing;
    using Fanex.AccountingReports.Racing.Services.Implement.NumberOActiveMembersForRacing;
    using Fanex.AccountingReports.Racing.Services.Interface.NumberOActiveMembersForRacing;
    using Fanex.NAP.Core.Business;
    using Fanex.NAP.Core.Web.Attribute;

    /// <summary>
    /// Racing - Number Of Active Member.
    /// </summary>
    public class NumberOfActiveMembersForRacingController : BaseController
    {
        private readonly INumberOfActiveMembersService _numberOfActiveMemberService;
        private readonly INumberOfActiveMembersExportDataService _numberOfActiveMemberExportDataService;

        public NumberOfActiveMembersForRacingController()
            : this(null, null)
        {
        }

        public NumberOfActiveMembersForRacingController(
            INumberOfActiveMembersService numberOfActiveMemberService,
            INumberOfActiveMembersExportDataService numberOfActiveMemberExportDataService)
        {
            this._numberOfActiveMemberService = numberOfActiveMemberService ?? new NumberOfActiveMembersService();
            this._numberOfActiveMemberExportDataService = numberOfActiveMemberExportDataService ?? new NumberOfActiveMembersExportDataService();
        }

        [HttpGet]
        [NapAuth(FunctionKey = AccReportFunctionKeys.NumberofActiveMembersForRacing, Policy = Constant.Policy.Read)]
        public ActionResult Index()
        {
            return View("~/Areas/Racing/Views/NumberOfActiveMembersForRacing/Index.cshtml");
        }

        [HttpPost]
        [HandleAjaxError]
        public JsonResult GetNumberOfActiveMembers(NumberOfActiveMembersCriteria getNumberOfActiveMembersCriteria)
        {
            getNumberOfActiveMembersCriteria.BalanceUpDay = ReportService.BalanceUp.BalanceUpDay;
            var result = _numberOfActiveMemberService.GetNumberOfActiveMembers(getNumberOfActiveMembersCriteria);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ExportReport(NumberOfActiveMembersCriteria getNumberOfActiveMembersCriteria)
        {
            getNumberOfActiveMembersCriteria.BalanceUpDay = ReportService.BalanceUp.BalanceUpDay;
            _numberOfActiveMemberExportDataService.ExportNumberOfActiveMembers(getNumberOfActiveMembersCriteria);
            return new EmptyResult();
        }
    }
}