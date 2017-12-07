namespace Fanex.AccountingReports.Test.Criteria
{
    using System;
    using Fanex.Data.Repository;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class ExternalDailyWinlossCriteria : CriteriaBase, ICriteria
    {
        public DateTime Winlostdate { get; set; }

        public int? Custid { get; set; }

        public string GroupName { get; set; }

        public override bool IsValid()
        {
            return true;
        }

        public override string GetSettingKey()
        {
            return "External_DailyWinLoss_GetReport";
        }
    }
}
