// --------------------------------------------------------------------------
// <copyright file="DailyWinLoss2hController.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Louis.Nguyen" date="2012.12.07">Created</change>
// </history>

namespace Fanex.AccountingReports.Main.Controllers
{
    using System.Web.Mvc;
    using Fanex.AccountingReports.Core.Exceptions;
    using Fanex.AccountingReports.Core.Web;
    using Fanex.AccountingReports.Core.Web.Controllers;
    using Fanex.AccountingReports.Main.Criteria.DailyWinLoss2h;
    using Fanex.AccountingReports.Main.Services.Implement.DailyWinLoss2h;
    using Fanex.AccountingReports.Main.Services.Interface;
    using Fanex.AccountingReports.Main.Services.Interface.DailyWinLoss2h;
    using Fanex.NAP.Core.Business;
    using Fanex.NAP.Core.Web.Attribute;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class DailyWinLoss2hController : BaseController
    {
        private readonly IDailyWinLoss2hService _reportService;
        private readonly IMetricGamingService _metricGamingService;
        private readonly IBettradeService _bettradeService;
        private readonly IColossusBetService _colossusBetService;
        private readonly ICockFightingService _cockFightingService;

        public DailyWinLoss2hController()
            : this(null, null, null, null, null)
        {
        }

        public DailyWinLoss2hController(IDailyWinLoss2hService reportService, IMetricGamingService metricGamingService, IBettradeService bettradeService, IColossusBetService colossusBetService, ICockFightingService cockFightingService)
        {
            this._reportService = reportService ?? new DailyWinLoss2hService();
            this._metricGamingService = metricGamingService ?? new MetricGamingService();
            this._bettradeService = bettradeService ?? new BettradeService();
            this._colossusBetService = colossusBetService ?? new ColossusBetService();
            this._cockFightingService = cockFightingService ?? new CockFightingService();
        }

        [HttpGet]
        [NapAuth(FunctionKey = AccReportFunctionKeys.DailyWinLoss2h, Policy = Constant.Policy.Read)]
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [HandleAjaxError]
        public JsonWithoutNullField GetMain2hReport(GetMain2hReportCriteria criteria)
        {
            var result = this._reportService.GetMain2hReport(criteria);
            var jsonResult = new JsonWithoutNullField(result);
            return jsonResult;
        }

        [HttpPost]
        [HandleAjaxError]
        public JsonWithoutNullField GetCasinoReport(GetCasinoReportCriteria criteria)
        {
            var result = this._reportService.GetCasinoReport(criteria);
            var jsonResult = new JsonWithoutNullField(result);
            return jsonResult;
        }

        [HttpPost]
        [HandleAjaxError]
        public JsonWithoutNullField GetLiveCasinoReport(GetLiveCasinoReportCriteria criteria)
        {
            var result = this._reportService.GetLiveCasinoReport(criteria);
            var jsonResult = new JsonWithoutNullField(result);
            return jsonResult;
        }

        [HttpPost]
        [HandleAjaxError]
        public JsonWithoutNullField GetKenoReport(GetKenoReportCriteria criteria)
        {
            var result = this._reportService.GetKenoReport(criteria);
            var jsonResult = new JsonWithoutNullField(result);
            return jsonResult;
        }

        [HttpPost]
        [HandleAjaxError]
        public JsonWithoutNullField GetMixParlayReport(GetMixParlayReportCriteria criteria)
        {
            var result = this._reportService.GetMixParlayReport(criteria);
            var jsonResult = new JsonWithoutNullField(result);
            return jsonResult;
        }

        [HttpPost]
        [HandleAjaxError]
        public JsonWithoutNullField GetSummaryRacingReport(GetSummaryRacingReportCriteria criteria)
        {
            var result = this._reportService.GetSummaryRacingReport(criteria);
            var jsonResult = new JsonWithoutNullField(result);
            return jsonResult;
        }

        [HttpPost]
        [HandleAjaxError]
        public JsonWithoutNullField GetRacingReportBySport(GetRacingReportBySportCriteria criteria)
        {
            var result = this._reportService.GetRacingReportBySport(criteria);
            var jsonResult = new JsonWithoutNullField(result);
            return jsonResult;
        }

        [HttpPost]
        [HandleAjaxError]
        public JsonWithoutNullField GetRacingReportByBetType(GetRacingReportByBetTypeCriteria criteria)
        {
            var result = this._reportService.GetRacingReportByBetType(criteria);
            var jsonResult = new JsonWithoutNullField(result);
            return jsonResult;
        }

        [HttpPost]
        [HandleAjaxError]
        public JsonWithoutNullField GetSummaryNumberGameReport(GetSummaryNumberGameReportCriteria criteria)
        {
            var result = this._reportService.GetSummaryNumberGameReport(criteria);
            var jsonResult = new JsonWithoutNullField(result);
            return jsonResult;
        }

        [HttpPost]
        [HandleAjaxError]
        public JsonWithoutNullField GetSummaryOtherSportReport(GetSummaryOtherSportReportCriteria criteria)
        {
            var result = this._reportService.GetSummaryOtherSportReport(criteria);
            var jsonResult = new JsonWithoutNullField(result);
            return jsonResult;
        }

        [HttpPost]
        [HandleAjaxError]
        public JsonWithoutNullField GetOtherSportReportBySport(GetOtherSportReportBySportCriteria criteria)
        {
            var result = this._reportService.GetOtherSportReportBySport(criteria);
            var jsonResult = new JsonWithoutNullField(result);
            return jsonResult;
        }

        [HttpPost]
        [HandleAjaxError]
        public JsonWithoutNullField GetSoccerReport(GetSoccerReportCriteria criteria)
        {
            var result = this._reportService.GetSoccerReport(criteria);
            var jsonResult = new JsonWithoutNullField(result);
            return jsonResult;
        }

        [HttpPost]
        [HandleAjaxError]
        public JsonWithoutNullField GetBingoReport(GetBingoReportCriteria criteria)
        {
            var result = this._reportService.GetBingoReport(criteria);
            var jsonResult = new JsonWithoutNullField(result);
            return jsonResult;
        }

        [HttpPost]
        [HandleAjaxError]
        public JsonWithoutNullField GetVirtualSportReport(GetVirtualSportReportCriteria criteria)
        {
            var result = this._reportService.GetVirtualSportReport(criteria);
            var jsonResult = new JsonWithoutNullField(result);
            return jsonResult;
        }

        [HttpPost]
        [HandleAjaxError]
        public JsonWithoutNullField GetVirtualSportReportBySport(GetVirtualSportReportBySportCriteria criteria)
        {
            var result = this._reportService.GetVirtualSportReportBySport(criteria);
            var jsonResult = new JsonWithoutNullField(result);
            return jsonResult;
        }

        [HttpPost]
        [HandleAjaxError]
        public JsonWithoutNullField GetVirtualSports_VirtualHorseRacingReport(GetVirtualSports_VirtualHorseRacingReportCriteria criteria)
        {
            var result = this._reportService.GetVirtualSports_VirtualHorseRacingReport(criteria);
            var jsonResult = new JsonWithoutNullField(result);
            return jsonResult;
        }

        [HttpPost]
        [HandleAjaxError]
        public JsonWithoutNullField GetReportByBetType(GetReportByBetTypeCriteria criteria)
        {
            var result = this._reportService.GetReportByBetType(criteria);
            var jsonResult = new JsonWithoutNullField(result);
            return jsonResult;
        }

        [HttpPost]
        [HandleAjaxError]
        public JsonWithoutNullField GetWinLossBySite_BetType(GetWinLossBySite_BetTypeCriteria criteria)
        {
            var result = this._reportService.GetWinLossBySite_BetType(criteria);
            var jsonResult = new JsonWithoutNullField(result);
            return jsonResult;
        }

        [HttpPost]
        [HandleAjaxError]
        public JsonResult GetParlayReport(GetParlayReportCriteria criteria)
        {
            var result = this._reportService.GetParlayReport(criteria);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [HandleAjaxError]
        public JsonResult GetMetricGamingBettypeReport(GetMetricGamingBettypeCriteria criteria)
        {
            var report = _metricGamingService.GetMetricGamingBetTypes(criteria);
            return Json(report);
        }

        [HttpPost]
        [HandleAjaxError]
        public JsonResult GetMainLiveCasinoReport(GetLiveCasinoReportCriteria criteria)
        {
            var report = _reportService.GetMainLiveCasinoReport(criteria);
            return Json(report);
        }

        [HttpPost]
        [HandleAjaxError]
        public JsonResult GetLiveCasinoWinLossByProductReport(GetLiveCasinoWinLossByProductCriteria criteria)
        {
            var report = _reportService.GetLiveCasinoWinLossByProductReport(criteria);
            return Json(report);
        }

        [HttpPost]
        [HandleAjaxError]
        public JsonResult GetBettradeReport(GetBettradeCriteria criteria)
        {
            var report = _bettradeService.GetBettradeReport(criteria);
            return Json(report);
        }

        [HttpPost]
        [HandleAjaxError]
        public JsonResult GetColossusBetReport(GetColossusBetCriteria criteria)
        {
            var report = _colossusBetService.GetColossusBetReport(criteria);
            return Json(report);
        }

        [HttpPost]
        [HandleAjaxError]
        public JsonResult GetCockFightingReport(GetCockFightingCriteria criteria)
        {
            var report = _cockFightingService.GetCockFightingReport(criteria);
            return Json(report);
        }

        [HttpPost]
        [HandleAjaxError]
        public JsonResult GetCockFightingArenaReport(GetCockFightingArenaCriteria criteria)
        {
            var report = _cockFightingService.GetCockFightingArenaReport(criteria);
            return Json(report);
        }

        //// ExportExcel

        public ActionResult ExportMain2hReport(GetMain2hReportCriteria criteria)
        {
            this._reportService.ExportMain2hReport(criteria);
            return new EmptyResult();
        }

        public ActionResult ExportSummaryRacingReport(GetSummaryRacingReportCriteria criteria)
        {
            this._reportService.ExportSummaryRacingReport(criteria);
            return new EmptyResult();
        }

        public ActionResult ExportRacingReportBySport(GetRacingReportBySportCriteria criteria)
        {
            this._reportService.ExportRacingReportBySport(criteria);
            return new EmptyResult();
        }

        public ActionResult ExportRacingReportByBetType(GetRacingReportByBetTypeCriteria criteria)
        {
            this._reportService.ExportRacingReportByBetType(criteria);
            return new EmptyResult();
        }

        public ActionResult ExportCasinoReport(GetCasinoReportCriteria criteria)
        {
            this._reportService.ExportCasinoReport(criteria);
            return new EmptyResult();
        }

        public ActionResult ExportLiveCasinoReport(GetLiveCasinoReportCriteria criteria)
        {
            this._reportService.ExportLiveCasinoReport(criteria);
            return new EmptyResult();
        }

        public ActionResult ExportKenoReport(GetKenoReportCriteria criteria)
        {
            this._reportService.ExportKenoReport(criteria);
            return new EmptyResult();
        }

        public ActionResult ExportBingoReport(GetBingoReportCriteria criteria)
        {
            this._reportService.ExportBingoReport(criteria);
            return new EmptyResult();
        }

        public ActionResult ExportNumberGameReport(GetSummaryNumberGameReportCriteria criteria)
        {
            this._reportService.ExportNumberGameReport(criteria);
            return new EmptyResult();
        }

        public ActionResult ExportNumberGameReportByBetType(GetReportByBetTypeCriteria criteria)
        {
            this._reportService.ExportNumberGameReportByBetType(criteria);
            return new EmptyResult();
        }

        public ActionResult ExportSummaryOtherSportReport(GetSummaryOtherSportReportCriteria criteria)
        {
            this._reportService.ExportSummaryOtherSportReport(criteria);
            return new EmptyResult();
        }

        public ActionResult ExportOtherSportReportBySport(GetOtherSportReportBySportCriteria criteria)
        {
            this._reportService.ExportOtherSportReportBySport(criteria);
            return new EmptyResult();
        }

        public ActionResult ExportOtherSportReportByBetType(GetReportByBetTypeCriteria criteria)
        {
            this._reportService.ExportOtherSportReportByBetType(criteria);
            return new EmptyResult();
        }

        public ActionResult ExportSoccerReport(GetSoccerReportCriteria criteria)
        {
            this._reportService.ExportSoccerReport(criteria);
            return new EmptyResult();
        }

        public ActionResult ExportSoccerReportByBetType(GetReportByBetTypeCriteria criteria)
        {
            this._reportService.ExportSoccerReportByBetType(criteria);
            return new EmptyResult();
        }

        public ActionResult ExportSystemParlayReport(GetMixParlayReportCriteria criteria)
        {
            this._reportService.ExportSystemParlayReport(criteria);
            return new EmptyResult();
        }

        public ActionResult ExportVirtualSportReport(GetVirtualSportReportCriteria criteria)
        {
            this._reportService.ExportVirtualSportReport(criteria);
            return new EmptyResult();
        }

        public ActionResult ExportVirtualSportReportBySport(GetVirtualSportReportBySportCriteria criteria)
        {
            this._reportService.ExportVirtualSportReportBySport(criteria);
            return new EmptyResult();
        }

        public ActionResult ExportVirtualSportReportByBetType(GetReportByBetTypeCriteria criteria)
        {
            this._reportService.ExportVirtualSportReportByBetType(criteria);
            return new EmptyResult();
        }

        public ActionResult ExportVirtualSports_VirtualHorseRacingReport(GetVirtualSports_VirtualHorseRacingReportCriteria criteria)
        {
            this._reportService.ExportVirtualSports_VirtualHorseRacingReport(criteria);
            return new EmptyResult();
        }

        public ActionResult ExportMixParlayReport(GetMixParlayReportCriteria criteria)
        {
            this._reportService.ExportMixParlayReport(criteria);
            return new EmptyResult();
        }

        public ActionResult ExportWinLossBySite_BetType(GetWinLossBySite_BetTypeCriteria criteria)
        {
            this._reportService.ExportWinLossBySite_BetType(criteria);
            return new EmptyResult();
        }

        public ActionResult ExportParlayReport(GetParlayReportCriteria criteria)
        {
            this._reportService.ExportParlayReport(criteria);
            return new EmptyResult();
        }

        public ActionResult ExportMetricGamingBettypeReport(GetMetricGamingBettypeCriteria criteria)
        {
            this._metricGamingService.ExportMetricGamingBettypeReport(criteria);
            return new EmptyResult();
        }

        public ActionResult ExportMainLiveCasinoReport(GetLiveCasinoReportCriteria criteria)
        {
            this._reportService.ExportMainLiveCasinoReport(criteria);
            return new EmptyResult();
        }

        public ActionResult ExportLiveCasinoWinLossByProductReport(GetLiveCasinoWinLossByProductCriteria criteria)
        {
            this._reportService.ExportGetLiveCasinoWinLossByProductReport(criteria);
            return new EmptyResult();
        }

        public ActionResult ExportBettradeReport(GetBettradeCriteria criteria)
        {
            this._bettradeService.ExportBettradeReport(criteria);
            return new EmptyResult();
        }

        public ActionResult ExportColossusBetReport(GetColossusBetCriteria criteria)
        {
            this._colossusBetService.ExportColossusBetReport(criteria);
            return new EmptyResult();
        }

        public ActionResult ExportCockFightingReport(GetCockFightingCriteria criteria)
        {
            this._cockFightingService.ExportCockFightingReport(criteria);
            return new EmptyResult();
        }

        public ActionResult ExportCockFightingArenaReport(GetCockFightingArenaCriteria criteria)
        {
            this._cockFightingService.ExportCockFightingArenaReport(criteria);
            return new EmptyResult();
        }

        //// ExportExcel
    }
}