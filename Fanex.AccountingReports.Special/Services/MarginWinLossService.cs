namespace Fanex.AccountingReports.Special.Services.Implement.MarginReport
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using ExcelUtil.Spreadsheet;
    using Fanex.AccountingReports.Core.Constants;
    using Fanex.AccountingReports.Core.Extensions;
    using Fanex.AccountingReports.Core.Services.Impl;
    using Fanex.AccountingReports.Core.Web;
    using Fanex.AccountingReports.Special.Criteria.MarginReport;
    using Fanex.AccountingReports.Special.Models.MarginReport;
    using Fanex.AccountingReports.Special.Resources;
    using Fanex.Common;

    /// <summary>
    /// Implement Margin Win Loss Service interface.
    /// </summary>
    public class MarginWinLossService : ServiceBase
    {
        public static readonly string MarginReportSessionKey = "Fanex.AccountingReports.Special.MarginReport";

        /// <summary>
        /// Get Data Margin Report.
        /// </summary>
        /// <param name="criteria">The Get margin win loss criteria.</param>
        /// <returns>The margin win loss model.</returns>
        public MarginWinLoss GetDataMarginReport(GetMarginWinLossCriteria criteria)
        {
            var marginWinLoss = new MarginWinLoss();
            var rawData = this.GetReportData<dynamic>(criteria);
            if (rawData.Count() == 0)
            {
                return marginWinLoss;
            }

            foreach (var row in rawData)
            {
                var marginDetail = new MarginWinLoss()
                {
                    UserName = Converter.ToString2(row.username),
                    BettypeId = Converter.ToInt(row.bettype),
                    BettypeName = CacheRef.GetBettype(row.bettype),
                    WinningAmount = Converter.ToDecimal(row.totalwon),
                    Turnover = Converter.ToDecimal(row.turnover),
                    Ratio = Converter.ToDecimal(row.ratio),
                    TotalBet = Converter.ToInt(row.n)
                };
                marginWinLoss.ChildsInfo.Add(marginDetail);
            }

            return marginWinLoss;
        }

        /// <summary>
        /// Export excel for margin report.
        /// </summary>
        /// <param name="criteria">The get margin win loss criteria. </param>
        [SuppressMessage("StyleCop.CSharp.StylecopPlus.Rules",
           "SP2101:MethodMustNotContainMoreLinesThan",
           Justification = "Reviewed. Suppression is OK here.")]
        public void ExcelReport(GetMarginWinLossCriteria criteria)
        {
            var marginWinLoss = new MarginWinLoss();
            marginWinLoss = this.GetDataMarginReport(criteria);

            ExcelFile excelFile = new ExcelFile();
            ExcelWorksheetCollection worksheets = excelFile.Worksheets;
            string sheetName = SpecialResources.MarginReport_SheetName;
            ExcelWorksheet workSheet = worksheets.Add(sheetName);

            var titleColor = ExcelExtensions.HeaderTitleColor;
            int startRow = 0;
            int startColumn = 0;
            var currentRow = startRow;

            var currentColumn = startColumn;
            CellRange currentCellRange;

            workSheet.Columns[0].Width = 80 * ExcelExtensions.FontWidth;
            workSheet.Columns[6].Width = 80 * ExcelExtensions.FontWidth;
            currentColumn = startColumn;
            for (int colIndex = 1; colIndex < 7; colIndex++)
            {
                workSheet.Columns[startColumn + colIndex].Width = 120 * ExcelExtensions.FontWidth;
            }

            var title = string.Format(
                SpecialResources.Margin_ReportTitle,
                criteria.ActualStake);

            currentColumn = startColumn;
            currentCellRange = workSheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow, currentColumn + 6);
            currentCellRange.Merged = true;
            currentCellRange.Value = title;
            currentCellRange.Style.FormatTitleCell(titleColor);
            currentRow++;
            if (marginWinLoss != null && marginWinLoss.ChildsInfo.Count > 0)
            {
                workSheet.ExportEmptyRow(startRow, startColumn, ref currentRow, startColumn + 6);

                this.MarginReportExportHeader(ref workSheet, startColumn, ref currentRow);
                this.MarginReportExportBody(ref workSheet, marginWinLoss.ChildsInfo, startColumn, ref currentRow);
            }
            else
            {
                workSheet.ExportEmptyReport(startRow, startColumn, ref currentRow, startColumn + 5);
            }

            var reportName = SpecialResources.MarginReport_SheetName;
            excelFile.ReponseStream(reportName);
        }

        /// <summary>
        /// Export header for margin report.
        /// </summary>
        /// <param name="workSheet">The work sheet.</param>
        /// <param name="startColumn">The start column.</param>
        /// <param name="currentRow">The current row.</param>
        private void MarginReportExportHeader(ref ExcelWorksheet workSheet, int startColumn, ref int currentRow)
        {
            var headerColor = ExcelExtensions.HeaderTitleColor;
            var currentColumn = startColumn;
            ExcelCell currentCell;
            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Value = Labels.No;
            currentCell.Style.FormatTitleCell(headerColor);
            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Value = Labels.UserName;
            currentCell.Style.FormatTitleCell(headerColor);
            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Value = Labels.Type;
            currentCell.Style.FormatTitleCell(headerColor);
            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Value = SpecialResources.WinningAmount;
            currentCell.Style.FormatTitleCell(headerColor);
            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Value = Labels.Turnover;
            currentCell.Style.FormatTitleCell(headerColor);
            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Value = SpecialResources.Ratio;
            currentCell.Style.FormatTitleCell(headerColor);
            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Value = Labels.Bet_Count;
            currentCell.Style.FormatTitleCell(headerColor);

            currentRow++;
        }

        /// <summary>
        /// Export body for margin report.
        /// </summary>
        /// <param name="workSheet">The work sheet.</param>
        /// <param name="childsInfo">The list margin win loss.</param>
        /// <param name="startColumn">The start column.</param>
        /// <param name="currentRow">The current row.</param>
        [SuppressMessage("StyleCop.CSharp.StylecopPlus.Rules",
           "SP2101:MethodMustNotContainMoreLinesThan",
           Justification = "Reviewed. Suppression is OK here.")]
        private void MarginReportExportBody(ref ExcelWorksheet workSheet, List<MarginWinLoss> childsInfo, int startColumn, ref int currentRow)
        {
            var textStyle = ExcelExtensions.GetCellTextStyle(ExcelExtensions.BackGroundColor, false);
            var twoDecimalNumberStyle = ExcelExtensions.GetCellNumberStyle(
                NumberFormatConstants.TwoDecimalDigitFormat,
                ExcelExtensions.BackGroundColor,
                false,
                false,
                false);
            ExcelCell currentCell;
            var info = new MarginWinLoss();
            for (var i = 0; i < childsInfo.Count; i++)
            {
                var currentColumn = startColumn;
                if (i % 2 == 0)
                {
                    textStyle.FillPattern.SetSolid(ExcelExtensions.EvenRowColor);
                    twoDecimalNumberStyle.FillPattern.SetSolid(ExcelExtensions.EvenRowColor);
                }
                else
                {
                    textStyle.FillPattern.SetSolid(ExcelExtensions.OddRowColor);
                    twoDecimalNumberStyle.FillPattern.SetSolid(ExcelExtensions.OddRowColor);
                }

                info = childsInfo[i];
                currentCell = workSheet.Cells[currentRow, currentColumn];
                currentCell.Value = Converter.ToInt(i + 1);
                currentCell.Style = textStyle;
                currentCell.Style.HorizontalAlignment = HorizontalAlignmentStyle.Center;
                currentColumn++;

                currentCell = workSheet.Cells[currentRow, currentColumn];
                currentCell.Value = info.UserName;
                currentCell.Style = textStyle;
                currentCell.Style.HorizontalAlignment = HorizontalAlignmentStyle.Left;
                currentColumn++;

                currentCell = workSheet.Cells[currentRow, currentColumn];
                currentCell.Value = info.BettypeName;
                currentCell.Style = textStyle;
                currentCell.Style.HorizontalAlignment = HorizontalAlignmentStyle.Center;
                currentColumn++;

                currentCell = workSheet.Cells[currentRow, currentColumn];
                currentCell.Value = info.WinningAmount;
                currentCell.Style = twoDecimalNumberStyle;
                currentColumn++;

                currentCell = workSheet.Cells[currentRow, currentColumn];
                currentCell.Value = info.Turnover;
                currentCell.Style = twoDecimalNumberStyle;
                currentColumn++;

                currentCell = workSheet.Cells[currentRow, currentColumn];
                currentCell.Value = info.Ratio;
                currentCell.Style = twoDecimalNumberStyle;
                currentColumn++;

                currentCell = workSheet.Cells[currentRow, currentColumn];
                currentCell.Value = info.TotalBet;
                currentCell.Style = textStyle;
                currentColumn++;
                currentRow++;
            }
        }
    }
}