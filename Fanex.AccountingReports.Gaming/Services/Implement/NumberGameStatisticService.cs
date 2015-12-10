namespace Fanex.AccountingReports.Gaming.Services.Implement
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using Fanex.AccountingReports.Core.Extensions;
    using Fanex.AccountingReports.Core.Services.Impl;
    using Fanex.AccountingReports.Core.Web;
    using Fanex.AccountingReports.Gaming.Criteria.NumberGameStatistic;
    using Fanex.AccountingReports.Gaming.Models.NumberGameStatistic;
    using Fanex.AccountingReports.Gaming.Resources;
    using Fanex.AccountingReports.Gaming.Services.Interface;
    using Fanex.Common;
    using Fanex.Data;
    using NPOI.HSSF.UserModel;
    using NPOI.HSSF.Util;
    using NPOI.SS.UserModel;

    public class NumberGameStatisticService : ServiceBase, INumberGameStatisticService
    {
        private const int ExtendCachedTimeOut = 5;
        private const short EvenRowColorIndex = 37;
        private const short GrandTotalColorIndex = 41;
        private const short GrandTotalBorderColorIndex = 47;
        private const short HighLightColorIndex = 49;

        private string _numberGameStatisticReportCachedDataKey = "Fanex.AccountingReports.Gaming.NumberGameStatistic." +
           (AppSession.CurrentUser != null ? AppSession.CurrentUser.Username : string.Empty);

        public NumberGameStatistic GetReport(GetNumberGameStatisticCriteria criteria)
        {
            NumberGameStatistic report = new NumberGameStatistic()
                {
                    GrandTotal = new NumberGameStatistic()
                };
            Hashtable ouputParams = new Hashtable();
            try
            {
                using (var objectDb = new ObjectDb(criteria.GetSettingKey()))
                {
                    criteria.MoveBetsDay = ReportService.BalanceUp.MoveBetsDay;
                    var reportParams = criteria.GetSpParams();
                    var reader = objectDb.ExecuteReader(reportParams);
                    while (reader.Read())
                    {
                        var item = new NumberGameStatistic
                        {
                            LeagueName = Converter.ToString2(reader["LeagueName"]),
                            Players = Converter.ToInt(reader["Players"]),
                            BetCount = Converter.ToInt(reader["BetCount"]),
                            TotalBets = Converter.ToDecimal(reader["TotalBets"]),
                            TotalWins = Converter.ToDecimal(reader["TotalWins"]),
                            GameIncome = Converter.ToDecimal(reader["GameIncome"]),
                            GamePayout = Converter.ToDecimal(reader["GamePayout"]),
                            CompanyTurnover = Converter.ToDecimal(reader["CompanyTurnover"]),
                            NetIncome = Converter.ToDecimal(reader["NetIncome"])
                        };

                        report.CalCulateGrandTotal(item);
                        report.ReportRows.Add(item);
                    }

                    reader.Close();
                    ouputParams = objectDb.GetOuputParams();
                }
            }
            catch (Exception ex)
            {
                HandleException(ex);
                throw;
            }

            GetTotalPlayers(report.GrandTotal, ouputParams);
            CalCulateGrandTotalGamePayout(report.GrandTotal);
            CacheManager.Set<NumberGameStatistic>(_numberGameStatisticReportCachedDataKey, report, DateTime.Now.AddHours(ExtendCachedTimeOut));
            return report;
        }

        public void ExportReport(GetNumberGameStatisticCriteria criteria)
        {
            var report = CacheManager.Get<NumberGameStatistic>(_numberGameStatisticReportCachedDataKey);

            if (report == null)
            {
                report = GetReport(criteria);
            }

            HSSFWorkbook workbook = new HSSFWorkbook();
            string sheetName = GamingResources.NumberGameFinancial_SheetName;
            ISheet worksheet = workbook.CreateSheet(sheetName);
            this.SetColorAtIndex(workbook);
            this.InitColumnWith(ref worksheet);
            int currentRow = 0;
            this.ExportReportTitle(workbook, ref worksheet, ref currentRow, criteria);
            currentRow += 2;
            this.ExportReportHeader(workbook, ref worksheet, ref currentRow);
            currentRow++;
            this.ExportReportContent(workbook, ref worksheet, ref currentRow, report.ReportRows);
            this.ExportGrandTotal(workbook, ref worksheet, ref currentRow, report.GrandTotal);

            string reportName = string.Format(
                   GamingResources.NumberGameStatistic_ReportName,
                   criteria.FromDate.ToShortFormatedDateWithSlash(),
                   criteria.ToDate.ToShortFormatedDateWithSlash());
            NpoiExcelExtensions.ResponseStreamExcel(workbook, reportName);
        }

        private void CalCulateGrandTotalGamePayout(NumberGameStatistic grandTotal)
        {
            grandTotal.GamePayout = grandTotal.TotalBets == 0 ? 0 : grandTotal.TotalWins / grandTotal.TotalBets * 100;
        }

        private void GetTotalPlayers(NumberGameStatistic grandTotal, Hashtable ouputParams)
        {
            grandTotal.Players = Converter.ToInt(ouputParams["totalplayers"]);
        }

        private void InitColumnWith(ref ISheet worksheet)
        {
            for (int i = 0; i < 9; i++)
            {
                worksheet.SetColumnWidth(i, 170 * ExcelExtensions.FontWidth);
            }

            worksheet.SetColumnWidth(0, 220 * ExcelExtensions.FontWidth);
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

        private void ExportReportHeader(
            HSSFWorkbook workbook,
            ref ISheet worksheet,
            ref int currentRow)
        {
            int currentColumn = 0;
            IRow row = worksheet.CreateRow(currentRow);
            ICellStyle headerStyle = NpoiExcelExtensions.HeaderStyle(workbook);
            NpoiExcelExtensions.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow, currentColumn, Labels.League, headerStyle);
            NpoiExcelExtensions.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow, currentColumn, Labels.No_Players, headerStyle);
            NpoiExcelExtensions.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow, currentColumn, Labels.Bet_Count, headerStyle);
            NpoiExcelExtensions.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow, currentColumn, Labels.Total_Bets, headerStyle);
            NpoiExcelExtensions.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow, currentColumn, Labels.Total_Wins, headerStyle);
            NpoiExcelExtensions.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow, currentColumn, Labels.GameInCome, headerStyle);
            NpoiExcelExtensions.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow, currentColumn, Labels.GamePayout, headerStyle);
            NpoiExcelExtensions.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow, currentColumn, Labels.Company_Turnover, headerStyle);
            NpoiExcelExtensions.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow, currentColumn, Labels.CompanyNetInCome, headerStyle);
        }

        private void ExportReportContent(
            HSSFWorkbook workbook,
            ref ISheet worksheet,
            ref int currentRow,
            List<NumberGameStatistic> reportContent)
        {
            ICellStyle textStyle = NpoiExcelExtensions.TextStyle(workbook, HSSFColor.White.Index, false, NpoiExcelExtensions.Alignment.Center);
            ICellStyle intNumberStyle = NpoiExcelExtensions.IntStyle(workbook, HSSFColor.White.Index, false, NpoiExcelExtensions.Alignment.Right);
            ICellStyle decimalNumberStyle = NpoiExcelExtensions.DecimalStyle(workbook, HSSFColor.White.Index, false);
            ICellStyle evenRowTextStyle = NpoiExcelExtensions.TextStyle(workbook, EvenRowColorIndex, false, NpoiExcelExtensions.Alignment.Center);
            ICellStyle evenRowIntNumberStyle = NpoiExcelExtensions.IntStyle(workbook, EvenRowColorIndex, false, NpoiExcelExtensions.Alignment.Right);
            ICellStyle evenRowDecimalNumberStyle = NpoiExcelExtensions.DecimalStyle(workbook, EvenRowColorIndex, false);
            IRow row = null;
            int currentColumn = 0;

            for (int i = 0; i < reportContent.Count; i++)
            {
                var currentItem = reportContent[i];
                row = worksheet.CreateRow(currentRow);
                currentColumn = 0;

                NpoiExcelExtensions.RenderCell(
                    ref worksheet,
                    ref row,
                    currentRow,
                    ref currentColumn,
                    currentRow,
                    currentColumn,
                    currentItem.LeagueName,
                    i % 2 == 0 ? textStyle : evenRowTextStyle);

                NpoiExcelExtensions.RenderCell(
                   ref worksheet,
                   ref row,
                   currentRow,
                   ref currentColumn,
                   currentRow,
                   currentColumn,
                   currentItem.Players,
                   i % 2 == 0 ? intNumberStyle : evenRowIntNumberStyle);

                NpoiExcelExtensions.RenderCell(
                   ref worksheet,
                   ref row,
                   currentRow,
                   ref currentColumn,
                   currentRow,
                   currentColumn,
                   currentItem.BetCount,
                   i % 2 == 0 ? intNumberStyle : evenRowIntNumberStyle);

                NpoiExcelExtensions.RenderCell(
                  ref worksheet,
                  ref row,
                  currentRow,
                  ref currentColumn,
                  currentRow,
                  currentColumn,
                  currentItem.TotalBets,
                  i % 2 == 0 ? decimalNumberStyle : evenRowDecimalNumberStyle);

                NpoiExcelExtensions.RenderCell(
                   ref worksheet,
                   ref row,
                   currentRow,
                   ref currentColumn,
                   currentRow,
                   currentColumn,
                   currentItem.TotalWins,
                   i % 2 == 0 ? decimalNumberStyle : evenRowDecimalNumberStyle);

                NpoiExcelExtensions.RenderCell(
                  ref worksheet,
                  ref row,
                  currentRow,
                  ref currentColumn,
                  currentRow,
                  currentColumn,
                  currentItem.GameIncome,
                  i % 2 == 0 ? decimalNumberStyle : evenRowDecimalNumberStyle);

                NpoiExcelExtensions.RenderCell(
                   ref worksheet,
                   ref row,
                   currentRow,
                   ref currentColumn,
                   currentRow,
                   currentColumn,
                   currentItem.GamePayout,
                   i % 2 == 0 ? decimalNumberStyle : evenRowDecimalNumberStyle);

                NpoiExcelExtensions.RenderCell(
                  ref worksheet,
                  ref row,
                  currentRow,
                  ref currentColumn,
                  currentRow,
                  currentColumn,
                  currentItem.CompanyTurnover,
                  i % 2 == 0 ? decimalNumberStyle : evenRowDecimalNumberStyle);

                NpoiExcelExtensions.RenderCell(
                   ref worksheet,
                   ref row,
                   currentRow,
                   ref currentColumn,
                   currentRow,
                   currentColumn,
                   currentItem.NetIncome,
                   i % 2 == 0 ? decimalNumberStyle : evenRowDecimalNumberStyle);

                currentRow++;
            }
        }

        private void ExportGrandTotal(
            HSSFWorkbook workbook,
            ref ISheet worksheet,
            ref int currentRow,
            NumberGameStatistic grandTotal)
        {
            ICellStyle grandTotalTextStyle = NpoiExcelExtensions.TextStyle(workbook, GrandTotalColorIndex, true, NpoiExcelExtensions.Alignment.Center);
            grandTotalTextStyle.BorderTop = BorderStyle.Medium;
            grandTotalTextStyle.TopBorderColor = GrandTotalBorderColorIndex;
            ICellStyle grandTotalIntNumberStyle = NpoiExcelExtensions.IntStyle(workbook, GrandTotalColorIndex, true, NpoiExcelExtensions.Alignment.Right);
            grandTotalIntNumberStyle.BorderTop = BorderStyle.Medium;
            grandTotalIntNumberStyle.TopBorderColor = GrandTotalBorderColorIndex;
            ICellStyle grandTotalDecimalNumberStyle = NpoiExcelExtensions.DecimalStyle(workbook, GrandTotalColorIndex, true);
            grandTotalDecimalNumberStyle.BorderTop = BorderStyle.Medium;
            grandTotalDecimalNumberStyle.TopBorderColor = GrandTotalBorderColorIndex;

            ICellStyle grandTotalHighLightDecimalNumberStyle = NpoiExcelExtensions.DecimalStyle(workbook, HighLightColorIndex, true);
            grandTotalHighLightDecimalNumberStyle.BorderTop = BorderStyle.Medium;
            grandTotalHighLightDecimalNumberStyle.TopBorderColor = GrandTotalBorderColorIndex;

            IRow row = worksheet.CreateRow(currentRow);
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

            NpoiExcelExtensions.RenderCell(
               ref worksheet,
               ref row,
               currentRow,
               ref currentColumn,
               currentRow,
               currentColumn,
               grandTotal.BetCount,
               grandTotalIntNumberStyle);

            NpoiExcelExtensions.RenderCell(
              ref worksheet,
              ref row,
              currentRow,
              ref currentColumn,
              currentRow,
              currentColumn,
              grandTotal.TotalBets,
              grandTotalDecimalNumberStyle);

            NpoiExcelExtensions.RenderCell(
               ref worksheet,
               ref row,
               currentRow,
               ref currentColumn,
               currentRow,
               currentColumn,
               grandTotal.TotalWins,
               grandTotalDecimalNumberStyle);

            NpoiExcelExtensions.RenderCell(
              ref worksheet,
              ref row,
              currentRow,
              ref currentColumn,
              currentRow,
              currentColumn,
              grandTotal.GameIncome,
              grandTotalHighLightDecimalNumberStyle);

            NpoiExcelExtensions.RenderCell(
               ref worksheet,
               ref row,
               currentRow,
               ref currentColumn,
               currentRow,
               currentColumn,
               grandTotal.GamePayout,
               grandTotalDecimalNumberStyle);

            NpoiExcelExtensions.RenderCell(
              ref worksheet,
              ref row,
              currentRow,
              ref currentColumn,
              currentRow,
              currentColumn,
              grandTotal.CompanyTurnover,
              grandTotalDecimalNumberStyle);

            NpoiExcelExtensions.RenderCell(
               ref worksheet,
               ref row,
               currentRow,
               ref currentColumn,
               currentRow,
               currentColumn,
               grandTotal.NetIncome,
               grandTotalHighLightDecimalNumberStyle);
        }

        private void SetColorAtIndex(HSSFWorkbook excelFile)
        {
            HSSFPalette palette = excelFile.GetCustomPalette();
            palette.SetColorAtIndex(EvenRowColorIndex, (byte)228, (byte)242, (byte)253);
            palette.SetColorAtIndex(GrandTotalColorIndex, (byte)219, (byte)225, (byte)187);
            palette.SetColorAtIndex(GrandTotalBorderColorIndex, (byte)255, (byte)102, (byte)0);
            palette.SetColorAtIndex(HighLightColorIndex, (byte)181, (byte)196, (byte)208);
        }
    }
}