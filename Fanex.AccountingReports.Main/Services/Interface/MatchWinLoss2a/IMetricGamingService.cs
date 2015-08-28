// -----------------------------------------------------------------------
// <copyright file="IMetricGamingService.cs" company="Nexcel Solutions Vietnam">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Fanex.AccountingReports.Main.Services.Interface.MatchWinLoss2a
{
    using Fanex.AccountingReports.Main.Criteria.MatchWinLoss2a;
    using Fanex.Excel.Models;
    using NPOI.SS.UserModel;

    /// <summary>
    /// Metric gaming service.
    /// </summary>
    public interface IMetricGamingService
    {
        GridOptions GetMetricGamingByBettypeReport(GetMetricGamingCriteria criteria);

        IWorkbook GetMetricGamingByBettypeReportWorkboook(GetMetricGamingCriteria criteria);

        string GetMetricGamingByBetypeReportName(GetMetricGamingCriteria criteria);
    }
}