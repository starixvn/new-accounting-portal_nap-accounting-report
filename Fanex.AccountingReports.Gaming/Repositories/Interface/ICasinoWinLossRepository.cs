namespace Fanex.AccountingReports.Gaming.Repositories.Interface
{
    using System.Data;
    using Fanex.AccountingReports.Gaming.Criteria.CasinoWinLoss;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public interface ICasinoWinLossRepository
    {
        DataTable GetData(GetCasinoWinLossCriteria criteria);
    }
}
