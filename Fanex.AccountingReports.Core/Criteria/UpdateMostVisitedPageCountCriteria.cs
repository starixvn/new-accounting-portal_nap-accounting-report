namespace Fanex.AccountingReports.Core.Criteria
{
    using Fanex.Data.Repository;

    public class UpdateMostVisitedPageCountCriteria : CriteriaBase
    {
        public int CustId { get; set; }

        public string MenuId { get; set; }

        public string Count { get; set; }

        public override string GetSettingKey()
        {
            return "Home_MostVisitedPage_UpdateMostvisitedPageCount";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
