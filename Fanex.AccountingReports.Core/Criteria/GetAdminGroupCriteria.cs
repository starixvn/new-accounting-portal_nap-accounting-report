namespace Fanex.AccountingReports.Core.Criteria
{
    using Fanex.Data.Repository;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class GetAdminGroupCriteria : CriteriaBase
    {
        public string UserName { get; set; }

        public override bool IsValid()
        {
            return true;
        }

        public override string GetSettingKey()
        {
            return "Acc_AdminGroupSel";
        }
    }
}
