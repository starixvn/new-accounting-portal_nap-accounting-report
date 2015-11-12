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
    using Fanex.AccountingReports.Gaming.Criteria.CasinoGameStatistic;
    using Fanex.AccountingReports.Gaming.Models.CasinoGameStatistic;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public interface ICasinoGameStatisticRepository
    {
        List<List<WinLossInfoEntity>> GetReport(GetReportByDailyCriteria criteria);

        ProductList GetProductList(GetProductListCriteria criteria);

        List<GameTypeItem> GetGameType(GetGameTypeListCriteria criteria);

        List<EntityBase<int, string>> GetGameTableList(GetGameTableListCriteria criteria);

        List<WinLossInfo> GetReportByGrandTotal(GetReportByGrandRMCriteria criteria);

        Hourly GetReportByHourly(GetReportByHourlyCriteria criteria);
    }
}
