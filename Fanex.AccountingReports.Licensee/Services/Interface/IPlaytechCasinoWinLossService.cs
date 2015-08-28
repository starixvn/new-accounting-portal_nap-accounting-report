// -----------------------------------------------------------------------
// <copyright file="IPlaytechCasinoWinLossService.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
// <history>
//   <change who="Franklin.Nguyen" date="2014.04.14">Created</change>
// </history>

namespace Fanex.AccountingReports.Licensee.Services.Interface
{
    using Fanex.AccountingReports.Licensee.Criteria.PlaytechCasinoWinLoss;
    using Fanex.AccountingReports.Licensee.Models.PlaytechCasinoWinLoss;

    public interface IPlaytechCasinoWinLossService
    {
        PlaytechCasinoWinLoss GetPlaytechCasinoWinLoss(GetPlaytechCasinoWinLossCriteria criteria);

        void ExportPlaytechCasinoWinLoss(GetPlaytechCasinoWinLossCriteria criteria);
    }
}