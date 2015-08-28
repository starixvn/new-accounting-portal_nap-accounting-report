// -----------------------------------------------------------------------
// <copyright file="IRacingWinLossByLicenseeService.cs" company="Nexcel Solutions Viet Nam">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Fanex.AccountingReports.Racing.Services.Interface.RacingWinLossByLicensee
{
    using Fanex.AccountingReports.Racing.Criteria.RacingWinLossByLicensee;
    using Fanex.AccountingReports.Racing.Models.RacingWinLossByLicensee;

    /// <summary>
    /// The interface of get report service of Racing Win Loss By Licensee.
    /// </summary>
    public interface IRacingWinLossByLicenseeService
    {
        RacingDailyReport GetReport(GetRacingDailyReportCriteria criteria);
    }
}
