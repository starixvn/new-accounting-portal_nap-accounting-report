// -----------------------------------------------------------------------
// <copyright file="PlaytechCasinoWinLossController.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
// <history>
//   <change who="Franklin.Nguyen" date="2014.04.14">Created</change>
// </history>

namespace Fanex.AccountingReports.Licensee.Controllers
{
    using System.Web.Mvc;
    using Fanex.AccountingReports.Core.Exceptions;
    using Fanex.AccountingReports.Core.Web.Controllers;
    using Fanex.AccountingReports.Licensee.Criteria.PlaytechCasinoWinLoss;
    using Fanex.AccountingReports.Licensee.Services.Implement.PlaytechCasinoWinLoss;
    using Fanex.AccountingReports.Licensee.Services.Interface;
    using Fanex.NAP.Core.Business;
    using Fanex.NAP.Core.Web.Attribute;

    /// <summary>
    /// Play tech casino win loss controller.
    /// </summary>
    public class PlaytechCasinoWinLossController : BaseController
    {
        private IPlaytechCasinoWinLossService _playtechCasinoWinLossService;

        public PlaytechCasinoWinLossController()
            : this(null)
        {
        }

        public PlaytechCasinoWinLossController(IPlaytechCasinoWinLossService playtechCasinoWinLossService)
        {
            _playtechCasinoWinLossService = playtechCasinoWinLossService ?? new PlaytechCasinoWinLossService();
        }

        [HttpGet]
        [NapAuth(FunctionKey = AccReportFunctionKeys.PlaytechCasinoWinLoss, Policy = Constant.Policy.Read)]
        public ActionResult Index()
        {
            return View("~/Areas/Licensee/Views/PlaytechCasinoWinLoss/Index.cshtml");
        }

        [HttpPost]
        [HandleAjaxError]
        public ActionResult GetPlaytechCasinoWinLoss(GetPlaytechCasinoWinLossCriteria criteria)
        {
            var result = _playtechCasinoWinLossService.GetPlaytechCasinoWinLoss(criteria);
            var jsonResult = Json(result);
            jsonResult.MaxJsonLength = int.MaxValue;
            jsonResult.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return jsonResult;
        }

        public ActionResult ExportReport(GetPlaytechCasinoWinLossCriteria criteria)
        {
            this._playtechCasinoWinLossService.ExportPlaytechCasinoWinLoss(criteria);
            return new EmptyResult();
        }
    }
}