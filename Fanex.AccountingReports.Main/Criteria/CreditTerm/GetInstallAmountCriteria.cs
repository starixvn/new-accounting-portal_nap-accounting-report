namespace Fanex.AccountingReports.Main.Criteria.CreditTerm
{
    using Fanex.Data.Repository;

    public class GetInstallAmountCriteria : CriteriaBase
    {
        public int RoleID { get; set; }

        public override string GetSettingKey()
        {
            return "InstallAmount";
        }

        public override bool IsValid()
        {
            return RoleID == 1 || RoleID == 2 || RoleID == 3 || RoleID == 4;
        }
    }
}
