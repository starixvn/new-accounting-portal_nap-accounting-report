namespace Fanex.AccountingReports.WinLoss.Services.Interface
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Fanex.AccountingReports.WinLoss.Criteria.GetAllCustomerTurnOver;
    using Fanex.AccountingReports.WinLoss.Models.AllCustomerTurnOver;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public interface IAllCustomerTurnOverGetReportService
    {
        CustomerTurnoverBySite GetReport(GetAllCustomerTurnOverCriteria criteria);
    }
}
