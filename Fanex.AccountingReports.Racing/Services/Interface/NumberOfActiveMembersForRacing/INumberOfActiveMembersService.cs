namespace Fanex.AccountingReports.Racing.Services.Interface.NumberOActiveMembersForRacing
{
    using Fanex.AccountingReports.Racing.Criteria.NumberOActiveMembersForRacing;
    using Fanex.AccountingReports.Racing.Models.NumberOActiveMembersForRacing;

    public interface INumberOfActiveMembersService
    {
        NumberOfActiveMembers GetNumberOfActiveMembers(NumberOfActiveMembersCriteria getNumberOfActiveMembersCriteria);
    }
}
