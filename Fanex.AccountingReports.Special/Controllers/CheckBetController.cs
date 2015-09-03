// --------------------------------------------------------------------------
// <copyright file="CheckBetController.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Faisal.Tran" date="2012.12.20">Created</change>
// </history>

namespace Fanex.AccountingReports.Special.Controllers
{
    using System.Web.Mvc;
    using Fanex.AccountingReports.Core.Web.Controllers;
    using Fanex.AccountingReports.Core.Web.Security;
    using Fanex.AccountingReports.Special.Criteria.CheckBet;
    using Fanex.AccountingReports.Special.Services.Implement;
    using Fanex.AccountingReports.Special.Services.Interface;
    using Fanex.NAP.Core.Web.Attribute;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class CheckBetController : BaseController
    {
        private readonly ICheckBetService _checkBetService;

        public CheckBetController()
            : this(null)
        {
        }

        public CheckBetController(ICheckBetService checkBetService)
        {
            this._checkBetService = checkBetService ?? new CheckBetService();
        }

        [AuthenticateAction(AuthenticateAction.AuthenticationType.Read, "Index")]
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public JsonResult SelCheckedBet(CheckBetCriteria checkBetCriteria)
        {
            var result = this._checkBetService.GetCheckedBet(checkBetCriteria);
            return Json(result);
        }

        [HttpPost]
        public JsonResult SelNormalTickets(CheckBetCriteria checkBetCriteria)
        {
            var result = this._checkBetService.GetNormalChangedTicket(checkBetCriteria);
            return Json(result);
        }

        [HttpPost]
        public JsonResult SelRejectTickets(CheckBetCriteria checkBetCriteria)
        {
            var result = this._checkBetService.GetRejectChangedTicket(checkBetCriteria);
            return Json(result);
        }
    }
}
