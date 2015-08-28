// -----------------------------------------------------------------------
// <copyright file="IVirtualSportsWinLossRepository.cs" company="Nexcel Solutions Vietnam">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Fanex.AccountingReports.Gaming.Repositories.Interface
{
    using Fanex.AccountingReports.Gaming.Criteria.VirtualSportsWinLoss;
    using Fanex.AccountingReports.Gaming.Models.VirtualSportsWinLoss;

    /// <summary>
    /// The interface of Virtual Sports Win Loss repository.
    /// </summary>
    public interface IVirtualSportsWinLossRepository
    {
        VirtualSportsWinLoss GetVirtualSportsWinLossReport(GetVirtualSportsWinLossCriteria criteria);
    }
}
