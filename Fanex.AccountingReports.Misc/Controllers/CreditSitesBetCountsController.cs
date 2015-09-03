// --------------------------------------------------------------------------
// <copyright file="CreditSitesBetCountsController.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Louis.Nguyen" date="2012.12.07">Created</change>
// </history>

namespace Fanex.AccountingReports.Misc.Controllers
{
    using System.Web.Mvc;
    using Fanex.AccountingReports.Core.Web.Controllers;
    using Fanex.AccountingReports.Misc.Criteria.CreditSitesBetCounts;
    using Fanex.AccountingReports.Misc.Services.Implement;
    using Fanex.AccountingReports.Misc.Services.Interface;
    using Fanex.NAP.Core.Business;
    using Fanex.NAP.Core.Web.Attribute;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class CreditSitesBetCountsController : BaseController
    {
        private readonly ICreditSitesBetCountsService _reportService;

        public CreditSitesBetCountsController()
            : this(null)
        {
        }

        public CreditSitesBetCountsController(ICreditSitesBetCountsService reportService)
        {
            this._reportService = reportService ?? new CreditSitesBetCountsService();
        }

        [HttpGet]
        [NapAuth(FunctionKey = AccReportFunctionKeys.NumberofbetsforCreditsites, Policy = Constant.Policy.Read)]
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public JsonResult GetReport(GetCreditSitesBetCountsCriteria criteria)
        {
            var report = this._reportService.GetReport(criteria);
            return Json(report, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ExportReport(GetCreditSitesBetCountsCriteria criteria)
        {
            this._reportService.ExportReport(criteria);
            return new EmptyResult();
        }
    }
}
