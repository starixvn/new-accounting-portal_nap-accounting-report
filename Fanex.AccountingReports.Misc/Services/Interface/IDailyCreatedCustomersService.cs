namespace Fanex.AccountingReports.Misc.Services.Interface
{
    using System.Collections.Generic;
    using Fanex.AccountingReports.Misc.Criteria.DailyCreatedCustomers;
    using Fanex.AccountingReports.Misc.Models.DailyCreatedCustomers;

    public interface IDailyCreatedCustomersService
    {
        SummaryInformation GetReport(GetDailyCreatedCustomersCriteria criteria);

        List<DetailInformation> GetReportDetails(GetDailyCreatedCustomersDetailsCriteria criteria);

        void ExportReport(GetDailyCreatedCustomersCriteria criteria);

        void ExportReportDetails(GetDailyCreatedCustomersDetailsCriteria criteria);
    }
}
