// --------------------------------------------------------------------------
// <copyright file="PunterAnalysisController.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Louis.Nguyen" date="2012.12.07">Created</change>
// </history>

using System;

namespace Fanex.AccountingReports.Special.Controllers
{
    using System.Web.Mvc;
    using Fanex.AccountingReports.Core.Web.Controllers;
    using Fanex.AccountingReports.Special.Criteria.PunterAnalysis;
    using Fanex.AccountingReports.Special.Services.Implement.PunterAnalysis;
    using Fanex.AccountingReports.Special.Services.Interface;
    using Fanex.NAP.Core.Business;
    using Fanex.NAP.Core.Web.Attribute;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class PunterAnalysisController : BaseController
    {
        private readonly IPunterAnalysisService _reportService;

        public PunterAnalysisController()
            : this(null)
        {
        }

        public PunterAnalysisController(IPunterAnalysisService reportService)
        {
            this._reportService = reportService ?? new PunterAnalysisService();
        }

        [HttpGet]
        [NapAuth(FunctionKey = AccReportFunctionKeys.PunterAnalysis, Policy = Constant.Policy.Read)]
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public JsonResult GetProducts(GetProductsCriteria criteria)
        {
            var result = this._reportService.GetProducts(criteria);
            var jsonResult = Json(result);
            jsonResult.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return jsonResult;
        }

        [HttpPost]
        public JsonResult GetSports(GetProductsCriteria criteria)
        {
            var result = this._reportService.GetSports(criteria);
            var jsonResult = Json(result);
            jsonResult.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return jsonResult;
        }

        [HttpPost]
        public JsonResult GetLeaguesBySport(GetLeaguesBySportCriteria criteria)
        {
            var result = this._reportService.GetLeaguesBySport(criteria);
            var jsonResult = Json(result);
            jsonResult.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return jsonResult;
        }

        [HttpPost]
        public JsonResult GetBetTypesBySport(GetBetTypesBySportCriteria criteria)
        {
            var result = this._reportService.GetBetTypesBySport(criteria);
            var jsonResult = Json(result);
            jsonResult.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return jsonResult;
        }

        [HttpPost]
        public JsonResult GetTopPunterReport(GetTopPunterReportCriteria criteria)
        {
            var result = this._reportService.GetTopPunterReport(criteria);
            var jsonResult = Json(result);
            jsonResult.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return jsonResult;
        }

        [HttpPost]
        public JsonResult GetTopPunterByUser(GetTopPunterByUserCriteria criteria)
        {
            var result = this._reportService.GetTopPunterByUser(criteria);
            var jsonResult = Json(result);
            jsonResult.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return jsonResult;
        }

        [HttpPost]
        public JsonResult GetTopPunterBetCount_ByUser(GetTopPunterBetCountByUserCriteria criteria)
        {
            var result = this._reportService.GetTopPunterBetCount_ByUser(criteria);
            var jsonResult = Json(result);
            jsonResult.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return jsonResult;
        }

        [HttpPost]
        public JsonResult GetTopPunterIP_ByUser(GetTopPunterIPByUserCriteria criteria)
        {
            var result = this._reportService.GetTopPunterIP_ByUser(criteria);
            var jsonResult = Json(result);
            jsonResult.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return jsonResult;
        }

        [HttpPost]
        public JsonResult GetTopPunter_ByUser_SportType(GetTopPunter_ByUser_SportTypeCriteria criteria)
        {
            var result = this._reportService.GetTopPunter_ByUser_SportType(criteria);
            var jsonResult = Json(result);
            jsonResult.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return jsonResult;
        }

        [HttpPost]
        public JsonResult GetTopPunterBetCount_ByUser_BetType(GetTopPunterBetCount_ByUser_BetTypeCriteria criteria)
        {
            var result = this._reportService.GetTopPunterBetCount_ByUser_BetType(criteria);
            var jsonResult = Json(result);
            jsonResult.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return jsonResult;
        }

        [HttpPost]
        public JsonResult GetTopPunter_ByUser_BetType(GetTopPunter_ByUser_BetTypeCriteria criteria)
        {
            var result = this._reportService.GetTopPunter_ByUser_BetType(criteria);
            var jsonResult = Json(result);
            jsonResult.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return jsonResult;
        }

        [HttpPost]
        public JsonResult GetTopPunter_ByLeague(GetTopPunter_ByLeagueCriteria criteria)
        {
            var result = this._reportService.GetTopPunter_ByLeague(criteria);
            var jsonResult = Json(result);
            jsonResult.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return jsonResult;
        }

        [HttpPost]
        public JsonResult GetTopPunter_ByLeague_User(GetTopPunter_ByLeague_UserCriteria criteria)
        {
            var result = this._reportService.GetTopPunter_ByLeague_User(criteria);
            var jsonResult = Json(result);
            jsonResult.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return jsonResult;
        }

        public ActionResult ExportTopPunterReport(GetTopPunterReportCriteria criteria)
        {
            this._reportService.ExportTopPunterReport(criteria);
            return new EmptyResult();
        }

        public ActionResult ExportTopPunterByUser(GetTopPunterByUserCriteria criteria)
        {
            this._reportService.ExportTopPunterByUser(criteria);
            return new EmptyResult();
        }

        public ActionResult ExportTopPunterBetCount_ByUser(GetTopPunterBetCountByUserCriteria criteria)
        {
            this._reportService.ExportTopPunterBetCount_ByUser(criteria);
            return new EmptyResult();
        }

        public ActionResult ExportTopPunterIP_ByUser(GetTopPunterIPByUserCriteria criteria)
        {
            this._reportService.ExportTopPunterIP_ByUser(criteria);
            return new EmptyResult();
        }

        public ActionResult ExportTopPunterBetCount_ByUser_BetType(GetTopPunterBetCount_ByUser_BetTypeCriteria criteria)
        {
            this._reportService.ExportTopPunterBetCount_ByUser_BetType(criteria);
            return new EmptyResult();
        }

        public ActionResult ExportTopPunter_ByUser_SportType(GetTopPunter_ByUser_SportTypeCriteria criteria)
        {
            this._reportService.ExportTopPunter_ByUser_SportType(criteria);
            return new EmptyResult();
        }

        public ActionResult ExportTopPunter_ByUser_BetType(GetTopPunter_ByUser_BetTypeCriteria criteria)
        {
            this._reportService.ExportTopPunter_ByUser_BetType(criteria);
            return new EmptyResult();
        }

        public ActionResult ExportTopPunter_ByLeague(GetTopPunter_ByLeagueCriteria criteria)
        {
            this._reportService.ExportTopPunter_ByLeague(criteria);
            return new EmptyResult();
        }

        public ActionResult ExportTopPunter_ByLeague_User(GetTopPunter_ByLeague_UserCriteria criteria)
        {
            this._reportService.ExportTopPunter_ByLeague_User(criteria);
            return new EmptyResult();
        }
    }
}
