// -----------------------------------------------------------------------
// <copyright file="NumberOfActiveMembersService.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
// <history>
//   <change who="Franklin.Nguyen" date="2013.12.12">Created</change>
// </history>

namespace Fanex.AccountingReports.Racing.Services.Implement.NumberOActiveMembersForRacing
{
    using System;
    using Fanex.AccountingReports.Core.Services.Impl;
    using Fanex.AccountingReports.Core.Web;
    using Fanex.AccountingReports.Racing.Criteria.NumberOActiveMembersForRacing;
    using Fanex.AccountingReports.Racing.Models.NumberOActiveMembersForRacing;
    using Fanex.AccountingReports.Racing.Services.Interface.NumberOActiveMembersForRacing;
    using Fanex.Data;

    /// <summary>
    /// Racing - Number Of Active Member Service.
    /// </summary>
    public class NumberOfActiveMembersService : ServiceBase, INumberOfActiveMembersService
    {
        private string _numberOfActiveMemberReportSessionKey = "Fanex.AccountingReports.Racing.NumberOfActiveMember." +
            (AppSession.CurrentUser != null ? AppSession.CurrentUser.Username : string.Empty);

        /// <summary>
        /// Gets the number of active member.
        /// </summary>
        /// <param name="getNumberOfActiveMembersCriteria">The get number of active members criteria.</param>
        /// <returns>Number Of Active Member.</returns>
        public NumberOfActiveMembers GetNumberOfActiveMembers(NumberOfActiveMembersCriteria getNumberOfActiveMembersCriteria)
        {
            try
            {
                var result = new NumberOfActiveMembers();

                using (var db = new ObjectDb(getNumberOfActiveMembersCriteria.GetSettingKey()))
                {
                    var param = getNumberOfActiveMembersCriteria.GetSpParams();
                    result = db.QueryEntity<NumberOfActiveMembers>(param);
                }

                SessionManager.Set<NumberOfActiveMembers>(_numberOfActiveMemberReportSessionKey, result);
                return result;
            }
            catch (Exception ex)
            {
                HandleException(ex);
                throw;
            }
        }
    }
}