// --------------------------------------------------------------------------
// <copyright file="MixParlayReportService.cs" company="Nexcel Solutions Vietnam">
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
    using Fanex.AccountingReports.Core.Extensions;
    using Fanex.AccountingReports.Core.Services.Impl;
    using Fanex.AccountingReports.Core.Web;
    using Fanex.AccountingReports.Main.Common;
    using Fanex.AccountingReports.Main.Constants;
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
        /// Gets the mix parlay report.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>The mix parlay report.</returns>
        [SuppressMessage("StyleCop.CSharp.StylecopPlus.Rules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        public WinLossInfo GetMixParlayReport(GetMixParlayReportCriteria criteria)
        {
            try
            {
                WinLossInfo report = new WinLossInfo
                {
                    GrandTotal = new WinLossInfo
                    {
                        Name = Labels.All_Total
                    },
                    ChildsInfo = new List<WinLossInfo>()
                };

                var rawData = this._reportRepository.GetSystemParlayReport(criteria);
                if (rawData != null)
                {
                    var sportGroups = rawData.GroupBy(data => data.SportType);
                    var betTypeList = rawData.Select(data => Converter.ToInt(data.BetType))
                                                .ToList();
                    CacheRef.SaveCacheRefResponse(betTypeList, CacheRef.GetBettype, ref this._betTypeNameCache);
                    foreach (var sportGroup in sportGroups)
                    {
                        var sportWinLoss = new WinLossInfo
                        {
                            Id = Converter.ToInt(sportGroup.Key),
                            ChildsInfo = new List<WinLossInfo>(),
                            GrandTotal = new WinLossInfo()
                        };

                        sportWinLoss.Name = this.GetSportName_MixParlay(sportWinLoss.Id);
                        sportWinLoss.GrandTotal.Name = sportWinLoss.Name + " " + Labels.Total;
                        foreach (var data in sportGroup)
                        {
                            WinLossInfo info = new WinLossInfo
                            {
                                Id = Converter.ToInt(data.BetType),
                                Status = Converter.ToString2(data.Status),
                                WinLoss = Converter.ToDecimal(data.WinLost),
                                Position = Converter.ToDecimal(data.Position),
                                Commission = Converter.ToDecimal(data.Commission),
                                LineTotal = Converter.ToDecimal(data.LineTotal),
                                Turnover = Converter.ToDecimal(data.Turnover)
                            };

                            info.Name = this._betTypeNameCache[info.Id];
                            sportWinLoss.GrandTotal.Add(info);
                            info = WinLossInfo.Format_Total(info);
                            sportWinLoss.ChildsInfo.Add(info);
                        }

                        sportWinLoss.ChildsInfo = sportWinLoss.ChildsInfo.OrderBy(info => info.Id).ToList();
                        report.GrandTotal.Add(sportWinLoss.GrandTotal);
                        sportWinLoss = WinLossInfo.Format_Total(sportWinLoss);
                        report.ChildsInfo.Add(sportWinLoss);
                    }

                    report.ChildsInfo = report.ChildsInfo.OrderBy(sportGroup => sportGroup.Id).ToList();
                    report = WinLossInfo.Format_Total(report);
                }

                SessionManager.Set<WinLossInfo>(MainConstants.DailyWinLoss2hMixParlayReportSessionKey, report);
                return report;
            }
            catch (Exception ex)
            {
                HandleException(ex);
                SessionManager.Set<WinLossInfo>(MainConstants.DailyWinLoss2hMixParlayReportSessionKey, null);
                throw;
            }
        }

        /// <summary>
        /// Exports the system parlay report.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        [SuppressMessage("StyleCop.CSharp.StylecopPlus.Rules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        public void ExportSystemParlayReport(GetMixParlayReportCriteria criteria)
        {
            try
            {
                var report = SessionManager.Get<WinLossInfo>(MainConstants.DailyWinLoss2hMixParlayReportSessionKey);
                if (report == null)
                {
                    report = this.GetMixParlayReport(criteria);
                }

                //// ExcelTypes

                ExcelFile excelFile = new ExcelFile();
                ExcelWorksheetCollection worksheets = excelFile.Worksheets;
                string sheetName = MainResources.DailyWinLoss2h_SystemParlayReport_SheetName;
                ExcelWorksheet workSheet = worksheets.Add(sheetName);

                int startRow = 0;
                int startColumn = 0;
                int maxColumnIndex = 6;
                var currentRow = startRow;

                var headerTextStyle = ExcelExtensions.GetCellTitleStyle(ExcelExtensions.HeaderTitleColor);
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

                var subTotalRowColor = ExcelExtensions.TotalRowColor;
                var subTotalTextStyle = ExcelExtensions.GetCellTextStyle(subTotalRowColor, false);
                var subTotalTwoDecimalNumberStyle = ExcelExtensions.GetCellNumberStyle(
                    CoreConstants.NumberFormatConstants.TwoDecimalDigitFormat,
                    subTotalRowColor,
                    false,
                    false,
                    false);
                var subTotalTwoDecimalPositiveNumberStyle = ExcelExtensions.GetCellNumberStyle(
                    CoreConstants.NumberFormatConstants.TwoDecimalDigitFormat,
                    subTotalRowColor,
                    false,
                    true,
                    false);
                var subTotalTwoDecimalPositivePercentNumberStyle = ExcelExtensions.GetCellNumberStyle(
                    CoreConstants.NumberFormatConstants.TwoDecimalDigitPercentFormat,
                    subTotalRowColor,
                    false,
                    true,
                    false);

                var grandTotalRowTextStyle_FirstRow = ExcelExtensions.GetCellTextStyle(grandTotalRowColor, true);
                grandTotalRowTextStyle_FirstRow.Borders.SetBorders(MultipleBorders.Top, ExcelExtensions.GrandTotalSeparatorColor, LineStyle.Medium);
                var grandTotalRowTwoDecimalNumberStyle_FirstRow = ExcelExtensions.GetCellNumberStyle(
                    CoreConstants.NumberFormatConstants.TwoDecimalDigitFormat,
                    grandTotalRowColor,
                    true,
                    false,
                    false);
                grandTotalRowTwoDecimalNumberStyle_FirstRow.Borders.SetBorders(MultipleBorders.Top, ExcelExtensions.GrandTotalSeparatorColor, LineStyle.Medium);
                var grandTotalRowTwoDecimalPositiveNumberStyle_FirstRow = ExcelExtensions.GetCellNumberStyle(
                    CoreConstants.NumberFormatConstants.TwoDecimalDigitFormat,
                    grandTotalRowColor,
                    true,
                    true,
                    false);
                grandTotalRowTwoDecimalPositiveNumberStyle_FirstRow.Borders.SetBorders(MultipleBorders.Top, ExcelExtensions.GrandTotalSeparatorColor, LineStyle.Medium);
                var gtrPercentNumberStyle_FirstRow = ExcelExtensions.GetCellNumberStyle(
                    CoreConstants.NumberFormatConstants.TwoDecimalDigitPercentFormat,
                    grandTotalRowColor,
                    true,
                    true,
                    false);
                gtrPercentNumberStyle_FirstRow.Borders.SetBorders(MultipleBorders.Top, ExcelExtensions.GrandTotalSeparatorColor, LineStyle.Medium);
                var currentColumn = startColumn;
                CellRange currentCellRange;
                ExcelCell currentCell;

                //// ExcelTypes
                //// MakeExcelFile

                workSheet.Columns[startColumn].Width = 200 * ExcelExtensions.FontWidth;
                for (int colIndex = 1; colIndex <= maxColumnIndex; colIndex++)
                {
                    workSheet.Columns[startColumn + colIndex].Width = 140 * ExcelExtensions.FontWidth;
                }

                //// Header

                currentColumn = startColumn;
                currentCellRange = workSheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow + 1, currentColumn + maxColumnIndex);
                currentCellRange.Merged = true;
                currentCellRange.Value = string.Format(
                    MainResources.DailyWinLoss2h_SystemParlayReport_Title,
                    ReportService.GetSiteTypeName(criteria.Site)).ToUpper();
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
                    var reportLength = report.ChildsInfo.Count;
                    WinLossInfo info;

                    for (var i = 0; i < reportLength; i++)
                    {
                        var sportGroup = report.ChildsInfo[i];
                        for (var j = 0; j < sportGroup.ChildsInfo.Count; j++)
                        {
                            //// NormalRow

                            info = sportGroup.ChildsInfo[j];
                            currentColumn = startColumn;
                            currentCell = workSheet.Cells[currentRow, currentColumn];
                            currentCell.Value = string.Format("{0}({1})", info.Name, info.Status);
                            currentCell.Style = textStyle;
                            currentCell.Style.Font.Weight = ExcelFont.BoldWeight;
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
                            _dailyWinLoss2hUtilityService.ExportColumnTurnover(info.Margin, info.Turnover, ref currentCell);
                            currentColumn++;
                            currentRow++;

                            //// NormalRow
                        }

                        //// SubTotal

                        info = sportGroup.GrandTotal;
                        currentColumn = startColumn;
                        currentCell = workSheet.Cells[currentRow, currentColumn];
                        currentCell.Value = info.Name;
                        currentCell.Style = subTotalTextStyle;
                        currentCell.Style.Font.Weight = ExcelFont.BoldWeight;
                        currentColumn++;

                        currentCell = workSheet.Cells[currentRow, currentColumn];
                        currentCell.Value = info.WinLoss;
                        currentCell.Style = subTotalTwoDecimalPositiveNumberStyle;
                        currentColumn++;

                        currentCell = workSheet.Cells[currentRow, currentColumn];
                        currentCell.Value = info.Position;
                        currentCell.Style = subTotalTwoDecimalNumberStyle;
                        currentColumn++;

                        currentCell = workSheet.Cells[currentRow, currentColumn];
                        currentCell.Value = info.Commission;
                        currentCell.Style = subTotalTwoDecimalNumberStyle;
                        currentColumn++;

                        currentCell = workSheet.Cells[currentRow, currentColumn];
                        currentCell.Value = info.LineTotal;
                        currentCell.Style = subTotalTwoDecimalPositiveNumberStyle;
                        currentColumn++;

                        currentCell = workSheet.Cells[currentRow, currentColumn];
                        currentCell.Value = info.Turnover;
                        currentCell.Style = subTotalTwoDecimalNumberStyle;
                        currentColumn++;

                        currentCell = workSheet.Cells[currentRow, currentColumn];
                        currentCell.Style = subTotalTwoDecimalPositivePercentNumberStyle;
                        _dailyWinLoss2hUtilityService.ExportColumnTurnover(info.Margin, info.Turnover, ref currentCell);
                        currentColumn++;
                        currentRow++;

                        //// SubTotal

                        workSheet.ExportEmptyRow(startRow, startColumn, ref currentRow, startColumn + maxColumnIndex);
                    }

                    //// GrandTotal

                    info = report.GrandTotal;
                    currentColumn = startColumn;
                    currentCell = workSheet.Cells[currentRow, currentColumn];
                    currentCell.Value = info.Name;
                    currentCell.Style = grandTotalRowTextStyle_FirstRow;
                    currentCell.Style.HorizontalAlignment = HorizontalAlignmentStyle.Right;
                    currentColumn++;

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
                    currentCell.Style.Font.Weight = ExcelFont.BoldWeight;
                    currentCell.Style.FillPattern.SetSolid(ExcelExtensions.NoteInformationCellColor);
                    currentColumn++;

                    currentCell = workSheet.Cells[currentRow, currentColumn];
                    currentCell.Value = info.Turnover;
                    currentCell.Style = grandTotalRowTwoDecimalNumberStyle_FirstRow;
                    currentCell.Style.Font.Weight = ExcelFont.BoldWeight;
                    currentColumn++;

                    currentCell = workSheet.Cells[currentRow, currentColumn];
                    currentCell.Style = gtrPercentNumberStyle_FirstRow;
                    _dailyWinLoss2hUtilityService.ExportColumnTurnover(info.Margin, info.Turnover, ref currentCell);
                    currentColumn++;
                    currentRow++;

                    //// GrandTotal
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

        /// <summary>
        /// Exports the mix parlay report.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        [SuppressMessage("StyleCop.CSharp.StylecopPlus.Rules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        public void ExportMixParlayReport(GetMixParlayReportCriteria criteria)
        {
            try
            {
                var report = SessionManager.Get<WinLossInfo>(MainConstants.DailyWinLoss2hMixParlayReportSessionKey);
                if (report == null)
                {
                    report = this.GetMixParlayReport(criteria);
                }

                //// ExcelTypes

                ExcelFile excelFile = new ExcelFile();
                ExcelWorksheetCollection worksheets = excelFile.Worksheets;
                string sheetName = MainResources.DailyWinLoss2h_MixParlayReport_SheetName;
                ExcelWorksheet workSheet = worksheets.Add(sheetName);

                int startRow = 0;
                int startColumn = 0;
                int maxColumnIndex = 6;
                var currentRow = startRow;

                var headerTextStyle = ExcelExtensions.GetCellTitleStyle(ExcelExtensions.HeaderTitleColor);
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

                var subTotalRowColor = ExcelExtensions.TotalRowColor;
                var subTotalTextStyle = ExcelExtensions.GetCellTextStyle(subTotalRowColor, false);
                var subTotalTwoDecimalNumberStyle = ExcelExtensions.GetCellNumberStyle(
                    CoreConstants.NumberFormatConstants.TwoDecimalDigitFormat,
                    subTotalRowColor,
                    false,
                    false,
                    false);
                var subTotalTwoDecimalPositiveNumberStyle = ExcelExtensions.GetCellNumberStyle(
                    CoreConstants.NumberFormatConstants.TwoDecimalDigitFormat,
                    subTotalRowColor,
                    false,
                    true,
                    false);
                var subTotalTwoDecimalPositivePercentNumberStyle = ExcelExtensions.GetCellNumberStyle(
                    CoreConstants.NumberFormatConstants.TwoDecimalDigitPercentFormat,
                    subTotalRowColor,
                    false,
                    true,
                    false);

                var grandTotalRowTextStyle_FirstRow = ExcelExtensions.GetCellTextStyle(grandTotalRowColor, true);
                grandTotalRowTextStyle_FirstRow.Borders.SetBorders(MultipleBorders.Top, ExcelExtensions.GrandTotalSeparatorColor, LineStyle.Medium);
                var grandTotalRowTwoDecimalNumberStyle_FirstRow = ExcelExtensions.GetCellNumberStyle(
                    CoreConstants.NumberFormatConstants.TwoDecimalDigitFormat,
                    grandTotalRowColor,
                    true,
                    false,
                    false);
                grandTotalRowTwoDecimalNumberStyle_FirstRow.Borders.SetBorders(MultipleBorders.Top, ExcelExtensions.GrandTotalSeparatorColor, LineStyle.Medium);
                var grandTotalRowTwoDecimalPositiveNumberStyle_FirstRow = ExcelExtensions.GetCellNumberStyle(
                    CoreConstants.NumberFormatConstants.TwoDecimalDigitFormat,
                    grandTotalRowColor,
                    true,
                    true,
                    false);
                grandTotalRowTwoDecimalPositiveNumberStyle_FirstRow.Borders.SetBorders(MultipleBorders.Top, ExcelExtensions.GrandTotalSeparatorColor, LineStyle.Medium);
                var gtrPercentNumberStyle_FirstRow = ExcelExtensions.GetCellNumberStyle(
                    CoreConstants.NumberFormatConstants.TwoDecimalDigitPercentFormat,
                    grandTotalRowColor,
                    true,
                    true,
                    false);
                gtrPercentNumberStyle_FirstRow.Borders.SetBorders(MultipleBorders.Top, ExcelExtensions.GrandTotalSeparatorColor, LineStyle.Medium);

                var currentColumn = startColumn;
                CellRange currentCellRange;
                ExcelCell currentCell;

                //// ExcelTypes
                //// MakeExcelFile

                workSheet.Columns[startColumn].Width = 200 * ExcelExtensions.FontWidth;
                for (int colIndex = 1; colIndex <= maxColumnIndex; colIndex++)
                {
                    workSheet.Columns[startColumn + colIndex].Width = 140 * ExcelExtensions.FontWidth;
                }

                //// Header

                currentColumn = startColumn;
                currentCellRange = workSheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow + 1, currentColumn + maxColumnIndex);
                currentCellRange.Merged = true;
                currentCellRange.Value = MainResources.DailyWinLoss2h_MixParlayReport_Title;
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
                    var reportLength = report.ChildsInfo.Count;
                    WinLossInfo info;

                    for (var i = 0; i < reportLength; i++)
                    {
                        var sportGroup = report.ChildsInfo[i];
                        for (var j = 0; j < sportGroup.ChildsInfo.Count; j++)
                        {
                            //// NormalRow

                            info = sportGroup.ChildsInfo[j];
                            currentColumn = startColumn;
                            currentCell = workSheet.Cells[currentRow, currentColumn];
                            currentCell.Value = string.Format("{0}({1})", info.Name, info.Status);
                            currentCell.Style = textStyle;
                            currentCell.Style.Font.Weight = ExcelFont.BoldWeight;
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
                            _dailyWinLoss2hUtilityService.ExportColumnTurnover(info.Margin, info.Turnover, ref currentCell);
                            currentColumn++;
                            currentRow++;

                            //// NormalRow
                        }

                        //// SubTotal

                        info = sportGroup.GrandTotal;
                        currentColumn = startColumn;
                        currentCell = workSheet.Cells[currentRow, currentColumn];
                        currentCell.Value = info.Name;
                        currentCell.Style = subTotalTextStyle;
                        currentColumn++;

                        currentCell = workSheet.Cells[currentRow, currentColumn];
                        currentCell.Value = info.WinLoss;
                        currentCell.Style = subTotalTwoDecimalPositiveNumberStyle;
                        currentColumn++;

                        currentCell = workSheet.Cells[currentRow, currentColumn];
                        currentCell.Value = info.Position;
                        currentCell.Style = subTotalTwoDecimalNumberStyle;
                        currentColumn++;

                        currentCell = workSheet.Cells[currentRow, currentColumn];
                        currentCell.Value = info.Commission;
                        currentCell.Style = subTotalTwoDecimalNumberStyle;
                        currentColumn++;

                        currentCell = workSheet.Cells[currentRow, currentColumn];
                        currentCell.Value = info.LineTotal;
                        currentCell.Style = subTotalTwoDecimalPositiveNumberStyle;
                        currentCell.Style.Font.Weight = ExcelFont.BoldWeight;
                        currentColumn++;

                        currentCell = workSheet.Cells[currentRow, currentColumn];
                        currentCell.Value = info.Turnover;
                        currentCell.Style = subTotalTwoDecimalNumberStyle;
                        currentCell.Style.Font.Weight = ExcelFont.BoldWeight;
                        currentColumn++;

                        currentCell = workSheet.Cells[currentRow, currentColumn];
                        currentCell.Style = subTotalTwoDecimalPositivePercentNumberStyle;
                        _dailyWinLoss2hUtilityService.ExportColumnTurnover(info.Margin, info.Turnover, ref currentCell);
                        currentColumn++;
                        currentRow++;

                        //// SubTotal

                        workSheet.ExportEmptyRow(startRow, startColumn, ref currentRow, startColumn + maxColumnIndex);
                    }

                    //// GrandTotal

                    info = report.GrandTotal;
                    currentColumn = startColumn;
                    currentCell = workSheet.Cells[currentRow, currentColumn];
                    currentCell.Value = info.Name;
                    currentCell.Style = grandTotalRowTextStyle_FirstRow;
                    currentCell.Style.HorizontalAlignment = HorizontalAlignmentStyle.Right;
                    currentColumn++;

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
                    currentCell.Style.Font.Weight = ExcelFont.BoldWeight;
                    currentCell.Style.FillPattern.SetSolid(ExcelExtensions.NoteInformationCellColor);
                    currentColumn++;

                    currentCell = workSheet.Cells[currentRow, currentColumn];
                    currentCell.Value = info.Turnover;
                    currentCell.Style = grandTotalRowTwoDecimalNumberStyle_FirstRow;
                    currentCell.Style.Font.Weight = ExcelFont.BoldWeight;
                    currentColumn++;

                    currentCell = workSheet.Cells[currentRow, currentColumn];
                    currentCell.Style = gtrPercentNumberStyle_FirstRow;
                    _dailyWinLoss2hUtilityService.ExportColumnTurnover(info.Margin, info.Turnover, ref currentCell);
                    currentColumn++;
                    currentRow++;

                    //// GrandTotal
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

        /// <summary>
        /// Gets the sport name mix parlay.
        /// </summary>
        /// <param name="sportType">The sport type.</param>
        /// <returns>The sport name mix parlay.</returns>
        private string GetSportName_MixParlay(int sportType)
        {
            var sportTotalName = string.Empty;
            switch (sportType)
            {
                case SportContants._2hReportSystemParlaySoccer:
                    sportTotalName = Labels.Soccer + " " + Labels.Sport;
                    break;
                case SportContants._2hReportSystemParlayOtherSports:
                    sportTotalName = Labels.Others + " " + Labels.Sport;
                    break;
                case SportContants._2hReportSystemParlayMultiSport:
                    sportTotalName = Labels.Multi + " " + Labels.Sport;
                    break;
                default:
                    sportTotalName = string.Empty;
                    break;
            }

            return sportTotalName;
        }
    }
}
