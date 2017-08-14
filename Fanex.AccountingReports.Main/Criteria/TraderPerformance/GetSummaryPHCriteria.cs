using System;
using Fanex.Data.Repository;

namespace Fanex.AccountingReports.Main.Criteria.TraderPerformance
{
    public class GetSummaryPHCriteria : CriteriaBase
    {
        public DateTime Fromdate { get; set; }

        public DateTime Todate { get; set; }

        public bool IsSoccer { get; set; }

        public override string GetSettingKey()
        {
            return "Main_TraderPerformance_GetReport_PH";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}