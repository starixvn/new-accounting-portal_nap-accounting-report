// --------------------------------------------------------------------------
// <copyright file="IWinLossSummaryService.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Louis.Nguyen" date="2012.12.07">Created</change>
// </history>

namespace Fanex.AccountingReports.Main.Services.Interface
{
    using Fanex.AccountingReports.Main.Models.WinLossSummary;
    using Fanex.Data.Repository;

    public interface IWinLossSummaryService
    {
        WinLossInfo GetReport(CriteriaBase criteria);
    }
}
