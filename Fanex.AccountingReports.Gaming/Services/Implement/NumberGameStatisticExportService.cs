namespace Fanex.AccountingReports.Gaming.Services.Implement
{
    using System.Collections.Generic;
    using System.Linq;
    using Fanex.AccountingReports.Core.Extensions;
    using Fanex.AccountingReports.Core.Web;
    using Fanex.AccountingReports.Gaming.Constants;
    using Fanex.AccountingReports.Gaming.Criteria.NumberGameStatistic;
    using Fanex.AccountingReports.Gaming.Models.NumberGameStatistic;
    using Fanex.AccountingReports.Gaming.Resources;
    using Fanex.AccountingReports.Gaming.Services.Interface;
    using NPOI.HSSF.UserModel;
    using NPOI.HSSF.Util;
    using NPOI.SS.UserModel;

    public class NumberGameStatisticExportService : INumberGameStatisticExportService
    {
        private const short EvenRowColorIndex = 37;
        private const short GrandTotalColorIndex = 41;
        private const short GrandTotalBorderColorIndex = 47;
        private const short HighLightColorIndex = 49;

        private const int RowsPerSheet = 30000;

        private ICellStyle headerStyle;
        private ICellStyle textStyle;
        private ICellStyle intNumberStyle;
        private ICellStyle decimalNumberStyle;
        private ICellStyle decimalNegativeNumberStyle;
        private ICellStyle percentStyle;
        private ICellStyle moreThanHundressPercentStyle;
        private ICellStyle evenRowTextStyle;
        private ICellStyle evenRowIntNumberStyle;
        private ICellStyle evenRowDecimalNumberStyle;
        private ICellStyle evenRowDecimalNegativeNumberStyle;
        private ICellStyle evenRowPercentStyle;
        private ICellStyle moreThanHundressEvenRowPercentStyle;

        private ICellStyle grandTotalTextStyle;
        private ICellStyle grandTotalIntNumberStyle;
        private ICellStyle grandTotalDecimalNumberStyle;
        private ICellStyle grandTotalDecimalNegativeNumberStyle;
        private ICellStyle grandTotalHighLightDecimalNegativeNumberStyle;
        private ICellStyle grandTotalPercentStyle;
        private ICellStyle moreThanHundressGrandTotalPercentStyle;

        private INumberGameStatisticService _service;

        public NumberGameStatisticExportService()
            : this(null)
        {
        }

        public NumberGameStatisticExportService(INumberGameStatisticService service)
        {
            this._service = service ?? new NumberGameStatisticService();
        }

        public void ExportReportByDaily(GetNumberGameStatisticCriteria criteria)
        {
            var report = CacheManager.Get<NumberGameStatisticByDaily>(GamingConstants.NumberGameStatisticReportByDailyCachedDataKey);

            if (report == null)
            {
                report = _service.GetNumberGameStatisticByDaily(criteria);
            }

            HSSFWorkbook workbook = new HSSFWorkbook();
            this.SetColorAtIndex(workbook);
            this.InitCellStyle(workbook);

            string sheetName = GamingResources.NumberGameStatistic_SheetName;
            ISheet worksheet = workbook.CreateSheet(sheetName);

            this.InitColumnWith(ref worksheet);
            int currentRow = 0;
            this.ExportReportTitle(workbook, ref worksheet, ref currentRow, criteria);
            currentRow += 3;
            this.ExportReportByDailyHeader(ref worksheet, ref currentRow);
            currentRow++;
            this.ExportReportByDailyContent(ref worksheet, ref currentRow, report.ReportRows);
            this.ExportReportByDailyGrandTotal(ref worksheet, ref currentRow, report.GrandTotal);

            string reportName = this.CreateReportName(criteria);
            NpoiExcelExtensions.ResponseStreamExcel(workbook, reportName);
        }

        public void ExportReportByUserName(GetNumberGameStatisticCriteria criteria)
        {
            var report = CacheManager.Get<NumberGameStatisticByUserName>(GamingConstants.NumberGameStatisticReportByUserNameCachedDataKey);

            if (report == null)
            {
                report = _service.GetNumberGameStatisticFullReportByUserName(criteria);
            }

            HSSFWorkbook workbook = new HSSFWorkbook();
            this.SetColorAtIndex(workbook);
            this.InitCellStyle(workbook);

            int sheetIndex = 0;
            int sheetZise = RowsPerSheet;
            int totalSheet = (report.ReportRows.Count / sheetZise) + (report.ReportRows.Count % sheetZise > 0 ? 1 : 0);

            for (sheetIndex = 0; sheetIndex < totalSheet; sheetIndex++)
            {
                var rows = report.ReportRows.Skip(sheetIndex * sheetZise).Take(sheetZise).ToList();

                string sheetName = GamingResources.NumberGameStatistic_SheetNameByUser;
                if (totalSheet > 1)
                {
                    sheetName = string.Format(sheetName, "(" + (sheetIndex + 1).ToString() + ")");
                }
                else
                {
                    sheetName = string.Format(sheetName, string.Empty);
                }

                ISheet worksheet = workbook.CreateSheet(sheetName);

                this.InitColumnWith(ref worksheet);
                int currentRow = 0;
                this.ExportReportTitle(workbook, ref worksheet, ref currentRow, criteria);
                currentRow += 3;
                this.ExportReportByUserNameHeader(ref worksheet, ref currentRow);
                currentRow++;
                this.ExportReportByUserNameContent(ref worksheet, ref currentRow, rows);

                if (sheetIndex == totalSheet - 1)
                {
                    this.ExportReportByUserNameGrandTotal(ref worksheet, ref currentRow, report.GrandTotal);
                }
            }           

            string reportName = this.CreateReportName(criteria);
            NpoiExcelExtensions.ResponseStreamExcel(workbook, reportName);
        }

        public void ExportReportByLeague(GetNumberGameStatisticCriteria criteria)
        {
            var report = CacheManager.Get<NumberGameStatisticByLeague>(GamingConstants.NumberGameStatisticReportByLeagueCachedDataKey);

            if (report == null)
            {
                report = _service.GetNumberGameStatisticByLeague(criteria);
            }

            HSSFWorkbook workbook = new HSSFWorkbook();
            this.SetColorAtIndex(workbook);
            this.InitCellStyle(workbook);

            string sheetName = GamingResources.NumberGameStatistic_SheetName;
            ISheet worksheet = workbook.CreateSheet(sheetName);

            this.InitColumnWith(ref worksheet);
            int currentRow = 0;
            this.ExportReportTitle(workbook, ref worksheet, ref currentRow, criteria);
            currentRow += 3;
            this.ExportReportByLeagueHeader(ref worksheet, ref currentRow);
            currentRow++;
            this.ExportReportByLeagueContent(ref worksheet, ref currentRow, report.ReportRows);
            this.ExportReportByLeagueGrandTotal(ref worksheet, ref currentRow, report.GrandTotal);

            string reportName = this.CreateReportName(criteria);
            NpoiExcelExtensions.ResponseStreamExcel(workbook, reportName);
        }

        public void ExportReportByBettype(GetNumberGameStatisticCriteria criteria)
        {
            var report = CacheManager.Get<NumberGameStatisticByBettype>(GamingConstants.NumberGameStatisticReportByBettypeCachedDataKey);

            if (report == null)
            {
                report = _service.GetNumberGameStatisticByBettype(criteria);
            }

            HSSFWorkbook workbook = new HSSFWorkbook();
            this.SetColorAtIndex(workbook);
            this.InitCellStyle(workbook);

            string sheetName = GamingResources.NumberGameStatistic_SheetName;
            ISheet worksheet = workbook.CreateSheet(sheetName);

            this.InitColumnWith(ref worksheet);
            int currentRow = 0;
            this.ExportReportTitle(workbook, ref worksheet, ref currentRow, criteria);
            currentRow += 3;
            this.ExportReportByBettypeHeader(ref worksheet, ref currentRow);
            currentRow++;
            this.ExportReportByBettypeContent(ref worksheet, ref currentRow, report.ReportRows);
            this.ExportReportByBettypeGrandTotal(ref worksheet, ref currentRow, report.GrandTotal);

            string reportName = this.CreateReportName(criteria);
            NpoiExcelExtensions.ResponseStreamExcel(workbook, reportName);
        }

        private string CreateReportName(GetNumberGameStatisticCriteria criteria)
        {
            return string.Format(
                   GamingResources.NumberGameStatistic_ReportName,
                   criteria.FromDate.ToShortFormatedDateWithSlash(),
                   criteria.ToDate.ToShortFormatedDateWithSlash());
        }

        private void SetColorAtIndex(HSSFWorkbook excelFile)
        {
            HSSFPalette palette = excelFile.GetCustomPalette();
            palette.SetColorAtIndex(EvenRowColorIndex, (byte)228, (byte)242, (byte)253);
            palette.SetColorAtIndex(GrandTotalColorIndex, (byte)219, (byte)225, (byte)187);
            palette.SetColorAtIndex(GrandTotalBorderColorIndex, (byte)255, (byte)102, (byte)0);
            palette.SetColorAtIndex(HighLightColorIndex, (byte)181, (byte)196, (byte)208);
        }

        private void InitCellStyle(HSSFWorkbook workbook)
        {
            InitHeaderStyle(workbook);
            InitBodyStyle(workbook);
            InitGrandTotalStyle(workbook);
        }

        private void InitHeaderStyle(HSSFWorkbook workbook)
        {
            headerStyle = NpoiExcelExtensions.HeaderStyle(workbook);
        }

        private void InitBodyStyle(HSSFWorkbook workbook)
        {
            textStyle = NpoiExcelExtensions.TextStyle(workbook, HSSFColor.White.Index, false, NpoiExcelExtensions.Alignment.Center);
            intNumberStyle = NpoiExcelExtensions.IntStyle(workbook, HSSFColor.White.Index, false, NpoiExcelExtensions.Alignment.Right);
            decimalNumberStyle = NpoiExcelExtensions.DecimalStyle(workbook, HSSFColor.White.Index, false);
            decimalNegativeNumberStyle = NpoiExcelExtensions.NegativeDecimalStyle(workbook, HSSFColor.White.Index, false);
            percentStyle = NpoiExcelExtensions.PercentStyle(workbook, HSSFColor.White.Index, false);
            moreThanHundressPercentStyle = NpoiExcelExtensions.PercentStyle(workbook, HSSFColor.White.Index, false, HSSFColor.Red.Index);

            evenRowTextStyle = NpoiExcelExtensions.TextStyle(workbook, EvenRowColorIndex, false, NpoiExcelExtensions.Alignment.Center);
            evenRowIntNumberStyle = NpoiExcelExtensions.IntStyle(workbook, EvenRowColorIndex, false, NpoiExcelExtensions.Alignment.Right);
            evenRowDecimalNumberStyle = NpoiExcelExtensions.DecimalStyle(workbook, EvenRowColorIndex, false);
            evenRowDecimalNegativeNumberStyle = NpoiExcelExtensions.NegativeDecimalStyle(workbook, EvenRowColorIndex, false);
            evenRowPercentStyle = NpoiExcelExtensions.PercentStyle(workbook, EvenRowColorIndex, false);
            moreThanHundressEvenRowPercentStyle = NpoiExcelExtensions.PercentStyle(workbook, EvenRowColorIndex, false, HSSFColor.Red.Index);
        }

        private void InitGrandTotalStyle(HSSFWorkbook workbook)
        {
            grandTotalTextStyle = NpoiExcelExtensions.TextStyle(workbook, GrandTotalColorIndex, true, NpoiExcelExtensions.Alignment.Center);
            grandTotalTextStyle.BorderTop = BorderStyle.Medium;
            grandTotalTextStyle.TopBorderColor = GrandTotalBorderColorIndex;

            grandTotalIntNumberStyle = NpoiExcelExtensions.IntStyle(workbook, GrandTotalColorIndex, true, NpoiExcelExtensions.Alignment.Right);
            grandTotalIntNumberStyle.BorderTop = BorderStyle.Medium;
            grandTotalIntNumberStyle.TopBorderColor = GrandTotalBorderColorIndex;

            grandTotalDecimalNumberStyle = NpoiExcelExtensions.DecimalStyle(workbook, GrandTotalColorIndex, true);
            grandTotalDecimalNumberStyle.BorderTop = BorderStyle.Medium;
            grandTotalDecimalNumberStyle.TopBorderColor = GrandTotalBorderColorIndex;

            grandTotalDecimalNegativeNumberStyle = NpoiExcelExtensions.NegativeDecimalStyle(workbook, GrandTotalColorIndex, true);
            grandTotalDecimalNegativeNumberStyle.BorderTop = BorderStyle.Medium;
            grandTotalDecimalNegativeNumberStyle.TopBorderColor = GrandTotalBorderColorIndex;

            grandTotalHighLightDecimalNegativeNumberStyle = NpoiExcelExtensions.NegativeDecimalStyle(workbook, HighLightColorIndex, true);
            grandTotalHighLightDecimalNegativeNumberStyle.BorderTop = BorderStyle.Medium;
            grandTotalHighLightDecimalNegativeNumberStyle.TopBorderColor = GrandTotalBorderColorIndex;

            grandTotalPercentStyle = NpoiExcelExtensions.PercentStyle(workbook, GrandTotalColorIndex, true);
            grandTotalPercentStyle.BorderTop = BorderStyle.Medium;
            grandTotalPercentStyle.TopBorderColor = GrandTotalBorderColorIndex;

            moreThanHundressGrandTotalPercentStyle = NpoiExcelExtensions.PercentStyle(workbook, GrandTotalColorIndex, true, HSSFColor.Red.Index);
            moreThanHundressGrandTotalPercentStyle.BorderTop = BorderStyle.Medium;
            moreThanHundressGrandTotalPercentStyle.TopBorderColor = GrandTotalBorderColorIndex;
        }

        private void InitColumnWith(ref ISheet worksheet)
        {
            for (int i = 0; i < 9; i++)
            {
                worksheet.SetColumnWidth(i, 170 * ExcelExtensions.FontWidth);
            }
        }

        private void ExportReportTitle(
            HSSFWorkbook workbook,
            ref ISheet worksheet,
            ref int currentRow,
            GetNumberGameStatisticCriteria criteria)
        {
            ICellStyle headerStyle = NpoiExcelExtensions.HeaderStyle(workbook);
            int currentColumn = 0;
            string reportTitle = string.Format(
                   GamingResources.NumberGameStatistic_ReportTitle,
                   criteria.FromDate.ToShortFormatedDate(),
                   criteria.ToDate.ToShortFormatedDate());
            IRow row = worksheet.CreateRow(currentRow);
            NpoiExcelExtensions.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow + 1, currentColumn + 8, reportTitle, headerStyle);
        }

        private void ExportReportByDailyHeader(
            ref ISheet worksheet,
            ref int currentRow)
        {
            int currentColumn = 0;
            IRow row = worksheet.GetRow(currentRow) ?? worksheet.CreateRow(currentRow);
            NpoiExcelExtensions.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow, currentColumn, Labels.Date, headerStyle);
            NpoiExcelExtensions.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow, currentColumn, Labels.No_Players, headerStyle);
            this.ExportNumberGameStatisticDetailHeader(ref worksheet, ref currentRow);
        }

        private void ExportReportByUserNameHeader(
            ref ISheet worksheet,
            ref int currentRow)
        {
            int currentColumn = 0;
            IRow row = worksheet.GetRow(currentRow) ?? worksheet.CreateRow(currentRow);
            NpoiExcelExtensions.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow, currentColumn, Labels.User_Name, headerStyle);
            NpoiExcelExtensions.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow, currentColumn, Labels.Player_ID, headerStyle);
            this.ExportNumberGameStatisticDetailHeader(ref worksheet, ref currentRow);
        }

        private void ExportReportByLeagueHeader(
            ref ISheet worksheet,
            ref int currentRow)
        {
            int currentColumn = 0;
            IRow row = worksheet.GetRow(currentRow) ?? worksheet.CreateRow(currentRow);
            NpoiExcelExtensions.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow, currentColumn, Labels.League, headerStyle);
            NpoiExcelExtensions.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow, currentColumn, Labels.No_Players, headerStyle);
            this.ExportNumberGameStatisticDetailHeader(ref worksheet, ref currentRow);
        }

        private void ExportReportByBettypeHeader(
           ref ISheet worksheet,
           ref int currentRow)
        {
            int currentColumn = 0;
            IRow row = worksheet.GetRow(currentRow) ?? worksheet.CreateRow(currentRow);
            NpoiExcelExtensions.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow, currentColumn, Labels.Bettype, headerStyle);
            NpoiExcelExtensions.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow, currentColumn, Labels.No_Players, headerStyle);
            this.ExportNumberGameStatisticDetailHeader(ref worksheet, ref currentRow);
        }

        private void ExportNumberGameStatisticDetailHeader(
            ref ISheet worksheet,
            ref int currentRow)
        {
            int currentColumn = 2;
            IRow row = worksheet.GetRow(currentRow) ?? worksheet.CreateRow(currentRow);
            NpoiExcelExtensions.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow, currentColumn, Labels.Bet_Count, headerStyle);
            NpoiExcelExtensions.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow, currentColumn, Labels.Total_Bets, headerStyle);
            NpoiExcelExtensions.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow, currentColumn, Labels.Total_Wins, headerStyle);
            NpoiExcelExtensions.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow, currentColumn, Labels.GameInCome, headerStyle);
            NpoiExcelExtensions.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow, currentColumn, Labels.GamePayout, headerStyle);
            NpoiExcelExtensions.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow, currentColumn, Labels.Company_Turnover, headerStyle);
            NpoiExcelExtensions.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow, currentColumn, Labels.CompanyNetInCome, headerStyle);
        }

        private void ExportReportByDailyContent(
           ref ISheet worksheet,
           ref int currentRow,
           List<NumberGameStatisticByDailyDetail> reportContent)
        {
            IRow row = null;
            int currentColumn = 0;

            foreach (var currentItem in reportContent)
            {
                row = worksheet.GetRow(currentRow) ?? worksheet.CreateRow(currentRow);
                currentColumn = 0;
                NpoiExcelExtensions.RenderCell(
                    ref worksheet,
                    ref row,
                    currentRow,
                    ref currentColumn,
                    currentRow,
                    currentColumn,
                    currentItem.Date,
                    currentRow % 2 == 0 ? textStyle : evenRowTextStyle);

                NpoiExcelExtensions.RenderCell(
                   ref worksheet,
                   ref row,
                   currentRow,
                   ref currentColumn,
                   currentRow,
                   currentColumn,
                   currentItem.Players,
                   currentRow % 2 == 0 ? intNumberStyle : evenRowIntNumberStyle);

                this.ExportNumberGameStatisticDetail(ref worksheet, currentRow, currentItem.NumberGameStatisticDetail);
                currentRow++;
            }
        }

        private void ExportReportByUserNameContent(
           ref ISheet worksheet,
           ref int currentRow,
           List<NumberGameStatisticByUserNameDetail> reportContent)
        {
            IRow row = null;
            int currentColumn = 0;

            foreach (var currentItem in reportContent)
            {
                row = worksheet.GetRow(currentRow) ?? worksheet.CreateRow(currentRow);
                currentColumn = 0;
                NpoiExcelExtensions.RenderCell(
                    ref worksheet,
                    ref row,
                    currentRow,
                    ref currentColumn,
                    currentRow,
                    currentColumn,
                    currentItem.UserName,
                    currentRow % 2 == 0 ? textStyle : evenRowTextStyle);

                NpoiExcelExtensions.RenderCell(
                   ref worksheet,
                   ref row,
                   currentRow,
                   ref currentColumn,
                   currentRow,
                   currentColumn,
                   currentItem.CustId,
                   currentRow % 2 == 0 ? textStyle : evenRowTextStyle);

                this.ExportNumberGameStatisticDetail(ref worksheet, currentRow, currentItem.NumberGameStatisticDetail);
                currentRow++;
            }
        }

        private void ExportReportByLeagueContent(
            ref ISheet worksheet,
            ref int currentRow,
            List<NumberGameStatisticByLeagueDetail> reportContent)
        {
            IRow row = null;
            int currentColumn = 0;

            foreach (var currentItem in reportContent)
            {
                row = worksheet.GetRow(currentRow) ?? worksheet.CreateRow(currentRow);
                currentColumn = 0;
                NpoiExcelExtensions.RenderCell(
                    ref worksheet,
                    ref row,
                    currentRow,
                    ref currentColumn,
                    currentRow,
                    currentColumn,
                    currentItem.LeagueName,
                    currentRow % 2 == 0 ? textStyle : evenRowTextStyle);

                NpoiExcelExtensions.RenderCell(
                   ref worksheet,
                   ref row,
                   currentRow,
                   ref currentColumn,
                   currentRow,
                   currentColumn,
                   currentItem.Players,
                   currentRow % 2 == 0 ? intNumberStyle : evenRowIntNumberStyle);

                this.ExportNumberGameStatisticDetail(ref worksheet, currentRow, currentItem.NumberGameStatisticDetail);
                currentRow++;
            }
        }

        private void ExportReportByBettypeContent(
           ref ISheet worksheet,
           ref int currentRow,
           List<NumberGameStatisticByBettypeDetail> reportContent)
        {
            IRow row = null;
            int currentColumn = 0;

            foreach (var currentItem in reportContent)
            {
                row = worksheet.GetRow(currentRow) ?? worksheet.CreateRow(currentRow);
                currentColumn = 0;
                NpoiExcelExtensions.RenderCell(
                    ref worksheet,
                    ref row,
                    currentRow,
                    ref currentColumn,
                    currentRow,
                    currentColumn,
                    currentItem.Bettype,
                    currentRow % 2 == 0 ? textStyle : evenRowTextStyle);

                NpoiExcelExtensions.RenderCell(
                   ref worksheet,
                   ref row,
                   currentRow,
                   ref currentColumn,
                   currentRow,
                   currentColumn,
                   currentItem.Players,
                   currentRow % 2 == 0 ? intNumberStyle : evenRowIntNumberStyle);

                this.ExportNumberGameStatisticDetail(ref worksheet, currentRow, currentItem.NumberGameStatisticDetail);
                currentRow++;
            }
        }

        private void ExportNumberGameStatisticDetail(ref ISheet worksheet, int currentRow, NumberGameStatisticDetail item)
        {
            int currentColumn = 2;
            IRow row = worksheet.GetRow(currentRow) ?? worksheet.CreateRow(currentRow);

            NpoiExcelExtensions.RenderCell(
                   ref worksheet,
                   ref row,
                   currentRow,
                   ref currentColumn,
                   currentRow,
                   currentColumn,
                   item.BetCount,
                   currentRow % 2 == 0 ? intNumberStyle : evenRowIntNumberStyle);

            NpoiExcelExtensions.RenderCell(
              ref worksheet,
              ref row,
              currentRow,
              ref currentColumn,
              currentRow,
              currentColumn,
              item.TotalBets,
              currentRow % 2 == 0 ? decimalNumberStyle : evenRowDecimalNumberStyle);

            NpoiExcelExtensions.RenderCell(
               ref worksheet,
               ref row,
               currentRow,
               ref currentColumn,
               currentRow,
               currentColumn,
               item.TotalWins,
               currentRow % 2 == 0 ? decimalNegativeNumberStyle : evenRowDecimalNegativeNumberStyle);

            NpoiExcelExtensions.RenderCell(
              ref worksheet,
              ref row,
              currentRow,
              ref currentColumn,
              currentRow,
              currentColumn,
              item.GameIncome,
              currentRow % 2 == 0 ? decimalNegativeNumberStyle : evenRowDecimalNegativeNumberStyle);

            if (item.GamePayout > 1)
            {
                NpoiExcelExtensions.RenderCell(
                  ref worksheet,
                  ref row,
                  currentRow,
                  ref currentColumn,
                  currentRow,
                  currentColumn,
                  item.GamePayout,
                  currentRow % 2 == 0 ? moreThanHundressPercentStyle : moreThanHundressEvenRowPercentStyle);
            }
            else
            {
                NpoiExcelExtensions.RenderCell(
                  ref worksheet,
                  ref row,
                  currentRow,
                  ref currentColumn,
                  currentRow,
                  currentColumn,
                  item.GamePayout,
                  currentRow % 2 == 0 ? percentStyle : evenRowPercentStyle);
            }

            NpoiExcelExtensions.RenderCell(
              ref worksheet,
              ref row,
              currentRow,
              ref currentColumn,
              currentRow,
              currentColumn,
              item.CompanyTurnover,
              currentRow % 2 == 0 ? decimalNumberStyle : evenRowDecimalNumberStyle);

            NpoiExcelExtensions.RenderCell(
               ref worksheet,
               ref row,
               currentRow,
               ref currentColumn,
               currentRow,
               currentColumn,
               item.NetIncome,
               currentRow % 2 == 0 ? decimalNegativeNumberStyle : evenRowDecimalNegativeNumberStyle);
        }

        private void ExportReportByDailyGrandTotal(
            ref ISheet worksheet,
            ref int currentRow,
            NumberGameStatisticByDailyDetail grandTotal)
        {
            IRow row = worksheet.GetRow(currentRow) ?? worksheet.CreateRow(currentRow);
            int currentColumn = 0;

            NpoiExcelExtensions.RenderCell(
                   ref worksheet,
                   ref row,
                   currentRow,
                   ref currentColumn,
                   currentRow,
                   currentColumn,
                   Labels.Grand_Total,
                   grandTotalTextStyle);

            NpoiExcelExtensions.RenderCell(
               ref worksheet,
               ref row,
               currentRow,
               ref currentColumn,
               currentRow,
               currentColumn,
               grandTotal.Players,
               grandTotalIntNumberStyle);

            this.ExportNumberGameStatisticGrandTotalDetail(ref worksheet, currentRow, grandTotal.NumberGameStatisticDetail);
        }

        private void ExportReportByUserNameGrandTotal(
            ref ISheet worksheet,
            ref int currentRow,
            NumberGameStatisticByUserNameDetail grandTotal)
        {
            IRow row = worksheet.GetRow(currentRow) ?? worksheet.CreateRow(currentRow);
            int currentColumn = 0;

            NpoiExcelExtensions.RenderCell(
                   ref worksheet,
                   ref row,
                   currentRow,
                   ref currentColumn,
                   currentRow,
                   currentColumn,
                   Labels.Grand_Total,
                   grandTotalTextStyle);

            NpoiExcelExtensions.RenderCell(
               ref worksheet,
               ref row,
               currentRow,
               ref currentColumn,
               currentRow,
               currentColumn,
               string.Empty,
               grandTotalTextStyle);

            this.ExportNumberGameStatisticGrandTotalDetail(ref worksheet, currentRow, grandTotal.NumberGameStatisticDetail);
        }

        private void ExportReportByLeagueGrandTotal(
            ref ISheet worksheet,
            ref int currentRow,
            NumberGameStatisticByLeagueDetail grandTotal)
        {
            IRow row = worksheet.GetRow(currentRow) ?? worksheet.CreateRow(currentRow);
            int currentColumn = 0;

            NpoiExcelExtensions.RenderCell(
                   ref worksheet,
                   ref row,
                   currentRow,
                   ref currentColumn,
                   currentRow,
                   currentColumn,
                   Labels.Grand_Total,
                   grandTotalTextStyle);

            NpoiExcelExtensions.RenderCell(
               ref worksheet,
               ref row,
               currentRow,
               ref currentColumn,
               currentRow,
               currentColumn,
               grandTotal.Players,
               grandTotalIntNumberStyle);

            this.ExportNumberGameStatisticGrandTotalDetail(ref worksheet, currentRow, grandTotal.NumberGameStatisticDetail);
        }

        private void ExportReportByBettypeGrandTotal(
            ref ISheet worksheet,
            ref int currentRow,
            NumberGameStatisticByBettypeDetail grandTotal)
        {
            IRow row = worksheet.GetRow(currentRow) ?? worksheet.CreateRow(currentRow);
            int currentColumn = 0;

            NpoiExcelExtensions.RenderCell(
                   ref worksheet,
                   ref row,
                   currentRow,
                   ref currentColumn,
                   currentRow,
                   currentColumn,
                   Labels.Grand_Total,
                   grandTotalTextStyle);

            NpoiExcelExtensions.RenderCell(
               ref worksheet,
               ref row,
               currentRow,
               ref currentColumn,
               currentRow,
               currentColumn,
               grandTotal.Players,
               grandTotalIntNumberStyle);

            this.ExportNumberGameStatisticGrandTotalDetail(ref worksheet, currentRow, grandTotal.NumberGameStatisticDetail);
        }

        private void ExportNumberGameStatisticGrandTotalDetail(
            ref ISheet worksheet,
            int currentRow,
            NumberGameStatisticDetail item)
        {
            int currentColumn = 2;
            IRow row = worksheet.GetRow(currentRow) ?? worksheet.CreateRow(currentRow);

            NpoiExcelExtensions.RenderCell(
               ref worksheet,
               ref row,
               currentRow,
               ref currentColumn,
               currentRow,
               currentColumn,
               item.BetCount,
               grandTotalIntNumberStyle);

            NpoiExcelExtensions.RenderCell(
              ref worksheet,
              ref row,
              currentRow,
              ref currentColumn,
              currentRow,
              currentColumn,
              item.TotalBets,
              grandTotalDecimalNumberStyle);

            NpoiExcelExtensions.RenderCell(
               ref worksheet,
               ref row,
               currentRow,
               ref currentColumn,
               currentRow,
               currentColumn,
               item.TotalWins,
               grandTotalDecimalNegativeNumberStyle);

            NpoiExcelExtensions.RenderCell(
              ref worksheet,
              ref row,
              currentRow,
              ref currentColumn,
              currentRow,
              currentColumn,
              item.GameIncome,
              grandTotalHighLightDecimalNegativeNumberStyle);

            NpoiExcelExtensions.RenderCell(
               ref worksheet,
               ref row,
               currentRow,
               ref currentColumn,
               currentRow,
               currentColumn,
               item.GamePayout,
               item.GamePayout > 1 ? moreThanHundressGrandTotalPercentStyle : grandTotalPercentStyle);

            NpoiExcelExtensions.RenderCell(
              ref worksheet,
              ref row,
              currentRow,
              ref currentColumn,
              currentRow,
              currentColumn,
              item.CompanyTurnover,
              grandTotalDecimalNumberStyle);

            NpoiExcelExtensions.RenderCell(
               ref worksheet,
               ref row,
               currentRow,
               ref currentColumn,
               currentRow,
               currentColumn,
               item.NetIncome,
               grandTotalHighLightDecimalNegativeNumberStyle);
        }
    }
}