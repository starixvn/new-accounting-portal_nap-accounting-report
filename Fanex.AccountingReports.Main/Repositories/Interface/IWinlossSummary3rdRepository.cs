namespace Fanex.AccountingReports.Main.Repositories.Interface
{
    using Fanex.Data.Repository;

    public interface IWinlossSummary3rdRepository
    {
        void RecalculateWinLoss(ICriteria criteria);
    }
}
