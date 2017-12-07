using System;
using Fanex.Data.Repository;

namespace Fanex.AccountingReports.Main.Criteria.WinLossSummary3rd
{
    public class GetWinLossSummary3rdBullfightingSportTypeCriteria : CriteriaBase
    {
        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public override string GetSettingKey()
        {
            return "Acc_Rpt_WinlostSummary_Sporttype_BF";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
