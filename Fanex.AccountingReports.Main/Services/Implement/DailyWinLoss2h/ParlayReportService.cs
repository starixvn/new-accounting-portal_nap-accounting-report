// -----------------------------------------------------------------------
// <copyright file="ParlayReportService.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
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
        private readonly string _parlaySessionKey = "Fanex.AccountingReports.Main.DailyWinLoss2h.ParlayReport" +
           (AppSession.CurrentUser != null ? AppSession.CurrentUser.Username : string.Empty);

        /// <summary>
        /// Gets the parlay report.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>The parlay report data.</returns>
        [SuppressMessage("StyleCop.CSharp.Nexcel.NexcelCustomRules",
        "SP2101:MethodMustNotContainMoreLinesThan",
        Justification = "Reviewed. Suppression is OK here.")]
        public WinLossInfo GetParlayReport(GetParlayReportCriteria criteria)
        {
            try
            {
                WinLossInfo report = new WinLossInfo
                {
                    ChildsInfo = new List<WinLossInfo>(),
                    GrandTotal = new WinLossInfo()
                    {
                        Name = Labels.Grand_Total
                    },
                    CreditTotal = new WinLossInfo
                    {
                        Name = Labels.Credit
                    },
                    DepositTotal = new WinLossInfo
                    {
                        Name = Labels.Deposit
                    }
                };

                var rawData = _reportRepository.GetParlayReport(criteria);

                var parlayGroup = rawData.GroupBy(r => r.Bettype).ToList();

                foreach (var group in parlayGroup)
                {
                    var parlayWinLossByBetType = new WinLossInfo
                    {
                        Id = Converter.ToInt(group.Key),
                        CreditTotal = new WinLossInfo
                        {
                            Name = Labels.Credit
                        },
                        DepositTotal = new WinLossInfo
                        {
                            Name = Labels.Deposit
                        },
                        GrandTotal = new WinLossInfo()
                    };

                    int parlayTypeId = Converter.ToInt(group.Key);

                    foreach (var data in group)
                    {
                        var info = new WinLossInfo
                        {
                            Id = parlayTypeId,
                            WinLoss = Converter.ToDecimal(data.Winlost),
                            Position = Converter.ToDecimal(data.Position),
                            Commission = Converter.ToDecimal(data.Commission),
                            LineTotal = Converter.ToDecimal(data.RealLineTotal),
                            VirtualLineTotal = Converter.ToDecimal(data.VirtualLineTotal),
                            Turnover = Converter.ToDecimal(data.RealTurnover),
                            VirtualTurnover = Converter.ToDecimal(data.VirtualTurnover),
                            SiteType = Converter.ToInt(data.Site),
                            EnableVirtual = Converter.ToBool(data.EnableVirtual),
                            Name = Converter.ToString2(data.SportName)
                        };

                        var rowType = _dailyWinLoss2hUtilityService.GetRowType(0, info.SiteType);

                        switch (rowType)
                        {
                            case RowType._2hReportCredit:
                                info.AliasName = Labels.Credit;
                                parlayWinLossByBetType.CreditTotal.Add(info);
                                report.CreditTotal.Add(info);
                                break;

                            case RowType._2hReportDeposit:
                                info.AliasName = Labels.Deposit;
                                parlayWinLossByBetType.DepositTotal.Add(info);
                                report.DepositTotal.Add(info);
                                break;
                        }

                        parlayWinLossByBetType.GrandTotal.Id = parlayTypeId;
                        parlayWinLossByBetType.GrandTotal.Name = info.Name;
                        parlayWinLossByBetType.GrandTotal.Add(info);
                        report.GrandTotal.Add(info);
                    }

                    report.ChildsInfo.Add(parlayWinLossByBetType);
                }

                SessionManager.Set<WinLossInfo>(_parlaySessionKey, report);
                return report;
            }
            catch (Exception ex)
            {
                HandleException(ex);
                throw;
            }
        }

        /// <summary>
        /// Exports the parlay report.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        [SuppressMessage("StyleCop.CSharp.Nexcel.NexcelCustomRules",
        "SP2101:MethodMustNotContainMoreLinesThan",
        Justification = "Reviewed. Suppression is OK here.")]
        public void ExportParlayReport(GetParlayReportCriteria criteria)
        {
            try
            {
                var report = SessionManager.Get<WinLossInfo>(_parlaySessionKey);

                if (report == null)
                {
                    report = this.GetParlayReport(criteria);
                }

                //// ExcelTypes

                ExcelFile excelFile = new ExcelFile();
                ExcelWorksheetCollection worksheets = excelFile.Worksheets;
                string sheetName = MainResources.DailyWinLoss2h_ParlayReport_SheetName;
                ExcelWorksheet workSheet = worksheets.Add(sheetName);

                int startRow = 0;
                int startColumn = 0;
                int maxColumnIndex = 10;
                var currentRow = startRow;

                var headerTextStyle = ExcelExtensions.GetCellTitleStyle(ExcelExtensions.HeaderTitleColor);
                var currentColumn = startColumn;
                CellRange currentCellRange;
                ExcelCell currentCell;

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

                //// ExcelTypes
                //// MakeExcelFile

                //// Header

                workSheet.Columns[startColumn].Width = 190 * ExcelExtensions.FontWidth;
                for (int colIndex = 1; colIndex <= maxColumnIndex; colIndex++)
                {
                    workSheet.Columns[startColumn + colIndex].Width = 130 * ExcelExtensions.FontWidth;
                }

                var title = MainResources.DailyWinLoss2h_ParlayReport_Title;
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
                currentCellRange.Value = Labels.Sport_Type;
                currentCellRange.Style = headerTextStyle;
                currentColumn++;

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
                currentColumn = startColumn + 4;
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
                int reportLength = report.ChildsInfo.Count;
                if (report != null && reportLength > 0)
                {
                    WinLossInfo info;
                    for (var i = 0; i < reportLength; i++)
                    {
                        var parlayType = report.ChildsInfo[i];

                        _dailyWinLoss2hUtilityService.SetOddEvenColor_ExportExcel(
                            ref textStyle,
                            ref twoDecimalNumberStyle,
                            ref twoDecimalPositiveNumberStyle,
                            ref twoDecimalPositivePercentNumberStyle,
                            false);

                        info = parlayType.GrandTotal;
                        _dailyWinLoss2hUtilityService.ExportNormalRow(
                            ref workSheet,
                            subTotalTextStyle,
                            subTotalTwoDecimalNumberStyle,
                            subTotalTwoDecimalPositiveNumberStyle,
                            subTotalTwoDecimalPositivePercentNumberStyle,
                            info,
                            startColumn,
                            ref currentRow,
                            RowType.Normal,
                            true,
                            true);

                        info = parlayType.CreditTotal;
                        _dailyWinLoss2hUtilityService.ExportNormalRow(
                            ref workSheet,
                            textStyle,
                            twoDecimalNumberStyle,
                            twoDecimalPositiveNumberStyle,
                            twoDecimalPositivePercentNumberStyle,
                            info,
                            startColumn,
                            ref currentRow,
                            RowType._2hReportCredit,
                            true,
                            true);

                        info = parlayType.DepositTotal;
                        _dailyWinLoss2hUtilityService.ExportNormalRow(
                            ref workSheet,
                            textStyle,
                            twoDecimalNumberStyle,
                            twoDecimalPositiveNumberStyle,
                            twoDecimalPositivePercentNumberStyle,
                            info,
                            startColumn,
                            ref currentRow,
                            RowType._2hReportDeposit,
                            true,
                            true);
                    }

                    _dailyWinLoss2hUtilityService.SetOddEvenColor_ExportExcel(
                        ref textStyle,
                        ref twoDecimalNumberStyle,
                        ref twoDecimalPositiveNumberStyle,
                        ref twoDecimalPositivePercentNumberStyle,
                        false);

                    //// GrandTotal

                    // GrandTotalValue
                    var grandTotalSeparatorColor = ExcelExtensions.GrandTotalSeparatorColor;
                    info = report.GrandTotal;
                    currentColumn = startColumn;
                    currentCell = workSheet.Cells[currentRow, currentColumn];
                    currentCell.Value = info.Name;
                    currentCell.Style = grandTotalRowTextStyle;
                    currentCell.Style.HorizontalAlignment = HorizontalAlignmentStyle.Right;
                    currentCell.Style.Borders.SetBorders(MultipleBorders.Top, grandTotalSeparatorColor, LineStyle.Medium);
                    currentColumn++;

                    currentCell = workSheet.Cells[currentRow, currentColumn];
                    currentCell.Style = grandTotalTwoDecimalPositiveNumberStyle;
                    currentCell.Style.Borders.SetBorders(MultipleBorders.Top, grandTotalSeparatorColor, LineStyle.Medium);
                    currentCell.Value = info.WinLoss;
                    currentColumn++;

                    currentCell = workSheet.Cells[currentRow, currentColumn];
                    currentCell.Style = grandTotalRowTwoDecimalNumberStyle;
                    currentCell.Style.Borders.SetBorders(MultipleBorders.Top, grandTotalSeparatorColor, LineStyle.Medium);
                    currentCell.Value = info.Position;
                    currentColumn++;

                    currentCell = workSheet.Cells[currentRow, currentColumn];
                    currentCell.Style = grandTotalRowTwoDecimalNumberStyle;
                    currentCell.Style.Borders.SetBorders(MultipleBorders.Top, grandTotalSeparatorColor, LineStyle.Medium);
                    currentCell.Value = info.Commission;
                    currentColumn++;

                    currentCell = workSheet.Cells[currentRow, currentColumn];
                    currentCell.Value = info.LineTotal;
                    currentCell.Style = grandTotalTwoDecimalPositiveNumberStyle;
                    currentCell.Style.Font.Weight = ExcelFont.BoldWeight;
                    currentCell.Style.Borders.SetBorders(MultipleBorders.Top, grandTotalSeparatorColor, LineStyle.Medium);
                    currentColumn++;

                    currentCell = workSheet.Cells[currentRow, currentColumn];
                    currentCell.Value = info.VirtualLineTotal;
                    currentCell.Style = grandTotalTwoDecimalPositiveNumberStyle;
                    currentCell.Style.Font.Weight = ExcelFont.BoldWeight;
                    currentCell.Style.Borders.SetBorders(MultipleBorders.Top, grandTotalSeparatorColor, LineStyle.Medium);
                    currentColumn++;

                    currentCell = workSheet.Cells[currentRow, currentColumn];
                    currentCell.Value = info.Variance;
                    currentCell.Style = grandTotalTwoDecimalPositiveNumberStyle;
                    currentCell.Style.Font.Weight = ExcelFont.BoldWeight;
                    currentCell.Style.Borders.SetBorders(MultipleBorders.Top, grandTotalSeparatorColor, LineStyle.Medium);
                    currentColumn++;

                    currentCell = workSheet.Cells[currentRow, currentColumn];
                    currentCell.Style = grandTotalRowTwoDecimalNumberStyle;
                    currentCell.Style.Borders.SetBorders(MultipleBorders.Top, grandTotalSeparatorColor, LineStyle.Medium);
                    currentCell.Value = info.Turnover;
                    currentColumn++;

                    currentCell = workSheet.Cells[currentRow, currentColumn];
                    currentCell.Style = grandTotalRowTwoDecimalNumberStyle;
                    currentCell.Style.Borders.SetBorders(MultipleBorders.Top, grandTotalSeparatorColor, LineStyle.Medium);
                    currentCell.Value = info.VirtualTurnover;
                    currentColumn++;

                    currentCell = workSheet.Cells[currentRow, currentColumn];
                    currentCell.Style = grandTotalTwoDecimalPositivePercentNumberStyle;
                    currentCell.Style.Borders.SetBorders(MultipleBorders.Top, grandTotalSeparatorColor, LineStyle.Medium);
                    currentCell.Value = info.Margin;
                    currentColumn++;

                    currentCell = workSheet.Cells[currentRow, currentColumn];
                    currentCell.Style = grandTotalTwoDecimalPositivePercentNumberStyle;
                    currentCell.Style.Borders.SetBorders(MultipleBorders.Top, grandTotalSeparatorColor, LineStyle.Medium);
                    currentCell.Value = info.VirtualMargin;
                    currentColumn++;
                    currentRow++;

                    info = report.CreditTotal;
                    _dailyWinLoss2hUtilityService.ExportNormalRow(
                        ref workSheet,
                        grandTotalRowTextStyle,
                        grandTotalRowTwoDecimalNumberStyle,
                        grandTotalTwoDecimalPositiveNumberStyle,
                        grandTotalTwoDecimalPositivePercentNumberStyle,
                        info,
                        startColumn,
                        ref currentRow,
                        RowType._2hReportCredit,
                        true,
                        true,
                        false);

                    info = report.DepositTotal;
                    _dailyWinLoss2hUtilityService.ExportNormalRow(
                        ref workSheet,
                        grandTotalRowTextStyle,
                        grandTotalRowTwoDecimalNumberStyle,
                        grandTotalTwoDecimalPositiveNumberStyle,
                        grandTotalTwoDecimalPositivePercentNumberStyle,
                        info,
                        startColumn,
                        ref currentRow,
                        RowType._2hReportDeposit,
                        true,
                        true,
                        false);
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