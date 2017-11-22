namespace Fanex.AccountingReports.Gaming.Repositories.Interface
{
    using System.Collections;
    using System.Collections.Generic;
    using Fanex.AccountingReports.Core.Models;
    using Fanex.AccountingReports.Gaming.Criteria.LiveCasinoGameFinancialReport;
    using Fanex.AccountingReports.Gaming.Models.LiveCasinoGameFinancialReport;

    public interface ILiveCasinoGameFinancialReportRepository
    {
        List<EntityBase<int, string>> GetCurrencies();

        List<dynamic> GetGameTypes(GetGameTypesCriteria criteria);

        List<dynamic> GetGameTables(GetGameTableCriteria criteria);

        List<dynamic> GetReseller(GetResellerCriteria criteria);

        List<FinancialReport> GetFinancialReport(GetFinancialReportCriteria criteria, ref Hashtable outputParam);
    }
}
