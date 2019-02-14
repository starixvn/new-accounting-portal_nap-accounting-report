namespace Fanex.AccountingReports.Balance.Criteria.DirectBalance
{
    using Data;
    using Fanex.AccountingReports.Core.Constants;
    using Fanex.Data.Repository;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class GetDirectBalanceCriteria : CriteriaBase
    {
        [SpParam(Ignored = true)]
        public int RoleID { get; set; }

        [SpParam(Ignored = true)]
        public int BalanceType { get; set; }

        public int? Currency { get; set; }

        public string Username { get; set; }

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
            var storeProcedureName = string.Empty;
            switch (RoleID)
            {
                case RoleConstants.Master:
                    storeProcedureName = "Balance_DirectBalance_DirectMaster";
                    break;
                case RoleConstants.Agent:
                    storeProcedureName = "Balance_DirectBalance_DirectAgent";
                    break;
                case RoleConstants.Member:
                    storeProcedureName = "Balance_DirectBalance_DirectMember";
                    break;
            }

            if (!string.IsNullOrEmpty(storeProcedureName) && BalanceType == 2)
            {
                storeProcedureName += "Yesterday";
            }

            return storeProcedureName;
        }
    }
}
