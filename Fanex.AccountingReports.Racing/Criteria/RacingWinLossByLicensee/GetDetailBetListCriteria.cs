namespace Fanex.AccountingReports.Racing.Criteria.RacingWinLossByLicensee
{
    using System;
    using Fanex.AccountingReports.Core.Services.Impl;
    using Fanex.Data.Repository;

    /// <summary>
    /// The criteria to get detail bet list.
    /// </summary>
    public class GetDetailBetListCriteria : CriteriaBase
    {
        public int LeagueID { get; set; }

        public string RaceNo { get; set; }

        public int Currency { get; set; }

        public DateTime WinLostDate { get; set; }

        public string Site { get; set; }

        /// <summary>
        /// Get setting key.
        /// </summary>
        /// <returns>Return key.</returns>
        public override string GetSettingKey()
        {
            string suffix = string.Empty;
            if (WinLostDate > ReportService.BalanceUp.MoveBetsDay)
            {
                suffix = string.Empty;
            }
            else
            {
                suffix = "_14";
            }

            return "Racing_RacingWinLostLicensee_DetailBetList" + suffix;
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
