namespace Fanex.AccountingReports.WinLoss.Criteria.LeagueWinLoss
{
    using System;
    using Fanex.AccountingReports.Core.Services.Impl;
    using Fanex.Data.Repository;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class GetLeagueWinLossCriteria : CriteriaBase
    {
        public GetLeagueWinLossCriteria()
        {
            WinLost_8RM = 0;
            WinLost_BARM = 0;
        }

        public DateTime Winlostdate { get; set; }

        public double WinLost_8RM { get; set; }

        public double WinLost_BARM { get; set; }

        public override string GetSettingKey()
        {
            var storeProcedureSuffix = ReportService.GetStoreMappingSuffix(Winlostdate);
            if (storeProcedureSuffix == ReportService.WarehouseSPSuffix)
            {
                storeProcedureSuffix = string.Empty;
            }

            return "WinLoss_LeagueWinLoss_GetLeagueWinLoss" + storeProcedureSuffix;
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
