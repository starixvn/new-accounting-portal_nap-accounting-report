// -----------------------------------------------------------------------
// <copyright file="ITreeWinLossRepository.cs" company="Nexcel Solutions Vietnam">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Fanex.AccountingReports.WinLoss.Repositories.Interface
{
    using System.Collections;
    using System.Collections.Generic;
    using Fanex.AccountingReports.WinLoss.Models.TreeWinLoss;
    using Fanex.Data.Repository;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public interface ITreeWinLossRepository
    {
        List<TreeWinLossInfo> GetTreeWinLossByProduct(ICriteria criteria, ref Hashtable outputParam);
    }
}
