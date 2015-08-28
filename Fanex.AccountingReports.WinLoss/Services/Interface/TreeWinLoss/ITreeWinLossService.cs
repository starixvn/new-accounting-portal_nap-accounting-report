// -----------------------------------------------------------------------
// <copyright file="ITreeWinLossService.cs" company="Nexcel Solutions Vietnam">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Fanex.AccountingReports.WinLoss.Services.Interface.TreeWinLoss
{
    using System.Collections.Generic;
    using Fanex.AccountingReports.WinLoss.Criteria.TreeWinLoss;
    using Fanex.AccountingReports.WinLoss.Models.TreeWinLoss;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public interface ITreeWinLossService
    {
        TreeWinLoss GetTreeWinLossByProduct(GetTreeWinLossByProductCriteria criteria);

        TreeWinLossVirtualRate GetTreeWinLossByLevel(GetTreeWinLossByLevelCriteria criteria);

        VirtualRateInfo GetVirtualRate(int custId);
    }
}
