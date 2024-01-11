namespace Fanex.AccountingReports.Main.Services.Implement.TraderPerformanceOtherSports
{
    using System.Diagnostics.CodeAnalysis;
    using Fanex.AccountingReports.Core.Extensions;
    using Fanex.AccountingReports.Core.Web;
    using Fanex.AccountingReports.Main.Criteria.TraderPerformance;
    using Fanex.AccountingReports.Main.Models.TraderPerformanceOtherSports;
    using Fanex.AccountingReports.Main.Resources;
    using NPOI.HSSF.UserModel;
    using NPOI.HSSF.Util;
    using NPOI.SS.UserModel;
    using NPOI.SS.Util;

    /// <summary>
    /// Trader performance export report service.
    /// </summary>
    public partial class TraderPerformanceOtherSportsService
    {
        /// <summary>
        /// Trader performance export report service.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        public void ExportReport(GetReportCriteria criteria)
        {
            var report = this.GetReport(criteria);

            HSSFWorkbook excelFile = new HSSFWorkbook();
            string sheetName = criteria.IsSoccer ?
                MainResources.TraderPerformanceSoccer_SheetName :
                MainResources.TraderPerformanceOtherSports_SheetName;

            ISheet worksheet = excelFile.CreateSheet(sheetName);
            int currentRow = 0;

            if (report.ReportRows.Count > 0)
            {
                this.ExportReportHeader(excelFile, ref worksheet, ref currentRow, criteria);
                currentRow += 1;
                this.ExportReportContent(excelFile, ref worksheet, ref currentRow, report);
            }

            string reportTypeName = criteria.IsSoccer ?
                MainResources.TraderPerformanceSoccer_ReportName :
                MainResources.TraderPerformanceOtherSports_ReportName;

            string reportName = string.Format(
                    reportTypeName,
                    criteria.Fromdate.ToShortFormatedDateWithSlash(),
                    criteria.Todate.ToShortFormatedDateWithSlash());

            NpoiExcelExtensions.ResponseStreamExcel(excelFile, reportName);
        }

        /// <summary>
        /// Exports the report header.
        /// </summary>
        /// <param name="excelFile">The excel file.</param>
        /// <param name="worksheet">The worksheet.</param>
        /// <param name="currentRow">The current row.</param>
        /// <param name="criteria">The criteria.</param>
        private void ExportReportHeader(
            HSSFWorkbook excelFile,
            ref ISheet worksheet,
            ref int currentRow,
            GetReportCriteria criteria)
        {
            ICellStyle headerStyle = NpoiExcelExtensions.HeaderStyle(excelFile);

            for (var i = 0; i < 53; i++)
            {
                worksheet.SetColumnWidth(i, 140 * ExcelExtensions.FontWidth);
            }

            worksheet.SetColumnWidth(0, 60 * ExcelExtensions.FontWidth);
            worksheet.SetColumnWidth(2, 60 * ExcelExtensions.FontWidth);
            worksheet.SetColumnWidth(3, 60 * ExcelExtensions.FontWidth);
            worksheet.SetColumnWidth(4, 60 * ExcelExtensions.FontWidth);
            worksheet.SetColumnWidth(5, 60 * ExcelExtensions.FontWidth);
            worksheet.SetColumnWidth(6, 60 * ExcelExtensions.FontWidth);
            worksheet.SetColumnWidth(7, 60 * ExcelExtensions.FontWidth);

            this.ExportReportHeaderInfo(
                            ref worksheet,
                            ref currentRow,
                            criteria,
                            headerStyle);
        }

        /// <summary>
        /// Renders the report header detail.
        /// </summary>
        /// <param name="worksheet">The worksheet.</param>
        /// <param name="currentRow">The current row.</param>
        /// <param name="criteria">The criteria.</param>
        /// <param name="headerStyle">The header style.</param>
        [SuppressMessage("StyleCop.CSharp.StylecopPlus.Rules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        private void ExportReportHeaderInfo(
           ref ISheet worksheet,
           ref int currentRow,
           GetReportCriteria criteria,
           ICellStyle headerStyle)
        {
            var currentColumn = 0;
            string reportTypeTitle = criteria.IsSoccer ?
                MainResources.TraderPerformanceSoccer_ReportTitle :
                MainResources.TraderPerformanceOtherSports_ReportTitle;

            string reportTitle = string.Format(
                    reportTypeTitle,
                    criteria.Fromdate.ToShortFormatedDate(),
                    criteria.Todate.ToShortFormatedDate());
            IRow row = worksheet.CreateRow(currentRow);

            this.RenderMergeCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow + 1, currentColumn + 52, reportTitle, headerStyle);
            currentRow += 2;
            worksheet.AddMergedRegion(new CellRangeAddress(currentRow, currentRow, 0, 52));
            currentRow++;
            row = worksheet.CreateRow(currentRow);
            currentColumn = 0;

            this.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, Labels.No, headerStyle);
            this.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, Labels.Trader, headerStyle);
            this.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, Labels.Match, headerStyle);
            this.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, Labels.Major, headerStyle);
            this.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, Labels.Minor1, headerStyle);
            this.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, Labels.Minor2, headerStyle);
            this.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, Labels.Minor3, headerStyle);
            this.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, Labels.Others, headerStyle);

            this.RenderChildGroupHeader(ref worksheet, ref row, ref currentRow, ref currentColumn, Labels.Win_Loss, headerStyle);
            this.RenderChildGroupHeader(ref worksheet, ref row, ref currentRow, ref currentColumn, Labels.Turnover, headerStyle);
            this.RenderChildGroupHeader(ref worksheet, ref row, ref currentRow, ref currentColumn, Labels.Live_BetCount, headerStyle);
            this.RenderChildGroupHeader(ref worksheet, ref row, ref currentRow, ref currentColumn, Labels.Margin_Percent, headerStyle);
            this.RenderChildGroupHeader(ref worksheet, ref row, ref currentRow, ref currentColumn, Labels.Commission, headerStyle);

            currentRow++;
            row = worksheet.CreateRow(currentRow);
            currentColumn = 0;

            this.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, Labels.No, headerStyle);
            this.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, Labels.Trader, headerStyle);
            this.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, Labels.Match, headerStyle);
            this.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, Labels.Major, headerStyle);
            this.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, Labels.Minor1, headerStyle);
            this.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, Labels.Minor2, headerStyle);
            this.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, Labels.Minor3, headerStyle);
            this.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, Labels.Others, headerStyle);

            this.RenderChildGroupDetailHeader(ref worksheet, ref row, ref currentRow, ref currentColumn, headerStyle);
            this.RenderChildGroupDetailHeader(ref worksheet, ref row, ref currentRow, ref currentColumn, headerStyle);
            this.RenderChildGroupDetailHeader(ref worksheet, ref row, ref currentRow, ref currentColumn, headerStyle);
            this.RenderChildGroupDetailHeader(ref worksheet, ref row, ref currentRow, ref currentColumn, headerStyle);
            this.RenderChildGroupDetailHeader(ref worksheet, ref row, ref currentRow, ref currentColumn, headerStyle);

            currentColumn = 0;
            this.RenderMergeCell(ref worksheet, ref row, currentRow - 1, ref currentColumn, currentRow, currentColumn, Labels.No, headerStyle);
            this.RenderMergeCell(ref worksheet, ref row, currentRow - 1, ref currentColumn, currentRow, currentColumn, Labels.Trader, headerStyle);
            this.RenderMergeCell(ref worksheet, ref row, currentRow - 1, ref currentColumn, currentRow, currentColumn, Labels.Match, headerStyle);
            this.RenderMergeCell(ref worksheet, ref row, currentRow - 1, ref currentColumn, currentRow, currentColumn, Labels.Major, headerStyle);
            this.RenderMergeCell(ref worksheet, ref row, currentRow - 1, ref currentColumn, currentRow, currentColumn, Labels.Minor1, headerStyle);
            this.RenderMergeCell(ref worksheet, ref row, currentRow - 1, ref currentColumn, currentRow, currentColumn, Labels.Minor2, headerStyle);
            this.RenderMergeCell(ref worksheet, ref row, currentRow - 1, ref currentColumn, currentRow, currentColumn, Labels.Minor3, headerStyle);
            this.RenderMergeCell(ref worksheet, ref row, currentRow - 1, ref currentColumn, currentRow, currentColumn, Labels.Others, headerStyle);
            this.RenderMergeCell(ref worksheet, ref row, currentRow - 1, ref currentColumn, currentRow - 1, currentColumn + 8, Labels._1st_1x2, headerStyle);
            this.RenderMergeCell(ref worksheet, ref row, currentRow - 1, ref currentColumn, currentRow - 1, currentColumn + 8, Labels._1st_1x2, headerStyle);
            this.RenderMergeCell(ref worksheet, ref row, currentRow - 1, ref currentColumn, currentRow - 1, currentColumn + 8, Labels._1st_1x2, headerStyle);
            this.RenderMergeCell(ref worksheet, ref row, currentRow - 1, ref currentColumn, currentRow - 1, currentColumn + 8, Labels._1st_1x2, headerStyle);
            this.RenderMergeCell(ref worksheet, ref row, currentRow - 1, ref currentColumn, currentRow - 1, currentColumn + 8, Labels._1st_1x2, headerStyle);
        }

        /// <summary>
        /// Renders the child group header.
        /// </summary>
        /// <param name="worksheet">The worksheet.</param>
        /// <param name="row">The row of sheet.</param>
        /// <param name="currentRow">The current row.</param>
        /// <param name="currentColumn">The current column.</param>
        /// <param name="cellValue">The cell value.</param>
        /// <param name="headerStyle">The header style.</param>
        private void RenderChildGroupHeader(
            ref ISheet worksheet,
            ref IRow row,
            ref int currentRow,
            ref int currentColumn,
            object cellValue,
            ICellStyle headerStyle)
        {
            for (int i = 0; i < 9; i++)
            {
                this.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, cellValue, headerStyle);
            }
        }

        /// <summary>
        /// Renders the child group header.
        /// </summary>
        /// <param name="worksheet">The worksheet.</param>
        /// <param name="row">The row of sheet.</param>
        /// <param name="currentRow">The current row.</param>
        /// <param name="currentColumn">The current column.</param>
        /// <param name="headerStyle">The header style.</param>
        private void RenderChildGroupDetailHeader(
            ref ISheet worksheet,
            ref IRow row,
            ref int currentRow,
            ref int currentColumn,
            ICellStyle headerStyle)
        {
            this.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, Labels._1st_1x2, headerStyle);
            this.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, Labels._1x2, headerStyle);
            this.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, Labels._1st_HDP, headerStyle);
            this.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, Labels.HDP, headerStyle);
            this.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, Labels._1st_O_U, headerStyle);
            this.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, Labels.O_U, headerStyle);
            this.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, Labels._1st_OE, headerStyle);
            this.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, Labels.OE, headerStyle);
            this.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, Labels.Overall, headerStyle);
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
           TraderPerformanceOtherSports report)
        {
            excelFile.CreateDataFormat();
            this.SetColorIndex(excelFile);

            this.CreateDecimalNumberStyle(excelFile, HSSFColor.White.Index, HorizontalAlignment.Right, false, true, false);
            ICellStyle decimalNumberWinLossStyle = this.CreateDecimalNumberStyle(excelFile, WinLossGroupColorIndex, HorizontalAlignment.Right, false, true, false);
            ICellStyle decimalNumberTurnOverStyle = this.CreateDecimalNumberStyle(excelFile, TurnOverGroupColorIndex, HorizontalAlignment.Right, false, true, false);
            ICellStyle intNumberLiveBetCountStyle = NpoiExcelExtensions.IntStyle(
                                                                            excelFile,
                                                                            LiveBetCountGroupColorIndex,
                                                                            false);

            ICellStyle decimalPercentMarginStyle = this.CreateDecimalPercentStyle(excelFile, MarginGroupColorIndex, HorizontalAlignment.Right, false, true, false);
            ICellStyle decimalNumberCommissionStyle = this.CreateDecimalNumberStyle(excelFile, CommissionGroupColorIndex, HorizontalAlignment.Right, false, true, false);

            ICellStyle intNumberStyle = NpoiExcelExtensions.IntStyle(excelFile, HSSFColor.White.Index, false);
            ICellStyle intNumberCenterStyle = NpoiExcelExtensions.IntStyle(excelFile, HSSFColor.White.Index, false, NpoiExcelExtensions.Alignment.Center);
            ICellStyle textStyle = NpoiExcelExtensions.TextStyle(excelFile, HSSFColor.White.Index, false, NpoiExcelExtensions.Alignment.Left);
            ICellStyle intNumberEvenRowStyle = NpoiExcelExtensions.IntStyle(excelFile, EvenColorIndex, false);
            ICellStyle intNumberCenterEvenRowStyle = NpoiExcelExtensions.IntStyle(excelFile, EvenColorIndex, false, NpoiExcelExtensions.Alignment.Center);
            ICellStyle textEvenRowStyle = NpoiExcelExtensions.TextStyle(excelFile, EvenColorIndex, false, NpoiExcelExtensions.Alignment.Left);

            ICellStyle grandTotalTextStyle = NpoiExcelExtensions.TextStyle(excelFile, GrandTotalColorIndex, true, NpoiExcelExtensions.Alignment.Center);
            grandTotalTextStyle.BorderTop = BorderStyle.Medium;
            grandTotalTextStyle.TopBorderColor = GrandTotalBorderColorIndex;

            ICellStyle grandTotalNumberStyle = NpoiExcelExtensions.IntStyle(excelFile, GrandTotalColorIndex, true);
            grandTotalNumberStyle.BorderTop = BorderStyle.Medium;
            grandTotalNumberStyle.TopBorderColor = GrandTotalBorderColorIndex;

            ICellStyle grandTotalDecimalNumberStyle = this.CreateDecimalNumberStyle(excelFile, GrandTotalColorIndex, HorizontalAlignment.Right, true, true, false);
            grandTotalDecimalNumberStyle.BorderTop = BorderStyle.Medium;
            grandTotalDecimalNumberStyle.TopBorderColor = GrandTotalBorderColorIndex;

            ICellStyle grandTotalDecimalPercentStyle = this.CreateDecimalPercentStyle(excelFile, GrandTotalColorIndex, HorizontalAlignment.Right, true, true, false);
            grandTotalDecimalPercentStyle.BorderTop = BorderStyle.Medium;
            grandTotalDecimalPercentStyle.TopBorderColor = GrandTotalBorderColorIndex;

            IRow row = worksheet.CreateRow(currentRow);
            var reportLength = report.ReportRows.Count;
            var currentColumn = 0;

            for (int rowIndex = 0; rowIndex < reportLength; rowIndex++)
            {
                ICellStyle indexColumnStyle = rowIndex % 2 == 0 ? intNumberCenterStyle : intNumberCenterEvenRowStyle;
                ICellStyle traderColumnStyle = rowIndex % 2 == 0 ? textStyle : textEvenRowStyle;
                ICellStyle matchColumnStyle = rowIndex % 2 == 0 ? intNumberStyle : intNumberEvenRowStyle;

                var currentRowReport = report.ReportRows[rowIndex];

                this.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, rowIndex + 1, indexColumnStyle);
                this.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRowReport.Name, traderColumnStyle);
                this.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRowReport.MatchNumber, matchColumnStyle);
                this.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRowReport.Major, matchColumnStyle);
                this.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRowReport.Minor1, matchColumnStyle);
                this.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRowReport.Minor2, matchColumnStyle);
                this.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRowReport.Minor3, matchColumnStyle);
                this.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRowReport.Others, matchColumnStyle);

                this.ExportChildGroupReportContent(ref worksheet, ref row, ref currentRow, ref currentColumn, currentRowReport.ChildInfo[0], decimalNumberWinLossStyle);
                this.ExportChildGroupReportContent(ref worksheet, ref row, ref currentRow, ref currentColumn, currentRowReport.ChildInfo[1], decimalNumberTurnOverStyle);
                this.ExportChildGroupReportContent(ref worksheet, ref row, ref currentRow, ref currentColumn, currentRowReport.ChildInfo[2], intNumberLiveBetCountStyle);
                this.ExportMarginGroupReportContent(ref worksheet, ref row, ref currentRow, ref currentColumn, currentRowReport.ChildInfo[3], decimalPercentMarginStyle);
                this.ExportChildGroupReportContent(ref worksheet, ref row, ref currentRow, ref currentColumn, currentRowReport.ChildInfo[4], decimalNumberCommissionStyle);

                currentRow++;
                row = worksheet.CreateRow(currentRow);
                currentColumn = 0;
            }

            this.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, Labels.Grand_Total, grandTotalTextStyle);
            this.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, Labels.Grand_Total, grandTotalTextStyle);
            worksheet.AddMergedRegion(new CellRangeAddress(currentRow, currentRow, 0, currentColumn - 1));
            this.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, report.GrandTotal.MatchNumber, grandTotalNumberStyle);
            this.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, report.GrandTotal.Major, grandTotalNumberStyle);
            this.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, report.GrandTotal.Minor1, grandTotalNumberStyle);
            this.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, report.GrandTotal.Minor2, grandTotalNumberStyle);
            this.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, report.GrandTotal.Minor3, grandTotalNumberStyle);
            this.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, report.GrandTotal.Others, grandTotalNumberStyle);

            this.ExportChildGroupReportContent(ref worksheet, ref row, ref currentRow, ref currentColumn, report.GrandTotal.ChildInfo[0], grandTotalDecimalNumberStyle);
            this.ExportChildGroupReportContent(ref worksheet, ref row, ref currentRow, ref currentColumn, report.GrandTotal.ChildInfo[1], grandTotalDecimalNumberStyle);
            this.ExportChildGroupReportContent(ref worksheet, ref row, ref currentRow, ref currentColumn, report.GrandTotal.ChildInfo[2], grandTotalNumberStyle);
            this.ExportMarginGroupReportContent(ref worksheet, ref row, ref currentRow, ref currentColumn, report.GrandTotal.ChildInfo[3], grandTotalDecimalPercentStyle);
            this.ExportChildGroupReportContent(ref worksheet, ref row, ref currentRow, ref currentColumn, report.GrandTotal.ChildInfo[4], grandTotalDecimalNumberStyle);
        }

        /// <summary>
        /// Exports the content of the child group report.
        /// </summary>
        /// <param name="worksheet">The worksheet.</param>
        /// <param name="row">The row of sheet.</param>
        /// <param name="currentRow">The current row.</param>
        /// <param name="currentColumn">The current column.</param>
        /// <param name="childInfo">The child information.</param>
        /// <param name="cellStyle">The cell style.</param>
        private void ExportChildGroupReportContent(
            ref ISheet worksheet,
            ref IRow row,
            ref int currentRow,
            ref int currentColumn,
            TraderPerformanceOtherSports childInfo,
            ICellStyle cellStyle)
        {
            this.ExportCellValue(ref worksheet, ref row, ref currentRow, ref currentColumn, childInfo.FirstHalfOneXTwo, cellStyle);
            this.ExportCellValue(ref worksheet, ref row, ref currentRow, ref currentColumn, childInfo.OneXTwo, cellStyle);
            this.ExportCellValue(ref worksheet, ref row, ref currentRow, ref currentColumn, childInfo.FirstHalfHdp, cellStyle);
            this.ExportCellValue(ref worksheet, ref row, ref currentRow, ref currentColumn, childInfo.Hdp, cellStyle);
            this.ExportCellValue(ref worksheet, ref row, ref currentRow, ref currentColumn, childInfo.FirstHalfOU, cellStyle);
            this.ExportCellValue(ref worksheet, ref row, ref currentRow, ref currentColumn, childInfo.OU, cellStyle);
            this.ExportCellValue(ref worksheet, ref row, ref currentRow, ref currentColumn, childInfo.FirstHalfOE, cellStyle);
            this.ExportCellValue(ref worksheet, ref row, ref currentRow, ref currentColumn, childInfo.OE, cellStyle);
            this.ExportCellValue(ref worksheet, ref row, ref currentRow, ref currentColumn, childInfo.Overall, cellStyle);
        }

        /// <summary>
        /// Exports the content of the margin group report.
        /// </summary>
        /// <param name="worksheet">The worksheet.</param>
        /// <param name="row">The row of sheet.</param>
        /// <param name="currentRow">The current row.</param>
        /// <param name="currentColumn">The current column.</param>
        /// <param name="marginInfo">The margin information.</param>
        /// <param name="cellStyle">The cell style.</param>
        private void ExportMarginGroupReportContent(
            ref ISheet worksheet,
            ref IRow row,
            ref int currentRow,
            ref int currentColumn,
            TraderPerformanceOtherSports marginInfo,
            ICellStyle cellStyle)
        {
            this.ExportCellValue(ref worksheet, ref row, ref currentRow, ref currentColumn, marginInfo.FirstHalfOneXTwo / 100, cellStyle);
            this.ExportCellValue(ref worksheet, ref row, ref currentRow, ref currentColumn, marginInfo.OneXTwo / 100, cellStyle);
            this.ExportCellValue(ref worksheet, ref row, ref currentRow, ref currentColumn, marginInfo.FirstHalfHdp / 100, cellStyle);
            this.ExportCellValue(ref worksheet, ref row, ref currentRow, ref currentColumn, marginInfo.Hdp / 100, cellStyle);
            this.ExportCellValue(ref worksheet, ref row, ref currentRow, ref currentColumn, marginInfo.FirstHalfOU / 100, cellStyle);
            this.ExportCellValue(ref worksheet, ref row, ref currentRow, ref currentColumn, marginInfo.OU / 100, cellStyle);
            this.ExportCellValue(ref worksheet, ref row, ref currentRow, ref currentColumn, marginInfo.FirstHalfOE / 100, cellStyle);
            this.ExportCellValue(ref worksheet, ref row, ref currentRow, ref currentColumn, marginInfo.OE / 100, cellStyle);
            this.ExportCellValue(ref worksheet, ref row, ref currentRow, ref currentColumn, marginInfo.Overall / 100, cellStyle);
        }
    }
}
