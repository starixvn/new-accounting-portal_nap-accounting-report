// --------------------------------------------------------------------------
// <copyright file="BingoOutStandingController.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Louis.Nguyen" date="2012.12.07">Created</change>
// </history>

namespace Fanex.AccountingReports.Gaming.Controllers
{
    using System.Web.Mvc;
    using Fanex.AccountingReports.Core.Exceptions;
    using Fanex.AccountingReports.Core.Web.Controllers;
    using Fanex.AccountingReports.Gaming.Criteria.BingoOutStanding;
    using Fanex.AccountingReports.Gaming.Services.Implement.BingoOutStanding;
    using Fanex.AccountingReports.Gaming.Services.Interface;
    using Fanex.NAP.Core.Business;
    using Fanex.NAP.Core.Web.Attribute;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class BingoOutStandingController : BaseController
    {
        private readonly IBingoOutStandingService _reportService;

        public BingoOutStandingController()
            : this(null)
        {
        }

        public BingoOutStandingController(IBingoOutStandingService reportService)
        {
            this._reportService = reportService ?? new BingoOutStandingService();
        }

        [HttpGet]
        [NapAuth(FunctionKey = AccReportFunctionKeys.BingoOutStanding, Policy = Constant.Policy.Read)]
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [HandleAjaxError]
        public JsonResult GetReport(GetReportCriteria criteria)
        {
            var result = this._reportService.GetReport(criteria);
            var jsonResult = Json(result);
            jsonResult.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return jsonResult;
        }

        public ActionResult ExportReport(GetReportCriteria criteria)
        {
            this._reportService.ExportReport(criteria);
            return new EmptyResult();
        }
    }
}
