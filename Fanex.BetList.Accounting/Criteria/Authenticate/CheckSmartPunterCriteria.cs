namespace Fanex.BetList.Accounting.Criteria.Authenticate
{
    using Fanex.Data.Repository;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class CheckSmartPunterCriteria : CriteriaBase
    {
        public int? CustID { get; set; }

        public string UserName { get; set; }

        public override string GetSettingKey()
        {
            return "Acc_CustomerSelCheckSmartPunter";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}