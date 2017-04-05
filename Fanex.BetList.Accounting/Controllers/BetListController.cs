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
        /// Gets the licensee win loss report member bet list.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>The licensee win loss report member bet list.</returns>
        [HttpPost]
        public ActionResult GetWinLossMemberBetList(WinLossMemberBetListCriteria criteria)
        {
            Hashtable paramHastable = new Hashtable();
            paramHastable.Add("custId", criteria.CustID);

            TempData["paramHastable"] = paramHastable;

            var result = _betListService.GetWinLossMemberBetList(criteria);
            return PartialView("~/Areas/BetList/Views/MemberBetListReport/_WinLossMemberBetList.cshtml", result);
        }

        [HttpGet]
        public ActionResult WinLossMemberBetList()
        {
            return View("~/Areas/BetList/Views/MemberBetListReport/Index.cshtml");
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
        /// Gets the bet list by reference no.
        /// </summary>
        /// <param name="refno">The ref no.</param>
        /// <returns>The bet list by reference no.</returns>
        public ActionResult GetBetListByRefNo(string refno)
        {
            IList<TicketDetail> result = new List<TicketDetail>();
            if (BetListHelper.IsNumeric(refno.Trim()))
            {
                result = _betListService.GetBetListByRefNoTickets(new BetListByRefNoCriteria
                {
                    TransId = Converter.To<long>(refno)
                });
            }

            Hashtable paramHastable = new Hashtable();
            int custId = result.Count == 0 ? 0 : Converter.To<int>(result[0].Ticket.CustId);
            paramHastable.Add("custId", custId);

            TempData["paramHastable"] = paramHastable;

            return PartialView("~/Views/BetList/BetListByRefNo/_BetListByRefNo.cshtml", result);
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
        /// Gets the bet list by customer info ref no.
        /// </summary>
        /// <param name="transactionId">The transaction identifier.</param>
        /// <param name="customerId">The customer identifier.</param>
        /// <returns>The bet list by customer info ref no.</returns>
        public ActionResult GetBetListByCustInfo_Refno(string transactionId, int customerId)
        {
            IList<TicketDetail> result = new List<TicketDetail>();
            if (BetListHelper.IsNumeric(transactionId.Trim()))
            {
                result = _betListService.GetBetListByCustInfo_Refno(transactionId, customerId);
            }

            Hashtable paramHastable = new Hashtable();
            paramHastable.Add("custId", customerId);

            TempData["paramHastable"] = paramHastable;

            ViewBag.Quantity = 1;
            ViewBag.NumberBegining = 1;
            return PartialView("~/Views/BetList/BetlistByCustInfo/Betlist/_BetList.cshtml", result);
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

            return View("CashOut/History.cshtml", cashOutHistory);
        }
    }
}