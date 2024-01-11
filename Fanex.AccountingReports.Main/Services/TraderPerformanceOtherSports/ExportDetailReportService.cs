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

    public partial class TraderPerformanceOtherSportsService
    {
        public void ExportDetailReport(GetDetailReportCriteria criteria)
        {
            var report = this.GetDetailReport(criteria);

            HSSFWorkbook excelFile = new HSSFWorkbook();
            string sheetName = MainResources.TraderPerformanceOtherSportsDetail_SheetName;
            ISheet worksheet = excelFile.CreateSheet(sheetName);
            int currentRow = 0;

            if (report.ReportRows.Count > 0)
            {
                this.ExportDetailReportHeader(excelFile, ref worksheet, ref currentRow, criteria);
                currentRow += 1;
                this.ExportDetailReportContent(excelFile, ref worksheet, ref currentRow, report);
            }

            string reportName = string.Format(
                    MainResources.TraderPerformanceOtherSportsDetail_ReportName,
                    criteria.Fromdate.ToShortFormatedDateWithSlash(),
                    criteria.Todate.ToShortFormatedDateWithSlash());

            NpoiExcelExtensions.ResponseStreamExcel(excelFile, reportName);
        }

        private void ExportDetailReportHeader(
            HSSFWorkbook excelFile,
            ref ISheet worksheet,
            ref int currentRow,
            GetDetailReportCriteria criteria)
        {
            ICellStyle headerStyle = NpoiExcelExtensions.HeaderStyle(excelFile);

            for (var i = 0; i < 53; i++)
            {
                worksheet.SetColumnWidth(i, 140 * ExcelExtensions.FontWidth);
            }

            worksheet.SetColumnWidth(0, 80 * ExcelExtensions.FontWidth);
            worksheet.SetColumnWidth(2, 100 * ExcelExtensions.FontWidth);
            worksheet.SetColumnWidth(6, 80 * ExcelExtensions.FontWidth);
            worksheet.SetColumnWidth(7, 80 * ExcelExtensions.FontWidth);

            worksheet.SetColumnWidth(3, 500 * ExcelExtensions.FontWidth);
            worksheet.SetColumnWidth(4, 750 * ExcelExtensions.FontWidth);

            this.ExportDetailReportHeaderInfo(
                            ref worksheet,
                            ref currentRow,
                            criteria,
                            headerStyle);
        }

        [SuppressMessage("StyleCop.CSharp.StylecopPlus.Rules",
        "SP2101:MethodMustNotContainMoreLinesThan",
        Justification = "Reviewed. Suppression is OK here.")]
        private void ExportDetailReportHeaderInfo(
           ref ISheet worksheet,
           ref int currentRow,
           GetDetailReportCriteria criteria,
           ICellStyle headerStyle)
        {
            var currentColumn = 0;
            string reportTitle = string.Format(
                    MainResources.TraderPerformanceOtherSportsDetail_ReportTitle,
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
            this.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, Labels.Date, headerStyle);
            this.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, Labels.League, headerStyle);
            this.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, Labels.Match, headerStyle);
            this.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, Labels.Maxbet, headerStyle);
            this.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, Labels.Score, headerStyle);
            this.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, Labels.Score, headerStyle);

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
            this.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, Labels.Date, headerStyle);
            this.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, Labels.League, headerStyle);
            this.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, Labels.Match, headerStyle);
            this.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, Labels.Maxbet, headerStyle);
            this.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, Labels.HT, headerStyle);
            this.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, Labels.FT, headerStyle);

            this.RenderChildGroupDetailHeader(ref worksheet, ref row, ref currentRow, ref currentColumn, headerStyle);
            this.RenderChildGroupDetailHeader(ref worksheet, ref row, ref currentRow, ref currentColumn, headerStyle);
            this.RenderChildGroupDetailHeader(ref worksheet, ref row, ref currentRow, ref currentColumn, headerStyle);
            this.RenderChildGroupDetailHeader(ref worksheet, ref row, ref currentRow, ref currentColumn, headerStyle);
            this.RenderChildGroupDetailHeader(ref worksheet, ref row, ref currentRow, ref currentColumn, headerStyle);

            currentColumn = 0;
            this.RenderMergeCell(ref worksheet, ref row, currentRow - 1, ref currentColumn, currentRow, currentColumn, Labels.No, headerStyle);
            this.RenderMergeCell(ref worksheet, ref row, currentRow - 1, ref currentColumn, currentRow, currentColumn, Labels.Trader, headerStyle);
            this.RenderMergeCell(ref worksheet, ref row, currentRow - 1, ref currentColumn, currentRow, currentColumn, Labels.Date, headerStyle);
            this.RenderMergeCell(ref worksheet, ref row, currentRow - 1, ref currentColumn, currentRow, currentColumn, Labels.League, headerStyle);
            this.RenderMergeCell(ref worksheet, ref row, currentRow - 1, ref currentColumn, currentRow, currentColumn, Labels.Match, headerStyle);
            this.RenderMergeCell(ref worksheet, ref row, currentRow - 1, ref currentColumn, currentRow, currentColumn, Labels.Maxbet, headerStyle);
            this.RenderMergeCell(ref worksheet, ref row, currentRow - 1, ref currentColumn, currentRow - 1, currentColumn + 1, Labels.HT, headerStyle);
            this.RenderMergeCell(ref worksheet, ref row, currentRow - 1, ref currentColumn, currentRow - 1, currentColumn + 8, Labels._1st_1x2, headerStyle);
            this.RenderMergeCell(ref worksheet, ref row, currentRow - 1, ref currentColumn, currentRow - 1, currentColumn + 8, Labels._1st_1x2, headerStyle);
            this.RenderMergeCell(ref worksheet, ref row, currentRow - 1, ref currentColumn, currentRow - 1, currentColumn + 8, Labels._1st_1x2, headerStyle);
            this.RenderMergeCell(ref worksheet, ref row, currentRow - 1, ref currentColumn, currentRow - 1, currentColumn + 8, Labels._1st_1x2, headerStyle);
            this.RenderMergeCell(ref worksheet, ref row, currentRow - 1, ref currentColumn, currentRow - 1, currentColumn + 8, Labels._1st_1x2, headerStyle);
        }

        [SuppressMessage("StyleCop.CSharp.StylecopPlus.Rules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        private void ExportDetailReportContent(
           HSSFWorkbook excelFile,
           ref ISheet worksheet,
           ref int currentRow,
           TraderPerformanceOtherSportsDetail report)
        {
            excelFile.CreateDataFormat();
            this.SetColorIndex(excelFile);

            ICellStyle decimalNumberStyle = this.CreateDecimalNumberStyle(excelFile, HSSFColor.White.Index, HorizontalAlignment.Right, false, true, false);
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
            ICellStyle centerTextStyle = NpoiExcelExtensions.TextStyle(excelFile, HSSFColor.White.Index, false, NpoiExcelExtensions.Alignment.Center);
            ICellStyle textEvenRowStyle = NpoiExcelExtensions.TextStyle(excelFile, EvenColorIndex, false, NpoiExcelExtensions.Alignment.Left);
            NpoiExcelExtensions.TextStyle(excelFile, EvenColorIndex, false, NpoiExcelExtensions.Alignment.Center);

            ICellStyle subTotalTextStyle = NpoiExcelExtensions.TextStyle(excelFile, SubTotalColorIndex, true, NpoiExcelExtensions.Alignment.Center);
            ICellStyle subTotalIntNumberStyle = NpoiExcelExtensions.IntStyle(excelFile, SubTotalColorIndex, true);
            ICellStyle subTotalDecimalNumberStyle = this.CreateDecimalNumberStyle(excelFile, SubTotalColorIndex, HorizontalAlignment.Right, true, true, false);
            ICellStyle subTotalDecimalPercentStyle = this.CreateDecimalPercentStyle(excelFile, SubTotalColorIndex, HorizontalAlignment.Right, true, true, false);

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
                var currentRowReport = report.ReportRows[rowIndex];
                var numberMergeRows = currentRowReport.ReportRows.Count;

                for (int currentChildRowIndex = 0; currentChildRowIndex < currentRowReport.ReportRows.Count; currentChildRowIndex++)
                {
                    var currentChildRow = currentRowReport.ReportRows[currentChildRowIndex];
                    ICellStyle textColumnStyle = currentChildRowIndex % 2 == 0 ? textStyle : textEvenRowStyle;
                    currentColumn = 0;

                    this.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, rowIndex + 1, intNumberCenterStyle);
                    this.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRowReport.Name, textStyle);
                    this.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentChildRow.EventDate, textColumnStyle);
                    this.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentChildRow.LeagueName, textColumnStyle);
                    this.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentChildRow.HomeName + "-" + currentChildRow.AwayName, textColumnStyle);
                    this.ExportCellValue(ref worksheet, ref row, ref currentRow, ref currentColumn, currentChildRow.Maxbet, decimalNumberStyle);
                    this.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentChildRow.HTHomeScore + "--" + currentChildRow.HTAwayScore, centerTextStyle);
                    this.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentChildRow.FTHomeScore + "--" + currentChildRow.FTAwayScore, centerTextStyle);

                    this.ExportChildGroupDetailReportContent(ref worksheet, ref row, ref currentRow, ref currentColumn, currentChildRow.ChildInfo[0], decimalNumberWinLossStyle);
                    this.ExportChildGroupDetailReportContent(ref worksheet, ref row, ref currentRow, ref currentColumn, currentChildRow.ChildInfo[1], decimalNumberTurnOverStyle);
                    this.ExportChildGroupDetailReportContent(
                                                            ref worksheet,
                                                            ref row,
                                                            ref currentRow,
                                                            ref currentColumn,
                                                            currentChildRow.ChildInfo[2],
                                                            intNumberLiveBetCountStyle);
                    this.ExportMarginGroupDetailReportContent(ref worksheet, ref row, ref currentRow, ref currentColumn, currentChildRow.ChildInfo[3], decimalPercentMarginStyle);
                    this.ExportChildGroupDetailReportContent(ref worksheet, ref row, ref currentRow, ref currentColumn, currentChildRow.ChildInfo[4], decimalNumberCommissionStyle);

                    currentRow++;
                    row = worksheet.CreateRow(currentRow);
                }

                currentColumn = 0;
                this.RenderMergeCell(
                    ref worksheet,
                    ref row,
                    currentRow - numberMergeRows,
                    ref currentColumn,
                    currentRow - 1,
                    currentColumn,
                    rowIndex + 1,
                    intNumberStyle);
                this.RenderMergeCell(
                    ref worksheet,
                    ref row,
                    currentRow - numberMergeRows,
                    ref currentColumn,
                    currentRow - 1,
                    currentColumn,
                    currentRowReport.Name,
                    intNumberStyle);

                currentColumn = 0;
                this.ExportDetailReportTotalInfo(
                ref worksheet,
                ref row,
                ref currentRow,
                ref currentColumn,
                currentRowReport.GrandTotal,
                currentRowReport.Name,
                subTotalTextStyle,
                subTotalIntNumberStyle,
                subTotalDecimalNumberStyle,
                subTotalDecimalPercentStyle);

                currentRow++;
                row = worksheet.CreateRow(currentRow);
                currentColumn = 0;
            }

            this.ExportDetailReportTotalInfo(
                ref worksheet,
                ref row,
                ref currentRow,
                ref currentColumn,
                report.GrandTotal,
                Labels.Grand_Total,
                grandTotalTextStyle,
                grandTotalNumberStyle,
                grandTotalDecimalNumberStyle,
                grandTotalDecimalPercentStyle);
        }

        private void ExportChildGroupDetailReportContent(
            ref ISheet worksheet,
            ref IRow row,
            ref int currentRow,
            ref int currentColumn,
            TraderPerformanceOtherSportsDetail childInfo,
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

        private void ExportMarginGroupDetailReportContent(
            ref ISheet worksheet,
            ref IRow row,
            ref int currentRow,
            ref int currentColumn,
            TraderPerformanceOtherSportsDetail marginInfo,
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

        private void ExportDetailReportTotalInfo(
            ref ISheet worksheet,
            ref IRow row,
            ref int currentRow,
            ref int currentColumn,
            TraderPerformanceOtherSportsDetail totalInfo,
            string totalTitle,
            ICellStyle cellTextStyle,
            ICellStyle cellNumberStyle,
            ICellStyle cellDecimalNumberStyle,
            ICellStyle cellPercentStyle)
        {
            this.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, totalTitle, cellTextStyle);
            this.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, totalTitle, cellTextStyle);
            this.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, totalTitle, cellTextStyle);
            this.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, totalTitle, cellTextStyle);
            this.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, totalTitle, cellTextStyle);
            this.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, totalTitle, cellTextStyle);
            worksheet.AddMergedRegion(new CellRangeAddress(currentRow, currentRow, 0, currentColumn - 1));
            this.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, null, cellTextStyle);
            this.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, null, cellTextStyle);

            this.ExportChildGroupDetailReportContent(
                ref worksheet,
                ref row,
                ref currentRow,
                ref currentColumn,
                totalInfo.ChildInfo[0],
                cellDecimalNumberStyle);
            this.ExportChildGroupDetailReportContent(
                ref worksheet,
                ref row,
                ref currentRow,
                ref currentColumn,
                totalInfo.ChildInfo[1],
                cellDecimalNumberStyle);
            this.ExportChildGroupDetailReportContent(
                ref worksheet,
                ref row,
                ref currentRow,
                ref currentColumn,
                totalInfo.ChildInfo[2],
                cellNumberStyle);
            this.ExportMarginGroupDetailReportContent(
                ref worksheet,
                ref row,
                ref currentRow,
                ref currentColumn,
                totalInfo.ChildInfo[3],
                cellPercentStyle);
            this.ExportChildGroupDetailReportContent(
                ref worksheet,
                ref row,
                ref currentRow,
                ref currentColumn,
                totalInfo.ChildInfo[4],
                cellDecimalNumberStyle);
        }
    }
}
