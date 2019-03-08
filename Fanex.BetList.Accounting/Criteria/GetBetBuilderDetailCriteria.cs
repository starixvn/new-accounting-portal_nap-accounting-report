using Fanex.Data.Repository;

namespace Fanex.BetList.Accounting.Criteria
{
    public class GetBetBuilderDetailCriteria : CriteriaBase
    {
        public long TransId { get; set; }

        public long CustId { get; set; }


        public override string GetSettingKey() => "Acc_BetList_BetBuilder_Detail";

        public override bool IsValid() => true;

    }
}
