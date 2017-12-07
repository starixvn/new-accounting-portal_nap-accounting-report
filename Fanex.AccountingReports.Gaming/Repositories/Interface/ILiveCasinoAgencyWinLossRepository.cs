namespace Fanex.AccountingReports.Gaming.Repositories.Interface
{
    using System.Collections.Generic;
    using Fanex.AccountingReports.Gaming.Criteria.LiveCasinoWinLossByAgency;

    public interface ILiveCasinoAgencyWinLossRepository
    {
        List<dynamic> GetReport(GetReportCriteria criteria);
    }
}
