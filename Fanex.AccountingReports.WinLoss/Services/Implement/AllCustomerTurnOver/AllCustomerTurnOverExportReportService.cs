// -----------------------------------------------------------------------
// <copyright file="AllCustomerTurnOverExportReportService.cs" company="Nexcel Solutions Vietnam">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Fanex.AccountingReports.WinLoss.Services.Implement.AllCustomerTurnOver
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;
    using System.Text;
    using ExcelUtil.Spreadsheet;
    using Fanex.AccountingReports.Core.Constants;
    using Fanex.AccountingReports.Core.Extensions;
    using Fanex.AccountingReports.Core.Services.Impl;
    using Fanex.AccountingReports.Core.Web;
    using Fanex.AccountingReports.WinLoss.Constants;
    using Fanex.AccountingReports.WinLoss.Criteria.GetAllCustomerTurnOver;
    using Fanex.AccountingReports.WinLoss.Models.AllCustomerTurnOver;
    using Fanex.AccountingReports.WinLoss.Resources;
    using Fanex.AccountingReports.WinLoss.Services.Interface;
    using Fanex.AccountingReports.WinLoss.Services.Interface.AllCustomerTurnOver;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class AllCustomerTurnOverExportReportService : ServiceBase, IAllCustomerTurnOverExportReportService
    {
        private const string GrandTotal = "GRAND TOTAL";

        private const string AllCustomerSessionKeyConstant = "Fanex.AccountingReports.WinLoss.AllCustomerTurnOver.";

        private const string Internet = "Internet";

        private const string Phone = "Phone";

        private const string InternetAndPhone = "Internet + Phone";

        private const int MaxRowsData = 65000;

        private AllCustomerTurnOverGetReportService _getReportService;

        private string _allCustomerTurnOverSessionKey;

        public AllCustomerTurnOverExportReportService()
        {
            this._getReportService = new AllCustomerTurnOverGetReportService();
            this._allCustomerTurnOverSessionKey = AllCustomerSessionKeyConstant + AppSession.CurrentUser.Username;
        }

        /// <summary>
        /// Export excel.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        public void ExportReport(GetAllCustomerTurnOverCriteria criteria)
        {
            try
            {
                var report = SessionManager.Get<CustomerTurnoverBySite>(this._allCustomerTurnOverSessionKey);
                if (report == null)
                {
                    report = this._getReportService.GetReport(criteria);
                }

                ExcelFile excelFile = new ExcelFile();
                ExcelWorksheetCollection worksheets = excelFile.Worksheets;

                int startRow = 0;
                int startColumn = 0;
                int currentRow = startRow;
                int currentIndex = 0;
                var numberOfSites = report.Site.Rows.Count;

                if (report.Site != null &&
                    report.Site.Rows != null && numberOfSites > 0)
                {
                    for (int i = 0; i < numberOfSites; i++)
                    {
                        var siteInfomation = report.Site.Rows[i];
                        this.ExportReportSite(ref worksheets, startRow, startColumn, ref currentRow, criteria, report, siteInfomation, ref currentIndex);
                    }

                    ExcelWorksheet worksheet = worksheets[worksheets.Count - 1];
                    this.ExportRow(ref worksheet, startRow, startColumn, ref currentRow, report.Site, null, WinLossContants.ReportRowType.GrandTotal, 0);
                }
                else
                {
                    string sheetName = WinLossResources.AllCustomerTurnOver_SheetName;
                    ExcelWorksheet worksheet = worksheets.Add(sheetName);
                    worksheet.ExportEmptyReport(startRow, startColumn, ref currentRow, startColumn + 2);
                }

                excelFile.ReponseStream(string.Format(
                    WinLossResources.AllCustomerTurnOver_ReportName,
                    criteria.FromDate.ToShortFormatedDateWithSlash().Replace('/', '_'),
                    criteria.ToDate.ToShortFormatedDateWithSlash().Replace('/', '_')));
            }
            catch (Exception ex)
            {
                this.HandleException(ex);
            }
        }

        /// <summary>
        /// Export report site.
        /// </summary>
        /// <param name="worksheets">The worksheets.</param>
        /// <param name="startRow">The start row.</param>
        /// <param name="startColumn">The start column.</param>
        /// <param name="currentRow">The current row.</param>
        /// <param name="criteria">The criteria.</param>
        /// <param name="report">The report.</param>
        /// <param name="siteInformation">The information of site.</param>
        /// <param name="currentIndex">The current index.</param>
        private void ExportReportSite(
            ref ExcelWorksheetCollection worksheets,
            int startRow,
            int startColumn,
            ref int currentRow,
            GetAllCustomerTurnOverCriteria criteria,
            CustomerTurnoverBySite report,
            CustomerTurnoverBySiteDetail siteInformation,
            ref int currentIndex)
        {
            try
            {
                if (siteInformation != null
                        && siteInformation.Rows != null
                        && siteInformation.Rows.Count > 0)
                {
                    ExcelWorksheet worksheet = worksheets.Count > 0 ? worksheets[worksheets.Count - 1] : null;
                    if (worksheet != null)
                    {
                        this.ExportRow(ref worksheet, startRow, startColumn, ref currentRow, siteInformation, siteInformation.Name, WinLossContants.ReportRowType.HeaderSite, 0);
                    }

                    var numberOfUsersInSite = siteInformation.Rows.Count;

                    for (int i = 0; i < numberOfUsersInSite; i++)
                    {
                        if (currentIndex % MaxRowsData == 0)
                        {
                            if (currentIndex != 0)
                            {
                                this.ExportRow(ref worksheet, startRow, startColumn, ref currentRow, report.Site, null, WinLossContants.ReportRowType.GrandTotal, 0);
                            }

                            string sheetName = "Sheet " + ((currentIndex / MaxRowsData) + 1);
                            worksheet = worksheets.Add(sheetName);

                            startRow = 0;
                            startColumn = 0;
                            currentRow = startRow;

                            this.ExportReportHeader(ref worksheet, startRow, startColumn, ref currentRow, criteria, report.Site);
                            this.ExportRow(ref worksheet, startRow, startColumn, ref currentRow, siteInformation, siteInformation.Name, WinLossContants.ReportRowType.HeaderSite, 0);
                        }

                        var userInSite = siteInformation.Rows[i];
                        currentIndex++;
                        var rowType = i % 2 == 0 ? WinLossContants.ReportRowType.EvenRow : WinLossContants.ReportRowType.OddRow;
                        this.ExportRow(ref worksheet, startRow, startColumn, ref currentRow, userInSite, null, rowType, currentIndex);
                    }

                    this.ExportRow(ref worksheet, startRow, startColumn, ref currentRow, siteInformation, siteInformation.Name, WinLossContants.ReportRowType.Total, 0);
                }
            }
            catch (Exception ex)
            {
                this.HandleException(ex);
            }
        }

        /// <summary>
        /// Export report header.
        /// </summary>
        /// <param name="worksheet">The worksheet.</param>
        /// <param name="startRow">The start row.</param>
        /// <param name="startColumn">The start column.</param>
        /// <param name="currentRow">The current row.</param>
        /// <param name="criteria">The criteria.</param>
        /// <param name="report">The report.</param>
        private void ExportReportHeader(
            ref ExcelWorksheet worksheet,
            int startRow,
            int startColumn,
            ref int currentRow,
            GetAllCustomerTurnOverCriteria criteria,
            CustomerTurnoverBySiteDetail report)
        {
            var titleColor = ExcelExtensions.HeaderTitleColor;
            var headerColor = ExcelExtensions.HeaderTitleColor;
            CellRange currentCellRange;
            CellStyle headerStyle = ExcelExtensions.GetCellTextStyle(headerColor, false);
            headerStyle.HorizontalAlignment = HorizontalAlignmentStyle.Center;
            var currentColumn = startColumn;

            var fromText = this.GetFromText(criteria.From);

            worksheet.Columns[startColumn].Width = 80 * ExcelExtensions.FontWidth;
            worksheet.Columns[startColumn + 1].Width = 250 * ExcelExtensions.FontWidth;
            worksheet.Columns[startColumn + 2].Width = 140 * ExcelExtensions.FontWidth;

            var messageTitle = string.Format(WinLossResources.AllCustomerTurnOver_ReportMessageTitle, criteria.Site, fromText);

            var title = string.Format(
                WinLossResources.AllCustomerTurnOver_ReportTitle,
                criteria.FromDate.ToShortFormatedDate(),
                criteria.ToDate.ToShortFormatedDate());

            currentColumn = startColumn;
            currentCellRange = worksheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow, currentColumn + 2);
            currentCellRange.Merged = true;
            currentCellRange.Value = messageTitle;
            currentRow++;

            worksheet.ExportEmptyRow(startRow, startColumn, ref currentRow, startColumn + 2);

            currentColumn = startColumn;
            currentCellRange = worksheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow + 1, currentColumn + 2);
            currentCellRange.Merged = true;
            currentCellRange.Value = title;
            currentCellRange.Style.FormatTitleCell(titleColor);
            currentRow = currentRow + 2;

            worksheet.ExportEmptyRow(startRow, startColumn, ref currentRow, startColumn + 2);

            currentColumn = startColumn;
            this.ExportCell(ref worksheet, currentRow, headerStyle, Labels.No, currentCellRange, ref currentColumn, currentColumn);

            this.ExportCell(ref worksheet, currentRow, headerStyle, Labels.User_Name, currentCellRange, ref currentColumn, currentColumn);

            this.ExportCell(ref worksheet, currentRow, headerStyle, Labels.Turnover, currentCellRange, ref currentColumn, currentColumn);

            currentRow++;
        }

        /// <summary>
        /// Export row.
        /// </summary>
        /// <param name="worksheet">The worksheet.</param>
        /// <param name="startRow">The start row.</param>
        /// <param name="startColumn">The start column.</param>
        /// <param name="currentRow">The current row.</param>        
        /// <param name="siteInformation">The information of site.</param>
        /// <param name="nameSite">The name of site.</param>
        /// <param name="reportRowType">The type of row.</param>
        /// <param name="currentIndex">The current index.</param>
        private void ExportRow(
            ref ExcelWorksheet worksheet,
            int startRow,
            int startColumn,
            ref int currentRow,
            CustomerTurnoverBySiteDetail siteInformation,
            string nameSite,
            WinLossContants.ReportRowType reportRowType,
            int currentIndex)
        {
            var textStyle = ExcelExtensions.GetCellTextStyle(ExcelExtensions.BackGroundColor, false);
            var intStyle = ExcelExtensions.GetCellNumberStyle(string.Empty, ExcelExtensions.BackGroundColor, false, false, false);
            var decimalStyle = ExcelExtensions.GetCellNumberStyle(string.Empty, ExcelExtensions.BackGroundColor, false, false, false);

            this.InitRowStyle(reportRowType, ref textStyle, ref intStyle, ref decimalStyle);

            CellRange currentCellRange = null;
            int currentColumn = startColumn;

            if (WinLossContants.ReportRowType.GrandTotal == reportRowType)
            {
                this.ExportCell(ref worksheet, currentRow, textStyle, string.Empty, currentCellRange, ref currentColumn, currentColumn);
                this.ExportCell(ref worksheet, currentRow, textStyle, GrandTotal, currentCellRange, ref currentColumn, currentColumn);
                this.ExportCell(ref worksheet, currentRow, decimalStyle, siteInformation.Turnover, currentCellRange, ref currentColumn, currentColumn);
            }
            else if (WinLossContants.ReportRowType.Total == reportRowType)
            {
                var labelSite = string.Format(WinLossResources.AllCustomerTurnOver_SiteTotal, nameSite);
                this.ExportCell(ref worksheet, currentRow, textStyle, string.Empty, currentCellRange, ref currentColumn, currentColumn);
                this.ExportCell(ref worksheet, currentRow, textStyle, labelSite, currentCellRange, ref currentColumn, currentColumn);
                this.ExportCell(ref worksheet, currentRow, decimalStyle, siteInformation.Turnover, currentCellRange, ref currentColumn, currentColumn);
            }
            else if (WinLossContants.ReportRowType.HeaderSite == reportRowType)
            {
                this.ExportCell(ref worksheet, currentRow, textStyle, nameSite, currentCellRange, ref currentColumn, currentColumn + 2);
            }
            else
            {
                this.ExportCell(ref worksheet, currentRow, intStyle, currentIndex, currentCellRange, ref currentColumn, currentColumn);
                this.ExportCell(ref worksheet, currentRow, textStyle, siteInformation.Name, currentCellRange, ref currentColumn, currentColumn);
                this.ExportCell(ref worksheet, currentRow, decimalStyle, siteInformation.Turnover, currentCellRange, ref currentColumn, currentColumn);
            }

            currentRow++;
        }

        /// <summary>
        /// Export cell.
        /// </summary>
        /// <param name="worksheet">The worksheet.</param>
        /// <param name="currentRow">The current row.</param>
        /// <param name="style">The style.</param>
        /// <param name="label">The label.</param>
        /// <param name="currentCellRange">The current cell range.</param>
        /// <param name="currentColumn">The current column.</param>
        /// <param name="lastColumn">The last column.</param>
        private void ExportCell(
            ref ExcelWorksheet worksheet,
            int currentRow,
            CellStyle style,
            object label,
            CellRange currentCellRange,
            ref int currentColumn,
            int lastColumn)
        {
            currentCellRange = worksheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow, lastColumn);
            currentCellRange.Merged = true;
            currentCellRange.Value = label;
            currentCellRange.Style = style;
            currentColumn++;
        }

        /// <summary>
        /// Initial Row Style.
        /// </summary>
        /// <param name="reportRowType">The report row type.</param>
        /// <param name="textStyle">The text style.</param>
        /// <param name="intStyle">The integer style.</param>
        /// <param name="decimalStyle">The decimal style.</param>
        private void InitRowStyle(
            WinLossContants.ReportRowType reportRowType,
            ref CellStyle textStyle,
            ref CellStyle intStyle,
            ref CellStyle decimalStyle)
        {
            Color totalRowColor = Color.FromArgb(211, 225, 200);

            switch (reportRowType)
            {
                case WinLossContants.ReportRowType.OddRow:
                    textStyle = ExcelExtensions.GetCellTextStyle(ExcelExtensions.BackGroundColor, false);
                    intStyle = ExcelExtensions.GetCellNumberStyle(string.Empty, ExcelExtensions.BackGroundColor, false, false, false);
                    decimalStyle = ExcelExtensions.GetCellNumberStyle(NumberFormatConstants.TwoDecimalDigitFormat, ExcelExtensions.BackGroundColor, false, false, false);
                    intStyle.HorizontalAlignment = HorizontalAlignmentStyle.Center;
                    textStyle.HorizontalAlignment = HorizontalAlignmentStyle.Center;
                    decimalStyle.HorizontalAlignment = HorizontalAlignmentStyle.Right;
                    break;

                case WinLossContants.ReportRowType.EvenRow:
                    textStyle = ExcelExtensions.GetCellTextStyle(ExcelExtensions.EvenRowColor, false);
                    intStyle = ExcelExtensions.GetCellNumberStyle(string.Empty, ExcelExtensions.EvenRowColor, false, false, false);
                    decimalStyle = ExcelExtensions.GetCellNumberStyle(NumberFormatConstants.TwoDecimalDigitFormat, ExcelExtensions.EvenRowColor, false, false, false);
                    textStyle.HorizontalAlignment = HorizontalAlignmentStyle.Center;
                    intStyle.HorizontalAlignment = HorizontalAlignmentStyle.Center;
                    decimalStyle.HorizontalAlignment = HorizontalAlignmentStyle.Right;
                    break;

                case WinLossContants.ReportRowType.GrandTotal:
                    textStyle = ExcelExtensions.GetCellTextStyle(ExcelExtensions.GrandTotalRowColor, true);
                    textStyle.HorizontalAlignment = HorizontalAlignmentStyle.Center;
                    textStyle.Borders.SetBorders(MultipleBorders.Top, Color.FromArgb(255, 102, 0), LineStyle.Medium);
                    decimalStyle = ExcelExtensions.GetCellNumberStyle(NumberFormatConstants.TwoDecimalDigitFormat, ExcelExtensions.GrandTotalRowColor, true, false, false);
                    decimalStyle.HorizontalAlignment = HorizontalAlignmentStyle.Right;
                    decimalStyle.Borders.SetBorders(MultipleBorders.Top, Color.FromArgb(255, 102, 0), LineStyle.Medium);
                    break;

                case WinLossContants.ReportRowType.Total:
                    textStyle = ExcelExtensions.GetCellTextStyle(totalRowColor, true);
                    textStyle.HorizontalAlignment = HorizontalAlignmentStyle.Center;
                    decimalStyle = ExcelExtensions.GetCellNumberStyle(NumberFormatConstants.TwoDecimalDigitFormat, totalRowColor, true, false, false);
                    decimalStyle.HorizontalAlignment = HorizontalAlignmentStyle.Right;
                    break;

                case WinLossContants.ReportRowType.HeaderSite:
                    textStyle = ExcelExtensions.GetCellTextStyle(ExcelExtensions.SubTotalRowColor, true);
                    textStyle.HorizontalAlignment = HorizontalAlignmentStyle.Center;
                    break;
            }
        }

        /// <summary>
        /// Get from text to display.
        /// </summary>
        /// <param name="from">The from information.</param>
        /// <returns>The text will be displayed.</returns>
        private string GetFromText(string from)
        {
            string fromText = string.Empty;
            if (from.Equals("p"))
            {
                fromText = Phone;
            }
            else if (from.Equals("i"))
            {
                fromText = Internet;
            }
            else
            {
                fromText = InternetAndPhone;
            }

            return fromText;
        }
    }
}
