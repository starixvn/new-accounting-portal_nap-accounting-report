using Fanex.Data.Repository;

namespace Fanex.AccountingReports.Misc.Criteria.BetCount
{
    public class GetProductsCriteria : CriteriaBase
    {
        //public bool IsSB { get; set; }

        public int IMode { get; set; } = 3;

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
