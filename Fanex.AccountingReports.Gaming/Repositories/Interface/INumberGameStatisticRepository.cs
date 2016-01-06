namespace Fanex.AccountingReports.Gaming.Repositories.Interface
{
    using Fanex.AccountingReports.Gaming.Criteria.NumberGameStatistic;
    using Fanex.AccountingReports.Gaming.Models.NumberGameStatistic;

    public interface INumberGameStatisticRepository
    {
        NumberGameStatisticByBettype GetNumberGameStatisticByBettype(GetNumberGameStatisticCriteria criteria);

        NumberGameStatisticByDaily GetNumberGameStatisticByDaily(GetNumberGameStatisticCriteria criteria);

        NumberGameStatisticByLeague GetNumberGameStatisticByLeague(GetNumberGameStatisticCriteria criteria);

        NumberGameStatisticByUserName GetNumberGameStatisticByUserName(GetNumberGameStatisticCriteria criteria);
    }
}