// -----------------------------------------------------------------------
// <copyright file="IRacingWinLossByLicenseeApply50PercentService.cs" company="Nexcel Solutions Viet Nam">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Fanex.AccountingReports.Racing.Services.Interface.RacingWinLossByLicensee
{
    using Fanex.AccountingReports.Racing.Criteria.RacingWinLossByLicensee;

    /// <summary>
    /// The export report service of Racing Win Loss By Licensee apply 50 percent.
    /// </summary>
    public interface IRacingWinLossByLicenseeApply50PercentService
    {
        void ExportReportApply50Percent(GetRacingDailyReportCriteria criteria);
    }
}
