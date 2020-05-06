namespace Fanex.AccountingReports.WinLoss.Criteria.LeagueWinLoss
{
    using Fanex.Data.Repository;
    using System;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class GetLeagueWinLossCriteria : CriteriaBase
    {

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public string SportTypeList { get; set; }

        public override string GetSettingKey()
        {

            return "WinLoss_LeagueWinLoss_GetLeagueWinLoss";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
