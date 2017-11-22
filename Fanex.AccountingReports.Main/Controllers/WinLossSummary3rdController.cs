// --------------------------------------------------------------------------
// <copyright file="WinLossSummary3rdController.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Kan.Nguyen" date="2013.06.25">Created</change>
// </history>

namespace Fanex.AccountingReports.Main.Controllers
{
    using Fanex.AccountingReports.Core.Constants;
    using Fanex.AccountingReports.Core.Exceptions;
    using Fanex.AccountingReports.Core.Web.Controllers;
    using Fanex.AccountingReports.Main.Criteria.WinLossSummary3rd;
    using Fanex.AccountingReports.Main.Models.WinLossSummary3rd;
    using Fanex.AccountingReports.Main.Services.Implement.WinLoss3rdSummary;
    using Fanex.AccountingReports.Main.Services.Interface;
    using Fanex.NAP.Core.Business;
    using Fanex.NAP.Core.Web.Attribute;
    using System.Linq;
    using System.Web.Mvc;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class WinLossSummary3rdController : BaseController
    {
        private readonly IWinLossSummary3rdService _reportService;

        public WinLossSummary3rdController()
        {
            this._reportService = new WinLossSummary3rdService();
        }

        [HttpGet]
        [NapAuth(FunctionKey = AccReportFunctionKeys.WinLossSummary, Policy = Constant.Policy.Read)]
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [NapAuth(FunctionKey = AccReportFunctionKeys.WinLossSummary, Policy = Constant.Policy.Read)]
        [HandleAjaxError]
        public JsonResult GetReport(GetReportCriteria criteria)
        {
            WinLoss3rdInfo result = this._reportService.GetReport(criteria);

            return Json(result, JsonRequestBehavior.DenyGet);
        }

        [HttpPost]
        [NapAuth(FunctionKey = AccReportFunctionKeys.WinLossSummary, Policy = Constant.Policy.Read)]
        [HandleAjaxError]
        public JsonResult ViewGameMatchDetails(GetMatchGameDetailsCriteria criteria)
        {
            var details = this._reportService.GetDetails(criteria);

            var jsonResult = Json(details, JsonRequestBehavior.DenyGet);
            jsonResult.MaxJsonLength = CommonConstants.MaxJsonLength;

            return jsonResult;
        }

        [HttpPost]
        [NapAuth(FunctionKey = AccReportFunctionKeys.WinLossSummary, Policy = Constant.Policy.Host)]
        [HandleAjaxError]
        public JsonResult RecalculateWinLoss(GetRecalculateWinLossCriteria criteria)
        {
            this._reportService.RecalculateReport(criteria);

            return Json(new { Code = 0 }, JsonRequestBehavior.DenyGet);
        }

        [HttpGet]
        [NapAuth(FunctionKey = AccReportFunctionKeys.WinLossSummary, Policy = Constant.Policy.Read)]
        [HandleAjaxError]
        public ActionResult ExportWinLossSummary3rd(GetReportCriteria criteria)
        {
            this._reportService.ExportReport(criteria);

            return new EmptyResult();
        }

        [HttpGet]
        [NapAuth(FunctionKey = AccReportFunctionKeys.WinLossSummary, Policy = Constant.Policy.Read)]
        [HandleAjaxError]
        public ActionResult ExportWinLossSummary3rdDetails(GetMatchGameDetailsCriteria criteria)
        {
            this._reportService.ExportDetails(criteria);

            return new EmptyResult();
        }
    }
}