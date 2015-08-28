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
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Web.Mvc;
    using Fanex.AccountingReports.Core.Extensions;
    using Fanex.BetList.Accounting.Core;
    using Fanex.BetList.Accounting.Criteria;
    using Fanex.BetList.Accounting.Models;
    using Fanex.BetList.Accounting.Services;
    using Fanex.Common;
    using Fanex.NAP.Core.Business;
    using Fanex.NAP.Core.Web.Attribute;
    using SmartPunterFilter = AuthenticationFilter.AuthenticateSmartPunterAction;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class BetListController : Controller
    {
        private readonly IBetListService _betListService;
        private readonly ICheckSmartPunterService _smartPunterService = new CheckSmartPunterService();
        private readonly IResultService _resultService;

        public BetListController()
        {
            this._betListService = new BetListService();
            this._resultService = new ResultService();
        }

        /// <summary>
        /// Gets the licensee win loss report member bet list.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>The licensee win loss report member bet list.</returns>
        [HttpPost]
        [SmartPunterFilter(SmartPunterFilter.AuthenticationType.Read)]
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
        public ActionResult GetMixParlayDetail(string refno)
        {
            var result = _betListService.GetMixParlayTickets(new MixParlayDetailCriteria
            {
                RefNo = refno
            });

            return PartialView("~/Areas/BetList/Views/MemberBetListReport/_MixParlayDetail.cshtml", result);
        }

        /// <summary>
        /// Gets the bet list by reference no.
        /// </summary>
        /// <param name="refno">The ref no.</param>
        /// <returns>The bet list by reference no.</returns>
        [SmartPunterFilter(SmartPunterFilter.AuthenticationType.Read)]
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
        [SmartPunterFilter(SmartPunterFilter.AuthenticationType.Read)]
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
        /// Render Partial for SystemParlay Tab - Customer Info.
        /// </summary>
        /// <param name="transactionId">Transaction ID.</param>
        /// <param name="customerId">Customer ID.</param>
        /// <returns>Partial for SystemParlay Tab - Customer Info.</returns>
         [SmartPunterFilter(SmartPunterFilter.AuthenticationType.Read)]
        public ActionResult GetBetListByCustInfo_SystemParLayTab(string transactionId, int customerId)
        {
            SystemParlayModel result = new SystemParlayModel();
            if (BetListHelper.IsNumeric(transactionId.Trim()))
            {
                result = _betListService.GetBetListByCustInfo_SystemParLayTab(transactionId, customerId);
            }

            Hashtable paramHastable = new Hashtable();
            paramHastable.Add("custId", customerId);

            TempData["paramHastable"] = paramHastable;
            ViewBag.Quantity = 1;
            ViewBag.NumberBegining = 1;
            return PartialView("~/Views/BetList/BetlistByCustInfo/SystemParlay/_SystemParlay.cshtml", result);
        }

        public ActionResult GetBetListByIP_Username(
            string ip,
            DateTime fromDate,
            int pageSize,
            int pageIndex,
            bool isCache)
        {
            var quantity = 0;
            var result = _betListService.GetListByIP_Username(ip, fromDate, pageSize, pageIndex, isCache, ref quantity);
            var dateTmp = fromDate.ToShortDateString();
            ViewBag.NumberBegining = (pageSize * (pageIndex - 1)) + 1;
            ViewBag.Quantity = quantity;
            ViewBag.FromDate = dateTmp;
            ViewBag.IP = ip;
            return PartialView("~/Views/BetList/BetListByIP/_UsernameByIP.cshtml", result);
        }

        public ActionResult GetVirtualHRPopup(ResultPopupCriteria virtualHRPopupCriteria)
        {
            var result = _betListService.GetDetailVirtualHRPopup(virtualHRPopupCriteria);
            return PartialView("~/Views/BetList/_VirtualHRPopup.cshtml", result);
        }

        public ActionResult GetVirtualTennisPopup(ResultPopupCriteria criteria)
        {
            var result = _betListService.GetDetailVirtualTennisPopup(criteria);
            return PartialView("~/Views/BetList/_VirtualTennisPopup.cshtml", result);
        }

        public ActionResult GetTennisResult(ResultPopupCriteria criteria)
        {
            var result = _resultService.GetTennisResult(criteria, "-1", Converter.ToInt(criteria.Bettype));
            return PartialView("~/Views/BetList/TennisSporting/_TennisSportting.cshtml", result);
        }

        [HttpPost]
        public ActionResult GetResultParlay(ResultParlayCriteria criteria)
        {
            var result = _resultService.GetTennisResult(criteria, "-1", -1);
            ViewBag.criteria = criteria;
            return PartialView("~/Views/BetList/TennisSporting/_TennisSportting.cshtml", result);
        }

        /// <summary>
        /// Gets the bet list by IP soccer.
        /// </summary>
        /// <param name="customerId">The customer identifier.</param>
        /// <param name="ip">The IP address.</param>
        /// <param name="fromDate">From date.</param>
        /// <param name="isSoccer">If set to <c>true</c> [is soccer].</param>
        /// <param name="isCache">If set to <c>true</c> [is cache].</param>
        /// <param name="pageSize">Size of the page.</param>
        /// <param name="pageIndex">Index of the page.</param>
        /// <returns>The bet list by IP soccer.</returns>
        [SmartPunterFilter(SmartPunterFilter.AuthenticationType.Read)]
        public ActionResult GetBetListByIP_Soccer(
            int customerId,
            string ip,
            DateTime fromDate,
            bool isSoccer,
            bool isCache,
            int pageSize,
            int pageIndex)
        {
            Hashtable paramHastable = new Hashtable();
            paramHastable.Add("custId", customerId);

            TempData["paramHastable"] = paramHastable;

            decimal totalComm = 0;
            decimal totalWinLost = 0;
            int quantity = 0;
            var result = _betListService.GetBetListByIP_Soccer(
                customerId,
                ip,
                fromDate,
                isSoccer,
                isCache,
                pageSize,
                pageIndex,
                ref quantity,
                ref totalComm,
                ref totalWinLost);
            ViewBag.Quantity = quantity;
            ViewBag.TotalComm = totalComm;
            ViewBag.NumberBegining = (pageSize * (pageIndex - 1)) + 1;
            ViewBag.TotalWinLos = totalWinLost;
            return PartialView("~/Views/BetList/BetListByIP/_BetListByIP.cshtml", result);
        }

        /// <summary>
        /// Gets the bet list statement.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <param name="transType">Type of the trans.</param>
        /// <param name="fromDate">From date.</param>
        /// <returns>The bet list statement.</returns>
        [SmartPunterFilter(SmartPunterFilter.AuthenticationType.Read)]
        public ActionResult GetBetListStatement(string username, int transType, DateTime fromDate)
        {
            Hashtable paramHastable = new Hashtable();
            paramHastable.Add("username", username);
            TempData["paramHastable"] = paramHastable;

            decimal totalComm = 0;
            decimal totalWinLost = 0;
            int quantity = 0;
            int customerId = _betListService.GetCustIDByUsername(username);
            var result = _betListService.GetBetListStatement(customerId, transType, fromDate, ref quantity, ref totalComm, ref totalWinLost);
            ViewBag.Quantity = quantity;
            ViewBag.TotalComm = totalComm;
            ViewBag.NumberBegining = 1;
            ViewBag.TotalWinLos = totalWinLost;

            ViewBag.CustID = customerId;
            ViewBag.TransType = transType;
            ViewBag.FromDate = fromDate.ToShortDateString();
            ViewBag.CustName = username;

            return PartialView("~/Views/BetList/BetListStatement/_BetListStatement.cshtml", result);
        }

        /// <summary>
        /// Exports the bet list statement.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <param name="transType">Type of the trans.</param>
        /// <param name="fromDate">From date.</param>
        /// <returns>The bet list statement.</returns>
        [HttpGet]
        public ActionResult ExportBetlistStatement(string username, int transType, DateTime fromDate)
        {
            string filename = string.Empty;
            int customerId = _betListService.GetCustIDByUsername(username);
            ExportExcelBetlist export = new ExportExcelBetlist();
            var ms = export.ExportExcelBetListStatement(customerId, transType, fromDate, ref filename);

            if (ms != null)
            {
                ms.Seek(0, System.IO.SeekOrigin.Begin);
                return File(ms, @"application/vnd.openxmlformats-officedocument.spreadsheetml.sheet.main+xml", filename);
            }

            return new EmptyResult();
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
        [SmartPunterFilter(SmartPunterFilter.AuthenticationType.Read)]
        public ActionResult GetCombinationMP(long refNo, long transId, long betId, int custId)
        {
            var result = this._betListService.GetCombinationMP(new CombinationParlayDetailCriteria
            {
                RefNo = refNo,
                TransId = transId,
                BetdId = betId,
                CustId = custId,
                BalanceUpDay = ReportService.BalanceUp.BalanceUpDay,
                MoveBetDay = ReportService.BalanceUp.MoveBetsDay
            });

            Hashtable paramHastable = new Hashtable();
            paramHastable.Add("custId", custId);
            TempData["paramHastable"] = paramHastable;

            return PartialView("~/Areas/Betlist/Views/_CombinationMPDetail.cshtml", result);
        }

        /// <summary>
        /// Exports the bet list.
        /// </summary>
        /// <param name="criteria">The criteria parameter.</param>
        /// <returns>The bet list excel file.</returns>
        [HttpGet]
        public ActionResult ExportBetlist(WinLossMemberBetListCriteria criteria)
        {
            string filename = string.Empty;
            ExportExcelBetlist export = new ExportExcelBetlist();
            var workbook = export.ExportExcelBetList(criteria, ref filename);

            if (workbook != null)
            {
                NpoiExcelExtensions.ResponseStreamExcel(workbook, filename);
            }

            return new EmptyResult();
        }

        [HttpPost]
        public ActionResult GetBetlistRacingToteWinLoss(BetListRacingWinLossToteCriteria criteria)
        {
            var result = this._betListService.GetBetListRacingToteWinLoss(criteria);
            return Json(result);
        }
    }
}