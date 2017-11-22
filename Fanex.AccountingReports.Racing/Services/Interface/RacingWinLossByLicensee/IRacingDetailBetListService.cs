namespace Fanex.AccountingReports.Racing.Services.Interface.RacingWinLossByLicensee
{
    using System.Collections.Generic;
    using Fanex.AccountingReports.Racing.Criteria.RacingWinLossByLicensee;
    using Fanex.AccountingReports.Racing.Models.RacingWinLossByLicensee;

    /// <summary>
    /// The interface of get detail bet list.
    /// </summary>
    public interface IRacingDetailBetListService
    {
        List<DetailBetList> GetDetailBetList(GetDetailBetListCriteria criteria);
    }
}
