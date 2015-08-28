// -----------------------------------------------------------------------
// <copyright file="BettypeReportService.cs"  company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Fanex.AccountingReports.Main.Services.Implement.DailyWinLoss2h
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using ExcelUtil.Spreadsheet;
    using Fanex.AccountingReports.Core.Extensions;
    using Fanex.AccountingReports.Core.Services.Impl;
    using Fanex.AccountingReports.Core.Web;
    using Fanex.AccountingReports.Main.Common;
    using Fanex.AccountingReports.Main.Criteria.DailyWinLoss2h;
    using Fanex.AccountingReports.Main.Models.DailyWinLoss2h;
    using Fanex.AccountingReports.Main.Resources;
    using Fanex.Common;
    using CoreConstants = Fanex.AccountingReports.Core.Constants;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public partial class DailyWinLoss2hService
    {
        /// <summary>
        /// Gets the type of the report by bet.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>The type of the report by bet.</returns>
        [SuppressMessage("StyleCop.CSharp.Nexcel.NexcelCustomRules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        public WinLossInfo GetReportByBetType(GetReportByBetTypeCriteria criteria)
        {
            try
            {
                WinLossInfo report = new WinLossInfo
                {
                    GrandTotal = new WinLossInfo
                    {
                        Name = criteria.BetTypeName + " " + Labels.Total
                    },
                    ChildsInfo = new List<WinLossInfo>(),
                    EightRMTotal = new WinLossInfo(),
                    BARMTotal = new WinLossInfo()
                };

                var rawData = this._reportRepository.GetReportByBetType(criteria);
                if (rawData != null)
                {
                    var rawReport = rawData[0] as List<dynamic>;
                    var leagueGroups = rawReport.GroupBy(data => data.leagueid);
                    var leagueIdList = rawReport.Select(data => Converter.ToInt(data.leagueid))
                                              .ToList();
                    CacheRef.SaveCacheRefResponse(leagueIdList, CacheRef.GetLeagueName, ref this._leagueNameCache);

                    var homeIdList = rawReport.Select(data => Converter.ToInt(data.HomeId))
                                            .ToList();
                    CacheRef.SaveCacheRefResponse(homeIdList, CacheRef.GetTeamName, ref this._teamNameCache);
                    var awayIdList = rawReport.Select(data => Converter.ToInt(data.AwayId))
                                            .ToList();
                    CacheRef.SaveCacheRefResponse(awayIdList, CacheRef.GetTeamName, ref this._teamNameCache);
                    foreach (var leagueGroup in leagueGroups)
                    {
                        var leagueWinLoss = new WinLossInfo
                        {
                            Id = Converter.ToInt(leagueGroup.Key),
                            ChildsInfo = new List<WinLossInfo>(),
                            GrandTotal = new WinLossInfo()
                        };

                        leagueWinLoss.Name = this._leagueNameCache[leagueWinLoss.Id];
                        foreach (var data in leagueGroup)
                        {
                            WinLossInfo info = new WinLossInfo
                            {
                                Id = Converter.ToInt(data.matchid),
                                MatchCode = Converter.ToString2(data.matchcode),
                                HomeTeamId = Converter.ToInt(data.HomeId),
                                AwayTeamId = Converter.ToInt(data.AwayId),
                                IsLive = Converter.ToBool(data.liveindicator),
                                WinLoss = Converter.ToDecimal(data.WinLost),
                                Position = Converter.ToDecimal(data.Position),
                                Commission = Converter.ToDecimal(data.Commission),
                                LineTotal = Converter.ToDecimal(data.RealLineTotal),
                                VirtualLineTotal = Converter.ToDecimal(data.VirtualLineTotal),
                                Turnover = Converter.ToDecimal(data.RealTurnover),
                                VirtualTurnover = Converter.ToDecimal(data.VirtualTurnover),
                                IsNeutral = Converter.ToBool(data.IsNeutral)
                            };

                            info.HomeTeamName = this._teamNameCache[info.HomeTeamId];
                            info.AwayTeamName = this._teamNameCache[info.AwayTeamId];
                            leagueWinLoss.GrandTotal.Add(info);
                            info = WinLossInfo.Format(info);
                            leagueWinLoss.ChildsInfo.Add(info);
                        }

                        report.GrandTotal.Add(leagueWinLoss.GrandTotal);
                        leagueWinLoss = WinLossInfo.Format_Total(leagueWinLoss);
                        report.ChildsInfo.Add(leagueWinLoss);
                    }

                    if (this._8rmConfig.IsActive)
                    {
                        var _8RMReport = rawData[1];
                        foreach (var data in _8RMReport)
                        {
                            WinLossInfo info = new WinLossInfo
                            {
                                WinLoss = Converter.ToDecimal(data.WinLost8RM),
                                Position = Converter.ToDecimal(data.Position),
                                Commission = Converter.ToDecimal(data.Commission),
                                LineTotal = Converter.ToDecimal(data.RealLineTotal),
                                VirtualLineTotal = Converter.ToDecimal(data.VirtualLineTotal),
                                Turnover = Converter.ToDecimal(data.RealTurnover),
                                VirtualTurnover = Converter.ToDecimal(data.VirtualTurnover)
                            };

                            report.EightRMTotal.Add(info);
                        }
                    }

                    if (this._barmConfig.IsActive)
                    {
                        var barmReport = rawData[2];
                        foreach (var data in barmReport)
                        {
                            WinLossInfo info = new WinLossInfo
                            {
                                WinLoss = Converter.ToDecimal(data.WinLostBARM),
                                IsLive = Converter.ToBool(data.liveindicator),
                                Position = Converter.ToDecimal(data.Position),
                                Commission = Converter.ToDecimal(data.Commission),
                                LineTotal = Converter.ToDecimal(data.RealLineTotal),
                                VirtualLineTotal = Converter.ToDecimal(data.VirtualLineTotal),
                                Turnover = Converter.ToDecimal(data.RealTurnover),
                                VirtualTurnover = Converter.ToDecimal(data.VirtualTurnover)
                            };

                            report.BARMTotal.Add(info);
                        }
                    }

                    report.ChildsInfo = report.ChildsInfo.OrderBy(sportGroup => sportGroup.Id).ToList();
                    report.ChildsInfo.Sort(delegate(WinLossInfo infoA, WinLossInfo infoB)
                    {
                        return this.SortWinLossInfoByLeague(infoA, infoB);
                    });

                    report = WinLossInfo.Format_Total(report);
                }

                SessionManager.Set<WinLossInfo>(MainConstants.DailyWinLoss2hReportByBetTypeSessionKey, report);
                return report;
            }
            catch (Exception ex)
            {
                HandleException(ex);
                SessionManager.Set<WinLossInfo>(MainConstants.DailyWinLoss2hReportByBetTypeSessionKey, null);
                throw;
            }
        }

        /// <summary>
        /// Exports the type of the report by bet.
        /// </summary>
        /// <param name="workSheet">The work sheet.</param>
        /// <param name="currentRow">The current row.</param>
        /// <param name="startRow">The start row.</param>
        /// <param name="startColumn">The start column.</param>
        /// <param name="isSportSummary">If set to <c>true</c> [is sport summary].</param>
        /// <param name="report">The report.</param>
        /// <param name="title">The title.</param>
        /// <param name="criteria">The criteria.</param>
        /// <param name="isShowEventDetail">If set to <c>true</c> [is show event detail].</param>
        [SuppressMessage("StyleCop.CSharp.Nexcel.NexcelCustomRules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        private void ExportReportByBetType(
            ref ExcelWorksheet workSheet,
            ref int currentRow,
            int startRow,
            int startColumn,
            bool isSportSummary,
            WinLossInfo report,
            string title,
            GetReportByBetTypeCriteria criteria,
            bool isShowEventDetail)
        {
            //// ExcelTypes

            var headerTextStyle = ExcelExtensions.GetCellTitleStyle(ExcelExtensions.HeaderTitleColor);
            var currentColumn = startColumn;
            CellRange currentCellRange;
            ExcelCell currentCell;
            var maxColumnIndex = 10; // 11 Columns
            if (isShowEventDetail)
            {
                maxColumnIndex++;
            }

            var backgroundColor = ExcelExtensions.BackGroundColor;
            var textStyle = ExcelExtensions.GetCellTextStyle(backgroundColor, false);
            var twoDecimalNumberStyle = ExcelExtensions.GetCellNumberStyle(CoreConstants.NumberFormatConstants.TwoDecimalDigitFormat, backgroundColor, false, false, false);
            var twoDecimalPositiveNumberStyle = ExcelExtensions.GetCellNumberStyle(
                CoreConstants.NumberFormatConstants.TwoDecimalDigitFormat,
                backgroundColor,
                false,
                true,
                false);
            var twoDecimalPositivePercentNumberStyle = ExcelExtensions.GetCellNumberStyle(
                CoreConstants.NumberFormatConstants.TwoDecimalDigitPercentFormat,
                backgroundColor,
                false,
                true,
                false);

            var grandTotalRowColor = ExcelExtensions.GrandTotalRowColor;
            var grandTotalRowTextStyle = ExcelExtensions.GetCellTextStyle(grandTotalRowColor, true);
            var grandTotalRowTwoDecimalNumberStyle = ExcelExtensions.GetCellNumberStyle(
                CoreConstants.NumberFormatConstants.TwoDecimalDigitFormat,
                grandTotalRowColor,
                true,
                false,
                false);
            var grandTotalTwoDecimalPositiveNumberStyle = ExcelExtensions.GetCellNumberStyle(
                CoreConstants.NumberFormatConstants.TwoDecimalDigitFormat,
                grandTotalRowColor,
                true,
                true,
                false);
            var grandTotalTwoDecimalPositivePercentNumberStyle = ExcelExtensions.GetCellNumberStyle(
                CoreConstants.NumberFormatConstants.TwoDecimalDigitPercentFormat,
                grandTotalRowColor,
                true,
                true,
                false);

            var grandTotalRowTextStyle_FirstRow = ExcelExtensions.GetCellTextStyle(ExcelExtensions.GrandTotalRowColor, true);
            grandTotalRowTextStyle_FirstRow.Borders.SetBorders(MultipleBorders.Top, ExcelExtensions.GrandTotalSeparatorColor, LineStyle.Medium);
            var grandTotalRowTwoDecimalNumberStyle_FirstRow = ExcelExtensions.GetCellNumberStyle(
                CoreConstants.NumberFormatConstants.TwoDecimalDigitFormat,
                ExcelExtensions.GrandTotalRowColor,
                true,
                false,
                false);
            grandTotalRowTwoDecimalNumberStyle_FirstRow.Borders.SetBorders(MultipleBorders.Top, ExcelExtensions.GrandTotalSeparatorColor, LineStyle.Medium);
            var grandTotalRowTwoDecimalPositiveNumberStyle_FirstRow = ExcelExtensions.GetCellNumberStyle(
                CoreConstants.NumberFormatConstants.TwoDecimalDigitFormat,
                ExcelExtensions.GrandTotalRowColor,
                true,
                true,
                false);
            grandTotalRowTwoDecimalPositiveNumberStyle_FirstRow.Borders.SetBorders(MultipleBorders.Top, ExcelExtensions.GrandTotalSeparatorColor, LineStyle.Medium);
            var gtrPercentNumberStyle_FirstRow = ExcelExtensions.GetCellNumberStyle(
                CoreConstants.NumberFormatConstants.TwoDecimalDigitPercentFormat,
                ExcelExtensions.GrandTotalRowColor,
                true,
                true,
                false);
            gtrPercentNumberStyle_FirstRow.Borders.SetBorders(MultipleBorders.Top, ExcelExtensions.GrandTotalSeparatorColor, LineStyle.Medium);

            //// ExcelTypes

            workSheet.Columns[startColumn].Width = 200 * ExcelExtensions.FontWidth;
            for (int colIndex = 1; colIndex <= maxColumnIndex; colIndex++)
            {
                workSheet.Columns[startColumn + colIndex].Width = 120 * ExcelExtensions.FontWidth;
            }

            if (isShowEventDetail)
            {
                workSheet.Columns[startColumn + 1].Width = 500 * ExcelExtensions.FontWidth;
            }

            //// Header

            currentColumn = startColumn;
            currentCellRange = workSheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow + 1, currentColumn + maxColumnIndex);
            currentCellRange.Merged = true;
            currentCellRange.Value = title;
            currentCellRange.Style = headerTextStyle;
            currentRow += 2;

            workSheet.ExportEmptyRow(startRow, startColumn, ref currentRow, startColumn + maxColumnIndex);

            // FirstRow
            currentColumn = startColumn;
            currentCellRange = workSheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow + 1, currentColumn);
            currentCellRange.Merged = true;
            currentCellRange.Value = Labels.Code;
            currentCellRange.Style = headerTextStyle;
            currentColumn++;

            if (isShowEventDetail)
            {
                currentCellRange = workSheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow + 1, currentColumn);
                currentCellRange.Merged = true;
                currentCellRange.Value = Labels.Event;
                currentCellRange.Style = headerTextStyle;
                currentColumn++;
            }

            currentCellRange = workSheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow + 1, currentColumn);
            currentCellRange.Merged = true;
            currentCellRange.Value = Labels.Member_Win_Loss;
            currentCellRange.Style = headerTextStyle;
            currentColumn++;

            currentCellRange = workSheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow + 1, currentColumn);
            currentCellRange.Merged = true;
            currentCellRange.Value = Labels.SMA_Win_Loss;
            currentCellRange.Style = headerTextStyle;
            currentColumn++;

            currentCellRange = workSheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow + 1, currentColumn);
            currentCellRange.Merged = true;
            currentCellRange.Value = Labels.Commission;
            currentCellRange.Style = headerTextStyle;
            currentColumn++;

            currentCellRange = workSheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow, currentColumn + 1);
            currentCellRange.Merged = true;
            currentCellRange.Value = Labels.Company_Income;
            currentCellRange.Style = headerTextStyle;
            currentColumn += 2;

            currentCellRange = workSheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow + 1, currentColumn);
            currentCellRange.Merged = true;
            currentCellRange.Value = Labels.Variance;
            currentCellRange.Style = headerTextStyle;
            currentColumn++;

            currentCellRange = workSheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow, currentColumn + 1);
            currentCellRange.Merged = true;
            currentCellRange.Value = Labels.Company_Turnover;
            currentCellRange.Style = headerTextStyle;
            currentColumn += 2;

            currentCellRange = workSheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow, currentColumn + 1);
            currentCellRange.Merged = true;
            currentCellRange.Value = Labels.Margin;
            currentCellRange.Style = headerTextStyle;
            currentColumn += 2;
            currentRow++;

            // SecondRow
            currentColumn = startColumn + 4 + (isShowEventDetail ? 1 : 0);
            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Style = headerTextStyle;
            currentCell.Value = Labels.Real;
            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Style = headerTextStyle;
            currentCell.Value = Labels.Virtual;
            currentColumn++;

            currentColumn++; // VarianceColumn

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Style = headerTextStyle;
            currentCell.Value = Labels.Real;
            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Style = headerTextStyle;
            currentCell.Value = Labels.Virtual;
            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Style = headerTextStyle;
            currentCell.Value = Labels.Real;
            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Style = headerTextStyle;
            currentCell.Value = Labels.Virtual;
            currentColumn++;
            currentRow++;

            //// Header
            //// Content

            if (report != null && report.ChildsInfo.Count > 0)
            {
                var leagueLength = report.ChildsInfo.Count;
                WinLossInfo info;

                for (var i = 0; i < leagueLength; i++)
                {
                    var league = report.ChildsInfo[i];

                    //// LeagueHeadLine

                    _dailyWinLoss2hUtilityService.SetOddEvenColor_ExportExcel(
                        ref textStyle,
                        ref twoDecimalNumberStyle,
                        ref twoDecimalPositiveNumberStyle,
                        ref twoDecimalPositivePercentNumberStyle,
                        true);

                    info = league;
                    currentColumn = startColumn;
                    currentCell = workSheet.Cells[currentRow, currentColumn];
                    currentCell.Style = textStyle;
                    currentColumn++;

                    currentCellRange = workSheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow, currentColumn + maxColumnIndex - 1);
                    currentCellRange.Merged = true;
                    currentCellRange.Value = info.Name;
                    currentCellRange.Style = textStyle;
                    currentCellRange.Style.Font.Weight = ExcelFont.BoldWeight;
                    currentRow++;

                    _dailyWinLoss2hUtilityService.SetOddEvenColor_ExportExcel(
                        ref textStyle,
                        ref twoDecimalNumberStyle,
                        ref twoDecimalPositiveNumberStyle,
                        ref twoDecimalPositivePercentNumberStyle,
                        false);

                    //// LeagueHeadLine

                    for (var j = 0; j < league.ChildsInfo.Count; j++)
                    {
                        //// NormalRow

                        info = league.ChildsInfo[j];
                        currentColumn = startColumn;
                        currentCell = workSheet.Cells[currentRow, currentColumn];
                        currentCell.Value = info.MatchCode;
                        currentCell.Style = textStyle;
                        currentCell.Style.HorizontalAlignment = HorizontalAlignmentStyle.Center;
                        currentColumn++;

                        if (isShowEventDetail)
                        {
                            currentCell = workSheet.Cells[currentRow, currentColumn];
                            currentCell.Value = info.HomeTeamName +
                                (info.IsNeutral ? " " + Labels.Symbol_NeutralSuffix : string.Empty) +
                               (string.IsNullOrEmpty(info.AwayTeamName) ?
                                string.Empty :
                                Labels.Vs + info.AwayTeamName);

                            currentCell.Style = textStyle;
                            currentCell.Style.HorizontalAlignment = HorizontalAlignmentStyle.Center;
                            currentColumn++;
                        }

                        currentCell = workSheet.Cells[currentRow, currentColumn];
                        currentCell.Value = info.WinLoss;
                        currentCell.Style = twoDecimalPositiveNumberStyle;
                        currentColumn++;

                        currentCell = workSheet.Cells[currentRow, currentColumn];
                        currentCell.Value = info.Position;
                        currentCell.Style = twoDecimalNumberStyle;
                        currentColumn++;

                        currentCell = workSheet.Cells[currentRow, currentColumn];
                        currentCell.Value = info.Commission;
                        currentCell.Style = twoDecimalNumberStyle;
                        currentColumn++;

                        currentCell = workSheet.Cells[currentRow, currentColumn];
                        currentCell.Value = info.LineTotal;
                        currentCell.Style = twoDecimalPositiveNumberStyle;
                        currentColumn++;

                        currentCell = workSheet.Cells[currentRow, currentColumn];
                        currentCell.Value = info.VirtualLineTotal;
                        currentCell.Style = twoDecimalPositiveNumberStyle;
                        currentColumn++;

                        currentCell = workSheet.Cells[currentRow, currentColumn];
                        currentCell.Value = info.Variance;
                        currentCell.Style = twoDecimalPositiveNumberStyle;
                        currentColumn++;

                        currentCell = workSheet.Cells[currentRow, currentColumn];
                        currentCell.Value = info.Turnover;
                        currentCell.Style = twoDecimalNumberStyle;
                        currentColumn++;

                        currentCell = workSheet.Cells[currentRow, currentColumn];
                        currentCell.Value = info.VirtualTurnover;
                        currentCell.Style = twoDecimalNumberStyle;
                        currentColumn++;

                        currentCell = workSheet.Cells[currentRow, currentColumn];
                        currentCell.Style = twoDecimalPositivePercentNumberStyle;
                        if (info.Turnover != 0)
                        {
                            currentCell.Value = info.Margin;
                        }
                        else
                        {
                            currentCell.Value = Labels.NotAvailable;
                        }

                        currentColumn++;

                        currentCell = workSheet.Cells[currentRow, currentColumn];
                        currentCell.Style = twoDecimalPositivePercentNumberStyle;
                        if (info.VirtualTurnover != 0)
                        {
                            currentCell.Value = info.VirtualMargin;
                        }
                        else
                        {
                            currentCell.Value = Labels.NotAvailable;
                        }

                        currentColumn++;
                        currentRow++;

                        //// NormalRow
                    }

                    _dailyWinLoss2hUtilityService.SetOddEvenColor_ExportExcel(
                        ref textStyle,
                        ref twoDecimalNumberStyle,
                        ref twoDecimalPositiveNumberStyle,
                        ref twoDecimalPositivePercentNumberStyle,
                        false);

                    if (league.ChildsInfo.Count >= 2)
                    {
                        //// LeagueTotal

                        info = league.GrandTotal;
                        currentColumn = startColumn;
                        currentCellRange = workSheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow, currentColumn + maxColumnIndex - 10);
                        currentCellRange.Merged = true;
                        currentCellRange.Style = textStyle;
                        currentColumn = currentColumn + 1 + maxColumnIndex - 10;

                        currentCell = workSheet.Cells[currentRow, currentColumn];
                        currentCell.Value = info.WinLoss;
                        currentCell.Style = twoDecimalPositiveNumberStyle;
                        currentColumn++;

                        currentCell = workSheet.Cells[currentRow, currentColumn];
                        currentCell.Value = info.Position;
                        currentCell.Style = twoDecimalNumberStyle;
                        currentColumn++;

                        currentCell = workSheet.Cells[currentRow, currentColumn];
                        currentCell.Value = info.Commission;
                        currentCell.Style = twoDecimalNumberStyle;
                        currentColumn++;

                        currentCell = workSheet.Cells[currentRow, currentColumn];
                        currentCell.Value = info.LineTotal;
                        currentCell.Style = twoDecimalPositiveNumberStyle;
                        currentColumn++;

                        currentCell = workSheet.Cells[currentRow, currentColumn];
                        currentCell.Value = info.VirtualLineTotal;
                        currentCell.Style = twoDecimalPositiveNumberStyle;
                        currentColumn++;

                        currentCell = workSheet.Cells[currentRow, currentColumn];
                        currentCell.Value = info.Variance;
                        currentCell.Style = twoDecimalPositiveNumberStyle;
                        currentColumn++;

                        currentCell = workSheet.Cells[currentRow, currentColumn];
                        currentCell.Value = info.Turnover;
                        currentCell.Style = twoDecimalNumberStyle;
                        currentColumn++;

                        currentCell = workSheet.Cells[currentRow, currentColumn];
                        currentCell.Value = info.VirtualTurnover;
                        currentCell.Style = twoDecimalNumberStyle;
                        currentColumn++;

                        currentCell = workSheet.Cells[currentRow, currentColumn];
                        currentCell.Style = twoDecimalPositivePercentNumberStyle;
                        if (info.Turnover != 0)
                        {
                            currentCell.Value = info.Margin;
                        }
                        else
                        {
                            currentCell.Value = Labels.NotAvailable;
                        }

                        currentColumn++;

                        currentCell = workSheet.Cells[currentRow, currentColumn];
                        currentCell.Style = twoDecimalPositivePercentNumberStyle;
                        if (info.VirtualTurnover != 0)
                        {
                            currentCell.Value = info.VirtualMargin;
                        }
                        else
                        {
                            currentCell.Value = Labels.NotAvailable;
                        }

                        currentColumn++;
                        currentRow++;

                        //// LeagueTotal
                    }
                }

                //// GrandTotal

                info = report.GrandTotal;
                currentColumn = startColumn;
                currentCellRange = workSheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow, currentColumn + maxColumnIndex - 10);
                currentCellRange.Merged = true;
                currentCellRange.Style = grandTotalRowTextStyle_FirstRow;
                currentCellRange.Style.HorizontalAlignment = HorizontalAlignmentStyle.Right;
                currentCellRange.Value = string.Format(
                    "{0}{1}{2} " + Labels.Total,
                    criteria.Liveindicator ? "!" + Labels.Live + " " : string.Empty,
                    criteria.BetTypeName,
                    string.Empty);
                currentColumn = currentColumn + 1 + maxColumnIndex - 10;

                currentCell = workSheet.Cells[currentRow, currentColumn];
                currentCell.Value = info.WinLoss;
                currentCell.Style = grandTotalRowTwoDecimalPositiveNumberStyle_FirstRow;
                currentColumn++;

                currentCell = workSheet.Cells[currentRow, currentColumn];
                currentCell.Value = info.Position;
                currentCell.Style = grandTotalRowTwoDecimalNumberStyle_FirstRow;
                currentColumn++;

                currentCell = workSheet.Cells[currentRow, currentColumn];
                currentCell.Value = info.Commission;
                currentCell.Style = grandTotalRowTwoDecimalNumberStyle_FirstRow;
                currentColumn++;

                currentCell = workSheet.Cells[currentRow, currentColumn];
                currentCell.Value = info.LineTotal;
                currentCell.Style = grandTotalRowTwoDecimalPositiveNumberStyle_FirstRow;
                currentColumn++;

                currentCell = workSheet.Cells[currentRow, currentColumn];
                currentCell.Value = info.VirtualLineTotal;
                currentCell.Style = grandTotalRowTwoDecimalPositiveNumberStyle_FirstRow;
                currentColumn++;

                currentCell = workSheet.Cells[currentRow, currentColumn];
                currentCell.Value = info.Variance;
                currentCell.Style = grandTotalRowTwoDecimalPositiveNumberStyle_FirstRow;
                currentColumn++;

                currentCell = workSheet.Cells[currentRow, currentColumn];
                currentCell.Value = info.Turnover;
                currentCell.Style = grandTotalRowTwoDecimalNumberStyle_FirstRow;
                currentCell.Style.Font.Weight = ExcelFont.BoldWeight;
                currentColumn++;

                currentCell = workSheet.Cells[currentRow, currentColumn];
                currentCell.Value = info.VirtualTurnover;
                currentCell.Style = grandTotalRowTwoDecimalNumberStyle_FirstRow;
                currentCell.Style.Font.Weight = ExcelFont.BoldWeight;
                currentColumn++;

                currentCell = workSheet.Cells[currentRow, currentColumn];
                currentCell.Style = gtrPercentNumberStyle_FirstRow;
                if (info.Turnover != 0)
                {
                    currentCell.Value = info.Margin;
                }
                else
                {
                    currentCell.Value = Labels.NotAvailable;
                }

                currentColumn++;

                currentCell = workSheet.Cells[currentRow, currentColumn];
                currentCell.Style = gtrPercentNumberStyle_FirstRow;
                if (info.VirtualTurnover != 0)
                {
                    currentCell.Value = info.VirtualMargin;
                }
                else
                {
                    currentCell.Value = Labels.NotAvailable;
                }

                currentColumn++;
                currentRow++;

                //// GrandTotal
                //// IBCTotal

                decimal allIBCTotal = 0;
                WinLossInfo winLossRM = new WinLossInfo();
                if (report.EightRMTotal != null)
                {
                    allIBCTotal += report.EightRMTotal.WinLoss;
                    winLossRM.Add(report.EightRMTotal);
                }

                if (report.BARMTotal != null)
                {
                    allIBCTotal += report.BARMTotal.WinLoss;
                    winLossRM.Add(report.BARMTotal);
                }

                if (allIBCTotal != 0)
                {
                    currentColumn = startColumn;
                    currentCellRange = workSheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow, currentColumn + maxColumnIndex - 10);
                    currentCellRange.Merged = true;
                    currentCellRange.Value = this.GetIBCTotalName();
                    currentCellRange.Style = grandTotalRowTextStyle;
                    currentCellRange.Style.HorizontalAlignment = HorizontalAlignmentStyle.Right;
                    currentColumn = currentColumn + maxColumnIndex - 10 + 1;

                    currentCell = workSheet.Cells[currentRow, currentColumn];
                    currentCell.Value = winLossRM.WinLoss;
                    currentCell.Style = grandTotalTwoDecimalPositiveNumberStyle;
                    currentCell.Style.Font.Weight = ExcelFont.BoldWeight;
                    currentColumn++;

                    currentCell = workSheet.Cells[currentRow, currentColumn];
                    currentCell.Value = winLossRM.Position;
                    currentCell.Style = grandTotalRowTwoDecimalNumberStyle;
                    currentCell.Style.Font.Weight = ExcelFont.BoldWeight;
                    currentColumn++;

                    currentCell = workSheet.Cells[currentRow, currentColumn];
                    currentCell.Value = winLossRM.Commission;
                    currentCell.Style = grandTotalRowTwoDecimalNumberStyle;
                    currentCell.Style.Font.Weight = ExcelFont.BoldWeight;
                    currentColumn++;

                    currentCell = workSheet.Cells[currentRow, currentColumn];
                    currentCell.Value = winLossRM.LineTotal;
                    currentCell.Style = grandTotalTwoDecimalPositiveNumberStyle;
                    currentCell.Style.Font.Weight = ExcelFont.BoldWeight;
                    currentColumn++;

                    currentCell = workSheet.Cells[currentRow, currentColumn];
                    currentCell.Value = winLossRM.VirtualLineTotal;
                    currentCell.Style = grandTotalTwoDecimalPositiveNumberStyle;
                    currentCell.Style.Font.Weight = ExcelFont.BoldWeight;
                    currentColumn++;

                    currentCell = workSheet.Cells[currentRow, currentColumn];
                    currentCell.Value = winLossRM.Variance;
                    currentCell.Style = grandTotalTwoDecimalPositiveNumberStyle;
                    currentCell.Style.Font.Weight = ExcelFont.BoldWeight;
                    currentColumn++;

                    currentCell = workSheet.Cells[currentRow, currentColumn];
                    currentCell.Value = winLossRM.Turnover;
                    currentCell.Style = grandTotalRowTwoDecimalNumberStyle;
                    currentCell.Style.Font.Weight = ExcelFont.BoldWeight;
                    currentColumn++;

                    currentCell = workSheet.Cells[currentRow, currentColumn];
                    currentCell.Value = winLossRM.VirtualTurnover;
                    currentCell.Style = grandTotalRowTwoDecimalNumberStyle;
                    currentCell.Style.Font.Weight = ExcelFont.BoldWeight;
                    currentColumn++;

                    currentCell = workSheet.Cells[currentRow, currentColumn];
                    currentCell.Value = Math.Round(ReportService.CalculateMargin(winLossRM.LineTotal, winLossRM.Turnover), 4);
                    currentCell.Style = grandTotalTwoDecimalPositivePercentNumberStyle;
                    currentCell.Style.Font.Weight = ExcelFont.BoldWeight;
                    currentColumn++;

                    currentCell = workSheet.Cells[currentRow, currentColumn];
                    currentCell.Value = Math.Round(ReportService.CalculateMargin(winLossRM.VirtualLineTotal, winLossRM.VirtualTurnover), 4);
                    currentCell.Style = grandTotalTwoDecimalPositivePercentNumberStyle;
                    currentCell.Style.Font.Weight = ExcelFont.BoldWeight;
                    currentRow++;
                }

                //// IBCTotal
            }
            else
            {
                workSheet.ExportEmptyReport(startRow, startColumn, ref currentRow, startColumn + maxColumnIndex);
            }

            //// Content
        }
    }
}