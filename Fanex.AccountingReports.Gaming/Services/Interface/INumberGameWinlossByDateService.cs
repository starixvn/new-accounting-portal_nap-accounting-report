namespace Fanex.AccountingReports.Gaming.Services.Interface
{
    using System.Collections.Generic;
    using Fanex.AccountingReports.Core.Models;
    using Fanex.AccountingReports.Gaming.Criteria;
    using Fanex.AccountingReports.Gaming.Criteria.NumberGameWinLossByDate;
    using Fanex.AccountingReports.Gaming.Models.NumberGameWinLossByDate;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public interface INumberGameWinlossByDateService
    {
        List<EntityBase<int, string>> GetLicensee(GetNumberGameLicenseeCriteria getNumberGameLicenseeCriteria);

        List<EntityBase<int, string>> GetBetTypes(GetNumberGameBetTypeListCriteria getNumberGameBettypeCriteria);

        List<EntityBase<int, string>> GetCurrencies();

        NumberGameWinLossDetail GetNumberGameWinLossByDate(GetNumberGameWinLossByDateCriteria getNumberGameWinlossByDateCriteria);

        void ExportNumberGameByDate(GetNumberGameWinLossByDateCriteria getNumberGameWinlossByDateCriteria);
    }
}
