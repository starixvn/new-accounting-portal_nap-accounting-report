// -----------------------------------------------------------------------
// <copyright file="BettradeService.cs" company="Nexcel Solutions Vietnam">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Fanex.AccountingReports.Main.Services.Implement.DailyWinLoss2h
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
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

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class BettradeService : ServiceBase, IBettradeService
    {
        private string _dailyWinLoss2hCacheBettradeKey = "Fanex.AccountingReports.Main.DailyWinLoss2h.Bettrade" +
    (AppSession.CurrentUser != null ? AppSession.CurrentUser.Username : string.Empty);

        private DailyWinLoss2hUtilityService _dailyWinLoss2hUtilityService;

        public BettradeService()
        {
            _dailyWinLoss2hUtilityService = new DailyWinLoss2hUtilityService();
        }

        public WinLossInfo GetBettradeReport(GetBettradeCriteria criteria)
        {
            var rawData = this.GetReportData<dynamic>(criteria);
            WinLossInfo report = BuildBettradeReport(rawData.ToList());
            CacheExtension.Set(_dailyWinLoss2hCacheBettradeKey, report);
            return report;
        }

        /// <summary>
        /// Exports the live casino report.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        [SuppressMessage("StyleCop.CSharp.Nexcel.NexcelCustomRules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        public void ExportBettradeReport(GetBettradeCriteria criteria)
        {
            try
            {
                var report = SessionManager.Get<WinLossInfo>(_dailyWinLoss2hCacheBettradeKey);
                if (report == null)
                {
                    report = this.GetBettradeReport(criteria);
                }

                //// ExcelTypes

                ExcelFile excelFile = new ExcelFile();
                ExcelWorksheetCollection worksheets = excelFile.Worksheets;
                string sheetName = MainResources.Daily_WinLoss2h_Bettrade_SheetName;
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

                var title = MainResources.DailyWinLoss2h_BettradeReport_Title;
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
                    SportContants.Bettrade);

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

        private WinLossInfo BuildBettradeReport(List<dynamic> rawData)
        {
            var report = InitWinLoss();
            report.GrandTotal.Name = Labels.All_Total;
            report.EnableVirtual = _dailyWinLoss2hUtilityService.IsApplyVirtualRate(SportContants.Bettrade);

            var bettradeWinLoss = InitWinLoss();
            bettradeWinLoss.Id = SportContants.Bettrade;
            bettradeWinLoss.Name = Labels.Bettrade;
            bettradeWinLoss.GrandTotal.Name = Labels.Bettrade;
            bettradeWinLoss.GrandTotal.Id = SportContants.Bettrade;

            foreach (var data in rawData)
            {
                WinLossInfo winLoss = MapWinLossData(data);
                bettradeWinLoss.CreditTotal.BettypeId = winLoss.BettypeId;
                bettradeWinLoss.DepositTotal.BettypeId = winLoss.BettypeId;
                var rowType = _dailyWinLoss2hUtilityService.GetRowType(0, winLoss.SiteType);
                switch (rowType)
                {
                    case RowType._2hReportCredit:
                        winLoss.AliasName = Labels.Credit;
                        bettradeWinLoss.CreditTotal.Add(winLoss);
                        break;
                    case RowType._2hReportDeposit:
                        winLoss.AliasName = Labels.Deposit;
                        bettradeWinLoss.DepositTotal.Add(winLoss);
                        break;
                }
            }

            bettradeWinLoss.GrandTotal.Add(bettradeWinLoss.CreditTotal);
            bettradeWinLoss.GrandTotal.Add(bettradeWinLoss.DepositTotal);
            report.CreditTotal.Add(bettradeWinLoss.CreditTotal);
            report.DepositTotal.Add(bettradeWinLoss.DepositTotal);
            bettradeWinLoss = WinLossInfo.Format_Total(bettradeWinLoss);
            report.ChildsInfo.Add(bettradeWinLoss);

            report.GrandTotal.Add(report.CreditTotal);
            report.GrandTotal.Add(report.DepositTotal);
            report = WinLossInfo.Format_Total(report);
            return report;
        }

        private WinLossInfo MapWinLossData(dynamic data)
        {
            return new WinLossInfo
            {
                WinLoss = Converter.ToDecimal(data.WinLost),
                Position = Converter.ToDecimal(data.Position),
                Commission = Converter.ToDecimal(data.Commission),
                LineTotal = Converter.ToDecimal(data.LineTotal),
                Turnover = Converter.ToDecimal(data.Turnover),
                SiteType = Converter.ToInt(data.site),
                BettypeId = Converter.ToInt(data.bettype)
            };
        }
    }
}