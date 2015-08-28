// --------------------------------------------------------------------------
// <copyright file="ILiveCasinoWinLossService.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="David.Tran" date="2012.11.08">Created</change>
// </history>

namespace Fanex.AccountingReports.Licensee.Services.Interface
{
    using System.Collections.Generic;
    using Fanex.AccountingReports.Licensee.Criteria;
    using Fanex.AccountingReports.Licensee.Criteria.LiveCasinoWinLoss;
    using Fanex.AccountingReports.Licensee.Models;
    using Fanex.AccountingReports.Licensee.Models.LiveCasinoWinLoss;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public interface ILiveCasinoWinLossService
    {
        List<Licensee> GetAllLicensees(GetLicenseeByResellerCriteria selAllLicenseeCriteria);

        List<GameType> GetGameType();

        LiveCasinoWinLoss GetLiveCasinoWinLoss(GetLiveCasinoWinLossCriteria liveCasinoCretia);

        void ExportExcelLiveCasinoWinLossReport(GetLiveCasinoWinLossCriteria liveCasinoCretia);
    }
}
