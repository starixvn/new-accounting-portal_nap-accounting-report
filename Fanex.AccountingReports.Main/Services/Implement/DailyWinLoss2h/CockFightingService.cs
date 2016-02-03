namespace Fanex.AccountingReports.Main.Services.Implement.DailyWinLoss2h
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using ExcelUtil.Spreadsheet;
    using Fanex.AccountingReports.Core.Constants;
    using Fanex.AccountingReports.Core.Extensions;
    using Fanex.AccountingReports.Core.Services.Impl;
    using Fanex.AccountingReports.Core.Web;
    using Fanex.AccountingReports.Main.Common;
    using Fanex.AccountingReports.Main.Criteria.DailyWinLoss2h;
    using Fanex.AccountingReports.Main.Models.DailyWinLoss2h;
    using Fanex.AccountingReports.Main.Resources;
    using Fanex.AccountingReports.Main.Services.Interface.DailyWinLoss2h;
    using Fanex.Common;
    using Fanex.Data.Repository;
    using NPOI.HSSF.UserModel;
    using NPOI.HSSF.Util;
    using NPOI.SS.UserModel;

    public class CockFightingService : ServiceBase, ICockFightingService
    {
        private const short ArenaColorIndex = 45;
        private const short GrandTotalColorIndex = 47;
        private const short GrandTotalBorderColorIndex = 49;

        private string _dailyWinLoss2hCacheCockFightingReportKey = "Fanex.AccountingReports.Main.DailyWinLoss2h.CockFightingReport" +
            (AppSession.CurrentUser != null ? AppSession.CurrentUser.Username : string.Empty);

        private string _dailyWinLoss2hCacheCockFightingArenaReportKey = "Fanex.AccountingReports.Main.DailyWinLoss2h.CockFightingArenaReport" +
            (AppSession.CurrentUser != null ? AppSession.CurrentUser.Username : string.Empty);

        private IDynamicRepository _repository;

        private DailyWinLoss2hUtilityService _dailyWinLoss2hUtilityService;

        public CockFightingService()
        {
            _dailyWinLoss2hUtilityService = new DailyWinLoss2hUtilityService();
            _repository = new DynamicRepository();
        }

        public WinLossInfo GetCockFightingReport(GetCockFightingCriteria criteria)
        {
            var rawData = this.GetReportData<dynamic>(criteria);
            WinLossInfo report = BuildCockFightingReport(rawData.ToList());
            CacheExtension.Set(_dailyWinLoss2hCacheCockFightingReportKey, report);
            return report;
        }

        public CockFightingArena GetCockFightingArenaReport(GetCockFightingArenaCriteria criteria)
        {
            var cockFightingArena = new CockFightingArena();
            var rawData = _repository.Fetch<CockFightingMatchDetail>(criteria).ToList();
            cockFightingArena.ReportRows = this.GetCockFightingArena(rawData);
            cockFightingArena.GrandTotal = this.GetCockFightingArenaGrandTotal(rawData);
            CacheExtension.Set(_dailyWinLoss2hCacheCockFightingArenaReportKey, cockFightingArena);
            return cockFightingArena;
        }

        public void ExportCockFightingReport(GetCockFightingCriteria criteria)
        {
            try
            {
                var report = SessionManager.Get<WinLossInfo>(_dailyWinLoss2hCacheCockFightingReportKey);
                if (report == null)
                {
                    report = this.GetCockFightingReport(criteria);
                }

                //// ExcelTypes

                ExcelFile excelFile = new ExcelFile();
                ExcelWorksheetCollection worksheets = excelFile.Worksheets;
                string sheetName = MainResources.Daily_WinLoss2h_Cockfighting_SheetName;
                ExcelWorksheet workSheet = worksheets.Add(sheetName);

                int startRow = 0;
                int startColumn = 0;
                int maxColumnIndex = 6;
                var currentRow = startRow;

                //// ExcelTypes
                //// MakeExcelFile

                var headerTextStyle = ExcelExtensions.GetCellTitleStyle(ExcelExtensions.HeaderTitleColor);
                var currentColumn = startColumn;
                CellRange currentCellRange;

                //// Header

                var title = MainResources.DailyWinLoss2h_CockfightingReport_Title;
                currentColumn = startColumn;
                currentCellRange = workSheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow + 1, currentColumn + maxColumnIndex);
                currentCellRange.Merged = true;
                currentCellRange.Value = title;
                currentCellRange.Style = headerTextStyle;
                currentRow += 2;

                workSheet.ExportEmptyRow(startRow, startColumn, ref currentRow, startColumn + maxColumnIndex);
                _dailyWinLoss2hUtilityService.ExportHeaderColumns_SportReport_NotApplyVirtualRate(
                    ref workSheet,
                    ref currentRow,
                    startRow,
                    startColumn,
                    false);

                //// Header

                _dailyWinLoss2hUtilityService.ExportContent_SportReport(
                    ref workSheet,
                    ref currentRow,
                    startRow,
                    startColumn,
                    report,
                    SportContants.CockFighting);

                //// MakeExcelFile

                var reportName = string.Format(
                    MainResources.DailyWinLoss2h_ReportName,
                    criteria.WinlostDate.ToShortFormatedDateWithSlash().Replace('/', '_'));
                excelFile.ReponseStream(reportName);
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }
        }

        public void ExportCockFightingArenaReport(GetCockFightingArenaCriteria criteria)
        {
            var report = CacheExtension.Get<CockFightingArena>(_dailyWinLoss2hCacheCockFightingArenaReportKey);

            if (report == null)
            {
                report = this.GetCockFightingArenaReport(criteria);
            }

            HSSFWorkbook workbook = new HSSFWorkbook();
            string sheetName = MainResources.DailyWinLoss2h_SheetName;
            ISheet worksheet = workbook.CreateSheet(sheetName);
            this.SetColorAtIndex(workbook);
            this.InitColumnWith(ref worksheet);
            int currentRow = 0;
            this.ExportCockFightingArenaReportTitle(workbook, ref worksheet, ref currentRow);
            currentRow += 3;
            this.ExportCockFightingArenaReportHeader(workbook, ref worksheet, ref currentRow);
            currentRow++;
            this.ExportCockFightingArenaReportContent(workbook, ref worksheet, ref currentRow, report.ReportRows);
            this.ExportCockFightingArenaGrandTotal(workbook, ref worksheet, ref currentRow, report.GrandTotal);

            string reportName = string.Format(
                   MainResources.DailyWinLoss2h_ReportName,
                   criteria.WinLostDate.ToShortFormatedDateWithSlash().Replace("/", "_"));
            NpoiExcelExtensions.ResponseStreamExcel(workbook, reportName);
        }

        private WinLossInfo BuildCockFightingReport(List<dynamic> rawData)
        {
            var report = InitWinLoss();
            report.GrandTotal.Name = Labels.All_Total;
            report.EnableVirtual = _dailyWinLoss2hUtilityService.IsApplyVirtualRate(SportContants.CockFighting);

            var cockFightingWinLoss = InitWinLoss();
            cockFightingWinLoss.Id = SportContants.CockFighting;
            cockFightingWinLoss.Name = Labels.Cockfighting;
            cockFightingWinLoss.GrandTotal.Name = Labels.Cockfighting;
            cockFightingWinLoss.GrandTotal.Id = SportContants.CockFighting;

            foreach (var data in rawData)
            {
                WinLossInfo winLoss = MapWinLossData(data);
                var rowType = _dailyWinLoss2hUtilityService.GetRowType(0, winLoss.SiteType);
                cockFightingWinLoss.CreditTotal.BettypeId = winLoss.BettypeId;
                cockFightingWinLoss.DepositTotal.BettypeId = winLoss.BettypeId;
                switch (rowType)
                {
                    case RowType._2hReportCredit:
                        winLoss.AliasName = Labels.Credit;
                        cockFightingWinLoss.CreditTotal.Add(winLoss);
                        break;

                    case RowType._2hReportDeposit:
                        winLoss.AliasName = Labels.Deposit;
                        cockFightingWinLoss.DepositTotal.Add(winLoss);
                        break;
                }
            }

            cockFightingWinLoss.GrandTotal.Add(cockFightingWinLoss.CreditTotal);
            cockFightingWinLoss.GrandTotal.Add(cockFightingWinLoss.DepositTotal);
            report.CreditTotal.Add(cockFightingWinLoss.CreditTotal);
            report.DepositTotal.Add(cockFightingWinLoss.DepositTotal);
            cockFightingWinLoss = WinLossInfo.Format_Total(cockFightingWinLoss);
            report.ChildsInfo.Add(cockFightingWinLoss);

            report.GrandTotal.Add(report.CreditTotal);
            report.GrandTotal.Add(report.DepositTotal);
            report = WinLossInfo.Format_Total(report);
            return report;
        }

        private WinLossInfo InitWinLoss()
        {
            WinLossInfo winLoss = new WinLossInfo
            {
                ChildsInfo = new List<WinLossInfo>(),
                GrandTotal = new WinLossInfo(),
                CreditTotal = new WinLossInfo()
                {
                    Name = Labels.Credit
                },
                DepositTotal = new WinLossInfo()
                {
                    Name = Labels.Deposit
                }
            };

            return winLoss;
        }

        private WinLossInfo MapWinLossData(dynamic data)
        {
            return new WinLossInfo
            {
                WinLoss = Converter.ToDecimal(data.WinLoss),
                Position = Converter.ToDecimal(data.Position),
                Commission = Converter.ToDecimal(data.Commission),
                LineTotal = Converter.ToDecimal(data.RealLineTotal),
                Turnover = Converter.ToDecimal(data.RealTurnover),
                SiteType = Converter.ToInt(data.Site),
                BettypeId = Converter.ToInt(data.bettype)
            };
        }

        private List<CockFightingArenaDetail> GetCockFightingArena(List<CockFightingMatchDetail> cockFightingMatchs)
        {
            List<CockFightingArenaDetail> cockFightingArena = new List<CockFightingArenaDetail>();
            var arenaGroup = cockFightingMatchs.GroupBy(x => x.ArenaId);

            foreach (var group in arenaGroup)
            {
                CockFightingArenaDetail arenaDetail = new CockFightingArenaDetail()
                {
                    ArenaName = CacheRef.GetLeagueName(Converter.ToInt(group.Key)),
                    CockFightingMatchs = new List<CockFightingMatchDetail>()
                };

                var arenaMatchs = group.ToList();
                foreach (var data in arenaMatchs)
                {
                    arenaDetail.CockFightingMatchs.Add(data);
                }

                if (arenaMatchs.Count > 1)
                {
                    var arenaTotal = GetCockFightingArenaGrandTotal(arenaMatchs);
                    arenaDetail.CockFightingMatchs.Add(arenaTotal);
                }

                cockFightingArena.Add(arenaDetail);
            }

            return cockFightingArena;
        }

        private CockFightingMatchDetail GetCockFightingArenaGrandTotal(List<CockFightingMatchDetail> cockFightingMatchs)
        {
            CockFightingMatchDetail grandTotal = new CockFightingMatchDetail();

            foreach (var item in cockFightingMatchs)
            {
                grandTotal.WinLoss += item.WinLoss;
                grandTotal.Position += item.Position;
                grandTotal.Commission += item.Commission;
                grandTotal.VirtualCommission += item.VirtualCommission;
                grandTotal.RealLineTotal += item.RealLineTotal;
                grandTotal.VirtualLineTotal += item.VirtualLineTotal;
                grandTotal.RealTurnover += item.RealTurnover;
            }

            return grandTotal;
        }

        private void InitColumnWith(ref ISheet worksheet)
        {
            for (int i = 0; i < 10; i++)
            {
                worksheet.SetColumnWidth(i, 170 * ExcelExtensions.FontWidth);
            }

            worksheet.SetColumnWidth(1, 220 * ExcelExtensions.FontWidth);
            worksheet.SetColumnWidth(2, 60 * ExcelExtensions.FontWidth);
            worksheet.SetColumnWidth(3, 220 * ExcelExtensions.FontWidth);
        }

        private void SetColorAtIndex(HSSFWorkbook excelFile)
        {
            HSSFPalette palette = excelFile.GetCustomPalette();
            palette.SetColorAtIndex(ArenaColorIndex, (byte)228, (byte)242, (byte)253);
            palette.SetColorAtIndex(GrandTotalColorIndex, (byte)219, (byte)225, (byte)187);
            palette.SetColorAtIndex(GrandTotalBorderColorIndex, (byte)255, (byte)102, (byte)0);
        }

        private void ExportCockFightingArenaReportTitle(
            HSSFWorkbook workbook,
            ref ISheet worksheet,
            ref int currentRow)
        {
            ICellStyle headerStyle = NpoiExcelExtensions.HeaderStyle(workbook);
            int currentColumn = 0;
            string reportTitle = "Main > Cockfighting > Cockfighting";
            IRow row = worksheet.CreateRow(currentRow);
            NpoiExcelExtensions.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow + 1, currentColumn + 9, reportTitle, headerStyle);
        }

        private void ExportCockFightingArenaReportHeader(
            HSSFWorkbook workbook,
            ref ISheet worksheet,
            ref int currentRow)
        {
            int currentColumn = 0;
            IRow row = worksheet.CreateRow(currentRow);
            ICellStyle headerStyle = NpoiExcelExtensions.HeaderStyle(workbook);
            NpoiExcelExtensions.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow, currentColumn, Labels.Fight_No, headerStyle);
            NpoiExcelExtensions.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow, currentColumn + 2, Labels.Event, headerStyle);
            NpoiExcelExtensions.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow, currentColumn, Labels.Member_Win_Loss, headerStyle);
            NpoiExcelExtensions.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow, currentColumn, Labels.SMA_Win_Loss, headerStyle);
            NpoiExcelExtensions.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow, currentColumn, Labels.Commission, headerStyle);
            NpoiExcelExtensions.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow, currentColumn, Labels.Company_Income, headerStyle);
            NpoiExcelExtensions.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow, currentColumn, Labels.Company_Turnover, headerStyle);
            NpoiExcelExtensions.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow, currentColumn, Labels.Margin, headerStyle);
        }

        private void ExportCockFightingArenaReportContent(
            HSSFWorkbook workbook,
            ref ISheet worksheet,
            ref int currentRow,
            List<CockFightingArenaDetail> reportRows)
        {
            ICellStyle textStyle = NpoiExcelExtensions.TextStyle(workbook, HSSFColor.White.Index, false, NpoiExcelExtensions.Alignment.Center);
            ICellStyle leftTextStyle = NpoiExcelExtensions.TextStyle(workbook, HSSFColor.White.Index, false, NpoiExcelExtensions.Alignment.Left);
            ICellStyle rightTextStyle = NpoiExcelExtensions.TextStyle(workbook, HSSFColor.White.Index, false, NpoiExcelExtensions.Alignment.Right);
            ICellStyle decimalNumberStyle = NpoiExcelExtensions.DecimalStyle(workbook, HSSFColor.White.Index, false);
            ICellStyle positiveDecimalNumberStyle = NpoiExcelExtensions.PositiveDecimalStyle(workbook, HSSFColor.White.Index, false);
            ICellStyle positivePercentStyle = NpoiExcelExtensions.PositivePercentStyle(workbook, HSSFColor.White.Index, false);
            ICellStyle arenaTextStyle = NpoiExcelExtensions.TextStyle(workbook, ArenaColorIndex, true, NpoiExcelExtensions.Alignment.Center);
            IRow row = null;
            int currentColumn = 0;

            foreach (var arena in reportRows)
            {
                row = worksheet.CreateRow(currentRow);
                currentColumn = 0;
                NpoiExcelExtensions.RenderCell(
                       ref worksheet,
                       ref row,
                       currentRow,
                       ref currentColumn,
                       currentRow,
                       currentColumn + 9,
                       arena.ArenaName,
                       arenaTextStyle);
                currentRow++;

                var numberMatch = arena.CockFightingMatchs.Count;
                for (int i = 0; i < numberMatch; i++)
                {
                    var item = arena.CockFightingMatchs[i];
                    row = worksheet.CreateRow(currentRow);
                    currentColumn = 0;
                    if (i > 0 && i == numberMatch - 1)
                    {
                        NpoiExcelExtensions.RenderCell(
                           ref worksheet,
                           ref row,
                           currentRow,
                           ref currentColumn,
                           currentRow,
                           currentColumn + 3,
                           string.Empty,
                           textStyle);
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
                    }

                    NpoiExcelExtensions.RenderCell(
                       ref worksheet,
                       ref row,
                       currentRow,
                       ref currentColumn,
                       currentRow,
                       currentColumn,
                       item.WinLoss,
                       positiveDecimalNumberStyle);

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
                       item.RealLineTotal,
                       positiveDecimalNumberStyle);

                    NpoiExcelExtensions.RenderCell(
                      ref worksheet,
                      ref row,
                      currentRow,
                      ref currentColumn,
                      currentRow,
                      currentColumn,
                      item.RealTurnover,
                      positiveDecimalNumberStyle);

                    NpoiExcelExtensions.RenderCell(
                       ref worksheet,
                       ref row,
                       currentRow,
                       ref currentColumn,
                       currentRow,
                       currentColumn,
                       item.Margin,
                       positivePercentStyle);

                    currentRow++;
                }
            }
        }

        private void ExportCockFightingArenaGrandTotal(
            HSSFWorkbook workbook,
            ref ISheet worksheet,
            ref int currentRow,
            CockFightingMatchDetail grandTotal)
        {
            ICellStyle grandTotalTextStyle = NpoiExcelExtensions.TextStyle(workbook, GrandTotalColorIndex, true, NpoiExcelExtensions.Alignment.Right);
            grandTotalTextStyle.BorderTop = BorderStyle.Medium;
            grandTotalTextStyle.TopBorderColor = GrandTotalBorderColorIndex;

            ICellStyle grandTotalDecimalNumberStyle = NpoiExcelExtensions.DecimalStyle(workbook, GrandTotalColorIndex, true);
            grandTotalDecimalNumberStyle.BorderTop = BorderStyle.Medium;
            grandTotalDecimalNumberStyle.TopBorderColor = GrandTotalBorderColorIndex;

            ICellStyle grandTotalPositiveDecimalNumberStyle = NpoiExcelExtensions.PositiveDecimalStyle(workbook, GrandTotalColorIndex, true);
            grandTotalPositiveDecimalNumberStyle.BorderTop = BorderStyle.Medium;
            grandTotalPositiveDecimalNumberStyle.TopBorderColor = GrandTotalBorderColorIndex;

            ICellStyle grandTotalPositivePercentStyle = NpoiExcelExtensions.PercentStyle(workbook, GrandTotalColorIndex, true);
            grandTotalPositivePercentStyle.BorderTop = BorderStyle.Medium;
            grandTotalPositivePercentStyle.TopBorderColor = GrandTotalBorderColorIndex;

            IRow row = worksheet.CreateRow(currentRow);
            int currentColumn = 0;

            NpoiExcelExtensions.RenderCell(
                   ref worksheet,
                   ref row,
                   currentRow,
                   ref currentColumn,
                   currentRow,
                   currentColumn + 3,
                   "Cockfighting Total",
                   grandTotalTextStyle);

            NpoiExcelExtensions.RenderCell(
               ref worksheet,
               ref row,
               currentRow,
               ref currentColumn,
               currentRow,
               currentColumn,
               grandTotal.WinLoss,
               grandTotalPositiveDecimalNumberStyle);

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
               grandTotal.RealLineTotal,
               grandTotalPositiveDecimalNumberStyle);

            NpoiExcelExtensions.RenderCell(
              ref worksheet,
              ref row,
              currentRow,
              ref currentColumn,
              currentRow,
              currentColumn,
              grandTotal.RealTurnover,
              grandTotalPositiveDecimalNumberStyle);

            NpoiExcelExtensions.RenderCell(
               ref worksheet,
               ref row,
               currentRow,
               ref currentColumn,
               currentRow,
               currentColumn,
               grandTotal.Margin,
               grandTotalPositivePercentStyle);
        }
    }
}