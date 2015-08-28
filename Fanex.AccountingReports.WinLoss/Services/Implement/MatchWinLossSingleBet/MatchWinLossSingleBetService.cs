// -----------------------------------------------------------------------
// <copyright file="MatchWinLossSingleBetService.cs" company="Nexcel Solutions Vietnam">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Fanex.AccountingReports.WinLoss.Services.Implement.MatchWinLossSingleBet
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using ExcelUtil.Spreadsheet;
    using Fanex.AccountingReports.Core.Constants;
    using Fanex.AccountingReports.Core.Extensions;
    using Fanex.AccountingReports.Core.Services.Impl;
    using Fanex.AccountingReports.Core.Web;
    using Fanex.AccountingReports.WinLoss.Constants;
    using Fanex.AccountingReports.WinLoss.Criteria.MatchWinLossSingleBet;
    using Fanex.AccountingReports.WinLoss.Models.MatchWinLossSingleBet;
    using Fanex.AccountingReports.WinLoss.Resources;
    using Fanex.AccountingReports.WinLoss.Services.Interface.MatchWinLossSingleBet;
    using Fanex.Common;

    /// <summary>
    /// The class contains two service : get report and export report.
    /// </summary>
    public class MatchWinLossSingleBetService : ServiceBase, IMatchWinLossSingleBetService
    {
        private const string Versus = "-vs-";

        private string _matchWinLossSingleBetSessionKey = "MatchWinLossSingleBet.";
        private int _maxRowsData = 65000;

        public MatchWinLossSingleBetService()
        {
            this._matchWinLossSingleBetSessionKey += AppSession.CurrentUser != null ? AppSession.CurrentUser.Username : string.Empty;
        }

        /// <summary>
        /// Get report.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>The report.</returns>
        public MatchWinLossSingleBet GetReport(GetMatchWinLossSingleBetCriteria criteria)
        {
            try
            {
                var report = new MatchWinLossSingleBet
                {
                    Rows = new List<MatchWinLossSingleBet>()
                };

                if (!criteria.IsValid())
                {
                    return report;
                }

                var rowsData = GetReportData<dynamic>(criteria);

                foreach (var row in rowsData)
                {
                    var homeID = Converter.ToInt(row.homeid);
                    var awayID = Converter.ToInt(row.awayid);
                    var match = new MatchWinLossSingleBet
                    {
                        HomeName = CacheRef.GetTeamName(homeID),
                        AwayName = CacheRef.GetTeamName(awayID),
                        MatchWinLost = Converter.ToDecimal(row.matchwinlost)
                    };

                    report.Rows.Add(match);
                    report.MatchWinLost += match.MatchWinLost;
                }

                SessionManager.Set<MatchWinLossSingleBet>(this._matchWinLossSingleBetSessionKey, report);
                return report;
            }
            catch (Exception ex)
            {
                this.HandleException(ex);
                throw;
            }
        }

        #region Export Report.

        /// <summary>
        /// Export Report.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        public void ExportReport(GetMatchWinLossSingleBetCriteria criteria)
        {
            try
            {
                var report = SessionManager.Get<MatchWinLossSingleBet>(this._matchWinLossSingleBetSessionKey);

                if (report == null)
                {
                    report = this.GetReport(criteria);
                }

                ExcelFile excelFile = new ExcelFile();
                ExcelWorksheetCollection worksheets = excelFile.Worksheets;
                var currentRow = 0;
                var currentIndex = 1;

                if (report.Rows != null && report.Rows.Count > 0)
                {
                    var numberOfMatchs = report.Rows.Count;
                    var numberSheets = Math.Ceiling((decimal)numberOfMatchs / _maxRowsData);

                    for (var i = 0; i < numberSheets; i++)
                    {
                        string sheetName = "Sheet " + (i + 1);
                        ExcelWorksheet worksheet = worksheets.Add(sheetName);
                        var startIndex = i * _maxRowsData;
                        var endIndex = (i == (numberSheets - 1)) 
                                        ? numberOfMatchs 
                                        : (i + 1) * _maxRowsData;

                        this.RenderReportHeader(ref worksheet, ref currentRow, criteria);
                        this.RenderReportContent(ref worksheet, ref currentRow, report, startIndex, endIndex, ref currentIndex);
                    }
                }
                else
                {
                    string sheetName = WinLossResources.MatchWinLossSingleBet_SheetName;
                    ExcelWorksheet worksheet = worksheets.Add(sheetName);
                    worksheet.ExportEmptyReport(0, 0, ref currentRow, 5);
                }

                excelFile.ReponseStream(string.Format(
                    WinLossResources.MatchWinLossSingleBet_ReportName,
                    criteria.FromDate.ToShortFormatedDateWithSlash().Replace('/', '_'),
                    criteria.ToDate.ToShortFormatedDateWithSlash().Replace('/', '_')));
            }
            catch (Exception ex)
            {
                this.HandleException(ex);
            }
        }

        /// <summary>
        /// Render report header.
        /// </summary>
        /// <param name="worksheet">The worksheet.</param>
        /// <param name="currentRow">The current row.</param>
        /// <param name="criteria">The criteria.</param>
        private void RenderReportHeader(
            ref ExcelWorksheet worksheet, 
            ref int currentRow, 
            GetMatchWinLossSingleBetCriteria criteria)
        {
            var headerStyle = ExcelExtensions.GetCellTitleStyle(ExcelExtensions.HeaderTitleColor);
            var startRow = 0;
            var startColumn = 0;
            var currentColumn = startColumn;

            worksheet.Columns[0].Width = 60 * ExcelExtensions.FontWidth;
            worksheet.Columns[1].Width = 350 * ExcelExtensions.FontWidth;
            worksheet.Columns[2].Width = 40 * ExcelExtensions.FontWidth;
            worksheet.Columns[3].Width = 350 * ExcelExtensions.FontWidth;
            worksheet.Columns[4].Width = 140 * ExcelExtensions.FontWidth;

            var reportTitle = string.Format(
                    WinLossResources.MatchWinLossSingleBet_ReportTitle,
                    criteria.FromDate,
                    criteria.ToDate);
            this.RenderCell(ref worksheet, currentRow, ref currentColumn, currentRow + 1, currentColumn + 4, reportTitle, headerStyle);
            currentRow += 2;

            worksheet.ExportEmptyRow(startRow, startColumn, ref currentRow, startColumn + 4);

            currentColumn = startColumn;

            this.RenderCell(ref worksheet, currentRow, ref currentColumn, currentRow, currentColumn + 3, Labels.Match, headerStyle);
            this.RenderCell(ref worksheet, currentRow, ref currentColumn, currentRow, currentColumn, Labels.Base_Currency, headerStyle);

            currentRow++;
        }

        /// <summary>
        /// Render report content.
        /// </summary>
        /// <param name="worksheet">The worksheet.</param>
        /// <param name="currentRow">The current row.</param>
        /// <param name="report">The report.</param>
        /// <param name="startIndex">The start index.</param>
        /// <param name="endIndex">The end index.</param>
        /// <param name="currentIndex">The current index.</param>
        private void RenderReportContent(
            ref ExcelWorksheet worksheet, 
            ref int currentRow, 
            MatchWinLossSingleBet report, 
            int startIndex, 
            int endIndex,
            ref int currentIndex)
        {
            for (var i = startIndex; i < endIndex; i++)
            {
                var match = report.Rows[i];
                var rowCssClass = 
                    (i + 1) % 2 == 0
                    ? WinLossContants.ReportRowType.EvenRow
                    : WinLossContants.ReportRowType.OddRow;

                this.RenderReportDetail(
                    ref worksheet,
                    ref currentRow,
                    match,
                    rowCssClass,
                    currentIndex);
                currentIndex++;
            }

            this.RenderReportDetail(
                ref worksheet,
                ref currentRow,
                report,
                WinLossContants.ReportRowType.GrandTotal,
                currentIndex);
        }

        /// <summary>
        /// Render report detail.
        /// </summary>
        /// <param name="worksheet">The worksheet.</param>
        /// <param name="currentRow">The current row.</param>
        /// <param name="match">The data of row.</param>
        /// <param name="rowCssClass">The class of row.</param>
        /// <param name="currentIndex">The current index.</param>
        private void RenderReportDetail(
            ref ExcelWorksheet worksheet,
            ref int currentRow,
            MatchWinLossSingleBet match,
            WinLossContants.ReportRowType rowCssClass,
            int currentIndex)
        {
            CellStyle textStyle;
            CellStyle intStyle;
            CellStyle decimalStyle;
            this.InitCellStyle(rowCssClass, out textStyle, out intStyle, out decimalStyle);

            var currentColumn = 0;

            if (WinLossContants.ReportRowType.GrandTotal == rowCssClass)
            {
                this.RenderCell(ref worksheet, currentRow, ref currentColumn, currentRow, currentColumn + 3, Labels.Grand_Total, textStyle);
                this.RenderCell(ref worksheet, currentRow, ref currentColumn, currentRow, currentColumn, match.MatchWinLost, decimalStyle);
            }
            else
            {
                this.RenderCell(ref worksheet, currentRow, ref currentColumn, currentRow, currentColumn, currentIndex, intStyle);
                this.RenderCell(ref worksheet, currentRow, ref currentColumn, currentRow, currentColumn, match.HomeName, textStyle);
                this.RenderCell(ref worksheet, currentRow, ref currentColumn, currentRow, currentColumn, Versus, textStyle);
                this.RenderCell(ref worksheet, currentRow, ref currentColumn, currentRow, currentColumn, match.AwayName, textStyle);
                this.RenderCell(ref worksheet, currentRow, ref currentColumn, currentRow, currentColumn, match.MatchWinLost, decimalStyle);
            }

            currentRow++;
        }

        /// <summary>
        /// Initial cell style.
        /// </summary>
        /// <param name="rowCssClass">The class of row.</param>
        /// <param name="textStyle">The style of row text.</param>
        /// <param name="intStyle">The style of row integer.</param>
        /// <param name="decimalStyle">The style of row decimal.</param>
        private void InitCellStyle(
            WinLossContants.ReportRowType rowCssClass, 
            out CellStyle textStyle, 
            out CellStyle intStyle, 
            out CellStyle decimalStyle)
        {
            textStyle = new CellStyle();
            intStyle = new CellStyle();
            decimalStyle = new CellStyle();

            switch (rowCssClass)
            {
                case WinLossContants.ReportRowType.OddRow:
                    textStyle = ExcelExtensions.GetCellTextStyle(ExcelExtensions.BackGroundColor, false);
                    textStyle.HorizontalAlignment = HorizontalAlignmentStyle.Center;

                    intStyle = ExcelExtensions.GetCellNumberStyle(string.Empty, ExcelExtensions.BackGroundColor, false, false, false);
                    intStyle.HorizontalAlignment = HorizontalAlignmentStyle.Center;

                    decimalStyle = ExcelExtensions.GetCellNumberStyle(NumberFormatConstants.TwoDecimalDigitFormat, ExcelExtensions.BackGroundColor, false, false, false);
                    decimalStyle.HorizontalAlignment = HorizontalAlignmentStyle.Right;
                    break;

                case WinLossContants.ReportRowType.EvenRow:
                    textStyle = ExcelExtensions.GetCellTextStyle(ExcelExtensions.EvenRowColor, false);
                    textStyle.HorizontalAlignment = HorizontalAlignmentStyle.Center;

                    intStyle = ExcelExtensions.GetCellNumberStyle(string.Empty, ExcelExtensions.EvenRowColor, false, false, false);
                    intStyle.HorizontalAlignment = HorizontalAlignmentStyle.Center;

                    decimalStyle = ExcelExtensions.GetCellNumberStyle(NumberFormatConstants.TwoDecimalDigitFormat, ExcelExtensions.EvenRowColor, false, false, false);
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
            }
        }

        /// <summary>
        /// Render cell.
        /// </summary>
        /// <param name="worksheet">The worksheet.</param>
        /// <param name="currentRow">The current row.</param>
        /// <param name="currentColumn">The current column.</param>
        /// <param name="endRow">The next row.</param>
        /// <param name="endColumn">The next column.</param>
        /// <param name="cellValue">The label of cell.</param>
        /// <param name="cellStyle">The style of cell.</param>
        private void RenderCell(
            ref ExcelWorksheet worksheet,
            int currentRow,
            ref int currentColumn,
            int endRow,
            int endColumn,
            object cellValue,
            CellStyle cellStyle)
        {
            CellRange currentCellRange = worksheet.Cells.GetSubrangeAbsolute(
                currentRow, 
                currentColumn, 
                endRow, 
                endColumn);
            currentCellRange.Merged = true;
            currentCellRange.Value = cellValue;
            currentCellRange.Style = cellStyle;
            currentColumn = endColumn + 1;
        }

        #endregion Export Report.
    }
}
