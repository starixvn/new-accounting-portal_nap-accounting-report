namespace Fanex.AccountingReports.Gaming.Criteria.NumberOfActiveMember
{
    using Fanex.Data.Repository;

    public class GetProductCriteria : CriteriaBase
    {
        public override bool IsValid()
        {
            return true;
        }

        public override string GetSettingKey()
        {
            return "Gaming_NumberOfActiveMember_Products";
        }
    }
}
