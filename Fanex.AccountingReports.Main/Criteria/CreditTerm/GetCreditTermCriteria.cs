namespace Fanex.AccountingReports.Main.Criteria.CreditTerm
{
    using Fanex.Data.Repository;

    public class GetCreditTermCriteria : CriteriaBase
    {
        public GetCreditTermCriteria()
        {
            CheckerName = null;
            Pre1 = null;
            Pre2 = null;
        }

        public string CheckerName { get; set; }

        public decimal? Pre1 { get; set; }

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
