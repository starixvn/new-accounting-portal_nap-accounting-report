using Fanex.Data.Repository;

namespace Fanex.AccountingReports.Misc.Criteria.BetCount
{
    public class GetProductsCriteria : CriteriaBase
    {
        public int IMode { get; set; } = 3;

        public string ProductID { get; set; }

        public override string GetSettingKey()
        {
            return "Misc_Bet_Count_GetProducts";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}