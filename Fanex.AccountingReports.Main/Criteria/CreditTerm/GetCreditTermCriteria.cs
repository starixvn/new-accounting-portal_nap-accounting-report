namespace Fanex.AccountingReports.Main.Criteria.CreditTerm
{
    using Fanex.Data.Repository;

    /// <summary>
    /// The criteria of Credit Term report.
    /// </summary>
    public class GetCreditTermCriteria : CriteriaBase
    {
        public GetCreditTermCriteria()
        {
            CheckerName = null;
            Pre1 = null;
            Pre2 = null;
        }

        public string CheckerName { get; set; }

        /// <summary>
        /// Gets or sets the first Pre-Agreed Amount value.
        /// </summary>
        /// <value>Pre-Agreed Amount.</value>
        public decimal? Pre1 { get; set; }

        /// <summary>
        /// Gets or sets the second Pre-Agreed Amount value.
        /// </summary>
        /// <value>Pre-Agreed Amount.</value>
        public decimal? Pre2 { get; set; }

        public override string GetSettingKey()
        {
            return "CreditTerm";
        }

        public override bool IsValid()
        {
            return Pre1 <= Pre2;
        }
    }
}
