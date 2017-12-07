namespace Fanex.AccountingReports.Licensee.Services.Interface
{
    using Fanex.AccountingReports.Licensee.Criteria.PeakConcurrentUser;
    using Fanex.AccountingReports.Licensee.Models.PeakConcurrentUser;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public interface IPeakConcurrentUserService
    {
        PeakConcurrentUser GetConcurrentUserList(PeakConcurrentUserCriteria peakConcurrentUserCriteria);

        void PeakExport2Excel(PeakConcurrentUserCriteria peakConcurrentUserCriteria);
    }
}
