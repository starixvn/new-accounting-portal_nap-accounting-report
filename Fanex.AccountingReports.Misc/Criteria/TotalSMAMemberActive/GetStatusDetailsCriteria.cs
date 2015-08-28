// --------------------------------------------------------------------------
// <copyright file="GetStatusDetailsCriteria.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Kan.Nguyen" date="2013.07.12">Created</change>
// </history>

namespace Fanex.AccountingReports.Misc.Criteria.TotalSMAMemberActive
{
    using System;
    using Fanex.NAP.Framework.Data;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class GetStatusDetailsCriteria : GetTotalSMAMemberActiveCriteria, ICriteria
    {
        public new DateTime FromDate { get; set; }

        public new DateTime ToDate { get; set; }

        public new string SiteList { get; set; }

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

        [SpParam(Ignored = true)]
        public int TotalRow { get; set; }

        #endregion Paging params

        #region Client params

        [SpParam(Ignored = true)]
        public int Status { get; set; }

        #endregion

        public override string GetSettingKey()
        {
            return "Misc_TotalSMAMemberActive_GetStatusDetail";
        }
    }
}