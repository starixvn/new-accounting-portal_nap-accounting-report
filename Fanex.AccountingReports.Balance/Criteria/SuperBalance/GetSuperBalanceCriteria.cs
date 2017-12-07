namespace Fanex.AccountingReports.Balance.Criteria.SuperBalance
{
    using Fanex.AccountingReports.Core.Constants;
    using Fanex.Data.Repository;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class GetSuperBalanceCriteria : CriteriaBase
    {
        [SpParam(Ignored = true)]
        public int BalanceType { get; set; }

        [SpParam(SqlParamName = "Keyword")]
        public string Username { get; set; }

        [SpParam(SqlParamName = "FirstChar")]
        public string FirstLetter { get; set; }

        [SpParam(SqlParamName = "Currency")]
        public int? Currency { get; set; }

        [SpParam(SqlParamName = "IsViewTotalOnly")]
        public bool IsViewTotalOnly { get; set; }

        public double Total { get; set; }  

        public override bool IsValid()
        {
            return true;
        }

        /// <summary>
        /// Gets the setting key.
        /// </summary>
        /// <returns>Setting key.</returns>
        public override string GetSettingKey()
        {
            var storeProcedureName = "Balance_SuperBalance_Super";
           
            if (!string.IsNullOrEmpty(storeProcedureName) && BalanceType == 2)
            {
                storeProcedureName += "Yesterday";
            }

            return storeProcedureName;
        }
    }
}
