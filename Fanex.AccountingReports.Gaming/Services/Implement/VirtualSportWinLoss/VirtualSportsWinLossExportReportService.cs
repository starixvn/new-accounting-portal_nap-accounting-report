// -----------------------------------------------------------------------
// <copyright file="VirtualSportsWinLossExportReportService.cs" company="Nexcel Solutions Vietname">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Fanex.AccountingReports.Gaming.Services.Implement.VirtualSportWinLoss
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Drawing;
    using ExcelUtil.Spreadsheet;
    using Fanex.AccountingReports.Core.Constants;
    using Fanex.AccountingReports.Core.Extensions;
    using Fanex.AccountingReports.Core.Services.Impl;
    using Fanex.AccountingReports.Core.Web;
    using Fanex.AccountingReports.Gaming.Constants;
    using Fanex.AccountingReports.Gaming.Criteria.VirtualSportsWinLoss;
    using Fanex.AccountingReports.Gaming.Models.VirtualSportsWinLoss;
    using Fanex.AccountingReports.Gaming.Resources;
    using Fanex.AccountingReports.Gaming.Services.Interface.VirtualSportWinLoss;

    /// <summary>
    /// The export report service.
    /// </summary>
    public class VirtualSportsWinLossExportReportService : ServiceBase, 
                                                          IVirtualSportsWinLossExportReportService
    {
        private string _virtualSportsWinLossPrefixSessionKey = 
            "Fanex.AccountingReports.Gaming.VirtualSportsWinLoss.";

        private string _virtualSportsWinLossSessionKey = string.Empty;

        private VirtualSportsWinLossGetReportService _getReportService;

        public VirtualSportsWinLossExportReportService()
        {
            if (AppSession.CurrentUser != null)
            {
                this._virtualSportsWinLossSessionKey = this._virtualSportsWinLossPrefixSessionKey
                                                        + AppSession.CurrentUser.Username;
            }
            else
            {
                this._virtualSportsWinLossSessionKey = this._virtualSportsWinLossPrefixSessionKey;
            }

            this._getReportService = new VirtualSportsWinLossGetReportService();
        }

        /// <summary>
        /// Export report.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        public void ExportReport(GetVirtualSportsWinLossCriteria criteria)
        {
            try
            {
                var report = SessionManager.Get<VirtualSportsWinLoss>(this._virtualSportsWinLossSessionKey);

                if (report == null)
                {
                    report = this._getReportService.GetReport(criteria);
                }

                ExcelFile excelFile = new ExcelFile();
                ExcelWorksheetCollection worksheets = excelFile.Worksheets;
                var currentRow = 0;

                if (report != null && report.Rows != null
                    && report.Rows.Count > 0)
                {
                    this.LoopAndPushDataToAllSheets(criteria, report, worksheets, ref currentRow);
                }
                else
                {
                    string sheetName = GamingResources.VirtualSportsWinLoss_SheetName;
                    ExcelWorksheet worksheet = worksheets.Add(sheetName);
                    worksheet.ExportEmptyReport(0, 0, ref currentRow, 7);
                }

                excelFile.ReponseStream(string.Format(
                    GamingResources.VirtualSportsWinLoss_ReportName,
                    criteria.FromDate.ToShortFormatedDateWithSlash().Replace('/', '_'),
                    criteria.ToDate.ToShortFormatedDateWithSlash().Replace('/', '_')));
            }
            catch (Exception ex) 
            {
                this.HandleException(ex);
            }
        }

        /// <summary>
        /// Loop and push data to all sheets.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <param name="report">The report.</param>
        /// <param name="worksheets">The worksheets.</param>
        /// <param name="currentRow">The current row.</param>
        private void LoopAndPushDataToAllSheets(
            GetVirtualSportsWinLossCriteria criteria, 
            VirtualSportsWinLoss report, 
            ExcelWorksheetCollection worksheets, 
            ref int currentRow)
        {
            var numberOfDates = report.Rows.Count;
            var currentIndex = 0;
            var totalRows = report.Rows[numberOfDates - 1];

            for (int i = 0; i < numberOfDates - 1; i++)
            {
                var date = report.Rows[i];
                this.ExportReportDate(
                    ref worksheets, 
                    ref currentRow, 
                    criteria, 
                    report, 
                    date, 
                    totalRows,
                    ref currentIndex);
            }

            ExcelWorksheet worksheet = worksheets[worksheets.Count - 1];
            this.PushTotalAndGrandTotalIntoSheet(ref worksheet, report, ref currentRow);
        }

        /// <summary>
        /// Export report by date.
        /// </summary>
        /// <param name="worksheets">The worksheets collection.</param>
        /// <param name="currentRow">The current row.</param>
        /// <param name="criteria">The criteria.</param>
        /// <param name="report">The report.</param>
        /// <param name="date">The date contain currency.</param>
        /// <param name="totalRows">The total rows.</param>
        /// <param name="currentIndex">The current index.</param>
        [SuppressMessage("StyleCop.CSharp.Nexcel.NexcelCustomRules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        private void ExportReportDate(
            ref ExcelWorksheetCollection worksheets, 
            ref int currentRow, 
            GetVirtualSportsWinLossCriteria criteria, 
            VirtualSportsWinLoss report, 
            VirtualSportsWinLoss date, 
            VirtualSportsWinLoss totalRows,
            ref int currentIndex)
        {
            if (date != null
                        && date.Currencies != null
                        && date.Currencies.Count > 0)
            {
                ExcelWorksheet worksheet = worksheets.Count > 0 ? worksheets[worksheets.Count - 1] : null;
                var numberCurrenciesInDate = date.Currencies.Count;
                var numberCurrenciesPushToSheet = 0;
                var numberCurrenciesRemainInDate = 0;
                var totalCurrencies = currentIndex + numberCurrenciesInDate;

                if (currentIndex % GamingConstants.MaxRowsData == 0)
                {
                    worksheet = this.CreateNewWorksheet(ref worksheets, ref worksheet, report, ref currentRow, criteria, currentIndex);
                    currentIndex = 0;
                }

                if (totalCurrencies < GamingConstants.MaxRowsData)
                {
                    this.PushCurrenciesIntoSheet(ref worksheet, date, numberCurrenciesInDate, 0, numberCurrenciesInDate, GamingConstants.ReportRowType.OddRow, ref currentRow);
                    currentIndex = totalCurrencies;
                }
                else
                {
                    numberCurrenciesRemainInDate = totalCurrencies - GamingConstants.MaxRowsData;
                    numberCurrenciesPushToSheet = numberCurrenciesInDate - numberCurrenciesRemainInDate;

                    this.PushCurrenciesIntoSheet(
                        ref worksheet, 
                        date, 
                        numberCurrenciesPushToSheet, 
                        0, 
                        numberCurrenciesPushToSheet, 
                        GamingConstants.ReportRowType.OddRow, 
                        ref currentRow);
                    currentIndex += numberCurrenciesPushToSheet;

                    if (numberCurrenciesRemainInDate > 0)
                    {
                        worksheet = this.CreateNewWorksheet(ref worksheets, ref worksheet, report, ref currentRow, criteria, currentIndex);
                        currentIndex = 0;
                        this.PushCurrenciesIntoSheet(
                            ref worksheet, 
                            date, 
                            numberCurrenciesRemainInDate,
                            numberCurrenciesPushToSheet, 
                            numberCurrenciesInDate,
                            GamingConstants.ReportRowType.OddRow, 
                            ref currentRow);
                        currentIndex += numberCurrenciesRemainInDate;
                    }
                }
            }
        }

        /// <summary>
        /// Push currencies into sheet.
        /// </summary>
        /// <param name="worksheet">The worksheet.</param>
        /// <param name="date">The data contain currencies.</param>
        /// <param name="numberCurrenciesPushToSheet">The number of currencies push into sheet.</param>
        /// <param name="startIndex">The start index.</param>
        /// <param name="endIndex">The end index.</param>
        /// <param name="reportRowType">The report row type.</param>
        /// <param name="currentRow">The current row.</param>
        private void PushCurrenciesIntoSheet(
            ref ExcelWorksheet worksheet, 
            VirtualSportsWinLoss date,
            int numberCurrenciesPushToSheet,
            int startIndex,
            int endIndex,
            GamingConstants.ReportRowType reportRowType,
            ref int currentRow)
        {
            if (reportRowType == GamingConstants.ReportRowType.GrandTotal)
            {
                this.PushGrandTotal(ref worksheet, date, ref currentRow, reportRowType);
            }
            else
            {
                this.PushOthersReportType(
                    ref worksheet, 
                    date,
                    numberCurrenciesPushToSheet,
                    startIndex,
                    endIndex,
                    ref currentRow, 
                    reportRowType);
            }
        }

        /// <summary>
        /// Push others report type : odd, total.
        /// </summary>
        /// <param name="worksheet">The worksheet.</param>
        /// <param name="date">The date contain data.</param>
        /// <param name="numberCurrenciesPushToSheet">The number of currencies are pushed to sheet.</param>
        /// <param name="startIndex">The start index.</param>
        /// <param name="endIndex">The end index.</param>
        /// <param name="currentRow">The current row.</param>
        /// <param name="reportRowType">The type of row.</param>
        private void PushOthersReportType(
            ref ExcelWorksheet worksheet, 
            VirtualSportsWinLoss date,
            int numberCurrenciesPushToSheet,
            int startIndex,
            int endIndex,
            ref int currentRow, 
            GamingConstants.ReportRowType reportRowType)
        {
            var textStyle = new CellStyle();
            var intStyle = new CellStyle();
            var decimalStyle = new CellStyle();
            this.InitCellStyle(ref textStyle, ref intStyle, ref decimalStyle, reportRowType);

            int currentColumn = 0;
            var lastRow = currentRow + numberCurrenciesPushToSheet - 1;
            this.ExportCell(ref worksheet, textStyle, date.Date, currentRow, lastRow, ref currentColumn, currentColumn);

            for (int i = startIndex; i < endIndex; i++)
            {
                var currencyInDate = date.Currencies[i];
                currentColumn = 1;
                this.ExportCell(ref worksheet, textStyle, currencyInDate.Name, currentRow, currentRow, ref currentColumn, currentColumn);
                this.ExportCell(ref worksheet, intStyle, currencyInDate.BetCount, currentRow, currentRow, ref currentColumn, currentColumn);
                this.ExportCell(ref worksheet, decimalStyle, currencyInDate.Turnover, currentRow, currentRow, ref currentColumn, currentColumn);
                this.ExportCell(ref worksheet, decimalStyle, currencyInDate.MemberWinLoss, currentRow, currentRow, ref currentColumn, currentColumn);
                this.ExportCell(ref worksheet, decimalStyle, currencyInDate.PositionWinLoss, currentRow, currentRow, ref currentColumn, currentColumn);
                this.ExportCell(ref worksheet, decimalStyle, currencyInDate.CompanyWinLoss, currentRow, currentRow, ref currentColumn, currentColumn);
                this.ExportCell(ref worksheet, decimalStyle, currencyInDate.AmountDueInspired, currentRow, currentRow, ref currentColumn, currentColumn);

                currentRow++;
            }
        }

        /// <summary>
        /// Push grand total row.
        /// </summary>
        /// <param name="worksheet">The worksheet.</param>
        /// <param name="date">The date contain data.</param>
        /// <param name="currentRow">The current row.</param>
        /// <param name="reportRowType">The report row type.</param>
        private void PushGrandTotal(
            ref ExcelWorksheet worksheet, 
            VirtualSportsWinLoss date, 
            ref int currentRow,
            GamingConstants.ReportRowType reportRowType)
        {
            var grandTotalInRM = date.Currencies[0];
            var grandTotalInUER = date.Currencies[1];

            var textStyle = new CellStyle();
            var intStyle = new CellStyle();
            var decimalStyle = new CellStyle();

            this.InitCellStyle(ref textStyle, ref intStyle, ref decimalStyle, reportRowType);

            int currentColumn = 0;
            textStyle.Borders.SetBorders(MultipleBorders.Top, Color.FromArgb(255, 102, 0), LineStyle.Medium);
            intStyle.Borders.SetBorders(MultipleBorders.Top, Color.FromArgb(255, 102, 0), LineStyle.Medium);
            decimalStyle.Borders.SetBorders(MultipleBorders.Top, Color.FromArgb(255, 102, 0), LineStyle.Medium);

            this.ExportCell(ref worksheet, textStyle, Labels.Grand_Total_RM, currentRow, currentRow, ref currentColumn, currentColumn + 1);
            this.ExportCell(ref worksheet, intStyle, grandTotalInRM.BetCount, currentRow, currentRow, ref currentColumn, currentColumn);
            this.ExportCell(ref worksheet, decimalStyle, grandTotalInRM.Turnover, currentRow, currentRow, ref currentColumn, currentColumn);
            this.ExportCell(ref worksheet, decimalStyle, grandTotalInRM.MemberWinLoss, currentRow, currentRow, ref currentColumn, currentColumn);
            this.ExportCell(ref worksheet, decimalStyle, grandTotalInRM.PositionWinLoss, currentRow, currentRow, ref currentColumn, currentColumn);
            this.ExportCell(ref worksheet, decimalStyle, grandTotalInRM.CompanyWinLoss, currentRow, currentRow, ref currentColumn, currentColumn);
            this.ExportCell(ref worksheet, decimalStyle, grandTotalInRM.AmountDueInspired, currentRow, currentRow, ref currentColumn, currentColumn);
            currentRow++;

            this.InitCellStyle(ref textStyle, ref intStyle, ref decimalStyle, reportRowType);
            currentColumn = 0;

            this.ExportCell(ref worksheet, textStyle, Labels.Grand_Total_EUR, currentRow, currentRow, ref currentColumn, currentColumn + 1);
            this.ExportCell(ref worksheet, intStyle, grandTotalInUER.BetCount, currentRow, currentRow, ref currentColumn, currentColumn);
            this.ExportCell(ref worksheet, decimalStyle, grandTotalInUER.Turnover, currentRow, currentRow, ref currentColumn, currentColumn);
            this.ExportCell(ref worksheet, decimalStyle, grandTotalInUER.MemberWinLoss, currentRow, currentRow, ref currentColumn, currentColumn);
            this.ExportCell(ref worksheet, decimalStyle, grandTotalInUER.PositionWinLoss, currentRow, currentRow, ref currentColumn, currentColumn);
            this.ExportCell(ref worksheet, decimalStyle, grandTotalInUER.CompanyWinLoss, currentRow, currentRow, ref currentColumn, currentColumn);
            this.ExportCell(ref worksheet, decimalStyle, grandTotalInUER.AmountDueInspired, currentRow, currentRow, ref currentColumn, currentColumn);
            currentRow++;
        }

        /// <summary>
        /// Initial styles of cell.
        /// </summary>
        /// <param name="textStyle">The text style.</param>
        /// <param name="intStyle">The integer style.</param>
        /// <param name="decimalStyle">The decimal style.</param>
        /// <param name="reportRowType">The type of row.</param>
        private void InitCellStyle(
            ref CellStyle textStyle, 
            ref CellStyle intStyle, 
            ref CellStyle decimalStyle, 
            GamingConstants.ReportRowType reportRowType)
        {
            Color totalRowColor = Color.FromArgb(232, 235, 211);

            switch (reportRowType)
            {
                case GamingConstants.ReportRowType.OddRow:
                    textStyle = ExcelExtensions.GetCellTextStyle(ExcelExtensions.BackGroundColor, false);
                    textStyle.HorizontalAlignment = HorizontalAlignmentStyle.Center;
                    intStyle = ExcelExtensions.GetCellNumberStyle(NumberFormatConstants.IntNumberFormat, ExcelExtensions.BackGroundColor, false, false, false);
                    intStyle.HorizontalAlignment = HorizontalAlignmentStyle.Right;
                    decimalStyle = ExcelExtensions.GetCellNumberStyle(NumberFormatConstants.TwoDecimalDigitFormat, ExcelExtensions.BackGroundColor, false, false, false);
                    decimalStyle.HorizontalAlignment = HorizontalAlignmentStyle.Right;
                    break;

                case GamingConstants.ReportRowType.GrandTotal:
                    textStyle = ExcelExtensions.GetCellTextStyle(ExcelExtensions.GrandTotalRowColor, true);
                    textStyle.HorizontalAlignment = HorizontalAlignmentStyle.Center;
                    decimalStyle = ExcelExtensions.GetCellNumberStyle(NumberFormatConstants.TwoDecimalDigitFormat, ExcelExtensions.GrandTotalRowColor, true, false, false);
                    decimalStyle.HorizontalAlignment = HorizontalAlignmentStyle.Right;
                    intStyle = ExcelExtensions.GetCellNumberStyle(NumberFormatConstants.IntNumberFormat, ExcelExtensions.GrandTotalRowColor, true, false, false);
                    intStyle.HorizontalAlignment = HorizontalAlignmentStyle.Right;
                    break;

                case GamingConstants.ReportRowType.Total:
                    textStyle = ExcelExtensions.GetCellTextStyle(totalRowColor, true);
                    textStyle.HorizontalAlignment = HorizontalAlignmentStyle.Center;
                    decimalStyle = ExcelExtensions.GetCellNumberStyle(NumberFormatConstants.TwoDecimalDigitFormat, totalRowColor, true, false, false);
                    decimalStyle.HorizontalAlignment = HorizontalAlignmentStyle.Right;
                    intStyle = ExcelExtensions.GetCellNumberStyle(NumberFormatConstants.IntNumberFormat, totalRowColor, true, false, false);
                    intStyle.HorizontalAlignment = HorizontalAlignmentStyle.Right;
                    break;
            }
        }

        /// <summary>
        /// Create a new worksheet and render header, total and grand total to old sheet.
        /// </summary>
        /// <param name="worksheets">The worksheet collection.</param>
        /// <param name="worksheet">The worksheet.</param>
        /// <param name="report">The report.</param>
        /// <param name="currentRow">The current row.</param>
        /// <param name="criteria">The criteria.</param>
        /// <param name="currentIndex">The current index.</param>
        /// <returns>The new worksheet.</returns>
        private ExcelWorksheet CreateNewWorksheet(
            ref ExcelWorksheetCollection worksheets,
            ref ExcelWorksheet worksheet,
            VirtualSportsWinLoss report,
            ref int currentRow,
            GetVirtualSportsWinLossCriteria criteria,
            int currentIndex)
        {
            if (currentIndex != 0)
            {
                this.PushTotalAndGrandTotalIntoSheet(ref worksheet, report, ref currentRow);
            }

            string sheetName = "Sheet " + (worksheets.Count + 1);
            worksheet = worksheets.Add(sheetName);
            currentRow = 0;
            this.ExportReportHeader(ref worksheet, ref currentRow, criteria);

            return worksheet;
        }

        /// <summary>
        /// Call method to push total and grand total into sheet.
        /// </summary>
        /// <param name="worksheet">The worksheet.</param>
        /// <param name="report">The report.</param>
        /// <param name="currentRow">The current row.</param>
        private void PushTotalAndGrandTotalIntoSheet(
            ref ExcelWorksheet worksheet, 
            VirtualSportsWinLoss report, 
            ref int currentRow)
        {
            var totalRows = report.Rows[report.Rows.Count - 1];

            this.PushCurrenciesIntoSheet(
                ref worksheet, 
                totalRows, 
                totalRows.Currencies.Count, 
                0,
                totalRows.Currencies.Count,
                GamingConstants.ReportRowType.Total, 
                ref currentRow);
            if (report.Currencies != null && report.Currencies.Count == 2)
            {
                this.PushCurrenciesIntoSheet(
                    ref worksheet, 
                    report, 
                    report.Currencies.Count, 
                    0,
                    report.Currencies.Count,
                    GamingConstants.ReportRowType.GrandTotal, 
                    ref currentRow);
            }
        }

        /// <summary>
        /// Export report header.
        /// </summary>
        /// <param name="worksheet">The worksheet.</param>
        /// <param name="currentRow">The index of current row.</param>
        /// <param name="criteria">The criteria.</param>
        private void ExportReportHeader(
            ref ExcelWorksheet worksheet, 
            ref int currentRow, 
            GetVirtualSportsWinLossCriteria criteria)
        {
            var titleAndHeaderStyle = ExcelExtensions.GetCellTitleStyle(ExcelExtensions.HeaderTitleColor);
            var startColumn = 0;
            var currentColumn = startColumn;

            for (int colIndex = 0; colIndex < 8; colIndex++)
            {
                worksheet.Columns[startColumn + colIndex].Width = 160 * ExcelExtensions.FontWidth;
            }

            var title = string.Format(
                GamingResources.VirtualSportsWinLoss_ReportTitle,
                criteria.FromDate.ToShortFormatedDate(),
                criteria.ToDate.ToShortFormatedDate());

            this.ExportCell(ref worksheet, titleAndHeaderStyle, title, currentRow, currentRow + 1, ref currentColumn, currentColumn + 7);

            currentColumn = startColumn;
            currentRow += 2;

            worksheet.ExportEmptyRow(0, startColumn, ref currentRow, startColumn + 7);
            currentColumn = startColumn;

            this.ExportCell(ref worksheet, titleAndHeaderStyle, Labels.Date, currentRow, currentRow + 1, ref currentColumn, currentColumn);
            this.ExportCell(ref worksheet, titleAndHeaderStyle, Labels.Currency, currentRow, currentRow + 1, ref currentColumn, currentColumn);
            this.ExportCell(ref worksheet, titleAndHeaderStyle, Labels.Bet_Count, currentRow, currentRow + 1, ref currentColumn, currentColumn);
            this.ExportCell(ref worksheet, titleAndHeaderStyle, Labels.Turnover, currentRow, currentRow + 1, ref currentColumn, currentColumn);
            this.ExportCell(ref worksheet, titleAndHeaderStyle, Labels.Member_Win_Loss, currentRow, currentRow + 1, ref currentColumn, currentColumn);
            this.ExportCell(ref worksheet, titleAndHeaderStyle, Labels.Win_Loss, currentRow, currentRow, ref currentColumn, currentColumn + 1);
            currentColumn -= 2;
            this.ExportCell(ref worksheet, titleAndHeaderStyle, Labels.Position, currentRow + 1, currentRow + 1, ref currentColumn, currentColumn);
            this.ExportCell(ref worksheet, titleAndHeaderStyle, Labels.Company, currentRow + 1, currentRow + 1, ref currentColumn, currentColumn);
            this.ExportCell(ref worksheet, titleAndHeaderStyle, Labels.AmountDueToInspired, currentRow, currentRow + 1, ref currentColumn, currentColumn);

            currentRow += 2;
        }

        /// <summary>
        /// Export cell.
        /// </summary>
        /// <param name="worksheet">The worksheet.</param>
        /// <param name="cellStyle">The style of cell.</param>
        /// <param name="cellValue">The value of cell.</param>
        /// <param name="currentRow">The current row.</param>
        /// <param name="lastRow">The last row.</param>
        /// <param name="currentColumn">The current column.</param>
        /// <param name="lastColumn">The last column.</param>
        private void ExportCell(
            ref ExcelWorksheet worksheet,
            CellStyle cellStyle,
            object cellValue,
            int currentRow,
            int lastRow,
            ref int currentColumn,
            int lastColumn)
        {
            CellRange currentCellRange = worksheet.Cells.GetSubrangeAbsolute(
                currentRow, 
                currentColumn, 
                lastRow, 
                lastColumn);
            currentCellRange.Merged = true;
            currentCellRange.Value = cellValue;
            currentCellRange.Style = cellStyle;
            currentColumn = lastColumn + 1;
        }
    }
}
