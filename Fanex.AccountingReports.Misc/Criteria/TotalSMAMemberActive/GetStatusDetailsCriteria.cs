namespace Fanex.AccountingReports.Misc.Criteria.TotalSMAMemberActive
{
    using System;
    using Data;
    using Fanex.Data.Repository;

    public class GetStatusDetailsCriteria : CriteriaBase
    {
        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public string SiteList { get; set; }

        // for title of detail report
        [SpParam(Ignored = true)]
        public string SiteName { get; set; }

        public int RoleId { get; set; }

        public bool ShowDetailNew
        {
            get
            {
                return this.Status == (int)Constants.Status.NewCreated;
            }

            set
            {
            }
        }

        public bool ShowDetailReject
        {
            get
            {
                return this.Status == (int)Constants.Status.Suspended;
            }

            set
            {
            }
        }

        public bool ShowDetailClosed
        {
            get
            {
                return this.Status == (int)Constants.Status.Closed;
            }

            set
            {
            }
        }

        #region Paging params

        public int PageSize { get; set; }

        public int PageIndex { get; set; }

        public int TotalRow { get; set; }

        #endregion Paging params

        #region Client params

        [SpParam(Ignored = true)]
        public int Status { get; set; }

        #endregion Client params

        public override string GetSettingKey()
        {
            return "Misc_TotalSMAMemberActive_GetStatusDetail";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
