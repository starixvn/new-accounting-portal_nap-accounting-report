namespace Fanex.AccountingReports.Main.Services.Implement.MatchWinLoss2a
{
    using System.Collections.Generic;
    using Core.Services.Impl;
    using Data;
    using Fanex.AccountingReports.Core.Web;
    using Fanex.AccountingReports.Main.Criteria.MatchWinLoss2a;
    using Fanex.AccountingReports.Main.Models.MatchWinLoss2a;
    using Fanex.AccountingReports.Main.Services.Interface.MatchWinLoss2a;
    using Fanex.Common;
    using Fanex.Data.Repository;

    public class CockFightingService : ServiceBase, ICockFightingService
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

            using (ObjectDb db = new ObjectDb(criteria.GetSettingKey()))
            {
                db.AssignParamValues(criteria.GetSpParams());
                var reader = db.ExecuteReader();
                while (reader.Read())
                {
                    var row = new CockFightingDetail
                    {
                        ArenaId = Converter.ToInt(reader["ArenaId"]),
                        WinLoss = Converter.ToDecimal(reader["WinLoss"]),
                        Position = Converter.ToDecimal(reader["Position"]),
                        Commission = Converter.ToDecimal(reader["Commission"]),
                        VirtualCommission = Converter.ToDecimal(reader["VirtualCommission"]),
                        RealLineTotal = Converter.ToDecimal(reader["RealLineTotal"]),
                        VirtualLineTotal = Converter.ToDecimal(reader["VirtualLineTotal"]),
                        RealTurnover = Converter.ToDecimal(reader["RealTurnover"])
                    };

                    report.ReportRows.Add(row);
                }
            }

            report.GrandTotal = this.CalculateGrandTotal(report.ReportRows);

            CacheExtension.Set(_cockFightingReportCachedDataKey, report);
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
            var rawData = this.GetReportData<dynamic>(criteria);
            foreach (var data in rawData)
            {
                var row = new CockFightMatchDetail
                {
                    FightNo = Converter.ToInt(data.FightNo),
                    HomeOwner = Converter.ToString2(data.HomeOwner),
                    AwayOwner = Converter.ToString2(data.AwayOwner),
                    WinLoss = Converter.ToDecimal(data.WinLoss),
                    Position = Converter.ToDecimal(data.Position),
                    Commission = Converter.ToDecimal(data.Commission),
                    RealLineTotal = Converter.ToDecimal(data.RealLineTotal),
                    VirtualLineTotal = Converter.ToDecimal(data.VirtualLineTotal),
                    RealTurnover = Converter.ToDecimal(data.RealTurnover)
                };
                report.ReportRows.Add(row);
            }

            report.GrandTotal = this.CalculateGrandTotalByArena(report.ReportRows);

            CacheExtension.Set(_cockFightingMatchByArenaReportCachedDataKey, report);
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