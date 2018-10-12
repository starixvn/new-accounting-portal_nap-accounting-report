namespace Fanex.BetList.Accounting.Services
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using Fanex.BetList.Accounting.Constants;
    using Fanex.BetList.Accounting.Criteria;
    using Fanex.BetList.Accounting.Extensions;
    using Fanex.BetList.Accounting.Models;
    using Fanex.BetList.Accounting.Repositories;
    using Fanex.BetList.Core.Builder;
    using Fanex.BetList.Core.Entities;
    using Fanex.BetList.Core.Factories;
    using Fanex.BetList.Core.TicketHelper;
    using Fanex.Common;
    using Fanex.Data.Repository;
    using RemotingRef;
    using AccBetListCore = Fanex.BetList.Accounting.Core;

    public interface BetListService
    {
        IList<TicketDetail> GetBetListByRefNoTickets(BetListByRefNoCriteria criteria);
        CombinationParlay GetCombinationMP(ICriteria criteria);
        IList<TicketDetail> GetMixParlayTickets(MixParlayDetailCriteria criteria, string handicap1, string handicap2, string betcheck, string betteam);
    }

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    [System.Runtime.InteropServices.Guid("6DF0115C-60E5-4182-BEF3-AC61F09BAF0A")]
    public class BetListServiceImp : BetListService
    {
        public const string BetListSessionKey = "Fanex.BetList.Accounting.WinLossByLicensee";
        private readonly BetListRepository _betListRepository;

        public BetListServiceImp()
        {
            _betListRepository = new BetListRepositoryImp();
        }

        /// <summary>
        /// Get Detail Mix Parlay ticket.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>Detail Mix Parlay ticket.</returns>
        [SuppressMessage("StyleCop.CSharp.StylecopPlus.Rules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        public IList<TicketDetail> GetMixParlayTickets(MixParlayDetailCriteria criteria, string handicap1, string handicap2, string betcheck, string betteam)
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
                    BetCheck = Converter.ToString2(r["BetCheck"]),
                    Stake = Converter.ToDecimal(r["Stake"]),
                    Currency = Converter.ToString2(r["Currency"]),
                    TransDesc = Converter.ToString2(r["TransDesc"])
                },
                ChoiceHtml = string.Empty,
                OddHtml = string.Empty,
                StakeHtml = string.Empty
            };

            ITicketHelper ticketHelper = new TicketHelper();
            IList<TicketDetail> betDetails = _betListRepository.Fetch<TicketDetail>(criteria, mapTicketObjectFunction);
            string comboParlayDetail = string.Empty;

            if (!string.IsNullOrEmpty(handicap1))
            {
                int hdp1 = Converter.ToInt(Math.Round(Convert.ToDouble(handicap1)));
                int hdp2 = Converter.ToInt(Math.Round(Convert.ToDouble(handicap2)));
                string[] comboParlay = new RemotingRefService().GetCombinationTypeCombo(betteam, betcheck, hdp1, hdp2, "en");
                comboParlayDetail = GetComboParlayDetail(hdp1, comboParlay, betDetails);
            }

            if (betDetails != null)
            {
                BetListHTMLFactory factory = new BetListHTMLFactory();
                BetListHTMLBuilder builder = factory.CreateBuilder();

                ticketHelper.IsShowOddsWithChoice = true;
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

                if (!string.IsNullOrEmpty(comboParlayDetail))
                {
                    betDetails[0].ChoiceHtml = comboParlayDetail + betDetails[0].ChoiceHtml;
                }

                return betDetails;
            }


            return new List<TicketDetail>();
        }

        /// <summary>
        /// Get Bet List by ref no.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>Bet List by ref no.</returns>
        [SuppressMessage("StyleCop.CSharp.StylecopPlus.Rules",
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
                    TransDesc = Converter.To<string>(r["TransDesc"]),

                    BetCheck = Convert.ToString(r["betcheck"]),
                    BetdaqId = Converter.To<long>(r["betdaqId"]),
                    ExtraInfo = Converter.ToString2(r["ExtraInfo"]),
                    Currency = Converter.ToString2(r["currency"]),
                    WalletType = Converter.To<byte>(r["WalletType"]),
                    RefCode = Converter.To<long>(r["RefCode"]),
                    OddsSpread = Converter.To<decimal>(r["Oddsspread"])
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

            ITicketHelper ticketHelper = new TicketHelper();
            List<ITicketData> outputLiveCasino = new List<ITicketData>();
            IList<TicketDetail> betDetails = _betListRepository.Fetch<TicketDetail>(criteria, mapTicketObjectFunction, ref outputLiveCasino);
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
                    betDetail.TransDateHtml = builder.BuildTrans(betDetail.Ticket);
                    betDetail.Ticket.Status = builder.BuildStatus(betDetail.Ticket, ticketHelper, outputLiveCasino, true);
                }

                return betDetails;
            }

            return new List<TicketDetail>();
        }

        public CombinationParlay GetCombinationMP(ICriteria criteria)
        {
            return this._betListRepository.BetList_SelDetailCombinationMP(criteria);
        }

        private bool IsShowOddsWithChoice(int betTypeId)
        {
            return betTypeId == BetTypeContants.MixParlay ||
                betTypeId == BetTypeContants.SystemParlay;
        }

        private string GetComboParlayDetail(int handicap1, string[] comboParlay, IList<TicketDetail> ticketDetails)
        {
            string comboParlayDetail = string.Empty;

            if (handicap1 == 0)
            {
                comboParlayDetail = GetOneComboParlayDetail(comboParlay, ticketDetails);
            }
            else
            {
                comboParlayDetail = GetMultipleComboParlayDetail(comboParlay, ticketDetails);
            }

            return comboParlayDetail;
        }

        private string GetOneComboParlayDetail(string[] comboParlay, IList<TicketDetail> ticketDetails)
        {
            string firstComboParlay = comboParlay.FirstOrDefault();
            var firstTicketDetail = ticketDetails.Select(x => x.Ticket).Where(y => y.Stake != 0).FirstOrDefault();
            var stakeInInteger = Convert.ToInt32(firstTicketDetail.Stake);
            return $"<div class='comboParlay'>{firstComboParlay} x" +
                $"<span class='currency'>{firstTicketDetail.Currency} </span>" +
                $"<span class='stake'>{stakeInInteger:#,##0}</span></div>";
        }

        private string GetMultipleComboParlayDetail(string[] comboParlay, IList<TicketDetail> ticketDetails)
        {
            string comboParlayDetail = string.Empty;
            var ticketDetailWithNoneZeroStakes = ticketDetails.Select(t => t.Ticket).Where(s => s.Stake != 0).OrderBy(m => m.MatchId).ToList();
            for (int i = 0; i < comboParlay.Length; i++)
            {
                var stakeInInteger = Convert.ToInt32(ticketDetailWithNoneZeroStakes[i].Stake);
                comboParlayDetail += $"<div class='comboParlay'>{comboParlay[i]} x " +
                    $"<span class='currency'>{ticketDetailWithNoneZeroStakes[i].Currency}</span>" +
                    $"<span class='stake'>{stakeInInteger:#,##0}</span><br/></div>";
            }

            return comboParlayDetail;
        }
    }
}
