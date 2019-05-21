namespace Fanex.AccountingReports.Racing.Services.Implement.NumberOActiveMembersForRacing
{
    using System;
    using Fanex.AccountingReports.Core.Services.Impl;
    using Fanex.AccountingReports.Core.Web;
    using Fanex.AccountingReports.Racing.Criteria.NumberOActiveMembersForRacing;
    using Fanex.AccountingReports.Racing.Models.NumberOActiveMembersForRacing;
    using Fanex.Data;

    /// <summary>
    /// Racing - Number Of Active Member Service.
    /// </summary>
    public class NumberOfActiveMembersService : ServiceBase
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
            var result = new NumberOfActiveMembers();

            using (var db = new ObjectDb(getNumberOfActiveMembersCriteria.GetSettingKey()))
            {
                var param = getNumberOfActiveMembersCriteria.GetSpParams();
                result = db.QueryEntity<NumberOfActiveMembers>(param);
            }

            SessionManager.Set<NumberOfActiveMembers>(_numberOfActiveMemberReportSessionKey, result);
            return result;
        }
    }
}