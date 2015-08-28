// -----------------------------------------------------------------------
// <copyright file="ICasinoWinLossGetReportService.cs" company="Nexcel Solutions Viet Nam">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Fanex.AccountingReports.Gaming.Services.Interface.CasinoWinLoss
{
    using System.Collections.Generic;
    using Fanex.AccountingReports.Gaming.Criteria.CasinoWinLoss;
    using Fanex.AccountingReports.Gaming.Models.CasinoWinLoss;

    /// <summary>
    /// The get report service of Casino Win Loss report.
    /// </summary>
    public interface ICasinoWinLossGetReportService
    {
        CasinoWinLoss GetReport(GetCasinoWinLossCriteria criteria);
    }
}
