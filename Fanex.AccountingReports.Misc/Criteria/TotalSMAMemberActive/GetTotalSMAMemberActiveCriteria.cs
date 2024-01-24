namespace Fanex.AccountingReports.Misc.Criteria.TotalSMAMemberActive
{
    using System;
    using Fanex.Data.Repository;

    public class GetTotalSMAMemberActiveCriteria : CriteriaBase
    {
        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public string SiteList { get; set; }

        public override string GetSettingKey()
        {
            return "Misc_TotalSMAMemberActive_GetReport";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
