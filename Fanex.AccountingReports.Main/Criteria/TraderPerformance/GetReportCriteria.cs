namespace Fanex.AccountingReports.Main.Criteria.TraderPerformance
{
    using System;
    using Fanex.Data.Repository;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class GetReportCriteria : CriteriaBase
    {
        public DateTime Fromdate { get; set; }

        public DateTime Todate { get; set; }

        public bool IsSoccer { get; set; }

        [SpParam(Ignored = true)]
        public string SortDirection { get; set; }

        [SpParam(Ignored = true)]
        public string SortColumnName { get; set; }

        public override string GetSettingKey()
        {
            return "Main_TraderPerformance_GetReport";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}