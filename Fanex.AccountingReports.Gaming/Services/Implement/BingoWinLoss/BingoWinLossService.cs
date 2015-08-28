// --------------------------------------------------------------------------
// <copyright file="BingoWinLossService.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Louis.Nguyen" date="2012.12.07">Created</change>
// </history>

namespace Fanex.AccountingReports.Gaming.Services.Implement.BingoWinLoss
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
    using Fanex.AccountingReports.Gaming.Constants;
    using Fanex.AccountingReports.Gaming.Criteria.BingoWinLoss;
    using Fanex.AccountingReports.Gaming.Models.BingoWinLoss;
    using Fanex.AccountingReports.Gaming.Resources;
    using Fanex.AccountingReports.Gaming.Services.Interface;
    using Fanex.Common;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class BingoWinLossService : ServiceBase, IBingoWinLossService
    {
        private Dictionary<object, string> _betTypeNameCache;

        /// <summary>
        /// Gets the report.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>The report.</returns>
        [SuppressMessage("StyleCop.CSharp.Nexcel.NexcelCustomRules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        public WinLossInfo GetReport(GetReportCriteria criteria)
        {
            try
            {
                WinLossInfo report = new WinLossInfo
                {
                    Total = new WinLossInfo
                    {
                        Name = Labels.Grand_Total
                    },
                    ChildsInfo = new List<WinLossInfo>()
                };

                Dictionary<string, int> betType_CurrencyIndexCache = new Dictionary<string, int>();
                Dictionary<int, int> betTypeIndexCache = new Dictionary<int, int>();

                if (criteria.FromDate <= ReportService.BalanceUp.MoveBetsDay &&
                    ReportService.BalanceUp.MoveBetsDay < criteria.ToDate)
                {
                    this.GetReport_ByRange(
                        new GetReportCriteria
                        {
                            FromDate = ReportService.BalanceUp.MoveBetsDay.AddDays(1),
                            ToDate = criteria.ToDate
                        },
                        ref report,
                        ref betType_CurrencyIndexCache,
                        ref betTypeIndexCache);

                    this.GetReport_ByRange(
                        new GetReportCriteria
                        {
                            FromDate = criteria.FromDate,
                            ToDate = ReportService.BalanceUp.MoveBetsDay
                        },
                        ref report,
                        ref betType_CurrencyIndexCache,
                        ref betTypeIndexCache);
                }
                else
                {
                    this.GetReport_ByRange(
                        criteria,
                        ref report,
                        ref betType_CurrencyIndexCache,
                    ref betTypeIndexCache);
                }

                if (!criteria.IsDirectFromFilter)
                {
                    SessionManager.Set<WinLossInfo>(GamingConstants.BingoWinLossReportSessionKey, report);
                }

                for (var i = 0; i < report.ChildsInfo.Count; i++)
                {
                    report.ChildsInfo[i].ChildsInfo = report.ChildsInfo[i].ChildsInfo.OrderBy(info => info.CurrencyName)
                                                   .ToList();
                }

                report.ChildsInfo = report.ChildsInfo.OrderBy(info => info.Name)
                                                     .ToList();
                return report;
            }
            catch (Exception ex)
            {
                HandleException(ex);
                throw;
            }
        }

        /// <summary>
        /// Exports the report.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        [SuppressMessage("StyleCop.CSharp.Nexcel.NexcelCustomRules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        public void ExportReport(GetReportCriteria criteria)
        {
            try
            {
                WinLossInfo report = SessionManager.Get<WinLossInfo>(GamingConstants.BingoWinLossReportSessionKey);
                if (criteria.IsDirectFromFilter)
                {
                    report = this.GetReport(criteria);
                }
                else
                {
                    report = SessionManager.Get<WinLossInfo>(GamingConstants.BingoWinLossReportSessionKey);
                    if (report == null)
                    {
                        report = this.GetReport(criteria);
                    }
                }

                ExcelFile excelFile = new ExcelFile();
                ExcelWorksheetCollection worksheets = excelFile.Worksheets;
                string sheetName = GamingResources.BingoWinLoss_SheetName;
                ExcelWorksheet workSheet = worksheets.Add(sheetName);

                int startRow = 0;
                int startColumn = 0;
                var currentRow = startRow;

                var headerTextStyle = ExcelExtensions.GetCellTitleStyle(ExcelExtensions.HeaderTitleColor);
                var textStyle = ExcelExtensions.GetCellTextStyle(ExcelExtensions.BackGroundColor, false);
                var twoDecimalNumberStyle = ExcelExtensions.GetCellNumberStyle(
                    NumberFormatConstants.TwoDecimalDigitFormat,
                    ExcelExtensions.BackGroundColor,
                    false,
                    false,
                    false);

                var grandTotalRowTextStyle_1stRow = ExcelExtensions.GetCellTextStyle(ExcelExtensions.GrandTotalRowColor, true);
                grandTotalRowTextStyle_1stRow.Borders.SetBorders(MultipleBorders.Top, ExcelExtensions.GrandTotalSeparatorColor, LineStyle.Medium);
                var grandTotalRowTwoDecimalNumberStyle_1stRow = ExcelExtensions.GetCellNumberStyle(
                    NumberFormatConstants.TwoDecimalDigitFormat,
                    ExcelExtensions.GrandTotalRowColor,
                    true,
                    false,
                    false);
                grandTotalRowTwoDecimalNumberStyle_1stRow.Borders.SetBorders(MultipleBorders.Top, ExcelExtensions.GrandTotalSeparatorColor, LineStyle.Medium);

                var currentColumn = startColumn;
                CellRange currentCellRange;
                ExcelCell currentCell;
                var maxColumnIndex = 6;

                workSheet.Columns[startColumn + currentColumn].Width = 100 * ExcelExtensions.FontWidth;
                currentColumn++;

                workSheet.Columns[startColumn + currentColumn].Width = 100 * ExcelExtensions.FontWidth;
                currentColumn++;

                for (; currentColumn <= maxColumnIndex; currentColumn++)
                {
                    workSheet.Columns[startColumn + currentColumn].Width = 120 * ExcelExtensions.FontWidth;
                }

                var title = string.Format(
                    GamingResources.BingoWinLoss_ReportTitle,
                    criteria.FromDate.ToShortFormatedDate(),
                    criteria.ToDate.ToShortFormatedDate());
                currentColumn = startColumn;
                currentCellRange = workSheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow + 1, currentColumn + maxColumnIndex);
                currentCellRange.Merged = true;
                currentCellRange.Value = title;
                currentCellRange.Style = headerTextStyle;
                currentRow += 2;

                workSheet.ExportEmptyRow(startRow, startColumn, ref currentRow, startColumn + maxColumnIndex);

                ////<Header>

                currentColumn = startColumn;
                currentCell = workSheet.Cells[currentRow, currentColumn];
                currentCell.Value = Labels.Bet_Type;
                currentCell.Style = headerTextStyle;
                currentColumn++;

                currentCell = workSheet.Cells[currentRow, currentColumn];
                currentCell.Value = Labels.Currency;
                currentCell.Style = headerTextStyle;
                currentColumn++;

                currentCell = workSheet.Cells[currentRow, currentColumn];
                currentCell.Value = Labels.Turnover;
                currentCell.Style = headerTextStyle;
                currentColumn++;

                currentCell = workSheet.Cells[currentRow, currentColumn];
                currentCell.Value = Labels.Win_Loss;
                currentCell.Style = headerTextStyle;
                currentColumn++;

                currentCell = workSheet.Cells[currentRow, currentColumn];
                currentCell.Value = Labels.Position;
                currentCell.Style = headerTextStyle;
                currentColumn++;

                currentCell = workSheet.Cells[currentRow, currentColumn];
                currentCell.Value = Labels.Commission;
                currentCell.Style = headerTextStyle;
                currentColumn++;

                currentCell = workSheet.Cells[currentRow, currentColumn];
                currentCell.Value = Labels.Line_Total;
                currentCell.Style = headerTextStyle;
                currentColumn++;
                currentRow++;

                ////</Header>

                if (report != null && report.ChildsInfo.Count > 0)
                {
                    WinLossInfo info = null;
                    var betTypeGroupLength = report.ChildsInfo.Count;
                    for (var i = 0; i < betTypeGroupLength; i++)
                    {
                        var betTypeWinLoss = report.ChildsInfo[i];
                        var infoLength = betTypeWinLoss.ChildsInfo.Count;
                        for (var j = 0; j < infoLength; j++)
                        {
                            ////<NormalRow>

                            info = betTypeWinLoss.ChildsInfo[j];
                            if (j % 2 == 1)
                            {
                                textStyle.FillPattern.SetSolid(ExcelExtensions.EvenRowColor);
                                twoDecimalNumberStyle.FillPattern.SetSolid(ExcelExtensions.EvenRowColor);
                            }
                            else
                            {
                                textStyle.FillPattern.SetSolid(ExcelExtensions.OddRowColor);
                                twoDecimalNumberStyle.FillPattern.SetSolid(ExcelExtensions.OddRowColor);
                            }

                            currentColumn = startColumn;
                            if (j == 0)
                            {
                                currentCellRange = workSheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow + infoLength - 1, currentColumn);
                                currentCellRange.Merged = true;
                                currentCellRange.Value = betTypeWinLoss.Name;
                                currentCellRange.Style = textStyle;
                                currentCellRange.Style.HorizontalAlignment = HorizontalAlignmentStyle.Center;
                            }

                            currentColumn++;

                            currentCell = workSheet.Cells[currentRow, currentColumn];
                            currentCell.Value = info.CurrencyName;
                            currentCell.Style = textStyle;
                            currentCell.Style.HorizontalAlignment = HorizontalAlignmentStyle.Center;
                            currentColumn++;

                            currentCell = workSheet.Cells[currentRow, currentColumn];
                            currentCell.Value = info.Turnover;
                            currentCell.Style = twoDecimalNumberStyle;
                            currentColumn++;

                            currentCell = workSheet.Cells[currentRow, currentColumn];
                            currentCell.Value = info.WinLoss;
                            currentCell.Style = twoDecimalNumberStyle;
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
                            currentCell.Style = twoDecimalNumberStyle;
                            currentColumn++;
                            currentRow++;

                            ////</NormalRow>
                        }
                    }

                    textStyle.FillPattern.SetSolid(ExcelExtensions.BackGroundColor);
                    twoDecimalNumberStyle.FillPattern.SetSolid(ExcelExtensions.BackGroundColor);

                    ////<TotalRow>

                    info = report.Total;

                    currentColumn = startColumn;
                    currentCellRange = workSheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow, currentColumn + 1);
                    currentCellRange.Merged = true;
                    currentCellRange.Value = info.Name;
                    currentCellRange.Style = grandTotalRowTextStyle_1stRow;
                    currentCellRange.Style.HorizontalAlignment = HorizontalAlignmentStyle.Center;
                    currentColumn += 2;

                    currentCell = workSheet.Cells[currentRow, currentColumn];
                    currentCell.Value = info.Turnover;
                    currentCell.Style = grandTotalRowTwoDecimalNumberStyle_1stRow;
                    currentColumn++;

                    currentCell = workSheet.Cells[currentRow, currentColumn];
                    currentCell.Value = info.WinLoss;
                    currentCell.Style = grandTotalRowTwoDecimalNumberStyle_1stRow;
                    currentColumn++;

                    currentCell = workSheet.Cells[currentRow, currentColumn];
                    currentCell.Value = info.Position;
                    currentCell.Style = grandTotalRowTwoDecimalNumberStyle_1stRow;
                    currentColumn++;

                    currentCell = workSheet.Cells[currentRow, currentColumn];
                    currentCell.Value = info.Commission;
                    currentCell.Style = grandTotalRowTwoDecimalNumberStyle_1stRow;
                    currentColumn++;

                    currentCell = workSheet.Cells[currentRow, currentColumn];
                    currentCell.Value = info.LineTotal;
                    currentCell.Style = grandTotalRowTwoDecimalNumberStyle_1stRow;
                    currentColumn++;
                    currentRow++;

                    ////</TotalRow>
                }
                else
                {
                    workSheet.ExportEmptyReport(startRow, startColumn, ref currentRow, startColumn + maxColumnIndex);
                }

                var reportName = string.Format(
                    GamingResources.BingoWinLoss_ReportName,
                    criteria.FromDate.ToShortFormatedDateWithSlash().Replace('/', '_'),
                    criteria.ToDate.ToShortFormatedDateWithSlash().Replace('/', '_'));
                excelFile.ReponseStream(reportName);
            }
            catch (Exception ex)
            {
                HandleException(ex);
                throw;
            }
        }

        /// <summary>
        /// Gets the report by range.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <param name="report">The report.</param>
        /// <param name="betType_CurrencyIndexCache">The bet type_ currency index cache.</param>
        /// <param name="betTypeIndexCache">The bet type index cache.</param>
        [SuppressMessage("StyleCop.CSharp.Nexcel.NexcelCustomRules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        private void GetReport_ByRange(
            GetReportCriteria criteria,
            ref WinLossInfo report,
            ref Dictionary<string, int> betType_CurrencyIndexCache,
            ref Dictionary<int, int> betTypeIndexCache)
        {
            var rawData = this.GetReportData<dynamic>(criteria).ToList();
            var betTypeList = rawData.Select(data => Converter.ToInt(data.BetType))
                                              .ToList();
            CacheRef.SaveCacheRefResponse(betTypeList, CacheRef.GetBettype, ref this._betTypeNameCache);

            var betTypeGroups = rawData.GroupBy(rawWinLossInfo => rawWinLossInfo.BetType);
            var betTypeGroupLength = betTypeGroups.Count();
            foreach (var betTypeGroup in betTypeGroups)
            {
                var betTypeId = Converter.ToInt(betTypeGroup.Key);
                WinLossInfo betTypeWinLoss;
                if (betTypeIndexCache.ContainsKey(betTypeId))
                {
                    betTypeWinLoss = report.ChildsInfo[betTypeIndexCache[betTypeId]];
                }
                else
                {
                    betTypeWinLoss = new WinLossInfo
                    {
                        Id = Converter.ToInt(betTypeGroup.Key),
                        ChildsInfo = new List<WinLossInfo>(),
                        Total = new WinLossInfo()
                    };
                }

                betTypeWinLoss.Name = this._betTypeNameCache[betTypeWinLoss.Id];
                var betTypeWinLossList = betTypeGroup.ToList();
                var infoLength = betTypeWinLossList.Count;
                for (var j = 0; j < infoLength; j++)
                {
                    var data = betTypeWinLossList[j];
                    var info = new WinLossInfo
                    {
                        WinLoss = Converter.ToDecimal(data.Winlost),
                        Turnover = Converter.ToDecimal(data.Turnover),
                        Position = Converter.ToDecimal(data.Position),
                        Commission = Converter.ToDecimal(data.Commission),
                        LineTotal = Converter.ToDecimal(data.LineTotal),
                        Currency = Converter.ToInt(data.Currency),
                        CurrencyName = Converter.ToString2(data.CurrencyName),
                    };

                    betTypeWinLoss.Total.Add(info);
                    var betType_CurrencyKey = betTypeWinLoss.Id + "_" + info.Currency;
                    if (betType_CurrencyIndexCache.ContainsKey(betType_CurrencyKey))
                    {
                        var betType_CurrencyIndex = betType_CurrencyIndexCache[betType_CurrencyKey];
                        betTypeWinLoss.ChildsInfo[betType_CurrencyIndex].Add(info);
                        betTypeWinLoss.ChildsInfo[betType_CurrencyIndex] = this.FormatWinLossInfo(betTypeWinLoss.ChildsInfo[betType_CurrencyIndex]);
                    }
                    else
                    {
                        betType_CurrencyIndexCache.Add(betType_CurrencyKey, betTypeWinLoss.ChildsInfo.Count);
                        info = this.FormatWinLossInfo(info);
                        betTypeWinLoss.ChildsInfo.Add(info);
                    }

                    report.Total.Add(info);
                }

                if (betTypeIndexCache.ContainsKey(betTypeId))
                {
                    report.ChildsInfo[betTypeIndexCache[betTypeId]] = betTypeWinLoss;
                }
                else
                {
                    betTypeIndexCache.Add(betTypeId, report.ChildsInfo.Count);
                    report.ChildsInfo.Add(betTypeWinLoss);
                }

                betTypeWinLoss.Total = this.FormatWinLossInfo(betTypeWinLoss.Total);
            }

            report.Total = this.FormatWinLossInfo(report.Total);
        }

        private WinLossInfo FormatWinLossInfo(WinLossInfo info)
        {
            info.WinLoss_Str = info.WinLoss.Format(2);
            info.Turnover_Str = info.Turnover.Format(2);
            info.Position_Str = info.Position.Format(2);
            info.Commission_Str = info.Commission.Format(2);
            info.LineTotal_Str = info.LineTotal.Format(2);
            return info;
        }
    }
}
