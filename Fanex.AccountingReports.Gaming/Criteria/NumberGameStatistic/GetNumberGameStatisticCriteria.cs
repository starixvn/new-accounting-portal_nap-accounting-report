namespace Fanex.AccountingReports.Gaming.Criteria.NumberGameStatistic
{
    using System;
    using Fanex.Data.Repository;

    public class GetNumberGameStatisticCriteria : CriteriaBase
    {
        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public DateTime MoveBetsDay { get; set; }

        public string UserName { get; set; }

        public int? LeagueId { get; set; }

        public string BetType { get; set; }

        public int ReportBy { get; set; }

        public int TotalPlayers { get; set; }

        [SpParam(Ignored = true)]
        public int PageIndex { get; set; }

        [SpParam(Ignored = true)]
        public int PageSize { get; set; }

        [SpParam(Ignored = true)]
        public bool IsPagingCallBack { get; set; }

        public override bool IsValid()
        {
            return true;
        }

        public override string GetSettingKey()
        {
            return "NumberGameStatistics_GetReport";
        }
    }
}