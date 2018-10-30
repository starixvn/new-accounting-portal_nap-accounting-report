namespace Fanex.AccountingReports.Misc.Services.Implement
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using ExcelUtil.Spreadsheet;
    using Fanex.AccountingReports.Core.Extensions;
    using Fanex.AccountingReports.Core.Services.Impl;
    using Fanex.AccountingReports.Core.Web;
    using Fanex.AccountingReports.Misc.Constants;
    using Fanex.AccountingReports.Misc.Criteria.CreditSitesBetCounts;
    using Fanex.AccountingReports.Misc.Models;
    using Fanex.AccountingReports.Misc.Resources;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class CreditSitesBetCountsService : ServiceBase
    {
        private enum ReportRowType
        {
            /// <summary>
            /// The normal row.
            /// </summary>
            NormalRow = 1,

            /// <summary>
            /// The total.
            /// </summary>
            Total = 2,

            /// <summary>
            /// The grand total.
            /// </summary>
            GrandTotal = 3
        }

        /// <summary>
        /// Gets the report.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>The report.</returns>
        public Transaction GetReport(GetCreditSitesBetCountsCriteria criteria)
        {
            try
            {
                if (criteria.IsValid())
                {
                    var rawData = GetReportData<dynamic>(criteria);
                    var report = new Transaction
                    {
                        TransactionList = new List<Transaction>(),
                        Total = new Transaction
                        {
                            SiteName = Labels.Total
                        }
                    };

                    foreach (var row in rawData)
                    {
                        Transaction transaction = new Transaction
                        {
                            SiteName = row.site,
                            BetCount = row.betcount
                        };

                        report.TransactionList.Add(transaction);
                        report.Total.BetCount += transaction.BetCount;
                    }

                    SessionManager.Set<Transaction>(MiscConstants.CreditSiteBetCountsSessionKey, report);
                    return report;
                }
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }

            SessionManager.Set<Transaction>(MiscConstants.CreditSiteBetCountsSessionKey, null);
            return null;
        }

        #region ExportExcel

        /// <summary>
        /// Exports the report.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        public void ExportReport(GetCreditSitesBetCountsCriteria criteria)
        {
            try
            {
                var report = SessionManager.Get<Transaction>(MiscConstants.CreditSiteBetCountsSessionKey);
                if (report == null)
                {
                    report = this.GetReport(criteria);
                }

                ExcelFile excelFile = new ExcelFile();
                ExcelWorksheetCollection worksheets = excelFile.Worksheets;
                string sheetName = MiscResources.CreditSitesBetCounts_SheetName;
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
                    criteria,
                    report);

                excelFile.ReponseStream(string.Format(
                    MiscResources.CreditSitesBetCounts_ReportName,
                    criteria.Datestart.ToShortFormatedDateWithSlash().Replace('/', '_'),
                    criteria.Dateend.ToShortFormatedDateWithSlash().Replace('/', '_')));
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
        /// <param name="criteria">The criteria.</param>
        private void ExportReportHeader(
            ref ExcelWorksheet worksheet,
            int startRow,
            int startColumn,
            ref int currentRow,
            GetCreditSitesBetCountsCriteria criteria)
        {
            var titleColor = ExcelExtensions.HeaderTitleColor;
            var headerColor = ExcelExtensions.HeaderTitleColor;
            var currentColumn = startColumn;
            CellRange currentCellRange;

            currentColumn = startColumn;
            for (int colIndex = 0; colIndex < 2; colIndex++)
            {
                worksheet.Columns[startColumn + colIndex].Width = 400 * ExcelExtensions.FontWidth;
            }

            var title = string.Format(
                MiscResources.CreditSitesBetCounts_ReportTitle,
                criteria.Datestart.ToShortFormatedDate(),
                criteria.Dateend.ToShortFormatedDate());

            currentColumn = startColumn;
            currentCellRange = worksheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow + 1, currentColumn + 1);
            currentCellRange.Merged = true;
            currentCellRange.Value = title;
            currentCellRange.Style.FormatTitleCell(titleColor);
            currentRow += 2;

            worksheet.ExportEmptyRow(startRow, startColumn, ref currentRow, startColumn + 1);

            currentColumn = startColumn;
            currentCellRange = worksheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow, currentColumn);
            currentCellRange.Merged = true;
            currentCellRange.Value = Labels.Site;
            currentCellRange.Style.FormatTitleCell(headerColor);
            currentColumn++;

            currentCellRange = worksheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow, currentColumn);
            currentCellRange.Merged = true;
            currentCellRange.Value = Labels.Bet_Count;
            currentCellRange.Style.FormatTitleCell(headerColor);
            currentColumn++;
            currentRow++;
        }

        /// <summary>
        /// Exports the content of the report.
        /// </summary>
        /// <param name="worksheet">The worksheet.</param>
        /// <param name="startRow">The start row.</param>
        /// <param name="startColumn">The start column.</param>
        /// <param name="currentRow">The current row.</param>
        /// <param name="criteria">The criteria.</param>
        /// <param name="report">The report.</param>
        private void ExportReportContent(
            ref ExcelWorksheet worksheet,
           int startRow,
           int startColumn,
           ref int currentRow,
           GetCreditSitesBetCountsCriteria criteria,
           Transaction report)
        {
            if (report != null &&
                report.TransactionList != null &&
                report.TransactionList.Count > 0)
            {
                var siteLength = report.TransactionList.Count;
                for (int i = 0; i < siteLength; i++)
                {
                    this.ExportTransactionRow(
                        ref worksheet,
                           startRow,
                           startColumn,
                           ref currentRow,
                           report.TransactionList[i],
                           ReportRowType.NormalRow);
                }

                this.ExportTransactionRow(
                    ref worksheet,
                    startRow,
                    startColumn,
                    ref currentRow,
                    report.Total,
                    ReportRowType.GrandTotal);
            }
            else
            {
                worksheet.ExportEmptyReport(startRow, startColumn, ref currentRow, startColumn + 1);
            }
        }

        /// <summary>
        /// Exports the transaction row.
        /// </summary>
        /// <param name="worksheet">The worksheet.</param>
        /// <param name="startRow">The start row.</param>
        /// <param name="startColumn">The start column.</param>
        /// <param name="currentRow">The current row.</param>
        /// <param name="transaction">The transaction.</param>
        /// <param name="rowType">Type of the row.</param>
        private void ExportTransactionRow(
            ref ExcelWorksheet worksheet,
           int startRow,
           int startColumn,
           ref int currentRow,
           Transaction transaction,
           ReportRowType rowType)
        {
            var textStyle = ExcelExtensions.GetCellTextStyle(ExcelExtensions.BackGroundColor, false);
            var intStyle = ExcelExtensions.GetCellNumberStyle(string.Empty, ExcelExtensions.BackGroundColor, false, false, false);

            switch (rowType)
            {
                case ReportRowType.NormalRow:
                    textStyle = ExcelExtensions.GetCellTextStyle(ExcelExtensions.BackGroundColor, false);
                    intStyle = ExcelExtensions.GetCellNumberStyle(string.Empty, ExcelExtensions.BackGroundColor, false, false, false);
                    intStyle.HorizontalAlignment = HorizontalAlignmentStyle.Right;
                    break;

                case ReportRowType.GrandTotal:
                    textStyle = ExcelExtensions.GetCellTextStyle(ExcelExtensions.GrandTotalRowColor, true);
                    textStyle.Borders.SetBorders(MultipleBorders.Top, Color.FromArgb(255, 102, 0), LineStyle.Medium);
                    intStyle = ExcelExtensions.GetCellNumberStyle(string.Empty, ExcelExtensions.GrandTotalRowColor, true, false, false);
                    intStyle.HorizontalAlignment = HorizontalAlignmentStyle.Right;
                    intStyle.Borders.SetBorders(MultipleBorders.Top, Color.FromArgb(255, 102, 0), LineStyle.Medium);
                    break;
            }

            var currentColumn = startColumn;
            CellRange currentCellRange;

            currentCellRange = worksheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow, currentColumn);
            currentCellRange.Merged = true;
            currentCellRange.Value = transaction.SiteName;
            currentCellRange.Style = textStyle;
            if (rowType == ReportRowType.GrandTotal)
            {
                currentCellRange.Style.HorizontalAlignment = HorizontalAlignmentStyle.Center;
            }

            currentColumn++;

            currentCellRange = worksheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow, currentColumn);
            currentCellRange.Merged = true;
            currentCellRange.Value = transaction.BetCount;
            currentCellRange.Style = intStyle;
            currentColumn++;
            currentRow++;
        }

        #endregion ExportExcel
    }
}