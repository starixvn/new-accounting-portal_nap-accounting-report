// -----------------------------------------------------------------------
// <copyright file="IVirtualSportsWinLossDetailService.cs" company="Nexcel Solutions Vietnam">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Fanex.AccountingReports.Gaming.Services.Interface.VirtualSportWinLoss
{
    using System.Collections.Generic;
    using Fanex.AccountingReports.Gaming.Criteria.VirtualSportsWinLoss;
    using Fanex.AccountingReports.Gaming.Models.VirtualSportsWinLoss;

    /// <summary>
    /// The interface of Virtual Sports Win Loss Detail service.
    /// </summary>
    public interface IVirtualSportsWinLossDetailService
    {
        List<VirtualSportsWinLossDetail> GetDetailReport(GetVirtualSportsWinLossDetailCriteria criteria);

        void ExportDetailReport(GetVirtualSportsWinLossDetailCriteria criteria);
    }
}
