namespace Fanex.AccountingReports.Racing.Criteria.RacingGroupDaily
{
    using System;
    using Fanex.Data.Repository;

    /// <summary>
    /// The criteria of Racing Group Daily.
    /// </summary>
    public class GetWinLostRacingGroupDailyCriteria : CriteriaBase
    {
        public DateTime WinLostDate { get; set; }

        public override string GetSettingKey()
        {
            return "Racing_WinLostGroupDaily";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
