// --------------------------------------------------------------------------
// <copyright file="IBetListService.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Ricky.Nguyen" date="2012.12.10">Created</change>
// </history>

namespace Fanex.BetList.Accounting.Services
{
    using System;
    using System.Collections.Generic;
    using Fanex.BetList.Accounting.Constants;
    using Fanex.BetList.Accounting.Criteria;
    using Fanex.BetList.Accounting.Models;
    using Fanex.NAP.Framework.Data;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public interface IBetListService
    {
        BetListDetail GetWinLossMemberBetList(WinLossMemberBetListCriteria winLossMemberBetListCriteria);

        BetListDetail GetWinLossMemberBetList(
            WinLossMemberBetListCriteria winLossMemberBetListCriteria,
            string userName,
            CurrencyContants baseCurrency);

        IList<TicketDetail> GetMixParlayTickets(MixParlayDetailCriteria mixParlayDetailCriteria);

        IList<TicketDetail> GetBetListByRefNoTickets(BetListByRefNoCriteria betListByRefNoCriteria);

        IList<BetType> GetBetTypeByDate(BetTypeByDateCriteria betTypebyDateCriteria);

        IList<TicketDetail> GetBetListByCustInfo_DateRange(
            int custId,
            DateTime fromDate,
            DateTime toDate,
            int betType,
            int pageSize,
            int pageIndex,
            ref int quantity,
            bool isCache,
            bool isLimitForU3 = false);

        IList<TicketDetail> GetBetListByCustInfo_Refno(string transactionId, int customerId);

        SystemParlayModel GetBetListByCustInfo_SystemParLayTab(string transId, int custId);

        IList<BetListByIP_Username> GetListByIP_Username(
            string ip,
            DateTime fromdate,
            int pageSize,
            int pageIndex,
            bool isCache,
            ref int quanlity);

        IList<VirtualHRPopupModel> GetDetailVirtualHRPopup(ResultPopupCriteria virtualHRPopupCriteria);

        VirtualTennisModel GetDetailVirtualTennisPopup(ResultPopupCriteria criteria);

        IList<TicketDetail> GetBetListByIP_Soccer(
            int custId,
            string ip,
            DateTime fromDate,
            bool isSoccer,
            bool isCache,
            int pageSize,
            int pageIndex,
            ref int quanlity,
            ref decimal totalComm,
            ref decimal totalWinlost);

        IList<TicketDetail> GetBetListStatement(
            int custId,
            int transType,
            DateTime fromDate,
            ref int quanlity,
            ref decimal totalComm,
            ref decimal totalWinlost);

        int GetCustIDByUsername(string username);

        CombinationParlay GetCombinationMP(ICriteria criteria);

        BetListRacingToteWinLoss GetBetListRacingToteWinLoss(BetListRacingWinLossToteCriteria criteria);

        bool IsU3SpecialCustomers(int custId);
    }
}