// --------------------------------------------------------------------------
// <copyright file="BetListController.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Ricky.Nguyen" date="2012.12.12">Created</change>
// </history>

namespace Fanex.BetList.Accounting.Controllers
{
    using Fanex.BetList.Accounting.Core;
    using Fanex.BetList.Accounting.Criteria;
    using Fanex.BetList.Accounting.Models;
    using Fanex.BetList.Accounting.Services;
    using Fanex.Common;
    using System.Collections;
    using System.Collections.Generic;
    using System.Web.Mvc;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class BetListController : Controller
    {
        private readonly IBetListService _betListService;
        private readonly ICheckSmartPunterService _smartPunterService = new CheckSmartPunterService();
        private readonly IResultService _resultService;
        private readonly BetListCashoutDetailService _betListCashoutDetailService;

        public BetListController()
        {
            this._betListService = new BetListService();
            this._resultService = new ResultService();
            this._betListCashoutDetailService = new BetListCashoutDetailServiceImp();
        }


        /// <summary>
        /// Gets the mix parlay detail.
        /// </summary>
        /// <param name="refno">The ref no.</param>
        /// <returns>The mix parlay detail.</returns>
        public ActionResult GetMixParlayDetail(string refno, string transId, string handicap1, string handicap2, string betcheck, string betteam)
        {
            var result = _betListService.GetMixParlayTickets(
                new MixParlayDetailCriteria { RefNo = refno, TransId = transId },
                handicap1,
                handicap2,
                betcheck,
                betteam);

            return PartialView("~/Areas/BetList/Views/MemberBetListReport/_MixParlayDetail.cshtml", result);
        }

        /// <summary>
        /// Gets the bet list report by reference no.
        /// </summary>
        /// <param name="refno">The ref no.</param>
        /// <returns>The bet list report by reference no.</returns>
        public ActionResult GetBetListReportByRefNo(string refno)
        {
            IList<TicketDetail> result = new List<TicketDetail>();
            if (BetListHelper.IsNumeric(refno.Trim()))
            {
                result = _betListService.GetBetListByRefNoTickets(new BetListByRefNoCriteria
                {
                    TransId = Converter.To<long>(refno)
                });
            }

            return PartialView("~/Areas/BetList/Views/BetlistReport/_BetListByRefNo.cshtml", result);
        }

        /// <summary>
        /// Gets the bet types.
        /// </summary>
        /// <param name="betTypeByDateCriteria">The bet type by date criteria.</param>
        /// <returns>Bet types after change date range.</returns>
        [HttpPost]
        public JsonResult GetBetTypes(BetTypeByDateCriteria betTypeByDateCriteria)
        {
            var result = _betListService.GetBetTypeByDate(betTypeByDateCriteria);
            return Json(result);
        }

        /// <summary>
        /// Gets the combination mix parlay.
        /// </summary>
        /// <param name="refNo">The reference no.</param>
        /// <param name="transId">The trans identifier.</param>
        /// <param name="betId">The bet identifier.</param>
        /// <param name="custId">The customer identifier.</param>
        /// <returns>The combination mix parlay.</returns>
        [HttpGet]
        public ActionResult GetCombinationMP(long refNo, long transId, long betId, int custId)
        {
            var result = this._betListService.GetCombinationMP(new CombinationParlayDetailCriteria
            {
                RefNo = refNo,
                TransId = transId,
                BetId = betId,
                CustId = custId,
                BalanceUpDay = ReportService.BalanceUp.BalanceUpDay,
                MoveBetDay = ReportService.BalanceUp.MoveBetsDay
            });

            Hashtable paramHastable = new Hashtable();
            paramHastable.Add("custId", custId);
            TempData["paramHastable"] = paramHastable;

            return PartialView("~/Areas/Betlist/Views/_CombinationMPDetail.cshtml", result);
        }

        [HttpPost]
        public ActionResult GetBetlistRacingToteWinLoss(BetListRacingWinLossToteCriteria criteria)
        {
            var result = this._betListService.GetBetListRacingToteWinLoss(criteria);
            return Json(result);
        }

        public ActionResult GetCashOutHistory(GetCashOutDetailCriteria criteria)
        {
            var cashOutHistory = _betListCashoutDetailService.GetCashOutHistory(criteria);

            return View("~/Areas/Betlist/Views/CashOut/History.cshtml", cashOutHistory);
        }
    }
}