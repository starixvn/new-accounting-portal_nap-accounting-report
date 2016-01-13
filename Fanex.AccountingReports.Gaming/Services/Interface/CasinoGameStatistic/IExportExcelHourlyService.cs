// -----------------------------------------------------------------------
// <copyright file="IExportExcelHourlyService.cs" company="Microsoft">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Fanex.AccountingReports.Gaming.Services.Interface.CasinoGameStatistic
{
    using Fanex.AccountingReports.Gaming.Criteria.CasinoGameStatistic;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public interface IExportExcelHourlyService
    {
        void ExportReport(GetReportByHourlyCriteria criteria);
    }
}