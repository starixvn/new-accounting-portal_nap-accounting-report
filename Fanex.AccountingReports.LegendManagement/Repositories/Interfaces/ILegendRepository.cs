namespace Fanex.AccountingReports.LegendManagement.Repositories.Interfaces
{
    using System.Collections;
    using Fanex.AccountingReports.LegendManagement.Criteria;

    public interface ILegendRepository
    {
        Hashtable SaveReportLegend(SaveReportLegendCriteria criteria);

        Hashtable SaveReportLegendMapping(SaveReportLegendMappingCriteria criteria);
    }
}
