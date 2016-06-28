// -----------------------------------------------------------------------
// <copyright file="ExportExcelService.cs" company="Microsoft">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Fanex.AccountingReports.Gaming.Services.Implement.CasinoGameStatistic
{
    using System.Diagnostics.CodeAnalysis;
    using System.Drawing;
    using ExcelUtil.Spreadsheet;
    using Fanex.AccountingReports.Core.Extensions;
    using Fanex.AccountingReports.Gaming.Constants;
    using Fanex.AccountingReports.Gaming.Criteria.CasinoGameStatistic;
    using Fanex.AccountingReports.Gaming.Models.CasinoGameStatistic;
    using Fanex.AccountingReports.Gaming.Resources;
    using Fanex.AccountingReports.Gaming.Services.Interface.CasinoGameStatistic;
    using Fanex.Common;
    using CoreConstants = Fanex.AccountingReports.Core.Constants;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class ExportExcelService : IExportExcelService
    {
        [SuppressMessage("StyleCop.CSharp.StylecopPlus.Rules",
           "SP2100:CodeLineMustNotBeLongerThan",
           Justification = "Reviewed. Suppression is OK here.")]
        [SuppressMessage("StyleCop.CSharp.StylecopPlus.Rules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        private void ExportReport_Header(ref ExcelWorksheet workSheet, int startRow, int startColumn, ref int currentRow, int periodType, int maxColumnIndex, GetReportByDailyCriteria criteria)
        {
            var currentColumn = startColumn;
            CellRange currentCellRange;
            ExcelCell currentCell;
            var headerTextStyle = ExcelExtensions.GetCellTitleStyle(ExcelExtensions.HeaderTitleColor);
            var isShowNoPlayers = true;
            var isShowColumnCurrency = periodType != PeriodTypeConstants.LiveCasinoGameStatisticCurrency;

            workSheet.Columns[startColumn].Width = 160 * ExcelExtensions.FontWidth;
            for (int colIndex = 1; colIndex <= maxColumnIndex; colIndex++)
            {
                workSheet.Columns[startColumn + colIndex].Width = 120 * ExcelExtensions.FontWidth;
            }

            workSheet.Columns[startColumn + 1].Width = 90 * ExcelExtensions.FontWidth;
            if (isShowNoPlayers)
            {
                workSheet.Columns[startColumn + 2].Width = 90 * ExcelExtensions.FontWidth;
            }

            if (isShowColumnCurrency)
            {
                workSheet.Columns[startColumn + 3 - (isShowNoPlayers ? 0 : 1)].Width = 90 * ExcelExtensions.FontWidth;
            }

            var columnCompanyTurnoverIndex = 9 - (criteria.ShowRefundColumn ? 0 : 1) -
                (isShowNoPlayers ? 0 : 1) - (isShowColumnCurrency ? 0 : 1);
            workSheet.Columns[startColumn + columnCompanyTurnoverIndex].Width = 140 * ExcelExtensions.FontWidth;
            workSheet.Columns[startColumn + maxColumnIndex].Width = 150 * ExcelExtensions.FontWidth;

            var title = string.Format(
                GamingResources.CasinoGameStatistic_ReportTitle,
                criteria.Product,
                Converter.ToDateTime(criteria.FromDate).ToShortFormatedDate(),
                Converter.ToDateTime(criteria.ToDate).ToShortFormatedDate());

            if (criteria.IsDrillDown)
            {
                title = string.Format(
                GamingResources.CasinoGameStatisticOnDateByCurrency_ReportTitle,
                criteria.Product,
                Converter.ToDateTime(criteria.FromDate).ToShortFormatedDate());
            }

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
            currentCell.Value = Labels.Grand_Total;

            currentColumn++;

            if (isShowNoPlayers)
            {
                currentCell = workSheet.Cells[currentRow, currentColumn];
                currentCell.Style = headerTextStyle;
                currentCell.Value = Labels.No_Players;
                currentColumn++;
            }

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Style = headerTextStyle;
            currentCell.Value = Labels.Bet_Count;
            currentColumn++;

            if (isShowColumnCurrency)
            {
                currentCell = workSheet.Cells[currentRow, currentColumn];
                currentCell.Style = headerTextStyle;
                currentCell.Value = Labels.Currency;
                currentColumn++;
            }

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Style = headerTextStyle;
            currentCell.Value = Labels.Total_Bets;
            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Style = headerTextStyle;
            currentCell.Value = Labels.Total_Wins;
            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Style = headerTextStyle;
            currentCell.Value = Labels.Game_income;
            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Style = headerTextStyle;
            currentCell.Value = Labels.Game_payout;
            currentColumn++;

            if (criteria.ShowRefundColumn)
            {
                currentCell = workSheet.Cells[currentRow, currentColumn];
                currentCell.Style = headerTextStyle;
                currentCell.Value = Labels.Total_refund;
                currentColumn++;
            }

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Style = headerTextStyle;
            currentCell.Value = Labels.Company_Turnover;
            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Style = headerTextStyle;
            currentCell.Value = Labels.Company_comm;
            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Style = headerTextStyle;
            currentCell.Value = Labels.Company_net_income;

            workSheet.Rows[currentRow].Height = 20 * ExcelExtensions.FontWidth;
            currentColumn++;
            currentRow++;
        }

        [SuppressMessage("StyleCop.CSharp.StylecopPlus.Rules",
           "SP2100:CodeLineMustNotBeLongerThan",
           Justification = "Reviewed. Suppression is OK here.")]
        [SuppressMessage("StyleCop.CSharp.StylecopPlus.Rules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        private void ExportReport_WinLossRow(ref ExcelWorksheet workSheet, int startColumn, ref int currentRow, CellStyle intNumberStyle, CellStyle textStyle, CellStyle twoDecimalNumberStyle, CellStyle twoDecimalNegativeNumberStyle, CellStyle twoDecimalNegativePercentNumberStyle, GetReportByDailyCriteria criteria, WinLossInfo info)
        {
            ExcelCell currentCell;
            var isShowNoPlayers = true;
            var isShowColumnCurrency = criteria.ReportType != PeriodTypeConstants.LiveCasinoGameStatisticCurrency;

            var currentColumn = startColumn;
            if (isShowNoPlayers)
            {
                currentCell = workSheet.Cells[currentRow, currentColumn];
                currentCell.Style = intNumberStyle;
                currentCell.Value = info.NumberOfPlayers;
                currentColumn++;
            }

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Style = intNumberStyle;
            currentCell.Value = info.NumberOfGames;
            currentColumn++;

            if (isShowColumnCurrency)
            {
                currentCell = workSheet.Cells[currentRow, currentColumn];
                currentCell.Style = textStyle;
                currentCell.Style.HorizontalAlignment = HorizontalAlignmentStyle.Center;
                currentCell.Value = info.CurrencyName;
                currentColumn++;
            }

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Style = twoDecimalNumberStyle;
            currentCell.Value = info.TotalBets;
            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Style = twoDecimalNumberStyle;
            currentCell.Value = info.TotalWins;
            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Style = twoDecimalNegativeNumberStyle;
            currentCell.Value = info.GameIncome;
            currentColumn++;

            if (info.GamePayout > 1)
            {
                twoDecimalNegativePercentNumberStyle.Font.Color = Color.Red;
            }
            else
            {
                twoDecimalNegativePercentNumberStyle.Font.Color = Color.Black;
            }

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Style = twoDecimalNegativePercentNumberStyle;
            currentCell.Value = info.GamePayout;
            currentColumn++;

            if (criteria.ShowRefundColumn)
            {
                currentCell = workSheet.Cells[currentRow, currentColumn];
                currentCell.Style = twoDecimalNegativeNumberStyle;
                currentCell.Value = info.TotalRefund;
                currentColumn++;
            }

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Style = twoDecimalNegativeNumberStyle;
            currentCell.Value = info.CompanyTurnover;
            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Style = twoDecimalNegativeNumberStyle;
            currentCell.Value = info.CompanyCommission;
            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Style = twoDecimalNegativeNumberStyle;
            currentCell.Value = info.CompanyNetIncome;
            currentColumn++;
        }

        [SuppressMessage("StyleCop.CSharp.StylecopPlus.Rules",
           "SP2100:CodeLineMustNotBeLongerThan",
           Justification = "Reviewed. Suppression is OK here.")]
        [SuppressMessage("StyleCop.CSharp.StylecopPlus.Rules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        private void ExportReport_GrandTotal(WinLossInfo grandTotal, ref ExcelWorksheet workSheet, int startColumn, ref int currentRow, GetReportByDailyCriteria criteria)
        {
            var grandTotalColor = ExcelExtensions.GrandTotalRowColor;
            var textStyle_GrandTotal = ExcelExtensions.GetCellTextStyle(grandTotalColor, true);
            var intNumberStyle_GrandTotal = ExcelExtensions.GetCellNumberStyle(CoreConstants.NumberFormatConstants.IntNumberFormat, grandTotalColor, true, false, false);
            var twoDecimalNumberStyle_GrandTotal = ExcelExtensions.GetCellNumberStyle(
                CoreConstants.NumberFormatConstants.TwoDecimalDigitFormat,
                grandTotalColor,
                true,
                false,
                false);
            var twoDecimalNegativeNumberStyle_GrandTotal = ExcelExtensions.GetCellNumberStyle(
                CoreConstants.NumberFormatConstants.TwoDecimalDigitFormat,
                grandTotalColor,
                true,
                false,
                true);
            var twoDecimalNegativePercentNumberStyle_GrandTotal = ExcelExtensions.GetCellNumberStyle(
                CoreConstants.NumberFormatConstants.TwoDecimalDigitPercentFormat,
                grandTotalColor,
                true,
                false,
                true);

            var grandTotalSeparatorColor = ExcelExtensions.GrandTotalSeparatorColor;

            textStyle_GrandTotal.Borders.SetBorders(MultipleBorders.Top, grandTotalSeparatorColor, LineStyle.Medium);
            intNumberStyle_GrandTotal.Borders.SetBorders(MultipleBorders.Top, grandTotalSeparatorColor, LineStyle.Medium);
            twoDecimalNumberStyle_GrandTotal.Borders.SetBorders(MultipleBorders.Top, grandTotalSeparatorColor, LineStyle.Medium);
            twoDecimalNegativeNumberStyle_GrandTotal.Borders.SetBorders(MultipleBorders.Top, grandTotalSeparatorColor, LineStyle.Medium);
            twoDecimalNegativePercentNumberStyle_GrandTotal.Borders.SetBorders(MultipleBorders.Top, grandTotalSeparatorColor, LineStyle.Medium);

            var currentColumn = startColumn;
            ExcelCell currentCell;

            WinLossInfo info = new WinLossInfo();
            var reportLength = grandTotal.GrandTotal.ChildsInfo.Count;

            if (reportLength > 0)
            {
                info = grandTotal.GrandTotal.ChildsInfo[0];
                info.Name = "Grand Total RM";
                info.CurrencyName = "RM";
            }

            currentColumn = startColumn;
            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Value = info.Name;
            currentCell.Style = textStyle_GrandTotal;
            currentCell.Style.HorizontalAlignment = HorizontalAlignmentStyle.Center;

            currentColumn++;

            ExportReport_WinLossRow(
                ref workSheet,
                currentColumn,
                ref currentRow,
                intNumberStyle_GrandTotal,
                textStyle_GrandTotal,
                twoDecimalNumberStyle_GrandTotal,
                twoDecimalNegativeNumberStyle_GrandTotal,
                twoDecimalNegativePercentNumberStyle_GrandTotal,
                criteria,
                info);
            currentRow++;
        }

        [SuppressMessage("StyleCop.CSharp.StylecopPlus.Rules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        public void ExportReport(WinLossInfo report, GetReportByDailyCriteria criteria)
        {
            ExcelFile excelFile = new ExcelFile();
            ExcelWorksheetCollection worksheets = excelFile.Worksheets;
            string sheetName = string.Format(Labels.CasinoGameStatistic_SheetName, criteria.Product);
            ExcelWorksheet workSheet = worksheets.Add(sheetName);

            int startRow = 0;
            int startColumn = 0;
            int maxColumnIndex = criteria.ShowRefundColumn ? 11 : 10;
            var currentRow = startRow;

            var currentColumn = startColumn;
            ExcelCell currentCell;

            var backgroundColor = ExcelExtensions.BackGroundColor;
            var textStyle = ExcelExtensions.GetCellTextStyle(backgroundColor, false);
            var intNumberStyle = ExcelExtensions.GetCellNumberStyle(CoreConstants.NumberFormatConstants.IntNumberFormat, backgroundColor, false, false, false);
            var twoDecimalNumberStyle = ExcelExtensions.GetCellNumberStyle(CoreConstants.NumberFormatConstants.TwoDecimalDigitFormat, backgroundColor, false, false, false);
            var twoDecimalNegativeNumberStyle = ExcelExtensions.GetCellNumberStyle(
                CoreConstants.NumberFormatConstants.TwoDecimalDigitFormat,
                backgroundColor,
                false,
                false,
                true);
            var twoDecimalNegativePercentNumberStyle = ExcelExtensions.GetCellNumberStyle(
                CoreConstants.NumberFormatConstants.TwoDecimalDigitPercentFormat,
                backgroundColor,
                false,
                false,
                true);

            ExportReport_Header(
                ref workSheet,
                startRow,
                startColumn,
                ref currentRow,
                PeriodTypeConstants.CasinoGameStatisticDaily,
                maxColumnIndex,
                criteria);

            var reportLength = report != null && report.ChildsInfo != null ?
                report.ChildsInfo.Count :
                0;
            if (reportLength > 0)
            {
                WinLossInfo info;
                var rows = report.ChildsInfo[0].ChildsInfo ?? report.ChildsInfo;
                for (var i = 0; i < rows.Count; i++)
                {
                    info = rows[i];
                    currentColumn = startColumn;

                    currentCell = workSheet.Cells[currentRow, currentColumn];
                    currentCell.Style = textStyle;
                    currentCell.Value = "Total " + info.CurrencyName;
                    currentCell.Style.HorizontalAlignment = HorizontalAlignmentStyle.Center;

                    currentColumn++;

                    ExportReport_WinLossRow(
                        ref workSheet,
                        currentColumn,
                        ref currentRow,
                        intNumberStyle,
                        textStyle,
                        twoDecimalNumberStyle,
                        twoDecimalNegativeNumberStyle,
                        twoDecimalNegativePercentNumberStyle,
                        criteria,
                        info);
                    currentRow++;
                }

                ExportReport_GrandTotal(report, ref workSheet, startColumn, ref currentRow, criteria);
            }
            else
            {
                workSheet.ExportEmptyReport(startRow, startColumn, ref currentRow, startColumn + maxColumnIndex);
            }

            var reportName = string.Format(
               GamingResources.CasinoGameStatistic_ReportName,
               criteria.Product.Replace(" ", "_"),
               Converter.ToDateTime(criteria.FromDate).ToShortFormatedDateWithSlash().Replace('/', '_'),
               Converter.ToDateTime(criteria.ToDate).ToShortFormatedDateWithSlash().Replace('/', '_'));
            excelFile.ReponseStream(reportName);
        }
    }
}