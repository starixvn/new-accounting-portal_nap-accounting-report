// --------------------------------------------------------------------------
// <copyright file="INumberGameWinlossService.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Ricky.Nguyen" date="2012.11.8">Created</change>
// </history>

namespace Fanex.AccountingReports.Licensee.Services.Interface
{
    using System.Collections.Generic;
    using Fanex.AccountingReports.Licensee.Criteria;
    using Fanex.AccountingReports.Licensee.Criteria.NumberGameWinLoss;
    using Fanex.AccountingReports.Licensee.Models;
    using Fanex.AccountingReports.Licensee.Models.NumberGameWinLoss;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public interface INumberGameWinLossService
    {
        List<Licensee> GetAllLicensees(GetLicenseeByResellerCriteria selAllLicenseeCriteria);

        NumberGameWinLoss GetNumberGameWinLoss(GetNumberGameWinLossCriteria numberGameWinLossCriteria);

        void ExportExcelNumberGameWinLossReport(GetNumberGameWinLossCriteria numberGameWinLossCriteria);
    }
}