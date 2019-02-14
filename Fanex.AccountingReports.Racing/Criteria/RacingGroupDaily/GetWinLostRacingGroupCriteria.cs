namespace Fanex.AccountingReports.Racing.Criteria.RacingGroupDaily
{
    using System;
    using Data;
    using Fanex.Data.Repository;

    /// <summary>
    /// The criteria of Racing Group.
    /// </summary>
    public class GetWinLostRacingGroupCriteria : CriteriaBase
    {
        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        [SpParam(Ignored = true)]
        public bool IsPagingCallback { get; set; }

        [SpParam(Ignored = true)]
        public int StartIndex { get; set; }

        [SpParam(Ignored = true)]
        public int DatesPerPage { get; set; }

        public override string GetSettingKey()
        {
            return "Racing_WinLossGroup";
        }

        public override bool IsValid()
        {
            return FromDate <= ToDate;
        }
    }
}
