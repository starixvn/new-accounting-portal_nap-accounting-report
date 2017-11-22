namespace Fanex.AccountingReports.Balance.Repositories.Interface
{
    using System.Collections;
    using System.Collections.Generic;
    using Fanex.AccountingReports.Balance.Criteria.SuperBalance;
    using Fanex.AccountingReports.Balance.Models.SuperBalance;
    using Fanex.Data.Repository;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public interface ISuperBalanceRepository
    {
        List<dynamic> GetSuperBalance(GetSuperBalanceCriteria criteria, ref Hashtable outputParam);

        List<SuperBalance> GetSuperBalanceDownline(ICriteria criteria, ref Hashtable outputParam);

        List<SuperBalance> GetSuperBalanceMasterDownline(ICriteria criteria, ref Hashtable outputParam);
    }
}
