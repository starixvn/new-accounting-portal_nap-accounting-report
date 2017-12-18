namespace Fanex.AccountingReports.Misc.Services.Implement.MAMonitorTool
{
    using System.Diagnostics.CodeAnalysis;
    using ExcelUtil.Spreadsheet;
    using Fanex.AccountingReports.Core.Extensions;
    using Fanex.AccountingReports.Core.Web;
    using Fanex.AccountingReports.Misc.Criteria.MAMonitorTool;
    using Fanex.AccountingReports.Misc.Models.MAMonitorTool;
    using Fanex.AccountingReports.Misc.Resources;
    using Fanex.Common;
    using CoreConstants = Fanex.AccountingReports.Core.Constants;

    /// <summary>
    /// MA Monitor Tool Export Excel Service.
    /// </summary>
    public class MAMonitorToolExportExcelService : MAMonitorToolService
    {
        private const int MaxItemPerSheet = 64000;

        /// <summary>
        /// Exports the report.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        public override void ExportReport(GetReportCriteria criteria)
        {
            WinLossInfo report = SessionManager.Get<WinLossInfo>(ReportSessionKey);
            if (report == null || criteria.ReportBy == 2)
            {
                report = GetReport(criteria);
            }

            ExportReport(criteria, report);
        }

        /// <summary>
        /// Exports the report.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <param name="report">The report.</param>
        [SuppressMessage("StyleCop.CSharp.StylecopPlus.Rules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        private void ExportReport(GetReportCriteria criteria, WinLossInfo report)
        {
            ExcelFile excelFile = new ExcelFile();
            ExcelWorksheetCollection worksheets = excelFile.Worksheets;
            string sheetName = MiscResources.MAMonitorTool_SheetName;
            ExcelWorksheet workSheet;
            int startRow = 0;
            int startColumn = 0;
            int maxColumnIndex = 9;
            var currentRow = startRow;
            var currentColumn = startColumn;

            var backgroundColor = ExcelExtensions.BackGroundColor;
            var textStyle = ExcelExtensions.GetCellTextStyle(backgroundColor, false);
            var intNumberStyle = ExcelExtensions.GetCellNumberStyle(CoreConstants.NumberFormatConstants.IntNumberFormat, backgroundColor, false, false, false);
            var twoDecimalNegativeNumberStyle = ExcelExtensions.GetCellNumberStyle(
                CoreConstants.NumberFormatConstants.TwoDecimalDigitFormat,
                backgroundColor,
                false,
                false,
                true);

            var reportLength = report != null && report.ChildsInfo != null ?
                report.ChildsInfo.Count :
                0;
            if (reportLength > 0)
            {
                var itemPerSheet = MaxItemPerSheet;
                var totalSheet = (reportLength / itemPerSheet) + (reportLength % itemPerSheet != 0 ? 1 : 0);
                WinLossInfo info;
                for (var sheetIndex = 0; sheetIndex < totalSheet; sheetIndex++)
                {
                    workSheet = worksheets.Add(sheetName + (totalSheet > 1 ? " - " + (sheetIndex + 1) : string.Empty));
                    currentRow = 0;
                    ExportReport_Header(
                        ref workSheet,
                        startRow,
                        startColumn,
                        ref currentRow,
                        maxColumnIndex,
                        criteria);
                    var startIndex = itemPerSheet * sheetIndex;
                    var endIndex = (sheetIndex == totalSheet - 1) ?
                        reportLength :
                        (itemPerSheet * sheetIndex) + itemPerSheet;
                    for (var i = startIndex; i < endIndex; i++)
                    {
                        SetOddEvenColor_ExportExcel(
                            ref textStyle,
                            ref intNumberStyle,
                            ref twoDecimalNegativeNumberStyle,
                            (i + 1) % 2 == 0);

                        info = report.ChildsInfo[i];
                        ExportReport_WinLossRow(
                            ref workSheet,
                            currentColumn,
                            ref currentRow,
                            intNumberStyle,
                            textStyle,
                            twoDecimalNegativeNumberStyle,
                            criteria,
                            info,
                            false);
                        currentRow++;
                    }

                    SetOddEvenColor_ExportExcel(
                        ref textStyle,
                        ref intNumberStyle,
                        ref twoDecimalNegativeNumberStyle,
                        false);
                    ExportReport_GrandTotal(
                        report.GrandTotal,
                        ref workSheet,
                        startColumn,
                        ref currentRow,
                        criteria);
                    currentRow++;
                }
            }
            else
            {
                workSheet = worksheets.Add(sheetName);
                ExportReport_Header(
                    ref workSheet,
                    startRow,
                    startColumn,
                    ref currentRow,
                    maxColumnIndex,
                    criteria);
                workSheet.ExportEmptyReport(startRow, startColumn, ref currentRow, startColumn + maxColumnIndex);
            }

            var reportName = string.Format(
                MiscResources.MAMonitorTool_ReportName,
                Converter.ToDateTime(criteria.FromDate).ToShortFormatedDateWithSlash().Replace('/', '_'),
                Converter.ToDateTime(criteria.ToDate).ToShortFormatedDateWithSlash().Replace('/', '_'));
            excelFile.ReponseStream(reportName);
        }

        /// <summary>
        /// Exports the report header.
        /// </summary>
        /// <param name="workSheet">The work sheet.</param>
        /// <param name="startRow">The start row.</param>
        /// <param name="startColumn">The start column.</param>
        /// <param name="currentRow">The current row.</param>
        /// <param name="maxColumnIndex">Maximum index of the column.</param>
        /// <param name="criteria">The criteria.</param>
        [SuppressMessage("StyleCop.CSharp.StylecopPlus.Rules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        private void ExportReport_Header(
            ref ExcelWorksheet workSheet,
            int startRow,
            int startColumn,
            ref int currentRow,
            int maxColumnIndex,
            GetReportCriteria criteria)
        {
            var currentColumn = startColumn;
            CellRange currentCellRange;
            ExcelCell currentCell;
            var headerTextStyle = ExcelExtensions.GetCellTitleStyle(ExcelExtensions.HeaderTitleColor);
            workSheet.Columns[startColumn].Width = 200 * ExcelExtensions.FontWidth;
            for (int colIndex = 1; colIndex <= maxColumnIndex; colIndex++)
            {
                workSheet.Columns[startColumn + colIndex].Width = 120 * ExcelExtensions.FontWidth;
            }

            var title = string.Format(
                MiscResources.MAMonitorTool_ReportTitle,
                Converter.ToDateTime(criteria.FromDate).ToShortFormatedDate(),
                Converter.ToDateTime(criteria.ToDate).ToShortFormatedDate());
            currentColumn = startColumn;
            currentCellRange = workSheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow + 1, currentColumn + maxColumnIndex);
            currentCellRange.Merged = true;
            currentCellRange.Value = title;
            currentCellRange.Style = headerTextStyle;
            currentCellRange.Style.HorizontalAlignment = HorizontalAlignmentStyle.Center;
            currentRow += 2;

            workSheet.ExportEmptyRow(startRow, startColumn, ref currentRow, startColumn + maxColumnIndex);

            currentColumn = startColumn;
            currentCellRange = workSheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow + 1, currentColumn);
            currentCellRange.Merged = true;
            currentCellRange.Value = GetReportTypeName(criteria);
            currentCellRange.Style = headerTextStyle;
            currentColumn++;

            currentCellRange = workSheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow, currentColumn + 2);
            currentCellRange.Merged = true;
            currentCellRange.Value = Labels.Member;
            currentCellRange.Style = headerTextStyle;
            currentColumn += 3;

            currentCellRange = workSheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow, currentColumn + 1);
            currentCellRange.Merged = true;
            currentCellRange.Value = Labels.Agent;
            currentCellRange.Style = headerTextStyle;
            currentColumn += 2;

            currentCellRange = workSheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow, currentColumn + 1);
            currentCellRange.Merged = true;
            currentCellRange.Value = Labels.Master;
            currentCellRange.Style = headerTextStyle;
            currentColumn += 2;

            currentCellRange = workSheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow, currentColumn + 1);
            currentCellRange.Merged = true;
            currentCellRange.Value = Labels.Super;
            currentCellRange.Style = headerTextStyle;
            currentColumn += 2;
            currentRow++;

            currentColumn = startColumn + 1;
            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Style = headerTextStyle;
            currentCell.Value = Labels.Turnover;
            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Style = headerTextStyle;
            currentCell.Value = Labels.Win_Loss;
            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Style = headerTextStyle;
            currentCell.Value = Labels.Bet_Count;
            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Style = headerTextStyle;
            currentCell.Value = Labels.Turnover;
            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Style = headerTextStyle;
            currentCell.Value = Labels.Win_Loss;
            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Style = headerTextStyle;
            currentCell.Value = Labels.Turnover;
            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Style = headerTextStyle;
            currentCell.Value = Labels.Win_Loss;
            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Style = headerTextStyle;
            currentCell.Value = Labels.Turnover;
            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Style = headerTextStyle;
            currentCell.Value = Labels.Win_Loss;
            currentColumn++;
            currentRow++;
        }

        /// <summary>
        /// Exports the report win loss row.
        /// </summary>
        /// <param name="workSheet">The work sheet.</param>
        /// <param name="startColumn">The start column.</param>
        /// <param name="currentRow">The current row.</param>
        /// <param name="intNumberStyle">The integer number style.</param>
        /// <param name="textStyle">The text style.</param>
        /// <param name="twoDecimalNegativeNumberStyle">The two decimal negative number style.</param>
        /// <param name="criteria">The criteria.</param>
        /// <param name="info">The information.</param>
        /// <param name="isGrandTotal">If set to <c>true</c> [is grand total].</param>
        [SuppressMessage("StyleCop.CSharp.StylecopPlus.Rules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        private void ExportReport_WinLossRow(
            ref ExcelWorksheet workSheet,
            int startColumn,
            ref int currentRow,
            CellStyle intNumberStyle,
            CellStyle textStyle,
            CellStyle twoDecimalNegativeNumberStyle,
            GetReportCriteria criteria,
            WinLossInfo info,
            bool isGrandTotal)
        {
            ExcelCell currentCell;
            var currentColumn = startColumn;
            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Style = textStyle;
            currentCell.Style.HorizontalAlignment =
                isGrandTotal ? HorizontalAlignmentStyle.Center : HorizontalAlignmentStyle.Left;
            currentCell.Value = info.Name;
            currentColumn++;

            var memberWinLoss = info.ChildsInfo[0];
            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Style = twoDecimalNegativeNumberStyle;
            currentCell.Value = memberWinLoss.Turnover;
            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Style = twoDecimalNegativeNumberStyle;
            currentCell.Value = memberWinLoss.WinLoss;
            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Style = intNumberStyle;
            currentCell.Value = memberWinLoss.BetCount;
            currentColumn++;

            var downlinesMaxIndex = info.ChildsInfo.Count - 1;
            for (var i = 1; i <= downlinesMaxIndex; i++)
            {
                var winLoss = info.ChildsInfo[i];
                currentCell = workSheet.Cells[currentRow, currentColumn];
                currentCell.Style = twoDecimalNegativeNumberStyle;
                currentCell.Value = winLoss.Turnover;
                currentColumn++;

                currentCell = workSheet.Cells[currentRow, currentColumn];
                currentCell.Style = twoDecimalNegativeNumberStyle;
                currentCell.Value = winLoss.WinLoss;
                currentColumn++;
            }
        }

        /// <summary>
        /// Exports the report grand total.
        /// </summary>
        /// <param name="grandTotal">The grand total.</param>
        /// <param name="workSheet">The work sheet.</param>
        /// <param name="startColumn">The start column.</param>
        /// <param name="currentRow">The current row.</param>
        /// <param name="criteria">The criteria.</param>
        [SuppressMessage("StyleCop.CSharp.StylecopPlus.Rules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        private void ExportReport_GrandTotal(
            WinLossInfo grandTotal,
            ref ExcelWorksheet workSheet,
            int startColumn,
            ref int currentRow,
            GetReportCriteria criteria)
        {
            var grandTotalColor = ExcelExtensions.GrandTotalRowColor;
            var grandTotalSeperatorColor = ExcelExtensions.GrandTotalSeparatorColor;
            var textStyle_GrandTotal_1stRow = ExcelExtensions.GetCellTextStyle(grandTotalColor, true);
            var intNumberStyle_GrandTotal_1stRow = ExcelExtensions.GetCellNumberStyle(
                CoreConstants.NumberFormatConstants.IntNumberFormat,
                grandTotalColor,
                true,
                false,
                false);
            var twoDecimalNegativeNumberStyle_GrandTotal_1stRow = ExcelExtensions.GetCellNumberStyle(
                CoreConstants.NumberFormatConstants.TwoDecimalDigitFormat,
                grandTotalColor,
                true,
                false,
                true);
            textStyle_GrandTotal_1stRow.Borders.SetBorders(MultipleBorders.Top, grandTotalSeperatorColor, LineStyle.Medium);
            intNumberStyle_GrandTotal_1stRow.Borders.SetBorders(MultipleBorders.Top, grandTotalSeperatorColor, LineStyle.Medium);
            twoDecimalNegativeNumberStyle_GrandTotal_1stRow.Borders.SetBorders(MultipleBorders.Top, grandTotalSeperatorColor, LineStyle.Medium);

            ExportReport_WinLossRow(
                ref workSheet,
                startColumn,
                ref currentRow,
                intNumberStyle_GrandTotal_1stRow,
                textStyle_GrandTotal_1stRow,
                twoDecimalNegativeNumberStyle_GrandTotal_1stRow,
                criteria,
                grandTotal,
                true);
            currentRow++;
        }

        /// <summary>
        /// Sets the odd even color export excel.
        /// </summary>
        /// <param name="textStyle">The text style.</param>
        /// <param name="intNumberStyle">The integer number style.</param>
        /// <param name="twoDecimalNegativeNumberStyle">The two decimal negative number style.</param>
        /// <param name="isEvenRow">If set to <c>true</c> [is even row].</param>
        private void SetOddEvenColor_ExportExcel(
            ref CellStyle textStyle,
            ref CellStyle intNumberStyle,
            ref CellStyle twoDecimalNegativeNumberStyle,
            bool isEvenRow)
        {
            if (isEvenRow)
            {
                var evenRowColor = ExcelExtensions.EvenRowColor;
                textStyle.FillPattern.SetSolid(evenRowColor);
                intNumberStyle.FillPattern.SetSolid(evenRowColor);
                twoDecimalNegativeNumberStyle.FillPattern.SetSolid(evenRowColor);
            }
            else
            {
                var oddRowColor = ExcelExtensions.OddRowColor;
                textStyle.FillPattern.SetSolid(oddRowColor);
                intNumberStyle.FillPattern.SetSolid(oddRowColor);
                twoDecimalNegativeNumberStyle.FillPattern.SetSolid(oddRowColor);
            }
        }
    }
}