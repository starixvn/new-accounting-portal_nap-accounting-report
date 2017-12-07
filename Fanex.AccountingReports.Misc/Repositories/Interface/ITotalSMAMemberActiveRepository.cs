namespace Fanex.AccountingReports.Misc.Repositories.Interface
{
    using System.Collections;
    using System.Collections.Generic;
    using Fanex.AccountingReports.Misc.Criteria.TotalSMAMemberActive;
    using Fanex.Data.Repository;

    public interface ITotalSMAMemberActiveRepository
    {
        void GetReport(ICriteria criteria, ref dynamic totalSMA, ref IList<dynamic> activeDetails);

        IList<dynamic> GetStatusDetail(GetStatusDetailsCriteria criteria, ref Hashtable outputParam);
    }
}
