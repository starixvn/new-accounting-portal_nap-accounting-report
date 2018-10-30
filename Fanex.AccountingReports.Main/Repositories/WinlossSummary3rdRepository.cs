namespace Fanex.AccountingReports.Main.Repositories.Implement
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class WinlossSummary3rdRepository
    {
        public void RecalculateWinLoss(ICriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                db.ExecuteNonQuery(criteria.GetSpParams());
            }
        }
    }
}