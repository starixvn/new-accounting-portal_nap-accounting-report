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

        [SuppressMessage("StyleCop.CSharp.StylecopPlus.Rules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        public LiveAndNoneLiveReport GetMetricGamingBetTypes(GetMetricGamingBettypeCriteria criteria)
        {
            var report = InitMetricGamingBetTypesLiveAndNonLive();
            var rawData = this.GetReportData<dynamic>(criteria);
            var bettypes = rawData.Select(x => x.bettype).Distinct().ToList();
            _dailyWinLoss2hUtilityService.SaveBetTypeCacheRef(bettypes, ref _cachedBettypesName);

            var betTypeGroups = rawData.GroupBy(x => new
            {
                BetType = x.bettype,
                IsLive = x.liveindicator
            });

            foreach (var group in betTypeGroups)
            {
                int betTypeId = Converter.ToInt(group.Key.BetType);
                var betTypeWinLoss = new WinLossInfo
                {
                    Id = betTypeId,
                    CreditTotal = new WinLossInfo
                    {
                        Name = Labels.Credit,
                        EnableVirtual = true
                    },
                    DepositTotal = new WinLossInfo
                    {
                        Name = Labels.Deposit,
                        EnableVirtual = true
                    },
                    GrandTotal = new WinLossInfo
                    {
                        Id = betTypeId,
                        Name = _cachedBettypesName[group.Key.BetType],
                        EnableVirtual = true
                    },
                    IsLive = Converter.ToBool(group.Key.IsLive)
                };

                foreach (var betType in group)
                {
                    var winLoss = MapWinLossData(betType);
                    int rowType = _dailyWinLoss2hUtilityService.GetRowType(0, winLoss.SiteType);

                    switch (rowType)
                    {
                        case RowType._2hReportCredit:
                            winLoss.AliasName = Labels.Credit;
                            betTypeWinLoss.CreditTotal.Add(winLoss);
                            break;

                        case RowType._2hReportDeposit:
                            winLoss.AliasName = Labels.Deposit;
                            betTypeWinLoss.DepositTotal.Add(winLoss);
                            break;
                    }

                    betTypeWinLoss.GrandTotal.Add(winLoss);
                }

                if (betTypeWinLoss.IsLive)
                {
                    report.LiveReport.GrandTotal.Add(betTypeWinLoss.GrandTotal);
                    report.LiveReport.CreditTotal.Add(betTypeWinLoss.CreditTotal);
                    report.LiveReport.DepositTotal.Add(betTypeWinLoss.DepositTotal);
                    report.LiveReport.ChildsInfo.Add(betTypeWinLoss);
                }
                else
                {
                    report.NoneLiveReport.GrandTotal.Add(betTypeWinLoss.GrandTotal);
                    report.NoneLiveReport.CreditTotal.Add(betTypeWinLoss.CreditTotal);
                    report.NoneLiveReport.DepositTotal.Add(betTypeWinLoss.DepositTotal);
                    report.NoneLiveReport.ChildsInfo.Add(betTypeWinLoss);
                }
            }

            CacheExtension.Set(_dailyWinLoss2hCacheMetricGamingBettypeDataKey, report);
            return report;
        }

        /// <summary>
        /// Exports the live casino report.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        [SuppressMessage("StyleCop.CSharp.StylecopPlus.Rules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        public void ExportMetricGamingBettypeReport(GetMetricGamingBettypeCriteria criteria)
        {
            try
            {
                var report = SessionManager.Get<LiveAndNoneLiveReport>(_dailyWinLoss2hCacheMetricGamingBettypeDataKey);
                if (report == null)
                {
                    report = this.GetMetricGamingBetTypes(criteria);
                }

                ExcelFile excelFile = new ExcelFile();
                ExcelWorksheetCollection worksheets = excelFile.Worksheets;
                string sheetName = MainResources.DailyWinLoss2h_SoccerReport_SheetName;
                ExcelWorksheet workSheet = worksheets.Add(sheetName);
                int startRow = 0;
                int startColumn = 0;
                int maxColumnIndex = 10;
                var currentRow = startRow;

                this.ExportSportReport(
                    ref workSheet,
                    ref currentRow,
                    startRow,
                    startColumn,
                    false,
                    report.NoneLiveReport,
                    MainResources.DailyWinLoss2h_MetricGamingNonLiveReportBySport_Title,
                    false,
                    Constants.SportContants._2hSoccer);

                workSheet.ExportEmptyRow(startRow, startColumn, ref currentRow, startColumn + maxColumnIndex);

                this.ExportSportReport(
                    ref workSheet,
                    ref currentRow,
                    startRow,
                    startColumn,
                    false,
                    report.LiveReport,
                    MainResources.DailyWinLoss2h_MetricGamingLiveReportBySport_Title,
                    false,
                    Constants.SportContants._2hSoccer);

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
                report.GrandTotal.Add(bettypeWinLoss.CreditTotal);
                report.GrandTotal.Add(bettypeWinLoss.DepositTotal);
                bettypeWinLoss = WinLossInfo.Format_Total(bettypeWinLoss);
                report.ChildsInfo.Add(bettypeWinLoss);
            }

            report.ChildsInfo = report.ChildsInfo.OrderBy(sportGroup => sportGroup.Id).ToList();
            report = WinLossInfo.Format_Total(report);
            return report;
        }

        private WinLossInfo MapWinLossData(dynamic data)
        {
            return new WinLossInfo
            {
                Id = Converter.ToInt(data.bettype),
                IsLive = Converter.ToBool(data.liveindicator),
                WinLoss = Converter.ToDecimal(data.WinLost),
                Position = Converter.ToDecimal(data.Position),
                Commission = Converter.ToDecimal(data.Commission),
                LineTotal = Converter.ToDecimal(data.RealLineTotal),
                VirtualLineTotal = Converter.ToDecimal(data.VirtualLineTotal),
                Turnover = Converter.ToDecimal(data.RealTurnover),
                VirtualTurnover = Converter.ToDecimal(data.VirtualTurnover),
                SiteType = Converter.ToInt(data.site)
            };
        }

        private LiveAndNoneLiveReport InitMetricGamingBetTypesLiveAndNonLive()
        {
            return new LiveAndNoneLiveReport
            {
                NoneLiveReport = new WinLossInfo
                {
                    ChildsInfo = new List<WinLossInfo>(),
                    GrandTotal = new WinLossInfo
                    {
                        Name = Labels.All_Total,
                        ChildsInfo = new List<WinLossInfo>(),
                        EnableVirtual = true
                    },
                    CreditTotal = new WinLossInfo
                    {
                        Name = Labels.Credit,
                        EnableVirtual = true
                    },
                    DepositTotal = new WinLossInfo
                    {
                        Name = Labels.Deposit,
                        EnableVirtual = true
                    },
                    EnableVirtual = true,
                    IsLive = false
                },

                LiveReport = new WinLossInfo
                {
                    ChildsInfo = new List<WinLossInfo>(),
                    GrandTotal = new WinLossInfo
                    {
                        Name = Labels.All_Total,
                        EnableVirtual = true
                    },
                    CreditTotal = new WinLossInfo
                    {
                        Name = Labels.Credit,
                        EnableVirtual = true
                    },
                    DepositTotal = new WinLossInfo
                    {
                        Name = Labels.Deposit,
                        EnableVirtual = true
                    },
                    EnableVirtual = true,
                    IsLive = true
                }
            };
        }

        private void ExportSportReport(
            ref ExcelWorksheet workSheet,
            ref int currentRow,
            int startRow,
            int startColumn,
            bool isSportSummary,
            WinLossInfo report,
            string title,
            bool isPartOfReport,
            int sportId)
        {
            var headerTextStyle = ExcelExtensions.GetCellTitleStyle(ExcelExtensions.HeaderTitleColor);
            var currentColumn = startColumn;
            int maxColumnIndex = 10;
            CellRange currentCellRange;

            currentColumn = startColumn;
            if (isPartOfReport)
            {
                currentCellRange = workSheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow, currentColumn + maxColumnIndex);
            }
            else
            {
                currentCellRange = workSheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow + 1, currentColumn + maxColumnIndex);
            }

            currentCellRange.Merged = true;
            currentCellRange.Value = title;
            currentCellRange.Style = headerTextStyle;
            if (!isPartOfReport)
            {
                currentRow += 2;
                workSheet.ExportEmptyRow(startRow, startColumn, ref currentRow, startColumn + maxColumnIndex);
            }
            else
            {
                currentRow++;
            }

            _dailyWinLoss2hUtilityService.ExportHeaderColumns_SportReport(
                ref workSheet,
                ref currentRow,
                startRow,
                startColumn,
                isSportSummary);

            _dailyWinLoss2hUtilityService.ExportContent_SportReport(
                    ref workSheet,
                    ref currentRow,
                    startRow,
                    startColumn,
                    report,
                    sportId);
        }
    }
}