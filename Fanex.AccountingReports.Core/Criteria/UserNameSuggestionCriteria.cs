// --------------------------------------------------------------------------
// <copyright file="UserNameSuggestionCriteria.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Louis.Nguyen" date="2012.10.18">Created</change>
// </history>

namespace Fanex.AccountingReports.Core.Criteria
{
    using Fanex.AccountingReports.Core.Constants;
    using Fanex.AccountingReports.Core.Services.Impl;
    using Fanex.AccountingReports.Core.Web;
    using Fanex.Data.Repository;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class UserNameSuggestionCriteria : CriteriaBase
    {
        public string Username { get; set; }

        public int Custid
        {
            get
            {
                return AppSession.CurrentUser.Id;
            }
        }

        public string System
        {
            get
            {
                return SystemNameConstants.Accounting;
            }
        }

        public bool IsAllowFISearch
        {
            get
            {
                return PermissionService.HasPermission(AccReportFunctionKeys.ViewFinanceAccount);
            }
        }

        [SpParam(Ignored = true)]
        public bool ExcludeTestAccounts { get; set; }

        public override string GetSettingKey()
        {
            return "Core_ReportService_UserNameSuggestion_Select";
        }

        public override bool IsValid()
        {
            return AppSession.CurrentUser != null;
        }
    }
}