// --------------------------------------------------------------------------
// <copyright file="CompanyBalanceDifferenceController.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Louis.Nguyen" date="2012.12.07">Created</change>
// </history>

namespace Fanex.AccountingReports.Balance.Controllers
{
    using System.Web.Mvc;
    using Fanex.AccountingReports.Balance.Services.Implement;
    using Fanex.AccountingReports.Balance.Services.Interface;
    using Fanex.AccountingReports.Core.Web.Controllers;
    using Fanex.NAP.Core.Business;
    using Fanex.NAP.Core.Web.Attribute;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class CompanyBalanceDifferenceController : BaseController
    {
        private readonly ICompanyBalanceDifferenceService _reportService;

        public CompanyBalanceDifferenceController()
            : this(null)
        {
        }

        public CompanyBalanceDifferenceController(ICompanyBalanceDifferenceService reportService)
        {
            this._reportService = reportService ?? new CompanyBalanceDifferenceService();
        }

        [HttpGet]
        [NapAuth(FunctionKey = AccReportFunctionKeys.CompanyBalanceDifference, Policy = Constant.Policy.Read)]
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public JsonResult GetCompanyBalanceDifference()
        {
            var report = this._reportService.GetCompanyBalanceDifference();
            var jsonResult = Json(report);
            jsonResult.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }
    }
}
