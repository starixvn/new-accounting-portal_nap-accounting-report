namespace Fanex.AccountingReports.WinLoss.Services.Interface.AllCustomerTurnOver
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Fanex.AccountingReports.WinLoss.Criteria.GetAllCustomerTurnOver;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public interface IAllCustomerTurnOverExportReportService
    {
        void ExportReport(GetAllCustomerTurnOverCriteria criteria);
    }
}
