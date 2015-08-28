// --------------------------------------------------------------------------
// <copyright file="NumberGameWinlossByDateService.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="" date="2012.12.07">Created</change>
// </history>

namespace Fanex.AccountingReports.Gaming.Services.Implement
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using ExcelUtil.Spreadsheet;
    using Fanex.AccountingReports.Core.Constants;
    using Fanex.AccountingReports.Core.Criteria;
    using Fanex.AccountingReports.Core.Extensions;
    using Fanex.AccountingReports.Core.Models;
    using Fanex.AccountingReports.Core.Services.Impl;
    using Fanex.AccountingReports.Core.Web;
    using Fanex.AccountingReports.Gaming.Constants;
    using Fanex.AccountingReports.Gaming.Criteria;
    using Fanex.AccountingReports.Gaming.Criteria.NumberGameWinLossByDate;
    using Fanex.AccountingReports.Gaming.Models.NumberGameWinLossByDate;
    using Fanex.AccountingReports.Gaming.Resources;
    using Fanex.AccountingReports.Gaming.Services.Interface;
    using Fanex.Common;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class NumberGameWinlossByDateService : ServiceBase, INumberGameWinlossByDateService
    {
        /// <summary>
        /// Rounds the specified win loss.
        /// </summary>
        /// <param name="winLoss">The win loss.</param>
        public static void Round(ref NumberGameWinLossDetail winLoss)
        {
            if (winLoss != null)
            {
                winLoss.CustomerWinloss = Math.Round(winLoss.CustomerWinloss, 2);
                winLoss.AgencyWinloss = Math.Round(winLoss.AgencyWinloss, 2);
                winLoss.Comm = Math.Round(winLoss.Comm, 2);
                winLoss.LineTotal = Math.Round(winLoss.LineTotal, 2);
                winLoss.VirtualTotal = Math.Round(winLoss.VirtualTotal, 2);
                winLoss.Turnover = Math.Round(winLoss.Turnover, 2);
                winLoss.Variance = Math.Round(winLoss.Variance, 2);
            }
        }

        /// <summary>
        /// Gets the licensee.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>The licensee.</returns>
        public List<EntityBase<int, string>> GetLicensee(GetNumberGameLicenseeCriteria criteria)
        {
            var result = new List<EntityBase<int, string>>();
            try
            {
                var rawData = GetReportData<dynamic>(criteria);
                if (rawData != null)
                {
                    foreach (var row in rawData)
                    {
                        result.Add(new EntityBase<int, string>()
                        {
                            Id = row.LicenseeID,
                            Name = row.Licenseename
                        });
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
        /// Gets the bet types.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>The bet types.</returns>
        public List<EntityBase<int, string>> GetBetTypes(GetNumberGameBetTypeListCriteria criteria)
        {
            var result = new List<EntityBase<int, string>>();
            try
            {
                var rawData = GetReportData<dynamic>(criteria);
                if (rawData != null)
                {
                    foreach (var row in rawData)
                    {
                        result.Add(new EntityBase<int, string>()
                        {
                            Id = row.typeID,
                            Name = row.typenamee
                        });
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
        /// Gets the number game win loss by date.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>The number game win loss by date.</returns>
        [SuppressMessage("StyleCop.CSharp.Nexcel.NexcelCustomRules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        public NumberGameWinLossDetail GetNumberGameWinLossByDate(GetNumberGameWinLossByDateCriteria criteria)
        {
            var result = new NumberGameWinLossDetail
            {
                ChildInfo = new List<NumberGameWinLossDetail>(),
                GrandTotal = new NumberGameWinLossDetail()
            };

            try
            {
                var rawData = GetReportData<dynamic>(criteria);
                if (rawData != null)
                {
                    rawData
                        .GroupBy(r => r.DATE)
                        .ToList()
                        .ForEach(group =>
                        {
                            var winLossByDate = new NumberGameWinLossDetail
                            {
                                WinLossDate = DateTimeExtensions.ToShortFormatedDate(Converter.ToDateTime(group.Key)),
                                ChildInfo = new List<NumberGameWinLossDetail>(),
                                GrandTotal = new NumberGameWinLossDetail()
                            };

                            group.GroupBy(l => l.islive)
                                 .ToList()
                                 .ForEach(status =>
                            {
                                var winLossInfo = new NumberGameWinLossDetail
                                {
                                    Status = status.Key ? "Live" : "Non Live",
                                    ChildInfo = new List<NumberGameWinLossDetail>(),
                                    GrandTotal = new NumberGameWinLossDetail()
                                };

                                status.ToList().ForEach(bettype =>
                                {
                                    var detail = new NumberGameWinLossDetail
                                    {
                                        Bettype = CacheRef.GetBettype(bettype.bettype),
                                        CustomerWinloss = (decimal)bettype.winlost,
                                        AgencyWinloss = (decimal)bettype.position,
                                        Comm = (decimal)bettype.commission,
                                        LineTotal = (decimal)bettype.linetotal,
                                        VirtualTotal = (decimal)bettype.virtuallinetotal,
                                        Variance = (decimal)bettype.linetotal - (decimal)bettype.virtuallinetotal,
                                        Turnover = (decimal)bettype.turnover
                                    };

                                    detail.Margin = this.GetWinLossMargin(detail.Turnover, detail.LineTotal);
                                    winLossInfo.AddToGrandTotal(detail);
                                    Round(ref detail);
                                    winLossInfo.ChildInfo.Add(detail);
                                });

                                winLossInfo.GrandTotal.Margin = this.GetWinLossMargin(winLossInfo.GrandTotal.Turnover, winLossInfo.GrandTotal.LineTotal);
                                winLossByDate.ChildInfo.Add(winLossInfo);
                                winLossByDate.AddToGrandTotal(winLossInfo.GrandTotal);
                            });

                            winLossByDate.GrandTotal.Margin = this.GetWinLossMargin(winLossByDate.GrandTotal.Turnover, winLossByDate.GrandTotal.LineTotal);
                            result.ChildInfo.Add(winLossByDate);
                            result.AddToGrandTotal(winLossByDate.GrandTotal);
                        });

                    result.GrandTotal.Margin = this.GetWinLossMargin(result.GrandTotal.Turnover, result.GrandTotal.LineTotal);
                }

                SessionManager.Set<NumberGameWinLossDetail>(GamingConstants.NumberGameWinLossByDateSessionKey, result);
                return result;
            }
            catch (Exception ex)
            {
                HandleException(ex);
                throw;
            }
        }

        /// <summary>
        /// Gets the currencies.
        /// </summary>
        /// <returns>The currencies.</returns>
        public List<EntityBase<int, string>> GetCurrencies()
        {
            var result = new List<EntityBase<int, string>>();
            try
            {
                Dictionary<int, string> currList = ReportService.ExchangeCurrencies;

                foreach (var row in currList)
                {
                    if (ReportService.IsExcludeCurrency(row.Value))
                    {
                        continue;
                    }

                    result.Add(new EntityBase<int, string>()
                    {
                        Id = row.Key,
                        Name = row.Value
                    });
                }
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }

            return result;
        }

        /// <summary>
        /// Exports the number game by date.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        public void ExportNumberGameByDate(GetNumberGameWinLossByDateCriteria criteria)
        {
            try
            {
                var fullReport = SessionManager.Get<NumberGameWinLossDetail>(GamingConstants.NumberGameWinLossByDateSessionKey);
                if (fullReport == null)
                {
                    fullReport = this.GetNumberGameWinLossByDate(criteria);
                }

                ExcelFile excelFile = new ExcelFile();
                ExcelWorksheetCollection worksheets = excelFile.Worksheets;
                string sheetName = GamingResources.DirectBalance_SheetName;
                ExcelWorksheet workSheet = worksheets.Add(sheetName);

                int startRow = 0;
                int startColumn = 0;
                var currentRow = startRow;

                this.ExportReportHeader(
                    ref workSheet,
                    startRow,
                    startColumn,
                    ref currentRow,
                    criteria);

                this.ExportReportContent(
                    ref workSheet,
                    startRow,
                    startColumn,
                    ref currentRow,
                    fullReport);

                string reportName = "NumberGameWinlossByDate";
                excelFile.ReponseStream(reportName);
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }
        }

        private string GetWinLossMargin(decimal turnOver, decimal lineTotal)
        {
            if (turnOver == 0)
            {
                return Labels.NotAvailable;
            }

            return (lineTotal / turnOver).ToString();
        }

        /// <summary>
        /// Exports the report header.
        /// </summary>
        /// <param name="worksheet">The worksheet.</param>
        /// <param name="startRow">The start row.</param>
        /// <param name="startColumn">The start column.</param>
        /// <param name="currentRow">The current row.</param>
        /// <param name="criteria">The criteria.</param>
        [SuppressMessage("StyleCop.CSharp.Nexcel.NexcelCustomRules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        private void ExportReportHeader(
            ref ExcelWorksheet worksheet,
            int startRow,
            int startColumn,
            ref int currentRow,
            GetNumberGameWinLossByDateCriteria criteria)
        {
            try
            {
                var titleColor = ExcelExtensions.HeaderTitleColor;
                var headerColor = ExcelExtensions.HeaderTitleColor;
                worksheet.Columns[startColumn].Width = 140 * ExcelExtensions.FontWidth;
                worksheet.Columns[startColumn + 1].Width = 120 * ExcelExtensions.FontWidth;
                worksheet.Columns[startColumn + 2].Width = 120 * ExcelExtensions.FontWidth;
                worksheet.Columns[startColumn + 3].Width = 120 * ExcelExtensions.FontWidth;
                worksheet.Columns[startColumn + 4].Width = 120 * ExcelExtensions.FontWidth;
                worksheet.Columns[startColumn + 5].Width = 120 * ExcelExtensions.FontWidth;
                worksheet.Columns[startColumn + 6].Width = 120 * ExcelExtensions.FontWidth;
                worksheet.Columns[startColumn + 7].Width = 120 * ExcelExtensions.FontWidth;
                worksheet.Columns[startColumn + 8].Width = 120 * ExcelExtensions.FontWidth;
                var currentColumn = startColumn;
                CellRange currentCellRange;

                currentColumn = startColumn;
                var title = "Number Game Win Loss By Date - From " + criteria.FromDate + " To " + criteria.ToDate;
                currentColumn = startColumn;
                currentCellRange = worksheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow, currentColumn + 8);
                currentCellRange.Merged = true;
                currentCellRange.Value = title;
                currentCellRange.Style.FormatTitleCell(titleColor);
                currentRow++;

                // First Row
                currentColumn = startColumn;
                currentCellRange = worksheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow + 1, currentColumn);
                currentCellRange.Merged = true;
                currentCellRange.Value = Labels.Date;
                currentCellRange.Style.FormatTitleCell(headerColor);
                currentColumn++;

                currentCellRange = worksheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow + 1, currentColumn);
                currentCellRange.Merged = true;
                currentCellRange.Value = Labels.CustomerWinLoss;
                currentCellRange.Style.FormatTitleCell(headerColor);
                currentColumn++;

                currentCellRange = worksheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow + 1, currentColumn);
                currentCellRange.Merged = true;
                currentCellRange.Value = Labels.Agency_Win_Loss;
                currentCellRange.Style.FormatTitleCell(headerColor);
                currentColumn++;

                currentCellRange = worksheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow + 1, currentColumn);
                currentCellRange.Merged = true;
                currentCellRange.Value = Labels.Comm;
                currentCellRange.Style.FormatTitleCell(headerColor);
                currentColumn++;

                currentCellRange = worksheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow, currentColumn + 4);
                currentCellRange.Merged = true;
                currentCellRange.Value = Labels.Company;
                currentCellRange.Style.FormatTitleCell(headerColor);

                worksheet.Cells[currentRow + 1, currentColumn].Value = Labels.Turnover;
                worksheet.Cells[currentRow + 1, currentColumn].Style.FormatTitleCell(headerColor);
                currentColumn++;

                worksheet.Cells[currentRow + 1, currentColumn].Value = Labels.LineTotal;
                worksheet.Cells[currentRow + 1, currentColumn].Style.FormatTitleCell(headerColor);
                currentColumn++;

                worksheet.Cells[currentRow + 1, currentColumn].Value = Labels.VirtualTotal;
                worksheet.Cells[currentRow + 1, currentColumn].Style.FormatTitleCell(headerColor);
                currentColumn++;

                worksheet.Cells[currentRow + 1, currentColumn].Value = Labels.Variance;
                worksheet.Cells[currentRow + 1, currentColumn].Style.FormatTitleCell(headerColor);
                currentColumn++;

                worksheet.Cells[currentRow + 1, currentColumn].Value = Labels.Margin;
                worksheet.Cells[currentRow + 1, currentColumn].Style.FormatTitleCell(headerColor);

                currentRow += 2;
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }
        }

        /// <summary>
        /// Exports the content of the report.
        /// </summary>
        /// <param name="worksheet">The worksheet.</param>
        /// <param name="startRow">The start row.</param>
        /// <param name="startColumn">The start column.</param>
        /// <param name="currentRow">The current row.</param>
        /// <param name="report">The report.</param>
        private void ExportReportContent(
            ref ExcelWorksheet worksheet,
            int startRow,
            int startColumn,
            ref int currentRow,
            NumberGameWinLossDetail report)
        {
            try
            {
                if (report != null &&
               report.ChildInfo != null &&
               report.ChildInfo.Count > 0)
                {
                    var reportLength = report.ChildInfo.Count;

                    this.ExportDirectBalanceDetail(
                        ref worksheet,
                        startRow,
                        startColumn,
                        ref currentRow,
                        report.ChildInfo);

                    this.ExportDirectBalanceTotal(ref worksheet, startRow, startColumn, ref currentRow, report.GrandTotal);
                }
                else
                {
                    worksheet.ExportEmptyReport(startRow, startColumn, ref currentRow, startColumn + 8);
                }
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }
        }

        /// <summary>
        /// Exports the direct balance detail.
        /// </summary>
        /// <param name="worksheet">The worksheet.</param>
        /// <param name="startRow">The start row.</param>
        /// <param name="startColumn">The start column.</param>
        /// <param name="currentRow">The current row.</param>
        /// <param name="winLossDateList">The win loss date list.</param>
        [SuppressMessage("StyleCop.CSharp.Nexcel.NexcelCustomRules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        private void ExportDirectBalanceDetail(
            ref ExcelWorksheet worksheet,
            int startRow,
            int startColumn,
            ref int currentRow,
            List<NumberGameWinLossDetail> winLossDateList)
        {
            try
            {
                var textStyle = ExcelExtensions.GetCellTextStyle(ExcelExtensions.BackGroundColor, false);
                var textParentStyle = ExcelExtensions.GetCellTextStyle(ExcelExtensions.ParentColor, true);
                var textChildStyle = ExcelExtensions.GetCellTextStyle(ExcelExtensions.ChildColor, true);

                var twoDecimalPositiveNumberParentStyle = ExcelExtensions.GetCellNumberStyle(
                    NumberFormatConstants.TwoDecimalDigitFormat,
                    ExcelExtensions.ParentColor,
                    true,
                    true,
                    false);
                var twoDecimalNumberParentStyle = ExcelExtensions.GetCellNumberStyle(
                    NumberFormatConstants.TwoDecimalDigitFormat,
                    ExcelExtensions.ParentColor,
                    true,
                    false,
                    false);
                var twoDecimalNumberMarginParentStyle = ExcelExtensions.GetCellNumberStyle(
                    NumberFormatConstants.TwoDecimalDigitPercentFormat,
                    ExcelExtensions.ParentColor,
                    true,
                    true,
                    false);

                var twoDecimalPositiveNumberChildStyle = ExcelExtensions.GetCellNumberStyle(
                    NumberFormatConstants.TwoDecimalDigitFormat,
                    ExcelExtensions.ChildColor,
                    true,
                    true,
                    false);
                var twoDecimalNumberChildStyle = ExcelExtensions.GetCellNumberStyle(
                    NumberFormatConstants.TwoDecimalDigitFormat,
                    ExcelExtensions.ChildColor,
                    true,
                    false,
                    false);
                var twoDecimalNumberMarginChildStyle = ExcelExtensions.GetCellNumberStyle(
                    NumberFormatConstants.TwoDecimalDigitPercentFormat,
                    ExcelExtensions.ChildColor,
                    true,
                    true,
                    false);

                var twoDecimalNumberStyle = ExcelExtensions.GetCellNumberStyle(
                    NumberFormatConstants.TwoDecimalDigitFormat,
                    ExcelExtensions.BackGroundColor,
                    false,
                    false,
                    false);
                var twoDecimalNumberPositiveStyle = ExcelExtensions.GetCellNumberStyle(
                    NumberFormatConstants.TwoDecimalDigitFormat,
                    ExcelExtensions.BackGroundColor,
                    false,
                    true,
                    false);
                var twoDecimalNumberMarginStyle = ExcelExtensions.GetCellNumberStyle(
                    NumberFormatConstants.TwoDecimalDigitPercentFormat,
                    ExcelExtensions.BackGroundColor,
                    false,
                    true,
                    false);

                for (var k = 0; k < winLossDateList.Count; k++)
                {
                    var data = winLossDateList[k];
                    ExcelCell currCellRange;
                    var currentColumn = startColumn;
                    currCellRange = worksheet.Cells[currentRow, currentColumn];
                    currCellRange.Value = data.WinLossDate;
                    currCellRange.Style = textParentStyle;
                    currentColumn++;

                    currCellRange = worksheet.Cells[currentRow, currentColumn];
                    currCellRange.Value = data.GrandTotal.CustomerWinloss;
                    currCellRange.Style = twoDecimalPositiveNumberParentStyle;
                    currentColumn++;

                    currCellRange = worksheet.Cells[currentRow, currentColumn];
                    currCellRange.Value = data.GrandTotal.AgencyWinloss;
                    currCellRange.Style = twoDecimalNumberParentStyle;
                    currentColumn++;

                    currCellRange = worksheet.Cells[currentRow, currentColumn];
                    currCellRange.Value = data.GrandTotal.Comm;
                    currCellRange.Style = twoDecimalNumberParentStyle;
                    currentColumn++;

                    currCellRange = worksheet.Cells[currentRow, currentColumn];
                    currCellRange.Value = data.GrandTotal.Turnover;
                    currCellRange.Style = twoDecimalNumberParentStyle;
                    currentColumn++;

                    currCellRange = worksheet.Cells[currentRow, currentColumn];
                    currCellRange.Value = data.GrandTotal.LineTotal;
                    currCellRange.Style = twoDecimalPositiveNumberParentStyle;
                    currentColumn++;

                    currCellRange = worksheet.Cells[currentRow, currentColumn];
                    currCellRange.Value = data.GrandTotal.VirtualTotal;
                    currCellRange.Style = twoDecimalPositiveNumberParentStyle;
                    currentColumn++;

                    currCellRange = worksheet.Cells[currentRow, currentColumn];
                    currCellRange.Value = data.GrandTotal.Variance;
                    currCellRange.Style = twoDecimalPositiveNumberParentStyle;
                    currentColumn++;

                    currCellRange = worksheet.Cells[currentRow, currentColumn];
                    currCellRange.Value = Converter.ToDecimal(data.GrandTotal.Margin);
                    currCellRange.Style = twoDecimalNumberMarginParentStyle;
                    currentColumn++;

                    currentRow++;
                    var statusLength = data.ChildInfo.Count;
                    for (int i = statusLength - 1; i >= 0; i--)
                    {
                        currentColumn = startColumn;
                        var statusData = data.ChildInfo[i];
                        ExcelCell statusCellRange;
                        statusCellRange = worksheet.Cells[currentRow, currentColumn];
                        statusCellRange.Value = statusData.Status;
                        statusCellRange.Style = textChildStyle;
                        currentColumn++;

                        statusCellRange = worksheet.Cells[currentRow, currentColumn];
                        statusCellRange.Value = statusData.GrandTotal.CustomerWinloss;
                        statusCellRange.Style = twoDecimalPositiveNumberChildStyle;
                        currentColumn++;

                        statusCellRange = worksheet.Cells[currentRow, currentColumn];
                        statusCellRange.Value = statusData.GrandTotal.AgencyWinloss;
                        statusCellRange.Style = twoDecimalNumberChildStyle;
                        currentColumn++;

                        statusCellRange = worksheet.Cells[currentRow, currentColumn];
                        statusCellRange.Value = statusData.GrandTotal.Comm;
                        statusCellRange.Style = twoDecimalNumberChildStyle;
                        currentColumn++;

                        statusCellRange = worksheet.Cells[currentRow, currentColumn];
                        statusCellRange.Value = statusData.GrandTotal.Turnover;
                        statusCellRange.Style = twoDecimalNumberChildStyle;
                        currentColumn++;

                        statusCellRange = worksheet.Cells[currentRow, currentColumn];
                        statusCellRange.Value = statusData.GrandTotal.LineTotal;
                        statusCellRange.Style = twoDecimalPositiveNumberChildStyle;
                        currentColumn++;

                        statusCellRange = worksheet.Cells[currentRow, currentColumn];
                        statusCellRange.Value = statusData.GrandTotal.VirtualTotal;
                        statusCellRange.Style = twoDecimalPositiveNumberChildStyle;
                        currentColumn++;

                        statusCellRange = worksheet.Cells[currentRow, currentColumn];
                        statusCellRange.Value = statusData.GrandTotal.Variance;
                        statusCellRange.Style = twoDecimalPositiveNumberChildStyle;
                        currentColumn++;

                        statusCellRange = worksheet.Cells[currentRow, currentColumn];
                        statusCellRange.Value = Converter.ToDecimal(statusData.GrandTotal.Margin);
                        statusCellRange.Style = twoDecimalNumberMarginChildStyle;
                        currentColumn++;

                        currentRow++;
                        var detailLength = statusData.ChildInfo.Count;
                        for (int j = 0; j < detailLength; j++)
                        {
                            currentColumn = startColumn;
                            var detailData = statusData.ChildInfo[j];
                            ExcelCell detailCellRange;

                            detailCellRange = worksheet.Cells[currentRow, currentColumn];
                            detailCellRange.Value = detailData.Bettype;
                            detailCellRange.Style = textStyle;
                            currentColumn++;

                            detailCellRange = worksheet.Cells[currentRow, currentColumn];
                            detailCellRange.Value = detailData.CustomerWinloss;
                            detailCellRange.Style = twoDecimalNumberPositiveStyle;
                            currentColumn++;

                            detailCellRange = worksheet.Cells[currentRow, currentColumn];
                            detailCellRange.Value = detailData.AgencyWinloss;
                            detailCellRange.Style = twoDecimalNumberStyle;
                            currentColumn++;

                            detailCellRange = worksheet.Cells[currentRow, currentColumn];
                            detailCellRange.Value = detailData.Comm;
                            detailCellRange.Style = twoDecimalNumberStyle;
                            currentColumn++;

                            detailCellRange = worksheet.Cells[currentRow, currentColumn];
                            detailCellRange.Value = detailData.Turnover;
                            detailCellRange.Style = twoDecimalNumberStyle;
                            currentColumn++;

                            detailCellRange = worksheet.Cells[currentRow, currentColumn];
                            detailCellRange.Value = detailData.LineTotal;
                            detailCellRange.Style = twoDecimalNumberPositiveStyle;
                            currentColumn++;

                            detailCellRange = worksheet.Cells[currentRow, currentColumn];
                            detailCellRange.Value = detailData.VirtualTotal;
                            detailCellRange.Style = twoDecimalNumberPositiveStyle;
                            currentColumn++;

                            detailCellRange = worksheet.Cells[currentRow, currentColumn];
                            detailCellRange.Value = detailData.Variance;
                            detailCellRange.Style = twoDecimalNumberPositiveStyle;
                            currentColumn++;

                            detailCellRange = worksheet.Cells[currentRow, currentColumn];
                            detailCellRange.Value = Converter.ToDecimal(detailData.Margin);
                            detailCellRange.Style = twoDecimalNumberMarginStyle;
                            currentColumn++;

                            currentRow++;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }
        }

        /// <summary>
        /// Exports the direct balance total.
        /// </summary>
        /// <param name="worksheet">The worksheet.</param>
        /// <param name="startRow">The start row.</param>
        /// <param name="startColumn">The start column.</param>
        /// <param name="currentRow">The current row.</param>
        /// <param name="grandTotal">The grand total.</param>
        [SuppressMessage("StyleCop.CSharp.Nexcel.NexcelCustomRules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        private void ExportDirectBalanceTotal(
            ref ExcelWorksheet worksheet,
            int startRow,
            int startColumn,
            ref int currentRow,
            NumberGameWinLossDetail grandTotal)
        {
            try
            {
                var textGrandTotalStyle = ExcelExtensions.GetCellTextStyle(ExcelExtensions.GrandTotalRowColor, true);
                var twoDecimalPositiveNumberStyle = ExcelExtensions.GetCellNumberStyle(
                    NumberFormatConstants.TwoDecimalDigitFormat,
                    ExcelExtensions.GrandTotalRowColor,
                    true,
                    true,
                    false);
                var twoDecimalNumberStyle = ExcelExtensions.GetCellNumberStyle(
                    NumberFormatConstants.TwoDecimalDigitFormat,
                    ExcelExtensions.GrandTotalRowColor,
                    true,
                    false,
                    false);
                var twoDecimalNumberMarginStyle = ExcelExtensions.GetCellNumberStyle(
                    NumberFormatConstants.TwoDecimalDigitPercentFormat,
                    ExcelExtensions.GrandTotalRowColor,
                    true,
                    true,
                    false);

                var currentColumn = startColumn;
                ExcelCell grandCellRange;

                grandCellRange = worksheet.Cells[currentRow, currentColumn];
                grandCellRange.Value = "Grand Total";
                grandCellRange.Style = textGrandTotalStyle;
                currentColumn++;

                grandCellRange = worksheet.Cells[currentRow, currentColumn];
                grandCellRange.Value = grandTotal.CustomerWinloss;
                grandCellRange.Style = twoDecimalPositiveNumberStyle;
                currentColumn++;

                grandCellRange = worksheet.Cells[currentRow, currentColumn];
                grandCellRange.Value = grandTotal.AgencyWinloss;
                grandCellRange.Style = twoDecimalNumberStyle;
                currentColumn++;

                grandCellRange = worksheet.Cells[currentRow, currentColumn];
                grandCellRange.Value = grandTotal.Comm;
                grandCellRange.Style = twoDecimalNumberStyle;
                currentColumn++;

                grandCellRange = worksheet.Cells[currentRow, currentColumn];
                grandCellRange.Value = grandTotal.Turnover;
                grandCellRange.Style = twoDecimalNumberStyle;
                currentColumn++;

                grandCellRange = worksheet.Cells[currentRow, currentColumn];
                grandCellRange.Value = grandTotal.LineTotal;
                grandCellRange.Style = twoDecimalPositiveNumberStyle;
                currentColumn++;

                grandCellRange = worksheet.Cells[currentRow, currentColumn];
                grandCellRange.Value = grandTotal.VirtualTotal;
                grandCellRange.Style = twoDecimalPositiveNumberStyle;
                currentColumn++;

                grandCellRange = worksheet.Cells[currentRow, currentColumn];
                grandCellRange.Value = grandTotal.Variance;
                grandCellRange.Style = twoDecimalPositiveNumberStyle;
                currentColumn++;

                grandCellRange = worksheet.Cells[currentRow, currentColumn];
                grandCellRange.Value = Converter.ToDecimal(grandTotal.Margin);
                grandCellRange.Style = twoDecimalNumberMarginStyle;
                currentColumn++;
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }
        }
    }
}