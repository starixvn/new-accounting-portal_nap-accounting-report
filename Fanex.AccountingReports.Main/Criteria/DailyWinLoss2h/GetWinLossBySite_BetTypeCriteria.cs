namespace Fanex.AccountingReports.Main.Criteria.DailyWinLoss2h
{
    using System;
    using Data;
    using Fanex.AccountingReports.Core.Services.Impl;
    using Fanex.Data.Repository;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class GetWinLossBySite_BetTypeCriteria : CriteriaBase
    {
        public DateTime WinLostDate { get; set; }

        public int SportType { get; set; }

        public int BetType { get; set; }

        public bool LiveIndicator { get; set; }

        public bool IsHedge { get; set; }

        public int Site { get; set; }

        [SpParam(Ignored = true)]
        public string PreviousBreadScrum { get; set; }

        [SpParam(Ignored = true)]
        public int SportGroupId { get; set; }

        [SpParam(Ignored = true)]
        public int ProductId { get; set; }

        public override string GetSettingKey()
        {
            var winLossDateSuffix = ReportService.GetStoreMappingSuffix(this.WinLostDate);
            return string.Format("Main_DailyWinLoss2h_WinLossBySiteBetType{0}", winLossDateSuffix);
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}