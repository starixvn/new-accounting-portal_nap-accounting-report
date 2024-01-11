namespace Fanex.AccountingReports.Gaming.Criteria
{
    using Fanex.Data.Repository;

    public class GetNumberGameBetTypeListCriteria : CriteriaBase
    {
        public override bool IsValid()
        {
            return true;
        }

        public override string GetSettingKey()
        {
            return "Gaming_NumberGameWinLossByDate_GetBetType";
        }
    }
}
