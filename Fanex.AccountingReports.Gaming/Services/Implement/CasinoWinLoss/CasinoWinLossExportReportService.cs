// -----------------------------------------------------------------------
// <copyright file="CasinoWinLossExportReportService.cs" company="Nexcel Solutions Viet Nam">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Fanex.AccountingReports.Gaming.Services.Implement.CasinoWinLoss
{
    using System;
    using Fanex.AccountingReports.Core.Configurations;
    using Fanex.AccountingReports.Core.Extensions;
    using Fanex.AccountingReports.Core.Services.Impl;
    using Fanex.AccountingReports.Core.Web;
    using Fanex.AccountingReports.Gaming.Criteria.CasinoWinLoss;
    using Fanex.AccountingReports.Gaming.Models.CasinoWinLoss;
    using Fanex.AccountingReports.Gaming.Resources;
    using Fanex.AccountingReports.Gaming.Services.Interface.CasinoWinLoss;
    using NPOI.HSSF.UserModel;
    using NPOI.HSSF.Util;
    using NPOI.SS.UserModel;
    using NPOI.SS.Util;

    /// <summary>
    /// The export report service.
    /// </summary>
    public class CasinoWinLossExportReportService : ServiceBase, ICasinoWinLossExportReportService
    {
        #region declare variable

        private const string CasinoWinLossPrefixSessionKey = "Fanex.AccountingReports.Gaming.CasinoWinLoss.";

        private const string HeaderRowClass = "header";

        private const string BetTypeNormalRowClass = "bet-type-normal-row";

        private const string BetTypeBoldRowClass = "bet-type-bold-row";

        private const string BetTypeTotalRowClass = "bet-type-total-row";

        private const string OddTotalRowClass = "odd-total-row";

        private const string EvenTotalRowClass = "even-total-row";

        private const string GrandTotalRowClass = "grand-total-row";

        private CustomStyleCell _headerStyle;

        private CustomStyleCell _betTypeNormalStyle;

        private CustomStyleCell _betTypeBoldStyle;

        private CustomStyleCell _betTypeTotalStyle;

        private CustomStyleCell _oddTotalStyle;

        private CustomStyleCell _evenTotalStyle;

        private CustomStyleCell _grandTotalStyle;

        private string _casinoWinLossSessionKey = string.Empty;

        private CasinoWinLossGetReportService _getReportService;

        #endregion declare variable

        public CasinoWinLossExportReportService()
        {
            if (AppSession.CurrentUser != null)
            {
                _casinoWinLossSessionKey = CasinoWinLossPrefixSessionKey + AppSession.CurrentUser.Username;
            }
            else
            {
                _casinoWinLossSessionKey = CasinoWinLossPrefixSessionKey;
            }

            _getReportService = new CasinoWinLossGetReportService();
        }

        #region export report.

        /// <summary>
        /// Export report.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        public void ExportReport(GetCasinoWinLossCriteria criteria)
        {
            try
            {
                CasinoWinLoss report = SessionManager.Get<CasinoWinLoss>(_casinoWinLossSessionKey);

                if (report == null)
                {
                    report = _getReportService.GetReport(criteria);
                }

                HSSFWorkbook excelFile = new HSSFWorkbook();
                string sheetName = GamingResources.CasinoWinLoss_SheetName;
                ISheet worksheet = excelFile.CreateSheet(sheetName);
                var currentRow = 0;

                if (report != null && report.Rows != null && report.Rows.Count > 0)
                {
                    this.InitialStyle(excelFile);
                    this.ExportReportHeader(ref worksheet, ref currentRow, criteria);
                    this.ExportReportContent(ref worksheet, ref currentRow, report);
                }

                excelFile.ToReponseStream(string.Format(
                    GamingResources.CasinoWinLoss_ReportName,
                    criteria.WinLostDate.ToShortFormatedDateWithSlash().Replace("/", "_")));
            }
            catch (Exception ex)
            {
                this.HandleException(ex);
            }
        }

        /// <summary>
        /// Initial style.
        /// </summary>
        /// <param name="excelFile">The file excel.</param>
        private void InitialStyle(HSSFWorkbook excelFile)
        {
            this._headerStyle = new CustomStyleCell(excelFile);
            this.InitCellStyle(excelFile, ref this._headerStyle, HeaderRowClass);

            this._betTypeNormalStyle = new CustomStyleCell(excelFile);
            this.InitCellStyle(excelFile, ref this._betTypeNormalStyle, BetTypeNormalRowClass);

            this._betTypeBoldStyle = new CustomStyleCell(excelFile);
            this.InitCellStyle(excelFile, ref this._betTypeBoldStyle, BetTypeBoldRowClass);

            this._betTypeTotalStyle = new CustomStyleCell(excelFile);
            this.InitCellStyle(excelFile, ref this._betTypeTotalStyle, BetTypeTotalRowClass);

            this._oddTotalStyle = new CustomStyleCell(excelFile);
            this.InitCellStyle(excelFile, ref this._oddTotalStyle, OddTotalRowClass);

            this._evenTotalStyle = new CustomStyleCell(excelFile);
            this.InitCellStyle(excelFile, ref this._evenTotalStyle, EvenTotalRowClass);

            this._grandTotalStyle = new CustomStyleCell(excelFile);
            this.InitCellStyle(excelFile, ref this._grandTotalStyle, GrandTotalRowClass);
        } 

        /// <summary>
        /// Export report header.
        /// </summary>
        /// <param name="worksheet">The worksheet.</param>
        /// <param name="currentRow">The current row.</param>
        /// <param name="criteria">The criteria.</param>
        private void ExportReportHeader(
            ref ISheet worksheet, 
            ref int currentRow, 
            GetCasinoWinLossCriteria criteria)
        {
            var currentColumn = 0;
            ICellStyle headerStyle = this._headerStyle.TextStyle;
            IRow row = worksheet.GetRow(currentRow) ?? worksheet.CreateRow(currentRow);

            for (var i = 0; i < 7; i++)
            {
                worksheet.SetColumnWidth(i, 140 * NpoiExcelExtensions.FontWidth);
            }

            var reportTitle = string.Format(
                GamingResources.CasinoWinLoss_ReportTitle,
                criteria.WinLostDate.ToShortFormatedDate());
            NpoiExcelExtensions.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow + 1, currentColumn + 6, reportTitle, headerStyle);

            currentRow += 2;
            worksheet.AddMergedRegion(new CellRangeAddress(currentRow, currentRow, 0, 6));

            currentRow++;
            currentColumn = 0;
            row = worksheet.GetRow(currentRow) ?? worksheet.CreateRow(currentRow);

            NpoiExcelExtensions.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow, currentColumn, Labels.Bettype, headerStyle);
            NpoiExcelExtensions.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow, currentColumn, Labels.Currency, headerStyle);
            NpoiExcelExtensions.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow, currentColumn, Labels.Turnover, headerStyle);
            NpoiExcelExtensions.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow, currentColumn, Labels.WinLoss, headerStyle);
            NpoiExcelExtensions.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow, currentColumn, Labels.Position, headerStyle);
            NpoiExcelExtensions.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow, currentColumn, Labels.Commission, headerStyle);
            NpoiExcelExtensions.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow, currentColumn, Labels.LineTotal, headerStyle);

            currentRow++;
        }

        /// <summary>
        /// Export report content.
        /// </summary>
        /// <param name="worksheet">The worksheet.</param>
        /// <param name="currentRow">The current row.</param>
        /// <param name="report">The report.</param>
        private void ExportReportContent(ref ISheet worksheet, ref int currentRow, CasinoWinLoss report)
        {
            int numberBetType = report.Rows.Count - 1;
            bool isDisplayTotalCurrency = AppSettings.IsDisplayTotalCurrency;

            for (var i = 0; i < numberBetType; i++)
            {
                ExportBetType(ref worksheet, ref currentRow, report.Rows[i]);

                if (isDisplayTotalCurrency)
                {
                    worksheet.AddMergedRegion(new CellRangeAddress(currentRow, currentRow, 0, 6));
                    currentRow++;
                }
                else
                {
                    if (numberBetType > 1)
                    {
                        worksheet.AddMergedRegion(new CellRangeAddress(currentRow, currentRow, 0, 6));
                        currentRow++;
                    }
                }
            }

            if (isDisplayTotalCurrency)
            {
                ExportTotalCurrency(ref worksheet, ref currentRow, report.Rows[numberBetType]);
            }
            else
            {
                if (numberBetType > 1)
                {
                    ExportTotalCurrency(ref worksheet, ref currentRow, report.Rows[numberBetType]);
                }
            }

            if (isDisplayTotalCurrency)
            {
                ExportGrandTotal(ref worksheet, ref currentRow, report);
            }
        }

        /// <summary>
        /// Export bet type.
        /// </summary>
        /// <param name="worksheet">The worksheet.</param>
        /// <param name="currentRow">The current row.</param>
        /// <param name="betType">The bet type.</param>
        private void ExportBetType(ref ISheet worksheet, ref int currentRow, CasinoWinLoss betType)
        {
            int numberCurrencies = betType.Rows.Count;
            int currentColumn = 0;
            IRow row = worksheet.GetRow(currentRow) ?? worksheet.CreateRow(currentRow);
            NpoiExcelExtensions.RenderCell(
                ref worksheet, 
                ref row, 
                currentRow, 
                ref currentColumn, 
                currentRow + numberCurrencies, 
                currentColumn, 
                betType.Name, 
                _betTypeBoldStyle.TextStyle);

            for (var i = 0; i < numberCurrencies; i++)
            {
                CasinoWinLoss currency = betType.Rows[i];
                row = worksheet.GetRow(currentRow) ?? worksheet.CreateRow(currentRow);

                NpoiExcelExtensions.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow, currentColumn, currency.Name, _betTypeNormalStyle.TextStyle);
                ExportWinLoss(ref worksheet, ref row, currency, ref currentRow, currentColumn, _betTypeNormalStyle);
                currentColumn = 1;
            }

            row = worksheet.GetRow(currentRow) ?? worksheet.CreateRow(currentRow);
            string betTypeNameTotal = betType.Name + " " + Labels.Total;
            NpoiExcelExtensions.RenderCell(
                ref worksheet, 
                ref row, 
                currentRow, 
                ref currentColumn, 
                currentRow, 
                currentColumn, 
                betTypeNameTotal,
                _betTypeTotalStyle.TextStyle);
            ExportWinLoss(ref worksheet, ref row, betType, ref currentRow, currentColumn, _betTypeTotalStyle);
        }

        /// <summary>
        /// Export total of currency.
        /// </summary>
        /// <param name="worksheet">The worksheet.</param>
        /// <param name="currentRow">The current row.</param>
        /// <param name="casinoWinLoss">The value of currency.</param>
        private void ExportTotalCurrency(ref ISheet worksheet, ref int currentRow, CasinoWinLoss casinoWinLoss)
        {
            int numberCurrencies = casinoWinLoss.Rows.Count;
            int currentColumn = 0;
            IRow row = worksheet.GetRow(currentRow) ?? worksheet.CreateRow(currentRow);
            NpoiExcelExtensions.RenderCell(
                ref worksheet, 
                ref row, 
                currentRow, 
                ref currentColumn, 
                currentRow + numberCurrencies - 1, 
                currentColumn, 
                casinoWinLoss.Name, 
                _oddTotalStyle.TextStyle);

            for (var i = 0; i < numberCurrencies; i++)
            {
                currentColumn = 1;
                CasinoWinLoss currency = casinoWinLoss.Rows[i];
                row = worksheet.GetRow(currentRow) ?? worksheet.CreateRow(currentRow);
                CustomStyleCell rowStyle = i % 2 == 0 ? _oddTotalStyle : _evenTotalStyle;
                
                NpoiExcelExtensions.RenderCell(
                    ref worksheet, 
                    ref row, 
                    currentRow, 
                    ref currentColumn, 
                    currentRow, 
                    currentColumn, 
                    currency.Name, 
                    rowStyle.TextStyle);
                ExportWinLoss(ref worksheet, ref row, currency, ref currentRow, currentColumn, rowStyle);
            }
        }

        /// <summary>
        /// Export grand total.
        /// </summary>
        /// <param name="worksheet">The worksheet.</param>
        /// <param name="currentRow">The current row.</param>
        /// <param name="report">The report.</param>
        private void ExportGrandTotal(ref ISheet worksheet, ref int currentRow, CasinoWinLoss report)
        {
            IRow row = worksheet.GetRow(currentRow) ?? worksheet.CreateRow(currentRow);
            int currentColumn = 0;
            CustomStyleCell rowStyle = _grandTotalStyle;

            NpoiExcelExtensions.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow, currentColumn + 1, Labels.Grand_Total, rowStyle.TextStyle);
            ExportWinLoss(ref worksheet, ref row, report, ref currentRow, currentColumn, rowStyle);
        }
        
        /// <summary>
        /// Export win loss.
        /// </summary>
        /// <param name="worksheet">The worksheet.</param>
        /// <param name="row">The row to render.</param>
        /// <param name="casinoWinLoss">The value of win loss.</param>
        /// <param name="currentRow">The current row.</param>
        /// <param name="currentColumn">The current column.</param>
        /// <param name="rowStyle">The style of row.</param>
        private void ExportWinLoss(
            ref ISheet worksheet,
            ref IRow row,
            CasinoWinLoss casinoWinLoss,
            ref int currentRow,
            int currentColumn,
            CustomStyleCell rowStyle)
        {
            NpoiExcelExtensions.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow, currentColumn, casinoWinLoss.Turnover, rowStyle.DecimalStyle);
            NpoiExcelExtensions.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow, currentColumn, casinoWinLoss.WinLost, rowStyle.DecimalStyle);
            NpoiExcelExtensions.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow, currentColumn, casinoWinLoss.Position, rowStyle.DecimalStyle);
            NpoiExcelExtensions.RenderCell(
                ref worksheet, 
                ref row, 
                currentRow, 
                ref currentColumn, 
                currentRow, 
                currentColumn, 
                casinoWinLoss.Commission, 
                rowStyle.DecimalStyle);
            NpoiExcelExtensions.RenderCell(
                ref worksheet, 
                ref row, 
                currentRow, 
                ref currentColumn, 
                currentRow, 
                currentColumn, 
                casinoWinLoss.LineTotal, 
                rowStyle.DecimalStyle);
            currentRow++;
        } 

        /// <summary>
        /// Initial the style of cell.
        /// </summary>
        /// <param name="excelFile">The excel file.</param>
        /// <param name="styleCell">The style of cell.</param>
        /// <param name="rowClass">The class of row.</param> 
        private void InitCellStyle(
           HSSFWorkbook excelFile,
           ref CustomStyleCell styleCell,
           string rowClass)
        {
            HSSFPalette palette = excelFile.GetCustomPalette();
            short cellColor;

            switch (rowClass)
            {
                case HeaderRowClass:
                    styleCell.TextStyle = NpoiExcelExtensions.HeaderStyle(excelFile);
                    break;
                case BetTypeNormalRowClass:
                    cellColor = HSSFColor.White.Index;
                    styleCell.TextStyle = NpoiExcelExtensions.TextStyle(excelFile, cellColor, false, NpoiExcelExtensions.Alignment.Center);
                    styleCell.DecimalStyle = NpoiExcelExtensions.DecimalStyle(excelFile, cellColor, false);
                    break;
                case BetTypeBoldRowClass:
                    cellColor = HSSFColor.White.Index;
                    styleCell.TextStyle = NpoiExcelExtensions.TextStyle(excelFile, cellColor, true, NpoiExcelExtensions.Alignment.Center);
                    styleCell.DecimalStyle = NpoiExcelExtensions.DecimalStyle(excelFile, cellColor, false);
                    break;
                case BetTypeTotalRowClass:
                    cellColor = HSSFColor.White.Index;
                    styleCell.TextStyle = NpoiExcelExtensions.TextStyle(excelFile, cellColor, true, NpoiExcelExtensions.Alignment.Center);
                    styleCell.DecimalStyle = NpoiExcelExtensions.DecimalStyle(excelFile, cellColor, true);
                      break;
                case OddTotalRowClass:
                    palette.SetColorAtIndex(HSSFColor.Tan.Index, (byte)232, (byte)235, (byte)211);
                    cellColor = HSSFColor.Tan.Index;
                    styleCell.TextStyle = NpoiExcelExtensions.TextStyle(excelFile, cellColor, true, NpoiExcelExtensions.Alignment.Center);
                    styleCell.DecimalStyle = NpoiExcelExtensions.DecimalStyle(excelFile, cellColor, true);
                    break;
                case EvenTotalRowClass:
                    palette.SetColorAtIndex(HSSFColor.Teal.Index, (byte)228, (byte)242, (byte)253);
                    cellColor = HSSFColor.Teal.Index;
                    styleCell.TextStyle = NpoiExcelExtensions.TextStyle(excelFile, cellColor, true, NpoiExcelExtensions.Alignment.Center);
                    styleCell.DecimalStyle = NpoiExcelExtensions.DecimalStyle(excelFile, cellColor, true);
                    break; 
                case GrandTotalRowClass:
                    palette.SetColorAtIndex(HSSFColor.Turquoise.Index, (byte)219, (byte)225, (byte)187);
                    cellColor = HSSFColor.Turquoise.Index;
                    styleCell.TextStyle = NpoiExcelExtensions.TextStyle(excelFile, cellColor, true, NpoiExcelExtensions.Alignment.Center);
                    styleCell.DecimalStyle = NpoiExcelExtensions.DecimalStyle(excelFile, cellColor, true);
                    palette.SetColorAtIndex(HSSFColor.Plum.Index, (byte)255, (byte)102, (byte)0);
                    styleCell.TextStyle.BorderTop = BorderStyle.Medium;
                    styleCell.TextStyle.TopBorderColor = HSSFColor.Plum.Index;
                    styleCell.DecimalStyle.BorderTop = BorderStyle.Medium;
                    styleCell.DecimalStyle.TopBorderColor = HSSFColor.Plum.Index;
                    break;
            }
        }

        #endregion export report.

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
