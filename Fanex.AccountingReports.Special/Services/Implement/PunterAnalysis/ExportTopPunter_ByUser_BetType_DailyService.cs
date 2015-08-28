// --------------------------------------------------------------------------
// <copyright file="ExportTopPunter_ByUser_BetType_DailyService.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Louis.Nguyen" date="2012.12.07">Created</change>
// </history>

namespace Fanex.AccountingReports.Special.Services.Implement.PunterAnalysis
{
    using System.Diagnostics.CodeAnalysis;
    using ExcelUtil.Spreadsheet;
    using Fanex.AccountingReports.Core.Constants;
    using Fanex.AccountingReports.Core.Extensions;
    using Fanex.AccountingReports.Special.Models.PunterAnalysis;
    using Fanex.AccountingReports.Special.Resources;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public partial class PunterAnalysisService
    {
        /// <summary>
        /// Exports the top punter by user bet type daily.
        /// </summary>
        /// <param name="report">The report.</param>
        /// <param name="workSheet">The work sheet.</param>
        /// <param name="startRow">The start row.</param>
        /// <param name="startColumn">The start column.</param>
        /// <param name="currentRow">The current row.</param>
        [SuppressMessage("StyleCop.CSharp.Nexcel.NexcelCustomRules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        private void ExportTopPunter_ByUser_BetType_Daily(
            PunterWinLossInfo report,
            ref ExcelWorksheet workSheet,
            int startRow,
            int startColumn,
            ref int currentRow)
        {
            var headerTextStyle = ExcelExtensions.GetCellTitleStyle(ExcelExtensions.HeaderTitleColor);
            var currentColumn = startColumn;
            CellRange currentCellRange;
            ExcelCell currentCell;

            // gtr: grandTotalRow
            // 2d: TwoDecimal
            var textStyle = ExcelExtensions.GetCellTextStyle(ExcelExtensions.BackGroundColor, false);
            var intNumberStyle = ExcelExtensions.GetCellNumberStyle(NumberFormatConstants.IntNumberFormat, ExcelExtensions.BackGroundColor, false, false, false);
            var _2dNumberStyle = ExcelExtensions.GetCellNumberStyle(NumberFormatConstants.TwoDecimalDigitFormat, ExcelExtensions.BackGroundColor, false, false, false);
            var _2dNegativeNumberStyle = ExcelExtensions.GetCellNumberStyle(
                NumberFormatConstants.TwoDecimalDigitFormat,
                ExcelExtensions.BackGroundColor,
                false,
                false,
                true);
            _2dNegativeNumberStyle.NumberFormat = string.Format(
                    NegativeNumberFormat,
                    NumberFormatConstants.TwoDecimalDigitFormat,
                    NumberFormatConstants.TwoDecimalDigitFormat);
            var _2dNegativePercentNumberStyle = ExcelExtensions.GetCellNumberStyle(
                NumberFormatConstants.TwoDecimalDigitPercentFormat,
                ExcelExtensions.BackGroundColor,
                false,
                false,
                true);
            _2dNegativePercentNumberStyle.NumberFormat = string.Format(
                    NegativeNumberFormat,
                    NumberFormatConstants.TwoDecimalDigitPercentFormat,
                    NumberFormatConstants.TwoDecimalDigitPercentFormat);

            var gtrTextStyle_FirstRow = ExcelExtensions.GetCellTextStyle(ExcelExtensions.GrandTotalRowColor, true);
            gtrTextStyle_FirstRow.Borders.SetBorders(MultipleBorders.Top, ExcelExtensions.GrandTotalSeparatorColor, LineStyle.Medium);
            var gtr2dNumberStyle_FirstRow = ExcelExtensions.GetCellNumberStyle(
                NumberFormatConstants.TwoDecimalDigitFormat,
                ExcelExtensions.GrandTotalRowColor,
                true,
                false,
                false);
            gtr2dNumberStyle_FirstRow.Borders.SetBorders(MultipleBorders.Top, ExcelExtensions.GrandTotalSeparatorColor, LineStyle.Medium);
            var gtr2dNegativeNumberStyle_FirstRow = ExcelExtensions.GetCellNumberStyle(
                NumberFormatConstants.TwoDecimalDigitFormat,
                ExcelExtensions.GrandTotalRowColor,
                true,
                false,
                true);
            gtr2dNegativeNumberStyle_FirstRow.NumberFormat = string.Format(
                    NegativeNumberFormat,
                    NumberFormatConstants.TwoDecimalDigitFormat,
                    NumberFormatConstants.TwoDecimalDigitFormat);
            gtr2dNegativeNumberStyle_FirstRow.Borders.SetBorders(MultipleBorders.Top, ExcelExtensions.GrandTotalSeparatorColor, LineStyle.Medium);
            var gtr2dNegativePercentNumberStyle_FirstRow = ExcelExtensions.GetCellNumberStyle(
                NumberFormatConstants.TwoDecimalDigitPercentFormat,
                ExcelExtensions.GrandTotalRowColor,
                true,
                false,
                true);
            gtr2dNegativePercentNumberStyle_FirstRow.NumberFormat = string.Format(
                    NegativeNumberFormat,
                    NumberFormatConstants.TwoDecimalDigitPercentFormat,
                    NumberFormatConstants.TwoDecimalDigitPercentFormat);
            gtr2dNegativePercentNumberStyle_FirstRow.Borders.SetBorders(MultipleBorders.Top, ExcelExtensions.GrandTotalSeparatorColor, LineStyle.Medium);
            var gtrIntNumberStyle_FirstRow = ExcelExtensions.GetCellNumberStyle(
                NumberFormatConstants.IntNumberFormat,
                ExcelExtensions.GrandTotalRowColor,
                true,
                false,
                false);
            gtrIntNumberStyle_FirstRow.Borders.SetBorders(MultipleBorders.Top, ExcelExtensions.GrandTotalSeparatorColor, LineStyle.Medium);

            if (report != null)
            {
                //// Header

                currentColumn = startColumn;
                currentCellRange = workSheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow + 1, currentColumn);
                currentCellRange.Merged = true;
                currentCellRange.Value = Labels.League;
                currentCellRange.Style = headerTextStyle;
                workSheet.Columns[currentColumn].Width = 300 * ExcelExtensions.FontWidth;
                currentColumn++;

                var grandTotal = report.GrandTotal;
                var yearLength = grandTotal.ChildsInfo.Count;
                for (var i = 0; i < yearLength; i++)
                {
                    var year = grandTotal.ChildsInfo[i];
                    currentCellRange = workSheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow, currentColumn + 3);
                    currentCellRange.Merged = true;
                    currentCellRange.Value = year.Name;
                    currentCellRange.Style = headerTextStyle;

                    this.ExportTopPunter_BetType_Daily_PunterInfo(
                        ref workSheet,
                        currentRow + 1,
                        ref currentColumn,
                        headerTextStyle,
                        headerTextStyle,
                        headerTextStyle,
                        headerTextStyle,
                        Labels.Win_Loss,
                        Labels.Turnover,
                        Labels.Margin,
                        Labels.Bet_Count,
                        true);

                    var monthLength = year.ChildsInfo.Count;
                    for (var j = 0; j < monthLength; j++)
                    {
                        var month = year.ChildsInfo[j];
                        currentCellRange = workSheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow, currentColumn + 3);
                        currentCellRange.Merged = true;
                        currentCellRange.Value = month.Name;
                        currentCellRange.Style = headerTextStyle;

                        this.ExportTopPunter_BetType_Daily_PunterInfo(
                            ref workSheet,
                            currentRow + 1,
                            ref currentColumn,
                            headerTextStyle,
                            headerTextStyle,
                            headerTextStyle,
                            headerTextStyle,
                            Labels.Win_Loss,
                            Labels.Turnover,
                            Labels.Margin,
                            Labels.Bet_Count,
                            true);
                    }
                }

                currentCellRange = workSheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow, currentColumn + 3);
                currentCellRange.Merged = true;
                currentCellRange.Value = grandTotal.Name;
                currentCellRange.Style = headerTextStyle;
                this.ExportTopPunter_BetType_Daily_PunterInfo(
                    ref workSheet,
                    currentRow + 1,
                    ref currentColumn,
                    headerTextStyle,
                    headerTextStyle,
                    headerTextStyle,
                    headerTextStyle,
                    Labels.Win_Loss,
                    Labels.Turnover,
                    Labels.Margin,
                    Labels.Bet_Count,
                    true);
                currentRow += 2;

                //// Header

                var leagueLength = report.ChildsInfo.Count;
                for (var i = 0; i < leagueLength; i++)
                {
                    //// LeagueInfo

                    var league = report.ChildsInfo[i];
                    yearLength = league.ChildsInfo.Count;

                    currentColumn = startColumn;
                    currentCell = workSheet.Cells[currentRow, currentColumn];
                    currentCell.Value = league.Name;
                    currentCell.Style = textStyle;
                    currentColumn++;

                    for (var j = 0; j < yearLength; j++)
                    {
                        var year = league.ChildsInfo[j];
                        if (year != null)
                        {
                            var yearGrandTotal = year.GrandTotal;
                            var monthLength = year.ChildsInfo.Count;
                            this.ExportTopPunter_BetType_Daily_PunterInfo(
                                ref workSheet,
                                currentRow,
                                ref currentColumn,
                                _2dNegativeNumberStyle,
                                _2dNumberStyle,
                                _2dNegativePercentNumberStyle,
                                intNumberStyle,
                                yearGrandTotal.WinLoss,
                                yearGrandTotal.Turnover,
                                yearGrandTotal.Margin,
                                yearGrandTotal.BetCount,
                                false);

                            for (var k = 0; k < monthLength; k++)
                            {
                                //// MonthInfo

                                var month = year.ChildsInfo[k];
                                if (month != null)
                                {
                                    this.ExportTopPunter_BetType_Daily_PunterInfo(
                                        ref workSheet,
                                        currentRow,
                                        ref currentColumn,
                                        _2dNegativeNumberStyle,
                                        _2dNumberStyle,
                                        _2dNegativePercentNumberStyle,
                                        intNumberStyle,
                                        month.WinLoss,
                                        month.Turnover,
                                        month.Margin,
                                        month.BetCount,
                                        false);
                                }
                                else
                                {
                                    this.ExportTopPunter_BetType_Daily_PunterInfo(
                                        ref workSheet,
                                        currentRow,
                                        ref currentColumn,
                                        _2dNegativeNumberStyle,
                                        _2dNumberStyle,
                                        _2dNegativePercentNumberStyle,
                                        intNumberStyle,
                                        string.Empty,
                                        string.Empty,
                                        string.Empty,
                                        string.Empty,
                                        false);
                                }

                                //// MonthInfo
                            }
                        }
                        else
                        {
                            //// NullYearInfo

                            this.ExportTopPunter_BetType_Daily_PunterInfo(
                                ref workSheet,
                                currentRow,
                                ref currentColumn,
                                _2dNegativeNumberStyle,
                                _2dNumberStyle,
                                _2dNegativePercentNumberStyle,
                                intNumberStyle,
                                string.Empty,
                                string.Empty,
                                string.Empty,
                                string.Empty,
                                false);

                            var monthLength = report.GrandTotal.ChildsInfo[j].ChildsInfo.Count;
                            for (var k = 0; k < monthLength; k++)
                            {
                                this.ExportTopPunter_BetType_Daily_PunterInfo(
                                    ref workSheet,
                                    currentRow,
                                    ref currentColumn,
                                    _2dNegativeNumberStyle,
                                    _2dNumberStyle,
                                    _2dNegativePercentNumberStyle,
                                    intNumberStyle,
                                    string.Empty,
                                    string.Empty,
                                    string.Empty,
                                    string.Empty,
                                    false);
                            }

                            //// NullYearInfo
                        }
                    }

                    var leagueGrandTotal = league.GrandTotal;
                    this.ExportTopPunter_BetType_Daily_PunterInfo(
                        ref workSheet,
                        currentRow,
                        ref currentColumn,
                        _2dNegativeNumberStyle,
                        _2dNumberStyle,
                        _2dNegativePercentNumberStyle,
                        intNumberStyle,
                        leagueGrandTotal.WinLoss,
                        leagueGrandTotal.Turnover,
                        leagueGrandTotal.Margin,
                        leagueGrandTotal.BetCount,
                        false);
                    currentRow++;

                    //// LeagueInfo
                }

                //// GrandTotal

                currentColumn = startColumn;
                currentCell = workSheet.Cells[currentRow, currentColumn];
                currentCell.Value = grandTotal.Name;
                currentCell.Style = gtrTextStyle_FirstRow;
                currentCell.Style.HorizontalAlignment = HorizontalAlignmentStyle.Right;
                currentColumn++;

                yearLength = grandTotal.ChildsInfo.Count;
                for (var i = 0; i < yearLength; i++)
                {
                    var year = grandTotal.ChildsInfo[i];
                    this.ExportTopPunter_BetType_Daily_PunterInfo(
                        ref workSheet,
                        currentRow,
                        ref currentColumn,
                        gtr2dNegativeNumberStyle_FirstRow,
                        gtr2dNumberStyle_FirstRow,
                        gtr2dNegativePercentNumberStyle_FirstRow,
                        gtrIntNumberStyle_FirstRow,
                        year.WinLoss,
                        year.Turnover,
                        year.Margin,
                        year.BetCount,
                        false);

                    var monthLength = year.ChildsInfo.Count;
                    for (var j = 0; j < monthLength; j++)
                    {
                        var month = year.ChildsInfo[j];
                        this.ExportTopPunter_BetType_Daily_PunterInfo(
                            ref workSheet,
                            currentRow,
                            ref currentColumn,
                            gtr2dNegativeNumberStyle_FirstRow,
                            gtr2dNumberStyle_FirstRow,
                            gtr2dNegativePercentNumberStyle_FirstRow,
                            gtrIntNumberStyle_FirstRow,
                            month.WinLoss,
                            month.Turnover,
                            month.Margin,
                            month.BetCount,
                            false);
                    }

                    var allTotal = grandTotal.GrandTotal;
                    this.ExportTopPunter_BetType_Daily_PunterInfo(
                        ref workSheet,
                        currentRow,
                        ref currentColumn,
                        gtr2dNegativeNumberStyle_FirstRow,
                        gtr2dNumberStyle_FirstRow,
                        gtr2dNegativePercentNumberStyle_FirstRow,
                        gtrIntNumberStyle_FirstRow,
                        allTotal.WinLoss,
                        allTotal.Turnover,
                        allTotal.Margin,
                        allTotal.BetCount,
                        false);
                }

                //// GrandTotal
            }
            else
            {
                currentColumn = startColumn;
                currentCellRange = workSheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow + 1, currentColumn);
                currentCellRange.Merged = true;
                currentCellRange.Value = Labels.League;
                currentCellRange.Style = headerTextStyle;
                workSheet.Columns[currentColumn].Width = 200 * ExcelExtensions.FontWidth;
                currentColumn++;

                currentCellRange = workSheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow, currentColumn + 3);
                currentCellRange.Merged = true;
                currentCellRange.Value = Labels.Grand_Total;
                currentCellRange.Style = headerTextStyle;

                this.ExportTopPunter_BetType_Daily_PunterInfo(
                    ref workSheet,
                    currentRow + 1,
                    ref currentColumn,
                    headerTextStyle,
                    headerTextStyle,
                    headerTextStyle,
                    headerTextStyle,
                    Labels.Win_Loss,
                    Labels.Turnover,
                    Labels.Margin,
                    Labels.Bet_Count,
                    true);
                currentRow += 2;

                workSheet.ExportEmptyReport(startRow, startColumn, ref currentRow, startColumn + 4);
            }
        }
    }
}
