namespace Fanex.AccountingReports.LegendManagement.Repositories.Impls
{
    using System.Collections;
    using Fanex.AccountingReports.LegendManagement.Criteria;

    /// <summary>
    /// Legend Repository.
    /// </summary>
    public class LegendRepository : BaseRepository
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