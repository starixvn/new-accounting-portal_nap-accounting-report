// -----------------------------------------------------------------------
// <copyright file="INumberOfActiveMembersService.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
// <history>
//   <change who="Franklin.Nguyen" date="2013.12.12">Created</change>
// </history>

namespace Fanex.AccountingReports.Racing.Services.Interface.NumberOActiveMembersForRacing
{
    using Fanex.AccountingReports.Racing.Criteria.NumberOActiveMembersForRacing;
    using Fanex.AccountingReports.Racing.Models.NumberOActiveMembersForRacing;

    public interface INumberOfActiveMembersService
    {
        NumberOfActiveMembers GetNumberOfActiveMembers(NumberOfActiveMembersCriteria getNumberOfActiveMembersCriteria);
    }
}