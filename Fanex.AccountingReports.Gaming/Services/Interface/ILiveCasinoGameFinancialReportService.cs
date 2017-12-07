namespace Fanex.AccountingReports.Gaming.Services.LiveCasinoGameFinancialReport
{
    using System.Collections.Generic;
    using Fanex.AccountingReports.Core.Models;
    using Fanex.AccountingReports.Gaming.Criteria.LiveCasinoGameFinancialReport;
    using Fanex.AccountingReports.Gaming.Models.LiveCasinoGameFinancialReport;
    using Fanex.AccountingReports.Gaming.Models.NumberGameFinancialReport;

    public interface ILiveCasinoGameFinancialReportService
    {
        List<BaseModel> GetLicensees();

        List<BaseModel> GetCurrencies();

        List<ProductGameType> GetGameTypes(GetGameTypesCriteria criteria);

        List<BaseModel> GetGameTables(GetGameTableCriteria criteria);

        FinancialReport GetFinancialReport(GetFinancialReportCriteria criteria);

        void ExportFinancialReport(GetFinancialReportCriteria criteria);
    }
}
