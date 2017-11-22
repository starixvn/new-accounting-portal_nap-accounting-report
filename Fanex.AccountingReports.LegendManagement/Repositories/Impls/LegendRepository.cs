namespace Fanex.AccountingReports.LegendManagement.Repositories.Impls
{
    using System.Collections;
    using Fanex.AccountingReports.LegendManagement.Criteria;
    using Fanex.AccountingReports.LegendManagement.Repositories.Interfaces;
    using Fanex.Common;

    /// <summary>
    /// Legend Repository.
    /// </summary>
    public class LegendRepository : BaseRepository, ILegendRepository
    {
        public Hashtable SaveReportLegend(SaveReportLegendCriteria criteria)
        {
            return ExecuteCommand(criteria);
        }

        public Hashtable SaveReportLegendMapping(SaveReportLegendMappingCriteria criteria)
        {
            return ExecuteCommand(criteria);
        }
    }
}
