namespace Fanex.AccountingReports.Gaming.Services.Interface
{
    using Fanex.AccountingReports.Gaming.Criteria.NumberGameStatistic;
    using Fanex.AccountingReports.Gaming.Models.NumberGameStatistic;

    public interface INumberGameStatisticService
    {
        NumberGameStatistic GetReport(GetNumberGameStatisticCriteria criteria);

        void ExportReport(GetNumberGameStatisticCriteria criteria);
    }
}