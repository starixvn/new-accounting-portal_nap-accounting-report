namespace Fanex.AccountingReports.Main.Criteria.DailyWinLoss2h
{
    using System;
    using Fanex.AccountingReports.Core.Services.Impl;
    using Fanex.AccountingReports.Main.Constants;
    using Fanex.Data.Repository;
    using CoreConstants = Fanex.AccountingReports.Core.Constants;

    public class GetWinLossBySite_CasinoCriteria : CriteriaBase
    {
        public DateTime WinLostDate { get; set; }

        public int BetType { get; set; }

        public int Site { get; set; }

        public int ProductId { get; set; }

        /// <summary>
        /// Gets the setting key.
        /// </summary>
        /// <returns>The setting key.</returns>
        public override string GetSettingKey()
        {
            var winLossDateSuffix = ReportService.GetStoreMappingSuffix(this.WinLostDate);

            return string.Format("Main_DailyWinLoss2h_WinLossBySiteBetType_Casino{0}", winLossDateSuffix);
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
