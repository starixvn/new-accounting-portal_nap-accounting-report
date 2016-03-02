namespace Fanex.AccountingReports.Main.Services.Implement.MatchWinLoss2a
{
    using System.Collections.Generic;
    using Fanex.AccountingReports.Core.Extensions;
    using Fanex.AccountingReports.Core.Web;
    using Fanex.AccountingReports.Main.Criteria.MatchWinLoss2a;
    using Fanex.AccountingReports.Main.Models.MatchWinLoss2a;
    using Fanex.AccountingReports.Main.Resources;
    using Fanex.AccountingReports.Main.Services.Interface.MatchWinLoss2a;
    using NPOI.HSSF.UserModel;
    using NPOI.HSSF.Util;
    using NPOI.SS.UserModel;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class ExportCockFightingService : IExportCockFightingService
    {
        private const short GrandTotalColorIndex = 47;
        private const short GrandTotalBorderColorIndex = 49;

        private string _cockFightingReportCachedDataKey = "Fanex.AccountingReports.Main.MatchWinLoss2a.Cockfighting" +
          (AppSession.CurrentUser != null ? AppSession.CurrentUser.Username : string.Empty);

        private string _cockFightingMatchByArenaReportCachedDataKey = "Fanex.AccountingReports.Main.MatchWinLoss2a.CockfightingMatchByArena" +
          (AppSession.CurrentUser != null ? AppSession.CurrentUser.Username : string.Empty);

        private ICockFightingService _cockFightingService;

        public ExportCockFightingService()
            : this(null)
        {
        }

        public ExportCockFightingService(ICockFightingService cockFightingService)
        {
            this._cockFightingService = cockFightingService ?? new CockFightingService();
        }

        public void ExportCockFightingReport(GetCockFightingCriteria criteria)
        {
            var report = CacheExtension.Get<CockFighting>(_cockFightingReportCachedDataKey);

            if (report == null)
            {
                report = this._cockFightingService.GetCockFightingReport(criteria);
            }

            HSSFWorkbook workbook = new HSSFWorkbook();
            string sheetName = MainResources.MatchWinLoss2a_SheetName;
            ISheet worksheet = workbook.CreateSheet(sheetName);
            this.SetColorAtIndex(workbook);
            this.InitColumnWith(ref worksheet);
            int currentRow = 0;
            this.ExportReportTitle(workbook, ref worksheet, ref currentRow, criteria);
            currentRow += 3;
            this.ExportReportHeader(workbook, ref worksheet, ref currentRow);
            currentRow++;
            this.ExportReportContent(workbook, ref worksheet, ref currentRow, report.ReportRows);
            this.ExportGrandTotal(workbook, ref worksheet, ref currentRow, report.GrandTotal);

            string reportName = string.Format(
                   MainResources.MatchWinLoss2aDetailByArena_ReportName,
                   criteria.WinLostDate.ToShortFormatedDateWithSlash().Replace("/", "_"));
            NpoiExcelExtensions.ResponseStreamExcel(workbook, reportName);
        }

        public void ExportCockFightingMatchByArenaReport(GetCockFightingMatchByArena criteria)
        {
            var report = CacheExtension.Get<CockFightingMatchByArena>(_cockFightingMatchByArenaReportCachedDataKey);

            if (report == null)
            {
                report = this._cockFightingService.GetCockFightingMatchByArenaReport(criteria);
            }

            HSSFWorkbook workbook = new HSSFWorkbook();
            string sheetName = MainResources.MatchWinLoss2a_SheetName;
            ISheet worksheet = workbook.CreateSheet(sheetName);
            this.SetColorAtIndex(workbook);
            this.InitCockFightingMatchByArenaColumnWith(ref worksheet);
            int currentRow = 0;
            this.ExportCockFightingMatchByArenaReportTitle(workbook, ref worksheet, ref currentRow, criteria);
            currentRow += 3;
            this.ExportCockFightingMatchByArenaReportHeader(workbook, ref worksheet, ref currentRow);
            currentRow++;
            this.ExportCockFightingMatchByArenaReportContent(workbook, ref worksheet, ref currentRow, report.ReportRows);
            this.ExportCockFightingMatchByArenaGrandTotal(workbook, ref worksheet, ref currentRow, report.GrandTotal, criteria);

            string reportName = string.Format(
                   MainResources.MatchWinLoss2aDetailByMatch_ReportName,
                   criteria.WinLostDate.ToShortFormatedDateWithSlash().Replace("/", "_"));
            NpoiExcelExtensions.ResponseStreamExcel(workbook, reportName);
        }

        private void InitColumnWith(ref ISheet worksheet)
        {
            for (int i = 0; i < 10; i++)
            {
                worksheet.SetColumnWidth(i, 170 * ExcelExtensions.FontWidth);
            }

            worksheet.SetColumnWidth(0, 220 * ExcelExtensions.FontWidth);
        }

        private void SetColorAtIndex(HSSFWorkbook excelFile)
        {
            HSSFPalette palette = excelFile.GetCustomPalette();
            palette.SetColorAtIndex(GrandTotalColorIndex, (byte)219, (byte)225, (byte)187);
            palette.SetColorAtIndex(GrandTotalBorderColorIndex, (byte)255, (byte)102, (byte)0);
        }

        private void ExportReportTitle(
            HSSFWorkbook workbook,
            ref ISheet worksheet,
            ref int currentRow,
            GetCockFightingCriteria criteria)
        {
            ICellStyle headerStyle = NpoiExcelExtensions.HeaderStyle(workbook);
            int currentColumn = 0;
            string reportTitle = string.Format(
                   MainResources.CockFighting_ReportTitle.ToUpper().ToString(),
                   criteria.WinLostDate.ToShortFormatedDate().ToUpper().ToString());
            IRow row = worksheet.CreateRow(currentRow);
            NpoiExcelExtensions.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow + 1, currentColumn + 9, reportTitle, headerStyle);
        }

        private void ExportReportHeader(
            HSSFWorkbook workbook,
            ref ISheet worksheet,
            ref int currentRow)
        {
            int currentColumn = 0;
            IRow row = worksheet.CreateRow(currentRow);
            ICellStyle headerStyle = NpoiExcelExtensions.HeaderStyle(workbook);
            NpoiExcelExtensions.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow, currentColumn, Labels.Arena, headerStyle);
            NpoiExcelExtensions.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow, currentColumn, Labels.Win_Loss, headerStyle);
            NpoiExcelExtensions.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow, currentColumn, Labels.Position, headerStyle);
            NpoiExcelExtensions.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow, currentColumn, Labels.Comm, headerStyle);
            NpoiExcelExtensions.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow, currentColumn, Labels.Virtual_Comm, headerStyle);
            NpoiExcelExtensions.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow, currentColumn, Labels.Line_Total, headerStyle);
            NpoiExcelExtensions.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow, currentColumn, Labels.Virtual_Line_Total, headerStyle);
            NpoiExcelExtensions.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow, currentColumn, Labels.Variance, headerStyle);
            NpoiExcelExtensions.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow, currentColumn, Labels.Turnover, headerStyle);
            NpoiExcelExtensions.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow, currentColumn, Labels.Margin_Percent, headerStyle);
        }

        private void ExportReportContent(
            HSSFWorkbook workbook,
            ref ISheet worksheet,
            ref int currentRow,
            List<CockFightingDetail> reportContent)
        {
            ICellStyle textStyle = NpoiExcelExtensions.TextStyle(workbook, HSSFColor.White.Index, false, NpoiExcelExtensions.Alignment.Center);
            ICellStyle decimalNumberStyle = NpoiExcelExtensions.DecimalStyle(workbook, HSSFColor.White.Index, false);
            ICellStyle percentStyle = NpoiExcelExtensions.PercentStyle(workbook, HSSFColor.White.Index, false);
            IRow row = null;
            int currentColumn = 0;

            foreach (var item in reportContent)
            {
                row = worksheet.CreateRow(currentRow);
                currentColumn = 0;

                NpoiExcelExtensions.RenderCell(
                    ref worksheet,
                    ref row,
                    currentRow,
                    ref currentColumn,
                    currentRow,
                    currentColumn,
                    item.ArenaName,
                    textStyle);

                NpoiExcelExtensions.RenderCell(
                   ref worksheet,
                   ref row,
                   currentRow,
                   ref currentColumn,
                   currentRow,
                   currentColumn,
                   item.WinLoss,
                   decimalNumberStyle);

                NpoiExcelExtensions.RenderCell(
                   ref worksheet,
                   ref row,
                   currentRow,
                   ref currentColumn,
                   currentRow,
                   currentColumn,
                   item.Position,
                   decimalNumberStyle);

                NpoiExcelExtensions.RenderCell(
                  ref worksheet,
                  ref row,
                  currentRow,
                  ref currentColumn,
                  currentRow,
                  currentColumn,
                  item.Commission,
                  decimalNumberStyle);

                NpoiExcelExtensions.RenderCell(
                   ref worksheet,
                   ref row,
                   currentRow,
                   ref currentColumn,
                   currentRow,
                   currentColumn,
                   item.VirtualCommission,
                   decimalNumberStyle);

                NpoiExcelExtensions.RenderCell(
                  ref worksheet,
                  ref row,
                  currentRow,
                  ref currentColumn,
                  currentRow,
                  currentColumn,
                  item.RealLineTotal,
                  decimalNumberStyle);

                NpoiExcelExtensions.RenderCell(
                   ref worksheet,
                   ref row,
                   currentRow,
                   ref currentColumn,
                   currentRow,
                   currentColumn,
                   item.VirtualLineTotal,
                   decimalNumberStyle);

                NpoiExcelExtensions.RenderCell(
                  ref worksheet,
                  ref row,
                  currentRow,
                  ref currentColumn,
                  currentRow,
                  currentColumn,
                  item.Variance,
                  decimalNumberStyle);

                NpoiExcelExtensions.RenderCell(
                   ref worksheet,
                   ref row,
                   currentRow,
                   ref currentColumn,
                   currentRow,
                   currentColumn,
                   item.RealTurnover,
                   decimalNumberStyle);

                NpoiExcelExtensions.RenderCell(
                   ref worksheet,
                   ref row,
                   currentRow,
                   ref currentColumn,
                   currentRow,
                   currentColumn,
                   item.Margin_Str,
                   percentStyle);

                currentRow++;
            }
        }

        private void ExportGrandTotal(
            HSSFWorkbook workbook,
            ref ISheet worksheet,
            ref int currentRow,
            CockFightingDetail grandTotal)
        {
            ICellStyle grandTotalTextStyle = NpoiExcelExtensions.TextStyle(workbook, GrandTotalColorIndex, true, NpoiExcelExtensions.Alignment.Center);
            grandTotalTextStyle.BorderTop = BorderStyle.Medium;
            grandTotalTextStyle.TopBorderColor = GrandTotalBorderColorIndex;

            ICellStyle grandTotalDecimalNumberStyle = NpoiExcelExtensions.DecimalStyle(workbook, GrandTotalColorIndex, true);
            grandTotalDecimalNumberStyle.BorderTop = BorderStyle.Medium;
            grandTotalDecimalNumberStyle.TopBorderColor = GrandTotalBorderColorIndex;

            ICellStyle grandTotalPercentStyle = NpoiExcelExtensions.PercentStyle(workbook, GrandTotalColorIndex, true);
            grandTotalPercentStyle.BorderTop = BorderStyle.Medium;
            grandTotalPercentStyle.TopBorderColor = GrandTotalBorderColorIndex;

            IRow row = worksheet.CreateRow(currentRow);
            int currentColumn = 0;

            NpoiExcelExtensions.RenderCell(
                   ref worksheet,
                   ref row,
                   currentRow,
                   ref currentColumn,
                   currentRow,
                   currentColumn,
                   "COCK FIGHTING TOTAL",
                   grandTotalTextStyle);

            NpoiExcelExtensions.RenderCell(
               ref worksheet,
               ref row,
               currentRow,
               ref currentColumn,
               currentRow,
               currentColumn,
               grandTotal.WinLoss,
               grandTotalDecimalNumberStyle);

            NpoiExcelExtensions.RenderCell(
               ref worksheet,
               ref row,
               currentRow,
               ref currentColumn,
               currentRow,
               currentColumn,
               grandTotal.Position,
               grandTotalDecimalNumberStyle);

            NpoiExcelExtensions.RenderCell(
              ref worksheet,
              ref row,
              currentRow,
              ref currentColumn,
              currentRow,
              currentColumn,
              grandTotal.Commission,
              grandTotalDecimalNumberStyle);

            NpoiExcelExtensions.RenderCell(
               ref worksheet,
               ref row,
               currentRow,
               ref currentColumn,
               currentRow,
               currentColumn,
               grandTotal.VirtualCommission,
               grandTotalDecimalNumberStyle);

            NpoiExcelExtensions.RenderCell(
              ref worksheet,
              ref row,
              currentRow,
              ref currentColumn,
              currentRow,
              currentColumn,
              grandTotal.RealLineTotal,
              grandTotalDecimalNumberStyle);

            NpoiExcelExtensions.RenderCell(
               ref worksheet,
               ref row,
               currentRow,
               ref currentColumn,
               currentRow,
               currentColumn,
               grandTotal.VirtualLineTotal,
               grandTotalDecimalNumberStyle);

            NpoiExcelExtensions.RenderCell(
              ref worksheet,
              ref row,
              currentRow,
              ref currentColumn,
              currentRow,
              currentColumn,
              grandTotal.Variance,
              grandTotalDecimalNumberStyle);

            NpoiExcelExtensions.RenderCell(
               ref worksheet,
               ref row,
               currentRow,
               ref currentColumn,
               currentRow,
               currentColumn,
               grandTotal.RealTurnover,
               grandTotalDecimalNumberStyle);

            NpoiExcelExtensions.RenderCell(
               ref worksheet,
               ref row,
               currentRow,
               ref currentColumn,
               currentRow,
               currentColumn,
               grandTotal.Margin_Str,
               grandTotalPercentStyle);
        }

        private void InitCockFightingMatchByArenaColumnWith(ref ISheet worksheet)
        {
            for (int i = 0; i < 13; i++)
            {
                worksheet.SetColumnWidth(i, 170 * ExcelExtensions.FontWidth);
            }

            worksheet.SetColumnWidth(1, 220 * ExcelExtensions.FontWidth);
            worksheet.SetColumnWidth(2, 80 * ExcelExtensions.FontWidth);
            worksheet.SetColumnWidth(3, 220 * ExcelExtensions.FontWidth);
        }

        private void ExportCockFightingMatchByArenaReportTitle(
            HSSFWorkbook workbook,
            ref ISheet worksheet,
            ref int currentRow,
            GetCockFightingMatchByArena criteria)
        {
            ICellStyle headerStyle = NpoiExcelExtensions.HeaderStyle(workbook);
            int currentColumn = 0;
            string reportTitle = string.Format(
                   MainResources.CockFightingMatchByArena_ReportTitle.ToUpper().ToString(),
                   criteria.ArenaName,
                   criteria.WinLostDate.ToShortFormatedDate().ToUpper().ToString());
            IRow row = worksheet.CreateRow(currentRow);
            NpoiExcelExtensions.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow + 1, currentColumn + 12, reportTitle, headerStyle);
        }

        private void ExportCockFightingMatchByArenaReportHeader(
            HSSFWorkbook workbook,
            ref ISheet worksheet,
            ref int currentRow)
        {
            int currentColumn = 0;
            IRow row = worksheet.CreateRow(currentRow);
            ICellStyle headerStyle = NpoiExcelExtensions.HeaderStyle(workbook);
            NpoiExcelExtensions.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow, currentColumn, Labels.Fight_No, headerStyle);
            NpoiExcelExtensions.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow, currentColumn + 2, Labels.Match, headerStyle);
            NpoiExcelExtensions.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow, currentColumn, Labels.Win_Loss, headerStyle);
            NpoiExcelExtensions.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow, currentColumn, Labels.Position, headerStyle);
            NpoiExcelExtensions.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow, currentColumn, Labels.Comm, headerStyle);
            NpoiExcelExtensions.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow, currentColumn, Labels.Virtual_Comm, headerStyle);
            NpoiExcelExtensions.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow, currentColumn, Labels.Line_Total, headerStyle);
            NpoiExcelExtensions.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow, currentColumn, Labels.Virtual_Line_Total, headerStyle);
            NpoiExcelExtensions.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow, currentColumn, Labels.Variance, headerStyle);
            NpoiExcelExtensions.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow, currentColumn, Labels.Turnover, headerStyle);
            NpoiExcelExtensions.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow, currentColumn, Labels.Margin_Percent, headerStyle);
        }

        private void ExportCockFightingMatchByArenaReportContent(
            HSSFWorkbook workbook,
            ref ISheet worksheet,
            ref int currentRow,
            List<CockFightMatchDetail> reportContent)
        {
            ICellStyle textStyle = NpoiExcelExtensions.TextStyle(workbook, HSSFColor.White.Index, false, NpoiExcelExtensions.Alignment.Center);
            ICellStyle leftTextStyle = NpoiExcelExtensions.TextStyle(workbook, HSSFColor.White.Index, false, NpoiExcelExtensions.Alignment.Left);
            ICellStyle rightTextStyle = NpoiExcelExtensions.TextStyle(workbook, HSSFColor.White.Index, false, NpoiExcelExtensions.Alignment.Right);
            ICellStyle decimalNumberStyle = NpoiExcelExtensions.DecimalStyle(workbook, HSSFColor.White.Index, false);
            ICellStyle percentStyle = NpoiExcelExtensions.PercentStyle(workbook, HSSFColor.White.Index, false);
            IRow row = null;
            int currentColumn = 0;

            foreach (var item in reportContent)
            {
                row = worksheet.CreateRow(currentRow);
                currentColumn = 0;

                NpoiExcelExtensions.RenderCell(
                    ref worksheet,
                    ref row,
                    currentRow,
                    ref currentColumn,
                    currentRow,
                    currentColumn,
                    item.FightNo,
                    textStyle);

                NpoiExcelExtensions.RenderCell(
                   ref worksheet,
                   ref row,
                   currentRow,
                   ref currentColumn,
                   currentRow,
                   currentColumn,
                   item.HomeOwner,
                   rightTextStyle);

                NpoiExcelExtensions.RenderCell(
                   ref worksheet,
                   ref row,
                   currentRow,
                   ref currentColumn,
                   currentRow,
                   currentColumn,
                   Labels.Vs,
                   textStyle);

                NpoiExcelExtensions.RenderCell(
                   ref worksheet,
                   ref row,
                   currentRow,
                   ref currentColumn,
                   currentRow,
                   currentColumn,
                   item.AwayOwner,
                   leftTextStyle);

                NpoiExcelExtensions.RenderCell(
                   ref worksheet,
                   ref row,
                   currentRow,
                   ref currentColumn,
                   currentRow,
                   currentColumn,
                   item.WinLoss,
                   decimalNumberStyle);

                NpoiExcelExtensions.RenderCell(
                   ref worksheet,
                   ref row,
                   currentRow,
                   ref currentColumn,
                   currentRow,
                   currentColumn,
                   item.Position,
                   decimalNumberStyle);

                NpoiExcelExtensions.RenderCell(
                  ref worksheet,
                  ref row,
                  currentRow,
                  ref currentColumn,
                  currentRow,
                  currentColumn,
                  item.Commission,
                  decimalNumberStyle);

                NpoiExcelExtensions.RenderCell(
                   ref worksheet,
                   ref row,
                   currentRow,
                   ref currentColumn,
                   currentRow,
                   currentColumn,
                   item.VirtualCommission,
                   decimalNumberStyle);

                NpoiExcelExtensions.RenderCell(
                  ref worksheet,
                  ref row,
                  currentRow,
                  ref currentColumn,
                  currentRow,
                  currentColumn,
                  item.RealLineTotal,
                  decimalNumberStyle);

                NpoiExcelExtensions.RenderCell(
                   ref worksheet,
                   ref row,
                   currentRow,
                   ref currentColumn,
                   currentRow,
                   currentColumn,
                   item.VirtualLineTotal,
                   decimalNumberStyle);

                NpoiExcelExtensions.RenderCell(
                  ref worksheet,
                  ref row,
                  currentRow,
                  ref currentColumn,
                  currentRow,
                  currentColumn,
                  item.Variance,
                  decimalNumberStyle);

                NpoiExcelExtensions.RenderCell(
                   ref worksheet,
                   ref row,
                   currentRow,
                   ref currentColumn,
                   currentRow,
                   currentColumn,
                   item.RealTurnover,
                   decimalNumberStyle);

                NpoiExcelExtensions.RenderCell(
                   ref worksheet,
                   ref row,
                   currentRow,
                   ref currentColumn,
                   currentRow,
                   currentColumn,
                   item.Margin_Str,
                   percentStyle);

                currentRow++;
            }
        }

        private void ExportCockFightingMatchByArenaGrandTotal(
            HSSFWorkbook workbook,
            ref ISheet worksheet,
            ref int currentRow,
            CockFightMatchDetail grandTotal,
            GetCockFightingMatchByArena criteria)
        {
            ICellStyle grandTotalTextStyle = NpoiExcelExtensions.TextStyle(workbook, GrandTotalColorIndex, true, NpoiExcelExtensions.Alignment.Right);
            grandTotalTextStyle.BorderTop = BorderStyle.Medium;
            grandTotalTextStyle.TopBorderColor = GrandTotalBorderColorIndex;

            ICellStyle grandTotalDecimalNumberStyle = NpoiExcelExtensions.DecimalStyle(workbook, GrandTotalColorIndex, true);
            grandTotalDecimalNumberStyle.BorderTop = BorderStyle.Medium;
            grandTotalDecimalNumberStyle.TopBorderColor = GrandTotalBorderColorIndex;

            ICellStyle grandTotalPercentStyle = NpoiExcelExtensions.PercentStyle(workbook, GrandTotalColorIndex, true);
            grandTotalPercentStyle.BorderTop = BorderStyle.Medium;
            grandTotalPercentStyle.TopBorderColor = GrandTotalBorderColorIndex;

            IRow row = worksheet.CreateRow(currentRow);
            int currentColumn = 0;

            NpoiExcelExtensions.RenderCell(
                   ref worksheet,
                   ref row,
                   currentRow,
                   ref currentColumn,
                   currentRow,
                   currentColumn + 3,
                   string.Format("{0} TOTAL", criteria.ArenaName),
                   grandTotalTextStyle);

            NpoiExcelExtensions.RenderCell(
               ref worksheet,
               ref row,
               currentRow,
               ref currentColumn,
               currentRow,
               currentColumn,
               grandTotal.WinLoss,
               grandTotalDecimalNumberStyle);

            NpoiExcelExtensions.RenderCell(
               ref worksheet,
               ref row,
               currentRow,
               ref currentColumn,
               currentRow,
               currentColumn,
               grandTotal.Position,
               grandTotalDecimalNumberStyle);

            NpoiExcelExtensions.RenderCell(
              ref worksheet,
              ref row,
              currentRow,
              ref currentColumn,
              currentRow,
              currentColumn,
              grandTotal.Commission,
              grandTotalDecimalNumberStyle);

            NpoiExcelExtensions.RenderCell(
               ref worksheet,
               ref row,
               currentRow,
               ref currentColumn,
               currentRow,
               currentColumn,
               grandTotal.VirtualCommission,
               grandTotalDecimalNumberStyle);

            NpoiExcelExtensions.RenderCell(
              ref worksheet,
              ref row,
              currentRow,
              ref currentColumn,
              currentRow,
              currentColumn,
              grandTotal.RealLineTotal,
              grandTotalDecimalNumberStyle);

            NpoiExcelExtensions.RenderCell(
               ref worksheet,
               ref row,
               currentRow,
               ref currentColumn,
               currentRow,
               currentColumn,
               grandTotal.VirtualLineTotal,
               grandTotalDecimalNumberStyle);

            NpoiExcelExtensions.RenderCell(
              ref worksheet,
              ref row,
              currentRow,
              ref currentColumn,
              currentRow,
              currentColumn,
              grandTotal.Variance,
              grandTotalDecimalNumberStyle);

            NpoiExcelExtensions.RenderCell(
               ref worksheet,
               ref row,
               currentRow,
               ref currentColumn,
               currentRow,
               currentColumn,
               grandTotal.RealTurnover,
               grandTotalDecimalNumberStyle);

            NpoiExcelExtensions.RenderCell(
               ref worksheet,
               ref row,
               currentRow,
               ref currentColumn,
               currentRow,
               currentColumn,
               grandTotal.Margin_Str,
               grandTotalPercentStyle);
        }
    }
}