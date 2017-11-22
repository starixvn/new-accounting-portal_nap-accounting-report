using Fanex.Data.Repository;

namespace Fanex.BetList.Accounting.Criteria
{
    public class GetCashOutDetailCriteria : CriteriaBase
    {
        public long TransId { get; set; }

        public long RefNo { get; set; }

        public short BetType { get; set; }

        public long OriginalTransId { get; set; }

        [SpParam(Ignored = true)]
        public string Title { get; set; }

        public override string GetSettingKey()
        {
            return "Acc_BetList_CashOut_Detail";
        }

        public override bool IsValid()
        {
            return TransId > 0 &&
                RefNo > 0 &&
                BetType > 0 &&
                OriginalTransId > 0;
        }
    }
}
