namespace Fanex.AccountingReports.Misc.Criteria.CreditSitesBetCounts
{
    using System;
    using Fanex.Data.Repository;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class GetCreditSitesBetCountsCriteria : CriteriaBase
    {
        public DateTime Datestart { get; set; }

        public DateTime Dateend { get; set; }

        public override string GetSettingKey()
        {
            return "Misc_CreditSiteBetCount_GetReport";
        }

        public override bool IsValid()
        {
            return this.Datestart <= this.Dateend;
        }
    }
}
