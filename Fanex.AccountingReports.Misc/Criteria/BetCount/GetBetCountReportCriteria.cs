namespace Fanex.AccountingReports.Misc.Criteria.BetCount
{
    using System;
    using Data;
    using Fanex.Data.Repository;

    public class GetBetCountReportCriteria : CriteriaBase
    {
        public GetBetCountReportCriteria()
        {
            this.Datestart = DateTime.Now.AddDays(-1);
            this.Dateend = this.Datestart;
        }

        public DateTime Datestart { get; set; }

        public DateTime Dateend { get; set; }

        public string SportBettype { get; set; }

        [SpParam(Ignored = true)]
        public string SortBy { get; set; }

        [SpParam(Ignored = true)]
        public string SortDirection { get; set; }

        [SpParam(Ignored = true)]
        public bool IsHistoryReport { get; set; }

        public override string GetSettingKey()
        {
            return "Misc_BetCount_GetBetCountReport";
        }

        public override bool IsValid()
        {
            return this.Datestart <= this.Dateend;
        }
    }
}
