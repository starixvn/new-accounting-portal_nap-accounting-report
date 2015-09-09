// --------------------------------------------------------------------------
// <copyright file="SelectMostvisitedPageCriteria.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Louis.Nguyen" date="2012.10.18">Created</change>
// </history>

namespace Fanex.AccountingReports.Core.Criteria
{
    using Fanex.AccountingReports.Core.Configurations;
    using Fanex.AccountingReports.Core.Web;
    using Fanex.Data.Repository;
    using Fanex.NAP.Core.Configuration;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class SelectMostVisitedPageCriteria : CriteriaBase
    {
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