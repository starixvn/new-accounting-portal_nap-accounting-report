// -----------------------------------------------------------------------
// <copyright file="ExportExcelHourlyService.cs" company="Microsoft">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Fanex.AccountingReports.Gaming.Services.Implement.CasinoGameStatistic
{
    using System;
    using ExcelUtil.Spreadsheet;
    using Fanex.AccountingReports.Core.Extensions;
    using Fanex.AccountingReports.Gaming.Constants;
    using Fanex.AccountingReports.Gaming.Criteria.CasinoGameStatistic;
    using Fanex.AccountingReports.Gaming.Resources;
    using Fanex.AccountingReports.Gaming.Services.Interface.CasinoGameStatistic;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class ExportExcelHourlyService : IExportExcelHourlyService
    {
        private ICasinoGameStatisticService _service;

        public ExportExcelHourlyService()
            : this(null)
        {
        }

        public ExportExcelHourlyService(ICasinoGameStatisticService service)
        {
            _service = service ?? new CasinoGameStatisticService();
        }

        public void ExportReport(GetReportByHourlyCriteria criteria)
        {
            throw new NotImplementedException();
        }

        private void ExportHeader(ref ExcelWorksheet workSheet, int startRow, int startColumn, ref int currentRow, int periodType, int maxColumnIndex, GetReportByHourlyCriteria criteria)
        {
            var currentColumn = startColumn;
            CellRange currentCellRange;
            ExcelCell currentCell;
            var headerTextStyle = ExcelExtensions.GetCellTitleStyle(ExcelExtensions.HeaderTitleColor);
            var isShowNoPlayers = periodType != PeriodTypeConstants.LiveCasinoGameStatisticUsername;
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

            var title = "Hourly Title";

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
            currentCell.Value = "Time";

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
    }
}