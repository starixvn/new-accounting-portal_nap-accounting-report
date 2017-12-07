namespace Fanex.AccountingReports.Main.Criteria.DailyWinLoss2h
{
    using System;
    using Fanex.AccountingReports.Core.Services.Impl;
    using Fanex.Data.Repository;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class GetWinLossBySite_ColossusBetCriteria : CriteriaBase
    {
        public DateTime WinLostDate { get; set; }

        public int Site { get; set; }

        /// <summary>
        /// Gets the setting key.
        /// </summary>
        /// <returns>The setting key.</returns>
        public override string GetSettingKey()
        {
            var winLossDateSuffix = ReportService.GetStoreMappingSuffix(this.WinLostDate);

            return string.Format("Main_DailyWinLoss2h_WinLossBySiteBetType_ColossusBet{0}", winLossDateSuffix);
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
