namespace Fanex.AccountingReports.Special.Criteria.CheckBet
{
    using System;
    using Data;
    using Fanex.Data.Repository;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class CheckBetCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "winlostdate")]
        public DateTime WinLossDate { get; set; }

        [SpParam(Ignored = true)]
        public int TicketType { get; set; }

        public override bool IsValid()
        {
            return true;
        }

        /// <summary>
        /// Gets the setting key.
        /// </summary>
        /// <returns>The setting key.</returns>
        public override string GetSettingKey()
        {
            string storeProcedureName = "Special_CheckBet_GetReport";
            if (TicketType == 1)
            {
                storeProcedureName += "_Normal";
            }
            else if (TicketType == 2)
            {
                storeProcedureName += "_Reject";
            }

            return storeProcedureName;
        }
    }
}
