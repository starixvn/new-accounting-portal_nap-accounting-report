// --------------------------------------------------------------------------
// <copyright file="VirtualSport_VirtualHorseRacingReportService.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Louis.Nguyen" date="2012.12.07">Created</change>
// </history>

namespace Fanex.AccountingReports.Main.Services.Implement.DailyWinLoss2h
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using ExcelUtil.Spreadsheet;
    using Fanex.AccountingReports.Core.Constants;
    using Fanex.AccountingReports.Core.Extensions;
    using Fanex.AccountingReports.Core.Services.Impl;
    using Fanex.AccountingReports.Core.Web;
    using Fanex.AccountingReports.Main.Common;
    using Fanex.AccountingReports.Main.Criteria.DailyWinLoss2h;
    using Fanex.AccountingReports.Main.Models.DailyWinLoss2h;
    using Fanex.AccountingReports.Main.Resources;
    using Fanex.Common;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public partial class DailyWinLoss2hService
    {
        /// <summary>
        /// Gets the virtual sports virtual horse racing report.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>The virtual sports virtual horse racing report.</returns>
        [SuppressMessage("StyleCop.CSharp.Nexcel.NexcelCustomRules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]  
        public WinLossInfo GetVirtualSports_VirtualHorseRacingReport(GetVirtualSports_VirtualHorseRacingReportCriteria criteria)
        {
            try
            {
                WinLossInfo report = new WinLossInfo
                {
                    GrandTotal = new WinLossInfo
                    {
                        Name = Labels.Total
                    },
                    ChildsInfo = new List<WinLossInfo>()
                };

                var rawData = this.GetReportData<dynamic>(criteria);
                if (rawData != null)
                {
                    var leagueGroups = rawData.GroupBy(data => data.LeagueID);
                    var leagueIdList = rawData.Select(data => Converter.ToInt(data.LeagueID))
                                              .ToList();
                    CacheRef.SaveCacheRefResponse(leagueIdList, CacheRef.GetLeagueName, ref this._leagueNameCache);
                    foreach (var leagueGroup in leagueGroups)
                    {
                        var leagueWinLoss = new WinLossInfo
                        {
                            Id = Converter.ToInt(leagueGroup.Key),
                            ChildsInfo = new List<WinLossInfo>(),
                            GrandTotal = new WinLossInfo()
                        };

                        leagueWinLoss.Name = this._leagueNameCache[leagueWinLoss.Id];
                        leagueWinLoss.GrandTotal.Name = Labels.Total;
                        foreach (var data in leagueGroup)
                        {
                            WinLossInfo info = new WinLossInfo
                            {
                                WinLoss = Converter.ToDecimal(data.WinLost),
                                Position = Converter.ToDecimal(data.Position),
                                Commission = Converter.ToDecimal(data.Commission),
                                LineTotal = Converter.ToDecimal(data.LineTotal),
                                Turnover = Converter.ToDecimal(data.Turnover)
                            };

                            DateTime raceTime = Converter.ToDateTime(data.Race);
                            info.Name = string.Format(DateTimeFormatConstants.ShortTime, raceTime);
                            info.AliasName = raceTime.ToLongFormatedDateWithSlash();
                            leagueWinLoss.GrandTotal.Add(info);
                            info = WinLossInfo.Format_Total(info);
                            leagueWinLoss.ChildsInfo.Add(info);
                        }

                        leagueWinLoss.ChildsInfo = leagueWinLoss.ChildsInfo.OrderBy(info => Converter.ToDateTime(info.AliasName)).ToList();
                        report.GrandTotal.Add(leagueWinLoss.GrandTotal);
                        leagueWinLoss = WinLossInfo.Format_Total(leagueWinLoss);
                        report.ChildsInfo.Add(leagueWinLoss);
                    }

                    report.ChildsInfo = report.ChildsInfo.OrderBy(leagueGroup => leagueGroup.Id).ToList();
                    report = WinLossInfo.Format_Total(report);
                }

                SessionManager.Set<WinLossInfo>(MainConstants.DailyWinLoss2hVirtualSportsVirtualHorseRacingReportSessionKey, report);
                return report;
            }
            catch (Exception ex)
            {
                HandleException(ex);
                SessionManager.Set<WinLossInfo>(MainConstants.DailyWinLoss2hVirtualSportsVirtualHorseRacingReportSessionKey, null);
                throw;
            }
        }

        /// <summary>
        /// Exports the virtual sports virtual horse racing report.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        [SuppressMessage("StyleCop.CSharp.Nexcel.NexcelCustomRules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]  
        public void ExportVirtualSports_VirtualHorseRacingReport(GetVirtualSports_VirtualHorseRacingReportCriteria criteria)
        {
            try
            {
                var report = SessionManager.Get<WinLossInfo>(MainConstants.DailyWinLoss2hVirtualSportsVirtualHorseRacingReportSessionKey);
                if (report == null)
                {
                    report = this.GetVirtualSports_VirtualHorseRacingReport(criteria);
                }

                //// ExcelTypes

                ExcelFile excelFile = new ExcelFile();
                ExcelWorksheetCollection worksheets = excelFile.Worksheets;
                string sheetName = MainResources.DailyWinLoss2h_SummaryRacingReport_SheetName;
                ExcelWorksheet workSheet = worksheets.Add(sheetName);

                int startRow = 0;
                int startColumn = 0;
                int maxColumnIndex = 6;
                var currentRow = startRow;

                //// ExcelTypes
                //// MakeExcelFile

                var headerTextStyle = ExcelExtensions.GetCellTitleStyle(ExcelExtensions.HeaderTitleColor);
                var backgroundColor = ExcelExtensions.BackGroundColor;
                var textStyle = ExcelExtensions.GetCellTextStyle(backgroundColor, false);
                var twoDecimalNumberStyle = ExcelExtensions.GetCellNumberStyle(NumberFormatConstants.TwoDecimalDigitFormat, backgroundColor, false, false, false);
                var twoDecimalPositiveNumberStyle = ExcelExtensions.GetCellNumberStyle(NumberFormatConstants.TwoDecimalDigitFormat, backgroundColor, false, true, false);
                var twoDecimalPositivePercentNumberStyle = ExcelExtensions.GetCellNumberStyle(
                    NumberFormatConstants.TwoDecimalDigitPercentFormat,
                    backgroundColor,
                    false,
                    true,
                    false);

                var grandTotalRowColor = ExcelExtensions.GrandTotalRowColor;
                var grandTotalRowTextStyle = ExcelExtensions.GetCellTextStyle(grandTotalRowColor, true);
                var grandTotalRowTwoDecimalNumberStyle = ExcelExtensions.GetCellNumberStyle(
                    NumberFormatConstants.TwoDecimalDigitFormat,
                    grandTotalRowColor,
                    true,
                    false,
                    false);
                var grandTotalTwoDecimalPositiveNumberStyle = ExcelExtensions.GetCellNumberStyle(
                    NumberFormatConstants.TwoDecimalDigitFormat,
                    grandTotalRowColor,
                    true,
                    true,
                    false);
                var grandTotalTwoDecimalPositivePercentNumberStyle = ExcelExtensions.GetCellNumberStyle(
                    NumberFormatConstants.TwoDecimalDigitPercentFormat,
                    grandTotalRowColor,
                    true,
                    true,
                    false);

                var grandTotalRowTextStyle_FirstRow = ExcelExtensions.GetCellTextStyle(ExcelExtensions.GrandTotalRowColor, true);
                grandTotalRowTextStyle_FirstRow.Borders.SetBorders(MultipleBorders.Top, ExcelExtensions.GrandTotalSeparatorColor, LineStyle.Medium);
                var grandTotalRowTwoDecimalNumberStyle_FirstRow = ExcelExtensions.GetCellNumberStyle(
                    NumberFormatConstants.TwoDecimalDigitFormat,
                    ExcelExtensions.GrandTotalRowColor,
                    true,
                    false,
                    false);
                grandTotalRowTwoDecimalNumberStyle_FirstRow.Borders.SetBorders(MultipleBorders.Top, ExcelExtensions.GrandTotalSeparatorColor, LineStyle.Medium);
                var grandTotalRowTwoDecimalPositiveNumberStyle_FirstRow = ExcelExtensions.GetCellNumberStyle(
                    NumberFormatConstants.TwoDecimalDigitFormat,
                    ExcelExtensions.GrandTotalRowColor,
                    true,
                    true,
                    false);
                grandTotalRowTwoDecimalPositiveNumberStyle_FirstRow.Borders.SetBorders(MultipleBorders.Top, ExcelExtensions.GrandTotalSeparatorColor, LineStyle.Medium);
                var gtrPercentNumberStyle_FirstRow = ExcelExtensions.GetCellNumberStyle(
                    NumberFormatConstants.TwoDecimalDigitPercentFormat,
                    ExcelExtensions.GrandTotalRowColor,
                    true,
                    true,
                    false);
                gtrPercentNumberStyle_FirstRow.Borders.SetBorders(MultipleBorders.Top, ExcelExtensions.GrandTotalSeparatorColor, LineStyle.Medium);
                var currentColumn = startColumn;
                CellRange currentCellRange;
                ExcelCell currentCell;

                //// Header

                for (int colIndex = 0; colIndex <= maxColumnIndex; colIndex++)
                {
                    workSheet.Columns[startColumn + colIndex].Width = 100 * ExcelExtensions.FontWidth;
                }

                var title = string.Format(
                    MainResources.DailyWinLoss2h_RacingReportByBetType_Title,
                    criteria.SportTypeName,
                    criteria.BetTypeName).ToUpper();
                currentColumn = startColumn;
                currentCellRange = workSheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow + 1, currentColumn + maxColumnIndex);
                currentCellRange.Merged = true;
                currentCellRange.Value = title;
                currentCellRange.Style = headerTextStyle;
                currentRow += 2;

                workSheet.ExportEmptyRow(startRow, startColumn, ref currentRow, startColumn + maxColumnIndex);

                this.ExportHeaderColumns_SportReport_NotApplyVirtualRate(
                    ref workSheet,
                    ref currentRow,
                    startRow,
                    startColumn,
                    false);

                //// Header

                if (report != null && report.ChildsInfo.Count > 0)
                {
                    var leagueLength = report.ChildsInfo.Count;
                    WinLossInfo info;
                    for (var i = 0; i < leagueLength; i++)
                    {
                        //// LeagueHeadLine

                        _dailyWinLoss2hUtilityService.SetOddEvenColor_ExportExcel(
                            ref textStyle,
                            ref twoDecimalNumberStyle,
                            ref twoDecimalPositiveNumberStyle,
                            ref twoDecimalPositivePercentNumberStyle,
                            true);

                        var league = report.ChildsInfo[i];
                        info = league;
                        currentColumn = startColumn;
                        currentCellRange = workSheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow, currentColumn + maxColumnIndex);
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
                            currentCell.Value = info.Name;
                            currentCell.Style = textStyle;
                            currentCell.Style.Font.Weight = ExcelFont.BoldWeight;
                            currentCell.Style.HorizontalAlignment = HorizontalAlignmentStyle.Center;
                            currentColumn++;

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
                            currentCell.Value = info.Turnover;
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
                            currentRow++;

                            //// NormalRow
                        }

                        _dailyWinLoss2hUtilityService.SetOddEvenColor_ExportExcel(
                            ref textStyle,
                            ref twoDecimalNumberStyle,
                            ref twoDecimalPositiveNumberStyle,
                            ref twoDecimalPositivePercentNumberStyle,
                            false);

                        info = league.GrandTotal;
                        _dailyWinLoss2hUtilityService.ExportNormalRow(
                            ref workSheet,
                            grandTotalRowTextStyle_FirstRow,
                            grandTotalRowTwoDecimalNumberStyle_FirstRow,
                            grandTotalRowTwoDecimalPositiveNumberStyle_FirstRow,
                            gtrPercentNumberStyle_FirstRow,
                            info,
                            startColumn,
                            ref currentRow,
                            RowType._2hReportGrandTotal,
                            false);
                    }
                }
                else
                {
                    workSheet.ExportEmptyReport(startRow, startColumn, ref currentRow, startColumn + maxColumnIndex);
                }

                //// MakeExcelFile

                var reportName = string.Format(
                    MainResources.DailyWinLoss2h_ReportName,
                    criteria.Winlostdate.ToShortFormatedDateWithSlash().Replace('/', '_'));
                excelFile.ReponseStream(reportName);
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }
        }
    }
}
