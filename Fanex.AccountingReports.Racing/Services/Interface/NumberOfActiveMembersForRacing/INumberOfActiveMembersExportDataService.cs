namespace Fanex.AccountingReports.Racing.Services.Interface.NumberOActiveMembersForRacing
{
    using Fanex.AccountingReports.Racing.Criteria.NumberOActiveMembersForRacing;

    public interface INumberOfActiveMembersExportDataService
    {
        void ExportNumberOfActiveMembers(NumberOfActiveMembersCriteria getNumberOfActiveMembersCriteria);
    }
}
