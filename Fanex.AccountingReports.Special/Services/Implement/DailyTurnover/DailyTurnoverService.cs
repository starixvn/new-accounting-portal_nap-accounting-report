// -----------------------------------------------------------------------
// <copyright file="DailyTurnoverService.cs" company="Nexcel Solutions Vietnam">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Fanex.AccountingReports.Special.Services.Implement.DailyTurnover
{
    using System;
    using System.Collections.Generic;
    using Fanex.AccountingReports.Core.Extensions;
    using Fanex.AccountingReports.Core.Services.Impl;
    using Fanex.AccountingReports.Core.Web;
    using Fanex.AccountingReports.Special.Constants;
    using Fanex.AccountingReports.Special.Criteria.DailyTurnover;
    using Fanex.AccountingReports.Special.Models.DailyTurnover;
    using Fanex.AccountingReports.Special.Resources;
    using Fanex.AccountingReports.Special.Services.Interface.IDailyTurnover;
    using Fanex.Common;
    using NPOI.HSSF.UserModel;
    using NPOI.HSSF.Util;
    using NPOI.SS.UserModel;
    using NPOI.SS.Util;

    /// <summary>
    /// The implement service of Daily Turnover report.
    /// </summary>
    public class DailyTurnoverService : ServiceBase, IDailyTurnoverService
    {
        private const string HeaderRowClass = "header";

        private const string CurrencyRowClass = "currency-row";

        private const string NormalRowClass = "normal-row";

        private readonly string _dailyTurnoverPrefixSessionKey =
                    "Fanex.AccountingReports.Special.DailyTurnover.";

        private string _dailyTurnoverSessionKey = string.Empty;

        private ExcelUtil.CustomStyleCell _headerStyle;

        private ExcelUtil.CustomStyleCell _currencyRowStyle;

        private ExcelUtil.CustomStyleCell _normalRowStyle;

        public DailyTurnoverService()
        {
            if (AppSession.CurrentUser != null)
            {
                this._dailyTurnoverSessionKey = this._dailyTurnoverPrefixSessionKey +
                                                    AppSession.CurrentUser.Username;
            }
            else
            {
                this._dailyTurnoverSessionKey = this._dailyTurnoverPrefixSessionKey;
            }
        }

        /// <summary>
        /// Get report.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>The list DailyTurnover.</returns>
        public List<DailyTurnover> GetReport(GetDailyTurnoverCriteria criteria) 
        {
            try
            {
                var report = new List<DailyTurnover>();

                if (!criteria.IsValid())
                {
                    return report;
                }

                report = this.ConvertReportData(criteria);

                SessionManager.Set<List<DailyTurnover>>(this._dailyTurnoverSessionKey, report);

                return report;
            }
            catch (Exception ex)
            {
                this.HandleException(ex);
                throw;
            }
        }

        /// <summary>
        /// Export Report.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        public void ExportReport(GetDailyTurnoverCriteria criteria)
        {
            try
            {
                var report = SessionManager.Get<List<DailyTurnover>>(this._dailyTurnoverSessionKey);

                if (report == null)
                {
                    report = this.GetReport(criteria);
                }

                HSSFWorkbook excelFile = new HSSFWorkbook();

                string sheetName = SpecialResources.DailyTurnover_SheetName;
                ISheet worksheet = excelFile.CreateSheet();

                int currentRow = 0;

                if (report.Count > 0)
                {
                    this.InitStyle(excelFile);
                    this.ExportReportHeader(ref worksheet, ref currentRow, criteria);
                    this.ExportReportContent(ref worksheet, ref currentRow, report);
                }

                excelFile.ToReponseStream(string.Format(
                    SpecialResources.DailyTurnover_ReportName,
                    criteria.FromDate.ToShortFormatedDateWithSlash().Replace('/', '_')));
            }
            catch (Exception ex)
            {
                this.HandleException(ex);
            }
        }

        /// <summary>
        /// Convert and put data into report.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>The list Daily Turnover.</returns>
        private List<DailyTurnover> ConvertReportData(GetDailyTurnoverCriteria criteria)
        {
            var report = new List<DailyTurnover>();

            var virtualCurrency = this.GetCurrency(criteria, false);
            var actualCurrency = this.GetCurrency(criteria, true);

            if (virtualCurrency != null && actualCurrency != null)
            {
                report.Add(virtualCurrency);
                report.Add(actualCurrency);
            }

            return report;
        }

        /// <summary>
        /// Get information of currency.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <param name="isActual">The parameter to identify type of currency.</param>
        /// <returns>The Daily Turnover.</returns>
        private DailyTurnover GetCurrency(GetDailyTurnoverCriteria criteria, bool isActual)
        {
            criteria.IsActual = isActual;
            var rowsData = GetReportData<dynamic>(criteria);
            DailyTurnover dailyTurnover = null;

            foreach (var row in rowsData)
            {
                DateTime eventDate = Converter.ToDateTime(row.eventdate);
                int ticketCount = Converter.ToInt(row.TicketCNT);
                int normalCount = Converter.ToInt(row.NormalCNT);
                int rejectCount = Converter.ToInt(row.RejectCNT);

                var turnover = new TurnoverInformation
                {
                    TotalTurnover = Converter.ToDecimal(row.total),
                    SuperTurnover = Converter.ToDecimal(row.super),
                    MasterTurnover = Converter.ToDecimal(row.master),
                    AgentTurnover = Converter.ToDecimal(row.agent),
                    IBCTurnover = Converter.ToDecimal(row.after),
                    IsActual = isActual
                };

                dailyTurnover = new DailyTurnover
                {
                    Date = eventDate.ToShortFormatedDate(),
                    Turnover = turnover,
                    TicketNormalRejectCount = string.Format("{0:n0}", ticketCount)
                                                + "/" + string.Format("{0:n0}", normalCount)
                                                + "/" + string.Format("{0:n0}", rejectCount)
                };
            }

            return dailyTurnover;
        }

        /// <summary>
        /// Initial the common style.
        /// </summary>
        /// <param name="excelFile">The excel file.</param>
        private void InitStyle(HSSFWorkbook excelFile)
        {
            this._headerStyle = new ExcelUtil.CustomStyleCell(excelFile);
            this.InitCellStyle(excelFile, ref this._headerStyle, HeaderRowClass);

            this._currencyRowStyle = new ExcelUtil.CustomStyleCell(excelFile);
            this.InitCellStyle(excelFile, ref this._currencyRowStyle, CurrencyRowClass);

            this._normalRowStyle = new ExcelUtil.CustomStyleCell(excelFile);
            this.InitCellStyle(excelFile, ref this._normalRowStyle, NormalRowClass);
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
            GetDailyTurnoverCriteria criteria)
        {
            ICellStyle headerStyle = this._headerStyle.TextStyle;
            var currentColumn = 0;

            for (var i = 0; i < 7; i++)
            {
                worksheet.SetColumnWidth(i, 170 * ExcelExtensions.FontWidth);
            }

            worksheet.SetColumnWidth(6, 300 * ExcelExtensions.FontWidth);

            var reportTitle = string.Format(
                    SpecialResources.DailyTurnover_ReportTitle,
                    criteria.FromDate.ToShortFormatedDate());
            IRow row = worksheet.GetRow(currentRow) ?? worksheet.CreateRow(currentRow);

            ExcelUtil.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow + 1, currentColumn + 6, reportTitle, headerStyle);
            currentRow += 2;
            currentColumn = 0;

            worksheet.AddMergedRegion(new CellRangeAddress(currentRow, currentRow, currentColumn, 6));
            currentRow++;
            currentColumn = 0;

            row = worksheet.GetRow(currentRow) ?? worksheet.CreateRow(currentRow);

            ExcelUtil.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow, currentColumn, Labels.Date, headerStyle);
            ExcelUtil.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow, currentColumn, Labels.Total_Turnover, headerStyle);
            ExcelUtil.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow, currentColumn, Labels.Super_Turnover, headerStyle);
            ExcelUtil.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow, currentColumn, Labels.Master_Turnover, headerStyle);
            ExcelUtil.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow, currentColumn, Labels.Agent_Turnover, headerStyle);
            ExcelUtil.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow, currentColumn, Labels.IBC_Turnover, headerStyle);
            ExcelUtil.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow, currentColumn, Labels.CNT, headerStyle);

            currentRow++;
        }

        /// <summary>
        /// Export report content.
        /// </summary>
        /// <param name="worksheet">The worksheet.</param>
        /// <param name="currentRow">The current row.</param>
        /// <param name="report">The report.</param>
        private void ExportReportContent(
            ref ISheet worksheet, 
            ref int currentRow, 
            List<DailyTurnover> report)
        {
            this.ExportReportCurrency(ref worksheet, ref currentRow, report[0], true);
            this.ExportReportCurrency(ref worksheet, ref currentRow, report[1], false);
        }

        /// <summary>
        /// Export report by currency.
        /// </summary>
        /// <param name="worksheet">The worksheet.</param>
        /// <param name="currentRow">The current row.</param>
        /// <param name="currentCurrency">The current currency.</param>
        /// <param name="isVirtual">The boolean isVirtual.</param>
        private void ExportReportCurrency(
            ref ISheet worksheet, 
            ref int currentRow, 
            DailyTurnover currentCurrency,
            bool isVirtual)
        {
            var currentColumn = 0;
            IRow row = worksheet.CreateRow(currentRow);

            if (isVirtual)
            {
                ExcelUtil.RenderCell(
                    ref worksheet, 
                    ref row, 
                    currentRow, 
                    ref currentColumn, 
                    currentRow, 
                    currentColumn + 6, 
                    Labels.Virtual_Currency, 
                    _currencyRowStyle.TextStyle);
            }
            else 
            {
                ExcelUtil.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow, currentColumn + 6, Labels.Actual_Currency, _currencyRowStyle.TextStyle);
            }

            currentRow++;
            row = worksheet.GetRow(currentRow) ?? worksheet.CreateRow(currentRow);
            currentColumn = 0;

            ICellStyle textStyle = this._normalRowStyle.TextStyle;
            ICellStyle decimalStyle = this._normalRowStyle.DecimalStyle;

            ExcelUtil.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow, currentColumn, currentCurrency.Date, textStyle);
            ExcelUtil.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow, currentColumn, currentCurrency.Turnover.TotalTurnover, decimalStyle);
            ExcelUtil.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow, currentColumn, currentCurrency.Turnover.SuperTurnover, decimalStyle);
            ExcelUtil.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow, currentColumn, currentCurrency.Turnover.MasterTurnover, decimalStyle);
            ExcelUtil.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow, currentColumn, currentCurrency.Turnover.AgentTurnover, decimalStyle);
            ExcelUtil.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow, currentColumn, currentCurrency.Turnover.IBCTurnover, decimalStyle);
            ExcelUtil.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow, currentColumn, currentCurrency.TicketNormalRejectCount, textStyle);

            currentRow++;
        }  

        /// <summary>
        /// Initial cell style.
        /// </summary>
        /// <param name="excelFile">The excel file.</param>
        /// <param name="styleCell">The style of cell.</param>
        /// <param name="cssClass">The class of row.</param>
        private void InitCellStyle(
            HSSFWorkbook excelFile,
            ref ExcelUtil.CustomStyleCell styleCell, 
            string cssClass)
        {
            HSSFPalette palette = excelFile.GetCustomPalette();
            IFont font = excelFile.CreateFont();
            IDataFormat dataFormat = excelFile.CreateDataFormat();

            switch (cssClass)
            {
                case HeaderRowClass:
                    CellStyleHeaderRow(ref styleCell.TextStyle, palette, font);
                    break;

                case CurrencyRowClass:
                    CellStyleCurrencyRow(ref styleCell.TextStyle, palette, font);
                    break;

                case NormalRowClass:
                    short cellColor = HSSFColor.White.Index;
                    SetCellStyle(ref styleCell, font, dataFormat, cellColor);
                    break;
            }
        }

        /// <summary>
        /// The cell style of header row.
        /// </summary>
        /// <param name="textStyle">The text style.</param>
        /// <param name="palette">The palette.</param>
        /// <param name="font">The font setting.</param>
        private void CellStyleHeaderRow(ref ICellStyle textStyle, HSSFPalette palette, IFont font)
        {
            palette.SetColorAtIndex(HSSFColor.Pink.Index, (byte)60, (byte)94, (byte)165);
            font.Boldweight = (short)FontBoldWeight.Bold;
            font.Color = HSSFColor.White.Index;
            font.FontHeightInPoints = SpecialConstants.FontSize;
            font.FontName = SpecialConstants.FontName;

            textStyle.FillForegroundColor = HSSFColor.Pink.Index;
            textStyle.FillPattern = FillPattern.SolidForeground;
            textStyle.Alignment = HorizontalAlignment.Center;
            textStyle.VerticalAlignment = VerticalAlignment.Center;
            textStyle.SetFont(font);
            textStyle = ExcelUtil.SetBorderToCell(textStyle);
        }

        /// <summary>
        /// The cell style of currency row.
        /// </summary>
        /// <param name="textStyle">The text style.</param>
        /// <param name="palette">The palette.</param>
        /// <param name="font">The font setting.</param>
        private void CellStyleCurrencyRow(ref ICellStyle textStyle, HSSFPalette palette, IFont font)
        {
            palette.SetColorAtIndex(HSSFColor.Tan.Index, (byte)255, (byte)243, (byte)200);
            font.Boldweight = (short)FontBoldWeight.Bold;
            font.FontHeightInPoints = SpecialConstants.FontSize;
            font.FontName = SpecialConstants.FontName;

            textStyle.FillForegroundColor = HSSFColor.Tan.Index;
            textStyle.FillPattern = FillPattern.SolidForeground;
            textStyle.Alignment = HorizontalAlignment.Left;
            textStyle.SetFont(font);
            textStyle = ExcelUtil.SetBorderToCell(textStyle);
        }

        /// <summary>
        /// Set cell style.
        /// </summary>
        /// <param name="styleCell">The style cell.</param>
        /// <param name="font">The font setting.</param>
        /// <param name="dataFormat">The data format.</param>
        /// <param name="cellColor">The cell color.</param>
        private void SetCellStyle(
            ref ExcelUtil.CustomStyleCell styleCell,
            IFont font,
            IDataFormat dataFormat,
            short cellColor)
        {
            font.FontHeightInPoints = SpecialConstants.FontSize;
            font.FontName = SpecialConstants.FontName;

            styleCell.TextStyle.FillForegroundColor = cellColor;
            styleCell.TextStyle.FillPattern = FillPattern.SolidForeground;
            styleCell.TextStyle.Alignment = HorizontalAlignment.Center;
            styleCell.TextStyle.VerticalAlignment = VerticalAlignment.Center;
            styleCell.TextStyle.SetFont(font);
            styleCell.TextStyle = ExcelUtil.SetBorderToCell(styleCell.TextStyle);

            styleCell.DecimalStyle.FillForegroundColor = cellColor;
            styleCell.DecimalStyle.FillPattern = FillPattern.SolidForeground;
            styleCell.DecimalStyle.Alignment = HorizontalAlignment.Right;
            styleCell.DecimalStyle.DataFormat = dataFormat.GetFormat(SpecialConstants.IntNumberFormat);
            styleCell.DecimalStyle.SetFont(font);
            styleCell.DecimalStyle = ExcelUtil.SetBorderToCell(styleCell.DecimalStyle);
        }
    }
}
