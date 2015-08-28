// -----------------------------------------------------------------------
// <copyright file="IVirtualSportsWinLossGetReportService.cs" company="Nexcel Solutions Vietnam">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Fanex.AccountingReports.Gaming.Services.Interface.VirtualSportWinLoss
{
    using System.Collections.Generic;
    using Fanex.AccountingReports.Gaming.Criteria.VirtualSportsWinLoss;
    using Fanex.AccountingReports.Gaming.Models.VirtualSportsWinLoss;

    /// <summary>
    /// The interface to  get report service.
    /// </summary>
    public interface IVirtualSportsWinLossGetReportService
    {
        VirtualSportsWinLoss GetReport(GetVirtualSportsWinLossCriteria criteria);

        List<SportsType> GetSportsCollection();
    }
}
