namespace Fanex.AccountingReports.Gaming.Services.Interface.NumberGameFinancial
{
    using Fanex.AccountingReports.Gaming.Criteria.NumberGameFinancial;
    using Fanex.AccountingReports.Gaming.Models.NumberGameFinancial;

    public interface INumberGameFinancialService
    {
        NumberGameFinancial GetNumberGameFinancial(GetNumberGameFinancialCriteria getNumberGameFinancialCriteria);
    }
}
