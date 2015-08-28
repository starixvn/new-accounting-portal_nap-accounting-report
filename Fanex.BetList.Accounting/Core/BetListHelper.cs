// --------------------------------------------------------------------------
// <copyright file="BetListHelper.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="" date="2012.11.22">Created</change>
// </history>

namespace Fanex.BetList.Accounting.Core
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Text.RegularExpressions;
    using Fanex.BetList.Accounting.Constants;
    using Fanex.BetList.Accounting.Extensions;
    using Fanex.BetList.Accounting.Models;
    using Fanex.BetList.Accounting.Resources;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class BetListHelper
    {
        public static readonly Regex IsNumericRegex = new Regex("^(" +
            /*Hex*/ @"0x[0-9a-f]+" + "|" +
            /*Bin*/ @"0b[01]+" + "|" +
            /*Oct*/ @"0[0-7]+" + "|" +
            /*Dec*/ @"((?!0)|[-+])(\d*\.)?\d+(e\d+)?" +
                                                    ")$");

        public static bool IsNumeric(string value)
        {
            return IsNumericRegex.IsMatch(value);
        }

        public string IsNegative(decimal input)
        {
            return input < 0 ? "negative" : "positive";
        }

        public bool IsHRWinPlaceBettype(int betType)
        {
            return betType == BetTypeContants.WinPlace ||
                betType == BetTypeContants.WinPlaceUKTote ||
                betType == BetTypeContants.VirtualSportWinPlace;
        }

        public bool IsHRFixOdds(int betType)
        {
            return betType == BetTypeContants.WinUKTote ||
              betType == BetTypeContants.PlaceUKTote ||
              betType == BetTypeContants.WinPlaceUKTote;
        }

        public bool IsHRBettype(int betType)
        {
            return betType == BetTypeContants.Win ||
                betType == BetTypeContants.Place ||
                betType == BetTypeContants.WinPlace ||
                betType == BetTypeContants.WinUKTote ||
                betType == BetTypeContants.PlaceUKTote ||
                betType == BetTypeContants.WinPlaceUKTote;
        }

        public bool IsVirtualHRBettype(int betType)
        {
            return betType == BetTypeContants.VirtualSportWin ||
                betType == BetTypeContants.VirtualSportPlace ||
                betType == BetTypeContants.VirtualSportWinPlace;
        }

        public string PercentFormat(decimal input)
        {
            var isInputInterger = (input * 100) % 1 == 0;
            return isInputInterger ?
                Math.Round(input * 100, 0).ToString() + Labels.Symbol_Percent :
                Math.Round(input * 100, 2).ToString() + Labels.Symbol_Percent;
        }

        /// <summary>
        /// Gets the status reference no.
        /// </summary>
        /// <param name="ticketDetail">The ticket detail.</param>
        /// <returns>The reference no.</returns>
        [SuppressMessage("StyleCop.CSharp.Nexcel.NexcelCustomRules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        public string StatusRefNo(TicketDetail ticketDetail)
        {
            string status = ticketDetail.Ticket.Status.ToLower();
            int _liveAwayScore = ticketDetail.Ticket.LiveAwayScore;
            int _liveHomeScore = ticketDetail.Ticket.LiveHomeScore;
            string statusLabel = string.Empty;
            BetListHelper betlistHelper = new BetListHelper();
            bool isWonStatus = string.Equals(status, "won", StringComparison.OrdinalIgnoreCase);
            bool isLoseStatus = string.Equals(status, "lose", StringComparison.OrdinalIgnoreCase);
            bool isDrawStatus = string.Equals(status, "draw", StringComparison.OrdinalIgnoreCase);
            
            if (betlistHelper.IsHRWinPlaceBettype(ticketDetail.Ticket.BetTypeId) &&
                (isWonStatus || isLoseStatus))
            {
                var _homeTemp = BetListExtension.GetScoreStatusName(_liveHomeScore);
                var _placeTemp = BetListExtension.GetScoreStatusName(_liveAwayScore);
                statusLabel =
                    "<div>" +
                        "<span style=\"color: #2673CE;\">" + Labels.Win + ": </span>" +
                        "<span class=\"positive\">" + _homeTemp + "</span>" +
                    "</div>" +
                    "<div class=\"margin-top-5\">" +
                        "<span>" + Labels.Place + ": </span>" +
                        "<span>" + _placeTemp + "</span>" +
                     "</div>";
            }
            else
            {
                statusLabel = "<div class=\"bl_status c\">" + status.ToUpper() + "</div>";
            }
            
            string resultLink = string.Empty;
            if (betlistHelper.IsHRBettype(ticketDetail.Ticket.BetTypeId))
            {
                if (isWonStatus || isLoseStatus)
                {
                    int horseRacingBettype = 0;
                    int height = 200;
                    if (betlistHelper.IsHRFixOdds(ticketDetail.Ticket.BetTypeId))
                    {
                        height = 250;
                        horseRacingBettype = BetTypeContants.Handicap;
                    }

                    string hrefRaceResult = "Report/HRRaceResult.aspx?" +
                        "leagueid=" + ticketDetail.Ticket.LeagueId +
                        "&raceno=" + ticketDetail.Ticket.Race +
                        "&eventdate=" + ticketDetail.Ticket.EventDate.ToShortDateString() +
                        "&HRBettype=" + horseRacingBettype.ToString();

                    resultLink = "openHRresult('" + hrefRaceResult + "','Race_Result',300," + height + ")";
                }
            }
            else if (betlistHelper.IsVirtualHRBettype(ticketDetail.Ticket.BetTypeId))
            {
                if (isWonStatus || isLoseStatus)
                {
                    resultLink = "virtualHRPopup(" + ticketDetail.Ticket.SportTypeId + "," +
                     ticketDetail.Ticket.MatchId + "," +
                     ticketDetail.Ticket.Race + ")";
                }
            }
            else
            {
                if (isWonStatus ||
                    isLoseStatus ||
                    isDrawStatus)
                {
                    int sportTypeId = ticketDetail.Ticket.SportTypeId;
                    switch (ticketDetail.Ticket.SportTypeId)
                    {
                        case SportContants.Politics:
                            resultLink = string.Empty;
                            break;
                        case SportContants.NumberGame:
                            resultLink = "ViewResultNumberGame('Betlist/BetListNumberGameResult.aspx?" +
                                "matchId=" + ticketDetail.Ticket.MatchId.ToString() +
                                "')";
                            break;
                        case SportContants.LiveCasino:
                            resultLink = "ViewResultLiveCasino('LiveCasino/Betlist/BetListLiveCasinoResult.aspx?" +
                                "matchId=" + ticketDetail.Ticket.MatchId.ToString() +
                                "&bettype=" + ticketDetail.Ticket.BetTypeId +
                                "&league=" + CacheRef.GetLeagueName(ticketDetail.Ticket.LeagueId) +
                                "')";
                            break;
                        case SportContants.VirtualTennis:
                            resultLink = "virtualHRPopup(" + ticketDetail.Ticket.SportTypeId + "," +
                                ticketDetail.Ticket.MatchId + ")";
                            break;
                        case SportContants.Tennis:
                            resultLink = "ViewTennisPopup(" + ticketDetail.Ticket.MatchId + "," +
                                ticketDetail.Ticket.BetTypeId + "," +
                                ticketDetail.Ticket.BetId + ",'" +
                                CacheRef.GetLeagueName(ticketDetail.Ticket.LeagueId) + "')";
                            break;
                        default:
                            switch (ticketDetail.Ticket.BetTypeId)
                            {
                                case BetTypeContants.SystemParlay:
                                    resultLink = "ViewCombinationMixParlayResult(" + ticketDetail.Ticket.RefNo + "," +
                                        ticketDetail.Ticket.MatchId.ToString() + "," +
                                        ticketDetail.Ticket.BetTypeId + "," +
                                        (ticketDetail.Ticket.BetId == null ?
                                        0 :
                                        ticketDetail.Ticket.BetId) + ")";
                                    break;

                                case BetTypeContants.MixParlay:
                                    resultLink = "ViewMixParlayResult(" + ticketDetail.Ticket.RefNo + "," +
                                        ticketDetail.Ticket.MatchId.ToString() + "," +
                                        ticketDetail.Ticket.BetTypeId + "," +
                                        ticketDetail.Ticket.BetId + ")";
                                    break;

                                case BetTypeContants.Outright:
                                case BetTypeContants.CasinoGames:
                                case BetTypeContants.P2P:
                                case BetTypeContants.Bingo:
                                case BetTypeContants.P2PGames:
                                    resultLink = string.Empty;
                                    break;
                                case BetTypeContants.GoldDeluxeBaccarat:
                                case BetTypeContants.GoldDeluxeParlayBaccarat:
                                case BetTypeContants.GoldDeluxeRoulette:
                                    resultLink = "ViewGoldDeluxeResult(" + ticketDetail.Ticket.TransId + ", " + ticketDetail.Ticket.BetTypeId.ToString() + ")";
                                    break;
                                default:
                                    resultLink = "ViewResult(" + ticketDetail.Ticket.MatchId.ToString() + "," +
                                        ticketDetail.Ticket.BetTypeId + "," +
                                        ticketDetail.Ticket.BetId + ")";
                                    break;
                            }

                            break;
                    }
                }
                else
                {
                    resultLink = string.Empty;
                }
            }

            var resultLink1 = ((string.IsNullOrEmpty(resultLink) == true) || (ticketDetail.Ticket.SportTypeId == 202)) ?
                string.Empty :
                "<br />" +
                "<a id=\"result_" + ticketDetail.Ticket.TransId + "\" onclick=\"" + resultLink + "\">" + Labels.Result + "</a>" +
                "<br />";
            return statusLabel + resultLink1;
        }

        /// <summary>
        /// Gets the status statement.
        /// </summary>
        /// <param name="ticketDetail">The ticket detail.</param>
        /// <returns>The status statement.</returns>
        [SuppressMessage("StyleCop.CSharp.Nexcel.NexcelCustomRules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        public string StatusStatement(TicketDetail ticketDetail)
        {
            string statusHtml = string.Empty;
            int betTypeId = ticketDetail.Ticket.BetTypeId;
            int sportTypeId = ticketDetail.Ticket.SportTypeId;
            string status = ticketDetail.Ticket.Status;
            string finalHomeScore = ticketDetail.FinalHomeScore;
            string finalAwayScore = ticketDetail.FinalAwayScore;
            string halfTimeHomeScore = ticketDetail.HTHomeScore;
            string halfTimeAwayScore = ticketDetail.HTAwayScore;
            string statusFormat = string.Equals(status, "reject", StringComparison.OrdinalIgnoreCase) ||
              string.Equals(status, "refund", StringComparison.OrdinalIgnoreCase) ||
              string.Equals(status, "void", StringComparison.OrdinalIgnoreCase) ?
              "<span style=\"color:red;\">" + status.ToUpper() + "</span>" :
              status;
            bool isWonStatus = string.Equals(status, "won", StringComparison.OrdinalIgnoreCase);
            bool isLoseStatus = string.Equals(status, "lose", StringComparison.OrdinalIgnoreCase);
            bool isDrawStatus = string.Equals(status, "draw", StringComparison.OrdinalIgnoreCase);
            var betlistHelper = new BetListHelper();
            
            if (sportTypeId <= 42 ||
                sportTypeId == SportContants.OtherSports ||
                sportTypeId == SportContants.ESport)
            {
                switch (betTypeId)
                {
                    case BetTypeContants.CasinoGames:
                    case BetTypeContants.RNGCasinoBonus:
                    case BetTypeContants.Bingo:
                    case 113:
                    case 114:
                    case 112:
                    case BetTypeContants.MixParlay:
                    case BetTypeContants.P2PGames:
                    case BetTypeContants.Outright:
                        statusHtml = statusFormat;
                        break;
                    case BetTypeContants.SystemParlay:
                        statusHtml = status;
                        break;
                    default:
                        if (33 <= sportTypeId && sportTypeId <= 42)
                        {
                            statusHtml = string.Format(
                                "{0}@{1}-{2}",
                                statusFormat,
                                finalHomeScore,
                                finalAwayScore);
                        }
                        else
                        {
                            statusHtml =
                                string.Format("{0}@{1}-{2}",
                                                statusFormat,
                                                finalHomeScore,
                                                finalAwayScore) +
                                "<br />" +
                                string.Format("[{0}:{1}-{2}]",
                                                Labels.HT,
                                                halfTimeHomeScore,
                                                halfTimeAwayScore);
                        }

                        break;
                }
            }
            else if (betlistHelper.IsHRWinPlaceBettype(betTypeId) &&
                (isWonStatus || isLoseStatus))
            {
                string winStatus = BetListExtension.GetScoreStatusName(ticketDetail.Ticket.LiveHomeScore);
                string placeStatus = BetListExtension.GetScoreStatusName(ticketDetail.Ticket.LiveAwayScore);
                string winStatusHtml =
                    "<div>" +
                        "<span style='color: #2673CE;'>" + Labels.Win + ": </span>" +
                        "<span class='positive'>" + winStatus + "</span>" +
                    "</div>";
                string placeStatusHtml =
                    "<div class='margin-top-5'>" +
                        "<span>" + Labels.Place + ": </span>" +
                        "<span>" + placeStatus + "</span>" +
                    "<div>";
                statusHtml = winStatusHtml + placeStatusHtml;
            }
            else
            {
                switch (betTypeId)
                {
                    case 1201:
                    case 1203:
                    case 1204:
                    case 1205:
                    case 1206:
                    case 1220:
                    case 1224:
                    case 1235:
                    case 1236:
                        statusHtml = string.Format(
                                        "{0}@{1}-{2}",
                                        statusFormat,
                                        finalHomeScore,
                                        finalAwayScore) +
                                    "<br />" +
                                    string.Format(
                                        "[{0}:{1}-{2}]",
                                        Labels.HT,
                                        halfTimeHomeScore,
                                        halfTimeAwayScore);
                        break;
                    default:
                        statusHtml = status;
                        break;
                }
            }
            
            string resultLink = string.Empty;
            if (betlistHelper.IsHRBettype(ticketDetail.Ticket.BetTypeId))
            {
                if (isWonStatus || isLoseStatus)
                {
                    int horseRacingBetType = 0;
                    int height = 200;
                    if (betlistHelper.IsHRFixOdds(ticketDetail.Ticket.BetTypeId))
                    {
                        height = 250;
                        horseRacingBetType = BetTypeContants.Handicap;
                    }

                    string hrefRaceResult = "Report/HRRaceResult.aspx?" +
                        "leagueid=" + ticketDetail.Ticket.LeagueId +
                        "&raceno=" + ticketDetail.Ticket.Race +
                        "&eventdate=" + ticketDetail.Ticket.EventDate.ToShortDateString() +
                        "&HRBettype=" + horseRacingBetType.ToString();

                    resultLink = "openHRresult('" + hrefRaceResult + "','Race_Result',300," + height + ")";
                }
            }
            else if (betlistHelper.IsVirtualHRBettype(ticketDetail.Ticket.BetTypeId))
            {
                if (isWonStatus || isLoseStatus)
                {
                    resultLink = "virtualHRPopup(" + ticketDetail.Ticket.SportTypeId + "," +
                        ticketDetail.Ticket.MatchId + "," +
                        ticketDetail.Ticket.Race + ")";
                }
            }
            else
            {
                if (isWonStatus ||
                   isLoseStatus ||
                   isDrawStatus)
                {
                    switch (ticketDetail.Ticket.SportTypeId)
                    {
                        case SportContants.NumberGame:
                            resultLink = "ViewResultNumberGame('Betlist/BetListNumberGameResult.aspx?" +
                                "matchId=" + ticketDetail.Ticket.MatchId.ToString() + "')";
                            break;
                        case SportContants.LiveCasino:
                            resultLink = "ViewResultLiveCasino('LiveCasino/Betlist/BetListLiveCasinoResult.aspx?" +
                                 "matchId=" + ticketDetail.Ticket.MatchId.ToString() +
                                 "&bettype=" + ticketDetail.Ticket.BetTypeId +
                                 "&league=" + CacheRef.GetLeagueName(ticketDetail.Ticket.LeagueId) +
                                 "')";
                            break;
                        case SportContants.Politics:
                        case SportContants.VirtualTennis:
                            break;
                        case SportContants.GoldDeluxe:
                            resultLink = "\"ViewGoldDeluxeResult(" + ticketDetail.Ticket.TransId + ", " + ticketDetail.Ticket.BetTypeId.ToString() + ")\"";
                            break;
                        default:
                            switch (ticketDetail.Ticket.BetTypeId)
                            {
                                case BetTypeContants.SystemParlay:
                                    resultLink = "ViewCombinationMixParlayResult(" + ticketDetail.Ticket.RefNo + "," +
                                        ticketDetail.Ticket.MatchId.ToString() + "," +
                                        ticketDetail.Ticket.BetTypeId + "," +
                                        ticketDetail.Ticket.BetId + ")";
                                    break;
                                case BetTypeContants.MixParlay:
                                    resultLink = "ViewMixParlayResult(" + ticketDetail.Ticket.RefNo + "," +
                                        ticketDetail.Ticket.MatchId.ToString() + "," +
                                        ticketDetail.Ticket.BetTypeId + "," +
                                        ticketDetail.Ticket.BetId + ")";
                                    break;
                                default:
                                    resultLink = string.Empty;
                                    break;
                            }

                            break;
                    }
                }
                else
                {
                    resultLink = string.Empty;
                }
            }

            var resultLink1 = ((string.IsNullOrEmpty(resultLink) == true) || (ticketDetail.Ticket.SportTypeId == 202)) ?
                string.Empty :
                "<br />" +
                "<a id=\"result_" + ticketDetail.Ticket.TransId + "\" onclick=" + resultLink + ">" + Labels.Result + "</a>" +
                "<br />";
            return statusHtml + resultLink1;
        }
    }
}
