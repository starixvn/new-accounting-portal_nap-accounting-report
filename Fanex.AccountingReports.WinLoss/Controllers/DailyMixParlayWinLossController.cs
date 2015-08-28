// --------------------------------------------------------------------------
// <copyright file="DailyMixParlayWinLossController.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Louis.Nguyen" date="2012.12.07">Created</change>
// </history>

namespace Fanex.AccountingReports.WinLoss.Controllers
{
    using System.Web.Mvc;
    using Fanex.AccountingReports.Core.Exceptions;
    using Fanex.AccountingReports.Core.Web.Controllers;
    using Fanex.AccountingReports.WinLoss.Criteria.DailyMixParlayWinLoss;
    using Fanex.AccountingReports.WinLoss.Services.Implement.DailyMixParlayWinLoss;
    using Fanex.AccountingReports.WinLoss.Services.Interface.DailyMixParlayWinLoss;
    using Fanex.NAP.Core.Business;
    using Fanex.NAP.Core.Web.Attribute;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class DailyMixParlayWinLossController : BaseController
    {
        private readonly IDailyMixParlayWinLossService _reportService;

        public DailyMixParlayWinLossController()
            : this(null)
        {
        }

        public DailyMixParlayWinLossController(IDailyMixParlayWinLossService reportService)
        {
            this._reportService = reportService ?? new DailyMixParlayWinLossService();
        }

        [HttpGet]
        [NapAuth(FunctionKey = AccReportFunctionKeys.DailyMixParlayWinLoss, Policy = Constant.Policy.Read)]
        public ActionResult Index()
        {
            return View();
        }

        [NapAuth(FunctionKey = AccReportFunctionKeys.DailyMixParlayWinLoss, Policy = Constant.Policy.Read)]
        public ActionResult MixParlayPopup()
        {
            return View("~/Areas/WinLoss/Views/DailyMixParlayWinLoss/MixParlayReportPopup.cshtml");
        }

        [HttpPost]
        [HandleAjaxError]
        public JsonResult GetCombinationWinLossReport(GetCombinationWinLossReportCriteria criteria)
        {
            var report = this._reportService.GetCombinationWinLossReport(criteria);
            return Json(report, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [HandleAjaxError]
        public JsonResult GetAdvanceMixParlayReport(GetAdvanceMixParlayReportCriteria criteria)
        {
            var report = this._reportService.GetAdvanceMixParlayReport(criteria);
            return Json(report, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [HandleAjaxError]
        public JsonResult GetMixParlayReport(GetMixParlayReportCriteria criteria)
        {
            var report = this._reportService.GetMixParlayReport(criteria);
            return Json(report, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ExportCombinationWinLossReport(GetCombinationWinLossReportCriteria criteria)
        {
            this._reportService.ExportCombinationWinLossReport(criteria);
            return new EmptyResult();
        }

        public ActionResult ExportAdvanceMixParlayReport(GetAdvanceMixParlayReportCriteria criteria)
        {
            this._reportService.ExportAdvanceMixParlayReport(criteria);
            return new EmptyResult();
        }

        public ActionResult ExportMixParlayReport(GetMixParlayReportCriteria criteria)
        {
            this._reportService.ExportMixParlayReport(criteria);
            return new EmptyResult();
        }
    }
}
