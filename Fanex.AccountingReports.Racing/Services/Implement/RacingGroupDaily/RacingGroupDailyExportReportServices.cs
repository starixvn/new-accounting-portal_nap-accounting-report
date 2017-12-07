namespace Fanex.AccountingReports.Racing.Services.Implement.RacingGroupDaily
{
    using System;
    using System.Collections.Generic;
    using Fanex.AccountingReports.Core.Extensions;
    using Fanex.AccountingReports.Core.Services.Impl;
    using Fanex.AccountingReports.Core.Web;
    using Fanex.AccountingReports.Racing.Constants;
    using Fanex.AccountingReports.Racing.Criteria.RacingGroupDaily;
    using Fanex.AccountingReports.Racing.Models.RacingGroupDaily;
    using Fanex.AccountingReports.Racing.Resources;
    using Fanex.AccountingReports.Racing.Services.Interface.RacingGroupDaily;
    using Fanex.NAP.Core.Service.Impl;
    using NPOI.HSSF.UserModel;
    using NPOI.HSSF.Util;
    using NPOI.SS.UserModel;
    using NPOI.SS.Util;

    /// <summary>
    /// The implement of export Racing Group Daily report.
    /// </summary>
    public class RacingGroupDailyExportReportServices : ServiceBase, IRacingGroupDailyExportReportServices
    {
        private const short DailySubTotalColorIndex = 29;

        private const short PartnerTypeSubTotalColorIndex = 32;

        private const short GrandTotalColorIndex = 41;

        private const short GrandTotalBorderColorIndex = 47;

        private readonly string _racingGroupDailyPreficSessionKey =
                                "Fanex.AccountingReports.Racing.RacingGroupDaily.";

        private string _racingGroupDailySessionKey = string.Empty;

        private IRacingGroupDailyGetReportServices _getReportService;

        public RacingGroupDailyExportReportServices()
        {
            if (AppSession.CurrentUser != null)
            {
                this._racingGroupDailySessionKey = this._racingGroupDailyPreficSessionKey +
                                                    AppSession.CurrentUser.Username;
            }
            else
            {
                this._racingGroupDailySessionKey = this._racingGroupDailyPreficSessionKey;
            }

            _getReportService = new RacingGroupDailyGetReportServices();
        }

        /// <summary>
        /// Export report.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        public void ExportReport(GetWinLostRacingGroupCriteria criteria)
        {
            try
            {
                var report = SessionManager.Get<List<RacingGroupDaily>>(this._racingGroupDailySessionKey);

                if (report == null)
                {
                    report = this._getReportService.GetReport(criteria);
                }

                HSSFWorkbook excelFile = new HSSFWorkbook();
                this.SetColorAtIndex(excelFile);

                string sheetName = RacingResources.RacingGroupDaily_SheetName;
                ISheet worksheet = excelFile.CreateSheet(sheetName);
                int currentRow = 0;

                if (report != null && report.Count >= 2)
                {
                    this.RenderReportHeader(excelFile, ref worksheet, ref currentRow, criteria);
                    this.RenderReportContent(excelFile, ref worksheet, ref currentRow, report);
                }

                string reportTitle = string.Format(
                        RacingResources.RacingGroupDaily_ReportName,
                        criteria.FromDate.ToShortFormatedDateWithSlash(),
                        criteria.ToDate.ToShortFormatedDateWithSlash());

                excelFile.ToReponseStream(reportTitle);
            }
            catch (Exception ex)
            {
                this.HandleException(ex);
            }
        }

        /// <summary>
        /// Render header of report.
        /// </summary>
        /// <param name="excelFile">The excel file.</param>
        /// <param name="worksheet">The worksheet.</param>
        /// <param name="currentRow">The current row.</param>
        /// <param name="criteria">The criteria.</param>
        private void RenderReportHeader(
            HSSFWorkbook excelFile,
            ref ISheet worksheet,
            ref int currentRow,
            GetWinLostRacingGroupCriteria criteria)
        {
            ICellStyle headerStyle = NpoiExcelExtensions.HeaderStyle(excelFile);

            for (var i = 0; i < 18; i++)
            {
                worksheet.SetColumnWidth(i, 140 * ExcelExtensions.FontWidth);
            }

            worksheet.SetColumnWidth(1, 180 * ExcelExtensions.FontWidth);

            this.RenderReportHeaderDetail(
                            ref worksheet,
                            ref currentRow,
                            criteria,
                            headerStyle);
        }

        /// <summary>
        /// Render the detail of header report.
        /// </summary>
        /// <param name="worksheet">The worksheet.</param>
        /// <param name="currentRow">The index of current row.</param>
        /// <param name="criteria">The criteria.</param>
        /// <param name="headerStyle">The style of header.</param>
        private void RenderReportHeaderDetail(
            ref ISheet worksheet,
            ref int currentRow,
            GetWinLostRacingGroupCriteria criteria,
            ICellStyle headerStyle)
        {
            var currentColumn = 0;
            string reportTitle = string.Format(
                    RacingResources.RacingGroupDaily_ReportTitle,
                    criteria.FromDate.ToShortFormatedDate(),
                    criteria.ToDate.ToShortFormatedDate());
            IRow row = worksheet.CreateRow(currentRow);

            NpoiExcelExtensions.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow + 1, currentColumn + 17, reportTitle, headerStyle);
            currentRow += 2;
            worksheet.AddMergedRegion(new CellRangeAddress(currentRow, currentRow, 0, 17));
            currentRow++;

            row = worksheet.CreateRow(currentRow);
            currentColumn = 0;

            NpoiExcelExtensions.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow, currentColumn + 1, string.Empty, headerStyle);
            NpoiExcelExtensions.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow, currentColumn + 3, Labels.Turnover_RM, headerStyle);
            NpoiExcelExtensions.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow, currentColumn + 2, Labels.IBC_RM, headerStyle);
            NpoiExcelExtensions.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow, currentColumn + 2, Labels._12Bet_RM, headerStyle);
            NpoiExcelExtensions.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow + 1, currentColumn, Labels.GroupTotalIncome, headerStyle);
            NpoiExcelExtensions.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow, currentColumn + 4, Labels.Partner_RM, headerStyle);

            currentRow++;
            currentColumn = 0;
            row = worksheet.CreateRow(currentRow);
            NpoiExcelExtensions.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow, currentColumn, Labels.Date, headerStyle);
            NpoiExcelExtensions.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow, currentColumn, Labels.PartnerType, headerStyle);
            NpoiExcelExtensions.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow, currentColumn, Labels.Agency, headerStyle);
            NpoiExcelExtensions.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow, currentColumn, Labels.IBC, headerStyle);
            NpoiExcelExtensions.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow, currentColumn, Labels.Partner, headerStyle);
            NpoiExcelExtensions.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow, currentColumn, Labels.Total_Turnover, headerStyle);
            NpoiExcelExtensions.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow, currentColumn, Labels.PositionTaking, headerStyle);
            NpoiExcelExtensions.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow, currentColumn, Labels.NetCommission, headerStyle);
            NpoiExcelExtensions.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow, currentColumn, Labels.TotalIncome, headerStyle);
            NpoiExcelExtensions.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow, currentColumn, Labels.PositionTaking, headerStyle);
            NpoiExcelExtensions.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow, currentColumn, Labels.Commission, headerStyle);
            NpoiExcelExtensions.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow, currentColumn, Labels.TotalIncome, headerStyle);
            currentColumn++;
            NpoiExcelExtensions.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow, currentColumn, Labels.Turnover, headerStyle);
            NpoiExcelExtensions.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow, currentColumn, Labels.DividendWon, headerStyle);
            NpoiExcelExtensions.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow, currentColumn, Labels.CommissionExpense, headerStyle);
            NpoiExcelExtensions.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow, currentColumn, Labels.Fee, headerStyle);
            NpoiExcelExtensions.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow, currentColumn, Labels.AmountDue, headerStyle);

            currentRow++;
        }

        /// <summary>
        /// Render the content of report.
        /// </summary>
        /// <param name="excelFile">The excel file.</param>
        /// <param name="worksheet">The worksheet.</param>
        /// <param name="currentRow">The current row.</param>
        /// <param name="report">The report.</param>
        private void RenderReportContent(
            HSSFWorkbook excelFile,
            ref ISheet worksheet,
            ref int currentRow,
            List<RacingGroupDaily> report)
        {
            ICellStyle textStyle = NpoiExcelExtensions.TextStyle(excelFile, HSSFColor.White.Index, false, NpoiExcelExtensions.Alignment.Center);
            ICellStyle decimalStyle = NpoiExcelExtensions.DecimalStyle(excelFile, HSSFColor.White.Index, false);
            ICellStyle dailySubTotalTextStyle = NpoiExcelExtensions.TextStyle(excelFile, DailySubTotalColorIndex, true, NpoiExcelExtensions.Alignment.Center);
            ICellStyle dailySubTotalDecimalStyle = NpoiExcelExtensions.DecimalStyle(excelFile, DailySubTotalColorIndex, true);
            ICellStyle partnerTypeSubTotalTextStyle = NpoiExcelExtensions.TextStyle(excelFile, PartnerTypeSubTotalColorIndex, true, NpoiExcelExtensions.Alignment.Center);
            ICellStyle partnerTypeSubTotalDecimalStyle = NpoiExcelExtensions.DecimalStyle(excelFile, PartnerTypeSubTotalColorIndex, true);
            IRow row = null;
            int currentColumn = 0;
            for (int i = 0; i < report.Count - 1; i++)
            {
                var reportByDate = report[i];
                foreach (var reportRow in reportByDate.Rows)
                {
                    row = worksheet.CreateRow(currentRow);
                    this.ExportReportRow(ref worksheet, ref row, ref currentRow, ref currentColumn, textStyle, decimalStyle, reportRow);
                    currentRow++;
                    currentColumn = 0;
                }

                int startMergeRow = currentRow - reportByDate.Rows.Count;
                NpoiExcelExtensions.RenderCell(ref worksheet, ref row, startMergeRow, ref currentColumn, currentRow - 1, currentColumn, reportByDate.WinLostDate, textStyle);
                currentColumn = 0;
                row = worksheet.CreateRow(currentRow);
                this.ExportSubTotalRow(ref worksheet, ref row, ref currentRow, ref currentColumn, dailySubTotalTextStyle, dailySubTotalDecimalStyle, reportByDate);
                currentColumn = 0;
                currentRow++;
            }

            foreach (var partnerTypeSubTotal in report[report.Count - 1].Rows)
            {
                row = worksheet.CreateRow(currentRow);
                currentColumn = 0;
                this.ExportPartnerTypeSubTotalRow(
                    ref worksheet,
                    ref row,
                    ref currentRow,
                    ref currentColumn,
                    partnerTypeSubTotalTextStyle,
                    partnerTypeSubTotalDecimalStyle,
                    partnerTypeSubTotal);
                currentRow++;
            }

            currentColumn = 0;
            row = worksheet.CreateRow(currentRow);
            this.ExportGrandTotalRow(excelFile, ref worksheet, ref row, ref currentRow, ref currentColumn, report[report.Count - 1]);
        }

        /// <summary>
        /// Exports the report row.
        /// </summary>
        /// <param name="worksheet">The worksheet.</param>
        /// <param name="row">The row of sheet.</param>
        /// <param name="currentRow">The current row.</param>
        /// <param name="currentColumn">The current column.</param>
        /// <param name="textStyle">The text style.</param>
        /// <param name="decimalStyle">The decimal style.</param>
        /// <param name="reportRow">The report row.</param>
        private void ExportReportRow(
            ref ISheet worksheet,
            ref IRow row,
            ref int currentRow,
            ref int currentColumn,
            ICellStyle textStyle,
            ICellStyle decimalStyle,
            RacingGroupDaily reportRow)
        {
            this.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, string.Empty, textStyle);
            this.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, reportRow.Name, textStyle);
            this.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, reportRow.TurnoverAgent, decimalStyle);
            this.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, reportRow.TurnoverIBC, decimalStyle);
            this.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, reportRow.PartnerTurnover, decimalStyle);
            this.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, reportRow.TotalTurnover, decimalStyle);
            this.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, reportRow.IncomeIBCPostitionTaking, decimalStyle);
            this.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, reportRow.IncomeIBCNetCommission, decimalStyle);
            this.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, reportRow.TotalIBCIncome, decimalStyle);
            this.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, reportRow.Income12BETPositionTaking, decimalStyle);
            this.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, reportRow.Income12BETCommission, decimalStyle);
            this.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, reportRow.Total12BETIncome, decimalStyle);
            this.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, reportRow.GroupTotalIncome, decimalStyle);
            this.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, reportRow.PartnerTurnover, decimalStyle);
            this.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, reportRow.PartnerDividendWon, decimalStyle);
            this.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, reportRow.PartnerExpenseCommission, decimalStyle);
            this.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, reportRow.PartnerFee, decimalStyle);
            this.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, reportRow.PartnerAmountDue, decimalStyle);
        }

        /// <summary>
        /// Exports the sub total row.
        /// </summary>
        /// <param name="worksheet">The worksheet.</param>
        /// <param name="row">The row of sheet.</param>
        /// <param name="currentRow">The current row.</param>
        /// <param name="currentColumn">The current column.</param>
        /// <param name="textStyle">The text style.</param>
        /// <param name="decimalStyle">The decimal style.</param>
        /// <param name="total">The total.</param>
        private void ExportSubTotalRow(
            ref ISheet worksheet,
            ref IRow row,
            ref int currentRow,
            ref int currentColumn,
            ICellStyle textStyle,
            ICellStyle decimalStyle,
            RacingGroupDaily total)
        {
            this.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, string.Empty, textStyle);
            this.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, string.Empty, textStyle);
            currentColumn = 0;
            this.RenderMergeCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow, currentColumn + 1, Labels.Total, textStyle);
            this.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, total.TurnoverAgent, decimalStyle);
            this.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, total.TurnoverIBC, decimalStyle);
            this.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, total.PartnerTurnover, decimalStyle);
            this.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, total.TotalTurnover, decimalStyle);
            this.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, total.IncomeIBCPostitionTaking, decimalStyle);
            this.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, total.IncomeIBCNetCommission, decimalStyle);
            this.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, total.TotalIBCIncome, decimalStyle);
            this.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, total.Income12BETPositionTaking, decimalStyle);
            this.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, total.Income12BETCommission, decimalStyle);
            this.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, total.Total12BETIncome, decimalStyle);
            this.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, total.GroupTotalIncome, decimalStyle);
            this.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, total.PartnerTurnover, decimalStyle);
            this.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, total.PartnerDividendWon, decimalStyle);
            this.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, total.PartnerExpenseCommission, decimalStyle);
            this.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, total.PartnerFee, decimalStyle);
            this.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, total.PartnerAmountDue, decimalStyle);
        }

        /// <summary>
        /// Exports the partner type sub total row.
        /// </summary>
        /// <param name="worksheet">The worksheet.</param>
        /// <param name="row">The row of sheet.</param>
        /// <param name="currentRow">The current row.</param>
        /// <param name="currentColumn">The current column.</param>
        /// <param name="textStyle">The text style.</param>
        /// <param name="decimalStyle">The decimal style.</param>
        /// <param name="total">The total.</param>
        private void ExportPartnerTypeSubTotalRow(
            ref ISheet worksheet,
            ref IRow row,
            ref int currentRow,
            ref int currentColumn,
            ICellStyle textStyle,
            ICellStyle decimalStyle,
            RacingGroupDaily total)
        {
            this.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, string.Empty, textStyle);
            this.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, string.Empty, textStyle);
            currentColumn = 0;
            var totalTilte = string.Format("{0} {1}", total.Name, Labels.Total);
            this.RenderMergeCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow, currentColumn + 1, totalTilte, textStyle);
            this.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, total.TurnoverAgent, decimalStyle);
            this.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, total.TurnoverIBC, decimalStyle);
            this.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, total.PartnerTurnover, decimalStyle);
            this.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, total.TotalTurnover, decimalStyle);
            this.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, total.IncomeIBCPostitionTaking, decimalStyle);
            this.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, total.IncomeIBCNetCommission, decimalStyle);
            this.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, total.TotalIBCIncome, decimalStyle);
            this.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, total.Income12BETPositionTaking, decimalStyle);
            this.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, total.Income12BETCommission, decimalStyle);
            this.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, total.Total12BETIncome, decimalStyle);
            this.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, total.GroupTotalIncome, decimalStyle);
            this.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, total.PartnerTurnover, decimalStyle);
            this.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, total.PartnerDividendWon, decimalStyle);
            this.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, total.PartnerExpenseCommission, decimalStyle);
            this.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, total.PartnerFee, decimalStyle);
            this.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, total.PartnerAmountDue, decimalStyle);
        }

        /// <summary>
        /// Exports the grand total row.
        /// </summary>
        /// <param name="excelFile">The excel file.</param>
        /// <param name="worksheet">The worksheet.</param>
        /// <param name="row">The row of sheet.</param>
        /// <param name="currentRow">The current row.</param>
        /// <param name="currentColumn">The current column.</param>
        /// <param name="total">The total.</param>
        private void ExportGrandTotalRow(
            HSSFWorkbook excelFile,
            ref ISheet worksheet,
            ref IRow row,
            ref int currentRow,
            ref int currentColumn,
            RacingGroupDaily total)
        {
            ICellStyle grandTotalTextStyle = NpoiExcelExtensions.TextStyle(excelFile, GrandTotalColorIndex, true, NpoiExcelExtensions.Alignment.Center);
            grandTotalTextStyle.BorderTop = BorderStyle.Medium;
            grandTotalTextStyle.TopBorderColor = GrandTotalBorderColorIndex;

            ICellStyle grandTotalDecimalStyle = NpoiExcelExtensions.DecimalStyle(excelFile, GrandTotalColorIndex, true);
            grandTotalDecimalStyle.BorderTop = BorderStyle.Medium;
            grandTotalDecimalStyle.TopBorderColor = GrandTotalBorderColorIndex;

            this.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, string.Empty, grandTotalTextStyle);
            this.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, string.Empty, grandTotalTextStyle);
            currentColumn = 0;
            this.RenderMergeCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow, currentColumn + 1, Labels.GrandTotal, grandTotalTextStyle);
            this.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, total.TurnoverAgent, grandTotalDecimalStyle);
            this.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, total.TurnoverIBC, grandTotalDecimalStyle);
            this.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, total.PartnerTurnover, grandTotalDecimalStyle);
            this.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, total.TotalTurnover, grandTotalDecimalStyle);
            this.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, total.IncomeIBCPostitionTaking, grandTotalDecimalStyle);
            this.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, total.IncomeIBCNetCommission, grandTotalDecimalStyle);
            this.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, total.TotalIBCIncome, grandTotalDecimalStyle);
            this.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, total.Income12BETPositionTaking, grandTotalDecimalStyle);
            this.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, total.Income12BETCommission, grandTotalDecimalStyle);
            this.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, total.Total12BETIncome, grandTotalDecimalStyle);
            this.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, total.GroupTotalIncome, grandTotalDecimalStyle);
            this.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, total.PartnerTurnover, grandTotalDecimalStyle);
            this.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, total.PartnerDividendWon, grandTotalDecimalStyle);
            this.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, total.PartnerExpenseCommission, grandTotalDecimalStyle);
            this.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, total.PartnerFee, grandTotalDecimalStyle);
            this.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, total.PartnerAmountDue, grandTotalDecimalStyle);
        }

        /// <summary>
        /// Sets the index of the color at.
        /// </summary>
        /// <param name="excelFile">The excel file.</param>
        private void SetColorAtIndex(HSSFWorkbook excelFile)
        {
            HSSFPalette palette = excelFile.GetCustomPalette();
            palette.SetColorAtIndex(DailySubTotalColorIndex, (byte)232, (byte)235, (byte)211);
            palette.SetColorAtIndex(PartnerTypeSubTotalColorIndex, (byte)211, (byte)225, (byte)200);
            palette.SetColorAtIndex(GrandTotalColorIndex, (byte)219, (byte)225, (byte)187);
            palette.SetColorAtIndex(GrandTotalBorderColorIndex, (byte)255, (byte)102, (byte)0);
        }

        /// <summary>
        /// Renders the merge cell.
        /// </summary>
        /// <param name="worksheet">The worksheet.</param>
        /// <param name="row">The row of sheet.</param>
        /// <param name="currentRow">The current row.</param>
        /// <param name="currentColumn">The current column.</param>
        /// <param name="endRow">The end row.</param>
        /// <param name="endColumn">The end column.</param>
        /// <param name="cellValue">The cell value.</param>
        /// <param name="cellStyle">The cell style.</param>
        private void RenderMergeCell(
            ref ISheet worksheet,
            ref IRow row,
            int currentRow,
            ref int currentColumn,
            int endRow,
            int endColumn,
            object cellValue,
            ICellStyle cellStyle)
        {
            ICell cell = row.CreateCell(currentColumn);

            if (cellValue is string)
            {
                cell.SetCellValue((string)cellValue);
            }
            else if (cellValue is decimal)
            {
                cell.SetCellValue(Convert.ToDouble(cellValue));
            }
            else if (cellValue is int)
            {
                cell.SetCellValue(Convert.ToInt32(cellValue));
            }

            cell.CellStyle = cellStyle;
            worksheet.AddMergedRegion(new CellRangeAddress(currentRow, endRow, currentColumn, endColumn));
            currentColumn = endColumn + 1;
        }

        /// <summary>
        /// Renders the cell.
        /// </summary>
        /// <param name="worksheet">The worksheet.</param>
        /// <param name="row">The row of sheet.</param>
        /// <param name="currentRow">The current row.</param>
        /// <param name="currentColumn">The current column.</param>
        /// <param name="cellValue">The cell value.</param>
        /// <param name="cellStyle">The cell style.</param>
        private void RenderCell(
            ref ISheet worksheet,
            ref IRow row,
            int currentRow,
            ref int currentColumn,
            object cellValue,
            ICellStyle cellStyle)
        {
            ICell cell = row.CreateCell(currentColumn);

            if (cellValue is string)
            {
                cell.SetCellValue((string)cellValue);
            }
            else if (cellValue is decimal)
            {
                cell.SetCellValue(Convert.ToDouble(cellValue));
            }
            else if (cellValue is int)
            {
                cell.SetCellValue(Convert.ToInt32(cellValue));
            }

            cell.CellStyle = cellStyle;
            currentColumn += 1;
        }
    }
}
