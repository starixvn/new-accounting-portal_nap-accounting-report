// -----------------------------------------------------------------------
// <copyright file="PlaytechCasinoWinLossService.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
// <history>
//   <change who="Franklin.Nguyen" date="2014.04.14">Created</change>
// </history>

namespace Fanex.AccountingReports.Licensee.Services.Implement.PlaytechCasinoWinLoss
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using Fanex.AccountingReports.Core.Constants;
    using Fanex.AccountingReports.Core.Extensions;
    using Fanex.AccountingReports.Core.Services.Impl;
    using Fanex.AccountingReports.Core.Web;
    using Fanex.AccountingReports.Licensee.Constants;
    using Fanex.AccountingReports.Licensee.Criteria.PlaytechCasinoWinLoss;
    using Fanex.AccountingReports.Licensee.Models.PlaytechCasinoWinLoss;
    using Fanex.AccountingReports.Licensee.Resources;
    using Fanex.AccountingReports.Licensee.Services.Interface;
    using Fanex.Common;
    using NPOI.HSSF.UserModel;
    using NPOI.HSSF.Util;
    using NPOI.SS.UserModel;

    /// <summary>
    /// Play tech casino win loss service.
    /// </summary>
    public class PlaytechCasinoWinLossService : ServiceBase, IPlaytechCasinoWinLossService
    {
        private const short EvenRowColorIndex = 37;
        private const short SubTotalColorIndex = 32;
        private const short GrandTotalColorIndex = 41;
        private const short GrandTotalBorderColorIndex = 47;

        private string _playtechCasinoWinLossReportSessionKey = "Fanex.AccountingReports.Licensee.PlaytechCasinoWinLoss." +
          (AppSession.CurrentUser != null ? AppSession.CurrentUser.Username : string.Empty);

        /// <summary>
        /// Gets the play tech casino win loss.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>Play tech casino win loss.</returns>
        public PlaytechCasinoWinLoss GetPlaytechCasinoWinLoss(GetPlaytechCasinoWinLossCriteria criteria)
        {
            var report = new PlaytechCasinoWinLoss
                {
                    Total = new List<PlaytechCasinoWinLoss>(),
                    BaseCurrencyTotal = new PlaytechCasinoWinLoss()
                };
            try
            {
                var rawData = GetReportData<dynamic>(criteria)
                                   .OrderBy(data => data.ReportType);
                var reportByDateGroup = rawData.GroupBy(data => data.ReportType).ToList();
                Dictionary<int, PlaytechCasinoWinLoss> totalByCurrency = new Dictionary<int, PlaytechCasinoWinLoss>();
                Dictionary<string, PlaytechCasinoWinLoss> baseCurrencyTotal = this.CreateBaseCurrency();
                var startIndex = criteria.StartIndex;

                foreach (var reportByDate in reportByDateGroup)
                {
                    var playtechCasinoWinLossByDateGroup = new PlaytechCasinoWinLoss();
                    DateTime bettingDate = GetBettingDate(Converter.ToString2(reportByDate.Key), criteria.ReportType);
                    GetDateFromReportType(criteria, bettingDate, ref playtechCasinoWinLossByDateGroup);

                    foreach (var reportByCurrency in reportByDate)
                    {
                        var playtechCasinoWinLossByCurrency = this.GetWinLossDataByCurrency(reportByCurrency, criteria);
                        playtechCasinoWinLossByDateGroup.ReportRows.Add(playtechCasinoWinLossByCurrency);
                        CalculateTotal(reportByCurrency, playtechCasinoWinLossByCurrency, ref totalByCurrency, ref baseCurrencyTotal);
                    }

                    report.ReportRows.Add(playtechCasinoWinLossByDateGroup);
                }

                this.AssignTotalData(ref report, totalByCurrency, baseCurrencyTotal);
                SessionManager.Set<PlaytechCasinoWinLoss>(_playtechCasinoWinLossReportSessionKey, report);

                var reportPaging = new PlaytechCasinoWinLoss
                {
                    ReportRows = report.ReportRows.Skip(startIndex).Take(criteria.ItemsPerPage).ToList(),
                    Total = report.Total,
                    BaseCurrencyTotal = report.BaseCurrencyTotal,
                    TotalItems = report.ReportRows.Count
                };

                return reportPaging;
            }
            catch (Exception ex)
            {
                HandleException(ex);
                throw;
            }
        }

        /// <summary>
        /// Exports the play tech casino win loss.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        public void ExportPlaytechCasinoWinLoss(GetPlaytechCasinoWinLossCriteria criteria)
        {
            try
            {
                var report = SessionManager.Get<PlaytechCasinoWinLoss>(_playtechCasinoWinLossReportSessionKey);

                HSSFWorkbook excelFile = new HSSFWorkbook();
                string sheetName = LicenseeResources.PlaytechCasinoWinLoss_SheetName;
                ISheet worksheet = excelFile.CreateSheet(sheetName);
                this.SetColorAtIndex(excelFile);
                int currentRow = 0;

                if (report == null)
                {
                    report = this.GetPlaytechCasinoWinLoss(criteria);
                }

                if (report.ReportRows.Count > 0)
                {
                    this.ExportReportHeader(excelFile, ref worksheet, ref currentRow, criteria);
                    this.ExportReportContent(excelFile, ref worksheet, ref currentRow, report);
                }

                if (report.Total != null)
                {
                    this.ExportReportTotal(excelFile, ref worksheet, ref currentRow, report);
                }

                if (report.BaseCurrencyTotal != null)
                {
                    this.ExportReportGrandTotal(excelFile, ref worksheet, ref currentRow, report);
                }

                string reportName = string.Format(
                    LicenseeResources.PlaytechCasinoWinLoss_ReportName,
                    criteria.FromDate.ToShortFormatedDateWithSlash(),
                    criteria.ToDate.ToShortFormatedDateWithSlash());

                NpoiExcelExtensions.ResponseStreamExcel(excelFile, reportName);
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }
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
            GetPlaytechCasinoWinLossCriteria criteria)
        {
            ICellStyle headerStyle = NpoiExcelExtensions.HeaderStyle(excelFile);
            IRow row = null;
            int currentColumn = 0;

            for (int i = 0; i < 7; i++)
            {
                worksheet.SetColumnWidth(i, 140 * ExcelExtensions.FontWidth);
            }

            string reportTitle = string.Format(
                    LicenseeResources.PlaytechCasinoWinLoss_ReportTitle,
                    criteria.FromDate.ToShortFormatedDate(),
                    criteria.ToDate.ToShortFormatedDate());
            row = worksheet.CreateRow(currentRow);
            NpoiExcelExtensions.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow, currentColumn + 6, reportTitle, headerStyle);
            currentRow += 2;
            currentColumn = 0;

            row = worksheet.CreateRow(currentRow);
            NpoiExcelExtensions.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow, currentColumn, Labels.Date, headerStyle);
            NpoiExcelExtensions.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow, currentColumn, Labels.Currency, headerStyle);
            NpoiExcelExtensions.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow, currentColumn, Labels.Turnover, headerStyle);
            NpoiExcelExtensions.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow, currentColumn, Labels.TotalTrans, headerStyle);
            NpoiExcelExtensions.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow, currentColumn, Labels.CustomerWinLoss, headerStyle);
            NpoiExcelExtensions.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow, currentColumn, Labels.LicenseeWinLoss, headerStyle);
            NpoiExcelExtensions.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow, currentColumn, Labels.AmountDueTo, headerStyle);

            currentRow++;
        }

        /// <summary>
        /// Exports the content of the report.
        /// </summary>
        /// <param name="excelFile">The excel file.</param>
        /// <param name="worksheet">The worksheet.</param>
        /// <param name="currentRow">The current row.</param>
        /// <param name="report">The report.</param>
        [SuppressMessage("StyleCop.CSharp.Nexcel.NexcelCustomRules",
        "SP2101:MethodMustNotContainMoreLinesThan",
        Justification = "Reviewed. Suppression is OK here.")]
        private void ExportReportContent(HSSFWorkbook excelFile, ref ISheet worksheet, ref int currentRow, PlaytechCasinoWinLoss report)
        {
            ICellStyle textStyle = NpoiExcelExtensions.TextStyle(excelFile, HSSFColor.White.Index, false, NpoiExcelExtensions.Alignment.Center);
            ICellStyle intNumberStyle = NpoiExcelExtensions.IntStyle(excelFile, HSSFColor.White.Index, false, NpoiExcelExtensions.Alignment.Right);
            ICellStyle decimalNumberStyle = NpoiExcelExtensions.DecimalStyle(excelFile, HSSFColor.White.Index, false);
            ICellStyle evenRowTextStyle = NpoiExcelExtensions.TextStyle(excelFile, EvenRowColorIndex, false, NpoiExcelExtensions.Alignment.Center);
            ICellStyle evenRowIntNumberStyle = NpoiExcelExtensions.IntStyle(excelFile, EvenRowColorIndex, false, NpoiExcelExtensions.Alignment.Right);
            ICellStyle evenRowDecimalNumberStyle = NpoiExcelExtensions.DecimalStyle(excelFile, EvenRowColorIndex, false);
            IRow row = null;
            int currentColumn = 0;

            for (int i = 0; i < report.ReportRows.Count; i++)
            {
                var reportByDate = report.ReportRows[i];
                foreach (var reportByCurrency in reportByDate.ReportRows)
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
                        string.Empty,
                        i % 2 == 0 ? textStyle : evenRowTextStyle);
                    NpoiExcelExtensions.RenderCell(
                        ref worksheet,
                        ref row,
                        currentRow,
                        ref currentColumn,
                        currentRow,
                        currentColumn,
                        reportByCurrency.CurrencyName,
                        i % 2 == 0 ? textStyle : evenRowTextStyle);
                    NpoiExcelExtensions.RenderCell(
                        ref worksheet,
                        ref row,
                        currentRow,
                        ref currentColumn,
                        currentRow,
                        currentColumn,
                        reportByCurrency.TurnOver,
                        i % 2 == 0 ? decimalNumberStyle : evenRowDecimalNumberStyle);
                    NpoiExcelExtensions.RenderCell(
                        ref worksheet,
                        ref row,
                        currentRow,
                        ref currentColumn,
                        currentRow,
                        currentColumn,
                        reportByCurrency.TotalTransaction,
                        i % 2 == 0 ? intNumberStyle : evenRowIntNumberStyle);
                    NpoiExcelExtensions.RenderCell(
                        ref worksheet,
                        ref row,
                        currentRow,
                        ref currentColumn,
                        currentRow,
                        currentColumn,
                        reportByCurrency.CustomerWinLoss,
                        i % 2 == 0 ? decimalNumberStyle : evenRowDecimalNumberStyle);
                    NpoiExcelExtensions.RenderCell(
                        ref worksheet,
                        ref row,
                        currentRow,
                        ref currentColumn,
                        currentRow,
                        currentColumn,
                        reportByCurrency.LicenseeWinLoss,
                        i % 2 == 0 ? decimalNumberStyle : evenRowDecimalNumberStyle);
                    NpoiExcelExtensions.RenderCell(
                        ref worksheet,
                        ref row,
                        currentRow,
                        ref currentColumn,
                        currentRow,
                        currentColumn,
                        reportByCurrency.AmountDueTo,
                        i % 2 == 0 ? decimalNumberStyle : evenRowDecimalNumberStyle);
                    currentRow++;
                }

                int startMergeRow = currentRow - reportByDate.ReportRows.Count;
                currentColumn = 0;
                NpoiExcelExtensions.RenderCell(
                    ref worksheet,
                    ref row,
                    startMergeRow,
                    ref currentColumn,
                    currentRow - 1,
                    currentColumn,
                    reportByDate.Date,
                    i % 2 == 0 ? textStyle : evenRowTextStyle);
            }
        }

        /// <summary>
        /// Exports the report total.
        /// </summary>
        /// <param name="excelFile">The excel file.</param>
        /// <param name="worksheet">The worksheet.</param>
        /// <param name="currentRow">The current row.</param>
        /// <param name="report">The report.</param>
        [SuppressMessage("StyleCop.CSharp.Nexcel.NexcelCustomRules",
        "SP2101:MethodMustNotContainMoreLinesThan",
        Justification = "Reviewed. Suppression is OK here.")]
        private void ExportReportTotal(HSSFWorkbook excelFile, ref ISheet worksheet, ref int currentRow, PlaytechCasinoWinLoss report)
        {
            ICellStyle totalTextStyle = NpoiExcelExtensions.TextStyle(excelFile, SubTotalColorIndex, true, NpoiExcelExtensions.Alignment.Center);
            ICellStyle totalIntNumberStyle = NpoiExcelExtensions.IntStyle(excelFile, SubTotalColorIndex, true, NpoiExcelExtensions.Alignment.Right);
            ICellStyle totalDecimalNumberStyle = NpoiExcelExtensions.DecimalStyle(excelFile, SubTotalColorIndex, true);
            IRow row = null;
            int currentColumn = 0;

            foreach (var totalRow in report.Total)
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
                        string.Empty,
                        totalTextStyle);
                NpoiExcelExtensions.RenderCell(
                    ref worksheet,
                    ref row,
                    currentRow,
                    ref currentColumn,
                    currentRow,
                    currentColumn,
                    totalRow.CurrencyName,
                   totalTextStyle);
                NpoiExcelExtensions.RenderCell(
                    ref worksheet,
                    ref row,
                    currentRow,
                    ref currentColumn,
                    currentRow,
                    currentColumn,
                    totalRow.TurnOver,
                    totalDecimalNumberStyle);
                NpoiExcelExtensions.RenderCell(
                    ref worksheet,
                    ref row,
                    currentRow,
                    ref currentColumn,
                    currentRow,
                    currentColumn,
                    totalRow.TotalTransaction,
                   totalIntNumberStyle);
                NpoiExcelExtensions.RenderCell(
                    ref worksheet,
                    ref row,
                    currentRow,
                    ref currentColumn,
                    currentRow,
                    currentColumn,
                    totalRow.CustomerWinLoss,
                    totalDecimalNumberStyle);
                NpoiExcelExtensions.RenderCell(
                    ref worksheet,
                    ref row,
                    currentRow,
                    ref currentColumn,
                    currentRow,
                    currentColumn,
                    totalRow.LicenseeWinLoss,
                    totalDecimalNumberStyle);
                NpoiExcelExtensions.RenderCell(
                    ref worksheet,
                    ref row,
                    currentRow,
                    ref currentColumn,
                    currentRow,
                    currentColumn,
                    totalRow.AmountDueTo,
                    totalDecimalNumberStyle);
                currentRow++;
            }

            int startMergeRow = currentRow - report.Total.Count;
            currentColumn = 0;
            NpoiExcelExtensions.RenderCell(
                ref worksheet,
                ref row,
                startMergeRow,
                ref currentColumn,
                currentRow - 1,
                currentColumn,
                Labels.Total,
                totalTextStyle);
        }

        /// <summary>
        /// Exports the report grand total.
        /// </summary>
        /// <param name="excelFile">The excel file.</param>
        /// <param name="worksheet">The worksheet.</param>
        /// <param name="currentRow">The current row.</param>
        /// <param name="report">The report.</param>
        [SuppressMessage("StyleCop.CSharp.Nexcel.NexcelCustomRules",
        "SP2101:MethodMustNotContainMoreLinesThan",
        Justification = "Reviewed. Suppression is OK here.")]
        private void ExportReportGrandTotal(HSSFWorkbook excelFile, ref ISheet worksheet, ref int currentRow, PlaytechCasinoWinLoss report)
        {
            ICellStyle grandTotalBorderTextStyle = NpoiExcelExtensions.TextStyle(excelFile, GrandTotalColorIndex, true, NpoiExcelExtensions.Alignment.Center);
            grandTotalBorderTextStyle.BorderTop = BorderStyle.Medium;
            grandTotalBorderTextStyle.TopBorderColor = GrandTotalBorderColorIndex;
            ICellStyle grandTotalBorderIntNumberStyle = NpoiExcelExtensions.IntStyle(excelFile, GrandTotalColorIndex, true, NpoiExcelExtensions.Alignment.Right);
            grandTotalBorderIntNumberStyle.BorderTop = BorderStyle.Medium;
            grandTotalBorderIntNumberStyle.TopBorderColor = GrandTotalBorderColorIndex;
            ICellStyle grandTotalBorderDecimalNumberStyle = NpoiExcelExtensions.DecimalStyle(excelFile, GrandTotalColorIndex, true);
            grandTotalBorderDecimalNumberStyle.BorderTop = BorderStyle.Medium;
            grandTotalBorderDecimalNumberStyle.TopBorderColor = GrandTotalBorderColorIndex;
            ICellStyle grandTotalTextStyle = NpoiExcelExtensions.TextStyle(excelFile, GrandTotalColorIndex, true, NpoiExcelExtensions.Alignment.Center);
            ICellStyle grandTotalIntNumberStyle = NpoiExcelExtensions.IntStyle(excelFile, GrandTotalColorIndex, true, NpoiExcelExtensions.Alignment.Right);
            ICellStyle grandTotalDecimalNumberStyle = NpoiExcelExtensions.DecimalStyle(excelFile, GrandTotalColorIndex, true);
            IRow row = null;
            int currentColumn = 0;

            for (int i = 0; i < report.BaseCurrencyTotal.ReportRows.Count; i++)
            {
                var grandTotalRow = report.BaseCurrencyTotal.ReportRows[i];
                row = worksheet.CreateRow(currentRow);
                currentColumn = 0;
                NpoiExcelExtensions.RenderCell(
                        ref worksheet,
                        ref row,
                        currentRow,
                        ref currentColumn,
                        currentRow,
                        currentColumn,
                        string.Empty,
                        i == 0 ? grandTotalBorderTextStyle : grandTotalTextStyle);
                NpoiExcelExtensions.RenderCell(
                    ref worksheet,
                    ref row,
                    currentRow,
                    ref currentColumn,
                    currentRow,
                    currentColumn,
                    grandTotalRow.CurrencyName,
                    i == 0 ? grandTotalBorderTextStyle : grandTotalTextStyle);
                NpoiExcelExtensions.RenderCell(
                    ref worksheet,
                    ref row,
                    currentRow,
                    ref currentColumn,
                    currentRow,
                    currentColumn,
                    grandTotalRow.TurnOver,
                    i == 0 ? grandTotalBorderDecimalNumberStyle : grandTotalDecimalNumberStyle);
                NpoiExcelExtensions.RenderCell(
                    ref worksheet,
                    ref row,
                    currentRow,
                    ref currentColumn,
                    currentRow,
                    currentColumn,
                    string.Empty,
                    i == 0 ? grandTotalBorderIntNumberStyle : grandTotalIntNumberStyle);
                NpoiExcelExtensions.RenderCell(
                    ref worksheet,
                    ref row,
                    currentRow,
                    ref currentColumn,
                    currentRow,
                    currentColumn,
                    grandTotalRow.CustomerWinLoss,
                    i == 0 ? grandTotalBorderDecimalNumberStyle : grandTotalDecimalNumberStyle);
                NpoiExcelExtensions.RenderCell(
                    ref worksheet,
                    ref row,
                    currentRow,
                    ref currentColumn,
                    currentRow,
                    currentColumn,
                    grandTotalRow.LicenseeWinLoss,
                     i == 0 ? grandTotalBorderDecimalNumberStyle : grandTotalDecimalNumberStyle);
                NpoiExcelExtensions.RenderCell(
                    ref worksheet,
                    ref row,
                    currentRow,
                    ref currentColumn,
                    currentRow,
                    currentColumn,
                    grandTotalRow.AmountDueTo,
                    i == 0 ? grandTotalBorderDecimalNumberStyle : grandTotalDecimalNumberStyle);
                currentRow++;
            }

            int startMergeRow = currentRow - report.BaseCurrencyTotal.ReportRows.Count;
            currentColumn = 0;
            NpoiExcelExtensions.RenderCell(
                ref worksheet,
                ref row,
                startMergeRow,
                ref currentColumn,
                currentRow - 1,
                currentColumn,
                Labels.Total,
                grandTotalBorderTextStyle);

            currentColumn = 3;
            NpoiExcelExtensions.RenderCell(
               ref worksheet,
               ref row,
               startMergeRow,
               ref currentColumn,
               currentRow - 1,
               currentColumn,
               report.BaseCurrencyTotal.TotalTransaction,
               grandTotalBorderIntNumberStyle);
        }

        /// <summary>
        /// Sets the index of the color at.
        /// </summary>
        /// <param name="excelFile">The excel file.</param>
        private void SetColorAtIndex(HSSFWorkbook excelFile)
        {
            HSSFPalette palette = excelFile.GetCustomPalette();
            palette.SetColorAtIndex(EvenRowColorIndex, (byte)228, (byte)242, (byte)253);
            palette.SetColorAtIndex(SubTotalColorIndex, (byte)244, (byte)244, (byte)190);
            palette.SetColorAtIndex(GrandTotalColorIndex, (byte)219, (byte)225, (byte)187);
            palette.SetColorAtIndex(GrandTotalBorderColorIndex, (byte)255, (byte)102, (byte)0);
        }

        /// <summary>
        /// Gets the win loss data by currency.
        /// </summary>
        /// <param name="rawData">The raw data.</param>
        /// <param name="criteria">The criteria.</param>
        /// <returns>Play tech casino win loss by currency.</returns>
        private PlaytechCasinoWinLoss GetWinLossDataByCurrency(dynamic rawData, GetPlaytechCasinoWinLossCriteria criteria)
        {
            PlaytechCasinoWinLoss playtechCasinoWinLoss = null;
            CurrencyContants currencyToConvert = (CurrencyContants)criteria.ConvertToCurrency;

            switch (currencyToConvert)
            {
                case CurrencyContants.All:
                    playtechCasinoWinLoss = AssignAllCurrencyData(rawData);
                    break;

                case CurrencyContants.MYR:
                    playtechCasinoWinLoss = AssignMYRCurrencyData(rawData);
                    break;

                case CurrencyContants.EUR:
                    playtechCasinoWinLoss = AssignEURCurrencyData(rawData);
                    break;

                case CurrencyContants.USD:
                    playtechCasinoWinLoss = AssignUSDCurrencyData(rawData);
                    break;

                case CurrencyContants.GBP:
                    playtechCasinoWinLoss = AssignGBPCurrencyData(rawData);
                    break;

                default:
                    break;
            }

            DateTime bettingDate = GetBettingDate(Converter.ToString2(rawData.ReportType), criteria.ReportType);

            GetDateFromReportType(criteria, bettingDate, ref playtechCasinoWinLoss);

            return playtechCasinoWinLoss;
        }

        /// <summary>
        /// Gets the type of the date from report.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <param name="bettingDate">The betting date.</param>
        /// <param name="playtechCasinoWinLoss">The play tech casino win loss.</param>
        private void GetDateFromReportType(
            GetPlaytechCasinoWinLossCriteria criteria,
            DateTime bettingDate,
            ref PlaytechCasinoWinLoss playtechCasinoWinLoss)
        {
            switch (criteria.ReportType)
            {
                case (int)ReportTimeType.Daily:
                    playtechCasinoWinLoss.Date = bettingDate.ToShortFormatedDateWithSlash();
                    break;

                case (int)ReportTimeType.Monthly:
                    playtechCasinoWinLoss.Date = bettingDate.ToMonthYearWithSlash();
                    break;
            }
        }

        /// <summary>
        /// Assign all currency data.
        /// </summary>
        /// <param name="rawData">The raw data.</param>
        /// <returns>Play tech casino win loss by currency.</returns>
        private PlaytechCasinoWinLoss AssignAllCurrencyData(dynamic rawData)
        {
            return new PlaytechCasinoWinLoss
            {
                CurrencyId = Converter.ToInt(rawData.Currency),
                CurrencyName = Converter.ToString2(rawData.CurrencyName),
                TurnOver = Converter.ToDecimal(rawData.Turnover),
                TotalTransaction = Converter.ToDecimal(rawData.BetCount),
                CustomerWinLoss = Converter.ToDecimal(rawData.Winlost),
                LicenseeWinLoss = Converter.ToDecimal(rawData.LicenseeWL),
                AmountDueTo = Converter.ToDecimal(rawData.AmtDueTo)
            };
        }

        /// <summary>
        /// Assign the USD currency data.
        /// </summary>
        /// <param name="rawData">The raw data.</param>
        /// <returns>Play tech casino win loss by currency.</returns>
        private PlaytechCasinoWinLoss AssignUSDCurrencyData(dynamic rawData)
        {
            return new PlaytechCasinoWinLoss
            {
                CurrencyId = Converter.ToInt(rawData.Currency),
                CurrencyName = Converter.ToString2(rawData.CurrencyName),
                TurnOver = Converter.ToDecimal(rawData.TurnoverUSD),
                TotalTransaction = Converter.ToDecimal(rawData.BetCount),
                CustomerWinLoss = Converter.ToDecimal(rawData.WinlostUSD),
                LicenseeWinLoss = Converter.ToDecimal(rawData.LicenseeWLUSD),
                AmountDueTo = Converter.ToDecimal(rawData.AmtDueToUSD)
            };
        }

        /// <summary>
        /// Assign the EUR currency data.
        /// </summary>
        /// <param name="rawData">The raw data.</param>
        /// <returns>Play tech casino win loss by currency.</returns>
        private PlaytechCasinoWinLoss AssignEURCurrencyData(dynamic rawData)
        {
            return new PlaytechCasinoWinLoss
            {
                CurrencyId = Converter.ToInt(rawData.Currency),
                CurrencyName = Converter.ToString2(rawData.CurrencyName),
                TurnOver = Converter.ToDecimal(rawData.TurnoverEUR),
                TotalTransaction = Converter.ToDecimal(rawData.BetCount),
                CustomerWinLoss = Converter.ToDecimal(rawData.WinlostEUR),
                LicenseeWinLoss = Converter.ToDecimal(rawData.LicenseeWLEUR),
                AmountDueTo = Converter.ToDecimal(rawData.AmtDueToEUR)
            };
        }

        /// <summary>
        /// Assign the MYR currency data.
        /// </summary>
        /// <param name="rawData">The raw data.</param>
        /// <returns>Play tech casino win loss by currency.</returns>
        private PlaytechCasinoWinLoss AssignMYRCurrencyData(dynamic rawData)
        {
            return new PlaytechCasinoWinLoss
            {
                CurrencyId = Converter.ToInt(rawData.Currency),
                CurrencyName = Converter.ToString2(rawData.CurrencyName),
                TurnOver = Converter.ToDecimal(rawData.TurnoverMYR),
                TotalTransaction = Converter.ToDecimal(rawData.BetCount),
                CustomerWinLoss = Converter.ToDecimal(rawData.WinlostMYR),
                LicenseeWinLoss = Converter.ToDecimal(rawData.LicenseeWLMYR),
                AmountDueTo = Converter.ToDecimal(rawData.AmtDueToMYR)
            };
        }

        /// <summary>
        /// Assign the GBP currency data.
        /// </summary>
        /// <param name="rawData">The raw data.</param>
        /// <returns>Play tech casino win loss by currency.</returns>
        private PlaytechCasinoWinLoss AssignGBPCurrencyData(dynamic rawData)
        {
            return new PlaytechCasinoWinLoss
            {
                CurrencyId = Converter.ToInt(rawData.Currency),
                CurrencyName = Converter.ToString2(rawData.CurrencyName),
                TurnOver = Converter.ToDecimal(rawData.TurnoverGBP),
                TotalTransaction = Converter.ToDecimal(rawData.BetCount),
                CustomerWinLoss = Converter.ToDecimal(rawData.WinlostGBP),
                LicenseeWinLoss = Converter.ToDecimal(rawData.LicenseeWLGBP),
                AmountDueTo = Converter.ToDecimal(rawData.AmtDueToGBP)
            };
        }

        /// <summary>
        /// Calculates the total.
        /// </summary>
        /// <param name="rawData">The raw data.</param>
        /// <param name="playtechCasinoWinLoss">The play tech casino win loss.</param>
        /// <param name="totalByCurrency">The total by currency.</param>
        /// <param name="baseCurrencyTotal">The base currency total.</param>
        private void CalculateTotal(
                dynamic rawData,
                PlaytechCasinoWinLoss playtechCasinoWinLoss,
                ref Dictionary<int, PlaytechCasinoWinLoss> totalByCurrency,
                ref Dictionary<string, PlaytechCasinoWinLoss> baseCurrencyTotal)
        {
            var currencies = ReportService.ExchangeCurrencies;

            if (!totalByCurrency.ContainsKey(playtechCasinoWinLoss.CurrencyId))
            {
                totalByCurrency.Add(
                    playtechCasinoWinLoss.CurrencyId,
                    new PlaytechCasinoWinLoss
                    {
                        CurrencyId = playtechCasinoWinLoss.CurrencyId,
                        CurrencyName = currencies[playtechCasinoWinLoss.CurrencyId]
                    });
            }

            var totalRow = totalByCurrency[playtechCasinoWinLoss.CurrencyId];
            totalRow.TurnOver += playtechCasinoWinLoss.TurnOver;
            totalRow.TotalTransaction += playtechCasinoWinLoss.TotalTransaction;
            totalRow.CustomerWinLoss += playtechCasinoWinLoss.CustomerWinLoss;
            totalRow.LicenseeWinLoss += playtechCasinoWinLoss.LicenseeWinLoss;
            totalRow.AmountDueTo += playtechCasinoWinLoss.AmountDueTo;

            var myrTotal = baseCurrencyTotal[Labels.MYR];
            myrTotal.TurnOver += Converter.ToDecimal(rawData.TurnoverMYR);
            myrTotal.TotalTransaction += Converter.ToDecimal(rawData.BetCount);
            myrTotal.CustomerWinLoss += Converter.ToDecimal(rawData.WinlostMYR);
            myrTotal.LicenseeWinLoss += Converter.ToDecimal(rawData.LicenseeWLMYR);
            myrTotal.AmountDueTo += Converter.ToDecimal(rawData.AmtDueToMYR);

            var eurTotal = baseCurrencyTotal[Labels.EUR];
            eurTotal.TurnOver += Converter.ToDecimal(rawData.TurnoverEUR);
            eurTotal.TotalTransaction += Converter.ToDecimal(rawData.BetCount);
            eurTotal.CustomerWinLoss += Converter.ToDecimal(rawData.WinlostEUR);
            eurTotal.LicenseeWinLoss += Converter.ToDecimal(rawData.LicenseeWLEUR);
            eurTotal.AmountDueTo += Converter.ToDecimal(rawData.AmtDueToEUR);

            var usdTotal = baseCurrencyTotal[Labels.USD];
            usdTotal.TurnOver += Converter.ToDecimal(rawData.TurnoverUSD);
            usdTotal.TotalTransaction += Converter.ToDecimal(rawData.BetCount);
            usdTotal.CustomerWinLoss += Converter.ToDecimal(rawData.WinlostUSD);
            usdTotal.LicenseeWinLoss += Converter.ToDecimal(rawData.LicenseeWLUSD);
            usdTotal.AmountDueTo += Converter.ToDecimal(rawData.AmtDueToUSD);

            var gbpTotal = baseCurrencyTotal[Labels.GBP];
            gbpTotal.TurnOver += Converter.ToDecimal(rawData.TurnoverGBP);
            gbpTotal.TotalTransaction += Converter.ToDecimal(rawData.BetCount);
            gbpTotal.CustomerWinLoss += Converter.ToDecimal(rawData.WinlostGBP);
            gbpTotal.LicenseeWinLoss += Converter.ToDecimal(rawData.LicenseeWLGBP);
            gbpTotal.AmountDueTo += Converter.ToDecimal(rawData.AmtDueToGBP);
        }

        /// <summary>
        /// Creates the base currency.
        /// </summary>
        /// <returns>Base currency list.</returns>
        private Dictionary<string, PlaytechCasinoWinLoss> CreateBaseCurrency()
        {
            var baseCurrency = new Dictionary<string, PlaytechCasinoWinLoss>();
            var currencies = ReportService.ExchangeCurrencies;

            baseCurrency.Add(
                Labels.MYR,
                new PlaytechCasinoWinLoss
                {
                    CurrencyId = 2,
                    CurrencyName = currencies[2]
                });

            baseCurrency.Add(
                Labels.USD,
                new PlaytechCasinoWinLoss
                {
                    CurrencyId = 3,
                    CurrencyName = currencies[3]
                });

            baseCurrency.Add(
                Labels.EUR,
                new PlaytechCasinoWinLoss
                {
                    CurrencyId = 6,
                    CurrencyName = currencies[6]
                });

            baseCurrency.Add(
                Labels.GBP,
                new PlaytechCasinoWinLoss
                {
                    CurrencyId = 12,
                    CurrencyName = currencies[12]
                });

            return baseCurrency;
        }

        /// <summary>
        /// Assigns the total data.
        /// </summary>
        /// <param name="report">The report.</param>
        /// <param name="totalByCurrency">The total by currency.</param>
        /// <param name="baseCurrencyTotal">The base currency total.</param>
        private void AssignTotalData(
            ref PlaytechCasinoWinLoss report,
            Dictionary<int, PlaytechCasinoWinLoss> totalByCurrency,
            Dictionary<string, PlaytechCasinoWinLoss> baseCurrencyTotal)
        {
            foreach (var currencyTotal in totalByCurrency)
            {
                report.Total.Add(currencyTotal.Value);
                report.BaseCurrencyTotal.TotalTransaction += currencyTotal.Value.TotalTransaction;
            }

            foreach (var baseTotal in baseCurrencyTotal)
            {
                report.BaseCurrencyTotal.ReportRows.Add(baseTotal.Value);
            }
        }

        /// <summary>
        /// To do: store procedure must return exactly string by report type.
        /// </summary>
        /// <param name="bettingDate">The betting date.</param>
        /// <param name="reportType">Type of the report.</param>
        /// <returns>Betting date.</returns>
        private DateTime GetBettingDate(string bettingDate, int reportType)
        {
            DateTime date = DateTime.Now;
            switch (reportType)
            {
                case (int)ReportTimeType.Daily:
                    date = new DateTime(
                        Converter.ToInt(bettingDate.Substring(0, 4)),
                        Converter.ToInt(bettingDate.Substring(4, 2)),
                        Converter.ToInt(bettingDate.Substring(6, 2)));
                    break;

                case (int)ReportTimeType.Monthly:
                    date = new DateTime(
                        Converter.ToInt(bettingDate.Substring(0, 4)),
                       Converter.ToInt(bettingDate.Substring(4, 2)),
                       1);
                    break;
            }

            return date;
        }
    }
}