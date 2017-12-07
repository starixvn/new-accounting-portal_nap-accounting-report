namespace Fanex.AccountingReports.Racing.Services.Interface.RacingWinLossByLicensee
{
    using System.Collections.Generic;
    using Fanex.AccountingReports.Racing.Criteria.RacingWinLossByLicensee;
    using Fanex.AccountingReports.Racing.Models.RacingWinLossByLicensee;

    /// <summary>
    /// The interface of get list licensee service.
    /// </summary>
    public interface IRacingGetListLicenseeService
    {
        List<RacingLicensee> GetListLicensee(GetLicenseeCriteria criteria);
    }
}
