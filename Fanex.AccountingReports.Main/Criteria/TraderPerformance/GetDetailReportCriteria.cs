namespace Fanex.AccountingReports.Main.Criteria.TraderPerformance
{
    using System;
    using Fanex.AccountingReports.Core.Criteria;
    using Fanex.Data.Repository;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class GetDetailReportCriteria : CriteriaBase, IPagingCriteria
    {
        public DateTime Fromdate { get; set; }

        public DateTime Todate { get; set; }

        public bool IsSoccer { get; set; }

        [SpParam(Ignored = true)]
        public string SortDirection { get; set; }

        [SpParam(Ignored = true)]
        public string SortColumnName { get; set; }

        [SpParam(Ignored = true)]
        public int PageSize { get; set; }

        [SpParam(Ignored = true)]
        public int PageIndex { get; set; }

        [SpParam(Ignored = true)]
        public int StartIndex
        {
            get
            {
                return (PageIndex - 1) * PageSize;
            }
        }

        [SpParam(Ignored = true)]
        public int EndIndex
        {
            get
            {
                return (PageIndex * PageSize) - 1;
            }
        }

        [SpParam(Ignored = true)]
        public bool IsPagingCallBack { get; set; }

        public override string GetSettingKey()
        {
            return "Main_TraderPerformance_GetDetailReport";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}