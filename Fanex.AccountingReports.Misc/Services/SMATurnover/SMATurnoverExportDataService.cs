namespace Fanex.AccountingReports.Misc.Services.Implement.SMATurnover
{
    using System.Diagnostics.CodeAnalysis;
    using Fanex.AccountingReports.Core.Extensions;
    using Fanex.AccountingReports.Core.Web;
    using Fanex.AccountingReports.Misc.Constants;
    using Fanex.AccountingReports.Misc.Criteria.SMATurnover;
    using Fanex.AccountingReports.Misc.Models.SMATurnover;
    using Fanex.AccountingReports.Misc.Resources;
    using NPOI.HSSF.UserModel;
    using NPOI.HSSF.Util;
    using NPOI.SS.UserModel;

    /// <summary>
    /// Export data service.
    /// </summary>
    public partial class SMATurnoverService
    {
        private const short SubTotalColorIndex = 49;
        private const short GrandTotalColorIndex = 50;
        private const short GrandTotalBorderColorIndex = 51;

        /// <summary>
        /// Exports the report data.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        public void ExportReportData(GetSMATurnoverReportCriteria criteria)
        {
            var report = SessionManager.Get<SMATurnover>(_smasTurnoverSessionKey);

            if (report == null)
            {
                report = this.GetSMAsTurnover(criteria);
            }

            HSSFWorkbook excelFile = new HSSFWorkbook();
            this.SetColorIndex(excelFile);
            string sheetName = MiscResources.SMA_TurnOver_SheetName;
            ISheet worksheet = excelFile.CreateSheet(sheetName);
            int currentRow = 0;
            this.InitColumn(ref worksheet);
            this.ExportReportTitle(excelFile, ref worksheet, ref currentRow, criteria);
            currentRow++;
            this.ExportReportHeader(excelFile, ref worksheet, ref currentRow);
            currentRow = 3;
            this.ExportReportContent(excelFile, ref worksheet, ref currentRow, report);
            this.ExportGrandTotal(excelFile, ref worksheet, ref currentRow, report.Total);
            string fileName = this.GetReportName(criteria);
            NpoiExcelExtensions.ResponseStreamExcel(excelFile, fileName);
        }

        private void SetColorIndex(HSSFWorkbook excelFile)
        {
            HSSFPalette palette = excelFile.GetCustomPalette();
            palette.SetColorAtIndex(SubTotalColorIndex, (byte)244, (byte)244, (byte)190);
            palette.SetColorAtIndex(GrandTotalColorIndex, (byte)219, (byte)225, (byte)187);
            palette.SetColorAtIndex(GrandTotalBorderColorIndex, (byte)255, (byte)102, (byte)0);
        }

        private void InitColumn(ref ISheet worksheet)
        {
            for (int i = 0; i < 9; i++)
            {
                worksheet.SetColumnWidth(i, 170 * NpoiExcelExtensions.FontWidth);
                worksheet.SetColumnWidth(0, 80 * NpoiExcelExtensions.FontWidth);
            }
        }

        /// <summary>
        /// Exports the report title.
        /// </summary>
        /// <param name="excelFile">The excel file.</param>
        /// <param name="worksheet">The worksheet.</param>
        /// <param name="currentRow">The current row.</param>
        /// <param name="criteria">The criteria.</param>
        private void ExportReportTitle(HSSFWorkbook excelFile, ref ISheet worksheet, ref int currentRow, GetSMATurnoverReportCriteria criteria)
        {
            IRow row = worksheet.CreateRow(currentRow);
            int currentColumn = 0;
            ICellStyle headerStyle = NpoiExcelExtensions.HeaderStyle(excelFile);
            string reportTitle = this.GetReportTitle(criteria);

            NpoiExcelExtensions.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow, currentColumn + 8, reportTitle, headerStyle);
        }

        /// <summary>
        /// Exports the report header.
        /// </summary>
        /// <param name="excelFile">The excel file.</param>
        /// <param name="worksheet">The worksheet.</param>
        /// <param name="currentRow">The current row.</param>
        private void ExportReportHeader(HSSFWorkbook excelFile, ref ISheet worksheet, ref int currentRow)
        {
            int currentColumn = 0;
            IRow row = worksheet.CreateRow(currentRow);
            ICellStyle headerStyle = NpoiExcelExtensions.HeaderStyle(excelFile);

            NpoiExcelExtensions.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow + 1, currentColumn, "#", headerStyle);
            NpoiExcelExtensions.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow + 1, currentColumn, Labels.GroupHeadline, headerStyle);
            NpoiExcelExtensions.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow + 1, currentColumn, Labels.UserName, headerStyle);
            NpoiExcelExtensions.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow + 1, currentColumn, Labels.Currency, headerStyle);
            NpoiExcelExtensions.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow + 1, currentColumn, Labels.Position, headerStyle);
            NpoiExcelExtensions.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow + 1, currentColumn, Labels.Turnover, headerStyle);
            NpoiExcelExtensions.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow + 1, currentColumn, Labels.TurnoverRM, headerStyle);
            NpoiExcelExtensions.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow, currentColumn + 1, Labels.MemberTotal, headerStyle);
            currentRow++;
            currentColumn = 7;
            NpoiExcelExtensions.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow, currentColumn, Labels.Turnover, headerStyle);
            NpoiExcelExtensions.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow, currentColumn, Labels.TurnoverRM, headerStyle);
        }

        /// <summary>
        /// Exports the content of the report.
        /// </summary>
        /// <param name="excelFile">The excel file.</param>
        /// <param name="worksheet">The worksheet.</param>
        /// <param name="currentRow">The current row.</param>
        /// <param name="report">The report.</param>
        [SuppressMessage("StyleCop.CSharp.StylecopPlus.Rules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        private void ExportReportContent(
            HSSFWorkbook excelFile,
            ref ISheet worksheet,
            ref int currentRow,
            SMATurnover report)
        {
            int rowNumber = 0, currentColumn = 0;
            IRow row = null;
            ICellStyle textStyle = NpoiExcelExtensions.TextStyle(excelFile, HSSFColor.White.Index, false, NpoiExcelExtensions.Alignment.Center);
            ICellStyle decimalStyle = NpoiExcelExtensions.DecimalStyle(excelFile, HSSFColor.White.Index, false);

            for (int groupHeadlineIndex = 0; groupHeadlineIndex < report.ReportRows.Count; groupHeadlineIndex++)
            {
                var currentGroupHeadline = report.ReportRows[groupHeadlineIndex];
                int groupHeadlineLength = currentGroupHeadline.ReportRows.Count;
                for (int rowIndex = 0; rowIndex < groupHeadlineLength; rowIndex++)
                {
                    var currentGroupHealineRow = currentGroupHeadline.ReportRows[rowIndex];
                    row = worksheet.CreateRow(currentRow);
                    NpoiExcelExtensions.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow, currentColumn, rowNumber + 1, textStyle);

                    if (rowIndex == 0)
                    {
                        NpoiExcelExtensions.RenderCell(
                            ref worksheet,
                            ref row,
                            currentRow,
                            ref currentColumn,
                            currentRow + groupHeadlineLength - 1,
                            currentColumn,
                            currentGroupHeadline.GroupHeadline,
                            textStyle);
                    }
                    else
                    {
                        currentColumn += 1;
                    }

                    NpoiExcelExtensions.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow, currentColumn, currentGroupHealineRow.UserName, textStyle);
                    NpoiExcelExtensions.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow, currentColumn, currentGroupHealineRow.Currency, textStyle);
                    NpoiExcelExtensions.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow, currentColumn, currentGroupHealineRow.Position, textStyle);
                    NpoiExcelExtensions.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow, currentColumn, currentGroupHealineRow.TurnOver, decimalStyle);
                    NpoiExcelExtensions.RenderCell(
                        ref worksheet,
                        ref row,
                        currentRow,
                        ref currentColumn,
                        currentRow,
                        currentColumn,
                        currentGroupHealineRow.RMTurnOver,
                        decimalStyle);
                    NpoiExcelExtensions.RenderCell(
                        ref worksheet,
                        ref row,
                        currentRow,
                        ref currentColumn,
                        currentRow,
                        currentColumn,
                        currentGroupHealineRow.MemberTurnOver,
                        decimalStyle);
                    NpoiExcelExtensions.RenderCell(
                        ref worksheet,
                        ref row,
                        currentRow,
                        ref currentColumn,
                        currentRow,
                        currentColumn,
                        currentGroupHealineRow.MemberRMTurnOver,
                        decimalStyle);
                    rowNumber++;
                    currentColumn = 0;
                    currentRow++;
                }

                this.ExportGroupHeadlineTotal(excelFile, ref worksheet, ref currentRow, currentGroupHeadline);
                currentRow++;
            }
        }

        /// <summary>
        /// Exports the group headline total.
        /// </summary>
        /// <param name="excelFile">The excel file.</param>
        /// <param name="worksheet">The worksheet.</param>
        /// <param name="currentRow">The current row.</param>
        /// <param name="groupHeadline">The group headline.</param>
        private void ExportGroupHeadlineTotal(
            HSSFWorkbook excelFile,
            ref ISheet worksheet,
            ref int currentRow,
            SMATurnover groupHeadline)
        {
            var total = groupHeadline.Total;
            int currentColumn = 0;
            IRow row = worksheet.CreateRow(currentRow);
            ICellStyle textStyle = NpoiExcelExtensions.TextStyle(excelFile, SubTotalColorIndex, true, NpoiExcelExtensions.Alignment.Center);
            ICellStyle decimalStyle = NpoiExcelExtensions.DecimalStyle(excelFile, SubTotalColorIndex, true);
            string totalTile = string.Format("{0} {1}", groupHeadline.GroupHeadline, Labels.Total);
            NpoiExcelExtensions.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow, currentColumn + 4, totalTile, textStyle);
            currentColumn = 5;
            NpoiExcelExtensions.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow, currentColumn, string.Empty, decimalStyle);
            NpoiExcelExtensions.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow, currentColumn, total.RMTurnOver, decimalStyle);
            NpoiExcelExtensions.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow, currentColumn, string.Empty, decimalStyle);
            NpoiExcelExtensions.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow, currentColumn, total.MemberRMTurnOver, decimalStyle);
        }

        /// <summary>
        /// Exports the grand total.
        /// </summary>
        /// <param name="excelFile">The excel file.</param>
        /// <param name="worksheet">The worksheet.</param>
        /// <param name="currentRow">The current row.</param>
        /// <param name="grandTotal">The grand total.</param>
        private void ExportGrandTotal(
            HSSFWorkbook excelFile,
            ref ISheet worksheet,
            ref int currentRow,
            SMATurnover grandTotal)
        {
            int currentColumn = 0;
            IRow row = worksheet.CreateRow(currentRow);
            ICellStyle textStyle = NpoiExcelExtensions.TextStyle(excelFile, GrandTotalColorIndex, true, NpoiExcelExtensions.Alignment.Center);
            textStyle.BorderTop = BorderStyle.Medium;
            textStyle.TopBorderColor = GrandTotalBorderColorIndex;
            ICellStyle decimalStyle = NpoiExcelExtensions.DecimalStyle(excelFile, GrandTotalColorIndex, true);
            NpoiExcelExtensions.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow, currentColumn + 4, Labels.Grand_Total, textStyle);
            decimalStyle.BorderTop = BorderStyle.Medium;
            decimalStyle.TopBorderColor = GrandTotalBorderColorIndex;
            currentColumn = 5;
            NpoiExcelExtensions.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow, currentColumn, string.Empty, decimalStyle);
            NpoiExcelExtensions.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow, currentColumn, grandTotal.RMTurnOver, decimalStyle);
            NpoiExcelExtensions.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow, currentColumn, string.Empty, decimalStyle);
            NpoiExcelExtensions.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow, currentColumn, string.Empty, decimalStyle);
        }

        /// <summary>
        /// Gets the report title.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>The report title.</returns>
        private string GetReportTitle(GetSMATurnoverReportCriteria criteria)
        {
            string title = string.Empty;

            switch (criteria.ReportType)
            {
                case SMATurnOverConstants.ReportByDate:
                    title = string.Format(
                        MiscResources.SMA_TurnOver_ReportTitle,
                        criteria.FromDate.ToShortFormatedDate());
                    break;
                case SMATurnOverConstants.ReportByMonth:
                    title = string.Format(
                        MiscResources.SMA_TurnOver_Monthly_ReportTitle,
                        criteria.FromDate.ToMonthYear());
                    break;
                case SMATurnOverConstants.ReportByYear:
                    title = title = string.Format(
                        MiscResources.SMA_TurnOver_Annually_ReportTitle,
                        criteria.FromDate.Year.ToString());
                    break;
                default:
                    break;
            }

            return title;
        }

        /// <summary>
        /// Gets the name of the report.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>The report name.</returns>
        private string GetReportName(GetSMATurnoverReportCriteria criteria)
        {
            string reportName = string.Empty;

            switch (criteria.ReportType)
            {
                case SMATurnOverConstants.ReportByDate:
                    reportName = string.Format(
                        MiscResources.SMA_TurnOver_ReportName,
                        criteria.FromDate.ToShortFormatedDateWithSlash().Replace("/", "_"));
                    break;
                case SMATurnOverConstants.ReportByMonth:
                    reportName = string.Format(
                        MiscResources.SMA_TurnOver_Monthly_ReportName,
                        criteria.FromDate.ToMonthYearWithSlash().Replace("/", "_"));
                    break;
                case SMATurnOverConstants.ReportByYear:
                    reportName = string.Format(
                        MiscResources.SMA_TurnOver_Annually_ReportName,
                        criteria.FromDate.Year.ToString());
                    break;
                default:
                    break;
            }

            return reportName;
        }
    }
}
