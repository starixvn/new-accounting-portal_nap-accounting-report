// --------------------------------------------------------------------------
// <copyright file="ILiveCasinoGameStatisticService.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Louis.Nguyen" date="2012.12.07">Created</change>
// </history>

namespace Fanex.AccountingReports.Gaming.Services.Interface
{
    using System.Collections.Generic;
    using Fanex.AccountingReports.Core.Criteria;
    using Fanex.AccountingReports.Core.Models;
    using Fanex.AccountingReports.Gaming.Criteria.LiveCasinoGameStatistic;
    using Fanex.AccountingReports.Gaming.Models.LiveCasinoGameStatistic;

    public interface ILiveCasinoGameStatisticService
    {
        List<BaseModel> GetProductList();

        List<EntityBase<int, string>> GetCurrencyList(GetExchangeCurrenciesCriteria criteria);

        List<ProductGameType> GetProductGameTypes(GetGameTypeListCriteria criteria);

        List<EntityBase<int, string>> GetGameTableList(GetGameTableListCriteria criteria);

        WinLossInfo GetReport(GetReportCriteria criteria);

        WinLossInfo SortReport(GetReportCriteria getReportCriteria);

        void ExportReport(GetReportCriteria criteria);
    }
}