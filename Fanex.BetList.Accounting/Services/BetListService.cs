namespace Fanex.BetList.Accounting.Services
{
    using System;
    using System.Collections.Generic;
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
    using Fanex.Data.Repository;
    using AccBetListCore = Fanex.BetList.Accounting.Core;

    public interface BetListService
    {
        IList<TicketDetail> GetBetListByRefNoTickets(BetListByRefNoCriteria criteria);
        BetListRacingToteWinLoss GetBetListRacingToteWinLoss(BetListRacingWinLossToteCriteria criteria);
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
            StringBuilder content = new StringBuilder();
            DataTable commandData = new DataTable();
            string comboParlayDetail = string.Empty;

            if (!string.IsNullOrEmpty(handicap1))
            {
                int hdp1 = Converter.ToInt(Math.Round(Convert.ToDouble(handicap1)));
                int hdp2 = Converter.ToInt(Math.Round(Convert.ToDouble(handicap2)));
                string[] comboParlay = RemotingRef.RefData.GetCombinationTypeCombo(betteam, betcheck, hdp1, hdp2, "en");
                comboParlayDetail = GetComboParlayDetail(hdp1, comboParlay, betDetails);
            }

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

        /// <summary>
        /// Get Bet list Racing Tote Win Loss.
        /// </summary>
        /// <param name="criteria">The bet list win loss tote criteria.</param>
        /// <returns>The bet list racing tote win loss.</returns>
        [SuppressMessage("StyleCop.CSharp.StylecopPlus.Rules",
      "SP2101:MethodMustNotContainMoreLinesThan",
      Justification = "Reviewed. Suppression is OK here.")]
        public BetListRacingToteWinLoss GetBetListRacingToteWinLoss(BetListRacingWinLossToteCriteria criteria)
        {
            var betListRacing = new BetListRacingToteWinLoss
            {
                BetListDetail = new List<BetListRacingToteWinLoss>()
            };

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


            return betListRacing;
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
