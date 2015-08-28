// -----------------------------------------------------------------------
// <copyright file="MatchWinLossByBettypeController.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
// <history>
//   <change who="Franklin.Nguyen" date="2013.12.13">Created</change>
// </history>

namespace Fanex.AccountingReports.WinLoss.Controllers
{
    using System.Web.Mvc;
    using Fanex.AccountingReports.Core.Exceptions;
    using Fanex.AccountingReports.Core.Web.Controllers;
    using Fanex.AccountingReports.WinLoss.Criteria.MatchWinLossByBettype;
    using Fanex.AccountingReports.WinLoss.Services.Implement.MatchWinLossByBettype;
    using Fanex.AccountingReports.WinLoss.Services.Interface.MatchWinLossByBettype;
    using Fanex.NAP.Core.Business;
    using Fanex.NAP.Core.Web.Attribute;

    /// <summary>
    /// Match Win Loss By Bet Type Controller.
    /// </summary>
    public class MatchWinLossByBettypeController : BaseController
    {
        private readonly IMatchWinLossByBettypeService _matchWinLossByBettypeService;
        private readonly IMatchWinLossByBettypeExportDataService _matchWinLossByBettypeExportDataService;

        public MatchWinLossByBettypeController()
            : this(null, null)
        {
        }

        public MatchWinLossByBettypeController(
            IMatchWinLossByBettypeService matchWinLossByBettypeService,
            IMatchWinLossByBettypeExportDataService matchWinLossByBettypeExportDataService)
        {
            this._matchWinLossByBettypeService = matchWinLossByBettypeService ?? new MatchWinLossByBettypeService();
            this._matchWinLossByBettypeExportDataService = new MatchWinLossByBettypeExportDataService();
        }

        [HttpGet]
        [NapAuth(FunctionKey = AccReportFunctionKeys.MatchWinLossByBetType, Policy = Constant.Policy.Read)]
        public ActionResult Index()
        {
            return View("~/Areas/WinLoss/Views/MatchWinLossByBettype/Index.cshtml");
        }

        [HttpPost]
        [HandleAjaxError]
        public JsonResult GetMatchWinLossByBettype(MatchWinLossByBettypeCriteria getMatchWinLossByBettypeCriteria)
        {
            var result = _matchWinLossByBettypeService.GetMatchWinLossByBettype(getMatchWinLossByBettypeCriteria);
            var jsonResult = Json(result, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        public ActionResult ExportReport(MatchWinLossByBettypeCriteria getMatchWinLossByBettypeCriteria)
        {
            _matchWinLossByBettypeExportDataService.ExportMatchWinLossByBettypeReport(getMatchWinLossByBettypeCriteria);
            return new EmptyResult();
        }
    }
}