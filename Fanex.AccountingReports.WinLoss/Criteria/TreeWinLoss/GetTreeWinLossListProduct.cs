using Fanex.Data.Repository;

namespace Fanex.AccountingReports.WinLoss.Criteria.TreeWinLoss
{
    public class GetTreeWinLossListProduct : CriteriaBase
    {
        public override bool IsValid() => true;

        public override string GetSettingKey() => "Acc_Rpt_TreeWinlostListProduct";
    }
}