// -----------------------------------------------------------------------
// <copyright file="MetricGamingService.cs" company="Nexcel Solutions Vietnam">
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
    /// Metric gaming service.
    /// </summary>
    public class MetricGamingService : ServiceBase, IMetricGamingService
    {
        private string _dailyWinLoss2hCacheMetricGamingBettypeDataKey = "Fanex.AccountingReports.Main.DailyWinLoss2h.MetricGamingBettype" +
    (AppSession.CurrentUser != null ? AppSession.CurrentUser.Username : string.Empty);

        private Dictionary<object, string> _cachedBettypesName;
        private DailyWinLoss2hUtilityService _dailyWinLoss2hUtilityService;

        public MetricGamingService()
        {
            _dailyWinLoss2hUtilityService = new DailyWinLoss2hUtilityService();
        }

        public WinLossInfo GetMetricGamingBettypeReport(GetMetricGamingBettypeCriteria criteria)
        {
            var rawData = this.GetReportData<dynamic>(criteria);
            var bettypes = rawData.Select(x => x.bettype).Distinct().ToList();
            _dailyWinLoss2hUtilityService.SaveBetTypeCacheRef(bettypes, ref _cachedBettypesName);
            WinLossInfo report = BuildMetricGamingBettypeReport(rawData.ToList());
            CacheExtension.Set(_dailyWinLoss2hCacheMetricGamingBettypeDataKey, report);
            return report;
        }

        /// <summary>
        /// Exports the live casino report.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        [SuppressMessage("StyleCop.CSharp.Nexcel.NexcelCustomRules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        public void ExportMetricGamingBettypeReport(GetMetricGamingBettypeCriteria criteria)
        {
            try
            {
                var report = SessionManager.Get<WinLossInfo>(_dailyWinLoss2hCacheMetricGamingBettypeDataKey);
                if (report == null)
                {
                    report = this.GetMetricGamingBettypeReport(criteria);
                }

                //// ExcelTypes

                ExcelFile excelFile = new ExcelFile();
                ExcelWorksheetCollection worksheets = excelFile.Worksheets;
                string sheetName = MainResources.DailyWinLoss2h_LiveCasinoReport_SheetName;
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

                var title = MainResources.DailyWinLoss2h_LiveCasinoReport_Title;
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
                    SportContants.MetricGamingSuperLive);

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

        private WinLossInfo BuildMetricGamingBettypeReport(List<dynamic> rawData)
        {
            var report = InitWinLoss();
            report.GrandTotal.Name = Labels.All_Total;
            report.EnableVirtual = _dailyWinLoss2hUtilityService.IsApplyVirtualRate(SportContants.MetricGamingSuperLive);
            var betTypeGroups = rawData.GroupBy(data => data.bettype);
            foreach (var bettypeGroup in betTypeGroups)
            {
                var bettypeWinLoss = InitWinLoss();
                bettypeWinLoss.Id = Converter.ToInt(bettypeGroup.Key);
                bettypeWinLoss.Name = _cachedBettypesName[bettypeGroup.Key];
                bettypeWinLoss.GrandTotal.Id = bettypeWinLoss.Id;
                bettypeWinLoss.GrandTotal.Name = bettypeWinLoss.Name;

                foreach (var data in bettypeGroup)
                {
                    WinLossInfo winLoss = MapWinLossData(data);
                    var rowType = _dailyWinLoss2hUtilityService.GetRowType(0, winLoss.SiteType);
                    switch (rowType)
                    {
                        case RowType._2hReportCredit:
                            winLoss.AliasName = Labels.Credit;
                            bettypeWinLoss.CreditTotal.Add(winLoss);
                            break;

                        case RowType._2hReportDeposit:
                            winLoss.AliasName = Labels.Deposit;
                            bettypeWinLoss.DepositTotal.Add(winLoss);
                            break;
                    }
                }

                bettypeWinLoss.GrandTotal.Add(bettypeWinLoss.CreditTotal);
                bettypeWinLoss.GrandTotal.Add(bettypeWinLoss.DepositTotal);
                report.CreditTotal.Add(bettypeWinLoss.CreditTotal);
                report.DepositTotal.Add(bettypeWinLoss.DepositTotal);
                bettypeWinLoss = WinLossInfo.Format_Total(bettypeWinLoss);
                report.ChildsInfo.Add(bettypeWinLoss);
            }

            report.ChildsInfo = report.ChildsInfo.OrderBy(sportGroup => sportGroup.Id).ToList();
            report.GrandTotal.Add(report.CreditTotal);
            report.GrandTotal.Add(report.DepositTotal);
            report = WinLossInfo.Format_Total(report);
            return report;
        }

        private WinLossInfo MapWinLossData(dynamic data)
        {
            return new WinLossInfo
            {
                Id = Converter.ToInt(data.bettype),
                WinLoss = Converter.ToDecimal(data.WinLost),
                Position = Converter.ToDecimal(data.Position),
                Commission = Converter.ToDecimal(data.Commission),
                LineTotal = Converter.ToDecimal(data.LineTotal),
                Turnover = Converter.ToDecimal(data.Turnover),
                SiteType = Converter.ToInt(data.site)
            };
        }
    }
}