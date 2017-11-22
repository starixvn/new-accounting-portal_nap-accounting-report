namespace Fanex.AccountingReports.WinLoss.Criteria.LeagueWinLoss
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using Fanex.AccountingReports.Core.Services.Impl;
    using Fanex.Data.Repository;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class GetLeagueWinLossBySportCriteria : CriteriaBase
    {
        public DateTime Winlostdate { get; set; }

        public int Leagueid { get; set; }

        [SpParam(Ignored = true)]
        public string LeagueName { get; set; }

        public int Sporttype { get; set; }

        [SpParam(Ignored = true)]
        public string SportName { get; set; }

        public override string GetSettingKey()
        {
            var storeProcedureSuffix = ReportService.GetStoreMappingSuffix(Winlostdate);
            if (storeProcedureSuffix == ReportService.WarehouseSPSuffix)
            {
                storeProcedureSuffix = ReportService.BetTrans14SPSuffix;
            }

            return "WinLoss_LeagueWinLoss_GetLeagueWinLossBySport" + storeProcedureSuffix;
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
