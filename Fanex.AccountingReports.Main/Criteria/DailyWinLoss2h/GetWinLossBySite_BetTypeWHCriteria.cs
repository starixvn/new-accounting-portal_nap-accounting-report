namespace Fanex.AccountingReports.Main.Criteria.DailyWinLoss2h
{
    using System;
    using Fanex.AccountingReports.Core.Services.Impl;
    using Fanex.AccountingReports.Main.Constants;
    using Fanex.Data.Repository;
    using CoreConstants = Fanex.AccountingReports.Core.Constants;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class GetWinLossBySite_BetTypeWHCriteria : CriteriaBase
    {
        public DateTime WinLostDate { get; set; }

        public int SportType { get; set; }

        public int BetType { get; set; }

        public bool LiveIndicator { get; set; }

        public bool IsHedge { get; set; }

        public int Site { get; set; }

        public int ProductId { get; set; }

        public override string GetSettingKey()
        {
            return "Main_DailyWinLoss2h_WinLossBySiteBetType_WH";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
