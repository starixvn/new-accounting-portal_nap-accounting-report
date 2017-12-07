namespace Fanex.AccountingReports.Racing.Repositories.Interface
{
    using System.Data;
    using Fanex.AccountingReports.Racing.Criteria.RacingGroupDaily;

    /// <summary>
    /// The interface of Racing Group Daily repository.
    /// </summary>
    public interface IRacingGroupDailyRepository
    {
        DataTable GetDataTable(GetWinLostRacingGroupCriteria criteria);
    }
}
