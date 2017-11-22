namespace Fanex.BetList.Accounting.Criteria
{
    using Fanex.Data.Repository;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class BetListByRefNoCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "TransID")]
        public long TransId { get; set; }

        public override string GetSettingKey()
        {
            return "BetList_RefNoSel";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
