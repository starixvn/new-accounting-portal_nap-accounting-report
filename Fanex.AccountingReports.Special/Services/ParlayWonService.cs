namespace Fanex.AccountingReports.Special.Services.Implement
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
    using Fanex.AccountingReports.Special.Criteria.ParlayWon;
    using Fanex.AccountingReports.Special.Models.ParlayWon;
    using Fanex.AccountingReports.Special.Resources;
    using Fanex.Common;

    public class ParlayWonService : ServiceBase
    {
        private enum ReportRowType
        {
            /// <summary>
            /// The total.
            /// </summary>
            Total = 1,

            /// <summary>
            /// The grand total.
            /// </summary>
            GrandTotal = 2,

            /// <summary>
            /// The normal even row.
            /// </summary>
            NormalEvenRow = 3,

            /// <summary>
            /// The normal odd row.
            /// </summary>
            NormalOddRow = 4
        }

        /// <summary>
        /// Gets the report.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>The report.</returns>
        public ParlayWonTransaction GetReport(GetParlayWonReportCriteria criteria)
        {
            ParlayWonTransaction fullReport = null;
            var startIndex = criteria.StartIndex;
            var endIndex = criteria.EndIndex;

            fullReport = this.GetFullReportFromDatabase(criteria);

            var pagingReport = new ParlayWonTransaction
            {
                TransactionList = fullReport.TransactionList.Skip(startIndex)
                                            .Take(criteria.ItemsPerPage)
                                            .ToList(),
                ReportItemLength = fullReport.ReportItemLength,
                Total = fullReport.Total
            };

            return pagingReport;
        }

        /// <summary>
        /// Sorts the report.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>The report.</returns>
        public ParlayWonTransaction SortReport(GetParlayWonReportCriteria criteria)
        {
            var fullReport = this.GetReport(criteria);

            if (fullReport != null &&
                fullReport.TransactionList != null &&
                fullReport.TransactionList.Count > 0)
            {
                var propertyName = this.GetSortedColumnName(criteria.SortBy);
                var startIndex = criteria.StartIndex;
                var endIndex = criteria.EndIndex;
                fullReport.TransactionList.SortByProperty<ParlayWonTransaction>(
                    criteria.SortDirection,
                    propertyName);


                var pagingReport = new ParlayWonTransaction
                {
                    TransactionList = fullReport.TransactionList.Skip(startIndex)
                                                .Take(criteria.ItemsPerPage)
                                                .ToList(),
                    ReportItemLength = fullReport.ReportItemLength,
                    Total = fullReport.Total
                };

                return pagingReport;
            }

            return null;
        }

        //// ExportExcel

        /// <summary>
        /// Exports the report.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        [SuppressMessage("StyleCop.CSharp.StylecopPlus.Rules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        public void ExportReport(GetParlayWonReportCriteria criteria)
        {
            var fullReport = this.GetReport(criteria);

            //// ExcelTypes

            ExcelFile excelFile = new ExcelFile();
            string sheetName = SpecialResources.ParlayWon_SheetName;
            ExcelWorksheet workSheet = excelFile.Worksheets.Add(string.Format("{0} - {1}", sheetName, 1));

            int startRow = 0;
            int startColumn = 0;
            var currentRow = startRow;

            var textStyle = ExcelExtensions.GetCellTextStyle(ExcelExtensions.BackGroundColor, false);
            var intStyle = ExcelExtensions.GetCellNumberStyle(string.Empty, ExcelExtensions.BackGroundColor, false, false, false);
            var twoDecimalNumberStyle = ExcelExtensions.GetCellNumberStyle(
                NumberFormatConstants.TwoDecimalDigitFormat,
                ExcelExtensions.BackGroundColor,
                false,
                false,
                false);
            var fourDecimalNumberStyle = ExcelExtensions.GetCellNumberStyle(
                NumberFormatConstants.FourDecimalDigitFormat,
                ExcelExtensions.BackGroundColor,
                false,
                false,
                false);
            textStyle = ExcelExtensions.GetCellTextStyle(ExcelExtensions.BackGroundColor, false);
            intStyle = ExcelExtensions.GetCellNumberStyle(string.Empty, ExcelExtensions.BackGroundColor, false, false, false);
            twoDecimalNumberStyle = ExcelExtensions.GetCellNumberStyle(NumberFormatConstants.TwoDecimalDigitFormat, ExcelExtensions.BackGroundColor, false, false, false);
            fourDecimalNumberStyle = ExcelExtensions.GetCellNumberStyle(NumberFormatConstants.FourDecimalDigitFormat, ExcelExtensions.BackGroundColor, false, false, false);
            textStyle = ExcelExtensions.GetCellTextStyle(ExcelExtensions.GrandTotalRowColor, true);
            textStyle.Borders.SetBorders(MultipleBorders.Top, Color.FromArgb(255, 102, 0), LineStyle.Medium);
            intStyle = ExcelExtensions.GetCellNumberStyle(string.Empty, ExcelExtensions.GrandTotalRowColor, true, false, false);
            intStyle.HorizontalAlignment = HorizontalAlignmentStyle.Right;
            intStyle.Borders.SetBorders(MultipleBorders.Top, Color.FromArgb(255, 102, 0), LineStyle.Medium);

            twoDecimalNumberStyle = ExcelExtensions.GetCellNumberStyle(NumberFormatConstants.TwoDecimalDigitFormat, ExcelExtensions.GrandTotalRowColor, true, false, false);
            twoDecimalNumberStyle.HorizontalAlignment = HorizontalAlignmentStyle.Right;
            twoDecimalNumberStyle.Borders.SetBorders(MultipleBorders.Top, Color.FromArgb(255, 102, 0), LineStyle.Medium);
            fourDecimalNumberStyle = ExcelExtensions.GetCellNumberStyle(
                NumberFormatConstants.FourDecimalDigitFormat,
                ExcelExtensions.GrandTotalRowColor,
                false,
                false,
                false);
            fourDecimalNumberStyle.HorizontalAlignment = HorizontalAlignmentStyle.Right;
            fourDecimalNumberStyle.Borders.SetBorders(MultipleBorders.Top, Color.FromArgb(255, 102, 0), LineStyle.Medium);

            var textStyle_EvenRow = ExcelExtensions.GetCellTextStyle(ExcelExtensions.BackGroundColor, false);
            var intStyle_EvenRow = ExcelExtensions.GetCellNumberStyle(string.Empty, ExcelExtensions.BackGroundColor, false, false, false);
            var twoDecimalNumberStyle_EvenRow = ExcelExtensions.GetCellNumberStyle(
                NumberFormatConstants.TwoDecimalDigitFormat,
                ExcelExtensions.BackGroundColor,
                false,
                false,
                false);
            var fourDecimalNumberStyle_EvenRow = ExcelExtensions.GetCellNumberStyle(
                NumberFormatConstants.FourDecimalDigitFormat,
                ExcelExtensions.BackGroundColor,
                false,
                false,
                false);
            textStyle_EvenRow = ExcelExtensions.GetCellTextStyle(ExcelExtensions.EvenRowColor, false);
            intStyle_EvenRow = ExcelExtensions.GetCellNumberStyle(string.Empty, ExcelExtensions.EvenRowColor, false, false, false);
            intStyle_EvenRow.HorizontalAlignment = HorizontalAlignmentStyle.Right;
            twoDecimalNumberStyle_EvenRow = ExcelExtensions.GetCellNumberStyle(
                NumberFormatConstants.TwoDecimalDigitFormat,
                ExcelExtensions.EvenRowColor,
                false,
                false,
                false);
            twoDecimalNumberStyle_EvenRow.HorizontalAlignment = HorizontalAlignmentStyle.Right;
            fourDecimalNumberStyle_EvenRow = ExcelExtensions.GetCellNumberStyle(
                NumberFormatConstants.FourDecimalDigitFormat,
                ExcelExtensions.EvenRowColor,
                false,
                false,
                false);
            fourDecimalNumberStyle_EvenRow.HorizontalAlignment = HorizontalAlignmentStyle.Right;

            var textStyle_OddRow = ExcelExtensions.GetCellTextStyle(ExcelExtensions.BackGroundColor, false);
            var intStyle_OddRow = ExcelExtensions.GetCellNumberStyle(string.Empty, ExcelExtensions.BackGroundColor, false, false, false);
            var twoDecimalNumberStyle_OddRow = ExcelExtensions.GetCellNumberStyle(
                NumberFormatConstants.TwoDecimalDigitFormat,
                ExcelExtensions.BackGroundColor,
                false,
                false,
                false);
            var fourDecimalNumberStyle_OddRow = ExcelExtensions.GetCellNumberStyle(
                NumberFormatConstants.FourDecimalDigitFormat,
                ExcelExtensions.BackGroundColor,
                false,
                false,
                false);
            textStyle_OddRow = ExcelExtensions.GetCellTextStyle(ExcelExtensions.BackGroundColor, false);
            intStyle_OddRow = ExcelExtensions.GetCellNumberStyle(string.Empty, ExcelExtensions.BackGroundColor, false, false, false);
            intStyle_OddRow.HorizontalAlignment = HorizontalAlignmentStyle.Right;
            twoDecimalNumberStyle_OddRow = ExcelExtensions.GetCellNumberStyle(
                NumberFormatConstants.TwoDecimalDigitFormat,
                ExcelExtensions.BackGroundColor,
                false,
                false,
                false);
            twoDecimalNumberStyle_OddRow.HorizontalAlignment = HorizontalAlignmentStyle.Right;
            fourDecimalNumberStyle_OddRow = ExcelExtensions.GetCellNumberStyle(
                NumberFormatConstants.FourDecimalDigitFormat,
                ExcelExtensions.BackGroundColor,
                false,
                false,
                false);
            fourDecimalNumberStyle_OddRow.HorizontalAlignment = HorizontalAlignmentStyle.Right;

            ExcelCell currentCell;
            var maxItemsInSheet = 3000;

            //// ExcelTypes
            //// MakeExcelFile

            this.ExportReportHeader(
                ref workSheet,
                startRow,
                startColumn,
                ref currentRow,
                criteria);

            if (fullReport != null &&
                fullReport.TransactionList != null &&
                fullReport.TransactionList.Count > 0)
            {
                var reportLength = fullReport.TransactionList.Count;
                var totalSheet = (fullReport.TransactionList.Count % maxItemsInSheet > 0 ? 1 : 0) + (fullReport.TransactionList.Count / maxItemsInSheet);
                int infoIndex = 0;
                int currentInfoIndexInSheet = 0;
                int sheetIndex = 1;
                for (int i = 0; i < totalSheet; i++)
                {
                    var currentColumn = startColumn;
                    for (; infoIndex < reportLength && currentInfoIndexInSheet < maxItemsInSheet;
                        infoIndex++, currentInfoIndexInSheet++)
                    {
                        var transaction = fullReport.TransactionList[infoIndex];
                        if (infoIndex % 2 == 0)
                        {
                            //// NormalEvenRow

                            currentColumn = startColumn;
                            currentCell = workSheet.Cells[currentRow, currentColumn];
                            currentCell.Value = transaction.RefNo;
                            currentCell.Style = textStyle_EvenRow;
                            currentCell.Style.HorizontalAlignment = HorizontalAlignmentStyle.Center;
                            currentColumn++;

                            currentCell = workSheet.Cells[currentRow, currentColumn];
                            currentCell.Value = transaction.UserName;
                            currentCell.Style = textStyle_EvenRow;
                            currentColumn++;

                            currentCell = workSheet.Cells[currentRow, currentColumn];
                            currentCell.Value = transaction.Currency;
                            currentCell.Style = textStyle_EvenRow;
                            currentCell.Style.HorizontalAlignment = HorizontalAlignmentStyle.Center;
                            currentColumn++;

                            currentCell = workSheet.Cells[currentRow, currentColumn];
                            currentCell.Value = transaction.Exchange;
                            currentCell.Style = fourDecimalNumberStyle_EvenRow;
                            currentColumn++;

                            currentCell = workSheet.Cells[currentRow, currentColumn];
                            currentCell.Value = transaction.OriginalStake;
                            currentCell.Style = twoDecimalNumberStyle_EvenRow;
                            currentColumn++;

                            currentCell = workSheet.Cells[currentRow, currentColumn];
                            currentCell.Value = transaction.OriginalWinLoss;
                            currentCell.Style = twoDecimalNumberStyle_EvenRow;
                            currentColumn++;

                            currentCell = workSheet.Cells[currentRow, currentColumn];
                            currentCell.Value = transaction.BaseStake;
                            currentCell.Style = twoDecimalNumberStyle_EvenRow;
                            currentColumn++;

                            currentCell = workSheet.Cells[currentRow, currentColumn];
                            currentCell.Value = transaction.BaseWinLoss;
                            currentCell.Style = twoDecimalNumberStyle_EvenRow;
                            currentColumn++;

                            currentCell = workSheet.Cells[currentRow, currentColumn];
                            currentCell.Value = transaction.CompanyStake;
                            currentCell.Style = twoDecimalNumberStyle_EvenRow;
                            currentColumn++;

                            currentCell = workSheet.Cells[currentRow, currentColumn];
                            currentCell.Value = transaction.CompanyWinLoss;
                            currentCell.Style = twoDecimalNumberStyle_EvenRow;
                            currentColumn++;

                            currentCell = workSheet.Cells[currentRow, currentColumn];
                            currentCell.Value = transaction.Selection;
                            currentCell.Style = intStyle_EvenRow;
                            currentColumn++;

                            currentCell = workSheet.Cells[currentRow, currentColumn];
                            currentCell.Value = transaction.WinOdds;
                            currentCell.Style = fourDecimalNumberStyle_EvenRow;
                            currentColumn++;
                            currentRow++;

                            //// NormalEvenRow
                        }
                        else
                        {
                            //// NormalOddRow

                            currentColumn = startColumn;
                            currentCell = workSheet.Cells[currentRow, currentColumn];
                            currentCell.Value = transaction.RefNo;
                            currentCell.Style = textStyle_OddRow;
                            currentCell.Style.HorizontalAlignment = HorizontalAlignmentStyle.Center;
                            currentColumn++;

                            currentCell = workSheet.Cells[currentRow, currentColumn];
                            currentCell.Value = transaction.UserName;
                            currentCell.Style = textStyle_OddRow;
                            currentColumn++;

                            currentCell = workSheet.Cells[currentRow, currentColumn];
                            currentCell.Value = transaction.Currency;
                            currentCell.Style = textStyle_OddRow;
                            currentCell.Style.HorizontalAlignment = HorizontalAlignmentStyle.Center;
                            currentColumn++;

                            currentCell = workSheet.Cells[currentRow, currentColumn];
                            currentCell.Value = transaction.Exchange;
                            currentCell.Style = fourDecimalNumberStyle_OddRow;
                            currentColumn++;

                            currentCell = workSheet.Cells[currentRow, currentColumn];
                            currentCell.Value = transaction.OriginalStake;
                            currentCell.Style = twoDecimalNumberStyle_OddRow;
                            currentColumn++;

                            currentCell = workSheet.Cells[currentRow, currentColumn];
                            currentCell.Value = transaction.OriginalWinLoss;
                            currentCell.Style = twoDecimalNumberStyle_OddRow;
                            currentColumn++;

                            currentCell = workSheet.Cells[currentRow, currentColumn];
                            currentCell.Value = transaction.BaseStake;
                            currentCell.Style = twoDecimalNumberStyle_OddRow;
                            currentColumn++;

                            currentCell = workSheet.Cells[currentRow, currentColumn];
                            currentCell.Value = transaction.BaseWinLoss;
                            currentCell.Style = twoDecimalNumberStyle_OddRow;
                            currentColumn++;

                            currentCell = workSheet.Cells[currentRow, currentColumn];
                            currentCell.Value = transaction.CompanyStake;
                            currentCell.Style = twoDecimalNumberStyle_OddRow;
                            currentColumn++;

                            currentCell = workSheet.Cells[currentRow, currentColumn];
                            currentCell.Value = transaction.CompanyWinLoss;
                            currentCell.Style = twoDecimalNumberStyle_OddRow;
                            currentColumn++;

                            currentCell = workSheet.Cells[currentRow, currentColumn];
                            currentCell.Value = transaction.Selection;
                            currentCell.Style = intStyle_OddRow;
                            currentColumn++;

                            currentCell = workSheet.Cells[currentRow, currentColumn];
                            currentCell.Value = transaction.WinOdds;
                            currentCell.Style = fourDecimalNumberStyle_OddRow;
                            currentColumn++;
                            currentRow++;

                            //// NormalOddRow
                        }
                    }

                    if (currentInfoIndexInSheet == maxItemsInSheet &&
                       sheetIndex < totalSheet)
                    {
                        this.ExportTransactionRow(
                            ref workSheet,
                            startRow,
                            startColumn,
                            ref currentRow,
                            fullReport.Total,
                            ReportRowType.GrandTotal,
                            textStyle,
                            intStyle,
                            twoDecimalNumberStyle,
                            fourDecimalNumberStyle);
                        sheetIndex++;
                        currentRow = 0;
                        currentInfoIndexInSheet = 0;
                        workSheet = excelFile.Worksheets.Add(string.Format("{0} - {1}", sheetName, sheetIndex));
                        this.ExportReportHeader(
                            ref workSheet,
                            startRow,
                            startColumn,
                            ref currentRow,
                            criteria);
                    }
                }

                this.ExportTransactionRow(
                    ref workSheet,
                    startRow,
                    startColumn,
                    ref currentRow,
                    fullReport.Total,
                    ReportRowType.GrandTotal,
                    textStyle,
                    intStyle,
                    twoDecimalNumberStyle,
                    fourDecimalNumberStyle);
            }
            else
            {
                workSheet.ExportEmptyReport(startRow, startColumn, ref currentRow, startColumn + 11);
            }

            //// MakeExcelFile

            var reportName = string.Format(
                SpecialResources.ParlayWon_ReportName,
                criteria.WinlostDate.ToShortFormatedDateWithSlash().Replace('/', '_'));
            excelFile.ReponseStream(reportName);
        }

        /// <summary>
        /// Gets the full report from data.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>The full report from data.</returns>
        [SuppressMessage("StyleCop.CSharp.StylecopPlus.Rules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        private ParlayWonTransaction GetFullReportFromDatabase(GetParlayWonReportCriteria criteria)
        {
            var rawData = GetReportData<dynamic>(criteria);
            var fullReport = new ParlayWonTransaction
            {
                TransactionList = new List<ParlayWonTransaction>(),
                ReportItemLength = rawData.Count(),
                Total = new ParlayWonTransaction
                {
                    UserName = Labels.Total
                }
            };

            foreach (var row in rawData)
            {
                ParlayWonTransaction transaction = new ParlayWonTransaction
                {
                    RefNo = Convert.ToUInt64(row.transid),
                    UserName = Converter.ToString2(row.username),
                    Currency = Converter.ToString2(row.Currency),
                    Exchange = Converter.ToDecimal(row.exchange),
                    OriginalWinLoss = Converter.ToDecimal(row.Winlost),
                    BaseWinLoss = Converter.ToDecimal(row.Winlost_RM),
                    CompanyWinLoss = Converter.ToDecimal(row.CompanyWinlost_RM),
                    Selection = Converter.ToInt(row.selection),
                    WinOdds = Converter.ToDecimal(row.modds)
                };

                switch (criteria.Bettype)
                {
                    case BetTypeConstants.MixParlay:
                        transaction.OriginalStake = Converter.ToDecimal(row.stake);
                        transaction.BaseStake = Converter.ToDecimal(row.stake_RM);
                        transaction.CompanyStake = Converter.ToDecimal(row.CompanyStake_RM);
                        break;

                    default:
                        transaction.OriginalStake = Converter.ToDecimal(row.actual_stake);
                        transaction.BaseStake = Converter.ToDecimal(row.actual_stake_RM);
                        transaction.CompanyStake = Converter.ToDecimal(row.CompanyActual_stake_RM);
                        break;
                }

                fullReport.TransactionList.Add(transaction);
                fullReport.SumDataToTotal(transaction);
            }

            return fullReport;
        }

        /// <summary>
        /// Gets the name of the sorted column.
        /// </summary>
        /// <param name="sortBy">The sort by.</param>
        /// <returns>The name of the sorted column.</returns>
        private string GetSortedColumnName(string sortBy)
        {
            var columnName = string.Empty;
            switch (sortBy)
            {
                case "BaseWinLoss":
                    columnName = "BaseWinLoss";
                    break;

                case "BaseCompanyWinLoss":
                    columnName = "CompanyWinLoss";
                    break;
            }

            return columnName;
        }

        /// <summary>
        /// Exports the report header.
        /// </summary>
        /// <param name="worksheet">The worksheet.</param>
        /// <param name="startRow">The start row.</param>
        /// <param name="startColumn">The start column.</param>
        /// <param name="currentRow">The current row.</param>
        /// <param name="criteria">The criteria.</param>
        [SuppressMessage("StyleCop.CSharp.StylecopPlus.Rules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        private void ExportReportHeader(
            ref ExcelWorksheet worksheet,
            int startRow,
            int startColumn,
            ref int currentRow,
            GetParlayWonReportCriteria criteria)
        {
            //// ExcelTypes

            var titleColor = ExcelExtensions.HeaderTitleColor;
            var headerColor = ExcelExtensions.HeaderTitleColor;
            var currentColumn = startColumn;
            CellRange currentCellRange;

            //// ExcelTypes
            //// Filter

            currentColumn = startColumn;
            for (int colIndex = 0; colIndex < 12; colIndex++)
            {
                worksheet.Columns[startColumn + colIndex].Width = 140 * ExcelExtensions.FontWidth;
            }

            worksheet.Columns[startColumn + 2].Width = 90 * ExcelExtensions.FontWidth;
            worksheet.Columns[startColumn + 3].Width = 90 * ExcelExtensions.FontWidth;
            worksheet.Columns[startColumn + 10].Width = 90 * ExcelExtensions.FontWidth;
            worksheet.Columns[startColumn + 11].Width = 90 * ExcelExtensions.FontWidth;

            var title = string.Format(
                SpecialResources.ParlayWon_ReportTitle,
                criteria.BetTypeName,
                criteria.SportName,
                criteria.WinlostDate.ToShortFormatedDate());

            currentColumn = startColumn;
            currentCellRange = worksheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow + 1, currentColumn + 11);
            currentCellRange.Merged = true;
            currentCellRange.Value = title;
            currentCellRange.Style.FormatTitleCell(titleColor);
            currentRow += 2;

            //// Filter

            worksheet.ExportEmptyRow(startRow, startColumn, ref currentRow, startColumn + 11);

            //// Title

            // First Row
            currentColumn = startColumn;
            currentCellRange = worksheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow + 1, currentColumn);
            currentCellRange.Merged = true;
            currentCellRange.Value = Labels.Ref_No;
            currentCellRange.Style.FormatTitleCell(headerColor);
            currentColumn++;

            currentCellRange = worksheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow + 1, currentColumn);
            currentCellRange.Merged = true;
            currentCellRange.Value = Labels.UserName;
            currentCellRange.Style.FormatTitleCell(headerColor);
            currentColumn++;

            currentCellRange = worksheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow + 1, currentColumn);
            currentCellRange.Merged = true;
            currentCellRange.Value = Labels.Currency;
            currentCellRange.Style.FormatTitleCell(headerColor);
            currentColumn++;

            currentCellRange = worksheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow + 1, currentColumn);
            currentCellRange.Merged = true;
            currentCellRange.Value = Labels.Exchange;
            currentCellRange.Style.FormatTitleCell(headerColor);
            currentColumn++;

            currentCellRange = worksheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow, currentColumn + 1);
            currentCellRange.Merged = true;
            currentCellRange.Value = Labels.Original_Currency;
            currentCellRange.Style.FormatTitleCell(headerColor);
            currentColumn += 2;

            currentCellRange = worksheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow, currentColumn + 3);
            currentCellRange.Merged = true;
            currentCellRange.Value = Labels.Base_Currency;
            currentCellRange.Style.FormatTitleCell(headerColor);
            currentColumn += 4;

            currentCellRange = worksheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow + 1, currentColumn);
            currentCellRange.Merged = true;
            currentCellRange.Value = Labels.Selection;
            currentCellRange.Style.FormatTitleCell(headerColor);
            currentColumn++;

            currentCellRange = worksheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow + 1, currentColumn);
            currentCellRange.Merged = true;
            currentCellRange.Value = Labels.Win_Odds;
            currentCellRange.Style.FormatTitleCell(headerColor);
            currentColumn++;
            currentRow++;

            // Second Row
            currentColumn = startColumn + 4;
            currentCellRange = worksheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow, currentColumn);
            currentCellRange.Merged = true;
            currentCellRange.Value = Labels.Stake;
            currentCellRange.Style.FormatTitleCell(headerColor);
            currentColumn++;

            currentCellRange = worksheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow, currentColumn);
            currentCellRange.Merged = true;
            currentCellRange.Value = Labels.Win_Loss;
            currentCellRange.Style.FormatTitleCell(headerColor);
            currentColumn++;

            currentCellRange = worksheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow, currentColumn);
            currentCellRange.Merged = true;
            currentCellRange.Value = Labels.Stake;
            currentCellRange.Style.FormatTitleCell(headerColor);
            currentColumn++;

            currentCellRange = worksheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow, currentColumn);
            currentCellRange.Merged = true;
            currentCellRange.Value = Labels.Win_Loss;
            currentCellRange.Style.FormatTitleCell(headerColor);
            currentColumn++;

            currentCellRange = worksheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow, currentColumn);
            currentCellRange.Merged = true;
            currentCellRange.Value = Labels.Company_Stake;
            currentCellRange.Style.FormatTitleCell(headerColor);
            currentColumn++;

            currentCellRange = worksheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow, currentColumn);
            currentCellRange.Merged = true;
            currentCellRange.Value = Labels.Company_Win_Loss;
            currentCellRange.Style.FormatTitleCell(headerColor);
            currentColumn++;
            currentRow++;

            //// Title
        }

        /// <summary>
        /// Exports the transaction row.
        /// </summary>
        /// <param name="workSheet">The work sheet.</param>
        /// <param name="startRow">The start row.</param>
        /// <param name="startColumn">The start column.</param>
        /// <param name="currentRow">The current row.</param>
        /// <param name="transaction">The transaction.</param>
        /// <param name="rowType">Type of the row.</param>
        /// <param name="textStyle">The text style.</param>
        /// <param name="intStyle">The INT style.</param>
        /// <param name="twoDecimalNumberStyle">The two decimal number style.</param>
        /// <param name="fourDecimalNumberStyle">The four decimal number style.</param>
        [SuppressMessage("StyleCop.CSharp.StylecopPlus.Rules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        private void ExportTransactionRow(
            ref ExcelWorksheet workSheet,
            int startRow,
            int startColumn,
            ref int currentRow,
            ParlayWonTransaction transaction,
            ReportRowType rowType,
            CellStyle textStyle,
            CellStyle intStyle,
            CellStyle twoDecimalNumberStyle,
            CellStyle fourDecimalNumberStyle)
        {
            ExcelCell currentCell;
            CellRange currentCellRange;

            var currentColumn = startColumn;
            var isNormalRow = rowType == ReportRowType.NormalEvenRow ||
                rowType == ReportRowType.NormalOddRow;
            if (isNormalRow)
            {
                currentCell = workSheet.Cells[currentRow, currentColumn];
                currentCell.Value = transaction.RefNo;
                currentCell.Style = textStyle;
                currentCell.Style.HorizontalAlignment = HorizontalAlignmentStyle.Center;
                currentColumn++;

                currentCell = workSheet.Cells[currentRow, currentColumn];
                currentCell.Value = transaction.UserName;
                currentCell.Style = textStyle;
                currentColumn++;

                currentCell = workSheet.Cells[currentRow, currentColumn];
                currentCell.Value = transaction.Currency;
                currentCell.Style = textStyle;
                currentCell.Style.HorizontalAlignment = HorizontalAlignmentStyle.Center;
                currentColumn++;

                currentCell = workSheet.Cells[currentRow, currentColumn];
                currentCell.Value = transaction.Exchange;
                currentCell.Style = fourDecimalNumberStyle;
                currentColumn++;
            }
            else
            {
                currentCellRange = workSheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow, currentColumn + 3);
                currentCellRange.Merged = true;
                currentCellRange.Value = transaction.UserName;
                currentCellRange.Style = textStyle;
                currentCellRange.Style.HorizontalAlignment = HorizontalAlignmentStyle.Right;
                currentColumn += 4;
            }

            currentCell = workSheet.Cells[currentRow, currentColumn];
            if (isNormalRow)
            {
                currentCell.Value = transaction.OriginalStake;
            }

            currentCell.Style = twoDecimalNumberStyle;
            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            if (isNormalRow)
            {
                currentCell.Value = transaction.OriginalWinLoss;
            }

            currentCell.Style = twoDecimalNumberStyle;
            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Value = transaction.BaseStake;
            currentCell.Style = twoDecimalNumberStyle;
            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Value = transaction.BaseWinLoss;
            currentCell.Style = twoDecimalNumberStyle;
            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Value = transaction.CompanyStake;
            currentCell.Style = twoDecimalNumberStyle;
            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Value = transaction.CompanyWinLoss;
            currentCell.Style = twoDecimalNumberStyle;
            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            if (isNormalRow)
            {
                currentCell.Value = transaction.Selection;
            }

            currentCell.Style = intStyle;
            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            if (isNormalRow)
            {
                currentCell.Value = transaction.WinOdds;
            }

            currentCell.Style = fourDecimalNumberStyle;
            currentColumn++;
            currentRow++;
        }

        //// ExportExcel
    }
}