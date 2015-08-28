// --------------------------------------------------------------------------
// <copyright file="UpdateMostVisitedPageCountCriteria.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Louis.Nguyen" date="2012.10.18">Created</change>
// </history>

namespace Fanex.AccountingReports.Core.Criteria
{
    using Fanex.NAP.Framework.Data;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
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
