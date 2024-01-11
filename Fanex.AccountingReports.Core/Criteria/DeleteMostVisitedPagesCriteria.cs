namespace Fanex.AccountingReports.Core.Criteria
{
    using Fanex.AccountingReports.Core.Web;
    using Fanex.Data.Repository;

    public class DeleteMostVisitedPagesCriteria : CriteriaBase
    {
        public int CustId
        {
            get
            {
                return AppSession.CurrentUser.Id;
            }
        }

        public int MenuId { get; set; }

        public override string GetSettingKey()
        {
            return "Home_MostVisitedPage_DeleteMostvisitedPage";
        }

        public override bool IsValid()
        {
            return AppSession.CurrentUser != null;
        }
    }
}
