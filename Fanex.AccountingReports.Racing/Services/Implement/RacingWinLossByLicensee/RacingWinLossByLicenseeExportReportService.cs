namespace Fanex.AccountingReports.Racing.Services.Implement.RacingWinLossByLicensee
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using Fanex.AccountingReports.Core.Configurations;
    using Fanex.AccountingReports.Core.Extensions;
    using Fanex.AccountingReports.Core.Services.Impl;
    using Fanex.AccountingReports.Core.Web;
    using Fanex.AccountingReports.Racing.Criteria.RacingWinLossByLicensee;
    using Fanex.AccountingReports.Racing.Models.RacingWinLossByLicensee;
    using Fanex.AccountingReports.Racing.Resources;
    using Fanex.AccountingReports.Racing.Services.Interface.RacingWinLossByLicensee;
    using NPOI.HSSF.UserModel;
    using NPOI.SS.UserModel;
    using NPOI.SS.Util;

    /// <summary>
    /// The export report service.
    /// </summary>
    public class RacingWinLossByLicenseeExportReportService : ServiceBase, IRacingWinLossByLicenseeExportReportService
    {
        #region declare variable

        private RacingWinLossByLicenseeUtility.CustomStyleCell _headerStyle;

        private RacingWinLossByLicenseeUtility.CustomStyleCell _normalStyle;

        private RacingWinLossByLicenseeUtility.CustomStyleCell _blankStyle;

        private RacingWinLossByLicenseeUtility.CustomStyleCell _leagueTotalStyle;

        private RacingWinLossByLicenseeUtility.CustomStyleCell _currencyTotalStyle;

        private RacingWinLossByLicenseeUtility.CustomStyleCell _grandTotalStyle;

        private string _racingWinLossByLicenseeSessionKey = string.Empty;

        private RacingWinLossByLicenseeService _getReportService;

        #endregion declare variable

        public RacingWinLossByLicenseeExportReportService()
        { 
            if (AppSession.CurrentUser != null)
            {
                _racingWinLossByLicenseeSessionKey +=
                    RacingWinLossByLicenseeUtility.RacingWinLossByLicenseePrefixSessionKey +
                                                      AppSession.CurrentUser.Username;
            }
            else
            {
                _racingWinLossByLicenseeSessionKey =
                    RacingWinLossByLicenseeUtility.RacingWinLossByLicenseePrefixSessionKey;
            }

            _getReportService = new RacingWinLossByLicenseeService();
        }

        #region export report

        /// <summary>
        /// Export report.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        public void ExportReport(GetRacingDailyReportCriteria criteria)
        {
            try
            {
                var report = SessionManager.Get<RacingDailyReport>(_racingWinLossByLicenseeSessionKey + criteria.Site);

                if (report == null)
                {
                    report = this._getReportService.GetAndConvertData(report, criteria);
                }

                var currentRow = 0;

                HSSFWorkbook excelFile = new HSSFWorkbook();
                var sheetName = RacingResources.RacingWinLossbyLicensee_SheetName;
                ISheet worksheet = excelFile.CreateSheet(sheetName);

                if (report != null && report.Rows != null && report.Rows.Count > 0)
                {
                    this.InitialStyle(excelFile);
                    this.ExportReportHeader(excelFile, ref worksheet, ref currentRow, criteria);
                    this.ExportReportContent(excelFile, ref worksheet, ref currentRow, report);
                }

                excelFile.ResponseStreamExcel(string.Format(
                    RacingResources.RacingWinLossByLicensee_ReportName,
                    criteria.Site,
                    criteria.WinLostDate.ToShortFormatedDateWithSlash().Replace("/", "_")));
            }
            catch (Exception ex)
            {
                this.HandleException(ex);
            }
        }

        /// <summary>
        /// Initial style.
        /// </summary>
        /// <param name="excelFile">The file excel.</param>
        private void InitialStyle(HSSFWorkbook excelFile)
        {
            this._headerStyle = new RacingWinLossByLicenseeUtility.CustomStyleCell(excelFile);
            RacingWinLossByLicenseeUtility.InitCellStyle(
                excelFile, 
                ref this._headerStyle,
                RacingWinLossByLicenseeUtility.HeaderRowClass);

            this._normalStyle = new RacingWinLossByLicenseeUtility.CustomStyleCell(excelFile);
            RacingWinLossByLicenseeUtility.InitCellStyle(
                excelFile, 
                ref this._normalStyle,
                RacingWinLossByLicenseeUtility.NormalRowClass);

            this._blankStyle = new RacingWinLossByLicenseeUtility.CustomStyleCell(excelFile);
            RacingWinLossByLicenseeUtility.InitCellStyle(
                excelFile,
                ref this._blankStyle,
                RacingWinLossByLicenseeUtility.BlankRowClass);

            this._leagueTotalStyle = new RacingWinLossByLicenseeUtility.CustomStyleCell(excelFile);
            RacingWinLossByLicenseeUtility.InitCellStyle(
                excelFile, 
                ref this._leagueTotalStyle,
                RacingWinLossByLicenseeUtility.LeagueTotalRowClass);

            this._currencyTotalStyle = new RacingWinLossByLicenseeUtility.CustomStyleCell(excelFile);
            RacingWinLossByLicenseeUtility.InitCellStyle(
                excelFile, 
                ref this._currencyTotalStyle,
                RacingWinLossByLicenseeUtility.CurrencyTotalRowClass);

            this._grandTotalStyle = new RacingWinLossByLicenseeUtility.CustomStyleCell();
            RacingWinLossByLicenseeUtility.InitCellStyle(
                excelFile, 
                ref this._grandTotalStyle,
                RacingWinLossByLicenseeUtility.GrandTotalRowClass);
        }

        /// <summary>
        /// Export report header.
        /// </summary>
        /// <param name="excelFile">The file excel.</param>
        /// <param name="worksheet">The worksheet.</param>
        /// <param name="currentRow">The current row.</param>
        /// <param name="criteria">The criteria.</param>
        [SuppressMessage("StyleCop.CSharp.StylecopPlus.Rules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        private void ExportReportHeader(
            HSSFWorkbook excelFile, 
            ref ISheet worksheet, 
            ref int currentRow, 
            GetRacingDailyReportCriteria criteria)
        {
            var currentColumn = 0;
            ICellStyle headerStyle = this._headerStyle.TextStyle;

            for (var i = 0; i < 16; i++)
            {
                worksheet.SetColumnWidth(i, 140 * NpoiExcelExtensions.FontWidth);
            }

            worksheet.SetColumnWidth(0, 200 * NpoiExcelExtensions.FontWidth);

            var reportTitle = string.Format(
                RacingResources.RacingWinLossByLicensee_ReportTitle,
                criteria.Site,
                criteria.WinLostDate.ToShortFormatedDate());

            IRow row = worksheet.CreateRow(currentRow);
            NpoiExcelExtensions.RenderCell(
                ref worksheet,
                ref row,
                currentRow,
                ref currentColumn,
                currentRow + 1,
                currentColumn + 15,
                reportTitle,
                headerStyle);

            currentRow += 2;
            worksheet.AddMergedRegion(new CellRangeAddress(currentRow, currentRow, 0, 15));

            currentRow++;
            currentColumn = 0;
            row = worksheet.CreateRow(currentRow);
            string licenseeTitle = string.Format("{0} {1}", Labels.Licensee, criteria.Site);

            NpoiExcelExtensions.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow + 1, currentColumn, Labels.MeetingCurrency, headerStyle);
            NpoiExcelExtensions.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow + 1, currentColumn, Labels.Race, headerStyle);
            NpoiExcelExtensions.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow, currentColumn + 4, Labels.Member_Overall, headerStyle);
            NpoiExcelExtensions.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow, currentColumn + 5, licenseeTitle, headerStyle);

            if (AppSettings.IsApply50PercentRacingReport)
            {
                NpoiExcelExtensions.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow, currentColumn + 1, Labels.IBC, headerStyle);
            }

            NpoiExcelExtensions.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow + 1, currentColumn, Labels.AmountDuetoIBC, headerStyle);

            currentRow++;
            row = worksheet.GetRow(currentRow) ?? worksheet.CreateRow(currentRow);
            currentColumn = 2;

            NpoiExcelExtensions.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow, currentColumn, Labels.WinTurnover, headerStyle);
            NpoiExcelExtensions.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow, currentColumn, Labels.PlaceTurnover, headerStyle);
            NpoiExcelExtensions.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow, currentColumn, Labels.WinPlaceTurnover, headerStyle);
            NpoiExcelExtensions.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow, currentColumn, Labels.TotalTurnover, headerStyle);
            NpoiExcelExtensions.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow, currentColumn, Labels.DividendWon, headerStyle);
            NpoiExcelExtensions.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow, currentColumn, Labels.WinTurnover, headerStyle);
            NpoiExcelExtensions.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow, currentColumn, Labels.PlaceTurnover, headerStyle);
            NpoiExcelExtensions.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow, currentColumn, Labels.WinPlaceTurnover, headerStyle);
            NpoiExcelExtensions.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow, currentColumn, Labels.TotalTurnover, headerStyle);
            NpoiExcelExtensions.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow, currentColumn, Labels.PositonTakingWinLoss, headerStyle);
            NpoiExcelExtensions.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow, currentColumn, Labels.CommissionIncome, headerStyle);

            if (AppSettings.IsApply50PercentRacingReport)
            {
                NpoiExcelExtensions.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow, currentColumn, Labels.PositonTakingWinLoss, headerStyle);
                NpoiExcelExtensions.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow, currentColumn, Labels.CommissionIncome, headerStyle);
            }

            currentRow++;
        }

        /// <summary>
        /// Export content of report.
        /// </summary>
        /// <param name="excelFile">The file excel.</param>
        /// <param name="worksheet">The worksheet.</param>
        /// <param name="currentRow">The current row.</param>
        /// <param name="report">The report.</param>
        private void ExportReportContent(
            HSSFWorkbook excelFile, 
            ref ISheet worksheet, 
            ref int currentRow, 
            RacingDailyReport report)
        {
            var numberCurrencies = report.Rows.Count;

            for (var i = 0; i < numberCurrencies; i++)
            {
                RacingDailyReport currency = report.Rows[i];
                this.RenderCurrency(excelFile, ref worksheet, ref currentRow, currency);
            }

            if (report.Races != null && report.Races.Count > 0)
            {
                this.RenderGrandTotal(excelFile, ref worksheet, ref currentRow, report.Races[1]);
            }
        }

        /// <summary>
        /// Render currency.
        /// </summary>
        /// <param name="excelFile">The file excel.</param>
        /// <param name="worksheet">The worksheet.</param>
        /// <param name="currentRow">The current row.</param>
        /// <param name="currency">The currency.</param>
        private void RenderCurrency(
            HSSFWorkbook excelFile, 
            ref ISheet worksheet, 
            ref int currentRow, 
            RacingDailyReport currency)
        {
            if (currency != null && currency.Rows != null && currency.Rows.Count > 0)
            {
                var numberLeagues = currency.Rows.Count;

                for (var i = 0; i < numberLeagues; i++)
                {
                    RacingDailyReport league = currency.Rows[i];
                    string leagueAndCurrencyName = league.Name + " - " + currency.Name;
                    this.RenderLeague(excelFile, ref worksheet, ref currentRow, league, leagueAndCurrencyName);
                }

                if (currency.Races != null && currency.Races.Count > 1) 
                {
                    this.RenderCurrencyTotal(excelFile, ref worksheet, ref currentRow, currency.Races, currency.Name);
                }
            }
        }

        /// <summary>
        /// Render league.
        /// </summary>
        /// <param name="excelFile">The file excel.</param>
        /// <param name="worksheet">The worksheet.</param>
        /// <param name="currentRow">The current row.</param>
        /// <param name="league">The league.</param>
        /// <param name="leagueAndCurrencyName">The league name combine with currency name.</param>
        private void RenderLeague(
            HSSFWorkbook excelFile, 
            ref ISheet worksheet, 
            ref int currentRow, 
            RacingDailyReport league,
            string leagueAndCurrencyName)
        {
            if (league != null && league.Rows != null && league.Rows.Count > 0)
            {
                var numberRaces = league.Rows.Count;
                var currentColumn = 0;
                IRow row = worksheet.GetRow(currentRow) ?? worksheet.CreateRow(currentRow);
                NpoiExcelExtensions.RenderCell(
                    ref worksheet, 
                    ref row, 
                    currentRow, 
                    ref currentColumn, 
                    currentRow + numberRaces - 1, 
                    currentColumn, 
                    leagueAndCurrencyName, 
                    _normalStyle.TextStyle);

                for (var i = 0; i < numberRaces; i++)
                {
                    RacingDailyReport race = league.Rows[i];
                    this.RenderRace(excelFile, ref worksheet, ref row, ref currentRow, currentColumn, race);
                }

                if (league.Races != null && league.Races.Count > 0)
                {
                    this.RenderLeagueTotal(excelFile, ref worksheet, ref currentRow, league.Races[0], leagueAndCurrencyName);
                }

                row = worksheet.GetRow(currentRow) ?? worksheet.CreateRow(currentRow);
                currentColumn = 0;
                var endColumn = 0;

                if (AppSettings.IsApply50PercentRacingReport) 
                {
                    endColumn = 15;
                }
                else 
                {
                    endColumn = 13;
                }

                NpoiExcelExtensions.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow, endColumn, string.Empty, this._blankStyle.TextStyle);
                currentRow++;
            }
        }

        /// <summary>
        /// Render race.
        /// </summary>
        /// <param name="excelFile">The excel file.</param>
        /// <param name="worksheet">The worksheet.</param>
        /// <param name="row">The row in excel file.</param>
        /// <param name="currentRow">The current row.</param>
        /// <param name="currentColumn">The current column.</param>
        /// <param name="race">The race in league.</param>
        private void RenderRace(
            HSSFWorkbook excelFile, 
            ref ISheet worksheet, 
            ref IRow row, 
            ref int currentRow,
            int currentColumn,
            RacingDailyReport race)
        {
            if (race != null && race.Races != null && race.Races.Count > 0)
            {
                RaceWinLost raceWinLoss = race.Races[0];
                row = worksheet.GetRow(currentRow) ?? worksheet.CreateRow(currentRow);

                ICellStyle textStyle = this._normalStyle.TextStyle;
                ICellStyle decimalStyle = this._normalStyle.DecimalStyle;

                NpoiExcelExtensions.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow, currentColumn, race.ID, textStyle);
                this.RenderRowWinLoss(excelFile, ref worksheet, ref row, ref currentRow, currentColumn, raceWinLoss, decimalStyle);
            }
        }

        /// <summary>
        /// Render total of league.
        /// </summary>
        /// <param name="excelFile">The file excel.</param>
        /// <param name="worksheet">The worksheet.</param>
        /// <param name="currentRow">The current row.</param>
        /// <param name="raceWinLost">The win lost of race.</param>
        /// <param name="leagueAndCurrencyName">The league name combine with currency name.</param>
        private void RenderLeagueTotal(
            HSSFWorkbook excelFile, 
            ref ISheet worksheet, 
            ref int currentRow, 
            RaceWinLost raceWinLost,
            string leagueAndCurrencyName)
        {
            var currentColumn = 0;
            IRow row = worksheet.GetRow(currentRow) ?? worksheet.CreateRow(currentRow);
            ICellStyle textStyle = this._leagueTotalStyle.TextStyle;
            ICellStyle decimalStyle = this._leagueTotalStyle.DecimalStyle;
            string cellValue = leagueAndCurrencyName + " " + Labels.Total;
            NpoiExcelExtensions.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow, currentColumn + 1, cellValue, textStyle);
            this.RenderRowWinLoss(excelFile, ref worksheet, ref row, ref currentRow, currentColumn, raceWinLost, decimalStyle);
        }

        /// <summary>
        /// Render total of currency.
        /// </summary>
        /// <param name="excelFile">The excel file.</param>
        /// <param name="worksheet">The worksheet.</param>
        /// <param name="currentRow">The current row.</param>
        /// <param name="totalWinLost">The total win lost.</param>
        /// <param name="currencyName">The current name.</param>
        private void RenderCurrencyTotal(
            HSSFWorkbook excelFile, 
            ref ISheet worksheet, 
            ref int currentRow, 
            List<RaceWinLost> totalWinLost,
            string currencyName)
        {
            var currentColumn = 0;
            IRow row = worksheet.GetRow(currentRow) ?? worksheet.CreateRow(currentRow);
            string currencyTotal = currencyName + " " + Labels.Total;
            ICellStyle textStyle = this._currencyTotalStyle.TextStyle;
            ICellStyle decimalStyle = this._currencyTotalStyle.DecimalStyle;

            NpoiExcelExtensions.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow + 1, currentColumn, currencyTotal, textStyle);
            NpoiExcelExtensions.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow, currentColumn, currencyName, textStyle);
            this.RenderRowWinLoss(excelFile, ref worksheet, ref row, ref currentRow, currentColumn, totalWinLost[0], decimalStyle);
            
            row = worksheet.GetRow(currentRow) ?? worksheet.CreateRow(currentRow);
            currentColumn = 1;

            NpoiExcelExtensions.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow, currentColumn, Labels.BaseCurrency, textStyle);
            this.RenderRowWinLoss(excelFile, ref worksheet, ref row, ref currentRow, currentColumn, totalWinLost[1], decimalStyle);
        }

        /// <summary>
        /// Render grand total.
        /// </summary>
        /// <param name="excelFile">The excel file.</param>
        /// <param name="worksheet">The worksheet.</param>
        /// <param name="currentRow">The current row.</param>
        /// <param name="raceWinLost">The win lost of race.</param>
        private void RenderGrandTotal(
            HSSFWorkbook excelFile, 
            ref ISheet worksheet, 
            ref int currentRow, 
            RaceWinLost raceWinLost)
        {
            var currentColumn = 0;
            ICellStyle textStyle = this._grandTotalStyle.TextStyle;
            ICellStyle decimalStyle = this._grandTotalStyle.DecimalStyle;
            IRow row = worksheet.GetRow(currentRow) ?? worksheet.CreateRow(currentRow);

            NpoiExcelExtensions.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow, currentColumn + 1, Labels.GrandTotal, textStyle);
            this.RenderRowWinLoss(excelFile, ref worksheet, ref row, ref currentRow, currentColumn, raceWinLost, decimalStyle);
        }

        /// <summary>
        /// Render win loss.
        /// </summary>
        /// <param name="excelFile">The excel file.</param>
        /// <param name="worksheet">The worksheet.</param>
        /// <param name="row">The row in excel file.</param>
        /// <param name="currentRow">The current row.</param>
        /// <param name="currentColumn">The current column.</param>
        /// <param name="raceWinLoss">The win loss of race.</param>
        /// <param name="decimalStyle">The decimal style.</param>
        [SuppressMessage("StyleCop.CSharp.StylecopPlus.Rules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        private void RenderRowWinLoss(
            HSSFWorkbook excelFile, 
            ref ISheet worksheet, 
            ref IRow row, 
            ref int currentRow, 
            int currentColumn, 
            RaceWinLost raceWinLoss,
            ICellStyle decimalStyle)
        {
            NpoiExcelExtensions.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow, currentColumn, raceWinLoss.MemberWinTurnover, decimalStyle);
            NpoiExcelExtensions.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow, currentColumn, raceWinLoss.MemberPlaceTurnover, decimalStyle);
            NpoiExcelExtensions.RenderCell(
                ref worksheet, 
                ref row, 
                currentRow, 
                ref currentColumn, 
                currentRow, 
                currentColumn, 
                raceWinLoss.MemberWinPlaceTurnover, 
                decimalStyle);
            NpoiExcelExtensions.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow, currentColumn, raceWinLoss.MemberTotalTurnover, decimalStyle);
            NpoiExcelExtensions.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow, currentColumn, raceWinLoss.MemberDividendWon, decimalStyle);
            NpoiExcelExtensions.RenderCell(
                ref worksheet, 
                ref row, 
                currentRow, 
                ref currentColumn, 
                currentRow, 
                currentColumn, 
                raceWinLoss.Site12BetWinTurnover, 
                decimalStyle);
            NpoiExcelExtensions.RenderCell(
                ref worksheet,
                ref row, 
                currentRow, 
                ref currentColumn, 
                currentRow, 
                currentColumn, 
                raceWinLoss.Site12BetPlaceTurnover, 
                decimalStyle);
            NpoiExcelExtensions.RenderCell(
                ref worksheet, 
                ref row, 
                currentRow, 
                ref currentColumn, 
                currentRow, 
                currentColumn, 
                raceWinLoss.Site12BetWinPlaceTurnover, 
                decimalStyle);
            NpoiExcelExtensions.RenderCell(
                ref worksheet, 
                ref row, 
                currentRow, 
                ref currentColumn, 
                currentRow, 
                currentColumn, 
                raceWinLoss.Site12BetTotalTurnover, 
                decimalStyle);

            NpoiExcelExtensions.RenderCell(
                ref worksheet, 
                ref row,
                currentRow, 
                ref currentColumn, 
                currentRow, 
                currentColumn, 
                raceWinLoss.Site12BetPositionTakingWinLoss, 
                decimalStyle);
            NpoiExcelExtensions.RenderCell(
                ref worksheet, 
                ref row, 
                currentRow, 
                ref currentColumn, 
                currentRow, 
                currentColumn, 
                raceWinLoss.Site12BetCommissionIncome, 
                decimalStyle);

            if (AppSettings.IsApply50PercentRacingReport)
            {
                NpoiExcelExtensions.RenderCell(
                    ref worksheet,
                    ref row,
                    currentRow,
                    ref currentColumn,
                    currentRow,
                    currentColumn,
                    raceWinLoss.IBCPositionTakingWinLoss,
                    decimalStyle);
                NpoiExcelExtensions.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow, currentColumn, raceWinLoss.IBCCommissionIncome, decimalStyle);
            }

            NpoiExcelExtensions.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow, currentColumn, raceWinLoss.AmountDueToIBC, decimalStyle);

            currentRow++;
        }

        #endregion export report  
    }
}
