// -----------------------------------------------------------------------
// <copyright file="INumberGameMonthlyWinLossExportDataService.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
// <history>
//   <change who="Franklin.Nguyen" date="2013.10.12">Created</change>
// </history>

namespace Fanex.AccountingReports.Gaming.Services.Interface.NumberGameMonthlyWinLoss
{
    using Fanex.AccountingReports.Gaming.Criteria.NumberGameMonthlyWinLoss;

    public interface INumberGameMonthlyWinLossExportDataService
    {
        void ExportNumberGameMonthlyWinLossReport(NumberGameMonthlyWinLossCriteria getNumberGameMonthlyWinLossCriteria);
    }
}