// -----------------------------------------------------------------------
// <copyright file="ILiveCasinoProductService.cs" company="Nexcel Solutions Vietnam">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Fanex.AccountingReports.Gaming.Services.Interface.CasinoGameStatistic
{
    using System.Collections.Generic;
    using Fanex.AccountingReports.Core.Criteria;
    using Fanex.AccountingReports.Core.Models;
    using Fanex.AccountingReports.Gaming.Criteria.CasinoGameStatistic;
    using Fanex.AccountingReports.Gaming.Models.CasinoGameStatistic;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public interface IFilterSectionService
    {
        List<string> GetProductList();

        List<ProductItem> Get3rdPartyByProduct(string product);

        List<EntityBase<int, string>> GetCurrencyList(GetExchangeCurrenciesCriteria criteria);

        List<ProductGameType> GetProductGameTypes(GetGameTypeListCriteria criteria);

        List<EntityBase<int, string>> GetGameTableList(GetGameTableListCriteria criteria);
    }
}