// --------------------------------------------------------------------------
// <copyright file="BingoOutStandingService.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Louis.Nguyen" date="2012.12.07">Created</change>
// </history>

namespace Fanex.AccountingReports.Gaming.Services.Implement.BingoOutStanding
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using ExcelUtil.Spreadsheet;
    using Fanex.AccountingReports.Core.Extensions;
    using Fanex.AccountingReports.Core.Services.Impl;
    using Fanex.AccountingReports.Core.Web;
    using Fanex.AccountingReports.Gaming.Constants;
    using Fanex.AccountingReports.Gaming.Criteria.BingoOutStanding;
    using Fanex.AccountingReports.Gaming.Models.BingoOutStanding;
    using Fanex.AccountingReports.Gaming.Repositories.Implement;
    using Fanex.AccountingReports.Gaming.Repositories.Interface;
    using Fanex.AccountingReports.Gaming.Resources;
    using Fanex.AccountingReports.Gaming.Services.Interface;
    using Fanex.Common;
    using CoreConstants = Fanex.AccountingReports.Core.Constants;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class BingoOutStandingService : ServiceBase, IBingoOutStandingService
    {
        private readonly IBingoOutStandingRepository _reportRepository;

        public BingoOutStandingService()
            : this(null)
        {
        }

        public BingoOutStandingService(IBingoOutStandingRepository reportRepository)
        {
            this._reportRepository = reportRepository ?? new BingoOutStandingRepository();
        }

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
                var report = new WinLossInfo
                {
                    ChildsInfo = new List<WinLossInfo>(),
                    GrandTotal = new WinLossInfo()
                };

                Hashtable outputParams = new Hashtable();
                var rawData = this._reportRepository.GetReport(criteria, ref outputParams);
                if (rawData != null && rawData.Count >= 3)
                {
                    var winLossInfoTable = rawData[0] as List<dynamic>;
                    var grandTotalInfoTable = rawData[1];
                    var grandTotalRMInfoTable = rawData[2];
                    WinLossInfo info;
                    var companyGroups = winLossInfoTable.GroupBy(data => data.Company);
                    foreach (var companyGroup in companyGroups)
                    {
                        var companyWinLoss = new WinLossInfo
                        {
                            Company = Converter.ToString2(companyGroup.Key),
                            ChildsInfo = new List<WinLossInfo>()
                        };

                        foreach (var data in companyGroup)
                        {
                            info = new WinLossInfo
                            {
                                Name = Converter.ToString2(data.UserName),
                                CurrencyName = Converter.ToString2(data.Currency),
                                Company = Converter.ToString2(data.Company),
                                OutStanding = Converter.ToDecimal(data.OutStanding),
                                Date = ((DateTime)Converter.ToDateTime(data.Date)).ToShortFormatedDate()
                            };

                            info = WinLossInfo.Fortmat(info);
                            companyWinLoss.ChildsInfo.Add(info);
                        }

                        companyWinLoss.ChildsInfo = companyWinLoss.ChildsInfo.OrderBy(winLoss => winLoss.CurrencyName).ToList();
                        report.ChildsInfo.Add(companyWinLoss);
                    }

                    report.ChildsInfo = report.ChildsInfo.OrderBy(winLoss => winLoss.Company)
                                                         .ToList();
                    report.GrandTotal = this.GetReport_GrandTotal(
                        grandTotalInfoTable,
                        grandTotalRMInfoTable,
                        criteria);
                }

                report.ErrorCode = Converter.ToInt(outputParams["errcode"]);
                report.ErrorMessage = Converter.ToString2(outputParams["errmsg"]);
                SessionManager.Set<WinLossInfo>(GamingConstants.BingoOutstandingReportSessionKey, report);
                return report;
            }
            catch (Exception ex)
            {
                HandleException(ex);
                SessionManager.Set<WinLossInfo>(GamingConstants.BingoOutstandingReportSessionKey, null);
                throw;
            }
        }

        /// <summary>
        /// Gets the report grand total.
        /// </summary>
        /// <param name="grandTotalInfoTable">The grand total information table.</param>
        /// <param name="grandTotalRMInfoTable">The grand total RM information table.</param>
        /// <param name="criteria">The criteria.</param>
        /// <returns>The report grand total.</returns>
        public WinLossInfo GetReport_GrandTotal(
            List<dynamic> grandTotalInfoTable,
            List<dynamic> grandTotalRMInfoTable,
            GetReportCriteria criteria)
        {
            var grandTotal = new WinLossInfo
            {
                ChildsInfo = new List<WinLossInfo>(),
                CurrencyName = Labels.RM
            };

            grandTotal.Name = string.Format("{0} {1}", Labels.Grand_Total, grandTotal.CurrencyName);
            WinLossInfo info;
            var maxTotalIndex = grandTotalInfoTable.Count;
            for (var i = 0; i < maxTotalIndex; i++)
            {
                var data = grandTotalInfoTable[i];
                info = new WinLossInfo
                {
                    CurrencyName = Converter.ToString2(data.Currency),
                    OutStanding = Converter.ToDecimal(data.OutStanding)
                };

                info.Name = string.Format("{0} {1}", Labels.Total, Converter.ToString2(info.CurrencyName));
                info = WinLossInfo.Fortmat(info);
                grandTotal.ChildsInfo.Add(info);
            }

            maxTotalIndex = grandTotalRMInfoTable.Count;
            for (var i = 0; i < maxTotalIndex; i++)
            {
                var data = grandTotalRMInfoTable[i];
                info = new WinLossInfo
                {
                    OutStanding = Converter.ToDecimal(data.OutStanding)
                };

                grandTotal.Add(info);
            }

            grandTotal = WinLossInfo.Fortmat(grandTotal);
            return grandTotal;
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
                WinLossInfo report;
                if (criteria.IsChangedValue)
                {
                    report = this.GetReport(criteria);
                }
                else
                {
                    report = SessionManager.Get<WinLossInfo>(GamingConstants.BingoOutstandingReportSessionKey);
                }
                
                ExcelFile excelFile = new ExcelFile();
                ExcelWorksheetCollection worksheets = excelFile.Worksheets;
                string sheetName = GamingResources.BingoOutStanding_SheetName;
                ExcelWorksheet workSheet = worksheets.Add(sheetName);

                int startRow = 0;
                int startColumn = 0;
                int maxColumnIndex = 4;
                var currentRow = startRow;

                var currentColumn = startColumn;
                CellRange currentCellRange;
                ExcelCell currentCell;

                var backgroundColor = ExcelExtensions.BackGroundColor;
                var textStyle = ExcelExtensions.GetCellTextStyle(backgroundColor, false);
                var twoDecimalNegativeNumberStyle = ExcelExtensions.GetCellNumberStyle(
                    CoreConstants.NumberFormatConstants.TwoDecimalDigitFormat,
                    backgroundColor,
                    false,
                    false,
                    true);
                
                this.ExportReport_Header(
                    ref workSheet,
                    startRow,
                    startColumn,
                    ref currentRow,
                    PeriodTypeConstants.GamingBingoStatisticUsername,
                    maxColumnIndex,
                    criteria);

                var companyLength = report != null && report.ChildsInfo != null ?
                    report.ChildsInfo.Count :
                    0;
                if (companyLength > 0)
                {
                    WinLossInfo info;
                    var reportLength = 0;
                    for (var i = 0; i < companyLength; i++)
                    {
                        var companyWinLoss = report.ChildsInfo[i];
                        reportLength = companyWinLoss.ChildsInfo.Count;
                        for (var j = 0; j < reportLength; j++)
                        {
                            ////<NormalRow>

                            info = companyWinLoss.ChildsInfo[j];
                            currentColumn = startColumn;
                            if (j == 0)
                            {
                                currentCellRange = workSheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow + reportLength - 1, currentColumn);
                                currentCellRange.Merged = true;
                                currentCellRange.Value = companyWinLoss.Company;
                                currentCellRange.Style = textStyle;
                                currentCellRange.Style.HorizontalAlignment = HorizontalAlignmentStyle.Center;
                            }

                            currentColumn++;

                            currentCell = workSheet.Cells[currentRow, currentColumn];
                            currentCell.Style = textStyle;
                            currentCell.Style.HorizontalAlignment = HorizontalAlignmentStyle.Center;
                            currentCell.Value = info.CurrencyName;
                            currentColumn++;

                            currentCell = workSheet.Cells[currentRow, currentColumn];
                            currentCell.Style = textStyle;
                            currentCell.Style.HorizontalAlignment = HorizontalAlignmentStyle.Center;
                            currentCell.Value = info.Name;
                            currentColumn++;

                            currentCell = workSheet.Cells[currentRow, currentColumn];
                            currentCell.Style = textStyle;
                            currentCell.Style.HorizontalAlignment = HorizontalAlignmentStyle.Center;
                            currentCell.Value = info.Date;
                            currentColumn++;

                            currentCell = workSheet.Cells[currentRow, currentColumn];
                            currentCell.Style = twoDecimalNegativeNumberStyle;
                            currentCell.Value = info.OutStanding;
                            currentColumn++;
                            currentRow++;

                            ////</NormalRow>
                        }
                    }

                    this.ExportReport_GrandTotal(
                        report.GrandTotal,
                        ref workSheet,
                        startColumn,
                        ref currentRow);
                }
                else
                {
                    currentCellRange = workSheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow, startColumn + maxColumnIndex);
                    currentCellRange.Merged = true;
                    currentCellRange.Value = Labels.There_is_no_data;
                    currentCellRange.Style = textStyle;
                    currentCellRange.Style.HorizontalAlignment = HorizontalAlignmentStyle.Center;
                    currentCellRange.Style.Font.Weight = ExcelFont.BoldWeight;
                }

                var reportName = GamingResources.BingoOutStanding_ReportName;
                excelFile.ReponseStream(reportName);
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }
        }

        /// <summary>
        /// Exports the report header.
        /// </summary>
        /// <param name="workSheet">The work sheet.</param>
        /// <param name="startRow">The start row.</param>
        /// <param name="startColumn">The start column.</param>
        /// <param name="currentRow">The current row.</param>
        /// <param name="periodType">Type of the period.</param>
        /// <param name="maxColumnIndex">Maximum index of the column.</param>
        /// <param name="criteria">The criteria.</param>
        [SuppressMessage("StyleCop.CSharp.Nexcel.NexcelCustomRules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        private void ExportReport_Header(
            ref ExcelWorksheet workSheet,
            int startRow,
            int startColumn,
            ref int currentRow,
            int periodType,
            int maxColumnIndex,
            GetReportCriteria criteria)
        {
            var currentColumn = startColumn;
            CellRange currentCellRange;
            ExcelCell currentCell;
            var headerTextStyle = ExcelExtensions.GetCellTitleStyle(ExcelExtensions.HeaderTitleColor);
            
            for (int colIndex = startColumn; colIndex <= maxColumnIndex; colIndex++)
            {
                workSheet.Columns[startColumn + colIndex].Width = 140 * ExcelExtensions.FontWidth;
            }

            workSheet.Columns[startColumn + 2].Width = 200 * ExcelExtensions.FontWidth;

            var title = GamingResources.BingoOutStanding_ReportTitle;
            currentColumn = startColumn;
            currentCellRange = workSheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow + 1, currentColumn + maxColumnIndex);
            currentCellRange.Merged = true;
            currentCellRange.Value = title;
            currentCellRange.Style = headerTextStyle;
            currentRow += 2;

            workSheet.ExportEmptyRow(startRow, startColumn, ref currentRow, startColumn + maxColumnIndex);

            currentColumn = startColumn;
            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Style = headerTextStyle;
            currentCell.Value = Labels.Company;
            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Style = headerTextStyle;
            currentCell.Value = Labels.Currency;
            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Style = headerTextStyle;
            currentCell.Value = Labels.User_Name;
            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Style = headerTextStyle;
            currentCell.Value = Labels.Date;
            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Style = headerTextStyle;
            currentCell.Value = Labels.Outstanding;
            currentColumn++;
            currentRow++;
        }

        /// <summary>
        /// Exports the report grand total.
        /// </summary>
        /// <param name="grandTotal">The grand total.</param>
        /// <param name="workSheet">The work sheet.</param>
        /// <param name="startColumn">The start column.</param>
        /// <param name="currentRow">The current row.</param>
        [SuppressMessage("StyleCop.CSharp.Nexcel.NexcelCustomRules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        private void ExportReport_GrandTotal(
            WinLossInfo grandTotal,
            ref ExcelWorksheet workSheet,
            int startColumn,
            ref int currentRow)
        {
            var grandTotalColor = ExcelExtensions.GrandTotalRowColor;
            var textStyle_GrandTotal = ExcelExtensions.GetCellTextStyle(grandTotalColor, true);
            var twoDecimalNegativeNumberStyle_GrandTotal = ExcelExtensions.GetCellNumberStyle(
                CoreConstants.NumberFormatConstants.TwoDecimalDigitFormat,
                grandTotalColor,
                true,
                false,
                true);

            var grandTotalSeparatorColor = ExcelExtensions.GrandTotalSeparatorColor;
            var textStyle_GrandTotal_1stRow = ExcelExtensions.GetCellTextStyle(grandTotalColor, true);
            var twoDecimalNegativeNumberStyle_GrandTotal_1stRow = ExcelExtensions.GetCellNumberStyle(
                CoreConstants.NumberFormatConstants.TwoDecimalDigitFormat,
                grandTotalColor,
                true,
                false,
                true);
            textStyle_GrandTotal_1stRow.Borders.SetBorders(MultipleBorders.Top, grandTotalSeparatorColor, LineStyle.Medium);
            twoDecimalNegativeNumberStyle_GrandTotal_1stRow.Borders.SetBorders(MultipleBorders.Top, grandTotalSeparatorColor, LineStyle.Medium);

            var currentColumn = startColumn;
            ExcelCell currentCell;
            CellRange currentCellRange;
            
            WinLossInfo info;
            var reportLength = grandTotal.ChildsInfo.Count;
            var isGrandTotalFirstRow = true;
            for (var i = 0; i < reportLength; i++)
            {
                ////GrandTotal_BySpecificCurrency

                info = grandTotal.ChildsInfo[i];
                currentColumn = startColumn;
                currentCell = workSheet.Cells[currentRow, currentColumn];
                currentCell.Value = info.Name;
                currentCell.Style = isGrandTotalFirstRow ?
                    textStyle_GrandTotal_1stRow :
                    textStyle_GrandTotal;
                currentCell.Style.HorizontalAlignment = HorizontalAlignmentStyle.Center;
                currentColumn++;

                currentCell = workSheet.Cells[currentRow, currentColumn];
                currentCell.Style = isGrandTotalFirstRow ?
                    textStyle_GrandTotal_1stRow :
                    textStyle_GrandTotal;
                currentCell.Style.HorizontalAlignment = HorizontalAlignmentStyle.Center;
                currentCell.Value = info.CurrencyName;
                currentColumn++;

                currentCellRange = workSheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow, currentColumn + 1);
                currentCellRange.Merged = true;
                currentCellRange.Style = isGrandTotalFirstRow ?
                    textStyle_GrandTotal_1stRow :
                    textStyle_GrandTotal;
                currentCellRange.Style.HorizontalAlignment = HorizontalAlignmentStyle.Center;
                currentCellRange.Value = string.Empty;
                currentColumn += 2;

                currentCell = workSheet.Cells[currentRow, currentColumn];
                currentCell.Style = isGrandTotalFirstRow ?
                    twoDecimalNegativeNumberStyle_GrandTotal_1stRow :
                    twoDecimalNegativeNumberStyle_GrandTotal;
                currentCell.Value = info.OutStanding;
                currentColumn++;
                currentRow++;

                if (isGrandTotalFirstRow)
                {
                    isGrandTotalFirstRow = false;
                }
            }

            ////GrandTotal
            info = grandTotal;
            currentColumn = startColumn;
            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Value = info.Name;
            currentCell.Style = textStyle_GrandTotal;
            currentCell.Style.HorizontalAlignment = HorizontalAlignmentStyle.Center;
            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Style = textStyle_GrandTotal;
            currentCell.Value = info.CurrencyName;
            currentCell.Style.HorizontalAlignment = HorizontalAlignmentStyle.Center;
            currentColumn++;

            currentCellRange = workSheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow, currentColumn + 1);
            currentCellRange.Merged = true;
            currentCellRange.Style = textStyle_GrandTotal;
            currentCellRange.Style.HorizontalAlignment = HorizontalAlignmentStyle.Center;
            currentCellRange.Value = string.Empty;
            currentColumn += 2;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Style = twoDecimalNegativeNumberStyle_GrandTotal;
            currentCell.Value = info.OutStanding;
            currentColumn++;
            currentRow++;
        }
    }
}
