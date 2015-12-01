// --------------------------------------------------------------------------
// <copyright file="ILiveCasinoGameStatisticRepository.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Louis.Nguyen" date="2012.12.07">Created</change>
// </history>

namespace Fanex.AccountingReports.Gaming.Repositories.Interface
{
    using System.Collections.Generic;
    using Fanex.AccountingReports.Core.Models;
    using Fanex.AccountingReports.Gaming.Criteria.LiveCasinoGameStatistic;
    using Fanex.AccountingReports.Gaming.Models.LiveCasinoGameStatistic;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public interface ILiveCasinoGameStatisticRepository
    {
        List<List<WinLossInfoEntity>> GetReport(GetReportCriteria criteria);
    }
}