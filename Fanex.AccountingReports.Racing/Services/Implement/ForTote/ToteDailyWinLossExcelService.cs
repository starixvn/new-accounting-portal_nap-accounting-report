// -----------------------------------------------------------------------
// <copyright file="ToteDailyWinLossExcelService.cs" company="Nexcel Solutions Vietnam">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Fanex.AccountingReports.Racing.Services.Implement
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using Fanex.AccountingReports.Core.Extensions;
    using Fanex.AccountingReports.Core.Web;
    using Fanex.AccountingReports.Racing.Criteria.ForTote;
    using Fanex.AccountingReports.Racing.Models.RacingTote;
    using Fanex.AccountingReports.Racing.Resources;
    using NPOI.HSSF.UserModel;
    using NPOI.HSSF.Util;
    using NPOI.SS.UserModel;
    using NPOI.SS.Util;

    /// <summary>
    /// Create racing daily win loss excel service.
    /// </summary>
    public class ToteDailyWinLossExcelService : ToteDailyWinLossAbstractExcelService
    {
        private const string HeaderRowClass = "header";

        private const string NormalRowClass = "normal-row";

        private const string TotalRowClass = "total-row";

        private const string GrandTotalRowClass = "grandtotal-row";

        private const string TotalRowByCurrDetail = "total-row-curr-detail";

        private const string TotalDetail = "total-detail";

        private CustomStyleCell _headerStyle;

        private CustomStyleCell _normalRowStyle;

        private CustomStyleCell _totalRowStyle;

        private CustomStyleCell _grandTotalRowStyle;

        private CustomStyleCell _totalRowCurrDetail;

        private CustomStyleCell _totalDetail;

        private ToteDailyWinLossService _reportService;

        public ToteDailyWinLossExcelService()
        {
            _reportService = new ToteDailyWinLossService();
        }

        /// <summary>
        /// Export excel for racing daily main.
        /// </summary>
        /// <param name="criteria">The racing for tote main criteria.</param>
        public override void ExportExcelRacingDailyMain(GetMainForToteCriteria criteria)
        {
            try
            {
                var reportType = GetReportName();
                var report = SessionManager.Get<IDailyWinLoss>(RacingDailyWinLossUtil.RacingWinLossMain);
                if (report == null)
                {
                    report = _reportService.GetRacingDailyWinLossMain(criteria);
                }

                HSSFWorkbook excelFile = new HSSFWorkbook();
                this.InitialStyle(excelFile);
                string sheetName = string.Format(RacingResources.RacingTote_SheetName, reportType[0], reportType[1]);
                ISheet worksheet = excelFile.CreateSheet(sheetName);
                int currentRow = 0;
                var currentColumn = 0;
                string reportTitle = string.Format(RacingResources.RacingTote_ReportName, reportType[0], reportType[1], criteria.WinlostDate.ToShortFormatedDate());
                IRow row = worksheet.CreateRow(currentRow);
                row.Height = 200 * NpoiExcelExtensions.FontHeight;
                RenderRow(ref worksheet, ref row, ref currentRow, currentColumn, currentColumn + 24, reportTitle, this._headerStyle.TextStyle);
                worksheet.AddMergedRegion(new CellRangeAddress(currentRow, currentRow, currentColumn, currentColumn + 24));
                currentRow++;
                RenderRow(
                    ref worksheet,
                    ref row,
                    ref currentRow,
                    currentColumn,
                    currentColumn + 24,
                    string.Empty,
                    this._normalRowStyle.TextStyle);
                worksheet.AddMergedRegion(new CellRangeAddress(currentRow, currentRow, currentColumn, currentColumn + 24));
                currentRow++;

                if (report != null && report.ChildsInfo.Count > 0)
                {
                    RenderRacingWLHeader(excelFile, ref worksheet, ref currentRow, RacingResources.Pool + " - " + Labels.Currency, Labels.Currency);
                    RenderRacingWLMainContent(excelFile, ref worksheet, ref currentRow, report.ChildsInfo);
                    RenderRacingWLMainFooter(excelFile, ref worksheet, ref currentRow, report.Total);
                }

                string reportName = string.Format(RacingResources.RacingTote_ReportTitle, reportType[0], reportType[1], criteria.WinlostDate.ToShortFormatedDateWithSlash());
                excelFile.ResponseStreamExcel(reportName);
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }
        }

        /// <summary>
        /// Export excel for racing daily detail.
        /// </summary>
        /// <param name="criteria">The racing for tote detail criteria.</param>
        [SuppressMessage("StyleCop.CSharp.Nexcel.NexcelCustomRules",
        "SP2101:MethodMustNotContainMoreLinesThan",
        Justification = "Reviewed. Suppression is OK here.")]
        public override void ExportExcelRacingDailyDetail(GetDetailForToteCriteria criteria)
        {
            try
            {
                var reportType = GetReportName();
                var report = SessionManager.Get<IDailyWinLossDetail>(RacingDailyWinLossUtil.RacingWinLossDetail);
                if (report == null)
                {
                    report = _reportService.GetRacingDailyWinLossDetail(criteria);
                }

                HSSFWorkbook excelFile = new HSSFWorkbook();
                this.InitialStyle(excelFile);
                string sheetName = string.Format(RacingResources.RacingTote_SheetName, reportType[0], reportType[1]);
                ISheet worksheet = excelFile.CreateSheet(sheetName);
                int currentRow = 0;
                var currentColumn = 0;
                string reportTitle = string.Format(
                    RacingResources.RacingToteDetail_ReportTitle,
                    reportType[0],
                    reportType[1],
                    criteria.PartnerName,
                    criteria.WinlostDate.ToShortFormatedDate());
                IRow row = worksheet.CreateRow(currentRow);
                RenderRow(ref worksheet, ref row, ref currentRow, currentColumn, currentColumn + 24, reportTitle, this._headerStyle.TextStyle);
                worksheet.AddMergedRegion(new CellRangeAddress(currentRow, currentRow, currentColumn, currentColumn + 24));
                currentRow++;
                RenderRow(
                    ref worksheet,
                    ref row,
                    ref currentRow,
                    currentColumn,
                    currentColumn + 24,
                    string.Empty,
                    this._normalRowStyle.TextStyle);
                worksheet.AddMergedRegion(new CellRangeAddress(currentRow, currentRow, currentColumn, currentColumn + 24));
                currentRow++;
                if (report != null && report.ChildsInfo.Count > 0)
                {
                    RenderRacingWLHeader(excelFile, ref worksheet, ref currentRow, RacingResources.Meeting + " - " + Labels.Currency, RacingResources.Race);
                    RenderRacingWLDetailContent(excelFile, ref worksheet, ref currentRow, report.ChildsInfo);
                    RenderRacingWLDetailFooter(excelFile, ref worksheet, ref currentRow, report.Total);
                }

                string reportName = string.Format(RacingResources.RacingTote_ReportTitle, reportType[0], reportType[1], criteria.WinlostDate.ToShortFormatedDateWithSlash());
                reportName = reportName.Replace(" ", string.Empty);
                excelFile.ResponseStreamExcel(reportName);
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }
        }

        /// <summary>
        /// Render header for racing win loss.
        /// </summary>
        /// <param name="excelFile">The excel file.</param>
        /// <param name="worksheet">The work sheet.</param>
        /// <param name="currentRow">The current row.</param>
        /// <param name="nameContainer">The name container.</param>
        /// <param name="name">The name(currency or race).</param>
        public void RenderRacingWLHeader(
            HSSFWorkbook excelFile,
            ref ISheet worksheet,
            ref int currentRow,
            string nameContainer,
            string name)
        {
            for (var i = 2; i < 25; i++)
            {
                worksheet.SetColumnWidth(i, 100 * NpoiExcelExtensions.FontWidth);
            }

            worksheet.SetColumnWidth(0, 180 * NpoiExcelExtensions.FontWidth);
            worksheet.SetColumnWidth(1, 80 * NpoiExcelExtensions.FontWidth);

            RenderReportHeaderDetail(
                            ref worksheet,
                            ref currentRow,
                            nameContainer,
                            name,
                            this._headerStyle.TextStyle);
        }

        /// <summary>
        /// Render header for report detail.
        /// </summary>
        /// <param name="worksheet">The worksheet.</param>
        /// <param name="currentRow">The current row.</param>
        /// <param name="nameContainer">The name container.</param>
        /// <param name="name">The name(currency or race).</param>
        /// <param name="headerStyle">Style for header.</param>
        [SuppressMessage("StyleCop.CSharp.Nexcel.NexcelCustomRules",
        "SP2101:MethodMustNotContainMoreLinesThan",
        Justification = "Reviewed. Suppression is OK here.")]
        public void RenderReportHeaderDetail(ref ISheet worksheet, ref int currentRow, string nameContainer, string name, ICellStyle headerStyle)
        {
            IRow row = worksheet.CreateRow(currentRow);
            var currentColumn = 0;
            RenderCell(ref row, ref currentColumn, nameContainer, headerStyle);
            RenderCell(ref row, ref currentColumn, name, headerStyle);
            while (currentColumn <= 7)
            {
                RenderCell(ref row, ref currentColumn, Labels.Agency, headerStyle);
            }

            worksheet.AddMergedRegion(new CellRangeAddress(currentRow, currentRow, currentColumn - 6, currentColumn - 1));
            while (currentColumn <= 16)
            {
                RenderCell(ref row, ref currentColumn, Labels.IBC, headerStyle);
            }

            worksheet.AddMergedRegion(new CellRangeAddress(currentRow, currentRow, currentColumn - 9, currentColumn - 1));
            while (currentColumn <= 24)
            {
                RenderCell(ref row, ref currentColumn, RacingResources.Partner, headerStyle);
            }

            worksheet.AddMergedRegion(new CellRangeAddress(currentRow, currentRow, currentColumn - 8, currentColumn - 1));
            currentRow++;
            currentColumn = 0;
            row = worksheet.CreateRow(currentRow);
            RenderCell(ref row, ref currentColumn, nameContainer, headerStyle);
            worksheet.AddMergedRegion(new CellRangeAddress(currentRow - 1, currentRow, currentColumn - 1, currentColumn - 1));
            RenderCell(ref row, ref currentColumn, name, headerStyle);
            worksheet.AddMergedRegion(new CellRangeAddress(currentRow - 1, currentRow, currentColumn - 1, currentColumn - 1));
            RenderCell(ref row, ref currentColumn, RacingResources.WinTO, headerStyle);
            RenderCell(ref row, ref currentColumn, RacingResources.PlaceTO, headerStyle);
            RenderCell(ref row, ref currentColumn, RacingResources.WPTO, headerStyle);
            RenderCell(ref row, ref currentColumn, RacingResources.TotalTO, headerStyle);
            RenderCell(ref row, ref currentColumn, RacingResources.CommIncome, headerStyle);
            RenderCell(ref row, ref currentColumn, RacingResources.PTWL, headerStyle);
            RenderCell(ref row, ref currentColumn, RacingResources.WinTO, headerStyle);
            RenderCell(ref row, ref currentColumn, RacingResources.PlaceTO, headerStyle);
            RenderCell(ref row, ref currentColumn, RacingResources.WPTO, headerStyle);
            RenderCell(ref row, ref currentColumn, RacingResources.TotalTO, headerStyle);
            RenderCell(ref row, ref currentColumn, RacingResources.CommIncome, headerStyle);
            RenderCell(ref row, ref currentColumn, RacingResources.ToteFee, headerStyle);
            RenderCell(ref row, ref currentColumn, RacingResources.CommExpense, headerStyle);
            RenderCell(ref row, ref currentColumn, RacingResources.PTWL, headerStyle);
            RenderCell(ref row, ref currentColumn, RacingResources.NetIncome, headerStyle);
            RenderCell(ref row, ref currentColumn, RacingResources.WinTO, headerStyle);
            RenderCell(ref row, ref currentColumn, RacingResources.PlaceTO, headerStyle);
            RenderCell(ref row, ref currentColumn, RacingResources.WPTO, headerStyle);
            RenderCell(ref row, ref currentColumn, RacingResources.TotalTO, headerStyle);
            RenderCell(ref row, ref currentColumn, RacingResources.DividendWon, headerStyle);
            RenderCell(ref row, ref currentColumn, RacingResources.CommExpense, headerStyle);
            RenderCell(ref row, ref currentColumn, RacingResources.ToteFee, headerStyle);
            RenderCell(ref row, ref currentColumn, RacingResources.AmountDue, headerStyle);
        }

        /// <summary>
        /// Render the content of report.
        /// </summary>
        /// <param name="excelFile">The excel file.</param>
        /// <param name="worksheet">The worksheet.</param>
        /// <param name="currentRow">The current row.</param>
        /// <param name="report">The report.</param>
        public void RenderRacingWLMainContent(
            HSSFWorkbook excelFile,
            ref ISheet worksheet,
            ref int currentRow,
            List<WinLossTote> report)
        {
            currentRow++;
            for (var i = 0; i < report.Count; i++)
            {
                var racingWL = report[i];
                RenderContentByGroup(excelFile, ref worksheet, ref currentRow, racingWL, this._normalRowStyle, false);
                RenderContentByGroup(excelFile, ref worksheet, ref currentRow, racingWL.Total, this._totalRowStyle, true);
                worksheet.AddMergedRegion(new CellRangeAddress(currentRow, currentRow, 0, 24));
                currentRow++;
            }
        }

        /// <summary>
        /// Render footer for racing win loss main.
        /// </summary>
        /// <param name="excelFile">The excel file.</param>
        /// <param name="worksheet">The work sheet.</param>
        /// <param name="currentRow">The current row.</param>
        /// <param name="report">The win loss tote.</param>
        public void RenderRacingWLMainFooter(
            HSSFWorkbook excelFile,
            ref ISheet worksheet,
            ref int currentRow,
            WinLossTote report)
        {
            RenderContentByGroup(excelFile, ref worksheet, ref currentRow, report, this._grandTotalRowStyle, true);
        }

        /// <summary>
        /// Render content for group.
        /// </summary>
        /// <param name="excelFile">The excel file.</param>
        /// <param name="worksheet">The work sheet.</param>
        /// <param name="currentRow">The current row.</param>
        /// <param name="winLossByPartner">The win loss tote.</param>
        /// <param name="cellStyle">Style for cell.</param>
        /// <param name="bold">Check bold.</param>
        public void RenderContentByGroup(
            HSSFWorkbook excelFile,
            ref ISheet worksheet,
            ref int currentRow,
            WinLossTote winLossByPartner,
            CustomStyleCell cellStyle,
            bool bold)
        {
            int partnerLength = winLossByPartner.ChildsInfo.Count;
            int currentColumn;
            for (var i = 0; i < partnerLength; i++)
            {
                IRow row = worksheet.CreateRow(currentRow);
                currentColumn = 0;
                RenderCell(ref row, ref currentColumn, winLossByPartner.PartnerName, cellStyle.TextStyle);
                var winLoss = winLossByPartner.ChildsInfo[i];
                RenderCell(ref row, ref currentColumn, winLoss.CurrencyName, cellStyle.TextStyle);
                RenderMainRow(ref row, ref currentColumn, winLoss, cellStyle.DecimalStyle);
                currentRow++;
            }

            worksheet.AddMergedRegion(new CellRangeAddress(currentRow - partnerLength, currentRow - 1, 0, 0));
        }

        /// <summary>
        /// Render main row.
        /// </summary>
        /// <param name="row">The row of work sheet.</param>
        /// <param name="currentColumn">The current column.</param>
        /// <param name="rowData">The win loss tote.</param>
        /// <param name="decimalStyle">The style of style.</param>
        public void RenderMainRow(
            ref IRow row,
            ref int currentColumn,
            WinLossTote rowData,
            ICellStyle decimalStyle)
        {
            RenderCell(ref row, ref currentColumn, rowData.WinTOAgency, decimalStyle);
            RenderCell(ref row, ref currentColumn, rowData.PlaceTOAgency, decimalStyle);
            RenderCell(ref row, ref currentColumn, rowData.WinPlaceTOAgency, decimalStyle);
            RenderCell(ref row, ref currentColumn, rowData.TotalTOAgency, decimalStyle);
            RenderCell(ref row, ref currentColumn, rowData.CommIncomeAgency, decimalStyle);
            RenderCell(ref row, ref currentColumn, rowData.PTWinLossAgency, decimalStyle);
            RenderCell(ref row, ref currentColumn, rowData.WinTOIBC, decimalStyle);
            RenderCell(ref row, ref currentColumn, rowData.PlaceTOIBC, decimalStyle);
            RenderCell(ref row, ref currentColumn, rowData.WinPlaceTOIBC, decimalStyle);
            RenderCell(ref row, ref currentColumn, rowData.TotalTOIBC, decimalStyle);
            RenderCell(ref row, ref currentColumn, rowData.CommIncomeIBC, decimalStyle);
            RenderCell(ref row, ref currentColumn, rowData.ToteFeeIBC, decimalStyle);
            RenderCell(ref row, ref currentColumn, rowData.CommExpenseIBC, decimalStyle);
            RenderCell(ref row, ref currentColumn, rowData.PTWinLossIBC, decimalStyle);
            RenderCell(ref row, ref currentColumn, rowData.NetIncomeIBC, decimalStyle);
            RenderCell(ref row, ref currentColumn, rowData.WinTOPartner, decimalStyle);
            RenderCell(ref row, ref currentColumn, rowData.PlaceTOPartner, decimalStyle);
            RenderCell(ref row, ref currentColumn, rowData.WinPlaceTOPartner, decimalStyle);
            RenderCell(ref row, ref currentColumn, rowData.TotalTOPartner, decimalStyle);
            RenderCell(ref row, ref currentColumn, rowData.DividendWonPartner, decimalStyle);
            RenderCell(ref row, ref currentColumn, rowData.CommExpensePartner, decimalStyle);
            RenderCell(ref row, ref currentColumn, rowData.ToteFeePartner, decimalStyle);
            RenderCell(ref row, ref currentColumn, rowData.AmountDuePartner, decimalStyle);
        }

        /// <summary>
        /// Render content racing for win loss detail.
        /// </summary>
        /// <param name="excelFile">The excel file.</param>
        /// <param name="worksheet">The work sheet.</param>
        /// <param name="currentRow">The current row.</param>
        /// <param name="report">The list win loss tote detail.</param>
        [SuppressMessage("StyleCop.CSharp.Nexcel.NexcelCustomRules",
        "SP2101:MethodMustNotContainMoreLinesThan",
        Justification = "Reviewed. Suppression is OK here.")]
        public void RenderRacingWLDetailContent(HSSFWorkbook excelFile, ref ISheet worksheet, ref int currentRow, List<WinLossToteDetail> report)
        {
            int currentColumn;
            IRow row;
            for (var i = 0; i < report.Count; i++)
            {
                currentColumn = 0;
                var racingWLByCurr = report[i];
                var numberOfLeague = racingWLByCurr.ChildsInfo.Count;
                for (var j = 0; j < numberOfLeague; j++)
                {
                    currentColumn = 0;
                    var racingWLByLeague = racingWLByCurr.ChildsInfo[j];
                    var numberOfRace = racingWLByLeague.ChildsInfo.Count;
                    for (var k = 0; k < numberOfRace; k++)
                    {
                        currentColumn = 0;
                        currentRow++;
                        row = worksheet.CreateRow(currentRow);
                        var raceDetail = racingWLByLeague.ChildsInfo[k];
                        RenderCell(ref row, ref currentColumn, racingWLByLeague.LeagueName, this._normalRowStyle.TextStyle);
                        RenderCell(ref row, ref currentColumn, raceDetail.RaceNo, this._normalRowStyle.TextStyle);
                        RenderRacingDetailRow(
                                ref row,
                                ref currentColumn,
                                raceDetail,
                                this._normalRowStyle.DecimalStyle);
                    }

                    worksheet.AddMergedRegion(new CellRangeAddress(currentRow - numberOfRace + 1, currentRow, 0, 0));
                    currentRow++;
                    row = worksheet.CreateRow(currentRow);
                    currentColumn = 0;
                    var leagueTotal = racingWLByLeague.Total;
                    RenderCell(ref row, ref currentColumn, leagueTotal.LeagueName, this._totalRowCurrDetail.TextStyle);
                    RenderCell(ref row, ref currentColumn, leagueTotal.LeagueName, this._totalRowCurrDetail.TextStyle);
                    worksheet.AddMergedRegion(new CellRangeAddress(currentRow, currentRow, currentColumn - 2, currentColumn - 1));
                    RenderRacingDetailRow(
                               ref row,
                               ref currentColumn,
                               leagueTotal,
                               this._totalRowCurrDetail.DecimalStyle);
                }

                currentColumn = 0;
                var totalByCurr = racingWLByCurr.Total;
                var totalByCurrLength = totalByCurr.ChildsInfo.Count;
                for (int l = 0; l < totalByCurrLength; l++)
                {
                    var grandTotalByCurrDetail = totalByCurr.ChildsInfo[l];
                    currentColumn = 0;
                    currentRow++;
                    row = worksheet.CreateRow(currentRow);
                    RenderCell(ref row, ref currentColumn, totalByCurr.LeagueName, this._totalRowStyle.TextStyle);
                    RenderCell(ref row, ref currentColumn, grandTotalByCurrDetail.CurrencyName, this._totalRowStyle.TextStyle);
                    RenderRacingDetailRow(ref row, ref currentColumn, grandTotalByCurrDetail, this._totalRowStyle.DecimalStyle);
                }

                worksheet.AddMergedRegion(new CellRangeAddress(currentRow - totalByCurrLength + 1, currentRow, 0, 0));
                currentRow++;
                worksheet.AddMergedRegion(new CellRangeAddress(currentRow, currentRow, 0, 24));
            }
        }

        /// <summary>
        /// Render footer for racing win loss detail.
        /// </summary>
        /// <param name="excelFile">The excel file.</param>
        /// <param name="worksheet">The work sheet.</param>
        /// <param name="currentRow">The current row.</param>
        /// <param name="report">The win loss tote detail.</param>
        public void RenderRacingWLDetailFooter(HSSFWorkbook excelFile, ref ISheet worksheet, ref int currentRow, WinLossToteDetail report)
        {
            int currentColumn = 0;
            var numberOfTotal = report.ChildsInfo.Count;
            for (int i = 0; i < numberOfTotal; i++)
            {
                currentColumn = 0;
                currentRow++;
                IRow row = worksheet.CreateRow(currentRow);
                var totalItem = report.ChildsInfo[i];
                if (i < numberOfTotal - 1)
                {
                    RenderCell(ref row, ref currentColumn, totalItem.LeagueName, this._totalDetail.TextStyle);
                    RenderCell(ref row, ref currentColumn, totalItem.CurrencyName, this._totalDetail.TextStyle);
                    RenderRacingDetailRow(ref row, ref currentColumn, totalItem, this._totalDetail.DecimalStyle);
                }
                else
                {
                    currentColumn = 0;
                    var grandTotalCount = totalItem.ChildsInfo.Count;
                    for (int j = 0; j < grandTotalCount; j++)
                    {
                        var grandTotalItem = totalItem.ChildsInfo[j];
                        currentColumn = 0;
                        row = worksheet.CreateRow(currentRow);
                        RenderCell(ref row, ref currentColumn, totalItem.LeagueName, this._grandTotalRowStyle.TextStyle);
                        RenderCell(ref row, ref currentColumn, grandTotalItem.CurrencyName, this._grandTotalRowStyle.TextStyle);
                        RenderRacingDetailRow(ref row, ref currentColumn, grandTotalItem, this._grandTotalRowStyle.DecimalStyle);
                        currentRow++;
                    }

                    worksheet.AddMergedRegion(new CellRangeAddress(currentRow - grandTotalCount, currentRow - 1, 0, 0));
                }
            }
        }

        /// <summary>
        /// Render racing for detail.
        /// </summary>
        /// <param name="row">The row of work sheet.</param>
        /// <param name="currentColumn">The current column.</param>
        /// <param name="rowData">The win loss tote detail.</param>
        /// <param name="decimalStyle">Style for decimal column.</param>
        public void RenderRacingDetailRow(
            ref IRow row,
            ref int currentColumn,
            WinLossToteDetail rowData,
            ICellStyle decimalStyle)
        {
            RenderCell(ref row, ref currentColumn, rowData.WinTOAgency, decimalStyle);
            RenderCell(ref row, ref currentColumn, rowData.PlaceTOAgency, decimalStyle);
            RenderCell(ref row, ref currentColumn, rowData.WinPlaceTOAgency, decimalStyle);
            RenderCell(ref row, ref currentColumn, rowData.TotalTOAgency, decimalStyle);
            RenderCell(ref row, ref currentColumn, rowData.CommIncomeAgency, decimalStyle);
            RenderCell(ref row, ref currentColumn, rowData.PTWinLossAgency, decimalStyle);
            RenderCell(ref row, ref currentColumn, rowData.WinTOIBC, decimalStyle);
            RenderCell(ref row, ref currentColumn, rowData.PlaceTOIBC, decimalStyle);
            RenderCell(ref row, ref currentColumn, rowData.WinPlaceTOIBC, decimalStyle);
            RenderCell(ref row, ref currentColumn, rowData.TotalTOIBC, decimalStyle);
            RenderCell(ref row, ref currentColumn, rowData.CommIncomeIBC, decimalStyle);
            RenderCell(ref row, ref currentColumn, rowData.ToteFeeIBC, decimalStyle);
            RenderCell(ref row, ref currentColumn, rowData.CommExpenseIBC, decimalStyle);
            RenderCell(ref row, ref currentColumn, rowData.PTWinLossIBC, decimalStyle);
            RenderCell(ref row, ref currentColumn, rowData.NetIncomeIBC, decimalStyle);
            RenderCell(ref row, ref currentColumn, rowData.WinTOPartner, decimalStyle);
            RenderCell(ref row, ref currentColumn, rowData.PlaceTOPartner, decimalStyle);
            RenderCell(ref row, ref currentColumn, rowData.WinPlaceTOPartner, decimalStyle);
            RenderCell(ref row, ref currentColumn, rowData.TotalTOPartner, decimalStyle);
            RenderCell(ref row, ref currentColumn, rowData.DividendWonPartner, decimalStyle);
            RenderCell(ref row, ref currentColumn, rowData.CommExpensePartner, decimalStyle);
            RenderCell(ref row, ref currentColumn, rowData.ToteFeePartner, decimalStyle);
            RenderCell(ref row, ref currentColumn, rowData.AmountDuePartner, decimalStyle);
        }

        /// <summary>
        /// Render row.
        /// </summary>
        /// <param name="workSheet">The worksheet.</param>
        /// <param name="row">The row of worksheet.</param>
        /// <param name="currentRow">The current row.</param>
        /// <param name="currentColumn">The current column.</param>
        /// <param name="endColumn">The end column.</param>
        /// <param name="cellValue">The value of cell.</param>
        /// <param name="cellStyle">The style of cell.</param>
        private void RenderRow(
            ref ISheet workSheet,
            ref IRow row,
            ref int currentRow,
            int currentColumn,
            int endColumn,
            object cellValue,
            ICellStyle cellStyle)
        {
            row = workSheet.CreateRow(currentRow);
            while (currentColumn <= endColumn)
            {
                RenderCell(ref row, ref currentColumn, cellValue, cellStyle);
            }
        }

        /// <summary>
        /// Render cell.
        /// </summary>
        /// <param name="row">The row of worksheet.</param>
        /// <param name="currentColumn">Current column.</param>
        /// <param name="cellValue">Value of cell.</param>
        /// <param name="cellStyle">Style of cell.</param>
        private void RenderCell(
            ref IRow row,
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
                cell.SetCellValue(Convert.ToInt64(cellValue));
            }

            cell.CellStyle = cellStyle;
            currentColumn++;
        }

        /// <summary>
        /// Initial the common style.
        /// </summary>
        /// <param name="excelFile">The excel file.</param>
        private void InitialStyle(HSSFWorkbook excelFile)
        {
            this._headerStyle = new CustomStyleCell(excelFile);
            this.InitCellStyle(excelFile, ref this._headerStyle, HeaderRowClass);

            this._normalRowStyle = new CustomStyleCell(excelFile);
            this.InitCellStyle(excelFile, ref this._normalRowStyle, NormalRowClass);

            this._totalRowStyle = new CustomStyleCell(excelFile);
            this.InitCellStyle(excelFile, ref this._totalRowStyle, TotalRowClass);

            this._grandTotalRowStyle = new CustomStyleCell(excelFile);
            this.InitCellStyle(excelFile, ref this._grandTotalRowStyle, GrandTotalRowClass);

            this._totalRowCurrDetail = new CustomStyleCell(excelFile);
            this.InitCellStyle(excelFile, ref this._totalRowCurrDetail, TotalRowByCurrDetail);

            this._totalDetail = new CustomStyleCell(excelFile);
            this.InitCellStyle(excelFile, ref this._totalDetail, TotalDetail);
        }

        /// <summary>
        /// Initial the style of cell.
        /// </summary>
        /// <param name="excelFile">The excel file.</param>
        /// <param name="styleCell">The style of cell.</param>
        /// <param name="rowClass">The class of row.</param>
        [SuppressMessage("StyleCop.CSharp.Nexcel.NexcelCustomRules",
        "SP2101:MethodMustNotContainMoreLinesThan",
        Justification = "Reviewed. Suppression is OK here.")]
        private void InitCellStyle(
           HSSFWorkbook excelFile,
           ref CustomStyleCell styleCell,
           string rowClass)
        {
            HSSFPalette palette = excelFile.GetCustomPalette();
            short fontBlackColor = HSSFColor.Black.Index;
            short cellColor;

            switch (rowClass)
            {
                case HeaderRowClass:
                    styleCell.TextStyle = NpoiExcelExtensions.HeaderStyle(excelFile);
                    break;

                case NormalRowClass:
                    cellColor = HSSFColor.White.Index;
                    styleCell.TextStyle = NpoiExcelExtensions.TextStyle(excelFile, cellColor, false, NpoiExcelExtensions.Alignment.Center, fontBlackColor);
                    styleCell.DecimalStyle = NpoiExcelExtensions.DecimalStyle(excelFile, cellColor, false, fontBlackColor);
                    break;

                case TotalRowClass:
                    palette.SetColorAtIndex(HSSFColor.Tan.Index, (byte)211, (byte)225, (byte)200);
                    cellColor = HSSFColor.Tan.Index;
                    styleCell.TextStyle = NpoiExcelExtensions.TextStyle(excelFile, cellColor, true, NpoiExcelExtensions.Alignment.Center, fontBlackColor);
                    styleCell.DecimalStyle = NpoiExcelExtensions.DecimalStyle(excelFile, cellColor, true, fontBlackColor);
                    break;

                case GrandTotalRowClass:
                    palette.SetColorAtIndex(HSSFColor.Teal.Index, (byte)219, (byte)225, (byte)187);
                    cellColor = HSSFColor.Teal.Index;
                    styleCell.TextStyle = NpoiExcelExtensions.TextStyle(excelFile, cellColor, true, NpoiExcelExtensions.Alignment.Center, fontBlackColor);
                    styleCell.DecimalStyle = NpoiExcelExtensions.DecimalStyle(excelFile, cellColor, true, fontBlackColor);
                    break;

                case TotalRowByCurrDetail:
                    palette.SetColorAtIndex(HSSFColor.Turquoise.Index, (byte)255, (byte)238, (byte)221);
                    cellColor = HSSFColor.Turquoise.Index;
                    styleCell.TextStyle = NpoiExcelExtensions.TextStyle(excelFile, cellColor, true, NpoiExcelExtensions.Alignment.Center, fontBlackColor);
                    styleCell.DecimalStyle = NpoiExcelExtensions.DecimalStyle(excelFile, cellColor, true, fontBlackColor);
                    break;
                case TotalDetail:
                    palette.SetColorAtIndex(HSSFColor.Violet.Index, (byte)244, (byte)244, (byte)190);
                    cellColor = HSSFColor.Violet.Index;
                    styleCell.TextStyle = NpoiExcelExtensions.TextStyle(excelFile, cellColor, true, NpoiExcelExtensions.Alignment.Center, fontBlackColor);
                    styleCell.DecimalStyle = NpoiExcelExtensions.DecimalStyle(excelFile, cellColor, true, fontBlackColor);
                    break;
            }
        }

        public struct CustomStyleCell
        {
            public ICellStyle TextStyle;

            public ICellStyle DecimalStyle;

            public CustomStyleCell(HSSFWorkbook excelFile)
            {
                TextStyle = excelFile.CreateCellStyle();
                DecimalStyle = excelFile.CreateCellStyle();
            }
        }
    }
}
