namespace Fanex.AccountingReports.Misc.Services.Implement
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using Fanex.AccountingReports.Core.Services.Impl;
    using Fanex.AccountingReports.Core.Web;
    using Fanex.AccountingReports.Misc.Criteria.MemberWinLossOnRNGCasino;
    using Fanex.AccountingReports.Misc.Models.MemberWinLossOnRNGCasino;
    using Fanex.AccountingReports.Misc.Resources;
    using Fanex.AccountingReports.Misc.Services.Interface;
    using Fanex.Common;
    using Fanex.Data;
    using Fanex.Excel;
    using Fanex.Excel.Enums;
    using Fanex.Excel.Models;
    using NPOI.SS.UserModel;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class MemberWinLossOnRNGCasinoService : ServiceBase, IMemberWinLossOnRNGCasinoService
    {
        private const int ExtendCachedTimeOut = 168;

        private readonly string _recentReportCachedDataKey = "Fanex.AccountingReports.Misc.MemberWinLossOnRNGCasinoRecentReport" +
            (AppSession.CurrentUser != null ? AppSession.CurrentUser.Username : string.Empty);

        private readonly string _historyReportCachedDataKey = "Fanex.AccountingReports.Misc.MemberWinLossOnRNGCasinoHistoryReport" +
            (AppSession.CurrentUser != null ? AppSession.CurrentUser.Username : string.Empty);

        private readonly string _nextRefreshTimeKey = "Fanex.AccountingReports.Misc.MemberWinLossOnRNGCasinoRecentReport.nextRefreshTimeKey";
        private readonly string _lastRefreshTimeKey = "Fanex.AccountingReports.Misc.MemberWinLossOnRNGCasinoRecentReport.lastRefreshTimeKey";
        public static readonly string NumOfMemberKey = "Fanex.AccountingReports.Misc.MemberWinLossOnRNGCasinoRecentReport.numOfMemberKey";
        
        public List<MemberWinLoss> GetReportData(IMemberWinLossOnRNGCasinoCriteria criteria, bool isRecent)
        {
            List<MemberWinLoss> memberWinLosses = new List<MemberWinLoss>();
            try
            {
                Dictionary<int, string> exchangeCurrencies = ReportService.ExchangeCurrencies;
                using (ObjectDb objectDb = new ObjectDb(criteria.GetSettingKey()))
                {
                    IDataReader reader = objectDb.ExecuteReader(criteria.GetSpParams());
                    while (reader.Read())
                    {
                        MemberWinLoss winLoss = MappingWinLossData(reader, isRecent);
                        winLoss.CurrencyName = exchangeCurrencies[winLoss.Currency];
                        memberWinLosses.Add(winLoss);
                    }

                    CacheManager.Set<int>(NumOfMemberKey, memberWinLosses.Count);

                    reader.Close();

                    if (isRecent)
                    {
                        var blockTimes = memberWinLosses.Select(s => Converter.ToDateTime(s.TransactionTime)).Distinct().ToList();
                        var minDate = criteria.FromDate;
                        var maxDate = Converter.ToDateTime(criteria.ToDate.AddSeconds(1).ToString("yyyy-MM-dd HH:00"));
                        var time = Converter.ToDateTime(minDate.ToString("yyyy-MM-dd HH:00"));
                        while (time < maxDate)
                        {
                            bool isExistTime = blockTimes.Where(s => s == time).Count() > 0;
                            if (!isExistTime)
                            {
                                memberWinLosses.Add(new MemberWinLoss() 
                                {
                                    TransactionTime = time.ToString("yyyy-MM-dd HH:00")
                                });
                            }

                            time = time.AddHours(1);
                        }

                        memberWinLosses = memberWinLosses.OrderByDescending(s => Converter.ToDateTime(s.TransactionTime)).ThenBy(x => x.Member).ToList();
                    }
                }
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }

            return memberWinLosses;
        }

        public GridOptions GetRecentReport(IMemberWinLossOnRNGCasinoCriteria criteria)
        {
            ConvertTimeZone(criteria); 
            GridOptions gridOptions = InitCasinoMemberWinLossGridOptions(criteria);            
            try
            {
                var memberWinLosses = GetReportData(criteria, true);
                var dataRows = RecentReportDataRows(memberWinLosses);
                gridOptions.BodyRows = dataRows;
            }
            catch (Exception ex)
            {
                HandleException(ex);
                throw;
            }

            CacheManager.Set<GridOptions>(_recentReportCachedDataKey, gridOptions, DateTime.Now.AddHours(ExtendCachedTimeOut));
            return gridOptions;
        }

        public GridOptions GetHistoryReport(IMemberWinLossOnRNGCasinoCriteria criteria)
        {
            GridOptions gridOptions = InitCasinoMemberWinLossGridOptions(criteria);            
            try
            {
                var memberWinLosses = GetReportData(criteria, false);
                var dataRows = HistoryReportDataRows(memberWinLosses);
                gridOptions.BodyRows = dataRows;
            }
            catch (Exception ex)
            {
                HandleException(ex);
                throw;
            }

            CacheManager.Set<GridOptions>(_historyReportCachedDataKey, gridOptions, DateTime.Now.AddHours(ExtendCachedTimeOut));
            return gridOptions;
        }

        public IWorkbook GetRecentReportWorkbook(IMemberWinLossOnRNGCasinoCriteria criteria)
        {
            var gridOptions = CacheManager.Get<GridOptions>(_recentReportCachedDataKey);
            if (gridOptions == null)
            {
                gridOptions = this.GetRecentReport(criteria);
            }

            FanexExcel result = new FanexExcel(gridOptions);
            IWorkbook workbook = result.ToExcel2003(MiscResources.MemberWinLossOnRNGCasino_SheetName);
            return workbook;
        }

        public IWorkbook GetHistoryReportWorkbook(IMemberWinLossOnRNGCasinoCriteria criteria)
        {
            var gridOptions = CacheManager.Get<GridOptions>(_historyReportCachedDataKey);
            if (gridOptions == null)
            {
                gridOptions = this.GetHistoryReport(criteria);
            }

            FanexExcel result = new FanexExcel(gridOptions);
            IWorkbook workbook = result.ToExcel2003(MiscResources.MemberWinLossOnRNGCasino_SheetName);
            return workbook;
        }

        public string GetReportName(IMemberWinLossOnRNGCasinoCriteria criteria)
        {
            return string.Format(
                MiscResources.MemberWinLossOnRNGCasino_ReportName,
                criteria.FromDate.ToString("HH_mm_dd_MMM_yyyy"),
                criteria.ToDate.ToString("HH_mm_dd_MMM_yyyy"));
        }

        private MemberWinLoss MappingWinLossData(IDataReader reader, bool isRecent)
        {
            return new MemberWinLoss
            {
                Member = Converter.ToString2(reader["member"]),
                Currency = Converter.ToInt(reader["currency"]),
                Agent = Converter.ToString2(reader["agent"]),
                WinLoss = Converter.ToDecimal(reader["winlost"]),
                WinLossRM = Converter.ToDecimal(reader["winlost_rm"]),                
                NumberOfBet = isRecent ? Converter.ToInt(reader["NumOfBets"]) : 0,
                TransactionTime = isRecent ? Converter.ToString2(reader["Truncate"]) : string.Empty
            };
        }

        private GridOptions InitCasinoMemberWinLossGridOptions(IMemberWinLossOnRNGCasinoCriteria criteria)
        {
            return new GridOptions
            {
                Columns = HeaderColumns(),
                ShowFooter = false,
                ShowTitle = true,
                ShowHighlightRow = false,
                ShowDistinctRow = false,
                TitleType = TitleType.TwoLineBlue,
                TitleInfoText = string.Format(
                MiscResources.RNGCasinoReportTitle,
                criteria.Amount.ToString("0,0"),
                criteria.FromDate.ToString("MMM dd, yyyy HH:mm"),
                criteria.ToDate.AddSeconds(1).ToString("MMM dd, yyyy HH:mm"))
            };
        }

        [SuppressMessage("StyleCop.CSharp.Nexcel.NexcelCustomRules",
        "SP2100:CodeLineMustNotBeLongerThan",
        Justification = "Reviewed. Suppression is OK here.")]
        private List<HeaderColumns> HeaderColumns()
        {
            var header = new List<HeaderColumns>();
            header.AddRange(new List<HeaderColumns>()
            {
                new HeaderColumns() { HeaderText = Labels.Member, Name = Labels.Member, Width = 150, CellAlignment = HAlignment.Left },
                new HeaderColumns() { HeaderText = Labels.Agent, Name = Labels.Agent, Width = 150, CellAlignment = HAlignment.Left },
                new HeaderColumns() { HeaderText = Labels.Currency, Name = Labels.Currency, Width = 100, CellAlignment = HAlignment.Center },
                new HeaderColumns() { HeaderText = Labels.WinLossRM, Name = Labels.WinLossRM, Width = 200, CellAlignment = HAlignment.Right, CustomCellType = ExcelCellType.DoubleRedPositive },
                new HeaderColumns() { HeaderText = Labels.WinLossInMemberCurrency, Name = Labels.WinLossInMemberCurrency, Width = 200, CellAlignment = HAlignment.Right, CustomCellType = ExcelCellType.DoubleRedPositive }
            });

            return header;
        }

        private List<GridDataRow> HistoryReportDataRows(List<MemberWinLoss> memberWinLosses)
        {
            List<GridDataRow> dataRows = new List<GridDataRow>();
            foreach (var memberWinLoss in memberWinLosses)
            {
                var dataRow = BuildMemberWinLossDataRow(memberWinLoss, false);
                dataRows.Add(dataRow);
            }

            return dataRows;
        }

        private List<GridDataRow> RecentReportDataRows(List<MemberWinLoss> memberWinLosses)
        {
            List<GridDataRow> dataRows = new List<GridDataRow>();
            var memberWinLossByTransactionGroups = memberWinLosses.GroupBy(x => x.TransactionTime).ToList();
            foreach (var transactionGroup in memberWinLossByTransactionGroups)
            {
                var dataRow = new GridDataRow() { RowType = RowType.ParentLevelOne };
                string fromTime = Converter.ToDateTime(transactionGroup.Key).ToString("HH:mm");
                if (fromTime.Equals("00:00"))
                {
                    fromTime = Converter.ToDateTime(transactionGroup.Key).ToString("MMM dd, yyyy HH:mm");
                }

                string toTime = Converter.ToDateTime(transactionGroup.Key).AddMinutes(60).ToString("HH:mm");
                int numOfMembers = transactionGroup.Count();

                if (numOfMembers == 1)
                {
                    var wl = transactionGroup.FirstOrDefault();
                    numOfMembers = wl.NumberOfBet;
                }

                string blockTime = fromTime + " - " + toTime + " (" + numOfMembers + " member" + (numOfMembers > 1 ? "s" : string.Empty) + ")";
                dataRow.CreateColumns(new Columns { Name = Labels.Member, MergeColNumber = 5, Value = blockTime, Bold = true,  });
                foreach (var memberWinLoss in transactionGroup)
                {
                    var memberWinLossDataRow = BuildMemberWinLossDataRow(memberWinLoss, true);
                    dataRow.CreateSubRows(memberWinLossDataRow);
                }

                dataRows.Add(dataRow);
            }

            return dataRows;
        }

        private GridDataRow BuildMemberWinLossDataRow(MemberWinLoss winLoss, bool isRecent)
        {
            int columnIndex = 0;
            GridDataRow dataRow = new GridDataRow();

            if (winLoss.NumberOfBet == 0 && isRecent)
            {
                dataRow.CreateColumns(new Columns() { Name = Labels.Member, Value = Labels.nodata, MergeColNumber = 5, CellAlignment = HAlignment.Center, CssClass = "nodata" });
            }
            else
            {
                dataRow.Columns.AddRange(new List<Columns>()
                {
                    new Columns(columnIndex++) { Name = Labels.Member, Value = winLoss.Member },
                    new Columns(columnIndex++) { Name = Labels.Agent, Value = winLoss.Agent },
                    new Columns(columnIndex++) { Name = Labels.Currency, Value = winLoss.CurrencyName },
                    new Columns(columnIndex++) { Name = Labels.WinLossRM, Value = winLoss.WinLossRM,  },
                    new Columns(columnIndex++) { Name = Labels.WinLossInMemberCurrency, Value = winLoss.WinLoss }
                });
            }

            return dataRow;
        }

        private void ConvertTimeZone(IMemberWinLossOnRNGCasinoCriteria criteria)
        {
            var lastRefreshTime = GetLastRefreshTime();
            if (lastRefreshTime < criteria.FromDate)
            {
                var moveBetsDay = ReportService.BalanceUp.MoveBetsDay;
                if (lastRefreshTime < moveBetsDay)
                {
                    lastRefreshTime = moveBetsDay;
                }

                criteria.FromDate = lastRefreshTime;
            }

            CacheManager.Set<DateTime>(_nextRefreshTimeKey, criteria.ToDate.AddHours(1).AddSeconds(1));

            if (criteria.IsAutoRefresh)
            {
                SetLastRefreshTime(criteria.ToDate.AddSeconds(1));
            }
        }

        public DateTime GetNextRefreshTime()
        {
            return CacheManager.Get<DateTime>(_nextRefreshTimeKey);
        }

        public DateTime GetLastRefreshTime()
        {
            return CacheManager.Get<DateTime>(_lastRefreshTimeKey);
        }

        public void SetLastRefreshTime(DateTime? time)
        {            
            DateTime now = DateTime.Now;
            var lastTime = time.HasValue ? time.Value : ReportService.ConvertTimeZone(new DateTime(now.Year, now.Month, now.Day, now.Hour, 0, 0).AddHours(-1));
            CacheManager.Set<DateTime>(_lastRefreshTimeKey, lastTime, DateTime.Now.AddHours(ExtendCachedTimeOut));
        }
    }
}