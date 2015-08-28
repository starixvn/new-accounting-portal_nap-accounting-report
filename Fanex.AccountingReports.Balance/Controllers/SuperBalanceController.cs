// -----------------------------------------------------------------------
// <copyright file="SuperBalanceController.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="" date="2013.23.10">Created</change>
// </history>

namespace Fanex.AccountingReports.Balance.Controllers
{
    using System.Collections.Generic;
    using System.Web.Mvc;
    using Fanex.AccountingReports.Balance.Criteria.SuperBalance;
    using Fanex.AccountingReports.Balance.Services.Implement;
    using Fanex.AccountingReports.Balance.Services.Interface;
    using Fanex.AccountingReports.Core.Exceptions;
    using Fanex.AccountingReports.Core.Models;
    using Fanex.AccountingReports.Core.Web.Controllers;
    using Fanex.AccountingReports.Core.Web.Security;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class SuperBalanceController : BaseController
    {
        private readonly ISuperBalanceService _superBalanceService;

        public SuperBalanceController()
            : this(null)
        {
        }

        public SuperBalanceController(ISuperBalanceService superBalaceService)
        {
            this._superBalanceService = superBalaceService ?? new SuperBalanceService();
        }

        [HttpGet]
        [AuthenticateAction(AuthenticateAction.AuthenticationType.Read, "Index")]
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [HandleAjaxError]
        public JsonResult GetSuperBalance(GetSuperBalanceCriteria getSuperBalanceCriteria)
        {
            var result = this._superBalanceService.GetSuperBalance(getSuperBalanceCriteria);
            var jsonResult = Json(result);
            jsonResult.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        [HttpPost]
        [HandleAjaxError]
        public JsonResult GetSuperBalanceSuperDownline(GetSuperBalanceSuperDownlineCriteria getSuperBalanceSuperDownlineCriteria)
        {
            var result = this._superBalanceService.GetSuperBalanceSuperDownLine(getSuperBalanceSuperDownlineCriteria);
            var jsonResult = Json(result);
            jsonResult.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        [HttpPost]
        [HandleAjaxError]
        public JsonResult GetSuperBalanceMasterDownline(GetSuperBalanceMasterDownlineCriteria getSuperBalanceMasterDownlineCriteria)
        {
            var result = this._superBalanceService.GetSuperBalanceMasterDownLine(getSuperBalanceMasterDownlineCriteria);
            var jsonResult = Json(result);
            jsonResult.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        [HttpPost]
        [HandleAjaxError]
        public JsonResult GetSuperBalanceAgentDownline(GetSuperBalanceAgentDownlineCriteria getSuperBalanceAgentDownlineCriteria)
        {
            var result = this._superBalanceService.GetSuperBalanceAgentDownLine(getSuperBalanceAgentDownlineCriteria);
            var jsonResult = Json(result);
            jsonResult.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        public ActionResult ExportReport(GetSuperBalanceCriteria getSuperBalanceCriteria)
        {
            var result = this._superBalanceService.ExportSuperBalanceReport(getSuperBalanceCriteria);
            return new EmptyResult();
        }

        public ActionResult ExportMasterBalance(GetSuperBalanceSuperDownlineCriteria getSuperBalanceSuperDownline)
        {
            var result = this._superBalanceService.ExportBalanceSuperDownLineReport(getSuperBalanceSuperDownline);
            return new EmptyResult();
        }

        public ActionResult ExportAgentBalance(GetSuperBalanceMasterDownlineCriteria getSuperBalanceMasterDownline)
        {
            var result = this._superBalanceService.ExportBalanceMasterDownLineReport(getSuperBalanceMasterDownline);
            return new EmptyResult();
        }

        public ActionResult ExportMemberBalance(GetSuperBalanceAgentDownlineCriteria getSuperBalanceAgentDownline)
        {
            var result = this._superBalanceService.ExportBalanceAgentDownLineReport(getSuperBalanceAgentDownline);
            return new EmptyResult();
        }

        [HttpPost]
        public JsonResult GetCurrencies()
        {
            var result = this._superBalanceService.GetCurrencies();
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetListAlphabet()
        {
            var result = this._superBalanceService.GetListAlphabet();
            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}
