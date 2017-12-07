namespace Fanex.AccountingReports.Main.Repositories.Interface
{
    using System.Collections;
    using System.Collections.Generic;
    using Fanex.Data.Repository;

    public interface IWinLossSummaryRepository
    {
        List<dynamic> GetReport(ICriteria criteria, ref Hashtable outputParam);
    }
}
