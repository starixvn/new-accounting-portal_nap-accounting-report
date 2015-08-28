// --------------------------------------------------------------------------
// <copyright file="IWinLossByBetTypeRepository.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Louis.Nguyen" date="2012.12.07">Created</change>
// </history>

namespace Fanex.AccountingReports.Gaming.Repositories.Interface.NumberGameWinLossByBetTypeMatch
{
    using System.Collections.Generic;
    using Fanex.AccountingReports.Gaming.Criteria.NumberGameWinLossByBetTypeMatch;
    using Fanex.AccountingReports.Gaming.Models.NumberGameWinLossByBetTypeMatch;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public interface IWinLossByBetTypeRepository
    {
        List<WinLossInfo> GetMainReport(GetWinLossByBetTypeMainReportCriteria criteria);

        List<WinLossDetailInfo> GetDetailReport(GetWinLossByBetTypeDetailReportCriteria criteria);
    }
}
