// -----------------------------------------------------------------------
// <copyright file="IWinLossByLicenseeRepository.cs" company="Microsoft">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Fanex.AccountingReports.Licensee.Repositories.Interface
{
    using System.Data;
    using Fanex.AccountingReports.Licensee.Criteria.WinLossByLicensee;
    using Fanex.Data.Repository;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public interface IWinLossByLicenseeRepository
    {
        DataTable GetCurrencyReport(GetCurrencyReportCriteria criteria);

        DataTable GetLevelReport(CriteriaBase criteria);
    }
}
