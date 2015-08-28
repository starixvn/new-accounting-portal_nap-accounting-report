// --------------------------------------------------------------------------
// <copyright file="BetListExtension.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Ricky.Nguyen" date="2012.12.11">Created</change>
// </history>

namespace Fanex.BetList.Accounting.Extensions
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using Fanex.BetList.Accounting.Constants;
    using Fanex.BetList.Accounting.Core;
    using Fanex.BetList.Accounting.Models;
    using Fanex.BetList.Accounting.Resources;
    using Fanex.BetList.Core.Builder;
    using Fanex.BetList.Core.Entities;
    using NPOI.HSSF.UserModel;
    using NPOI.SS.UserModel;
    using NPOI.SS.Util;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public static class BetListExtension
    {
        public static BetListHTMLBuilder AddLicenseeWinLoss(this BetListHTMLBuilder factory, int index, string openTag, string closeTag)
        {
            factory.Append(string.Join(null, new string[] { openTag, index.ToString(), closeTag }));
            return factory;
        }

        /// <summary>
        /// Builds the stake.
        /// </summary>
        /// <param name="betDetail">The bet detail.</param>
        /// <param name="builder">The builder.</param>
        /// <returns>The stake.</returns>
        public static string BuildStake(this TicketDetail betDetail, ref BetListHTMLBuilder builder)
        {
            string stakeBuilder = string.Empty;
            stakeBuilder = builder.BuildStake(betDetail.Ticket);
            if (betDetail.IsShowOtExRate)
            {
                stakeBuilder +=
                    "<br />" +
                    "<span class=\"r exrate2\">" + betDetail.VirtualRate + "</span>" +
                    "<br />" +
                    "<span class=\"r exrate2\">(" + Labels.Ex_Rate_2 + ")</span>";
            }

            return stakeBuilder;
        }

        public static BetListExcelBuilder AddCreator(this BetListExcelBuilder factory, TicketDetail ticketDetail, ITicketHelper ticketHelper)
        {
            IRichTextString rtfCreator = new HSSFRichTextString(ticketDetail.Creator + "\n" + ticketDetail.Ticket.IP);
            factory.AddCell(rtfCreator, ticketHelper.Index % 2 == 0 ? factory.OddCellStyleCenterAligned : factory.EvenCellStyleCenterAligned);

            return factory;
        }

        /// <summary>
        /// Adds the status statement.
        /// </summary>
        /// <param name="factory">The factory.</param>
        /// <param name="ticketDetail">The ticket detail.</param>
        /// <param name="ticketHelper">The ticket helper.</param>
        /// <returns>The status statement.</returns>
        [SuppressMessage("StyleCop.CSharp.Nexcel.NexcelCustomRules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        public static BetListExcelBuilder AddStatusStatement(
            this BetListExcelBuilder factory,
            TicketDetail ticketDetail,
            ITicketHelper ticketHelper)
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
                status.ToUpper() :
                status;
            bool isWonStatus = string.Equals(status, "won", StringComparison.OrdinalIgnoreCase);
            bool isLoseStatus = string.Equals(status, "lose", StringComparison.OrdinalIgnoreCase);
            var betlistHelper = new BetListHelper();
            if (sportTypeId <= 42 ||
                sportTypeId == SportContants.OtherSports)
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
                                "\n" +
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
                string winStatus = GetScoreStatusName(ticketDetail.Ticket.LiveHomeScore);
                string winStatusHtml = Labels.Win + ": " + winStatus + "\n";
                string placeStatus = GetScoreStatusName(ticketDetail.Ticket.LiveAwayScore);
                string placeStatusHtml = Labels.Place + ": " + placeStatus;
                statusHtml = winStatusHtml + placeStatusHtml;
            }
            else
            {
                statusHtml = status;
            }

            IRichTextString rtfCreator = new HSSFRichTextString(statusHtml);
            factory.AddCell(rtfCreator, ticketHelper.Index % 2 == 0 ? factory.OddCellStyleCenterAligned : factory.EvenCellStyleCenterAligned);
            return factory;
        }

        /// <summary>
        /// Gets the name of the score status.
        /// </summary>
        /// <param name="score">The score.</param>
        /// <returns>The name of the score status.</returns>
        public static string GetScoreStatusName(int score)
        {
            switch (score)
            {
                case ScoreStatusConstants.Won:
                    return Labels.Won.ToUpper();
                case ScoreStatusConstants.Lose:
                    return Labels.Lose.ToUpper();
                case ScoreStatusConstants.Refund:
                    return Labels.Refund.ToUpper();
            }

            return string.Empty;
        }

        /// <summary>
        /// Adds the header statement.
        /// </summary>
        /// <param name="factory">The factory.</param>
        /// <param name="isShowWinLost">If set to <c>true</c> [is show win lost].</param>
        /// <param name="isShowSuperPT">If set to <c>true</c> [is show super position taking].</param>
        /// <param name="isShowMasterPT">If set to <c>true</c> [is show master position taking].</param>
        /// <param name="isShowAgentPT">If set to <c>true</c> [is show agent position taking].</param>
        /// <param name="isShowMember">If set to <c>true</c> [is show member].</param>
        /// <returns>The header statement.</returns>
        [SuppressMessage("StyleCop.CSharp.Nexcel.NexcelCustomRules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        public static BetListExcelBuilder AddHeaderStatement(
            this BetListExcelBuilder factory,
            bool isShowWinLost,
            bool isShowSuperPT,
            bool isShowMasterPT,
            bool isShowAgentPT,
            bool isShowMember)
        {
            ICellStyle headerStyle = factory.CreateCellStyle();
            headerStyle.CloneStyleFrom(factory.EvenCellStyle);
            headerStyle.FillForegroundColor = 44;
            headerStyle.Alignment = HorizontalAlignment.Center;
            headerStyle.VerticalAlignment = VerticalAlignment.Center; 
            headerStyle.SetFont(factory.RTFHelper.PosFont);
            
            CellAddress cellAddr;
            cellAddr = factory.AddCell(new HSSFRichTextString(Labels.No), headerStyle);
            factory.SetColumnWidth(cellAddr.ColumnIndex, 1200);

            cellAddr = factory.AddCell(new HSSFRichTextString(Labels.Trans_Time), headerStyle);
            factory.SetColumnWidth(cellAddr.ColumnIndex, 5400);

            cellAddr = factory.AddCell(new HSSFRichTextString(Labels.Choice), headerStyle);
            factory.SetColumnWidth(cellAddr.ColumnIndex, 15500);

            cellAddr = factory.AddCell(new HSSFRichTextString(Labels.Odds), headerStyle);
            factory.SetColumnWidth(cellAddr.ColumnIndex, 2400);

            cellAddr = factory.AddCell(new HSSFRichTextString(Labels.Stake), headerStyle);
            factory.SetColumnWidth(cellAddr.ColumnIndex, 2400);

            if (isShowWinLost)
            {
                cellAddr = factory.AddCell(new HSSFRichTextString(Labels.Win_Loss), headerStyle);
                factory.SetColumnWidth(cellAddr.ColumnIndex, 3400);
            }

            cellAddr = factory.AddCell(new HSSFRichTextString(Labels.Status), headerStyle);
            factory.SetColumnWidth(cellAddr.ColumnIndex, 3400);

            if (isShowSuperPT)
            {
                cellAddr = factory.AddCell(new HSSFRichTextString(Labels.Super + " " + Labels.PT + " / " + Labels.WinLoss), headerStyle);
                factory.SetColumnWidth(cellAddr.ColumnIndex, 3400);
            }

            if (isShowMasterPT)
            {
                cellAddr = factory.AddCell(new HSSFRichTextString(Labels.Master + " " + Labels.PT + " /  " + Labels.WinLoss), headerStyle);
                factory.SetColumnWidth(cellAddr.ColumnIndex, 3400);
            }

            if (isShowAgentPT)
            {
                cellAddr = factory.AddCell(new HSSFRichTextString(Labels.Agent + " " + Labels.PT + " /  " + Labels.WinLoss), headerStyle);
                factory.SetColumnWidth(cellAddr.ColumnIndex, 3400);
            }

            if (isShowMember)
            {
                cellAddr = factory.AddCell(new HSSFRichTextString(Labels.Operator), headerStyle);
                factory.SetColumnWidth(cellAddr.ColumnIndex, 4800);
            }

            return factory;
        }

        /// <summary>
        /// Add footer for excel file.
        /// </summary>
        /// <param name="factory">Factory object Excel builder.</param>
        /// <param name="betlistDetail">Object bet list details.</param>
        /// <returns>Return object Bet list excel builder.</returns>
        public static BetListExcelBuilder AddFooter(
            this BetListExcelBuilder factory,
            BetListDetail betlistDetail)
        {
            ICellStyle style = factory.CreateCellStyle();
            style.CloneStyleFrom(factory.OddCellStyle);
            style.FillForegroundColor = (short)47;
            style.Alignment = HorizontalAlignment.Right;
            style.VerticalAlignment = VerticalAlignment.Center;
            style.BorderTop = BorderStyle.Thin;
            style.BorderLeft = BorderStyle.Thin;
            style.BorderBottom = BorderStyle.Thin;
            style.SetFont(factory.RTFHelper.PosFont);

            factory.AddCell(new HSSFRichTextString("Total"), style);
            factory.AddCell(new HSSFRichTextString("Total"), style);
            factory.AddCell(new HSSFRichTextString("Total"), style);
            factory.AddCell(new HSSFRichTextString("Total"), style);
            factory.AddCell(new HSSFRichTextString("Total"), style);

            CellRangeAddress cellRange = new CellRangeAddress(factory.LastContentRow, factory.LastContentRow, 0, 4);

            factory.AddCell(new HSSFRichTextString("\n" + betlistDetail.TotalWinLoss.Format(2) + "\n"), style);
            factory.AddCell(new HSSFRichTextString("\n" + betlistDetail.TotalLicenseeWinLoss.Format(2) + "\n"), style);
            factory.AddCell(new HSSFRichTextString(string.Empty), style);

            factory.MergeCell(cellRange, style);

            return factory;
        }

        /// <summary>
        /// Set value for cell in excel file.
        /// </summary>
        /// <param name="factory">Factory object Excel builder.</param>
        /// <param name="value">Value need to set at cell.</param>
        /// <param name="alignment">Set alignment style.</param>
        /// <param name="font">Set font style.</param>
        /// <param name="ticketHelper">Ticket helper.</param>
        /// <returns>Return object Bet list excel builder.</returns>
        public static BetListExcelBuilder AddValue(
            this BetListExcelBuilder factory,
            object value,
            HorizontalAlignment alignment,
            IFont font,
            ITicketHelper ticketHelper)
        {
            ICellStyle style = factory.CreateCellStyle();
            style.CloneStyleFrom(ticketHelper.Index % 2 == 0 ? factory.OddCellStyleCenterAligned : factory.EvenCellStyleCenterAligned);
            style.Alignment = HorizontalAlignment.Right;
            style.VerticalAlignment = VerticalAlignment.Center;
            style.BorderTop = BorderStyle.Thin;
            style.BorderLeft = BorderStyle.Thin;
            style.BorderBottom = BorderStyle.Thin; 
            style.SetFont(font);

            string valueHtml = value.Format(2) + "\n";
            IRichTextString rtfCreator = new HSSFRichTextString(valueHtml);
            factory.AddCell(rtfCreator, style);

            return factory;
        }
    }
}