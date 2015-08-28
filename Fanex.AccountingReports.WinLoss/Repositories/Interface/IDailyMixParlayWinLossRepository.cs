// --------------------------------------------------------------------------
// <copyright file="IDailyMixParlayWinLossRepository.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Louis.Nguyen" date="2012.12.07">Created</change>
// </history>

namespace Fanex.AccountingReports.WinLoss.Repositories.Interface
{
    using System.Collections.Generic;
    using Fanex.AccountingReports.WinLoss.Criteria.DailyMixParlayWinLoss;

    public interface IDailyMixParlayWinLossRepository
    {
        List<dynamic> GetCombinationWinLossReport(GetCombinationWinLossReportCriteria criteria);
    }
}
