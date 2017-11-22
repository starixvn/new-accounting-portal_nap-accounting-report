namespace Fanex.AccountingReports.Licensee.Repositories.Interface
{
    using System.Collections.Generic;
    using Fanex.AccountingReports.Licensee.Models.NegativeBalance;
    using Fanex.Data.Repository;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public interface INegativeBalanceRepository
    {
        List<NegativeBalanceUser> GetNegativeBalanceUser(ICriteria criteria);
    }
}
