// --------------------------------------------------------------------------
// <copyright file="DeleteMostVisitedPagesCriteria.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Louis.Nguyen" date="2012.10.18">Created</change>
// </history>

namespace Fanex.AccountingReports.Core.Criteria
{
    using Fanex.AccountingReports.Core.Web;
    using Fanex.NAP.Framework.Data;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
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
