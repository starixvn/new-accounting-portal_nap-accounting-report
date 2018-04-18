namespace Fanex.AccountingReports.Special.Services.Implement
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using Checkbet;
    using Common;
    using Fanex.AccountingReports.Core.Extensions;
    using Fanex.AccountingReports.Core.Helper.SportHelper;
    using Fanex.AccountingReports.Core.Services.Impl;
    using Fanex.AccountingReports.Special.Criteria.CheckBet;
    using Fanex.AccountingReports.Special.Models;
    using Fanex.AccountingReports.Special.Services.Interface;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class CheckBetService : ServiceBase, ICheckBetService
    {
        /// <summary>
        /// Gets the checked bet.
        /// </summary>
        /// <param name="checkBetCriteria">The check bet criteria.</param>
        /// <returns>The checked bet.</returns>
        public CheckBet GetCheckedBet(CheckBetCriteria checkBetCriteria)
        {
            var result = new CheckBet();
            try
            {
                var rawData = GetReportData<dynamic>(checkBetCriteria);
                if (rawData != null)
                {
                    foreach (var row in rawData)
                    {
                        result.NormalTicket = (int)row.CheckNormalBet;
                        result.RejectTicket = (int)row.CheckRejectBet;
                    }
                }
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }

            return result;
        }

        /// <summary>
        /// Gets the normal changed ticket.
        /// </summary>
        /// <param name="checkBetCriteria">The check bet criteria.</param>
        /// <returns>The normal changed ticket.</returns>
        [SuppressMessage("StyleCop.CSharp.StylecopPlus.Rules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        public NormalTickets GetNormalChangedTicket(CheckBetCriteria checkBetCriteria)
        {
            var result = new NormalTickets();
            try
            {
                var rawData = GetReportData<dynamic>(checkBetCriteria);
                if (rawData != null)
                {
                    var normalTicketList = new List<NormalTicketDetail>();

                    foreach (var row in rawData)
                    {
                        var transid = (long)row.transid;
                        var transdate = (DateTime)row.transdate;
                        var currency = (string)row.CurrencyName;
                        var bettype = (int)row.bettype;
                        var creator = (string)row.Creator;
                        var username = (string)row.Username;
                        var oldStake = (decimal)row.OldStake;
                        var newStake = (decimal)row.NewStake;
                        var oldOdds = (decimal)row.OldOdds;
                        var newOdds = (decimal)row.NewOdds;
                        var oldBetteam = BetTeam.GetBetTeam((string)row.OldBetteam, bettype, "e");
                        var newBetteam = BetTeam.GetBetTeam((string)row.NewBetteam, bettype, "e");
                        var oldTotalPT = (decimal)row.OldPT;
                        var newTotalPT = (decimal)row.NewPT;
                        var oddstype = (row.oddstype != null) ? CalcOdds.GetOddsType(bettype, (int)row.oddstype) : 0;
                        var status = (string)row.status;
                        var home = CacheRef.GetTeamName(row.homeid);
                        var newmatchcode = (string)row.NewMatchCode;
                        var oldmatchcode = (string)row.OldMatchCode;
                        var hdp1 = (decimal)row.hdp1;
                        var newHdp2 = (decimal)row.NewHdp2;
                        var oldHdp2 = (decimal)row.OldHdp2;
                        var liveindicator = (bool)row.liveindicator;
                        string bettypeName = CacheRef.GetBettype(bettype);
                        string oldAway = string.Empty;
                        string newAway = string.Empty;
                        var oddsSpread = Converter.ToDecimal(row.OddsSpread);
                        if (bettype == 85 || bettype == 86)
                        {
                            bettypeName = liveindicator ? "Next " + bettypeName : "FT." + bettypeName;
                        }

                        if (bettype == 31 ||
                            bettype == 32 ||
                            bettype == 33 ||
                            bettype == 41 ||
                            bettype == 42 ||
                            bettype == 43 ||
                            bettype == 1231 ||
                            bettype == 1232 ||
                            bettype == 1233 ||
                            bettype == 1237 ||
                            bettype == 1238)
                        {
                            newAway = newmatchcode + " - " + CacheRef.GetHorseTeamName(row.NewAwayID);
                            oldAway = oldmatchcode + " - " + CacheRef.GetHorseTeamName(row.OldAwayID);
                        }
                        else
                        {
                            newAway = CacheRef.GetTeamName(row.NewAwayID);
                            oldAway = CacheRef.GetTeamName(row.OldAwayID);
                        }

                        newBetteam = this.GetBetTeam(home, newAway, bettype, newBetteam, hdp1, newHdp2, oddsSpread);
                        oldBetteam = this.GetBetTeam(home, oldAway, bettype, oldBetteam, hdp1, oldHdp2, oddsSpread);
                        if (bettype == 10)
                        {
                            newBetteam = !string.IsNullOrEmpty(newAway) ? newAway : home;
                            oldBetteam = !string.IsNullOrEmpty(oldAway) ? oldAway : home;
                        }

                        normalTicketList.Add(new NormalTicketDetail()
                        {
                            RefNo = transid.ToString(),
                            TransDate = transdate.ToLongFormatedDateWithSlash(),
                            CurrencyName = currency,
                            Bettype = bettypeName,
                            Creator = creator,
                            OldStake = oldStake.Format(2),
                            NewStake = newStake.Format(2),
                            OldOdds = CalcOdds.GetOdds(oldOdds, bettype, oddstype).Format(2),
                            NewOdds = CalcOdds.GetOdds(newOdds, bettype, oddstype).Format(2),
                            OldBetteam = oldBetteam,
                            NewBetteam = newBetteam,
                            OldTotalPT = oldTotalPT.Format(2),
                            NewTotalPT = newTotalPT.Format(2),
                            Status = status,
                            Username = username
                        });

                        if (oldStake != newStake)
                        {
                            result.TotalChangedStake++;
                        }

                        if (oldOdds != newOdds)
                        {
                            result.TotalChangedOdds++;
                        }

                        if (oldBetteam != newBetteam)
                        {
                            result.TotalChangedBetteam++;
                        }

                        if (oldTotalPT != newTotalPT)
                        {
                            result.TotalChangedPT++;
                        }
                    }

                    result.NormalTicketList = normalTicketList;
                }
            }
            catch (Exception ex)
            {
                HandleException(ex);
                throw;
            }

            return result;
        }

        /// <summary>
        /// Gets the reject changed ticket.
        /// </summary>
        /// <param name="checkBetCriteria">The check bet criteria.</param>
        /// <returns>The reject changed ticket.</returns>
        [SuppressMessage("StyleCop.CSharp.StylecopPlus.Rules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        public RejectTickets GetRejectChangedTicket(CheckBetCriteria checkBetCriteria)
        {
            var result = new RejectTickets();
            try
            {
                var rawData = GetReportData<dynamic>(checkBetCriteria);
                if (rawData != null)
                {
                    var rejectTicketList = new List<RejectTicketDetail>();
                    foreach (var row in rawData)
                    {
                        int oddStyle = 0;
                        if (row.oddstype == null)
                        {
                            oddStyle = 0;
                        }
                        else
                        {
                            oddStyle = (int)row.oddstype;
                        }

                        var transid = (long)row.transid;
                        var transdate = (DateTime)row.transdate;
                        var currency = (string)row.CurrencyName;
                        var bettype = (int)row.bettype;
                        var creator = (string)row.Creator;
                        var username = (string)row.Username;
                        var stake = (decimal)row.stake;
                        var odds = (decimal)row.odds;
                        var betteam = BetTeam.GetBetTeam((string)row.betteam, bettype, "e");
                        var totalPT = (decimal)row.tpositiontaking;
                        var newStatus = (string)row.NewStatus;
                        var oldStatus = (string)row.OldStatus;
                        var home = CacheRef.GetTeamName(row.homeid);
                        var away = CacheRef.GetTeamName(row.awayid);
                        var hdp1 = (decimal)row.hdp1;
                        var hdp2 = (decimal)row.hdp2;
                        var liveindicator = (bool)row.liveindicator;
                        string betypeName = CacheRef.GetBettype(bettype);
                        var oddsSpread = Converter.ToDecimal(row.OddsSpread);
                        if (bettype == 85 || bettype == 86)
                        {
                            betypeName = liveindicator ? "Next " + betypeName : "FT." + betypeName;
                        }

                        if (bettype == 31 ||
                            bettype == 32 ||
                            bettype == 33 ||
                            bettype == 41 ||
                            bettype == 42 ||
                            bettype == 43 ||
                            bettype == 1231 ||
                            bettype == 1232 ||
                            bettype == 1233 ||
                            bettype == 1237 ||
                            bettype == 1238)
                        {
                            away = (string)row.matchcode + " - " + CacheRef.GetHorseTeamName(row.awayid);
                        }

                        if (bettype == 10)
                        {
                            betteam = !string.IsNullOrEmpty(away) ? away : home;
                        }

                        if (oldStatus != newStatus)
                        {
                            result.TotalChangedStatus++;
                        }

                        rejectTicketList.Add(new RejectTicketDetail()
                        {
                            RefNo = transid.ToString(),
                            TransDate = transdate.ToLongFormatedDateWithSlash(),
                            Currency = currency,
                            Bettype = betypeName,
                            Username = username,
                            Betteam = this.GetBetTeam(home, away, bettype, betteam, hdp1, hdp2, oddsSpread),
                            Stake = stake.Format(2),
                            Odds = odds.Format(2),
                            TotalPT = totalPT.Format(2),
                            NewStatus = newStatus,
                            OldStatus = oldStatus,
                            Creator = creator
                        });
                    }

                    result.RejectTicketList = rejectTicketList;
                }
            }
            catch (Exception ex)
            {
                HandleException(ex);
                throw;
            }

            return result;
        }

        /// <summary>
        /// Gets the bet team.
        /// </summary>
        /// <param name="homeTeam">The home team.</param>
        /// <param name="awayTeam">The away team.</param>
        /// <param name="betType">Type of the bet.</param>
        /// <param name="betTeam">The bet team.</param>
        /// <param name="hdp1">The HDP1 value.</param>
        /// <returns>The bet team result.</returns>
        [SuppressMessage("StyleCop.CSharp.StylecopPlus.Rules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        private string GetBetTeam(
            string homeTeam,
            string awayTeam,
            int betType,
            string betTeam,
            decimal hdp1,
            decimal hdp2,
            decimal oddsSpread)
        {
            string result = string.Empty;
            var getBetTeamEnhanced = new ContentChoiceBuilder();
            switch (betType)
            {
                case 1:
                case 7:
                case 124:
                case 125:
                case 20:
                case 21:
                case 1201:
                    result = betTeam == "h" ? homeTeam : awayTeam;
                    break;

                case 2:
                case 12:
                    result = betTeam == "h" ? "ODD" : "EVEN";
                    break;

                case 3:
                case 8:
                case 1203:
                    result = betTeam == "h" ? "OVER" : "UNDER";
                    break;

                case 4:
                case 30:
                case 1204:
                    result = (betTeam == "5:0" || betTeam == "0:5") ? betTeam + " UP" : betTeam;
                    if (betType == 30)
                    {
                        result = (betTeam == "4:0" || betTeam == "0:4") ? betTeam + " UP" : betTeam;
                    }

                    break;

                case 5:
                case 1205:
                    result = "FT." + betTeam;
                    break;

                case 15:
                    result = "HT." + betTeam;
                    break;

                case 14:
                case 16:
                case 1206:
                    result = betTeam;
                    break;

                case 10:
                    result = betTeam;

                    break;

                case 121:
                    result = betTeam == "h" ? "Draw" : awayTeam;
                    break;

                case 122:
                    result = betTeam == "h" ? homeTeam : "Draw";
                    break;

                case 123:
                    result = betTeam == "h" ? "Draw" : "No Draw";
                    break;

                case 31:
                case 32:
                case 33:
                case 41:
                case 42:
                case 43:
                case 1231:
                case 1232:
                case 1233:
                case 1237:
                case 1238:
                    result = awayTeam;
                    break;

                case 81: ////Number Game
                case 82:
                case 83:
                case 84:
                case 85:
                case 86:
                case 87:
                case 88:
                case 89:
                case 90:
                    if (betType == 81 || betType == 82 || betType == 85)
                    {
                        result = betTeam.ToLower() == "h" ? " OVER " : " UNDER ";
                    }
                    else if (betType == 83 || betType == 84 || betType == 86)
                    {
                        result = betTeam.ToLower() == "h" ? "ODD " : "EVEN ";
                    }
                    else if (betType == 87) ////High-Low
                    {
                        result = betTeam.ToLower() == "h" ? "HIGH " : "LOW ";
                    }
                    else if (betType == 88) ////warrior
                    {
                        result = betTeam.ToLower() == "h" ? "2nd ball to win " : "3rd ball to win ";
                    }
                    else if (betType == 89) ////next combo
                    {
                        switch (betTeam.ToLower())
                        {
                            case "1:1":
                                result = "Over/Odd ";
                                break;

                            case "1:2":
                                result = "Over/Even ";
                                break;

                            case "2:1":
                                result = "Under/Odd ";
                                break;

                            case "2:2":
                                result = "Under/Even ";
                                break;

                            default:
                                result = "Over/Odd ";
                                break;
                        }
                    }
                    else if (betType == 90)
                    {
                        result = "Group " + hdp1.Format() + "-" + betTeam + " ";
                    }

                    break;

                case 25:
                    result = betTeam == "h" ? homeTeam : awayTeam;
                    break;

                case 26:
                    if (betTeam.ToLower().Equals("b"))
                    {
                        result = "Both";
                    }
                    else if (betTeam.ToLower().Equals("o"))
                    {
                        result = "One";
                    }
                    else if (betTeam.ToLower().Equals("n"))
                    {
                        result = "No Goal";
                    }

                    break;

                case 27:
                    if (betTeam.ToLower().Equals("h"))
                    {
                        result = homeTeam;
                    }
                    else if (betTeam.ToLower().Equals("a"))
                    {
                        result = awayTeam;
                    }

                    break;

                case 13:
                    if (betTeam.ToLower().Equals("hy"))
                    {
                        result = homeTeam + " YES";
                    }
                    else if (betTeam.ToLower().Equals("hn"))
                    {
                        result = homeTeam + " NO";
                    }
                    else if (betTeam.ToLower().Equals("ay"))
                    {
                        result = awayTeam + " YES";
                    }
                    else if (betTeam.ToLower().Equals("an"))
                    {
                        result = awayTeam + " NO";
                    }

                    break;

                case 28:
                    if (betTeam.ToLower().Equals("1"))
                    {
                        result = homeTeam;
                    }
                    else if (betTeam.ToLower().Equals("2"))
                    {
                        result = awayTeam;
                    }

                    break;

                case 9:
                case 2799:
                    result = "Mix Parlay";

                    break;

                case 29:
                    result = "System Parlay";

                    break;

                case 2703:
                case 2704:
                    if (betTeam.ToLower().Equals("o"))
                    {
                        result = "Over";
                    }
                    if (betTeam.ToLower().Equals("u"))
                    {
                        result = "Under";
                    }

                    break;

                case 2705:
                case 2706:
                    if (betTeam.ToLower().Equals("h"))
                    {
                        result = homeTeam;
                    }
                    if (betTeam.ToLower().Equals("a"))
                    {
                        result = awayTeam;
                    }

                    break;

                case 1004:
                    result = CacheRef.GetResourceData("BF", betTeam);
                    break;

                default:
                    result = getBetTeamEnhanced.GetContentBetchoiceToShowOf(homeTeam, awayTeam, betType, betTeam, hdp1, hdp2, oddsSpread);

                    break;
            }

            return result;
        }
    }
}
