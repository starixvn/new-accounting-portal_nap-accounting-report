// --------------------------------------------------------------------------
// <copyright file="IVirtualSportWinlossService.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Robert.Tran" date="2012.11.26">Created</change>
// </history>

namespace Fanex.AccountingReports.Licensee.Services.Interface
{
    using System.Collections.Generic;
    using Fanex.AccountingReports.Licensee.Criteria;
    using Fanex.AccountingReports.Licensee.Criteria.VirtualSportWinLoss;
    using Fanex.AccountingReports.Licensee.Models;
    using Fanex.AccountingReports.Licensee.Models.VirtualSportWinLoss;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public interface IVirtualSportWinLossService
    {
        List<Licensee> GetAllLicensees(GetLicenseeByResellerCriteria selAllLicenseeCriteria);

        List<SportType> GetSportType();

        VirtualSportWinLossNEW SelVirtualSportWinLoss(GetVirtualSportWinLossCriteria virtualSportWinLossCriteria);

        void ExportExcelVirtualSportsWinLossReport(GetVirtualSportWinLossCriteria virtualSportWinLossCriteria);
    }
}