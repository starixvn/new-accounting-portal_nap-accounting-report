// --------------------------------------------------------------------------
// <copyright file="CommonConstants.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Louis.Nguyen" date="2012.12.07">Created</change>
// </history>

namespace Fanex.AccountingReports.Core.Constants
{
    using System;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public static class CommonConstants
    {
        public const string CurrentLogInUser = "Fanex.AccountingReports.Core.Contants.AppSession.CurrentLogInUser";
        public const string CurrentUserMenu = "Fanex.AccountingReports.Core.Contants.AppSession.CurrentUserMenu";
        public const string CurrentUserRoleAccess = "Fanex.AccountingReports.Core.Contants.AppSession.CurrentUserRoleAccess";
        public const string CurrentWorkingOnMenu = "Fanex.AccountingReports.Core.Contants.AppSession.CurrentWorkingOnMenu";
        public const string IsTrackedMenu = "Fanex.AccountingReports.Core.Contants.AppSession.IsTrackedMenu";
        public const string CurrentActionName = "Fanex.AccountingReports.Core.Contants.AppSession.CurrentActionName";

        public const string MistVisitedPageCacheKey = "Fanex.AccountingReports.Core.Contants.AppSession.IntervalGetMostVisitedPage";
        public const string SPParamSeparator = ",";
        public const string ViewIPVN = "ViewIPVN";
        public const int MaxJsonLength = 50000000;
        public static readonly object MostVisitedPageLockObject = new object();
        public static readonly int CurrencyIdBaseRM = -1;
    }
}
