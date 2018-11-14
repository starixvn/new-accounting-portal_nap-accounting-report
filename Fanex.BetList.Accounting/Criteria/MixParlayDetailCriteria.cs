namespace Fanex.BetList.Accounting.Criteria
{
    using Fanex.Data.Repository;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class MixParlayDetailCriteria
    {
        [SpParam(SqlParamName = "refno")]
        public string RefNo { get; set; }

        public string TransId { get; set; }

        public int CustId { get; set; }

        public string GetSettingKey()
        {
            return "BetList_MixParlayBetsSel";
        }
    }
}
