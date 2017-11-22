namespace Fanex.AccountingReports.Gaming.Services.Implement.VirtualSportWinLoss
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using ExcelUtil.Spreadsheet;
    using Fanex.AccountingReports.Core.Constants;
    using Fanex.AccountingReports.Core.Extensions;
    using Fanex.AccountingReports.Core.Services.Impl;
    using Fanex.AccountingReports.Core.Web;
    using Fanex.AccountingReports.Gaming.Constants;
    using Fanex.AccountingReports.Gaming.Criteria.VirtualSportsWinLoss;
    using Fanex.AccountingReports.Gaming.Models.VirtualSportsWinLoss;
    using Fanex.AccountingReports.Gaming.Resources;
    using Fanex.AccountingReports.Gaming.Services.Interface.VirtualSportWinLoss;
    using Fanex.Common;

    /// <summary>
    /// The get report and export report service of Virtual Sports Win Loss Detail report.
    /// </summary>
    public class VirtualSportsWinLossDetailService : ServiceBase, IVirtualSportsWinLossDetailService
    {
        private string _virtualSportsWinLossDetailPrefixSessionKey =
                "Fanex.AccountingReports.Gaming.VirtualSportsWinLossDetail.";

        private string _virtualSportsWinLossDetailSessionKey = string.Empty;

        public VirtualSportsWinLossDetailService()
        {
            if (AppSession.CurrentUser != null)
            {
                this._virtualSportsWinLossDetailSessionKey = this._virtualSportsWinLossDetailPrefixSessionKey
                                        + AppSession.CurrentUser.Username;
            }
            else
            {
                this._virtualSportsWinLossDetailSessionKey = this._virtualSportsWinLossDetailPrefixSessionKey;
            }
        }

        /// <summary>
        /// Get detail report.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>The report.</returns>
        public List<VirtualSportsWinLossDetail> GetDetailReport(
            GetVirtualSportsWinLossDetailCriteria criteria)
        {
            try
            {
                var report = new List<VirtualSportsWinLossDetail>();

                if (!criteria.IsValid())
                {
                    return report;
                }

                report = this.ConvertData(criteria);
                SessionManager.Set<List<VirtualSportsWinLossDetail>>(this._virtualSportsWinLossDetailSessionKey, report);
                return report;
            }
            catch (Exception ex)
            {
                this.HandleException(ex);
                throw;
            }
        }

        /// <summary>
        /// Export detail report.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        public void ExportDetailReport(GetVirtualSportsWinLossDetailCriteria criteria)
        {
            try
            {
                var report = SessionManager.Get<List<VirtualSportsWinLossDetail>>(
                                        this._virtualSportsWinLossDetailSessionKey);

                if (report == null)
                {
                    report = this.GetDetailReport(criteria);
                }

                ExcelFile excelFile = new ExcelFile();
                ExcelWorksheetCollection worksheets = excelFile.Worksheets;
                var currentRow = 0;

                if (report != null && report.Count > 0)
                {
                    var numberOfDates = report.Count;
                    var numberSheets = Math.Ceiling((decimal)numberOfDates / GamingConstants.MaxRowsData);

                    for (var i = 0; i < numberSheets; i++)
                    {
                        string sheetName = "Sheet " + (i + 1);
                        ExcelWorksheet worksheet = worksheets.Add(sheetName);

                        var startIndex = i * GamingConstants.MaxRowsData;
                        var endIndex = (i == (numberSheets - 1)) ? numberOfDates - 1
                                                                : (i + 1) * GamingConstants.MaxRowsData;
                        this.RenderReportHeader(ref worksheet, ref currentRow, criteria);
                        this.RenderReportContent(ref worksheet, ref currentRow, report, startIndex, endIndex);
                        this.RenderReportDetail(ref worksheet, ref currentRow, report[numberOfDates - 1], GamingConstants.ReportRowType.Total);
                    }
                }
                else
                {
                    string sheetName = GamingResources.VirtualSportsWinLossDetail_SheetName;
                    ExcelWorksheet worksheet = worksheets.Add(sheetName);
                    worksheet.ExportEmptyReport(0, 0, ref currentRow, 12);
                }

                excelFile.ReponseStream(string.Format(
                    GamingResources.VirtualSportsWinLossDetail_ReportName,
                    criteria.Date.ToShortFormatedDateWithSlash().Replace('/', '_'),
                    criteria.Currency));
            }
            catch (Exception ex)
            {
                this.HandleException(ex);
            }
        }

        /// <summary>
        /// Convert data.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>The report.</returns>
        private List<VirtualSportsWinLossDetail> ConvertData(GetVirtualSportsWinLossDetailCriteria criteria)
        {
            var report = new List<VirtualSportsWinLossDetail>();
            var rowsData = GetReportData<dynamic>(criteria);
            var currencyTotal = new VirtualSportsWinLossDetail
            {
                Name = Labels.Total
            };

            foreach (var row in rowsData)
            {
                var currency = new VirtualSportsWinLossDetail
                {
                    Name = Converter.ToString2(row.username),
                    BetCount = Converter.ToInt(row.BetCount),
                    Turnover = Converter.ToDecimal(row.TurnOver),
                    MemberWinLoss = Converter.ToDecimal(row.memberWL),
                    PositionWinLoss = Converter.ToDecimal(row.PositionWL),
                    CompanyWinLoss = Converter.ToDecimal(row.Company),
                    AmountDueInspired = Converter.ToDecimal(row.amtDuePartner)
                };

                currencyTotal.SumTotal(currency);
                report.Add(currency);
            }

            if (report.Count > 0)
            {
                report.Add(currencyTotal);
            }

            return report;
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
            GetVirtualSportsWinLossDetailCriteria criteria)
        {
            var headerStyle = ExcelExtensions.GetCellTitleStyle(ExcelExtensions.HeaderTitleColor);
            var startRow = 0;
            var startColumn = 0;
            var currentColumn = startColumn;

            for (int colIndex = 0; colIndex < 7; colIndex++)
            {
                worksheet.Columns[startColumn + colIndex].Width = 160 * ExcelExtensions.FontWidth;
            }

            var reportTitle = string.Format(
                GamingResources.VirtualSportsWinLossDetail_ReportTitle,
                criteria.Date.ToString("MMM dd, yyyy"),
                criteria.Currency);
            this.RenderCell(ref worksheet, currentRow, ref currentColumn, currentRow + 1, currentColumn + 6, reportTitle, headerStyle);
            currentRow += 2;

            worksheet.ExportEmptyRow(startRow, startColumn, ref currentRow, startColumn + 6);

            currentColumn = startColumn;

            this.RenderCell(ref worksheet, currentRow, ref currentColumn, currentRow + 1, currentColumn, Labels.UserName, headerStyle);
            this.RenderCell(ref worksheet, currentRow, ref currentColumn, currentRow + 1, currentColumn, Labels.Bet_Count, headerStyle);
            this.RenderCell(ref worksheet, currentRow, ref currentColumn, currentRow + 1, currentColumn, Labels.Turnover, headerStyle);
            this.RenderCell(ref worksheet, currentRow, ref currentColumn, currentRow + 1, currentColumn, Labels.Member_Win_Loss, headerStyle);
            this.RenderCell(ref worksheet, currentRow, ref currentColumn, currentRow, currentColumn + 1, Labels.Win_Loss, headerStyle);
            currentColumn -= 2;
            this.RenderCell(ref worksheet, currentRow + 1, ref currentColumn, currentRow + 1, currentColumn, Labels.Position, headerStyle);
            this.RenderCell(ref worksheet, currentRow + 1, ref currentColumn, currentRow + 1, currentColumn, Labels.Company, headerStyle);
            this.RenderCell(ref worksheet, currentRow, ref currentColumn, currentRow + 1, currentColumn, Labels.AmountDueToInspired, headerStyle);

            currentRow += 2;
        }

        /// <summary>
        /// Render report content.
        /// </summary>
        /// <param name="worksheet">The worksheet.</param>
        /// <param name="currentRow">The current row.</param>
        /// <param name="report">The report.</param>
        /// <param name="startIndex">The start index.</param>
        /// <param name="endIndex">The end index.</param>
        private void RenderReportContent(
            ref ExcelWorksheet worksheet,
            ref int currentRow,
            List<VirtualSportsWinLossDetail> report,
            int startIndex,
            int endIndex)
        {
            for (var i = startIndex; i < endIndex; i++)
            {
                var user = report[i];
                var rowCssClass =
                    i % 2 == 0
                    ? GamingConstants.ReportRowType.EvenRow
                    : GamingConstants.ReportRowType.OddRow;

                this.RenderReportDetail(
                    ref worksheet,
                    ref currentRow,
                    user,
                    rowCssClass);
            }
        }

        /// <summary>
        /// Render report detail.
        /// </summary>
        /// <param name="worksheet">The worksheet.</param>
        /// <param name="currentRow">The current row.</param>
        /// <param name="user">The data of row.</param>
        /// <param name="rowCssClass">The class of row.</param>
        private void RenderReportDetail(
            ref ExcelWorksheet worksheet,
            ref int currentRow,
            VirtualSportsWinLossDetail user,
            GamingConstants.ReportRowType rowCssClass)
        {
            CellStyle textStyle;
            CellStyle intStyle;
            CellStyle decimalStyle;

            this.InitCellStyle(rowCssClass, out textStyle, out intStyle, out decimalStyle);

            var startColumn = 0;
            var currentColumn = startColumn;

            this.RenderCell(ref worksheet, currentRow, ref currentColumn, currentRow, currentColumn, user.Name, textStyle);
            this.RenderCell(ref worksheet, currentRow, ref currentColumn, currentRow, currentColumn, user.BetCount, intStyle);
            this.RenderCell(ref worksheet, currentRow, ref currentColumn, currentRow, currentColumn, user.Turnover, decimalStyle);
            this.RenderCell(ref worksheet, currentRow, ref currentColumn, currentRow, currentColumn, user.MemberWinLoss, decimalStyle);
            this.RenderCell(ref worksheet, currentRow, ref currentColumn, currentRow, currentColumn, user.PositionWinLoss, decimalStyle);
            this.RenderCell(ref worksheet, currentRow, ref currentColumn, currentRow, currentColumn, user.CompanyWinLoss, decimalStyle);
            this.RenderCell(ref worksheet, currentRow, ref currentColumn, currentRow, currentColumn, user.AmountDueInspired, decimalStyle);

            currentRow++;
        }

        /// <summary>
        /// Initial Cell Style.
        /// </summary>
        /// <param name="rowCssClass">The type of row.</param>
        /// <param name="textStyle">The text style.</param>
        /// <param name="intStyle">The integer style.</param>
        /// <param name="decimalStyle">The decimal style.</param>
        private void InitCellStyle(
            GamingConstants.ReportRowType rowCssClass,
            out CellStyle textStyle,
            out CellStyle intStyle,
            out CellStyle decimalStyle)
        {
            textStyle = new CellStyle();
            intStyle = new CellStyle();
            decimalStyle = new CellStyle();

            switch (rowCssClass)
            {
                case GamingConstants.ReportRowType.OddRow:
                    textStyle = ExcelExtensions.GetCellTextStyle(ExcelExtensions.BackGroundColor, false);
                    textStyle.HorizontalAlignment = HorizontalAlignmentStyle.Center;
                    intStyle = ExcelExtensions.GetCellNumberStyle(string.Empty, ExcelExtensions.BackGroundColor, false, false, false);
                    intStyle.HorizontalAlignment = HorizontalAlignmentStyle.Right;
                    decimalStyle = ExcelExtensions.GetCellNumberStyle(NumberFormatConstants.TwoDecimalDigitFormat, ExcelExtensions.BackGroundColor, false, false, false);
                    decimalStyle.HorizontalAlignment = HorizontalAlignmentStyle.Right;
                    break;

                case GamingConstants.ReportRowType.EvenRow:
                    textStyle = ExcelExtensions.GetCellTextStyle(ExcelExtensions.EvenRowColor, false);
                    textStyle.HorizontalAlignment = HorizontalAlignmentStyle.Center;
                    intStyle = ExcelExtensions.GetCellNumberStyle(string.Empty, ExcelExtensions.EvenRowColor, false, false, false);
                    intStyle.HorizontalAlignment = HorizontalAlignmentStyle.Right;
                    decimalStyle = ExcelExtensions.GetCellNumberStyle(NumberFormatConstants.TwoDecimalDigitFormat, ExcelExtensions.EvenRowColor, false, false, false);
                    decimalStyle.HorizontalAlignment = HorizontalAlignmentStyle.Right;
                    break;
                case GamingConstants.ReportRowType.Total:
                    textStyle = ExcelExtensions.GetCellTextStyle(ExcelExtensions.TotalRowColor, false);
                    textStyle.HorizontalAlignment = HorizontalAlignmentStyle.Center;
                    textStyle.Borders.SetBorders(MultipleBorders.Top, Color.FromArgb(255, 102, 0), LineStyle.Medium);
                    intStyle = ExcelExtensions.GetCellNumberStyle(string.Empty, ExcelExtensions.TotalRowColor, false, false, false);
                    intStyle.HorizontalAlignment = HorizontalAlignmentStyle.Right;
                    intStyle.Borders.SetBorders(MultipleBorders.Top, Color.FromArgb(255, 102, 0), LineStyle.Medium);
                    decimalStyle = ExcelExtensions.GetCellNumberStyle(NumberFormatConstants.TwoDecimalDigitFormat, ExcelExtensions.TotalRowColor, false, false, false);
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
    }
}
