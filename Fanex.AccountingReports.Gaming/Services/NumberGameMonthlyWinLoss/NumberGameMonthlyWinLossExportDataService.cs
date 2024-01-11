namespace Fanex.AccountingReports.Gaming.Services.Implement.NumberGameMonthlyWinLoss
{
    using System.Diagnostics.CodeAnalysis;
    using ExcelUtil.Spreadsheet;
    using Fanex.AccountingReports.Core.Constants;
    using Fanex.AccountingReports.Core.Extensions;
    using Fanex.AccountingReports.Core.Services.Impl;
    using Fanex.AccountingReports.Core.Web;
    using Fanex.AccountingReports.Gaming.Criteria.NumberGameMonthlyWinLoss;
    using Fanex.AccountingReports.Gaming.Models.NumberGameMonthlyWinLoss;
    using Fanex.AccountingReports.Gaming.Resources;
    using Fanex.Common;

    public class NumberGameMonthlyWinLossExportDataService : ServiceBase
    {
        private string _numberGameMonthlyWinLossReportSessionKey = "Fanex.AccountingReports.Gaming.NumberGameMonthlyWinLoss." +
            (AppSession.CurrentUser != null ? AppSession.CurrentUser.Username : string.Empty);

        private NumberGameMonthlyWinLossService _numberGameMonthlyWinLossService;

        public NumberGameMonthlyWinLossExportDataService()
        {
            this._numberGameMonthlyWinLossService = new NumberGameMonthlyWinLossService();
        }

        public void ExportNumberGameMonthlyWinLossReport(NumberGameMonthlyWinLossCriteria getNumberGameMonthlyWinLossCriteria)
        {
            var fullReport = SessionManager.Get<NumberGameMonthlyWinLoss>(_numberGameMonthlyWinLossReportSessionKey);
            if (fullReport == null)
            {
                fullReport = _numberGameMonthlyWinLossService.GetNumberGameMonthlyWinLoss(getNumberGameMonthlyWinLossCriteria);
            }

            ExcelFile excelFile = new ExcelFile();
            ExcelWorksheetCollection worksheets = excelFile.Worksheets;
            string sheetName = GamingResources.NumberGameMonthlyWinLoss_SheetName;
            ExcelWorksheet workSheet = worksheets.Add(sheetName);

            int startRow = 0;
            int startColumn = 0;
            var currentRow = startRow;

            this.ExportReportHeader(
                ref workSheet,
                startRow,
                startColumn,
                ref currentRow,
                getNumberGameMonthlyWinLossCriteria);

            this.ExportReportContent(
                ref workSheet,
                startRow,
                startColumn,
                ref currentRow,
                fullReport);

            string reportName = string.Format(
            GamingResources.NumberGameMonthlyWinLoss_ReportName,
            getNumberGameMonthlyWinLossCriteria.FromDate.ToShortFormatedDateWithSlash().Replace('/', '_'),
            getNumberGameMonthlyWinLossCriteria.ToDate.ToShortFormatedDateWithSlash().Replace('/', '_'));
            excelFile.ReponseStream(reportName);
        }

        
        private void ExportReportHeader(
            ref ExcelWorksheet workSheet,
            int startRow,
            int startColumn,
            ref int currentRow,
            NumberGameMonthlyWinLossCriteria getNumberGameMonthlyWinLossCriteria)
        {
            var titleColor = ExcelExtensions.HeaderTitleColor;
            var headerColor = ExcelExtensions.HeaderTitleColor;
            workSheet.Columns[startColumn].Width = 160 * ExcelExtensions.FontWidth;
            workSheet.Columns[startColumn + 1].Width = 180 * ExcelExtensions.FontWidth;
            workSheet.Columns[startColumn + 2].Width = 160 * ExcelExtensions.FontWidth;
            workSheet.Columns[startColumn + 3].Width = 160 * ExcelExtensions.FontWidth;
            workSheet.Columns[startColumn + 4].Width = 160 * ExcelExtensions.FontWidth;
            workSheet.Columns[startColumn + 5].Width = 160 * ExcelExtensions.FontWidth;
            workSheet.Columns[startColumn + 6].Width = 160 * ExcelExtensions.FontWidth;
            var currentColumn = startColumn;
            CellRange currentCellRange;

            currentColumn = startColumn;
            var title = string.Format(
                                    GamingResources.NumberGameMonthlyWinLoss_ReportTitle,
                                    getNumberGameMonthlyWinLossCriteria.FromDate.ToShortFormatedDate(),
                                    getNumberGameMonthlyWinLossCriteria.ToDate.ToShortFormatedDate());
            currentColumn = startColumn;
            currentCellRange = workSheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow, currentColumn + 6);
            currentCellRange.Merged = true;
            currentCellRange.Value = title;
            currentCellRange.Style.FormatTitleCell(titleColor);
            currentRow++;

            // First Row
            currentColumn = startColumn;
            currentCellRange = workSheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow + 1, currentColumn);
            currentCellRange.Merged = true;
            currentCellRange.Value = Labels.Month;
            currentCellRange.Style.FormatTitleCell(headerColor);
            currentColumn++;

            currentCellRange = workSheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow + 1, currentColumn);
            currentCellRange.Merged = true;
            currentCellRange.Value = Labels.Member_Win_Loss;
            currentCellRange.Style.FormatTitleCell(headerColor);
            currentColumn++;

            currentCellRange = workSheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow + 1, currentColumn);
            currentCellRange.Merged = true;
            currentCellRange.Value = "SMA Win Loss";
            currentCellRange.Style.FormatTitleCell(headerColor);
            currentColumn++;

            currentCellRange = workSheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow + 1, currentColumn);
            currentCellRange.Merged = true;
            currentCellRange.Value = Labels.Commission;
            currentCellRange.Style.FormatTitleCell(headerColor);
            currentColumn++;

            currentCellRange = workSheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow + 1, currentColumn);
            currentCellRange.Merged = true;
            currentCellRange.Value = "House Real Income";
            currentCellRange.Style.FormatTitleCell(headerColor);
            currentColumn++;

            currentCellRange = workSheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow + 1, currentColumn);
            currentCellRange.Merged = true;
            currentCellRange.Value = "House Real Turnover";
            currentCellRange.Style.FormatTitleCell(headerColor);
            currentColumn++;

            currentCellRange = workSheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow + 1, currentColumn);
            currentCellRange.Merged = true;
            currentCellRange.Value = Labels.Margin;
            currentCellRange.Style.FormatTitleCell(headerColor);
            currentColumn++;

            currentRow += 2;
        }

        private void ExportReportContent(
            ref ExcelWorksheet workSheet,
            int startRow,
            int startColumn,
            ref int currentRow,
            NumberGameMonthlyWinLoss report)
        {
            if (report != null &&
           report.ReportRows != null &&
           report.ReportRows.Count > 0)
            {
                var reportLength = report.ReportRows.Count;

                foreach (var row in report.ReportRows)
                {
                    this.ExportReportDetail(
                        ref workSheet,
                        startRow,
                        startColumn,
                        ref currentRow,
                        row);
                }

                this.ExportFooterReport(ref workSheet, startRow, startColumn, ref currentRow, report);
            }
            else
            {
                workSheet.ExportEmptyReport(startRow, startColumn, ref currentRow, startColumn + 6);
            }
        }

        
        private void ExportReportDetail(
            ref ExcelWorksheet workSheet,
            int startRow,
            int startColumn,
            ref int currentRow,
            NumberGameMonthlyWinLoss detail)
        {
            ExcelCell currentCell;
            var textStyle = ExcelExtensions.GetCellTextStyle(ExcelExtensions.BackGroundColor, false);
            var twoDecimalNumberStyle = ExcelExtensions.GetCellNumberStyle(
                NumberFormatConstants.TwoDecimalDigitFormat,
                ExcelExtensions.BackGroundColor,
                false,
                false,
                false);
            var twoDecimalPositiveNumberStyle = ExcelExtensions.GetCellNumberStyle(
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

            var currentColumn = startColumn;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Value = string.Format("{0}/{1}", detail.Month, detail.Year);
            currentCell.Style = textStyle;
            currentCell.Style.HorizontalAlignment = HorizontalAlignmentStyle.Center;
            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Value = detail.Winloss;
            currentCell.Style = twoDecimalPositiveNumberStyle;
            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Value = detail.Position;
            currentCell.Style = twoDecimalNumberStyle;
            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Value = detail.Commission;
            currentCell.Style = twoDecimalNumberStyle;
            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Value = detail.Linetotal;
            currentCell.Style = twoDecimalPositiveNumberStyle;
            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Value = detail.Turnover;
            currentCell.Style = twoDecimalNumberStyle;
            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            if (detail.Margin == Labels.NotAvailable)
            {
                currentCell.Value = Labels.NotAvailable;
                currentCell.Style = textStyle;
            }
            else
            {
                currentCell.Value = Converter.ToDecimal(detail.Margin);
                currentCell.Style = twoDecimalNumberMarginStyle;
            }

            currentColumn++;

            currentRow++;
        }

        
        private void ExportFooterReport(
            ref ExcelWorksheet worksheet,
            int startRow,
            int startColumn,
            ref int currentRow,
            NumberGameMonthlyWinLoss grandTotal)
        {
            var grandTotalSeperatorColor = ExcelExtensions.GrandTotalSeparatorColor;
            var textGrandTotalStyle = ExcelExtensions.GetCellTextStyle(ExcelExtensions.GrandTotalRowColor, true);
            textGrandTotalStyle.Borders.SetBorders(MultipleBorders.Top, grandTotalSeperatorColor, LineStyle.Medium);
            var twoDecimalPositiveNumberStyle = ExcelExtensions.GetCellNumberStyle(
               NumberFormatConstants.TwoDecimalDigitFormat,
               ExcelExtensions.GrandTotalRowColor,
               true,
               true,
               false);
            twoDecimalPositiveNumberStyle.Borders.SetBorders(MultipleBorders.Top, grandTotalSeperatorColor, LineStyle.Medium);
            var twoDecimalNumberStyle = ExcelExtensions.GetCellNumberStyle(
                NumberFormatConstants.TwoDecimalDigitFormat,
                ExcelExtensions.GrandTotalRowColor,
                true,
                false,
                false);
            twoDecimalNumberStyle.Borders.SetBorders(MultipleBorders.Top, grandTotalSeperatorColor, LineStyle.Medium);
            var twoDecimalNumberMarginStyle = ExcelExtensions.GetCellNumberStyle(
                NumberFormatConstants.TwoDecimalDigitPercentFormat,
                ExcelExtensions.GrandTotalRowColor,
                true,
                true,
                false);
            twoDecimalNumberMarginStyle.Borders.SetBorders(MultipleBorders.Top, grandTotalSeperatorColor, LineStyle.Medium);

            var currentColumn = startColumn;
            ExcelCell grandCellRange;

            grandCellRange = worksheet.Cells[currentRow, currentColumn];
            grandCellRange.Value = Labels.Total;
            grandCellRange.Style = textGrandTotalStyle;
            grandCellRange.Style.HorizontalAlignment = HorizontalAlignmentStyle.Center;
            currentColumn++;

            grandCellRange = worksheet.Cells[currentRow, currentColumn];
            grandCellRange.Value = grandTotal.Winloss;
            grandCellRange.Style = twoDecimalPositiveNumberStyle;
            currentColumn++;

            grandCellRange = worksheet.Cells[currentRow, currentColumn];
            grandCellRange.Value = grandTotal.Position;
            grandCellRange.Style = twoDecimalNumberStyle;
            currentColumn++;

            grandCellRange = worksheet.Cells[currentRow, currentColumn];
            grandCellRange.Value = grandTotal.Commission;
            grandCellRange.Style = twoDecimalNumberStyle;
            currentColumn++;

            grandCellRange = worksheet.Cells[currentRow, currentColumn];
            grandCellRange.Value = grandTotal.Linetotal;
            grandCellRange.Style = twoDecimalPositiveNumberStyle;
            currentColumn++;

            grandCellRange = worksheet.Cells[currentRow, currentColumn];
            grandCellRange.Value = grandTotal.Turnover;
            grandCellRange.Style = twoDecimalNumberStyle;
            currentColumn++;

            var marginValue = grandTotal.Turnover == 0 ? Labels.NotAvailable : (grandTotal.Linetotal / grandTotal.Turnover).ToString();

            grandCellRange = worksheet.Cells[currentRow, currentColumn];
            if (marginValue != Labels.NotAvailable)
            {
                grandCellRange.Value = Converter.ToDecimal(marginValue);
                grandCellRange.Style = twoDecimalNumberMarginStyle;
            }
            else
            {
                grandCellRange.Value = Labels.NotAvailable;
                grandCellRange.Style = textGrandTotalStyle;
            }

            currentColumn++;
        }
    }
}
