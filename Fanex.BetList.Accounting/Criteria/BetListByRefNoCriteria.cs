namespace Fanex.BetList.Accounting.Criteria
{
    using Fanex.Data.Repository;

    public class BetListByRefNoCriteria : CriteriaBase
    {
        public long RefNo { get; set; }

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
