namespace Fanex.AccountingReports.Core.Criteria
{
    using Data;
    using Fanex.AccountingReports.Core.Configurations;
    using Fanex.AccountingReports.Core.Web;
    using Fanex.Data.Repository;
    using Fanex.NAP.Core.Configuration;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class SelectMostVisitedPageCriteria : CriteriaBase
    {
        [SpParam(Ignored = true)]
        public static int MaxPages
        {
            get
            {
                return AppSettings.TopRateMostVisitedPages;
            }
        }

        public string Environment
        {
            get
            {
                return NapSettings.Settings.Environment;
            }
        }

        public int UserId
        {
            get
            {
                if (AppSession.CurrentUser != null)
                {
                    return AppSession.CurrentUser.Id;
                }

                return 0;
            }
        }

        public int SiteId
        {
            get
            {
                return NapSettings.Settings.SiteId;
            }
        }

        public override string GetSettingKey()
        {
            return "Home_MostVisitedPage_GetMostvisitedPage";
        }

        public override bool IsValid()
        {
            return AppSession.CurrentUser != null &&
                 NapSettings.Settings != null;
        }
    }
}
