namespace Fanex.AccountingReports.Gaming.Services.Interface.NumberGameFinancial
{
    using Fanex.AccountingReports.Gaming.Criteria.NumberGameFinancial;

    public interface INumberGameFinancialExportDataService
    {
        void ExportNumberGameFinancial(GetNumberGameFinancialCriteria getNumberGameFinancialCriteria);
    }
}
