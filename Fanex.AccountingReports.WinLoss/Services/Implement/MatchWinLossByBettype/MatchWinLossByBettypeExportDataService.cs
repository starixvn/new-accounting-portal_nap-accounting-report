// -----------------------------------------------------------------------
// <copyright file="MatchWinLossByBettypeExportDataService.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
// <history>
//   <change who="Franklin.Nguyen" date="2013.12.16">Created</change>
// </history>

namespace Fanex.AccountingReports.WinLoss.Services.Implement.MatchWinLossByBettype
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Drawing;
    using ExcelUtil.Spreadsheet;
    using Fanex.AccountingReports.Core.Constants;
    using Fanex.AccountingReports.Core.Extensions;
    using Fanex.AccountingReports.Core.Web;
    using Fanex.AccountingReports.WinLoss.Criteria.MatchWinLossByBettype;
    using Fanex.AccountingReports.WinLoss.Models.MatchWinLossByBettype;
    using Fanex.AccountingReports.WinLoss.Resources;
    using Fanex.AccountingReports.WinLoss.Services.Interface.MatchWinLossByBettype;
    using Fanex.NAP.Core.Service.Impl;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class MatchWinLossByBettypeExportDataService : BaseService, IMatchWinLossByBettypeExportDataService
    {
        private string _matchWinLossByBettypeReportSessionKey = "Fanex.AccountingReports.WinLoss.MatchWinLossByBettype." +
           (AppSession.CurrentUser != null ? AppSession.CurrentUser.Username : string.Empty);

        private IMatchWinLossByBettypeService _matchWinLossByBettypeService;

        public MatchWinLossByBettypeExportDataService()
        {
            this._matchWinLossByBettypeService = new MatchWinLossByBettypeService();
        }

        /// <summary>
        /// Exports the match win loss by bet type report.
        /// </summary>
        /// <param name="getMatchWinLossByBettypeCriteria">The get match win loss by bet type criteria.</param>
        public void ExportMatchWinLossByBettypeReport(MatchWinLossByBettypeCriteria getMatchWinLossByBettypeCriteria)
        {
            try
            {
                var fullReport = SessionManager.Get<MatchWinLossByBettype>(_matchWinLossByBettypeReportSessionKey);
                if (fullReport == null)
                {
                    fullReport = _matchWinLossByBettypeService.GetMatchWinLossByBettype(getMatchWinLossByBettypeCriteria);
                }

                ExcelFile excelFile = new ExcelFile();
                ExcelWorksheetCollection worksheets = excelFile.Worksheets;
                string sheetName = WinLossResources.MatchWinLossByBettype_SheetName;
                ExcelWorksheet workSheet = worksheets.Add(sheetName);

                int startRow = 0;
                int startColumn = 0;
                var currentRow = startRow;

                this.ExportReportHeader(
                    ref workSheet,
                    startRow,
                    startColumn,
                    ref currentRow,
                    getMatchWinLossByBettypeCriteria);

                this.ExportReportContent(
                    ref workSheet,
                    startRow,
                    startColumn,
                    ref currentRow,
                    fullReport);

                string reportName = string.Format(
                WinLossResources.MatchWinLossByBettype_ReportName,
                getMatchWinLossByBettypeCriteria.FromDate.ToShortFormatedDateWithSlash().Replace('/', '_'),
                getMatchWinLossByBettypeCriteria.ToDate.ToShortFormatedDateWithSlash().Replace('/', '_'));
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
        /// <param name="getMatchWinLossByBettypeCriteria">The get match win loss by bet type criteria.</param>
        [SuppressMessage("StyleCop.CSharp.Nexcel.NexcelCustomRules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        private void ExportReportHeader(
            ref ExcelWorksheet workSheet,
            int startRow,
            int startColumn,
            ref int currentRow,
            MatchWinLossByBettypeCriteria getMatchWinLossByBettypeCriteria)
        {
            try
            {
                var titleColor = ExcelExtensions.HeaderTitleColor;
                var headerColor = ExcelExtensions.HeaderTitleColor;
                workSheet.Columns[startColumn].Width = 80 * ExcelExtensions.FontWidth;
                workSheet.Columns[startColumn + 1].Width = 250 * ExcelExtensions.FontWidth;
                workSheet.Columns[startColumn + 2].Width = 500 * ExcelExtensions.FontWidth;
                workSheet.Columns[startColumn + 3].Width = 250 * ExcelExtensions.FontWidth;
                var currentColumn = startColumn;
                CellRange currentCellRange;

                currentColumn = startColumn;
                var title = string.Format(
                                        WinLossResources.MatchWinLossByBettype_ReportTitle,
                                        getMatchWinLossByBettypeCriteria.FromDate.ToShortFormatedDate(),
                                        getMatchWinLossByBettypeCriteria.ToDate.ToShortFormatedDate());
                currentColumn = startColumn;
                currentCellRange = workSheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow, currentColumn + 3);
                currentCellRange.Merged = true;
                currentCellRange.Value = title;
                currentCellRange.Style.FormatTitleCell(titleColor);
                currentRow++;

                // First Row
                currentColumn = startColumn;
                currentCellRange = workSheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow + 1, currentColumn);
                currentCellRange.Merged = true;
                currentCellRange.Value = Labels.No;
                currentCellRange.Style.FormatTitleCell(headerColor);
                currentColumn++;

                currentCellRange = workSheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow + 1, currentColumn);
                currentCellRange.Merged = true;
                currentCellRange.Value = Labels.Bettype;
                currentCellRange.Style.FormatTitleCell(headerColor);
                currentColumn++;

                currentCellRange = workSheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow + 1, currentColumn);
                currentCellRange.Merged = true;
                currentCellRange.Value = Labels.Match;
                currentCellRange.Style.FormatTitleCell(headerColor);
                currentColumn++;

                currentCellRange = workSheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow + 1, currentColumn);
                currentCellRange.Merged = true;
                currentCellRange.Value = Labels.Base_Currency;
                currentCellRange.Style.FormatTitleCell(headerColor);
                currentColumn++;

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
        /// <param name="workSheet">The work sheet.</param>
        /// <param name="startRow">The start row.</param>
        /// <param name="startColumn">The start column.</param>
        /// <param name="currentRow">The current row.</param>
        /// <param name="report">The report.</param>
        private void ExportReportContent(
            ref ExcelWorksheet workSheet,
            int startRow,
            int startColumn,
            ref int currentRow,
            MatchWinLossByBettype report)
        {
            try
            {
                if (report != null &&
               report.ReportRows != null &&
               report.ReportRows.Count > 0)
                {
                    var reportLength = report.ReportRows.Count;

                    for (int rowIndex = 0; rowIndex < reportLength; rowIndex++)
                    {
                        this.ExportReportDetail(
                            ref workSheet,
                            rowIndex + 1,
                            startRow,
                            startColumn,
                            ref currentRow,
                            report.ReportRows[rowIndex]);
                    }

                    this.ExportFooterReport(ref workSheet, startRow, startColumn, ref currentRow, report);
                }
                else
                {
                    workSheet.ExportEmptyReport(startRow, startColumn, ref currentRow, startColumn + 6);
                }
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }
        }

        /// <summary>
        /// Exports the report detail.
        /// </summary>
        /// <param name="workSheet">The work sheet.</param>
        /// <param name="rowIndex">Index of the row.</param>
        /// <param name="startRow">The start row.</param>
        /// <param name="startColumn">The start column.</param>
        /// <param name="currentRow">The current row.</param>
        /// <param name="detail">The detail.</param>
        private void ExportReportDetail(
            ref ExcelWorksheet workSheet,
            int rowIndex,
            int startRow,
            int startColumn,
            ref int currentRow,
            MatchWinLossByBettype detail)
        {
            ExcelCell currentCell;
            var textStyle = ExcelExtensions.GetCellTextStyle(ExcelExtensions.BackGroundColor, false);
            var textBoldStyle = ExcelExtensions.GetCellTextStyle(ExcelExtensions.BackGroundColor, true);
            var intStyle = ExcelExtensions.GetCellNumberStyle(string.Empty, ExcelExtensions.BackGroundColor, false, false, false);
            var twoDecimalNumberStyle = ExcelExtensions.GetCellNumberStyle(
                NumberFormatConstants.TwoDecimalDigitFormat,
                ExcelExtensions.BackGroundColor,
                true,
                false,
                false);

            var currentColumn = startColumn;
            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Value = rowIndex;
            currentCell.Style = intStyle;
            currentCell.Style.HorizontalAlignment = HorizontalAlignmentStyle.Center;
            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            if (detail.IsLive)
            {
                var bettype = string.Format("{0}{1} - {2}", "!", Labels.Live, detail.Bettype);
                currentCell.Value = bettype;
                currentCell.Style = textStyle;
                ////currentCell.Style.Font.Color = Color.Red;
            }
            else
            {
                currentCell.Value = detail.Bettype;
                currentCell.Style = textStyle;
            }

            currentCell.Style.HorizontalAlignment = HorizontalAlignmentStyle.Center;
            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            var match = string.Format("{0} {1} {2}", detail.HomeTeam, "-vs-", detail.AwayTeam);
            currentCell.Value = match;
            currentCell.Style = textStyle;
            currentCell.Style.HorizontalAlignment = HorizontalAlignmentStyle.Center;
            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Value = detail.WinLoss;
            currentCell.Style = twoDecimalNumberStyle;
            currentColumn++;
            currentRow++;
        }

        /// <summary>
        /// Exports the footer report.
        /// </summary>
        /// <param name="worksheet">The worksheet.</param>
        /// <param name="startRow">The start row.</param>
        /// <param name="startColumn">The start column.</param>
        /// <param name="currentRow">The current row.</param>
        /// <param name="grandTotal">The grand total.</param>
        private void ExportFooterReport(
            ref ExcelWorksheet worksheet,
            int startRow,
            int startColumn,
            ref int currentRow,
            MatchWinLossByBettype grandTotal)
        {
            ExcelCell currentCell;
            var textStyle = ExcelExtensions.GetCellTextStyle(ExcelExtensions.GrandTotalRowColor, true);
            textStyle.Borders.SetBorders(MultipleBorders.Top, Color.FromArgb(255, 102, 0), LineStyle.Medium);
            var twoDecimalNumberStyle = ExcelExtensions.GetCellNumberStyle(
                NumberFormatConstants.TwoDecimalDigitFormat,
                ExcelExtensions.GrandTotalRowColor,
                true,
                false,
                false);
            twoDecimalNumberStyle.HorizontalAlignment = HorizontalAlignmentStyle.Right;
            twoDecimalNumberStyle.Borders.SetBorders(MultipleBorders.Top, Color.FromArgb(255, 102, 0), LineStyle.Medium);
            CellRange currentCellRange;
            currentCellRange = worksheet.Cells.GetSubrangeAbsolute(currentRow, startColumn, currentRow, startColumn + 2);
            currentCellRange.Merged = true;
            currentCellRange.Value = Labels.Grand_Total;
            currentCellRange.Style = textStyle;
            currentCellRange.Style.HorizontalAlignment = HorizontalAlignmentStyle.Center;

            currentCell = worksheet.Cells[currentRow, startColumn + 3];
            currentCell.Value = grandTotal.TotalWinLoss;
            currentCell.Style = twoDecimalNumberStyle;
        }
    }
}