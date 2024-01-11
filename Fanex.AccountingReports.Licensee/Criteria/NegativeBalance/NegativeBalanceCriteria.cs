namespace Fanex.AccountingReports.Licensee.Criteria.NegativeBalance
{
    using Data;
    using Fanex.Data.Repository;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class NegativeBalanceCriteria : CriteriaBase
    {
        public string Site { get; set; }

        [SpParam(Ignored = true)]
        public int PageSize { get; set; }

        [SpParam(Ignored = true)]
        public int PageIndex { get; set; }

        public override bool IsValid()
        {
            return true;
        }

        public override string GetSettingKey()
        {
            return "Licensee_NegativeBalance_GetReport";
        }
    }
}
