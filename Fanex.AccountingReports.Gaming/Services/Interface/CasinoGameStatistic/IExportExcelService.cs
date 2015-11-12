// -----------------------------------------------------------------------
// <copyright file="IExportExcelService.cs" company="Microsoft">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Fanex.AccountingReports.Gaming.Services.Interface.CasinoGameStatistic
{
    using Fanex.AccountingReports.Gaming.Criteria.CasinoGameStatistic;
    using Fanex.AccountingReports.Gaming.Models.CasinoGameStatistic;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public interface IExportExcelService
    {
        void ExportReport(WinLossInfo report, GetReportByDailyCriteria criteria);
    }
}