namespace Fanex.AccountingReports.Gaming.Services.Implement.NumberGameFinancial
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Drawing;
    using System.Linq;
    using ExcelUtil.Spreadsheet;
    using Fanex.AccountingReports.Core.Constants;
    using Fanex.AccountingReports.Core.Extensions;
    using Fanex.AccountingReports.Core.Services.Impl;
    using Fanex.AccountingReports.Core.Web;
    using Fanex.AccountingReports.Gaming.Criteria.NumberGameFinancial;
    using Fanex.AccountingReports.Gaming.Models.NumberGameFinancial;
    using Fanex.AccountingReports.Gaming.Resources;
    using Fanex.AccountingReports.Gaming.Services.Interface.NumberGameFinancial;
    using Fanex.Common;

    /// <summary>
    /// Export Number Game Financial Service.
    /// </summary>
    public class NumberGameFinancialExportDataService : ServiceBase, INumberGameFinancialExportDataService
    {
        private string _financialReportSessionKey = "Fanex.AccountingReports.Gaming.FinancialReport." +
           (AppSession.CurrentUser != null ? AppSession.CurrentUser.Username : string.Empty);

        private INumberGameFinancialService _numberGameFinancialService;

        public NumberGameFinancialExportDataService()
        {
            this._numberGameFinancialService = new NumberGameFinancialService();
        }

        /// <summary>
        /// Exports the number game financial report.
        /// </summary>
        /// <param name="getNumberGameFinancialCriteria">The get number game financial criteria.</param>
        public void ExportNumberGameFinancial(GetNumberGameFinancialCriteria getNumberGameFinancialCriteria)
        {
            try
            {
                var fullReport = SessionManager.Get<NumberGameFinancial>(_financialReportSessionKey);
                if (fullReport == null)
                {
                    fullReport = _numberGameFinancialService.GetNumberGameFinancial(getNumberGameFinancialCriteria);
                }

                ExcelFile excelFile = new ExcelFile();
                ExcelWorksheetCollection worksheets = excelFile.Worksheets;
                string sheetName = GamingResources.NumberGameFinancial_SheetName;
                ExcelWorksheet workSheet = worksheets.Add(sheetName);

                int startRow = 0;
                int startColumn = 0;
                var currentRow = startRow;

                this.ExportReportHeader(
                    ref workSheet,
                    startRow,
                    startColumn,
                    ref currentRow,
                    getNumberGameFinancialCriteria);

                this.ExportReportContent(
                    ref workSheet,
                    startRow,
                    startColumn,
                    ref currentRow,
                    fullReport);

                string reportName = string.Format(
                GamingResources.NumberGameFinancial_ReportName,
                getNumberGameFinancialCriteria.FromDate.ToShortFormatedDateWithSlash().Replace('/', '_'),
                getNumberGameFinancialCriteria.ToDate.ToShortFormatedDateWithSlash().Replace('/', '_'));
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
        /// <param name="worksheet">The worksheet.</param>
        /// <param name="startRow">The start row.</param>
        /// <param name="startColumn">The start column.</param>
        /// <param name="currentRow">The current row.</param>
        /// <param name="getNumberGameFinancialCriteria">The get number game financial criteria.</param>
        [SuppressMessage("StyleCop.CSharp.StylecopPlus.Rules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        private void ExportReportHeader(
            ref ExcelWorksheet worksheet,
            int startRow,
            int startColumn,
            ref int currentRow,
            GetNumberGameFinancialCriteria getNumberGameFinancialCriteria)
        {
            try
            {
                var titleColor = ExcelExtensions.HeaderTitleColor;
                var headerColor = ExcelExtensions.HeaderTitleColor;
                worksheet.Columns[startColumn].Width = 200 * ExcelExtensions.FontWidth;
                worksheet.Columns[startColumn + 1].Width = 120 * ExcelExtensions.FontWidth;
                worksheet.Columns[startColumn + 2].Width = 120 * ExcelExtensions.FontWidth;
                worksheet.Columns[startColumn + 3].Width = 150 * ExcelExtensions.FontWidth;
                worksheet.Columns[startColumn + 4].Width = 150 * ExcelExtensions.FontWidth;
                worksheet.Columns[startColumn + 5].Width = 150 * ExcelExtensions.FontWidth;
                worksheet.Columns[startColumn + 6].Width = 150 * ExcelExtensions.FontWidth;
                var currentColumn = startColumn;
                CellRange currentCellRange;

                currentColumn = startColumn;
                var title = string.Format(
                                        GamingResources.NumberGameFinancial_ReportTitle,
                                        getNumberGameFinancialCriteria.FromDate.ToShortFormatedDate(),
                                        getNumberGameFinancialCriteria.ToDate.ToShortFormatedDate());
                currentColumn = startColumn;
                currentCellRange = worksheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow, currentColumn + 6);
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
                currentCellRange.Value = Labels.No_Players;
                currentCellRange.Style.FormatTitleCell(headerColor);
                currentColumn++;

                currentCellRange = worksheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow + 1, currentColumn);
                currentCellRange.Merged = true;
                currentCellRange.Value = Labels.No_Games;
                currentCellRange.Style.FormatTitleCell(headerColor);
                currentColumn++;

                currentCellRange = worksheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow + 1, currentColumn);
                currentCellRange.Merged = true;
                currentCellRange.Value = Labels.Total_Bets;
                currentCellRange.Style.FormatTitleCell(headerColor);
                currentColumn++;

                currentCellRange = worksheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow + 1, currentColumn);
                currentCellRange.Merged = true;
                currentCellRange.Value = Labels.Total_Wins;
                currentCellRange.Style.FormatTitleCell(headerColor);
                currentColumn++;

                currentCellRange = worksheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow + 1, currentColumn);
                currentCellRange.Merged = true;
                currentCellRange.Value = Labels.Game_income;
                currentCellRange.Style.FormatTitleCell(headerColor);
                currentColumn++;

                currentCellRange = worksheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow + 1, currentColumn);
                currentCellRange.Merged = true;
                currentCellRange.Value = Labels.Payout;
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
            NumberGameFinancial report)
        {
            try
            {
                if (report != null &&
               report.ReportRows != null &&
               report.ReportRows.Count > 0)
                {
                    var reportLength = report.ReportRows.Count;

                    this.ExportReportDetail(
                        ref worksheet,
                        startRow,
                        startColumn,
                        ref currentRow,
                        report.ReportRows);

                    this.ExportFooterReport(ref worksheet, startRow, startColumn, ref currentRow, report);
                }
                else
                {
                    worksheet.ExportEmptyReport(startRow, startColumn, ref currentRow, startColumn + 6);
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
        /// <param name="worksheet">The worksheet.</param>
        /// <param name="startRow">The start row.</param>
        /// <param name="startColumn">The start column.</param>
        /// <param name="currentRow">The current row.</param>
        /// <param name="reportDateList">The report date list.</param>
        [SuppressMessage("StyleCop.CSharp.StylecopPlus.Rules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        private void ExportReportDetail(
           ref ExcelWorksheet worksheet,
           int startRow,
           int startColumn,
           ref int currentRow,
           List<NumberGameFinancial> reportDateList)
        {
            try
            {
                var parentColor = ExcelExtensions.SubTotalRowColor;
                var childColor = ExcelExtensions.EvenRowColor;
                var backgroundColor = ExcelExtensions.BackGroundColor;

                var textStyle = ExcelExtensions.GetCellTextStyle(backgroundColor, false);
                var textParentStyle = ExcelExtensions.GetCellTextStyle(parentColor, true);
                var textChildStyle = ExcelExtensions.GetCellTextStyle(childColor, true);

                var intNumberParentStyle = ExcelExtensions.GetCellNumberStyle(
                    NumberFormatConstants.IntNumberFormat,
                    parentColor,
                    true,
                    false,
                    false);

                var intNumberChildStyle = ExcelExtensions.GetCellNumberStyle(
                    NumberFormatConstants.IntNumberFormat,
                    childColor,
                    true,
                    false,
                    false);

                var intNumberStyle = ExcelExtensions.GetCellNumberStyle(
                    NumberFormatConstants.IntNumberFormat,
                    backgroundColor,
                    false,
                    false,
                    false);

                var twoDecimalNegativeNumberParentStyle = ExcelExtensions.GetCellNumberStyle(
                    NumberFormatConstants.TwoDecimalDigitFormat,
                    parentColor,
                    true,
                    false,
                    true);
                var twoDecimalNegativeNumberChildStyle = ExcelExtensions.GetCellNumberStyle(
                     NumberFormatConstants.TwoDecimalDigitFormat,
                     childColor,
                     true,
                     false,
                     true);
                var twoDecimalNegativeNumberStyle = ExcelExtensions.GetCellNumberStyle(
                    NumberFormatConstants.TwoDecimalDigitFormat,
                    backgroundColor,
                    false,
                    false,
                    true);
                var twoDecimalNumberParentStyle = ExcelExtensions.GetCellNumberStyle(
                    NumberFormatConstants.TwoDecimalDigitFormat,
                    parentColor,
                    true,
                    false,
                    false);
                var twoDecimalNumberPayoutParentStyle = ExcelExtensions.GetCellNumberStyle(
                    NumberFormatConstants.TwoDecimalDigitPercentFormat,
                    parentColor,
                    true,
                    false,
                    false);
                var twoDecimalNumberChildStyle = ExcelExtensions.GetCellNumberStyle(
                    NumberFormatConstants.TwoDecimalDigitFormat,
                    childColor,
                    true,
                    false,
                    false);
                var twoDecimalNumberPayoutChildStyle = ExcelExtensions.GetCellNumberStyle(
                    NumberFormatConstants.TwoDecimalDigitPercentFormat,
                    childColor,
                    true,
                    false,
                    false);

                var twoDecimalNumberStyle = ExcelExtensions.GetCellNumberStyle(
                    NumberFormatConstants.TwoDecimalDigitFormat,
                    backgroundColor,
                    false,
                    false,
                    false);
                var twoDecimalNumberPayoutStyle = ExcelExtensions.GetCellNumberStyle(
                    NumberFormatConstants.TwoDecimalDigitPercentFormat,
                    backgroundColor,
                    false,
                    false,
                    false);

                for (var k = 0; k < reportDateList.Count; k++)
                {
                    var data = reportDateList[k];
                    ExcelCell currCellRange;
                    var currentColumn = startColumn;
                    currCellRange = worksheet.Cells[currentRow, currentColumn];
                    currCellRange.Value = data.ReportDate;
                    currCellRange.Style = textParentStyle;
                    currentColumn++;

                    currCellRange = worksheet.Cells[currentRow, currentColumn];
                    currCellRange.Value = data.NumberPlayer;
                    currCellRange.Style = intNumberParentStyle;
                    currentColumn++;

                    currCellRange = worksheet.Cells[currentRow, currentColumn];
                    currCellRange.Value = data.NumberGame;
                    currCellRange.Style = intNumberParentStyle;
                    currentColumn++;

                    currCellRange = worksheet.Cells[currentRow, currentColumn];
                    currCellRange.Value = data.TotalBet;
                    currCellRange.Style = twoDecimalNumberParentStyle;
                    currentColumn++;

                    currCellRange = worksheet.Cells[currentRow, currentColumn];
                    currCellRange.Value = data.TotalWin;
                    currCellRange.Style = twoDecimalNumberParentStyle;
                    currentColumn++;

                    currCellRange = worksheet.Cells[currentRow, currentColumn];
                    currCellRange.Value = data.GameIncome;
                    currCellRange.Style = twoDecimalNegativeNumberParentStyle;
                    currentColumn++;

                    currCellRange = worksheet.Cells[currentRow, currentColumn];
                    var datePayoutValue = Converter.ToDecimal(data.Payout);
                    currCellRange.Value = datePayoutValue;
                    currCellRange.Style = twoDecimalNumberPayoutParentStyle;
                    if (datePayoutValue >= 1)
                    {
                        currCellRange.Style.Font.Color = Color.Red;
                    }

                    currentColumn++;

                    currentRow++;
                    var statusLength = data.ReportRows.Count;
                    for (int i = 0; i < statusLength; i++)
                    {
                        var paddingStatusName = "    ";
                        currentColumn = startColumn;
                        var statusData = data.ReportRows[i];
                        ExcelCell statusCellRange;
                        statusCellRange = worksheet.Cells[currentRow, currentColumn];
                        statusCellRange.Value = paddingStatusName + statusData.Status;
                        statusCellRange.Style = textChildStyle;
                        currentColumn++;

                        statusCellRange = worksheet.Cells[currentRow, currentColumn];
                        statusCellRange.Value = statusData.NumberPlayer;
                        statusCellRange.Style = intNumberChildStyle;
                        currentColumn++;

                        statusCellRange = worksheet.Cells[currentRow, currentColumn];
                        statusCellRange.Value = statusData.NumberGame;
                        statusCellRange.Style = intNumberChildStyle;
                        currentColumn++;

                        statusCellRange = worksheet.Cells[currentRow, currentColumn];
                        statusCellRange.Value = statusData.TotalBet;
                        statusCellRange.Style = twoDecimalNumberChildStyle;
                        currentColumn++;

                        statusCellRange = worksheet.Cells[currentRow, currentColumn];
                        statusCellRange.Value = statusData.TotalWin;
                        statusCellRange.Style = twoDecimalNumberChildStyle;
                        currentColumn++;

                        statusCellRange = worksheet.Cells[currentRow, currentColumn];
                        statusCellRange.Value = statusData.GameIncome;
                        statusCellRange.Style = twoDecimalNegativeNumberChildStyle;
                        currentColumn++;

                        statusCellRange = worksheet.Cells[currentRow, currentColumn];
                        var statusPayoutValue = Converter.ToDecimal(statusData.Payout);
                        statusCellRange.Value = statusPayoutValue;
                        statusCellRange.Style = twoDecimalNumberPayoutChildStyle;
                        if (statusPayoutValue >= 1)
                        {
                            statusCellRange.Style.Font.Color = Color.Red;
                        }

                        currentColumn++;

                        currentRow++;
                        var detailLength = statusData.ReportRows.Count;
                        for (int j = 0; j < detailLength; j++)
                        {
                            var paddingBettypeName = string.Join(string.Empty, Enumerable.Repeat(paddingStatusName, 2));
                            currentColumn = startColumn;
                            var detailData = statusData.ReportRows[j];
                            ExcelCell detailCellRange;

                            detailCellRange = worksheet.Cells[currentRow, currentColumn];
                            detailCellRange.Value = paddingBettypeName + detailData.Bettype;
                            detailCellRange.Style = textStyle;
                            currentColumn++;

                            detailCellRange = worksheet.Cells[currentRow, currentColumn];
                            detailCellRange.Value = detailData.NumberPlayer;
                            detailCellRange.Style = intNumberStyle;
                            currentColumn++;

                            detailCellRange = worksheet.Cells[currentRow, currentColumn];
                            detailCellRange.Value = detailData.NumberGame;
                            detailCellRange.Style = intNumberStyle;
                            currentColumn++;

                            detailCellRange = worksheet.Cells[currentRow, currentColumn];
                            detailCellRange.Value = detailData.TotalBet;
                            detailCellRange.Style = twoDecimalNumberStyle;
                            currentColumn++;

                            detailCellRange = worksheet.Cells[currentRow, currentColumn];
                            detailCellRange.Value = detailData.TotalWin;
                            detailCellRange.Style = twoDecimalNumberStyle;
                            currentColumn++;

                            detailCellRange = worksheet.Cells[currentRow, currentColumn];
                            detailCellRange.Value = detailData.GameIncome;
                            detailCellRange.Style = twoDecimalNegativeNumberStyle;
                            currentColumn++;

                            detailCellRange = worksheet.Cells[currentRow, currentColumn];
                            var detailPayoutValue = Converter.ToDecimal(detailData.Payout);
                            detailCellRange.Value = detailPayoutValue;
                            detailCellRange.Style = twoDecimalNumberPayoutStyle;
                            if (detailPayoutValue >= 1)
                            {
                                detailCellRange.Style.Font.Color = Color.Red;
                            }

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
        /// Exports the footer report.
        /// </summary>
        /// <param name="worksheet">The worksheet.</param>
        /// <param name="startRow">The start row.</param>
        /// <param name="startColumn">The start column.</param>
        /// <param name="currentRow">The current row.</param>
        /// <param name="grandTotal">The grand total.</param>
        [SuppressMessage("StyleCop.CSharp.StylecopPlus.Rules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        private void ExportFooterReport(
           ref ExcelWorksheet worksheet,
           int startRow,
           int startColumn,
           ref int currentRow,
           NumberGameFinancial grandTotal)
        {
            try
            {
                var grandTotalSeperatorColor = ExcelExtensions.GrandTotalSeparatorColor;
                var textGrandTotalStyle = ExcelExtensions.GetCellTextStyle(ExcelExtensions.GrandTotalRowColor, true);
                textGrandTotalStyle.Borders.SetBorders(MultipleBorders.Top, grandTotalSeperatorColor, LineStyle.Medium);
                var intNumberStyle = ExcelExtensions.GetCellNumberStyle(
                   NumberFormatConstants.IntNumberFormat,
                   ExcelExtensions.GrandTotalRowColor,
                   true,
                   false,
                   false);
                intNumberStyle.Borders.SetBorders(MultipleBorders.Top, grandTotalSeperatorColor, LineStyle.Medium);
                var twoDecimalNegativeNumberParentStyle = ExcelExtensions.GetCellNumberStyle(
                    NumberFormatConstants.TwoDecimalDigitFormat,
                    ExcelExtensions.GrandTotalRowColor,
                    true,
                    false,
                    true);
                twoDecimalNegativeNumberParentStyle.Borders.SetBorders(MultipleBorders.Top, grandTotalSeperatorColor, LineStyle.Medium);
                var twoDecimalNumberStyle = ExcelExtensions.GetCellNumberStyle(
                    NumberFormatConstants.TwoDecimalDigitFormat,
                    ExcelExtensions.GrandTotalRowColor,
                    true,
                    false,
                    false);
                twoDecimalNumberStyle.Borders.SetBorders(MultipleBorders.Top, grandTotalSeperatorColor, LineStyle.Medium);
                var twoDecimalNumberPayoutStyle = ExcelExtensions.GetCellNumberStyle(
                    NumberFormatConstants.TwoDecimalDigitPercentFormat,
                    ExcelExtensions.GrandTotalRowColor,
                    true,
                    false,
                    false);
                twoDecimalNumberPayoutStyle.Borders.SetBorders(MultipleBorders.Top, grandTotalSeperatorColor, LineStyle.Medium);

                var currentColumn = startColumn;
                ExcelCell grandCellRange;

                grandCellRange = worksheet.Cells[currentRow, currentColumn];
                grandCellRange.Value = Labels.Grand_Total;
                grandCellRange.Style = textGrandTotalStyle;
                currentColumn++;

                grandCellRange = worksheet.Cells[currentRow, currentColumn];
                grandCellRange.Value = grandTotal.NumberPlayer;
                grandCellRange.Style = intNumberStyle;
                currentColumn++;

                grandCellRange = worksheet.Cells[currentRow, currentColumn];
                grandCellRange.Value = grandTotal.NumberGame;
                grandCellRange.Style = intNumberStyle;
                currentColumn++;

                grandCellRange = worksheet.Cells[currentRow, currentColumn];
                grandCellRange.Value = grandTotal.TotalBet;
                grandCellRange.Style = twoDecimalNumberStyle;
                currentColumn++;

                grandCellRange = worksheet.Cells[currentRow, currentColumn];
                grandCellRange.Value = grandTotal.TotalWin;
                grandCellRange.Style = twoDecimalNumberStyle;
                currentColumn++;

                grandCellRange = worksheet.Cells[currentRow, currentColumn];
                grandCellRange.Value = grandTotal.GameIncome;
                grandCellRange.Style = twoDecimalNegativeNumberParentStyle;
                currentColumn++;

                grandCellRange = worksheet.Cells[currentRow, currentColumn];
                grandCellRange.Value = Converter.ToDecimal(grandTotal.Payout);
                grandCellRange.Style = twoDecimalNumberPayoutStyle;
                currentColumn++;
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }
        }
    }
}
