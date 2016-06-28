// -----------------------------------------------------------------------
// <copyright file="DailyDetailChangedTurnoverExportReportService.cs" company="Nexcel Solutions Vietnam">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Fanex.AccountingReports.Special.Services.Implement.DailyTurnover
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using Fanex.AccountingReports.Core.Extensions;
    using Fanex.AccountingReports.Core.Services.Impl;
    using Fanex.AccountingReports.Core.Web;
    using Fanex.AccountingReports.Special.Constants;
    using Fanex.AccountingReports.Special.Criteria.DailyTurnover;
    using Fanex.AccountingReports.Special.Models.DailyTurnover;
    using Fanex.AccountingReports.Special.Resources;
    using Fanex.AccountingReports.Special.Services.Interface.DailyTurnover;
    using NPOI.HSSF.UserModel;
    using NPOI.HSSF.Util;
    using NPOI.SS.UserModel;
    using NPOI.SS.Util;

    /// <summary>
    /// The export report service of detail report.
    /// </summary>
    public class DailyDetailChangedTurnoverExportReportService : ServiceBase, IDailyDetailChangedTurnoverExportReportService
    {
        private const string HeaderRowClass = "header";

        private const string CurrencyOddRowClass = "currency-odd-row";

        private const string NormalOddRowClass = "normal-odd-row";

        private const string CurrencyEvenRowClass = "currency-even-row";

        private const string NormalEvenRowClass = "normal-even-row";

        private const string NormalDifferenceRowClass = "normal-difference-row";

        private readonly DailyDetailChangedTurnoverService _detailReportService;

        private readonly string _dailyTurnoverDetailPrefixSessionKey =
                                        "Fanex.AccountingReports.Special.DailyTurnover.Detail.";

        private ExcelUtil.CustomStyleCell _headerStyle;

        private ExcelUtil.CustomStyleCell _currencyOddRowStyle;

        private ExcelUtil.CustomStyleCell _currencyEvenRowStyle;

        private ExcelUtil.CustomStyleCell _normalOddRowStyle;

        private ExcelUtil.CustomStyleCell _normalEvenRowStyle;

        private ExcelUtil.CustomStyleCell _normalDifferenceRowStyle;

        private string _dailyTurnoverDetailSessionKey = string.Empty;

        public DailyDetailChangedTurnoverExportReportService()
        {
            if (AppSession.CurrentUser != null)
            {
                this._dailyTurnoverDetailSessionKey = this._dailyTurnoverDetailPrefixSessionKey
                                                            + AppSession.CurrentUser.Username;
            }
            else
            {
                this._dailyTurnoverDetailSessionKey = this._dailyTurnoverDetailPrefixSessionKey;
            }

            this._detailReportService = new DailyDetailChangedTurnoverService();
        }

        /// <summary>
        /// Export detail report.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        public void ExportDetailReport(GetDailyTurnoverChangedDetailCriteria criteria)
        {
            try
            {
                var report = SessionManager.Get<List<DailyDetailChangedTurnover>>(
                                                    this._dailyTurnoverDetailSessionKey);

                if (report == null)
                {
                    report = this._detailReportService.GetDetailReport(criteria);
                }

                HSSFWorkbook excelFile = new HSSFWorkbook();

                string sheetName = SpecialResources.DailyTurnoverDetail_SheetName;
                ISheet worksheet = excelFile.CreateSheet(sheetName);

                var currentRow = 0;

                if (report != null && report.Count > 0)
                {
                    this.InitStyleDetailReport(excelFile);
                    this.ExportReportHeader(ref worksheet, ref currentRow, criteria);
                    this.ExportReportContent(ref worksheet, ref currentRow, report);
                }

                excelFile.ToReponseStream(string.Format(
                        SpecialResources.DailyTurnoverDetail_ReportName,
                        criteria.FromDate.ToShortFormatedDateWithSlash()));
            }
            catch (Exception ex)
            {
                this.HandleException(ex);
            }
        }

        /// <summary>
        /// Export detail difference report.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="fromDate">The from date.</param>
        [SuppressMessage("StyleCop.CSharp.StylecopPlus.Rules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        public void ExportDetailDifferenceReport(string index, string fromDate)
        {
            try
            {
                string actionName = string.Empty;
                string date = string.Empty;

                var report = SessionManager.Get<List<DailyDetailChangedTurnover>>(
                                    this._dailyTurnoverDetailSessionKey);

                if (report == null)
                {
                    report = GetDetailReportFromSession(fromDate, report);
                }

                if (report != null)
                {
                    HSSFWorkbook excelFile = new HSSFWorkbook();

                    var currentRow = 0;
                    var eventDetail = report[Convert.ToInt32(index)];

                    if (eventDetail != null)
                    {
                        actionName = eventDetail.ActionName;
                        date = eventDetail.Date;

                        string sheetName = string.Format(
                            SpecialResources.DailyTurnoverDetailAction_SheetName,
                            actionName.Replace(" ", "_"),
                            date.Replace("/", "_").Replace(":", "_").Replace(" ", "_"));
                        ISheet worksheet = excelFile.CreateSheet(sheetName);

                        this.InitStyleDetailDifferenceReport(excelFile);
                        this.ExportDetailActionReportHeader(ref worksheet, ref currentRow, eventDetail);
                        this.ExportDetailActionReportContent(ref worksheet, ref currentRow, eventDetail);
                    }

                    var reportDate = DateTimeExtensions.ToShortFormatedDateWithSlash(Convert.ToDateTime(date));
                    excelFile.ToReponseStream(string.Format(
                        SpecialResources.DailyTurnoverDetailAction_ReportName,
                        actionName.Replace(" ", "_"),
                        reportDate.Replace("/", "_")));
                }
            }
            catch (Exception ex)
            {
                this.HandleException(ex);
            }
        }
        #region export detail difference

        /// <summary>
        /// Initial the style of detail difference report.
        /// </summary>
        /// <param name="excelFile">The excel file.</param>
        private void InitStyleDetailDifferenceReport(HSSFWorkbook excelFile)
        {
            this._headerStyle = new ExcelUtil.CustomStyleCell(excelFile);
            this.InitCellStyle(excelFile, ref this._headerStyle, HeaderRowClass);

            this._currencyOddRowStyle = new ExcelUtil.CustomStyleCell(excelFile);
            this.InitCellStyle(excelFile, ref this._currencyOddRowStyle, CurrencyOddRowClass);

            this._normalOddRowStyle = new ExcelUtil.CustomStyleCell(excelFile);
            this.InitCellStyle(excelFile, ref this._normalOddRowStyle, NormalOddRowClass);

            this._normalDifferenceRowStyle = new ExcelUtil.CustomStyleCell(excelFile);
            this.InitCellStyle(excelFile, ref this._normalDifferenceRowStyle, NormalDifferenceRowClass);
        }

        /// <summary>
        /// Get detail report form session.
        /// </summary>
        /// <param name="fromDate">The from date.</param>
        /// <param name="report">The report.</param>
        /// <returns>The list DailyDetailChangedTurnover.</returns>
        private List<DailyDetailChangedTurnover> GetDetailReportFromSession(
            string fromDate,
            List<DailyDetailChangedTurnover> report)
        {
            GetDailyTurnoverChangedDetailCriteria criteria =
                new GetDailyTurnoverChangedDetailCriteria();
            criteria.FromDate = DateTime.Parse(fromDate);
            report = this._detailReportService.GetDetailReport(criteria);

            SessionManager.Set<List<DailyDetailChangedTurnover>>(
                            this._dailyTurnoverDetailSessionKey,
                            report);
            return report;
        }

        /// <summary>
        /// Export detail action report header.
        /// </summary>
        /// <param name="worksheet">The worksheet.</param>
        /// <param name="currentRow">The current row.</param>
        /// <param name="eventDetail">The detail of event.</param>
        private void ExportDetailActionReportHeader(
            ref ISheet worksheet,
            ref int currentRow,
            DailyDetailChangedTurnover eventDetail)
        {
            ICellStyle headerStyle = this._headerStyle.TextStyle;
            var currentColumn = 0;

            for (var i = 0; i < 6; i++)
            {
                worksheet.SetColumnWidth(i, 170 * ExcelExtensions.FontWidth);
            }

            worksheet.SetColumnWidth(0, 250 * ExcelExtensions.FontWidth);

            IRow row = worksheet.GetRow(currentRow) ?? worksheet.CreateRow(currentRow);
            var reportTitle = string.Format(
                SpecialResources.DailyTurnoverDetailAction_ReportTitle,
                eventDetail.ActionName,
                eventDetail.Date);

            ExcelUtil.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow + 1, currentColumn + 5, reportTitle, headerStyle);

            currentRow += 2;
            currentColumn = 0;

            worksheet.AddMergedRegion(new CellRangeAddress(currentRow, currentRow, currentColumn, currentColumn + 5));
            currentColumn = 0;
            
            currentRow++;
            row = worksheet.GetRow(currentRow) ?? worksheet.CreateRow(currentRow);
            ExcelUtil.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow, currentColumn, Labels.Match, headerStyle);
            ExcelUtil.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow, currentColumn, Labels.Total_Turnover, headerStyle);
            ExcelUtil.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow, currentColumn, Labels.Super_Turnover, headerStyle);
            ExcelUtil.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow, currentColumn, Labels.Master_Turnover, headerStyle);
            ExcelUtil.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow, currentColumn, Labels.Agent_Turnover, headerStyle);
            ExcelUtil.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow, currentColumn, Labels.IBC_Turnover, headerStyle);

            currentRow++;
        }

        /// <summary>
        /// The export content of detail different turnover.
        /// </summary>
        /// <param name="worksheet">The worksheet.</param>
        /// <param name="currentRow">The current row.</param>
        /// <param name="eventDetail">The detail of event.</param>
        private void ExportDetailActionReportContent(
            ref ISheet worksheet,
            ref int currentRow,
            DailyDetailChangedTurnover eventDetail)
        {
            if (eventDetail.TurnoverDifference != null
                        && eventDetail.TurnoverDifference.Count >= 2)
            {
                var currentColumn = 0;
                IRow row = worksheet.GetRow(currentRow) ?? worksheet.CreateRow(currentRow);
                ExcelUtil.RenderCell(
                    ref worksheet, 
                    ref row, 
                    currentRow, 
                    ref currentColumn, 
                    currentRow + 3, 
                    currentColumn, 
                    eventDetail.Match, 
                    this._normalOddRowStyle.TextStyle);

                this.RenderTurnover(
                    ref worksheet,
                    ref row,
                    ref currentRow,
                    ref currentColumn,
                    eventDetail.TurnoverDifference,
                    this._currencyOddRowStyle,
                    this._normalDifferenceRowStyle);
            }
        }

        #endregion ecport detail difference

        #region export detail report

        /// <summary>
        /// Initial style of detail report.
        /// </summary>
        /// <param name="excelFile">The excel file.</param>
        private void InitStyleDetailReport(HSSFWorkbook excelFile)
        {
            this._headerStyle = new ExcelUtil.CustomStyleCell(excelFile);
            this.InitCellStyle(excelFile, ref this._headerStyle, HeaderRowClass);

            this._currencyOddRowStyle = new ExcelUtil.CustomStyleCell(excelFile);
            this.InitCellStyle(excelFile, ref this._currencyOddRowStyle, CurrencyOddRowClass);

            this._normalOddRowStyle = new ExcelUtil.CustomStyleCell(excelFile);
            this.InitCellStyle(excelFile, ref this._normalOddRowStyle, NormalOddRowClass);

            this._currencyEvenRowStyle = new ExcelUtil.CustomStyleCell(excelFile);
            this.InitCellStyle(excelFile, ref this._currencyEvenRowStyle, CurrencyEvenRowClass);

            this._normalEvenRowStyle = new ExcelUtil.CustomStyleCell(excelFile);
            this.InitCellStyle(excelFile, ref this._normalEvenRowStyle, NormalEvenRowClass);
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
            GetDailyTurnoverChangedDetailCriteria criteria)
        {
            ICellStyle headerStyle = this._headerStyle.TextStyle;
            var currentColumn = 0;

            for (var i = 0; i < 8; i++)
            {
                worksheet.SetColumnWidth(i, 170 * ExcelExtensions.FontWidth);
            }

            IRow row = worksheet.GetRow(currentRow) ?? worksheet.CreateRow(currentRow);
            var reportTitle = string.Format(
                SpecialResources.DailyTurnoverDetail_ReportTitle,
                criteria.FromDate.ToShortFormatedDate());

            ExcelUtil.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow + 1, currentColumn + 7, reportTitle, headerStyle);

            currentRow += 2;
            currentColumn = 0;

            worksheet.AddMergedRegion(new CellRangeAddress(currentRow, currentRow, currentColumn, currentColumn + 7));
            currentColumn = 0;

            currentRow++;
            row = worksheet.GetRow(currentRow) ?? worksheet.CreateRow(currentRow);

            ExcelUtil.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow, currentColumn, Labels.Date_Time, headerStyle);
            ExcelUtil.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow, currentColumn, Labels.Action, headerStyle);
            ExcelUtil.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow, currentColumn, Labels.User_Name, headerStyle);
            ExcelUtil.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow, currentColumn, Labels.Total_Turnover, headerStyle);
            ExcelUtil.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow, currentColumn, Labels.Super_Turnover, headerStyle);
            ExcelUtil.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow, currentColumn, Labels.Master_Turnover, headerStyle);
            ExcelUtil.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow, currentColumn, Labels.Agent_Turnover, headerStyle);
            ExcelUtil.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow, currentColumn, Labels.IBC_Turnover, headerStyle);

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
            List<DailyDetailChangedTurnover> report)
        {
            var numberEvents = report.Count;

            for (var i = 0; i < numberEvents; i++)
            {
                var eventInDate = report[i];
                var currencyClass = i % 2 == 0 ? this._currencyOddRowStyle : this._currencyEvenRowStyle;
                var normalClass = i % 2 == 0 ? this._normalOddRowStyle : this._normalEvenRowStyle;
                this.ExportReportEvent(ref worksheet, ref currentRow, eventInDate, currencyClass, normalClass);
            }
        }

        /// <summary>
        /// Export event.
        /// </summary>
        /// <param name="worksheet">The worksheet.</param>
        /// <param name="currentRow">The current row.</param>
        /// <param name="eventInDate">The event in date.</param>
        /// <param name="currencyClass">The class of currency row class.</param>
        /// <param name="normalClass">The class of normal row class.</param>
        private void ExportReportEvent(
            ref ISheet worksheet,
            ref int currentRow,
            DailyDetailChangedTurnover eventInDate,
            ExcelUtil.CustomStyleCell currencyClass,
            ExcelUtil.CustomStyleCell normalClass)
        {
            var currentColumn = 0;
            IRow row = worksheet.GetRow(currentRow) ?? worksheet.CreateRow(currentRow);

            ExcelUtil.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow + 3, currentColumn, eventInDate.Date, normalClass.TextStyle);
            ExcelUtil.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow + 3, currentColumn, eventInDate.ActionName, normalClass.TextStyle);
            ExcelUtil.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow + 3, currentColumn, eventInDate.Username, normalClass.TextStyle);

            this.RenderTurnover(
                ref worksheet,
                ref row,
                ref currentRow,
                ref currentColumn,
                eventInDate.TurnoverAmount,
                currencyClass,
                normalClass);
        }

        /// <summary>
        /// Render turnover.
        /// </summary>
        /// <param name="worksheet">The worksheet.</param>
        /// <param name="row">The row in excel file.</param>
        /// <param name="currentRow">The current row.</param>
        /// <param name="currentColumn">The current column.</param>
        /// <param name="turnoverList">The list turnover.</param>
        /// <param name="currencyClass">The class of currency row.</param>
        /// <param name="normalClass">The class of normal row.</param>
        private void RenderTurnover(
            ref ISheet worksheet,
            ref IRow row,
            ref int currentRow,
            ref int currentColumn,
            List<TurnoverInformation> turnoverList,
            ExcelUtil.CustomStyleCell currencyClass,
            ExcelUtil.CustomStyleCell normalClass)
        {
            if (turnoverList != null && turnoverList.Count >= 2)
            {
                this.RenderCurrency(
                    ref worksheet,
                    ref row,
                    ref currentRow,
                    currentColumn,
                    turnoverList[0],
                    true,
                    currencyClass,
                    normalClass);

                row = worksheet.GetRow(currentRow) ?? worksheet.CreateRow(currentRow);
                this.RenderCurrency(
                    ref worksheet,
                    ref row,
                    ref currentRow,
                    currentColumn,
                    turnoverList[1],
                    false,
                    currencyClass,
                    normalClass);
            }
        }

        /// <summary>
        /// Render currency.
        /// </summary>
        /// <param name="worksheet">The worksheet.</param>
        /// <param name="row">The row in excel file.</param>
        /// <param name="currentRow">The current row.</param>
        /// <param name="startColumn">The start column.</param>
        /// <param name="currency">The currency.</param>
        /// <param name="isVirtual">The boolean isVirtual.</param>
        /// <param name="currencyClass">The currency row class.</param>
        /// <param name="normalClass">The class of normal row.</param>
        private void RenderCurrency(
            ref ISheet worksheet,
            ref IRow row,
            ref int currentRow,
            int startColumn,
            TurnoverInformation currency,
            bool isVirtual,
            ExcelUtil.CustomStyleCell currencyClass,
            ExcelUtil.CustomStyleCell normalClass)
        {
            var currentColumn = startColumn;

            if (isVirtual)
            {
                ExcelUtil.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow, currentColumn + 4, Labels.Virtual_Currency, currencyClass.TextStyle);
            }
            else
            {
                ExcelUtil.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow, currentColumn + 4, Labels.Actual_Currency, currencyClass.TextStyle);
            }

            currentColumn = startColumn;
            currentRow++;
            row = worksheet.GetRow(currentRow) ?? worksheet.CreateRow(currentRow);

            ExcelUtil.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow, currentColumn, currency.TotalTurnover, normalClass.DecimalStyle);
            ExcelUtil.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow, currentColumn, currency.SuperTurnover, normalClass.DecimalStyle);
            ExcelUtil.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow, currentColumn, currency.MasterTurnover, normalClass.DecimalStyle);
            ExcelUtil.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow, currentColumn, currency.AgentTurnover, normalClass.DecimalStyle);
            ExcelUtil.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow, currentColumn, currency.IBCTurnover, normalClass.DecimalStyle);

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

                case CurrencyOddRowClass:
                    short cellColor = HSSFColor.White.Index;
                    CellStyleCurrencyRow(ref styleCell.TextStyle, palette, font, cellColor);
                    break;

                case CurrencyEvenRowClass:
                    palette.SetColorAtIndex(HSSFColor.Tan.Index, (byte)235, (byte)241, (byte)222);
                    cellColor = HSSFColor.Tan.Index;
                    CellStyleCurrencyRow(ref styleCell.TextStyle, palette, font, cellColor);
                    break;

                case NormalOddRowClass:
                    cellColor = HSSFColor.White.Index;
                    SetCellStyle(ref styleCell, font, dataFormat, cellColor);
                    break;

                case NormalEvenRowClass:
                    palette.SetColorAtIndex(HSSFColor.Tan.Index, (byte)235, (byte)241, (byte)222);
                    cellColor = HSSFColor.Tan.Index;
                    SetCellStyle(ref styleCell, font, dataFormat, cellColor);
                    break;

                case NormalDifferenceRowClass:
                    palette.SetColorAtIndex(HSSFColor.Teal.Index, (byte)228, (byte)242, (byte)253);
                    cellColor = HSSFColor.Teal.Index;
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
        /// <param name="cellColor">The cell color.</param>
        private void CellStyleCurrencyRow(ref ICellStyle textStyle, HSSFPalette palette, IFont font, short cellColor)
        {
            font.Boldweight = (short)FontBoldWeight.Bold;
            font.FontHeightInPoints = SpecialConstants.FontSize;
            font.FontName = SpecialConstants.FontName;

            textStyle.FillForegroundColor = cellColor;
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

        #endregion export detail report 
    }
}
