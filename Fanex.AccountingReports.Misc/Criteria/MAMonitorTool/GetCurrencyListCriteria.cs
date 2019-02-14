namespace Fanex.AccountingReports.Misc.Criteria.MAMonitorTool
{
    using Data;
    using Fanex.AccountingReports.Core.Constants;
    using Fanex.Data.Repository;

    /// <summary>
    /// Get Currency List Criteria.
    /// </summary>
    public class GetCurrencyListCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "system")]
        public string SystemName
        {
            get
            {
                return SystemNameConstants.Accounting;
            }
        }

        public override string GetSettingKey()
        {
            return "NAP_ExchangeCurrencySel";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
