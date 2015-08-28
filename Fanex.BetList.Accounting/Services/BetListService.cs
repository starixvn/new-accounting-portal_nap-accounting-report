// --------------------------------------------------------------------------
// <copyright file="BetListService.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Ricky.Nguyen" date="2012.12.10">Created</change>
// </history>

namespace Fanex.BetList.Accounting.Services
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Data;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Text;
    using Fanex.BetList.Accounting.Constants;
    using Fanex.BetList.Accounting.Core;
    using Fanex.BetList.Accounting.Criteria;
    using Fanex.BetList.Accounting.Extensions;
    using Fanex.BetList.Accounting.Models;
    using Fanex.BetList.Accounting.Repositories;
    using Fanex.BetList.Accounting.Resources;
    using Fanex.BetList.Core.Builder;
    using Fanex.BetList.Core.Entities;
    using Fanex.BetList.Core.Factories;
    using Fanex.BetList.Core.TicketHelper;
    using Fanex.Common;
    using Fanex.Data;
    using Fanex.NAP.Framework.Data;
    using RemotingRef;
    using AccBetListCore = Fanex.BetList.Accounting.Core;
    using AppSession = Fanex.AccountingReports.Core.Web.AppSession;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class BetListService : IBetListService
    {
        public const string BetListSessionKey = "Fanex.BetList.Accounting.WinLossByLicensee";
        private readonly IBetListRepository _betListRepository;

        public BetListService()
        {
            _betListRepository = new BetListRepository();
        }

        /// <summary>
        /// Gets the win loss member bet list.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>The win loss member bet list.</returns>
        [SuppressMessage("StyleCop.CSharp.Nexcel.NexcelCustomRules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        public BetListDetail GetWinLossMemberBetList(WinLossMemberBetListCriteria criteria)
        {
            BetListDetail betListDetail = new BetListDetail();
            try
            {
                Func<DataRow, TicketDetail> mapTicketObjectFunction = r => new TicketDetail
                {
                    Ticket = new Ticket
                    {
                        ActualStake = Converter.To<int>(r["actual_stake"]),
                        AgentWinlost = Converter.ToDecimal(r["LicenseeWL"]),
                        Comm = Converter.ToDecimal(r["comm"]),
                        AwayId = Converter.To<long>(r["awayid"]),
                        BetId = Converter.To<long>(r["betid"]),
                        BetTeam = Converter.ToString2(r["betteam"]),
                        BetTypeId = Converter.To<int>(r["bettype"]),
                        CustId = Converter.To<long>(r["custid"]),
                        Handicap1 = Converter.ToDecimal(r["hdp1"]),
                        Handicap2 = Converter.ToDecimal(r["hdp2"]),
                        HomeId = Converter.To<long>(r["homeid"]),
                        EventDate = Converter.ToDateTime(r["eventdate"]),
                        IP = Converter.ToString2(r["ip"]),
                        IsLive = Converter.ToBool(r["liveindicator"]),
                        IsNeutral = Converter.ToBool(r["isNeutral"]),
                        LeagueId = Converter.To<int>(r["leagueid"]),
                        LiveHomeScore = Converter.To<int>(r["livehomescore"]),
                        LiveAwayScore = Converter.To<int>(r["liveawayscore"]),

                        /*MasterWinlost = Converter.ToDecimal(r["mwinlost"]),
                        MasterPositionTaking = Converter.ToDecimal(r["mpositiontaking"]),
                        AgentComm = Converter.ToDecimal(r["acomm"]),
                        MasterDiscount = Converter.ToDecimal(r["adiscount"]),*/

                        MasterOdds = Converter.ToDecimal(r["modds"]),
                        MatchCode = Converter.ToString2(r["matchcode"]),
                        MatchId = Converter.To<int>(r["matchid"]),
                        Odds = Converter.ToDecimal(r["odds"]),
                        OddsType = Converter.ToString2(r["OddsType"]),
                        PlayerComm = Converter.ToDecimal(r["PlayerComm"]),
                        Race = Converter.ToString2(r["race"]),
                        RefNo = Converter.ToString2(r["refno"]),
                        ShowTime = Converter.ToString2(r["showtime"]),
                        SportTypeId = Converter.To<int>(r["sporttype"]),
                        Stake = Converter.To<decimal>(r["stake"]),
                        Status = Converter.ToString2(r["status"]),

                        /*SuperWinlost = Converter.ToDecimal(r["swinlost"]),
                        SuperPositionTaking = Converter.ToDecimal(r["spositiontaking"]),
                        SuperComm = Converter.ToDecimal(r["scomm"]),
                        SuperDiscount = Converter.ToDecimal(r["sdiscount"]),*/

                        TransDate = Converter.ToDateTime(r["transdate"]),
                        TransId = Converter.To<long>(r["transid"]),
                        Winlost = Converter.ToDecimal(r["winlost"]),
                        WinlostDate = Convert.ToDateTime(r["winlostdate"])
                    },
                    ChoiceHtml = string.Empty,
                    OddHtml = string.Empty,
                    StakeHtml = string.Empty,
                    IsShowOtExRate = Converter.To<bool>(r["isShowOTExRate"]),
                    VirtualRate = Converter.ToDecimal(r["virtualrate"]),
                    BetFrom = r["betfrom"].ToString(),
                    AllowDelete = Converter.To<bool>(r["allowdelete"])
                };

                string baseCurrencyString = string.Empty;
                CurrencyContants baseCurrency = (CurrencyContants)criteria.BaseCurrency;
                var currencyName = ReportService.GetCurrencyName(baseCurrency);
                if (!string.IsNullOrEmpty(currencyName))
                {
                    baseCurrencyString = " - " + currencyName + " " + Labels.Base_Currency;
                }

                criteria.MoveBetsDay = ReportService.BalanceUp.MoveBetsDay;
                criteria.BalanceUpDay = ReportService.BalanceUp.BalanceUpDay;
                betListDetail.Title = string.Format(
                    BetListResources.BetList_MemberReport,
                    criteria.FromDate.ToShortFormatedDate(),
                    criteria.ToDate.ToShortFormatedDate(),
                    criteria.CustName,
                    baseCurrencyString);

                List<ITicketData> outputLiveCasino = new List<ITicketData>();
                betListDetail.BetDetails = _betListRepository.FetchMemberWinLoss<TicketDetail>(
                    criteria, mapTicketObjectFunction, ref outputLiveCasino);

                if (betListDetail.BetDetails != null)
                {
                    ITicketHelper ticketHelper = new TicketHelper();
                    int index = 0;
                    BetListHTMLFactory factory = new BetListHTMLFactory();
                    BetListHTMLBuilder builder = factory.CreateBuilder();
                    GetCachePropertyById funcGetOddsTypeName = AccBetListCore.CacheRef.GetOddsTypeName;
                    foreach (TicketDetail betDetail in betListDetail.BetDetails)
                    {
                        index++;
                        ticketHelper.Index = index;
                        ticketHelper.IsShowOddsWithChoice = this.IsShowOddsWithChoice(betDetail.Ticket.BetTypeId);

                        // build HTML
                        betDetail.TransDateHtml = builder.BuildTrans(betDetail.Ticket);
                        betDetail.ChoiceHtml = builder.BuildChoice(betDetail.Ticket, ticketHelper, outputLiveCasino, false);
                        betDetail.StakeHtml = BetListExtension.BuildStake(betDetail, ref builder);
                        betDetail.OddHtml = builder.BuildOdds(betDetail.Ticket, outputLiveCasino, funcGetOddsTypeName);

                        // calculate total
                        betListDetail.TotalWinLoss += betDetail.Ticket.Winlost + betDetail.Ticket.PlayerComm;
                        betListDetail.TotalLicenseeWinLoss += betDetail.Ticket.AgentWinlost + betDetail.Ticket.Comm;
                    }

                    Core.SessionManager.Set<BetListDetail>(BetListService.BetListSessionKey, betListDetail);

                    return betListDetail;
                }
            }
            catch (Exception ex)
            {
                LoggingHandler.Log(ex);
            }

            return betListDetail;
        }

        /// <summary>
        /// Gets the win loss member bet list.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <param name="userName">Name of the user.</param>
        /// <param name="baseCurrency">The base currency.</param>
        /// <returns>The win loss member bet list.</returns>
        [SuppressMessage("StyleCop.CSharp.Nexcel.NexcelCustomRules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        public BetListDetail GetWinLossMemberBetList(
            WinLossMemberBetListCriteria criteria,
            string userName,
            CurrencyContants baseCurrency)
        {
            BetListDetail betListDetail = new BetListDetail();
            try
            {
                string baseCurrencyString = string.Empty;
                var currencyName = ReportService.GetCurrencyName(baseCurrency);
                if (!string.IsNullOrEmpty(currencyName))
                {
                    baseCurrencyString = " - " + currencyName + " " + Labels.Base_Currency;
                }

                criteria.MoveBetsDay = ReportService.BalanceUp.MoveBetsDay;
                criteria.BalanceUpDay = ReportService.BalanceUp.BalanceUpDay;
                betListDetail.BetDetails = _betListRepository.Fetch(criteria, baseCurrency);
                betListDetail.Title = string.Format(
                    BetListResources.BetList_MemberReport,
                    criteria.FromDate.ToShortDateString(),
                    criteria.ToDate.ToShortDateString(),
                    userName,
                    baseCurrencyString);

                StringBuilder content = new StringBuilder();
                ITicketHelper ticketHelper = new TicketHelper();
                if (betListDetail.BetDetails != null)
                {
                    BetListHTMLFactory factory = new BetListHTMLFactory();
                    BetListHTMLBuilder builder = factory.CreateBuilder();
                    int index = 0;
                    GetCachePropertyById funcGetOddsTypeName = AccBetListCore.CacheRef.GetOddsTypeName;
                    foreach (TicketDetail betDetail in betListDetail.BetDetails)
                    {
                        index++;
                        ticketHelper.Index = index;
                        betDetail.ChoiceHtml = builder.BuildChoice(betDetail.Ticket, ticketHelper, null, false);
                        betDetail.StakeHtml = builder.BuildStake(betDetail.Ticket);
                        betDetail.OddHtml = builder.BuildOdds(betDetail.Ticket, null, funcGetOddsTypeName);
                        betDetail.Ticket.IP = IpService.ReplaceVNIP(betDetail.Ticket.IP);
                        betListDetail.TotalWinLoss += betDetail.Ticket.Winlost;
                        betListDetail.TotalLicenseeWinLoss += betDetail.Ticket.AgentWinlost;
                    }

                    return betListDetail;
                }
            }
            catch (Exception ex)
            {
                LoggingHandler.Log(ex);
            }

            return betListDetail;
        }

        /// <summary>
        /// Get Detail Mix Parlay ticket.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>Detail Mix Parlay ticket.</returns>
        [SuppressMessage("StyleCop.CSharp.Nexcel.NexcelCustomRules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        public IList<TicketDetail> GetMixParlayTickets(MixParlayDetailCriteria criteria)
        {
            Func<DataRow, TicketDetail> mapTicketObjectFunction = r => new TicketDetail
            {
                Ticket = new Ticket
                {
                    SportTypeId = Converter.ToInt(r["Sporttype"]),
                    TransId = Converter.To<long>(r["TransID"]),
                    BetTypeId = Converter.ToInt(r["Bettype"]),
                    Handicap1 = Converter.ToDecimal(r["hdp1"]),
                    Handicap2 = Converter.ToDecimal(r["hdp2"]),
                    Odds = Converter.ToDecimal(r["Odds"]),
                    BetTeam = Converter.ToString2(r["Betteam"]),
                    StatusId = Converter.ToInt(r["statusid"]),
                    IsLive = Converter.ToBool(r["Liveindicator"]),
                    LiveHomeScore = Converter.ToInt(r["Livehomescore"]),
                    LiveAwayScore = Converter.ToInt(r["Liveawayscore"]),
                    LeagueId = Converter.ToInt(r["LeagueID"]),
                    MatchId = Converter.ToInt(r["MatchID"]),
                    MatchCode = Converter.ToString2(r["MatchCode"]),
                    HomeId = Converter.To<long>(r["HomeID"]),
                    AwayId = Converter.To<long>(r["AwayID"]),
                    IsNeutral = Converter.ToBool(r["isNeutral"]),
                    BetCheck = Converter.ToString2(r["BetCheck"])
                },
                ChoiceHtml = string.Empty,
                OddHtml = string.Empty,
                StakeHtml = string.Empty
            };

            try
            {
                ITicketHelper ticketHelper = new TicketHelper();
                IList<TicketDetail> betDetails = _betListRepository.Fetch<TicketDetail>(criteria, mapTicketObjectFunction);
                StringBuilder content = new StringBuilder();
                DataTable commandData = new DataTable();
                if (betDetails != null)
                {
                    BetListHTMLFactory factory = new BetListHTMLFactory();
                    BetListHTMLBuilder builder = factory.CreateBuilder();
                    GetCachePropertyById funcGetOddsTypeName = AccBetListCore.CacheRef.GetOddsTypeName;

                    ticketHelper.IsShowOddsWithChoice = true;
                    var lastTicket = betDetails.LastOrDefault();
                    var totalItem = betDetails.Count;
                    foreach (TicketDetail betDetail in betDetails)
                    {
                        totalItem--;
                        betDetail.ChoiceHtml =
                            "<div class='ticketList'>" + builder.BuildChoice(betDetail.Ticket, ticketHelper, null, false)
                                + (totalItem > 0 ?
                                "</div>" + "<div class=\"line\"></div>" :
                                "</div>");
                    }

                    return betDetails;
                }
            }
            catch (Exception ex)
            {
                LoggingHandler.Log(ex);
            }

            return new List<TicketDetail>();
        }

        /// <summary>
        /// Get Bet List by ref no.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>Bet List by ref no.</returns>
        [SuppressMessage("StyleCop.CSharp.Nexcel.NexcelCustomRules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        public IList<TicketDetail> GetBetListByRefNoTickets(BetListByRefNoCriteria criteria)
        {
            Func<DataRow, TicketDetail> mapTicketObjectFunction = r => new TicketDetail
            {
                Ticket = new Ticket
                {
                    ActualStake = Converter.To<int>(r["actual_stake"]),

                    AgentWinlost = Converter.ToDecimal(r["awinlost"]),
                    AgentPositionTaking = Converter.ToDecimal(r["apositiontaking"]),
                    Comm = Converter.ToDecimal(r["comm"]),
                    AgentDiscount = Converter.ToDecimal(r["discount"]),

                    AwayId = Converter.To<long>(r["awayid"]),
                    BetId = Converter.To<long>(r["betid"]),
                    BetTeam = Converter.ToString2(r["betteam"]),
                    BetTypeId = Converter.To<int>(r["bettype"]),
                    CommStatus = Converter.To<string>(r["comstatus"]),
                    CustId = Converter.To<long>(r["custid"]),
                    Handicap1 = Converter.ToDecimal(r["hdp1"]),
                    Handicap2 = Converter.ToDecimal(r["hdp2"]),
                    HomeId = Converter.To<long>(r["homeid"]),
                    EventDate = Converter.ToDateTime(r["eventdate"]),
                    IP = Converter.ToString2(r["ip"]),
                    IsLive = Converter.ToBool(r["liveindicator"]),
                    IsNeutral = Converter.ToBool(r["isNeutral"]),
                    LeagueId = Converter.To<int>(r["leagueid"]),
                    LiveHomeScore = Converter.To<int>(r["livehomescore"]),
                    LiveAwayScore = Converter.To<int>(r["liveawayscore"]),

                    MasterWinlost = Converter.ToDecimal(r["mwinlost"]),
                    MasterPositionTaking = Converter.ToDecimal(r["mpositiontaking"]),
                    AgentComm = Converter.ToDecimal(r["acomm"]),
                    MasterDiscount = Converter.ToDecimal(r["adiscount"]),

                    MasterOdds = Converter.ToDecimal(r["modds"]),
                    MatchCode = Converter.ToString2(r["matchcode"]),
                    MatchId = Converter.To<int>(r["matchid"]),
                    Odds = Converter.ToDecimal(r["odds"]),
                    OddsType = Converter.ToString2(r["OddsType"]),
                    PlayerComm = Converter.ToDecimal(r["commission"]),
                    Race = Converter.ToString2(r["race"]),
                    RefNo = Converter.ToString2(r["refno"]),
                    ShowTime = Converter.ToString2(r["showtime"]),
                    SportTypeId = Converter.To<int>(r["sporttype"]),
                    Stake = Converter.To<decimal>(r["stake"]),
                    Status = Converter.ToString2(r["status"]),

                    SuperWinlost = Converter.ToDecimal(r["swinlost"]),
                    SuperPositionTaking = Converter.ToDecimal(r["spositiontaking"]),
                    SuperComm = Converter.ToDecimal(r["scomm"]),
                    SuperDiscount = Converter.ToDecimal(r["sdiscount"]),

                    TransDate = Converter.ToDateTime(r["transdate"]),
                    TransId = Converter.To<long>(r["transid"]),
                    UserName = Converter.ToString2(r["username"]),
                    Winlost = Converter.ToDecimal(r["winlost"]),
                    WinlostDate = Convert.ToDateTime(r["winlostdate"]),

                    BetCheck = Convert.ToString(r["betcheck"])
                },
                ChoiceHtml = string.Empty,
                OddHtml = string.Empty,
                StakeHtml = string.Empty,
                IsShowOtExRate = Converter.To<bool>(r["isShowOTExRate"]),
                VirtualRate = Converter.ToDecimal(r["virtualrate"]),
                AgentPositionTaking2 = Converter.ToDecimal(r["apositiontaking2"]),
                MasterPositionTaking2 = Converter.ToDecimal(r["mpositiontaking2"]),
                SuperPositionTaking2 = Converter.ToDecimal(r["spositiontaking2"])
            };

            try
            {
                ITicketHelper ticketHelper = new TicketHelper();
                List<ITicketData> outputLiveCasino = new List<ITicketData>();
                IList<TicketDetail> betDetails = _betListRepository.Fetch<TicketDetail>(criteria, mapTicketObjectFunction, ref outputLiveCasino);
                StringBuilder content = new StringBuilder();
                BetListHelper betListHelper = new BetListHelper();
                if (betDetails != null)
                {
                    BetListHTMLFactory factory = new BetListHTMLFactory();
                    BetListHTMLBuilder builder = factory.CreateBuilder();
                    GetCachePropertyById funcGetOddsTypeName = AccBetListCore.CacheRef.GetOddsTypeName;
                    foreach (TicketDetail betDetail in betDetails)
                    {
                        ticketHelper.IsShowOddsWithChoice = this.IsShowOddsWithChoice(betDetail.Ticket.BetTypeId);
                        betDetail.ChoiceHtml = builder.BuildChoice(betDetail.Ticket, ticketHelper, outputLiveCasino, false);
                        betDetail.StakeHtml = BetListExtension.BuildStake(betDetail, ref builder);
                        betDetail.OddHtml = builder.BuildOdds(betDetail.Ticket, outputLiveCasino, funcGetOddsTypeName);
                        betDetail.StatusHtml = betListHelper.StatusRefNo(betDetail);
                    }

                    return betDetails;
                }
            }
            catch (Exception ex)
            {
                LoggingHandler.Log(ex);
            }

            return new List<TicketDetail>();
        }

        /// <summary>
        /// Gets bet types by date after change date range.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>Bet types by date after change date range.</returns>
        [SuppressMessage("StyleCop.CSharp.Nexcel.NexcelCustomRules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        public IList<BetType> GetBetTypeByDate(BetTypeByDateCriteria criteria)
        {
            bool isLimitForU3 = (AppSession.IsU3Account && !AppSession.IsU3SpecialAccount)
            || (AppSession.IsU3SpecialAccount && !this.IsU3SpecialCustomers(criteria.CustId));

            if (isLimitForU3)
            {
                criteria.SetSettingKeyForU3();
            }

            IList<BetType> betTypeList = new List<BetType>();

            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                DataSet dataSet = db.ExecuteDataSet(criteria);
                if (dataSet != null &&
                    dataSet.Tables.Count > 0)
                {
                    foreach (DataRow r in dataSet.Tables[0].Rows)
                    {
                        string betTypeNameTmp = r["typenamee"].ToString();
                        int betTypeIdTmp = Converter.To<int>(r["typeid"]);
                        if (ReportService.IsVirtualSportBetType(betTypeIdTmp))
                        {
                            betTypeNameTmp += " (" + Labels.Virtual_Sports + ")";
                        }

                        if (ReportService.IsGoldDeluxe(betTypeIdTmp))
                        {
                            betTypeNameTmp += " (" + Labels.Gold_Deluxe + ")";
                        }

                        if (betTypeIdTmp == BetTypeContants.OverUnder ||
                            betTypeIdTmp == BetTypeContants.OddEven)
                        {
                            betTypeNameTmp += " (" + Labels.Number_Game + ")";
                        }

                        betTypeList.Add(new BetType
                        {
                            BetTypeID = betTypeIdTmp,
                            BetTypeName = betTypeNameTmp
                        });
                    }
                }
            }

            var all = new BetType
            {
                BetTypeName = Labels.All,
                BetTypeID = 0
            };

            betTypeList.Insert(0, all);
            if (betTypeList != null)
            {
                return betTypeList;
            }

            return new List<BetType>();
        }

        /// <summary>
        /// Gets the bet list by customer info date range.
        /// </summary>
        /// <param name="custId">The customer identifier.</param>
        /// <param name="fromDate">From date value.</param>
        /// <param name="toDate">To date value.</param>
        /// <param name="betType">Type of the bet.</param>
        /// <param name="pageSize">Size of the page.</param>
        /// <param name="pageIndex">Index of the page.</param>
        /// <param name="quantity">The quantity.</param>
        /// <param name="isCache">If set to <c>true</c> [is cache].</param>
        /// <param name="isLimitForU3">If set to <c>true</c> [is u3 account].</param>
        /// <returns>
        /// The bet list by customer info_ date range.
        /// </returns>
        [SuppressMessage("StyleCop.CSharp.Nexcel.NexcelCustomRules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        public IList<TicketDetail> GetBetListByCustInfo_DateRange(
            int custId,
            DateTime fromDate,
            DateTime toDate,
            int betType,
            int pageSize,
            int pageIndex,
            ref int quantity,
            bool isCache,
            bool isLimitForU3 = false)
        {
            Func<DataRow, TicketDetail> mapTicketObjectFunction = r => new TicketDetail
            {
                Ticket = new Ticket
                {
                    ActualStake = Converter.To<int>(r["actual_stake"]),
                    AgentWinlost = Converter.ToDecimal(r["awinlost"]),
                    AgentPositionTaking = Converter.ToDecimal(r["apositiontaking"]),
                    Comm = Converter.ToDecimal(r["comm"]),
                    AgentDiscount = Converter.ToDecimal(r["discount"]),

                    AwayId = Converter.To<long>(r["awayid"]),
                    BetId = Converter.To<long>(r["betid"]),
                    BetTeam = Converter.ToString2(r["betteam"]),
                    BetTypeId = Converter.To<int>(r["bettype"]),
                    CommStatus = Converter.To<string>(r["comstatus"]),
                    CustId = Converter.To<long>(r["custid"]),
                    Handicap1 = Converter.ToDecimal(r["hdp1"]),
                    Handicap2 = Converter.ToDecimal(r["hdp2"]),
                    HomeId = Converter.To<long>(r["homeid"]),
                    EventDate = Converter.ToDateTime(r["eventdate"]),
                    IP = Converter.ToString2(r["ip"]),
                    IsLive = Converter.ToBool(r["liveindicator"]),
                    IsNeutral = Converter.ToBool(r["isNeutral"]),
                    LeagueId = Converter.To<int>(r["leagueid"]),
                    LiveHomeScore = Converter.To<int>(r["livehomescore"]),
                    LiveAwayScore = Converter.To<int>(r["liveawayscore"]),

                    MasterWinlost = Converter.ToDecimal(r["mwinlost"]),
                    MasterPositionTaking = Converter.ToDecimal(r["mpositiontaking"]),
                    AgentComm = Converter.ToDecimal(r["acomm"]),
                    MasterDiscount = Converter.ToDecimal(r["adiscount"]),

                    MasterOdds = Converter.ToDecimal(r["modds"]),
                    MatchCode = Converter.ToString2(r["matchcode"]),
                    MatchId = Converter.To<int>(r["matchid"]),
                    Odds = Converter.ToDecimal(r["odds"]),
                    OddsType = Converter.ToString2(r["OddsType"]),
                    PlayerComm = Converter.ToDecimal(r["commission"]),
                    Race = Converter.ToString2(r["race"]),
                    RefNo = Converter.ToString2(r["refno"]),
                    ShowTime = Converter.ToString2(r["showtime"]),
                    SportTypeId = Converter.To<int>(r["sporttype"]),
                    Stake = Converter.To<decimal>(r["stake"]),
                    Status = Converter.ToString2(r["status"]),

                    SuperWinlost = Converter.ToDecimal(r["swinlost"]),
                    SuperPositionTaking = Converter.ToDecimal(r["spositiontaking"]),
                    SuperComm = Converter.ToDecimal(r["scomm"]),
                    SuperDiscount = Converter.ToDecimal(r["sdiscount"]),

                    TransDate = Converter.ToDateTime(r["transdate"]),
                    TransId = Converter.To<long>(r["transid"]),
                    Winlost = Converter.ToDecimal(r["winlost"]),
                    WinlostDate = Convert.ToDateTime(r["winlostdate"]),

                    BetCheck = Convert.ToString(r["betcheck"])
                },
                ChoiceHtml = string.Empty,
                OddHtml = string.Empty,
                StakeHtml = string.Empty,
                IsShowOtExRate = Converter.To<bool>(r["isShowOTExRate"]),
                VirtualRate = Converter.ToDecimal(r["virtualrate"]),
                AgentPositionTaking2 = Converter.ToDecimal(r["apositiontaking2"]),
                MasterPositionTaking2 = Converter.ToDecimal(r["mpositiontaking2"]),
                SuperPositionTaking2 = Converter.ToDecimal(r["spositiontaking2"]),
                BetFrom = r["betfrom"].ToString(),
                AllowDelete = Converter.To<bool>(r["allowdelete"])
            };

            try
            {
                BetListByCustInfoCriteria betListByCustInfoCriteria = new BetListByCustInfoCriteria
                {
                    CustId = custId,
                    FromDate = fromDate,
                    ToDate = toDate,
                    BetType = betType,
                    MoveBetsDay = ReportService.BalanceUp.MoveBetsDay,
                };

                BetListByCustInfoU3Criteria betListByCustInfoU3Criteria = new BetListByCustInfoU3Criteria
                {
                    CustId = custId,
                    FromDate = fromDate,
                    ToDate = toDate,
                    BetType = betType,
                    MoveBetsDay = ReportService.BalanceUp.MoveBetsDay,
                };

                BetListByCustInfoCriteria_Old betListByCustInfoCriteria_Old = new BetListByCustInfoCriteria_Old
                {
                    CustId = custId,
                    FromDate = fromDate,
                    ToDate = toDate,
                    BetType = betType,
                };

                BetListByCustInfoU3Criteria_Old betListByCustInfoU3Criteria_Old = new BetListByCustInfoU3Criteria_Old
                {
                    CustId = custId,
                    FromDate = fromDate,
                    ToDate = toDate,
                    BetType = betType,
                };

                ITicketHelper ticketHelper = new TicketHelper();
                List<ITicketData> outputLiveCasino = new List<ITicketData>();
                List<ITicketData> outputLiveCasino_Old = new List<ITicketData>();
                List<ITicketData> outputLiveCasino_Merge = new List<ITicketData>();
                IList<TicketDetail> betDetails = new List<TicketDetail>();
                IList<TicketDetail> betDetailsOld = new List<TicketDetail>();
                IList<TicketDetail> betDetailsMerge = new List<TicketDetail>();

                betDetailsMerge = SessionManager.Get<IList<TicketDetail>>(BetListConstants.BetListByCustomerInfoSessionKey);
                if (!isCache || betDetailsMerge == null || betDetailsMerge.Count == 0)
                {
                    if (toDate >= ReportService.BalanceUp.BalanceUpDay)
                    {
                        betDetails = isLimitForU3 ?
                            _betListRepository.Fetch<TicketDetail>(betListByCustInfoU3Criteria, mapTicketObjectFunction, ref outputLiveCasino) :
                            _betListRepository.Fetch<TicketDetail>(betListByCustInfoCriteria, mapTicketObjectFunction, ref outputLiveCasino);
                    }

                    if (fromDate < ReportService.BalanceUp.BalanceUpDay)
                    {
                        betDetailsOld = isLimitForU3 ?
                            _betListRepository.Fetch<TicketDetail>(betListByCustInfoU3Criteria_Old, mapTicketObjectFunction, ref outputLiveCasino_Old) :
                            _betListRepository.Fetch<TicketDetail>(betListByCustInfoCriteria_Old, mapTicketObjectFunction, ref outputLiveCasino_Old);
                    }

                    betDetailsMerge = betDetails.Concat(betDetailsOld).ToList();
                    betDetailsMerge = betDetailsMerge.OrderBy(x => x.Ticket.TransDate).ToList();
                    outputLiveCasino_Merge = outputLiveCasino.Concat(outputLiveCasino_Old).ToList();
                    SessionManager.Set<IList<TicketDetail>>(BetListConstants.BetListByCustomerInfoSessionKey, betDetailsMerge);
                    SessionManager.Set<List<ITicketData>>(BetListConstants.BetListOutputLiveCasinoSessionKey, outputLiveCasino_Merge);
                }
                else
                {
                    betDetailsMerge = SessionManager.Get<IList<TicketDetail>>(BetListConstants.BetListByCustomerInfoSessionKey);
                    outputLiveCasino_Merge = SessionManager.Get<List<ITicketData>>(BetListConstants.BetListOutputLiveCasinoSessionKey);
                }

                quantity = betDetailsMerge.Count;
                StringBuilder content = new StringBuilder();
                BetListHelper betlistHelper = new BetListHelper();

                if (pageIndex > 0 && betDetailsMerge != null)
                {
                    IList<TicketDetail> betDetailsMergeModel = new List<TicketDetail>();
                    BetListHTMLFactory factory = new BetListHTMLFactory();
                    BetListHTMLBuilder builder = factory.CreateBuilder();
                    GetCachePropertyById funcGetOddsTypeName = AccBetListCore.CacheRef.GetOddsTypeName;

                    int from = pageSize * (pageIndex - 1);
                    int to = pageSize * pageIndex;
                    for (int i = from; i < betDetailsMerge.Count && i < to; i++)
                    {
                        betDetailsMergeModel.Add(betDetailsMerge[i]);
                    }

                    foreach (TicketDetail betDetail in betDetailsMergeModel)
                    {
                        ticketHelper.IsShowOddsWithChoice = this.IsShowOddsWithChoice(betDetail.Ticket.BetTypeId);
                        betDetail.ChoiceHtml = builder.BuildChoice(betDetail.Ticket, ticketHelper, outputLiveCasino_Merge, false);
                        betDetail.StakeHtml = BetListExtension.BuildStake(betDetail, ref builder);
                        betDetail.OddHtml = builder.BuildOdds(betDetail.Ticket, outputLiveCasino, funcGetOddsTypeName);
                        betDetail.StatusHtml = betlistHelper.StatusRefNo(betDetail);
                    }

                    return betDetailsMergeModel;
                }
            }
            catch (Exception ex)
            {
                LoggingHandler.Log(ex);
            }

            return new List<TicketDetail>();
        }

        /// <summary>
        /// Get bet list data for Customer info bet list by ref no - bet list tab.
        /// </summary>
        /// <param name="transactionId">The transaction identifier.</param>
        /// <param name="customerId">The customer identifier.</param>
        /// <returns>
        /// Bet list data for Customer info bet list by ref no - bet list tab.
        /// </returns>
        [SuppressMessage("StyleCop.CSharp.Nexcel.NexcelCustomRules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        public IList<TicketDetail> GetBetListByCustInfo_Refno(string transactionId, int customerId)
        {
            var criteria = new BetListByCustInfoByRefnoCriteria()
            {
                TransId = transactionId,
                CustId = customerId
            };

            var criteriaForU3 = new BetListByCustInfoByRefnoU3Criteria()
            {
                TransId = transactionId,
                CustId = customerId
            };

            Func<DataRow, TicketDetail> mapTicketObjectFunction = r => new TicketDetail
            {
                Ticket = new Ticket
                {
                    ActualStake = Converter.To<int>(r["actual_stake"]),
                    AgentWinlost = Converter.ToDecimal(r["awinlost"]),
                    AgentPositionTaking = Converter.ToDecimal(r["apositiontaking"]),
                    Comm = Converter.ToDecimal(r["comm"]),
                    AgentDiscount = Converter.ToDecimal(r["discount"]),

                    AwayId = Converter.To<long>(r["awayid"]),
                    BetId = Converter.To<long>(r["betid"]),
                    BetTeam = Converter.ToString2(r["betteam"]),
                    BetTypeId = Converter.To<int>(r["bettype"]),
                    CommStatus = Converter.To<string>(r["comstatus"]),
                    CustId = Converter.To<long>(r["custid"]),
                    Handicap1 = Converter.ToDecimal(r["hdp1"]),
                    Handicap2 = Converter.ToDecimal(r["hdp2"]),
                    HomeId = Converter.To<long>(r["homeid"]),
                    EventDate = Converter.ToDateTime(r["eventdate"]),
                    IP = Converter.ToString2(r["ip"]),
                    IsLive = Converter.ToBool(r["liveindicator"]),
                    IsNeutral = Converter.ToBool(r["isNeutral"]),
                    LeagueId = Converter.To<int>(r["leagueid"]),
                    LiveHomeScore = Converter.To<int>(r["livehomescore"]),
                    LiveAwayScore = Converter.To<int>(r["liveawayscore"]),

                    MasterWinlost = Converter.ToDecimal(r["mwinlost"]),
                    MasterPositionTaking = Converter.ToDecimal(r["mpositiontaking"]),
                    AgentComm = Converter.ToDecimal(r["acomm"]),
                    MasterDiscount = Converter.ToDecimal(r["adiscount"]),

                    MasterOdds = Converter.ToDecimal(r["modds"]),
                    MatchCode = Converter.ToString2(r["matchcode"]),
                    MatchId = Converter.To<int>(r["matchid"]),
                    Odds = Converter.ToDecimal(r["odds"]),
                    OddsType = Converter.ToString2(r["OddsType"]),
                    PlayerComm = Converter.ToDecimal(r["commission"]),
                    Race = Converter.ToString2(r["race"]),
                    RefNo = Converter.ToString2(r["refno"]),
                    ShowTime = Converter.ToString2(r["showtime"]),
                    SportTypeId = Converter.To<int>(r["sporttype"]),
                    Stake = Converter.To<decimal>(r["stake"]),
                    Status = Converter.ToString2(r["status"]),

                    SuperWinlost = Converter.ToDecimal(r["swinlost"]),
                    SuperPositionTaking = Converter.ToDecimal(r["spositiontaking"]),
                    SuperComm = Converter.ToDecimal(r["scomm"]),
                    SuperDiscount = Converter.ToDecimal(r["sdiscount"]),

                    TransDate = Converter.ToDateTime(r["transdate"]),
                    TransId = Converter.To<long>(r["transid"]),
                    Winlost = Converter.ToDecimal(r["winlost"]),
                    WinlostDate = Convert.ToDateTime(r["winlostdate"]),

                    BetCheck = Convert.ToString(r["betcheck"])
                },
                ChoiceHtml = string.Empty,
                OddHtml = string.Empty,
                StakeHtml = string.Empty,
                IsShowOtExRate = Converter.To<bool>(r["isShowOTExRate"]),
                VirtualRate = Converter.ToDecimal(r["virtualrate"]),
                AgentPositionTaking2 = Converter.ToDecimal(r["apositiontaking2"]),
                MasterPositionTaking2 = Converter.ToDecimal(r["mpositiontaking2"]),
                SuperPositionTaking2 = Converter.ToDecimal(r["spositiontaking2"]),
                BetFrom = r["betfrom"].ToString(),
                AllowDelete = Converter.To<bool>(r["allowdelete"])
            };

            try
            {
                ITicketHelper ticketHelper = new TicketHelper();
                List<ITicketData> outputLiveCasino = new List<ITicketData>();
                bool isLimitForU3 = (AppSession.IsU3Account && !AppSession.IsU3SpecialAccount)
                || (AppSession.IsU3SpecialAccount && !this.IsU3SpecialCustomers(customerId));

                IList<TicketDetail> betDetails = isLimitForU3 ?
                    _betListRepository.Fetch<TicketDetail>(criteriaForU3, mapTicketObjectFunction, ref outputLiveCasino) :
                    _betListRepository.Fetch<TicketDetail>(criteria, mapTicketObjectFunction, ref outputLiveCasino);

                StringBuilder content = new StringBuilder();
                BetListHelper betListHelper = new BetListHelper();

                if (betDetails != null)
                {
                    BetListHTMLFactory factory = new BetListHTMLFactory();
                    BetListHTMLBuilder builder = factory.CreateBuilder();
                    GetCachePropertyById funcGetOddsTypeName = AccBetListCore.CacheRef.GetOddsTypeName;
                    foreach (TicketDetail betDetail in betDetails)
                    {
                        ticketHelper.IsShowOddsWithChoice = this.IsShowOddsWithChoice(betDetail.Ticket.BetTypeId);
                        betDetail.ChoiceHtml = builder.BuildChoice(betDetail.Ticket, ticketHelper, outputLiveCasino, false);
                        betDetail.StakeHtml = BetListExtension.BuildStake(betDetail, ref builder);
                        betDetail.OddHtml = builder.BuildOdds(betDetail.Ticket, outputLiveCasino, funcGetOddsTypeName);
                        betDetail.StatusHtml = betListHelper.StatusRefNo(betDetail);
                    }

                    return betDetails;
                }
            }
            catch (Exception ex)
            {
                LoggingHandler.Log(ex);
            }

            return new List<TicketDetail>();
        }

        /// <summary>
        /// Get bet list data for Customer info bet list by ref no - system parlay tab.
        /// </summary>
        /// <param name="transId">The transaction ID.</param>
        /// <param name="custId">The customer ID.</param>
        /// <returns>Bet list data for Customer info bet list by ref no - system parlay tab.</returns>
        [SuppressMessage("StyleCop.CSharp.Nexcel.NexcelCustomRules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        public SystemParlayModel GetBetListByCustInfo_SystemParLayTab(string transId, int custId)
        {
            try
            {
                BetListSystemParlayByRefnoCriteria systemParlayCriteria = new BetListSystemParlayByRefnoCriteria
                {
                    CustomerId = custId,
                    TransactionId = transId,
                    MoveBetDay = ReportService.BalanceUp.MoveBetsDay,
                    BalanceUpDay = ReportService.BalanceUp.BalanceUpDay
                };

                ITicketHelper ticketHelper = new TicketHelper();
                List<ITicketData> outputLiveCasino = new List<ITicketData>();
                SystemParlayModel systempParlayModel = _betListRepository.FetchSystemParlay(systemParlayCriteria);
                StringBuilder content = new StringBuilder();
                BetListHelper betListHelper = new BetListHelper();
                if (systempParlayModel.TicketsDetailList != null &&
                    systempParlayModel.TicketsDetailList.Count > 0)
                {
                    BetListHTMLFactory factory = new BetListHTMLFactory();
                    BetListHTMLBuilder builder = factory.CreateBuilder();
                    GetCachePropertyById funcGetOddsTypeName = AccBetListCore.CacheRef.GetOddsTypeName;
                    foreach (TicketDetail betDetail in systempParlayModel.TicketsDetailList)
                    {
                        ticketHelper.IsShowOddsWithChoice = this.IsShowOddsWithChoice(betDetail.Ticket.BetTypeId);
                        betDetail.ChoiceHtml = builder.BuildChoice(betDetail.Ticket, ticketHelper, outputLiveCasino, false);
                        betDetail.StakeHtml = BetListExtension.BuildStake(betDetail, ref builder);
                        betDetail.OddHtml = builder.BuildOdds(betDetail.Ticket, outputLiveCasino, funcGetOddsTypeName);
                        betDetail.StatusHtml = betListHelper.StatusRefNo(betDetail);
                    }

                    if (systempParlayModel.SystemParlayChoice != null)
                    {
                        BetListHTMLFactory factoryParlayDetaily = new BetListHTMLFactory();
                        BetListHTMLBuilder builderParlayDetail = factoryParlayDetaily.CreateBuilder();
                        int totalItem = systempParlayModel.SystemParlayChoice.Count;
                        ticketHelper.IsShowOddsWithChoice = true;
                        foreach (TicketDetail betDetail in systempParlayModel.SystemParlayChoice)
                        {
                            totalItem--;
                            betDetail.ChoiceHtml =
                                "<div class='ticketList'>" + builderParlayDetail.BuildChoice(betDetail.Ticket, ticketHelper, null, false) +
                                (totalItem > 0 ?
                                "</div><div class=\"line\"></div>" :
                                "</div>");
                            systempParlayModel.TicketsDetailList[0].ChoiceHtml += betDetail.ChoiceHtml;
                        }
                    }

                    return systempParlayModel;
                }
            }
            catch (Exception ex)
            {
                LoggingHandler.Log(ex);
            }

            return new SystemParlayModel();
        }

        /// <summary>
        /// Gets the list by IP address username.
        /// </summary>
        /// <param name="ip">The IP address.</param>
        /// <param name="fromDate">The from date.</param>
        /// <param name="pageSize">Size of the page.</param>
        /// <param name="pageIndex">Index of the page.</param>
        /// <param name="isCache">If set to <c>true</c> [is cache].</param>
        /// <param name="quality">The quality.</param>
        /// <returns>The list by IP address username.</returns>
        [SuppressMessage("StyleCop.CSharp.Nexcel.NexcelCustomRules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        public IList<BetListByIP_Username> GetListByIP_Username(
            string ip,
            DateTime fromDate,
            int pageSize,
            int pageIndex,
            bool isCache,
            ref int quality)
        {
            BetListByIP_UsernameCriteria criteria = new BetListByIP_UsernameCriteria
            {
                FromDate = fromDate,
                IP = ip
            };

            try
            {
                IList<BetListByIP_Username> usernameList = new List<BetListByIP_Username>();
                IList<BetListByIP_Username> usernameListModel = new List<BetListByIP_Username>();
                int soccerBetsTotalTmp = 0;
                int othersBetsTotalTmp = 0;
                int totalBetsTmp = 0;
                if (!isCache)
                {
                    using (var db = new ObjectDb(criteria.GetSettingKey()))
                    {
                        DataSet dataSet = db.ExecuteDataSet(criteria);
                        if (dataSet != null &&
                            dataSet.Tables.Count > 0)
                        {
                            foreach (DataRow r in dataSet.Tables[0].Rows)
                            {
                                usernameList.Add(new BetListByIP_Username
                                {
                                    CustID = Converter.To<int>(r["custid"]),
                                    Username = r["username"].ToString(),
                                    SoccerBets = Converter.To<int>(r["soccerBet"]),
                                    OtherBets = Converter.To<int>(r["otherBet"]),
                                    TotalBets = Converter.To<int>(r["totalBet"])
                                });

                                soccerBetsTotalTmp += Converter.To<int>(r["soccerBet"]);
                                othersBetsTotalTmp += Converter.To<int>(r["otherBet"]);
                                totalBetsTmp += Converter.To<int>(r["totalBet"]);
                            }
                        }
                    }

                    usernameList.Add(new BetListByIP_Username
                    {
                        CustID = 0,
                        Username = Labels.Total,
                        SoccerBets = soccerBetsTotalTmp,
                        OtherBets = othersBetsTotalTmp,
                        TotalBets = totalBetsTmp
                    });

                    SessionManager.Set<IList<BetListByIP_Username>>(BetListConstants.BetListByUsernameSessionKey, usernameList);
                }
                else
                {
                    usernameList = SessionManager.Get<IList<BetListByIP_Username>>(BetListConstants.BetListByUsernameSessionKey);
                }

                if (pageIndex > 0)
                {
                    quality = usernameList.Count - 1;
                    int from = pageSize * (pageIndex - 1);
                    int to = pageSize * pageIndex;

                    for (int i = from; i < usernameList.Count && i < to; i++)
                    {
                        usernameListModel.Add(usernameList[i]);
                    }

                    int pageIndexTmp = quality / pageSize;
                    if (pageIndex < pageIndexTmp)
                    {
                        usernameListModel.Add(new BetListByIP_Username
                        {
                            CustID = 0,
                            Username = Labels.Total,
                            SoccerBets = soccerBetsTotalTmp,
                            OtherBets = othersBetsTotalTmp,
                            TotalBets = totalBetsTmp
                        });
                    }

                    return usernameListModel;
                }
            }
            catch (Exception ex)
            {
                LoggingHandler.Log(ex);
            }

            return new List<BetListByIP_Username>();
        }

        /// <summary>
        /// Gets the detail virtual horse racing popup.
        /// </summary>
        /// <param name="virtualHRPopupCriteria">The virtual horse racing popup criteria.</param>
        /// <returns>The detail virtual horse racing popup.</returns>
        public IList<VirtualHRPopupModel> GetDetailVirtualHRPopup(ResultPopupCriteria virtualHRPopupCriteria)
        {
            try
            {
                IList<VirtualHRPopupModel> detailList = new List<VirtualHRPopupModel>();

                using (var db = new ObjectDb(virtualHRPopupCriteria.GetSettingKey()))
                {
                    DataSet dataSet = db.ExecuteDataSet(virtualHRPopupCriteria);
                    if (dataSet != null &&
                        dataSet.Tables.Count > 0)
                    {
                        foreach (DataRow r in dataSet.Tables[0].Rows)
                        {
                            detailList.Add(new VirtualHRPopupModel
                            {
                                EventTime = Converter.ToDateTime(r["EventTime"]),
                                Place = Converter.To<int>(r["placing"]),
                                HorseNumber = Converter.To<int>(r["HorseNo"]),
                                HorseName = RefData.GetHorseTeamName(r["runner"].ToString(), "en"),
                                WinOdds = Converter.To<decimal>(r["WinOdds"]),
                                PlaceOdds = Converter.To<decimal>(r["PlaceOdds"]),
                                Status = r["IsFavor"].ToString()
                            });
                        }
                    }
                }

                return detailList ?? new List<VirtualHRPopupModel>();
            }
            catch (Exception ex)
            {
                LoggingHandler.Log(ex);
            }

            return new List<VirtualHRPopupModel>();
        }

        /// <summary>
        /// Gets the detail virtual tennis popup.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>The detail virtual tennis popup.</returns>
        public VirtualTennisModel GetDetailVirtualTennisPopup(ResultPopupCriteria criteria)
        {
            try
            {
                VirtualTennisModel virtualTennis = new VirtualTennisModel();

                using (var db = new ObjectDb(criteria.GetSettingKey()))
                {
                    DataSet dataSet = db.ExecuteDataSet(criteria);
                    if (dataSet != null &&
                        dataSet.Tables.Count > 0 &&
                        dataSet.Tables[0].Rows.Count > 0)
                    {
                        DataRow row = dataSet.Tables[0].Rows[0];
                        virtualTennis.KickOffTime = Converter.To<DateTime>(row["kickofftime"]);
                        virtualTennis.HomeName = AccBetListCore.CacheRef.GetTeamName(row["homeID"]);
                        virtualTennis.AwayName = AccBetListCore.CacheRef.GetTeamName(row["awayID"]);
                        virtualTennis.HomePoints = Converter.To<int>(row["finalhomescore"]);
                        virtualTennis.AwayPoints = Converter.To<int>(row["finalawayscore"]);
                        virtualTennis.EventStatus = row["eventstatus"].ToString();
                        virtualTennis.LeageName = AccBetListCore.CacheRef.GetLeagueName(row["LeagueID"]);
                    }
                }

                return virtualTennis ?? new VirtualTennisModel();
            }
            catch (Exception ex)
            {
                LoggingHandler.Log(ex);
            }

            return new VirtualTennisModel();
        }

        /// <summary>
        /// Gets the bet list by IP address soccer.
        /// </summary>
        /// <param name="custId">The customer identifier.</param>
        /// <param name="ip">The IP address.</param>
        /// <param name="fromDate">From date.</param>
        /// <param name="isSoccer">If set to <c>true</c> [is soccer].</param>
        /// <param name="isCache">If set to <c>true</c> [is cache].</param>
        /// <param name="pageSize">Size of the page.</param>
        /// <param name="pageIndex">Index of the page.</param>
        /// <param name="quality">The quality.</param>
        /// <param name="totalComm">The total commission.</param>
        /// <param name="totalWinLoss">The total win loss.</param>
        /// <returns>The bet list by IP address soccer.</returns>
        [SuppressMessage("StyleCop.CSharp.Nexcel.NexcelCustomRules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        public IList<TicketDetail> GetBetListByIP_Soccer(
            int custId,
            string ip,
            DateTime fromDate,
            bool isSoccer,
            bool isCache,
            int pageSize,
            int pageIndex,
            ref int quality,
            ref decimal totalComm,
            ref decimal totalWinLoss)
        {
            try
            {
                ITicketHelper ticketHelper = new TicketHelper();
                List<ITicketData> outputLiveCasino = new List<ITicketData>();
                IList<TicketDetail> betDetails = new List<TicketDetail>();
                IList<TicketDetail> betDetailsModel = new List<TicketDetail>();
                StringBuilder content = new StringBuilder();
                DataSet dataSet = new DataSet();

                if (isSoccer)
                {
                    var criteria = new BetListByIP_SoccerCriteria
                    {
                        CustId = custId,
                        IP = ip,
                        FromDate = fromDate
                    };

                    using (var db = new ObjectDb(criteria.GetSettingKey()))
                    {
                        dataSet = db.ExecuteDataSet(criteria);
                    }
                }
                else
                {
                    var criteria = new BetListByIP_OthersCriteria
                    {
                        CustId = custId,
                        IP = ip,
                        FromDate = fromDate
                    };

                    using (var db = new ObjectDb(criteria.GetSettingKey()))
                    {
                        dataSet = db.ExecuteDataSet(criteria);
                    }
                }

                if (dataSet != null &&
                    dataSet.Tables.Count >= 2)
                {
                    DataTable ticketDetailTable = dataSet.Tables[0];
                    DataTable ticketEventTable = dataSet.Tables[1];
                    foreach (DataRow r in ticketDetailTable.Rows)
                    {
                        decimal commTmp = Converter.ToDecimal(r["commission"]);
                        decimal winlostTmp = Converter.ToDecimal(r["winlost"]);
                        string status = Converter.ToString2(r["status"]).ToLower();

                        if (status != "running" &&
                            status != "waiting" &&
                            status != "void" &&
                            status != "reject")
                        {
                            totalComm += commTmp;
                            totalWinLoss += winlostTmp;
                        }

                        TicketDetail ticketDetailTmp = new TicketDetail
                        {
                            Ticket = new Ticket
                            {
                                ActualStake = Converter.To<int>(r["actual_stake"]),
                                AgentWinlost = Converter.ToDecimal(r["awinlost"]),
                                AgentPositionTaking = Converter.ToDecimal(r["apositiontaking"]),
                                Comm = Converter.ToDecimal(r["comm"]),
                                AgentDiscount = Converter.ToDecimal(r["discount"]),

                                AwayId = Converter.To<long>(r["awayid"]),
                                BetId = Converter.To<long>(r["betid"]),
                                BetTeam = Converter.ToString2(r["betteam"]),
                                BetTypeId = Converter.To<int>(r["bettype"]),
                                CommStatus = Converter.To<string>(r["comstatus"]),
                                CustId = custId,
                                Handicap1 = Converter.ToDecimal(r["hdp1"]),
                                Handicap2 = Converter.ToDecimal(r["hdp2"]),
                                HomeId = Converter.To<long>(r["homeid"]),
                                EventDate = Converter.ToDateTime(r["eventdate"]),
                                IP = Converter.ToString2(r["ip"]),
                                IsLive = Converter.ToBool(r["liveindicator"]),
                                IsNeutral = Converter.ToBool(r["isNeutral"]),
                                LeagueId = Converter.To<int>(r["leagueid"]),
                                LiveHomeScore = Converter.To<int>(r["livehomescore"]),
                                LiveAwayScore = Converter.To<int>(r["liveawayscore"]),

                                MasterWinlost = Converter.ToDecimal(r["mwinlost"]),
                                MasterPositionTaking = Converter.ToDecimal(r["mpositiontaking"]),
                                AgentComm = Converter.ToDecimal(r["acomm"]),
                                MasterDiscount = Converter.ToDecimal(r["adiscount"]),

                                MasterOdds = Converter.ToDecimal(r["modds"]),
                                MatchCode = Converter.ToString2(r["matchcode"]),
                                MatchId = Converter.To<int>(r["matchid"]),
                                Odds = Converter.ToDecimal(r["odds"]),
                                OddsType = Converter.ToString2(r["OddsType"]),
                                PlayerComm = Converter.ToDecimal(r["commission"]),
                                Race = Converter.ToString2(r["race"]),
                                RefNo = Converter.ToString2(r["refno"]),
                                ShowTime = Converter.ToString2(r["showtime"]),
                                SportTypeId = Converter.To<int>(r["sporttype"]),
                                Stake = Converter.To<decimal>(r["stake"]),
                                Status = Converter.ToString2(r["status"]),

                                SuperWinlost = Converter.ToDecimal(r["swinlost"]),
                                SuperPositionTaking = Converter.ToDecimal(r["spositiontaking"]),
                                SuperComm = Converter.ToDecimal(r["scomm"]),
                                SuperDiscount = Converter.ToDecimal(r["sdiscount"]),

                                TransDate = Converter.ToDateTime(r["transdate"]),
                                TransId = Converter.To<long>(r["transid"]),
                                UserName = Converter.ToString2(r["username"]),
                                Winlost = Converter.ToDecimal(r["winlost"]),
                                WinlostDate = Convert.ToDateTime(r["winlostdate"]),

                                BetCheck = Convert.ToString(r["betcheck"])
                            },
                            ChoiceHtml = string.Empty,
                            OddHtml = string.Empty,
                            StakeHtml = string.Empty,
                            IsShowOtExRate = Converter.To<bool>(r["isShowOTExRate"]),
                            VirtualRate = Converter.ToDecimal(r["virtualrate"]),
                            Creator = Converter.ToString2(r["creator"])
                        };

                        foreach (DataRow row in ticketEventTable.Rows)
                        {
                            if (Converter.ToString2(row["refno"]) == ticketDetailTmp.Ticket.RefNo)
                            {
                                ticketDetailTmp.Ticket.AgentPositionTaking = Converter.ToDecimal(row["apositiontaking"]);
                                ticketDetailTmp.Ticket.MasterPositionTaking = Converter.ToDecimal(row["mpositiontaking"]);
                                ticketDetailTmp.Ticket.SuperPositionTaking = Converter.ToDecimal(row["spositiontaking"]);
                                break;
                            }
                        }

                        betDetails.Add(ticketDetailTmp);
                    }

                    foreach (DataRow row in ticketEventTable.Rows)
                    {
                        outputLiveCasino.Add(new TicketData
                        {
                            RefNo = Converter.ToString2(row["refno"]),
                            BetTypeId = Converter.To<int>(row["bettype"]),
                            Status = Converter.ToString2(row["status"]),
                            Odds = Converter.To<decimal>(row["odds"]),
                            Stake = Converter.To<decimal>(row["stake"]),
                            BetTeam = Converter.ToString2(row["betteam"]),
                            TransDesc = Converter.ToString2(row["TransDesc"])
                        });
                    }
                }

                if (betDetails != null && pageIndex > 0)
                {
                    BetListHTMLFactory factory = new BetListHTMLFactory();
                    BetListHTMLBuilder builder = factory.CreateBuilder();
                    GetCachePropertyById funcGetOddsTypeName = AccBetListCore.CacheRef.GetOddsTypeName;
                    BetListHelper betlistHelper = new BetListHelper();

                    int from = pageSize * (pageIndex - 1);
                    int to = pageSize * pageIndex;
                    quality = betDetails.Count;
                    for (int i = from; i < betDetails.Count && i < to; i++)
                    {
                        betDetailsModel.Add(betDetails[i]);
                    }

                    foreach (TicketDetail betDetail in betDetailsModel)
                    {
                        ticketHelper.IsShowOddsWithChoice = this.IsShowOddsWithChoice(betDetail.Ticket.BetTypeId);
                        betDetail.ChoiceHtml = builder.BuildChoice(betDetail.Ticket, ticketHelper, outputLiveCasino, false);
                        betDetail.StakeHtml = BetListExtension.BuildStake(betDetail, ref builder);
                        betDetail.OddHtml = builder.BuildOdds(betDetail.Ticket, outputLiveCasino, funcGetOddsTypeName);
                        betDetail.StatusHtml = betlistHelper.StatusRefNo(betDetail);
                    }

                    return betDetailsModel;
                }
            }
            catch (Exception ex)
            {
                LoggingHandler.Log(ex);
            }

            return new List<TicketDetail>();
        }

        /// <summary>
        /// Gets the bet list statement.
        /// </summary>
        /// <param name="custId">The customer identifier.</param>
        /// <param name="transType">Type of the trans.</param>
        /// <param name="fromDate">From date.</param>
        /// <param name="quality">The quality.</param>
        /// <param name="totalComm">The total commission.</param>
        /// <param name="totalWinLoss">The total win loss.</param>
        /// <returns>The bet list statement.</returns>
        [SuppressMessage("StyleCop.CSharp.Nexcel.NexcelCustomRules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        public IList<TicketDetail> GetBetListStatement(
            int custId,
            int transType,
            DateTime fromDate,
            ref int quality,
            ref decimal totalComm,
            ref decimal totalWinLoss)
        {
            try
            {
                ITicketHelper ticketHelper = new TicketHelper();
                List<ITicketData> outputLiveCasino = new List<ITicketData>();
                IList<TicketDetail> betDetails = new List<TicketDetail>();
                IList<TicketDetail> betDetailsModel = new List<TicketDetail>();
                StringBuilder content = new StringBuilder();
                BetListHelper betListHelper = new BetListHelper();
                DataSet dataSet = new DataSet();

                var criteria = new BetListStatement
                {
                    CustId = custId,
                    TransType = transType,
                    FromDate = fromDate
                };

                using (var db = new ObjectDb(criteria.GetSettingKey()))
                {
                    dataSet = db.ExecuteDataSet(criteria);
                }

                if (dataSet != null &&
                    dataSet.Tables.Count >= 2)
                {
                    DataTable ticketDetailTable = dataSet.Tables[0];
                    DataTable ticketEventTable = dataSet.Tables[1];
                    foreach (DataRow r in ticketDetailTable.Rows)
                    {
                        decimal commTmp = Converter.ToDecimal(r["commission"]);
                        decimal winlostTmp = Converter.ToDecimal(r["winlost"]);
                        string status = Converter.ToString2(r["status"]).ToLower();

                        if (status != "running" &&
                            status != "waiting" &&
                            status != "void" &&
                            status != "reject")
                        {
                            totalComm += commTmp;
                            totalWinLoss += winlostTmp;
                        }

                        TicketDetail ticketDetailTmp = new TicketDetail
                        {
                            Ticket = new Ticket
                            {
                                ActualStake = Converter.To<int>(r["actual_stake"]),
                                AgentWinlost = Converter.ToDecimal(r["awinlost"]),
                                AgentPositionTaking = Converter.ToDecimal(r["apositiontaking"]),
                                Comm = Converter.ToDecimal(r["comm"]),
                                AgentDiscount = Converter.ToDecimal(r["discount"]),

                                AwayId = Converter.To<long>(r["awayid"]),
                                BetId = Converter.To<long>(r["betid"]),
                                BetTeam = Converter.ToString2(r["betteam"]),
                                BetTypeId = Converter.To<int>(r["bettype"]),
                                CommStatus = Converter.To<string>(r["comstatus"]),
                                CustId = custId,
                                Handicap1 = Converter.ToDecimal(r["hdp1"]),
                                Handicap2 = Converter.ToDecimal(r["hdp2"]),
                                HomeId = Converter.To<long>(r["homeid"]),
                                EventDate = Converter.ToDateTime(r["eventdate"]),
                                IP = Converter.ToString2(r["ip"]),
                                IsLive = Converter.ToBool(r["liveindicator"]),
                                IsNeutral = Converter.ToBool(r["isNeutral"]),
                                LeagueId = Converter.To<int>(r["leagueid"]),
                                LiveHomeScore = Converter.To<int>(r["livehomescore"]),
                                LiveAwayScore = Converter.To<int>(r["liveawayscore"]),

                                MasterWinlost = Converter.ToDecimal(r["mwinlost"]),
                                MasterPositionTaking = Converter.ToDecimal(r["mpositiontaking"]),
                                AgentComm = Converter.ToDecimal(r["acomm"]),
                                MasterDiscount = Converter.ToDecimal(r["adiscount"]),

                                MasterOdds = Converter.ToDecimal(r["modds"]),
                                MatchCode = Converter.ToString2(r["matchcode"]),
                                MatchId = Converter.To<int>(r["matchid"]),
                                Odds = Converter.ToDecimal(r["odds"]),
                                OddsType = Converter.ToString2(r["OddsType"]),
                                PlayerComm = Converter.ToDecimal(r["commission"]),
                                Race = Converter.ToString2(r["race"]),
                                RefNo = Converter.ToString2(r["refno"]),
                                ShowTime = Converter.ToString2(r["showtime"]),
                                SportTypeId = Converter.To<int>(r["sporttype"]),
                                Stake = Converter.To<decimal>(r["stake"]),
                                Status = Converter.ToString2(r["status"]),

                                SuperWinlost = Converter.ToDecimal(r["swinlost"]),
                                SuperPositionTaking = Converter.ToDecimal(r["spositiontaking"]),
                                SuperComm = Converter.ToDecimal(r["scomm"]),
                                SuperDiscount = Converter.ToDecimal(r["sdiscount"]),

                                TransDate = Converter.ToDateTime(r["transdate"]),
                                TransId = Converter.To<long>(r["transid"]),
                                UserName = Converter.ToString2(r["username"]),
                                Winlost = Converter.ToDecimal(r["winlost"]),
                                WinlostDate = Convert.ToDateTime(r["winlostdate"]),

                                BetCheck = Convert.ToString(r["betcheck"])
                            },
                            ChoiceHtml = string.Empty,
                            OddHtml = string.Empty,
                            StakeHtml = string.Empty,
                            IsShowOtExRate = Converter.To<bool>(r["isShowOTExRate"]),
                            VirtualRate = Converter.ToDecimal(r["virtualrate"]),
                            Creator = Converter.ToString2(r["creator"]),
                            FinalHomeScore = Converter.ToString2(r["finalhomescore"]),
                            FinalAwayScore = Converter.ToString2(r["finalawayscore"]),
                            HTHomeScore = Converter.ToString2(r["hthomescore"]),
                            HTAwayScore = Converter.ToString2(r["htawayscore"])
                        };

                        foreach (DataRow row in ticketEventTable.Rows)
                        {
                            if (Converter.ToString2(row["refno"]) == ticketDetailTmp.Ticket.RefNo)
                            {
                                ticketDetailTmp.Ticket.AgentPositionTaking = Converter.ToDecimal(row["apositiontaking"]);
                                ticketDetailTmp.Ticket.MasterPositionTaking = Converter.ToDecimal(row["mpositiontaking"]);
                                ticketDetailTmp.Ticket.SuperPositionTaking = Converter.ToDecimal(row["spositiontaking"]);
                                break;
                            }
                        }

                        betDetails.Add(ticketDetailTmp);
                    }

                    foreach (DataRow row in ticketEventTable.Rows)
                    {
                        outputLiveCasino.Add(new TicketData
                        {
                            RefNo = Converter.ToString2(row["refno"]),
                            BetTypeId = Converter.To<int>(row["bettype"]),
                            Status = Converter.ToString2(row["status"]),
                            Odds = Converter.To<decimal>(row["odds"]),
                            Stake = Converter.To<decimal>(row["stake"]),
                            BetTeam = Converter.ToString2(row["betteam"]),
                            TransDesc = Converter.ToString2(row["TransDesc"])
                        });
                    }
                }

                if (betDetails != null)
                {
                    SessionManager.Set<IList<TicketDetail>>(BetListConstants.BetListByStatementSessionKey, betDetails);
                    SessionManager.Set<List<ITicketData>>(BetListConstants.BetListByStatementOutputLiveCasinoSessionKey, outputLiveCasino);

                    BetListHTMLFactory factory = new BetListHTMLFactory();
                    BetListHTMLBuilder builder = factory.CreateBuilder();
                    GetCachePropertyById funcGetOddsTypeName = AccBetListCore.CacheRef.GetOddsTypeName;

                    foreach (TicketDetail betDetail in betDetails)
                    {
                        ticketHelper.IsShowOddsWithChoice = this.IsShowOddsWithChoice(betDetail.Ticket.BetTypeId);
                        betDetail.ChoiceHtml = builder.BuildChoice(betDetail.Ticket, ticketHelper, outputLiveCasino, false);
                        betDetail.StakeHtml = BetListExtension.BuildStake(betDetail, ref builder);
                        betDetail.OddHtml = builder.BuildOdds(betDetail.Ticket, outputLiveCasino, funcGetOddsTypeName);
                        betDetail.StatusHtml = betListHelper.StatusStatement(betDetail);
                    }

                    return betDetails;
                }
            }
            catch (Exception ex)
            {
                LoggingHandler.Log(ex);
            }

            return new List<TicketDetail>();
        }

        /// <summary>
        /// Gets the customer identifier by username.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <returns>The customer identifier by username.</returns>
        public int GetCustIDByUsername(string username = "")
        {
            int custid = 0;
            try
            {
                DataSet dataSet = new DataSet();
                var criteria = new CustIDByUsernameCriteria
                {
                    CustomerName = username
                };

                using (var db = new ObjectDb(criteria.GetSettingKey()))
                {
                    dataSet = db.ExecuteDataSet(criteria.GetSpParams());
                }

                if (dataSet != null &&
                    dataSet.Tables.Count > 0 &&
                    dataSet.Tables[0].Rows.Count > 0)
                {
                    custid = Converter.To<int>(dataSet.Tables[0].Rows[0]["custid"]);
                }
            }
            catch (Exception ex)
            {
                LoggingHandler.Log(ex);
            }

            return custid;
        }

        public CombinationParlay GetCombinationMP(ICriteria criteria)
        {
            return this._betListRepository.BetList_SelDetailCombinationMP(criteria);
        }

        /// <summary>
        /// Get Bet list Racing Tote Win Loss.
        /// </summary>
        /// <param name="criteria">The bet list win loss tote criteria.</param>
        /// <returns>The bet list racing tote win loss.</returns>
        [SuppressMessage("StyleCop.CSharp.Nexcel.NexcelCustomRules",
      "SP2101:MethodMustNotContainMoreLinesThan",
      Justification = "Reviewed. Suppression is OK here.")]
        public BetListRacingToteWinLoss GetBetListRacingToteWinLoss(BetListRacingWinLossToteCriteria criteria)
        {
            var betListRacing = new BetListRacingToteWinLoss
            {
                BetListDetail = new List<BetListRacingToteWinLoss>()
            };
            try
            {
                using (var db = new ObjectDb(criteria.GetSettingKey()))
                {
                    DataSet dataSet = db.ExecuteDataSet(criteria);
                    if (dataSet != null && dataSet.Tables.Count > 0)
                    {
                        decimal stakeTotal = 0,
                            memberWinLossTotal = 0,
                            playerCommTotal = 0,
                            agencyWinLossTotal = 0,
                            agencyCommIncomeTotal = 0,
                            ibcWinLossTotal = 0,
                            ibcCommIncomeTotal = 0,
                            dividendTotal = 0;
                        var dataTable = dataSet.Tables[0];
                        int no = 0;
                        foreach (DataRow row in dataTable.Rows)
                        {
                            var stake = Converter.ToDecimal(row["stake"]);
                            var memberWinLoss = Converter.ToDecimal(row["winlost"]);
                            var playerComm = Converter.ToDecimal(row["playercomm"]);
                            var agencyWinLoss = Converter.ToDecimal(row["WLPTAgency"]);
                            var agencyCommIncome = Converter.ToDecimal(row["commIncomeAgency"]);
                            var ibcWinLoss = Converter.ToDecimal(row["WLPTIBC"]);
                            var ibcCommIncome = Converter.ToDecimal(row["commIncomeIBC"]);
                            var dividend = Converter.ToDecimal(row["dividend"]);
                            var betList = new BetListRacingToteWinLoss()
                            {
                                No = ++no,
                                Username = Converter.ToString2(row["username"]),
                                TransId = Converter.ToString2(row["transid"]),
                                TransDate = Converter.ToString2(row["transdate"]),
                                BettypeId = Converter.ToInt(row["bettype"]),
                                WinOdds = Converter.ToDecimal(row["winOdds"]).ToString("0.#####"),
                                PlaceOdds = Converter.ToDecimal(row["placeOdds"]).ToString("0.#####"),
                                Odds = "<b>" + Converter.ToDecimal(row["odds"]).ToString("0.#####") + "</b>",
                                Stake = stake.ToString("#,##0"),
                                MemberWinLoss = memberWinLoss.Format(2),
                                PlayerComm = playerComm.Format(2),
                                AgencyCommIncome = agencyCommIncome.Format(2),
                                AgencyPTWinLoss = agencyWinLoss.Format(2),
                                IBCCommIncome = ibcCommIncome.Format(2),
                                IBCPTWinLoss = ibcWinLoss.Format(2),
                                Dividend = dividend.Format(2),
                                WinStatus = Converter.ToString2(row["winStatus"]).Format(2),
                                PlaceStatus = Converter.ToString2(row["placeStatus"]).Format(2),
                                Status = Converter.ToString2(row["status"]).Format(2),
                                HorseId = Converter.ToInt(row["HorseID"]),
                                HorseNo = Converter.ToInt(row["HorseNo"]),
                                VirtualRate = Converter.ToDecimal(row["virtualrate"]).ToString("0.#####"),
                                IsShowOTExRate = Converter.ToBool(row["isShowOTExRate"])
                            };
                            betList.Choice = betList.HorseNo + " - " + Fanex.BetList.Core.TicketHelper.CacheRef.GetHorseTeamName(betList.HorseId);
                            betList.BettypeName = Fanex.BetList.Core.TicketHelper.CacheRef.GetBettype(betList.BettypeId);
                            if (betList.IsShowOTExRate)
                            {
                                betList.Stake += "<br/>" + "<span class='extra'>" + betList.VirtualRate + "</span><br/>" + "<span class='extra'>(" + Labels.Ex_Rate_2 + ")</span>";
                            }

                            if (betList.BettypeId == 31 || betList.BettypeId == 32)
                            {
                                if (!betList.Status.ToLower().Equals(BetStatus.Won) && !betList.Status.ToLower().Equals(BetStatus.Refund))
                                {
                                    betList.Odds = "-";
                                }
                            }
                            else
                            {
                                betList.WinOdds = betList.WinStatus.ToLower().Equals(BetStatus.Won) ? betList.WinOdds : "0";
                                betList.PlaceOdds = betList.PlaceStatus.ToLower().Equals(BetStatus.Won) ||
                                betList.PlaceStatus.ToLower().Equals(BetStatus.Lose) ?
                                betList.PlaceOdds.Format(2) : "0.00";
                                betList.Odds = "<span>Win:<b>" + betList.WinOdds + "</b></span><br/><span>Place:<b>" + betList.PlaceOdds + "</b></span>";
                                betList.Status = "<span>Win:<b>" + betList.WinStatus + "</b></span></br><span>Place:<b>" + betList.PlaceStatus + "</b></span>";
                            }

                            stakeTotal += stake;
                            memberWinLossTotal += memberWinLoss;
                            playerCommTotal += playerComm;
                            agencyWinLossTotal += agencyWinLoss;
                            agencyCommIncomeTotal += agencyCommIncome;
                            ibcCommIncomeTotal += ibcCommIncome;
                            ibcWinLossTotal += ibcWinLoss;
                            dividendTotal += dividend;
                            betListRacing.BetListDetail.Add(betList);
                        }

                        betListRacing.Total = new BetListRacingToteWinLoss()
                        {
                            Stake = stakeTotal.ToString("#,##0"),
                            MemberWinLoss = memberWinLossTotal.Format(2),
                            PlayerComm = playerCommTotal.Format(2),
                            AgencyCommIncome = agencyCommIncomeTotal.Format(2),
                            AgencyPTWinLoss = agencyWinLossTotal.Format(2),
                            IBCCommIncome = ibcCommIncomeTotal.Format(2),
                            IBCPTWinLoss = ibcWinLossTotal.Format(2),
                            Dividend = dividendTotal.Format(2)
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                LoggingHandler.Log(ex);
            }

            return betListRacing;
        }

        /// <summary>
        /// Determines whether [is u3 special account] [the specified customer identifier].
        /// </summary>
        /// <param name="custId">The customer identifier.</param>
        /// <returns>Is U3 special customer or not.</returns>
        public bool IsU3SpecialCustomers(int custId)
        {
            string[] specialCustomersU3 = Convert.ToString(ConfigurationManager.AppSettings["U3SpecialCustomers"]).Split(',');
            string customerId = Convert.ToString(custId);

            for (int i = 0; i < specialCustomersU3.Length; i++)
            {
                if (customerId == specialCustomersU3[i].Trim())
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Check where bet type is sport book or not.
        /// </summary>
        /// <param name="betType">The type of bet.</param>
        /// <returns>Bet type is sport book or not.</returns>
        private bool IsSportBook(int betType)
        {
            return betType == BetTypeContants.Handicap ||
                betType == BetTypeContants.OddEven ||
                betType == BetTypeContants.OverUnder2 ||
                betType == BetTypeContants._1stHandicap ||
                betType == BetTypeContants._1stOverUnder ||
                betType == BetTypeContants._1stOddEven ||
                betType == BetTypeContants.MoneyLine;
        }

        private bool IsShowOddsWithChoice(int betTypeId)
        {
            return betTypeId == BetTypeContants.MixParlay ||
                betTypeId == BetTypeContants.SystemParlay;
        }
    }
}