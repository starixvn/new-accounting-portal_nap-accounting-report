namespace Fanex.AccountingReports.Main.Services.Implement.MatchWinLoss2a
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Fanex.AccountingReports.Core.Web;
    using Fanex.AccountingReports.Main.Criteria.MatchWinLoss2a;
    using Fanex.AccountingReports.Main.Models.MatchWinLoss2a;
    using Fanex.AccountingReports.Main.Services.Interface.MatchWinLoss2a;
    using Fanex.Data.Repository;

    public class CockFightingService : ICockFightingService
    {
        private const int ExtendCachedTimeOut = 2;

        private string _cockFightingReportCachedDataKey = "Fanex.AccountingReports.Main.MatchWinLoss2a.Cockfighting" +
            (AppSession.CurrentUser != null ? AppSession.CurrentUser.Username : string.Empty);

        private string _cockFightingMatchByArenaReportCachedDataKey = "Fanex.AccountingReports.Main.MatchWinLoss2a.CockfightingMatchByArena" +
           (AppSession.CurrentUser != null ? AppSession.CurrentUser.Username : string.Empty);

        private IDynamicRepository _repository;

        public CockFightingService()
            : this(null)
        {
        }

        public CockFightingService(IDynamicRepository repository)
        {
            this._repository = repository ?? new DynamicRepository();
        }

        public CockFighting GetCockFightingReport(GetCockFightingCriteria criteria)
        {
            var report = new CockFighting()
            {
                ReportRows = new List<CockFightingDetail>(),
                GrandTotal = new CockFightingDetail()
            };

            report.ReportRows = _repository.Fetch<CockFightingDetail>(criteria).ToList();
            report.GrandTotal = this.CalculateGrandTotal(report.ReportRows);

            CacheManager.Set<CockFighting>(_cockFightingReportCachedDataKey, report, DateTime.Now.AddHours(ExtendCachedTimeOut));
            return report;
        }

        public CockFightingDetail CalculateGrandTotal(List<CockFightingDetail> report)
        {
            var grandTotal = new CockFightingDetail();

            foreach (var item in report)
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

        public CockFightingMatchByArena GetCockFightingMatchByArenaReport(GetCockFightingMatchByArena criteria)
        {
            var report = new CockFightingMatchByArena()
            {
                ReportRows = new List<CockFightMatchDetail>(),
                GrandTotal = new CockFightMatchDetail()
            };

            report.ReportRows = _repository.Fetch<CockFightMatchDetail>(criteria).ToList();
            report.GrandTotal = this.CalculateGrandTotalByArena(report.ReportRows);

            CacheManager.Set<CockFightingMatchByArena>(_cockFightingMatchByArenaReportCachedDataKey, report, DateTime.Now.AddHours(ExtendCachedTimeOut));
            return report;
        }

        public CockFightMatchDetail CalculateGrandTotalByArena(List<CockFightMatchDetail> report)
        {
            var grandTotal = new CockFightMatchDetail();

            foreach (var item in report)
            {
                grandTotal.WinLoss += item.WinLoss;
                grandTotal.Position += item.Position;
                grandTotal.Commission += item.Commission;
                grandTotal.RealLineTotal += item.RealLineTotal;
                grandTotal.VirtualLineTotal += item.VirtualLineTotal;
                grandTotal.RealTurnover += item.RealTurnover;
            }

            return grandTotal;
        }
    }
}